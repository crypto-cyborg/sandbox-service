using MediatR;
using Microsoft.AspNetCore.Mvc;
using SandboxService.Application.Commands.GetOrderTypes;
using SandboxService.Application.Services;
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
        var order = await orderService.CreateOrder(request);
        mbs.StartTrackingOrder(order.Id, order.Symbol, order.UserId);

        return Ok(order);
    }

    [HttpGet("orders/types")]
    public async Task<ActionResult<IEnumerable<object>>> GetOrderTypes()
    {
        var result = await mediator.Send(new GetOrderTypesQuery());

        return result.Match<ActionResult>(Ok, BadRequest);
    }
}