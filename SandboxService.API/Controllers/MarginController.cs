using MediatR;
using Microsoft.AspNetCore.Mvc;
using SandboxService.Application;
using SandboxService.Application.Commands;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services;
using SandboxService.Core.Extensions;

namespace SandboxService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarginController : ControllerBase
{
    private readonly MarginTradeService _mts;
    private readonly IMediator _mediator;

    public MarginController(MarginTradeService mts, IMediator mediator)
    {
        _mts = mts;
        _mediator = mediator;
    }

    [HttpPost("positions/open")]
    public async Task<IActionResult> OpenPosition(OpenMarginPositionRequest request)
    {
        var result = await _mts.OpenPosition(request);

        return Ok(result.MapToResponse());
    }

    [HttpPost("positions/close")]
    public async Task<IActionResult> ClosePosition(CloseMarginPositionRequest request)
    {
        var result = await _mts.ClosePosition(request);

        return Ok(result.MapToResponse());
    }

    [HttpGet("margin-positions/{apiKey}")]
    public async Task<IActionResult> GetAllPositions(string apiKey)
    {
        var positions = await _mediator.Send(new GetAllPositionsQuery(apiKey));

        return Ok(positions);
    }
}