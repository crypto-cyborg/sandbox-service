using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Models;

namespace SandboxService.Application;

// public class MarginTradeService
// {
//     private readonly IBinanceService _binanceService;
//
//     public MarginTradeService(IBinanceService binanceService)
//     {
//         _binanceService = binanceService;
//     }
//
//     public async Task<MarginPosition> OpenPosition(OpenMarginPositionRequest request)
//     {
//         var user = _cacheService.Get(request.UserId);
//         var entryPrice = (await _binanceService.GetPrice(request.Symbol)).Price;
//
//         var wallet =
//             user.Wallets.FirstOrDefault(w => w.Currency == request.Currency)
//             ?? throw new SandboxException(
//                 "Required wallet does not exist",
//                 SandboxExceptionType.WALLET_DOES_NOT_EXIST
//             );
//
//         decimal requiredMargin = request.Amount * entryPrice / request.Leverage;
//
//         if (wallet.Balance < requiredMargin)
//             throw new SandboxException(
//                 "Insufficient margin",
//                 SandboxExceptionType.INSUFFICIENT_FUNDS
//             );
//
//         var position = new MarginPosition
//         {
//             UserId = request.UserId,
//             Currency = request.Currency,
//             Symbol = request.Symbol,
//             Amount = request.Amount,
//             EntryPrice = entryPrice,
//             IsLong = request.IsLong,
//             Leverage = request.Leverage,
//         };
//
//         user.MarginPositions.Add(position);
//         wallet.Balance -= requiredMargin;
//
//         return position;
//     }
//
//     public async Task<MarginPosition> ClosePosition(CloseMarginPositionRequest request)
//     {
//         var user = _cacheService.Get(request.UserId);
//         var position = user.MarginPositions.FirstOrDefault(p => p.Id == request.PositionId);
//         var currentPrice = (await _binanceService.GetPrice(position.Symbol)).Price;
//
//         if (position == null || position.IsClosed)
//             throw new InvalidOperationException("Позиция не найдена или уже закрыта.");
//
//         var wallet = user.Wallets.FirstOrDefault(w => w.Currency == position.Currency);
//
//         decimal pnl;
//
//         if (position.IsLong)
//             pnl = (currentPrice - position.EntryPrice) * position.Amount;
//         else
//             pnl = (position.EntryPrice - currentPrice) * position.Amount;
//
//         wallet.Balance += pnl;
//         position.IsClosed = true;
//         position.CloseDate = DateTime.UtcNow;
//
//         return position;
//     }
//
//     public void CheckLiquidation(Guid userId, string currencyCode, decimal currentPrice)
//     {
//         var user = _cacheService.Get(userId);
//
//         var positions = user.MarginPositions.Where(p =>
//             p.Currency.Code == currencyCode && !p.IsClosed
//         );
//
//         foreach (var position in positions)
//         {
//             decimal marginUsed = position.Amount * position.EntryPrice / position.Leverage;
//             var wallet = user.Wallets.FirstOrDefault(w => w.Currency.Code == currencyCode);
//
//             decimal pnl = position.IsLong
//                 ? (currentPrice - position.EntryPrice) * position.Amount
//                 : (position.EntryPrice - currentPrice) * position.Amount;
//
//             if (wallet.Balance + pnl < marginUsed)
//             {
//                 position.IsClosed = true;
//                 position.CloseDate = DateTime.UtcNow;
//                 wallet.Balance += pnl;
//             }
//         }
//     }
// }
