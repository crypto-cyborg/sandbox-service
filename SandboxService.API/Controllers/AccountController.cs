using Microsoft.AspNetCore.Mvc;
using SandboxService.Core.Extensions;
using SandboxService.Persistence;

namespace SandboxService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(UnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetWallet(Guid userId)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return NotFound("Required user does not exist");
        }

        return Ok(user.Wallet.MapToResponse());
    }

    // [HttpPost("{userId:guid}/wallet/{walletId:guid}/accounts")]
    // public async Task<IActionResult> CreateAccount(System.Guid userId, System.Guid walletId)
    // {
    // }
}