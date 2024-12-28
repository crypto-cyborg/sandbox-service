using LanguageExt.Common;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Extensions;
using SandboxService.Core.Interfaces.Services;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Services;

public class PositionService(UnitOfWork unitOfWork, IBinanceService binanceService) : IPositionService
{
    public async Task<MarginPosition> Open(Order order)
    {
        var position = MarginPositionExtensions.Create(order);

        await unitOfWork.MarginPositionRepository.InsertAsync(position);
        await unitOfWork.SaveAsync();

        return position;
    }

    public async Task<Result<MarginPosition>> Close(Guid? positionId)
    {
        ArgumentNullException.ThrowIfNull(positionId);

        var position = await unitOfWork.MarginPositionRepository.GetByIdAsync(positionId);
        if (position is null)
        {
            return new Result<MarginPosition>(new SandboxException($"Position {positionId} does not exist",
                SandboxExceptionType.ENTITY_NOT_FOUND));
        }

        if (position.IsClosed)
        {
            return new Result<MarginPosition>(new SandboxException($"Position {positionId} is already closed",
                SandboxExceptionType.INVALID_OPERATION));
        }

        var currentPrice = (await binanceService.GetPrice(position.Symbol)).Price;
        position.ExitPrice = currentPrice;

        var user = await unitOfWork.UserRepository.GetByIdAsync(position.UserId);
        if (user is null)
        {
            return new Result<MarginPosition>(new SandboxException($"User {position.UserId} does not exist",
                SandboxExceptionType.ENTITY_NOT_FOUND));
        }

        var account = user.Wallet.Accounts.FirstOrDefault(a => a.CurrencyId == position.CurrencyId);
        if (account is null)
        {
            return new Result<MarginPosition>(new SandboxException($"Wallet not found",
                SandboxExceptionType.ENTITY_NOT_FOUND));
        }

        var profitOrLoss = CalculateProfitOrLoss(position);
        account.Balance += profitOrLoss;

        position.IsClosed = true;
        position.CloseDate = DateTimeOffset.UtcNow;

        await unitOfWork.SaveAsync();

        return position;
    }

    private static decimal CalculateProfitOrLoss(MarginPosition position)
    {
        // Расчёт прибыли или убытка:
        // Если длинная позиция (isLong = true): (цена закрытия - цена открытия) * количество
        // Если короткая позиция (isLong = false): (цена открытия - цена закрытия) * количество
        var priceDifference = position.IsLong
            ? position.ExitPrice - position.EntryPrice
            : position.EntryPrice - position.ExitPrice;

        return priceDifference * position.PositionAmount;
    }
}