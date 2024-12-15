using SandboxService.Application.Utilities;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Interfaces.Services;
using SandboxService.Core.Models;
using SandboxService.Persistence;
using SandboxService.Shared.Dtos;

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
        var requiredMargin = MarginUtilities.CalculateMargin(request.Amount, entryPrice, request.Leverage);

        if (wallet.Balance < requiredMargin)
            throw new SandboxException("Insufficient margin", SandboxExceptionType.INSUFFICIENT_FUNDS);

        var currency = await unitOfWork.CurrencyRepository.GetByTickerAsync(request.Ticker);

        var position = new MarginPosition
        {
            UserId = request.UserId,
            Currency = currency!,
            Symbol = request.Symbol,
            PositionAmount = request.Amount,
            EntryPrice = entryPrice,
            Leverage = request.Leverage,
            IsLong = request.IsLong,
            OpenDate = DateTimeOffset.UtcNow,
        };

        await unitOfWork.MarginPositionRepository.InsertAsync(position);

        user.MarginPositions.Add(position);
        wallet.Balance -= requiredMargin;

        await unitOfWork.SaveAsync();

        return position;
    }

    public async Task<MarginPosition> ClosePosition(CloseMarginPositionRequest request)
    {
        var user = await GetUserById(request.UserId);
        var position = GetPosition(user, request.PositionId);

        MarginUtilities.ValidatePosition(position);

        var currentPrice = (await binanceService.GetPrice(position.Symbol)).Price;
        var wallet = GetWallet(user, position.Currency.Ticker);
        var pnl = MarginUtilities.CalculatePnl(position, currentPrice);

        wallet.Balance += pnl;
        position.IsClosed = true;
        position.CloseDate = DateTime.UtcNow;

        var relatedOrders = await unitOfWork.OrderRepository.GetAsync(o => o.PositionId == position.Id);
        foreach (var order in relatedOrders)
        {
            order.Status = OrderStatus.CANCELED;
            order.CompletedAt = DateTimeOffset.UtcNow;
            marginBackgroundService.StopTrackingOrder(order.Id);
        }

        await unitOfWork.SaveAsync();

        marginBackgroundService.StopTrackingOrder(position.Id);

        return position;
    }

    public async Task<Order> ChangeStopLoss(Guid orderId, decimal value)
    {
        var order = await unitOfWork.OrderRepository.GetByIdAsync(orderId);

        // TODO: null check

        order!.Price = value;

        await unitOfWork.SaveAsync();
        return order;
    }

    public async Task<Order> ChangeTakeProfit(Guid orderId, decimal value)
    {
        var order = await unitOfWork.OrderRepository.GetByIdAsync(orderId);

        // TODO: null check

        order!.Price = value;

        await unitOfWork.SaveAsync();
        return order;
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
}