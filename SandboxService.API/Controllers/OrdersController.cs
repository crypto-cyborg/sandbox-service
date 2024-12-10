using Microsoft.AspNetCore.Mvc;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services;

namespace SandboxService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(OrderService orderService, MarginBackgroundService mbs) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder(CreateOrderDto request)
        {
            var order = await orderService.CreateOrder(request);
            mbs.StartTrackingOrder(order.Id, order.Symbol, order.UserId);
            
            return Ok(order);
        }
    }
}
