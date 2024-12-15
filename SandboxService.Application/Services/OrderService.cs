using SandboxService.Core.Extensions;
using SandboxService.Core.Interfaces.Services;
using SandboxService.Core.Models;
using SandboxService.Persistence;
using SandboxService.Shared.Dtos;

namespace SandboxService.Application.Services
{
    public class OrderService(UnitOfWork unitOfWork, IBinanceService binanceService)
    {
        public async Task<Order> CreateOrder(CreateOrderDto data)
        {
            var currency = await unitOfWork.CurrencyRepository.GetByTickerAsync(data.Ticker);

            switch (data.Type)
            {
                case OrderType.MARKET:
                {
                    var obj = await binanceService.GetPrice(data.Symbol);
                    var order = OrderExtensions.Create(data.Type, data.Symbol, data.Amount, obj.Price, data.IsLong, currency!.Id, data.UserId);
                    var position = MarginPositionExtensions.Create(order);
                    await unitOfWork.MarginPositionRepository.InsertAsync(position);

                    await unitOfWork.OrderRepository.InsertAsync(order);
                    await unitOfWork.SaveAsync();

                    return order;
                }
                case OrderType.LIMIT:
                case OrderType.TAKE_PROFIT:
                case OrderType.SPOT_LOSS:
                {
                    var order = OrderExtensions.Create(data.Type, data.Symbol, data.Amount, data.Price, data.IsLong, currency!.Id, data.UserId);

                    await unitOfWork.OrderRepository.InsertAsync(order);
                    await unitOfWork.SaveAsync();

                    return order;
                }
            }

            // TODO
            throw new Exception();
        }
    }
}
