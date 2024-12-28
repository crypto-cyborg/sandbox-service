using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SandboxService.Core.Interfaces.Services;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Services;

public class MarginBackgroundService(IServiceProvider serviceProvider, ILogger<MarginBackgroundService> logger)
    : BackgroundService
{
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _activeOrders = new();

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
        if (!_activeOrders.TryAdd(orderId, cts))
        {
            logger.LogWarning("Order {OrderId} is already being tracked.", orderId);
            return;
        }

        logger.LogInformation("Started tracking order {OrderId}.", orderId);

        Task.Run(() => TrackOrderAsync(orderId, symbol, userId, cts.Token), cts.Token);
    }

    public void StopTrackingOrder(Guid orderId)
    {
        if (_activeOrders.TryRemove(orderId, out var cts))
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
                var positionService = innerScope.ServiceProvider.GetRequiredService<IPositionService>();
                var orderService = innerScope.ServiceProvider.GetRequiredService<OrderService>();

                if (cancellationToken.IsCancellationRequested) return;

                await CheckOrder(unitOfWork, positionService, orderService, userId, orderId, currentPrice);
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

    private async Task CheckOrder(
        UnitOfWork unitOfWork,
        IPositionService positionService,
        OrderService orderService,
        Guid userId,
        Guid orderId,
        decimal currentPrice)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(userId);
        var order = await unitOfWork.OrderRepository.GetByIdAsync(orderId);

        if (order is null)
        {
            logger.LogWarning("Order {OrderId} not found.", orderId);
            StopTrackingOrder(orderId);
            return;
        }

        switch (order.Type)
        {
            case OrderType.STOP_LOSS:
            {
                if ((order.IsLong && currentPrice <= order.Price) ||
                    (!order.IsLong && currentPrice >= order.Price))
                {
                    await positionService.Close(order.PositionId);
                    await orderService.Close(orderId, OrderStatus.COMPLETED);
                    logger.LogInformation("Stop Loss triggered for Order {OrderId}.", orderId);
                    StopTrackingOrder(orderId);
                }

                break;
            }

            case OrderType.TAKE_PROFIT:
            {
                if ((order.IsLong && currentPrice >= order.Price) ||
                    (!order.IsLong && currentPrice <= order.Price))
                {
                    await positionService.Close(order.PositionId);
                    await orderService.Close(orderId, OrderStatus.COMPLETED);
                    logger.LogInformation("Take Profit triggered for Order {OrderId}.", orderId);
                    StopTrackingOrder(orderId);
                }

                break;
            }

            case OrderType.LIMIT:
            {
                if (order.Price < currentPrice)
                {
                    var pos = await positionService.Open(order);
                    await orderService.Close(orderId, OrderStatus.COMPLETED);
                    StopTrackingOrder(orderId);
                }

                break;
            }

            case OrderType.MARKET:
            default:
                throw new ArgumentOutOfRangeException($"Cannot handle value: ${order.Type}");
        }
    }
}