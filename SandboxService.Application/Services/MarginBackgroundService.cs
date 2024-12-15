using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Services;

public class MarginBackgroundService(IServiceProvider serviceProvider, ILogger<MarginBackgroundService> logger)
    : BackgroundService
{
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _activePositions = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public void StartTrackingOrder(Guid orderId, string symbol, Guid userId)
    {
        var cts = new CancellationTokenSource();
        if (!_activePositions.TryAdd(orderId, cts))
        {
            logger.LogWarning("Order {OrderId} is already being tracked.", orderId);
            return;
        }

        logger.LogInformation("Started tracking order {OrderId}.", orderId);

        Task.Run(() => TrackOrderAsync(orderId, symbol, userId, cts.Token), cts.Token);
    }

    public void StopTrackingOrder(Guid orderId)
    {
        if (_activePositions.TryRemove(orderId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
            logger.LogInformation("Stopped tracking order {OrderId}.", orderId);
        }
        else
        {
            logger.LogWarning("Order {OrderId} was not being tracked.", orderId);
        }
    }

    private async Task TrackOrderAsync(Guid orderId, string symbol, Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var binanceService = scope.ServiceProvider.GetRequiredService<IBinanceService>();

            await binanceService.ConnectToTickerStream(symbol, async currentPrice =>
            {
                using var innerScope = serviceProvider.CreateScope();
                var unitOfWork = innerScope.ServiceProvider.GetRequiredService<UnitOfWork>();

                if (cancellationToken.IsCancellationRequested)
                    return;

                await CheckOrder(unitOfWork, userId, orderId, currentPrice);
            }, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Tracking for order {OrderId} was cancelled.", orderId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error tracking order {OrderId}.", orderId);
        }
        finally
        {
            StopTrackingOrder(orderId);
        }
    }

    private async Task CheckOrder(UnitOfWork unitOfWork, Guid userId, Guid orderId, decimal currentPrice)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(userId);
        var order = await unitOfWork.OrderRepository.GetByIdAsync(orderId);

        if (order == null)
        {
            logger.LogWarning("Order {OrderId} not found.", orderId);
            StopTrackingOrder(orderId);
            return;
        }

        var position = order.Position;
        var pnl = CalculatePnl(position, currentPrice);
        var account = user?.Wallet.Accounts.FirstOrDefault(w => w.Currency.Ticker == position.Currency.Ticker);

        if (account == null)
        {
            logger.LogWarning("Account for user {UserId} not found.", userId);
            StopTrackingOrder(orderId);
            return;
        }

        var marginUsed = CalculateMargin(position.Amount, position.EntryPrice, position.Leverage);

        if (account.Balance + pnl < marginUsed)
        {
            await HandlePositionClosure(unitOfWork, position, order, pnl, account, "liquidation");
            return;
        }

        if ((order.Price != 0 &&
             ((order.IsLong && currentPrice >= order.Price) ||
              (!order.IsLong && currentPrice <= order.Price))) ||
            (order.Price != 0 &&
             ((order.IsLong && currentPrice <= order.Price) ||
              (!order.IsLong && currentPrice >= order.Price))))
        {
            await HandlePositionClosure(unitOfWork, position, order, pnl, account, "TP/SL triggered");
        }
    }

    private async Task HandlePositionClosure(UnitOfWork unitOfWork, MarginPosition position, Order order, decimal pnl,
        Account account, string reason)
    {
        position.IsClosed = true;
        position.CloseDate = DateTime.UtcNow;
        account.Balance += pnl;

        order.CompletedAt = DateTimeOffset.UtcNow;
        order.Status = OrderStatus.COMPLETED;

        var relatedOrders =
            await unitOfWork.OrderRepository.GetAsync(o => o.PositionId == position.Id && o.Id != order.Id);
        foreach (var relatedOrder in relatedOrders)
        {
            relatedOrder.Status = OrderStatus.CANCELED;
            relatedOrder.CompletedAt = DateTimeOffset.UtcNow;
            StopTrackingOrder(relatedOrder.Id);
        }

        logger.LogInformation("Position {PositionId} closed due to {Reason}.", position.Id, reason);

        await unitOfWork.SaveAsync();
        StopTrackingOrder(order.Id);
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