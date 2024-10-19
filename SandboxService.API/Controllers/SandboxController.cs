using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services;
using SandboxService.Application.Services.Interfaces;

namespace SandboxService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SandboxController : ControllerBase
{
    private readonly IAccountService _accountService;

    public SandboxController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    public async Task<IActionResult> Initialize(
        SanboxInitializeRequest request,
        IValidator<SanboxInitializeRequest> validator
    )
    {
        var validation = validator.Validate(request);

        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors);
        }

        var response = await _accountService.CreateSandboxUser(request);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetPrice(string symbol, BinanceService binanceService)
    {
        var price = await binanceService.GetPrice(symbol);

        return Ok(price);
    }
}
