using SandboxService.Core.Extensions;
using SandboxService.Core.Models;
using SandboxService.Persistence;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SandboxService.Application.Services;

public class PositionService(UnitOfWork unitOfWork)
{
    public async Task<MarginPosition> CreatePosition(Order order)
    {
        switch (order.Type)
        {
            case OrderType.MARKET:
            {
                var position = MarginPositionExtensions.Create(order);
                await unitOfWork.MarginPositionRepository.InsertAsync(position);
                
                return position;
            }
            case OrderType.LIMIT:
            {
                return new MarginPosition
                {
                    UserId = order.UserId,
                    CurrencyId = order.CurrencyId,
                    Symbol = order.Symbol,
                    PositionAmount = order.PositionAmount,
                    EntryPrice = order.Price,
                }
            }
        }
    }
}
