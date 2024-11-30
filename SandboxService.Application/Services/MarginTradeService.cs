using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Extensions;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Services;

public class MarginTradeService(
    IBinanceService binanceService,
    UnitOfWork unitOfWork,
    MarginBackgroundService marginBackgroundService)
{
    public async Task<MarginPosition> OpenPosition(OpenMarginPositionRequest request)
    {
        var user = await GetUserById(request.UserId);
        var wallet = GetWallet(user, request.Ticker);

        var entryPrice = (await binanceService.GetPrice(request.Symbol)).Price;
        var requiredMargin = CalculateMargin(request.Amount, entryPrice, request.Leverage);

        if (wallet.Balance < requiredMargin)
            throw new SandboxException("Insufficient margin", SandboxExceptionType.INSUFFICIENT_FUNDS);

        var currency = await unitOfWork.CurrencyRepository.GetByTickerAsync(request.Ticker);

        var position = new MarginPosition
        {
            UserId = request.UserId,
            Currency = currency!,
            Symbol = request.Symbol,
            Amount = request.Amount,
            EntryPrice = entryPrice,
            Leverage = request.Leverage == 0 ? 1 : request.Leverage,
            IsLong = request.IsLong,
            OpenDate = DateTimeOffset.UtcNow,
            TakeProfit = request.TakeProfit,
            StopLoss = request.StopLoss
        };

        await unitOfWork.MarginPositionRepository.InsertAsync(position);

        if (position.TakeProfit != 0)
        {
            var order = OrderExtensions.Create(position.Id, OrderType.TAKE_PROFIT, position.Symbol, entryPrice);
            await unitOfWork.OrderRepository.InsertAsync(order);
            marginBackgroundService.StartTrackingOrder(order.Id, request.Symbol, request.UserId);
        }

        if (position.StopLoss != 0)
        {
            var order = OrderExtensions.Create(position.Id, OrderType.SPOT_LOSS, position.Symbol, entryPrice);
            await unitOfWork.OrderRepository.InsertAsync(order);
            marginBackgroundService.StartTrackingOrder(order.Id, request.Symbol, request.UserId);
        }

        user.MarginPositions.Add(position);
        wallet.Balance -= requiredMargin;

        await unitOfWork.SaveAsync();

        return position;
    }

    public async Task<MarginPosition> ClosePosition(CloseMarginPositionRequest request)
    {
        var user = await GetUserById(request.UserId);
        var position = GetPosition(user, request.PositionId);

        ValidatePosition(position);

        var currentPrice = (await binanceService.GetPrice(position.Symbol)).Price;
        var wallet = GetWallet(user, position.Currency.Ticker);
        var pnl = CalculatePnl(position, currentPrice);

        wallet.Balance += pnl;
        position.IsClosed = true;
        position.CloseDate = DateTime.UtcNow;

        await unitOfWork.SaveAsync();

        marginBackgroundService.StopTrackingOrder(position.Id);

        return position;
    }

    public async Task<MarginPosition> ChangeStopLoss(Guid positionId, decimal value)
    {
        var position = await unitOfWork.MarginPositionRepository.GetByIdAsync(positionId);

        // TODO: null check

        position!.StopLoss = value;

        await unitOfWork.SaveAsync();
        return position;
    }

    public async Task<MarginPosition> ChangeTakeProfit(Guid positionId, decimal value)
    {
        var position = await unitOfWork.MarginPositionRepository.GetByIdAsync(positionId);

        // TODO: null check

        position!.TakeProfit = value;

        await unitOfWork.SaveAsync();
        return position;
    }

    // Utilities 
    private async Task<User> GetUserById(Guid userId)
    {
        return await unitOfWork.UserRepository.GetByIdAsync(userId)
               ?? throw new SandboxException("User not found", SandboxExceptionType.ENTITY_NOT_FOUND);
    }

    private static Account GetWallet(User user, string ticker)
    {
        return user.Wallet.Accounts.FirstOrDefault(w => w.Currency.Ticker == ticker)
               ?? throw new SandboxException("Wallet not found", SandboxExceptionType.WALLET_DOES_NOT_EXIST);
    }

    private static MarginPosition GetPosition(User user, Guid positionId)
    {
        return user.MarginPositions.FirstOrDefault(p => p.Id == positionId)
               ?? throw new SandboxException("Position not found", SandboxExceptionType.POSITION_NOT_FOUND);
    }

    private static void ValidatePosition(MarginPosition position)
    {
        if (position.IsClosed)
            throw new SandboxException("Position already closed", SandboxExceptionType.INVALID_OPERATION);
    }

    private static decimal CalculateMargin(decimal amount, decimal entryPrice, decimal leverage)
    {
        return amount * entryPrice / leverage;
    }

    private static decimal CalculatePnl(MarginPosition position, decimal currentPrice)
    {
        return position.IsLong
            ? (currentPrice - position.EntryPrice) * position.Amount
            : (position.EntryPrice - currentPrice) * position.Amount;
    }
}