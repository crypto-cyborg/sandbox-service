using MediatR;
using Microsoft.AspNetCore.Mvc;
using SandboxService.Application.Commands.GetOpenPositions;
using SandboxService.Application.Services;
using SandboxService.Core.Extensions;
using SandboxService.Shared.Dtos;

namespace SandboxService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarginController(MarginTradeService mts, IMediator mediator) : ControllerBase
{
    [HttpPost("positions/open")]
    public async Task<IActionResult> OpenPosition(OpenMarginPositionRequest request)
    {
        var result = await mts.OpenPosition(request);

        return Ok(result.MapToResponse());
    }

    [HttpPost("positions/close")]
    public async Task<IActionResult> ClosePosition(CloseMarginPositionRequest request)
    {
        var result = await mts.ClosePosition(request);

        return Ok(result.MapToResponse());
    }

    [HttpGet("positions/{userId:guid}")]
    public async Task<IActionResult> GetAllPositions(Guid userId)
    {
        var positions = await mediator.Send(new GetOpenPositionsQuery(userId));

        return Ok(positions);
    }

    [HttpPatch("positions/{positionId:guid}/sl")]
    public async Task<IActionResult> PatchStopLoss(Guid positionId, decimal stopLoss)
    {
        var updatedEntity = await mts.ChangeStopLoss(positionId, stopLoss);
        
        return Ok(updatedEntity);
    }
    
    [HttpPatch("positions/{positionId:guid}/tp")]
    public async Task<IActionResult> PatchTakeProfit(Guid positionId, decimal takeProfit)
    {
        var updatedEntity = await mts.ChangeTakeProfit(positionId, takeProfit);
        
        return Ok(updatedEntity);
    }
}