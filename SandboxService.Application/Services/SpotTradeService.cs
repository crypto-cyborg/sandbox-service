using LanguageExt.Common;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Extensions;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Services;

public class SpotTradeService(IBinanceService binanceService, UnitOfWork unitOfWork)
{
    public async Task<UserExtensions.UserReadDto> Buy(SpotPurchaseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        await using var transaction = await unitOfWork.BeginTransactionAsync();

        var user = await unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            throw new SandboxException("User not found", SandboxExceptionType.RECORD_NOT_FOUND);
        }

        var quoteAccount = user.Wallet.Accounts.FirstOrDefault(w => w.Currency.Ticker == request.QuoteAsset);
        if (quoteAccount is null)
        {
            throw new SandboxException("Quote asset wallet not found", SandboxExceptionType.WALLET_DOES_NOT_EXIST);
        }

        var priceResponse = await binanceService.GetPrice(request.Symbol);
        if (priceResponse is not { Price: > 0 })
        {
            throw new SandboxException("Invalid symbol price", SandboxExceptionType.INVALID_ASSET);
        }

        var price = priceResponse.Price;
        var totalPrice = price * request.Quantity;

        if (quoteAccount.Balance < totalPrice)
        {
            throw new SandboxException("Insufficient funds", SandboxExceptionType.INSUFFICIENT_FUNDS);
        }

        quoteAccount.Balance -= totalPrice;

        var baseAccount = user.Wallet.Accounts.FirstOrDefault(w => w.Currency.Ticker == request.BaseAsset);
        if (baseAccount is null)
        {
            var baseCurrency = await unitOfWork.CurrencyRepository.GetByTickerAsync(request.BaseAsset);
            if (baseCurrency is null)
            {
                throw new SandboxException("Base currency not found", SandboxExceptionType.INVALID_ASSET);
            }

            baseAccount = AccountExtensions.Create(user.Id, baseCurrency.Id, 0);

            await unitOfWork.AccountRepository.InsertAsync(baseAccount);

            user.Wallet.Accounts.Add(baseAccount);
        }

        baseAccount.Balance += request.Quantity;

        var quoteCurrency = await unitOfWork.CurrencyRepository.GetByTickerAsync(request.QuoteAsset);
        if (quoteCurrency is null)
        {
            throw new SandboxException("Quote currency not found", SandboxExceptionType.INVALID_ASSET);
        }

        var quoteTransaction =
            TransactionExtensions.Create(quoteAccount.Id, baseAccount.Id, user.Wallet.Id, totalPrice, quoteCurrency.Id);
        var baseTransaction = TransactionExtensions.Create(baseAccount.Id, quoteAccount.Id, user.Wallet.Id,
            request.Quantity,
            baseAccount.CurrencyId);

        await unitOfWork.TransactionRepository.InsertAsync(quoteTransaction);
        await unitOfWork.TransactionRepository.InsertAsync(baseTransaction);

        user.Wallet.Transactions.Add(quoteTransaction);
        user.Wallet.Transactions.Add(baseTransaction);

        unitOfWork.UserRepository.Update(user);

        await unitOfWork.SaveAsync();

        await transaction.CommitAsync();

        return user.MapToResponse();
    }

    public async Task<UserExtensions.UserReadDto> Sell(SpotSaleRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        await using var transaction = await unitOfWork.BeginTransactionAsync();

        var user = await unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            throw new SandboxException("User not found", SandboxExceptionType.RECORD_NOT_FOUND);
        }

        var baseAccount = user.Wallet.Accounts.FirstOrDefault(w => w.Currency.Ticker == request.BaseAsset);
        if (baseAccount is null)
        {
            throw new SandboxException("Base asset wallet not found", SandboxExceptionType.WALLET_DOES_NOT_EXIST);
        }

        var priceResponse = await binanceService.GetPrice(request.Symbol);
        if (priceResponse is not { Price: > 0 })
        {
            throw new SandboxException("Invalid symbol price", SandboxExceptionType.INVALID_ASSET);
        }

        var price = priceResponse.Price;
        var totalPrice = price * request.Quantity;

        if (baseAccount.Balance < request.Quantity)
        {
            throw new SandboxException("Insufficient balance", SandboxExceptionType.INSUFFICIENT_FUNDS);
        }

        baseAccount.Balance -= request.Quantity;

        var quoteAccount = user.Wallet.Accounts.FirstOrDefault(w => w.Currency.Ticker == request.QuoteAsset);
        if (quoteAccount is null)
        {
            var quoteCurrencyDb =
                await unitOfWork.CurrencyRepository
                    .GetByTickerAsync(request.QuoteAsset);
            if (quoteCurrencyDb is null)
            {
                throw new SandboxException("Quote currency not found", SandboxExceptionType.INVALID_ASSET);
            }

            quoteAccount = AccountExtensions.Create(user.Id, quoteCurrencyDb.Id, 0);

            await unitOfWork.AccountRepository.InsertAsync(quoteAccount);

            user.Wallet.Accounts.Add(quoteAccount);
        }

        quoteAccount.Balance += totalPrice;

        var baseTransaction = TransactionExtensions.Create(baseAccount.Id, quoteAccount.Id, user.Wallet.Id,
            request.Quantity, baseAccount.CurrencyId);
        var quoteTransaction = TransactionExtensions.Create(quoteAccount.Id, baseAccount.Id, user.Wallet.Id, totalPrice,
            quoteAccount.CurrencyId);

        await unitOfWork.TransactionRepository.InsertAsync(baseTransaction);
        await unitOfWork.TransactionRepository.InsertAsync(quoteTransaction);

        user.Wallet.Transactions.Add(baseTransaction);
        user.Wallet.Transactions.Add(quoteTransaction);

        unitOfWork.UserRepository.Update(user);

        await unitOfWork.SaveAsync();

        await transaction.CommitAsync();

        return user.MapToResponse();
    }
}