using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Interfaces;
using SandboxService.Core.Models;

namespace SandboxService.Application.Services;

public class AccountService : IAccountService
{
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public AccountService(IMapper mapper, ICacheService cacheService)
    {
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<UserData> CreateSandboxUser(SanboxInitializeRequest request)
    {
        // TODO: Validate userId
        // TODO: Validate api, secret keys

        var validation = await ValidateKeys(request.ApiKey, request.SecretKey);

        if (!validation)
        {
            throw new SandboxException("Keys are not valid", SandboxExceptionType.INVALID_KEYS);
        }

        var userData = _mapper.Map<UserData>(request);

        _cacheService.Set(userData.UserId, userData);

        return userData;
    }

    private async Task<bool> ValidateKeys(string apiKey, string secretKey)
    {
        try
        {
            var httpClient = new HttpClient();
            var baseUrl = "https://api.binance.com/api/v3/";
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

    private string Sign(string query, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(query));

        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
