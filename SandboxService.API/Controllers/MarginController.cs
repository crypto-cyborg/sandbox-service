using Microsoft.AspNetCore.Mvc;
using SandboxService.Application;

namespace SandboxService.API;

[ApiController]
[Route("margin")]
public class FuturesController : ControllerBase
{
    private readonly MarginTradeService _mts;

    public FuturesController(MarginTradeService mts)
    {
        _mts = mts;
    }

    [HttpPost]
    public async Task<IActionResult> OpenPosition(OpenMarginPositionRequest request)
    {
        var res = await _mts.OpenPosition(request);

        return Ok(res);
    }
}
