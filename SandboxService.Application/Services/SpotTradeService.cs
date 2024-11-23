using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Extensions;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Services;

public class SpotTradeService(IBinanceService binanceService, UnitOfWork unitOfWork)
{
    public async Task<User> Buy(SpotPurchaseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (unitOfWork == null || binanceService == null)
        {
            throw new InvalidOperationException("Dependencies are not properly initialized.");
        }

        var user = await unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new SandboxException("User not found", SandboxExceptionType.RECORD_NOT_FOUND);
        }

        var quoteWallet = user.Wallets.FirstOrDefault(w => w.Currency.Ticker == request.QuoteAsset);
        if (quoteWallet == null)
        {
            throw new SandboxException("Quote asset wallet not found", SandboxExceptionType.WALLET_DOES_NOT_EXIST);
        }

        var priceResponse = await binanceService.GetPrice(request.Symbol);
        if (priceResponse is not { Price: > 0 })
        {
            throw new SandboxException("Invalid symbol price", SandboxExceptionType.WALLET_DOES_NOT_EXIST);
        }

        var price = priceResponse.Price;
        var totalPrice = Math.Round(price * request.Quantity, 8);

        if (quoteWallet.Balance < totalPrice)
        {
            throw new SandboxException("Insufficient funds", SandboxExceptionType.INSUFFICIENT_FUNDS);
        }

        quoteWallet.Balance -= totalPrice;

        var baseWallet = user.Wallets.FirstOrDefault(w => w.Currency.Ticker == request.BaseAsset);
        if (baseWallet != null)
        {
            baseWallet.Balance += request.Quantity;
        }
        else
        {
            var baseCurrency = await unitOfWork.CurrencyRepository.GetByTickerAsync(request.BaseAsset);
            if (baseCurrency == null)
            {
                throw new SandboxException("Base currency not found", SandboxExceptionType.INVALID_ASSET);
            }

            user.Wallets.Add(WalletExtensions.Create(user.Id, baseCurrency.Id, request.Quantity));
        }

        var quoteCurrency = await unitOfWork.CurrencyRepository.GetByTickerAsync(request.QuoteAsset)
                            ?? CurrencyExtensions.Create(request.QuoteAsset, request.QuoteAsset);

        quoteWallet.Transactions.Add(TransactionExtensions.Create(user.Id, totalPrice, quoteCurrency));

        if (baseWallet != null)
        {
            var baseCurrency = await unitOfWork.CurrencyRepository.GetByTickerAsync(request.BaseAsset);
            baseWallet.Transactions.Add(TransactionExtensions.Create(user.Id, request.Quantity, baseCurrency));
        }

        await unitOfWork.SaveAsync();

        return user;
    }


    public async Task<User> Sell(SpotSellRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await unitOfWork.UserRepository.GetByIdAsync(request.UserId);

        var baseWallet = user.Wallets.FirstOrDefault(w => w.Currency.Ticker == request.BaseAsset);
        if (baseWallet is null || baseWallet.Balance < request.Quantity)
        {
            throw new SandboxException(
                "Insufficient balance in base asset wallet",
                SandboxExceptionType.INSUFFICIENT_FUNDS
            );
        }

        var price = (await binanceService.GetPrice(request.Symbol)).Price;
        var totalAmount = Math.Round(price * request.Quantity, 8);

        var quoteWallet = user.Wallets.FirstOrDefault(w => w.Currency.Ticker == request.QuoteAsset);
        if (quoteWallet != null)
        {
            quoteWallet.Balance += totalAmount;
        }
        else
        {
            user.Wallets.Add(
                new Wallet
                {
                    UserId = user.Id,
                    Currency = new Currency { Name = "SomeName", Ticker = request.QuoteAsset },
                    Balance = totalAmount,
                }
            );
        }

        baseWallet.Balance -= request.Quantity;

        var transaction = new Transaction
        {
            Currency = new Currency { Name = "SomeName", Ticker = request.BaseAsset },
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            SenderId = user.Id,
            Amount = -request.Quantity,
        };
        baseWallet.Transactions.Add(transaction);

        return user;
    }
}