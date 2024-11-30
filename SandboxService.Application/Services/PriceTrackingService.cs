using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Services;

public class PriceTrackingService(
    IBinanceService binanceService,
    IServiceProvider serviceProvider,
    ILogger<PriceTrackingService> logger)
    : BackgroundService
{
    private readonly Dictionary<Guid, CancellationTokenSource> _trackingTasks = new();

    public void StartTracking(string symbol, Guid positionId, Guid userId)
    {
        if (_trackingTasks.ContainsKey(positionId))
        {
            logger.LogWarning($"Tracking for position {positionId} is already running.");
            return;
        }

        var cts = new CancellationTokenSource();
        _trackingTasks[positionId] = cts;

        _ = Task.Run(() => TrackPriceAsync(symbol, positionId, userId, cts.Token), cts.Token);
    }

    public void StopTracking(Guid positionId)
    {
        if (!_trackingTasks.TryGetValue(positionId, out var cts)) return;
        
        cts.Cancel();
        _trackingTasks.Remove(positionId);
        logger.LogInformation($"Stopped tracking for position {positionId}.");
    }

    private async Task TrackPriceAsync(string symbol, Guid positionId, Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            await binanceService.ConnectToTickerStream(symbol, async currentPrice =>
            {
                using var scope = serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
                var user = await unitOfWork.UserRepository.GetByIdAsync(userId);
                var position = user?.MarginPositions.FirstOrDefault(mp => mp.Id == positionId);

                if (position == null || position.IsClosed)
                {
                    StopTracking(positionId);
                    return;
                }

                await CheckLiquidation(unitOfWork, position, user, currentPrice);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error tracking price for position {positionId}.");
        }
        finally
        {
            StopTracking(positionId);
        }
    }

    private async Task CheckLiquidation(UnitOfWork unitOfWork, MarginPosition position, User user, decimal currentPrice)
    {
        if (position.IsClosed) return;

        var marginUsed = MarginTradeService.CalculateMargin(position.Amount, position.EntryPrice, position.Leverage);
        var wallet = user.Wallet.Accounts.FirstOrDefault(a => a.Currency.Ticker == position.Currency.Ticker);
        var pnl = MarginTradeService.CalculatePnl(position, currentPrice);

        if (wallet == null) return;

        if (wallet.Balance + pnl < marginUsed)
        {
            position.IsClosed = true;
            position.CloseDate = DateTimeOffset.UtcNow;
            wallet.Balance += pnl;

            await unitOfWork.SaveAsync();
            logger.LogInformation($"Position {position.Id} liquidated.");
        }
        else if (position.TakeProfit.HasValue && pnl >= position.TakeProfit.Value ||
                 position.StopLoss.HasValue && pnl <= position.StopLoss.Value)
        {
            position.IsClosed = true;
            position.CloseDate = DateTimeOffset.UtcNow;
            wallet.Balance += pnl;

            await unitOfWork.SaveAsync();
            logger.LogInformation($"Position {position.Id} closed due to TP/SL.");
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("PriceTrackingService is running.");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        foreach (var cts in _trackingTasks.Values)
        {
            cts.Cancel();
        }

        _trackingTasks.Clear();
        logger.LogInformation("PriceTrackingService stopped.");
        await base.StopAsync(stoppingToken);
    }
}
