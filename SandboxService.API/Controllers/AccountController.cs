using Microsoft.AspNetCore.Mvc;
using SandboxService.Persistence;

namespace SandboxService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(UnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetAllWallets(Guid userId)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return NotFound("Required user does not exist");
        }

        return Ok(user.Wallets);
    }
}