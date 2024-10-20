using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Interfaces;
using SandboxService.Core.Models;

namespace SandboxService.Application;

public class MarginTradeService
{
    private readonly ICacheService _cacheService;
    private readonly IBinanceService _binanceService;

    public MarginTradeService(ICacheService cacheService, IBinanceService binanceService)
    {
        _cacheService = cacheService;
        _binanceService = binanceService;
    }

    public async Task<MarginPosition> OpenPosition(OpenMarginPositionRequest request)
    {
        var user = _cacheService.Get(request.UserId);
        var entryPrice = (await _binanceService.GetPrice(request.Symbol)).Price;

        var wallet =
            user.Wallets.FirstOrDefault(w => w.Currency == request.Currency)
            ?? throw new SandboxException(
                "Required wallet does not exist",
                SandboxExceptionType.WALLET_DOES_NOT_EXIST
            );

        decimal requiredMargin = request.Amount * entryPrice / request.Leverage;

        if (wallet.Balance < requiredMargin)
            throw new SandboxException(
                "Insufficient margin",
                SandboxExceptionType.INSUFFICIENT_FUNDS
            );

        var position = new MarginPosition
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Currency = request.Currency,
            Amount = request.Amount,
            EntryPrice = entryPrice,
            IsLong = request.IsLong,
            Leverage = request.Leverage,
            OpenDate = DateTime.UtcNow,
        };

        user.MarginPositions.Add(position);
        wallet.Balance -= requiredMargin;

        return position;
    }

    // public async Task ClosePositionAsync(Guid positionId, decimal currentPrice)
    // {
    //     var position = _positionRepository.Get().FirstOrDefault(p => p.Id == positionId);

    //     if (position == null || position.IsClosed)
    //         throw new InvalidOperationException("Позиция не найдена или уже закрыта.");

    //     var wallet = _walletRepository
    //         .Get()
    //         .FirstOrDefault(w =>
    //             w.UserId == position.UserId && w.CurrencyId == position.CurrencyId
    //         );

    //     decimal pnl;
    //     if (position.IsLong)
    //         pnl = (currentPrice - position.EntryPrice) * position.Amount;
    //     else
    //         pnl = (position.EntryPrice - currentPrice) * position.Amount;

    //     wallet.Value += pnl; // Обновляем баланс пользователя
    //     position.IsClosed = true;
    //     position.CloseDate = DateTime.UtcNow;

    //     await _walletRepository.SaveChangesAsync();
    // }

    // public async Task CheckLiquidationAsync(Guid userId, int currencyId, decimal currentPrice)
    // {
    //     var positions = _positionRepository
    //         .Get()
    //         .Where(p => p.UserId == userId && p.CurrencyId == currencyId && !p.IsClosed);

    //     foreach (var position in positions)
    //     {
    //         decimal marginUsed = (position.Amount * position.EntryPrice) / position.Leverage;
    //         var wallet = _walletRepository
    //             .Get()
    //             .FirstOrDefault(w => w.UserId == userId && w.CurrencyId == currencyId);

    //         decimal pnl = position.IsLong
    //             ? (currentPrice - position.EntryPrice) * position.Amount
    //             : (position.EntryPrice - currentPrice) * position.Amount;

    //         // Ликвидация, если PnL + маржа меньше нуля
    //         if (wallet.Value + pnl < marginUsed)
    //         {
    //             position.IsClosed = true;
    //             position.CloseDate = DateTime.UtcNow;
    //             wallet.Value += pnl;

    //             await _walletRepository.SaveChangesAsync();
    //         }
    //     }
    // }
}
