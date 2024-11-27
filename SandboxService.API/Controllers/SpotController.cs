using Microsoft.AspNetCore.Mvc;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services;
using SandboxService.Core.Models;

namespace SandboxService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SpotController : ControllerBase
{
    private readonly SpotTradeService _spotTradeService;

    public SpotController(SpotTradeService spotTradeService)
    {
        _spotTradeService = spotTradeService;
    }

    [HttpPost("buy")]
    public async Task<ActionResult<User>> Buy(SpotPurchaseRequest request)
    {
        var user = await _spotTradeService.Buy(request);
        return Ok(user);
        return Ok();
    }

    [HttpPost("sell")]
    public async Task<ActionResult<User>> Sell(SpotSellRequest request)
    {
        // var user = await _spotTradeService.Sell(request);
        // return Ok(user);
        return Ok();
    }
}
