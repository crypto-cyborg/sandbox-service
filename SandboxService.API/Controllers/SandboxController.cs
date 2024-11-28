using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services.Interfaces;

namespace SandboxService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SandboxController(IAccountService accountService) : ControllerBase
{
    [HttpPost("initialize")]
    public async Task<IActionResult> Initialize(
        SanboxInitializeRequest request,
        IValidator<SanboxInitializeRequest> validator
    )
    {
        var validation = await validator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors);
        }

        var response = await accountService.CreateSandboxUser(request);

        return Ok(response);
    }
}
