using Microsoft.AspNetCore.Mvc;
using SandboxService.Application;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services;
using SandboxService.Core.Extensions;

namespace SandboxService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarginController(MarginTradeService mts) : ControllerBase
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
}