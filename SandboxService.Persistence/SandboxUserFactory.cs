using System;
using SandboxService.Core.Models;

namespace SandboxService.Persistence;

public static class SandboxUserFactory
{
    public static User Create(Guid userId, string apiKey, string secretKey)
    {
        var user = new User
        {
            Id = userId,
            ApiKey = apiKey,
            SecretKey = secretKey,
        };

        user.Wallets.Add(
            new Wallet
            {
                UserId = user.Id,
                Currency = new Currency { Name = "Tether", Ticker = "USDT" },
                Balance = 10000,
            }
        );

        return user;
    }
}
