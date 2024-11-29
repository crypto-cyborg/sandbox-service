using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.ServiceClients;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Extensions;
using SandboxService.Core.Models;
using SandboxService.Persistence;
using static SandboxService.Core.Extensions.UserExtensions;

namespace SandboxService.Application.Services;

public class AccountService(HttpClient httpClient, UnitOfWork unitOfWork, UserServiceClient usc) : IAccountService
{
    public async Task<UserExtensions.UserReadDto> CreateSandboxUser(SanboxInitializeRequest request)
    {
        var user = await EnsureExists(request.UserId);

        if (user is not null) return user.MapToResponse();

        var userResponse = await usc.GetUser(request.UserId);

        var userData = await CreateUser(userResponse);

        await unitOfWork.UserRepository.InsertAsync(userData);
        await unitOfWork.SaveAsync();

        return userData.MapToResponse();
    }

    private async Task<User?> EnsureExists(Guid userId)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(userId);

        return user;
    }

    private async Task<bool> ValidateKeys(string apiKey, string secretKey)
    {
        try
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var query = $"timestamp={timestamp}";
            var signature = Sign(query, secretKey);

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"account?{query}&signature={signature}"
            );

            request.Headers.Add("X-MBX-APIKEY", apiKey);

            var response = await httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    [Pure]
    private static string Sign(string query, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(query));

        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    private async Task<User> CreateUser(GetUserResponse response)
    {
        var initialCurrency = await unitOfWork.CurrencyRepository.GetByTickerAsync("USDT");

        if (initialCurrency is null)
        {
            await unitOfWork.CurrencyRepository.InsertAsync(new Currency { Name = "Tether", Ticker = "USDT" });
            await unitOfWork.SaveAsync();
            initialCurrency = await unitOfWork.CurrencyRepository.GetByTickerAsync("USDT");

            if (initialCurrency is null)
            {
                throw new SandboxException("Failed to initialize default currency",
                    SandboxExceptionType.CURRENCY_NOT_FOUND);
            }
        }

        var wallet = WalletExtensions.Create(response.Id);
        var initialAccount = AccountExtensions.Create(wallet.Id, initialCurrency.Id, 10000);
        wallet.Accounts.Add(initialAccount);

        var user = new User
        {
            Id = response.Id,
            ApiKey = response.ApiKey,
            SecretKey = response.SecretKey,
            WalletId = wallet.Id,
            Wallet = wallet
        };

        await unitOfWork.SaveAsync();

        return user;
    }
}