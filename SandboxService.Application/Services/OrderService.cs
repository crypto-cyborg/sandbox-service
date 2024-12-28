using LanguageExt.Common;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Extensions;
using SandboxService.Core.Interfaces.Services;
using SandboxService.Core.Models;
using SandboxService.Persistence;
using SandboxService.Shared.Dtos;

namespace SandboxService.Application.Services
{
    public class OrderService(
        UnitOfWork unitOfWork,
        IBinanceService binanceService,
        IPositionService positionService
    )
    {
        public async Task<Order> Create(CreateOrderDto data)
        {
            var currency = await unitOfWork.CurrencyRepository.GetByTickerAsync(data.Ticker);

            var price = data.Type == OrderType.MARKET
                ? (await binanceService.GetPrice(data.Symbol)).Price
                : data.Price;

            var order = OrderExtensions.Create(data.Type, data.Symbol, data.Amount, price, data.IsLong,
                currency!.Id, data.UserId);

            await unitOfWork.OrderRepository.InsertAsync(order);

            if (order.Type == OrderType.LIMIT)
            {
                await unitOfWork.SaveAsync();
                return order;
            }

            var position = await positionService.Open(order);

            if (order.Type == OrderType.MARKET) await positionService.Close(position.Id);

            await unitOfWork.SaveAsync();

            return order;
        }

        public async Task<Result<Order>> Close(Guid orderId, OrderStatus status)
        {
            var order = await unitOfWork.OrderRepository.GetByIdAsync(orderId);

            if (order is null)
            {
                return new Result<Order>(new SandboxException("Required order does not exist",
                    SandboxExceptionType.ENTITY_NOT_FOUND));
            }
            
            order.CompletedAt = DateTimeOffset.UtcNow;
            order.Status = status;

            await unitOfWork.SaveAsync();

            return order;
        }
    }
}
