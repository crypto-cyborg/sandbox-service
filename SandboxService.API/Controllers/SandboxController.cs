using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SandboxService.Core.Interfaces.Services;

namespace SandboxService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SandboxController(IAccountService accountService) : ControllerBase
{
    [HttpPost("initialize")]
    public async Task<IActionResult> Initialize(
        SandboxInitializeRequest request,
        IValidator<SandboxInitializeRequest> validator
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
