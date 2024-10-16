using Microsoft.AspNetCore.Mvc;
using SandboxService.Core.Interfaces;
using SandboxService.Core.Models;

namespace SandboxService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ICacheService _cacheService;

    public TestController(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_cacheService.GetAll());
    }

    [HttpPost]
    public IActionResult Set(Guid key, UserData value)
    {
        _cacheService.Set(key, value);
        return CreatedAtAction(nameof(Set), value);
    }
}
