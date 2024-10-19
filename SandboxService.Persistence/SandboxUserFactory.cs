using System;
using SandboxService.Core.Models;

namespace SandboxService.Persistence;

public static class SandboxUserFactory
{
    public static UserData Create(Guid userId, string apiKey, string secretKey)
    {
        var user = new UserData
        {
            UserId = userId,
            ApiKey = apiKey,
            SecretKey = secretKey,
        };

        user.Wallets.Add(
            new Wallet
            {
                UserId = user.UserId,
                Currency = new Currency { Name = "Tether", Code = "USDT" },
                Balance = 10000,
            }
        );

        return user;
    }
}
