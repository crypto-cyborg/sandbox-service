using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Extensions;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Services;

public class AccountService(IConfiguration configuration, UnitOfWork unitOfWork) : IAccountService
{
    public async Task<User> CreateSandboxUser(SanboxInitializeRequest request)
    {
        // TODO: Validate userId

        var validation = await ValidateKeys(request.ApiKey, request.SecretKey);

        if (!validation)
        {
            throw new SandboxException("Keys are not valid", SandboxExceptionType.INVALID_KEYS);
        }

        var userData = await CreateUser(request.UserId, request.ApiKey, request.SecretKey);

        await unitOfWork.UserRepository.InsertAsync(userData);
        var rows = await unitOfWork.SaveAsync();
        Console.WriteLine(rows);

        return userData;
    }

    private async Task<bool> ValidateKeys(string apiKey, string secretKey)
    {
        try
        {
            var httpClient = new HttpClient();
            var baseUrl = configuration.GetSection("Binance:BaseUrl").Value;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var query = $"timestamp={timestamp}";
            var signature = Sign(query, secretKey);

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{baseUrl}account?{query}&signature={signature}"
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
    
    private async Task<User> CreateUser(Guid userId, string apiKey, string secretKey)
    {
        var user = new User
        {
            Id = userId,
            ApiKey = apiKey,
            SecretKey = secretKey
        };

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

        var wallet = WalletExtensions.Create(user.Id, initialCurrency.Id, 10000);
        user.Wallets.Add(wallet);

        return user;
    }
}
