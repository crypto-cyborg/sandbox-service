using System.Collections.Concurrent;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Services;

public class MarginBackgroundService(IServiceProvider serviceProvider) : BackgroundService
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
        _activePositions[orderId] = cts;

        Task.Run(() => TrackOrderAsync(orderId, symbol, userId, cts.Token), cts.Token);
    }

    // public void StopTrackingPosition(Guid positionId)
    // {
    //     if (_activePositions.TryRemove(positionId, out var cts))
    //     {
    //         cts.Cancel();
    //         Console.WriteLine($"Stopped tracking position {positionId}.");
    //     }
    // }

    public void StopTrackingOrder(Guid orderId)
    {
        if (_activePositions.TryRemove(orderId, out var cts))
        {
            cts.Cancel();
            Console.WriteLine($"Stopped tracking order {orderId}.");
        }
    }

    private async Task TrackOrderAsync(Guid orderId, string symbol, Guid userId,
        CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var binanceService = scope.ServiceProvider.GetRequiredService<IBinanceService>();

            await binanceService.ConnectToTickerStream(symbol, async currentPrice =>
            {
                using var innerScope = serviceProvider.CreateScope();
                var unitOfWork = innerScope.ServiceProvider.GetRequiredService<UnitOfWork>();

                await CheckOrder(unitOfWork, userId, orderId, currentPrice);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error tracking position {orderId}: {ex.Message}");
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

        if (order is null) return;

        if (order.Amount < currentPrice || order.Amount >= currentPrice)
        {
            var position = order.Position;
            var marginUsed = CalculateMargin(position.Amount, position.EntryPrice, position.Leverage);
            var account = user?.Wallet.Accounts.FirstOrDefault(w => w.Currency.Ticker == position.Currency.Ticker);
            var pnl = CalculatePnl(position, currentPrice);

            if (account.Balance + pnl < marginUsed)
            {
                position.IsClosed = true;
                position.CloseDate = DateTime.UtcNow;
                account.Balance += pnl;

                order.CompletedAt = DateTimeOffset.UtcNow;
                order.Status = OrderStatus.COMPLETED;

                var orders =
                    await unitOfWork.OrderRepository.GetAsync(o => o.PositionId == position.Id && o.Id != order.Id);
                foreach (var item in orders)
                {
                    item.Status = OrderStatus.CANCELED;
                    item.CompletedAt = DateTimeOffset.UtcNow;
                    StopTrackingOrder(item.Id);
                }

                Console.WriteLine($"Position {position.Id} liquidated.");
                await unitOfWork.SaveAsync();
                StopTrackingOrder(orderId);
                return;
            }

            if ((position.TakeProfit != 0 &&
                 ((position.IsLong && currentPrice >= position.TakeProfit.Value) ||
                  (!position.IsLong && currentPrice <= position.TakeProfit.Value))) ||
                (position.StopLoss != 0 &&
                 ((position.IsLong && currentPrice <= position.StopLoss.Value) ||
                  (!position.IsLong && currentPrice >= position.StopLoss.Value))))
            {
                position.IsClosed = true;
                position.CloseDate = DateTime.UtcNow;
                account.Balance += pnl;
                
                order.CompletedAt = DateTimeOffset.UtcNow;
                order.Status = OrderStatus.COMPLETED;

                var orders =
                    await unitOfWork.OrderRepository.GetAsync(o => o.PositionId == position.Id && o.Id != order.Id);
                foreach (var item in orders)
                {
                    item.Status = OrderStatus.CANCELED;
                    item.CompletedAt = DateTimeOffset.UtcNow;
                    StopTrackingOrder(item.Id);
                }

                Console.WriteLine($"Position {position.Id} closed (TP/SL triggered).");
                await unitOfWork.SaveAsync();
                StopTrackingOrder(orderId);
            }
        }
    }

    // private async Task CheckLiquidation(UnitOfWork unitOfWork, Guid userId, Guid positionId, decimal currentPrice)
    // {
    //     var user = await unitOfWork.UserRepository.GetByIdAsync(userId);
    //     var position = user?.MarginPositions.FirstOrDefault(mp => mp.Id == positionId);
    //
    //     if (position == null)
    //         return;
    //
    //     var marginUsed = CalculateMargin(position.Amount, position.EntryPrice, position.Leverage);
    //     var account = user.Wallet.Accounts.FirstOrDefault(w => w.Currency.Ticker == position.Currency.Ticker);
    //
    //     var pnl = CalculatePnl(position, currentPrice);
    //
    //     if (account.Balance + pnl < marginUsed)
    //     {
    //         position.IsClosed = true;
    //         position.CloseDate = DateTime.UtcNow;
    //         account.Balance += pnl;
    //
    //         Console.WriteLine($"Position {position.Id} liquidated.");
    //         await unitOfWork.SaveAsync();
    //         StopTrackingPosition(positionId);
    //         return;
    //     }
    //
    //     if ((position.TakeProfit != 0 &&
    //          ((position.IsLong && currentPrice >= position.TakeProfit.Value) ||
    //           (!position.IsLong && currentPrice <= position.TakeProfit.Value))) ||
    //         (position.StopLoss != 0 &&
    //          ((position.IsLong && currentPrice <= position.StopLoss.Value) ||
    //           (!position.IsLong && currentPrice >= position.StopLoss.Value))))
    //     {
    //         position.IsClosed = true;
    //         position.CloseDate = DateTime.UtcNow;
    //         account.Balance += pnl;
    //
    //         Console.WriteLine($"Position {position.Id} closed (TP/SL triggered).");
    //         await unitOfWork.SaveAsync();
    //         StopTrackingPosition(positionId);
    //     }
    // }

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