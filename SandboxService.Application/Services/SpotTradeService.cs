using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Interfaces;
using SandboxService.Core.Models;

namespace SandboxService.Application.Services;

public class SpotTradeService
{
    private readonly ICacheService _cacheService;
    private readonly IBinanceService _binanceService;

    public SpotTradeService(ICacheService cacheService, IBinanceService binanceService)
    {
        _cacheService = cacheService;
        _binanceService = binanceService;
    }

    public async Task<UserData> Buy(SpotPurchaseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = _cacheService.Get(request.UserId);

        var quoteWallet = user.Wallets.FirstOrDefault(w => w.Currency.Code == request.QuoteAsset);
        if (quoteWallet == null)
        {
            throw new SandboxException(
                "Quote asset wallet not found",
                SandboxExceptionType.INVALID_ASSET
            );
        }

        var price = (await _binanceService.GetPrice(request.Symbol)).Price;
        var totalPrice = Math.Round(price * request.Quantity, 8);

        if (quoteWallet.Balance < totalPrice)
        {
            throw new SandboxException(
                "Insufficient funds",
                SandboxExceptionType.INSUFFICIENT_FUNDS
            );
        }

        quoteWallet.Balance -= totalPrice;

        var baseWallet = user.Wallets.FirstOrDefault(w => w.Currency.Code == request.BaseAsset);
        if (baseWallet != null)
        {
            baseWallet.Balance += request.Quantity;
        }
        else
        {
            user.Wallets.Add(
                new Wallet
                {
                    UserId = user.UserId,
                    Currency = new Currency { Name = "SomeName", Code = request.BaseAsset },
                    Balance = request.Quantity,
                }
            );
        }

        var transaction = new Transaction
        {
            Currency = new Currency { Name = "Tether", Code = request.QuoteAsset },
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            SenderId = user.UserId,
            Amount = totalPrice,
        };
        quoteWallet.Transactions.Add(transaction);

        return user;
    }

    public async Task<UserData> Sell(SpotSellRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = _cacheService.Get(request.UserId);

        var baseWallet = user.Wallets.FirstOrDefault(w => w.Currency.Code == request.BaseAsset);
        if (baseWallet == null || baseWallet.Balance < request.Quantity)
        {
            throw new SandboxException(
                "Insufficient balance in base asset wallet",
                SandboxExceptionType.INSUFFICIENT_FUNDS
            );
        }

        var price = (await _binanceService.GetPrice(request.Symbol)).Price;
        var totalAmount = Math.Round(price * request.Quantity, 8);

        var quoteWallet = user.Wallets.FirstOrDefault(w => w.Currency.Code == request.QuoteAsset);
        if (quoteWallet != null)
        {
            quoteWallet.Balance += totalAmount;
        }
        else
        {
            user.Wallets.Add(
                new Wallet
                {
                    UserId = user.UserId,
                    Currency = new Currency { Name = "SomeName", Code = request.QuoteAsset },
                    Balance = totalAmount,
                }
            );
        }

        baseWallet.Balance -= request.Quantity;

        var transaction = new Transaction
        {
            Currency = new Currency { Name = "SomeName", Code = request.BaseAsset },
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            SenderId = user.UserId,
            Amount = -request.Quantity,
        };
        baseWallet.Transactions.Add(transaction);

        return user;
    }
}
