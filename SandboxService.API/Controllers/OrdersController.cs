using MediatR;
using Microsoft.AspNetCore.Mvc;
using SandboxService.Application.Commands.GetOrderTypes;
using SandboxService.Application.Services;
using SandboxService.Core.Models;
using SandboxService.Shared.Dtos;

namespace SandboxService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(OrderService orderService, MarginBackgroundService mbs, IMediator mediator)
    : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder(CreateOrderDto request)
    {
        var order = await orderService.Create(request);
        mbs.StartTrackingOrder(order.Id, order.Symbol, order.UserId);

        return Ok(order);
    }

    [HttpPost("{orderId:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId)
    {
        var result = await orderService.Close(orderId, OrderStatus.CANCELED);
        
        return result.Match<ActionResult>(Ok, BadRequest);
    }

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<object>>> GetOrderTypes()
    {
        var result = await mediator.Send(new GetOrderTypesQuery());

        return result.Match<ActionResult>(Ok, BadRequest);
    }
}
