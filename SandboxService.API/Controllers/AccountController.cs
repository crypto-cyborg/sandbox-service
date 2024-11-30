using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SandboxService.Application.Commands.CreateAccount;
using SandboxService.Application.Commands.GetOrders;
using SandboxService.Application.Commands.GetWallet;
using SandboxService.Core.Extensions;
using SandboxService.Persistence;

namespace SandboxService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(UnitOfWork unitOfWork, IMediator mediator) : ControllerBase
{
    [HttpGet("{userId:guid}/wallet")]
    public async Task<IActionResult> GetWallet(Guid userId)
    {
        var result = await mediator.Send(new GetWalletQuery(userId));

        return result.Match<ActionResult>(
            succ => Ok(succ.MapToResponse()),
            err => NotFound(err));
    }
    
    [HttpPost("wallet/{walletId:guid}/accounts")]
    public async Task<IActionResult> CreateAccount(Guid walletId, string ticker)
    {
        var result = await mediator.Send(new CreateAccountQuery(walletId, ticker));

        return result.Match<ActionResult>(
            succ => Ok(succ.MapToResponse()),
            err => NotFound(err.Message));
    }

    [HttpGet("{userId:guid}/orders")]
    public async Task<IActionResult> GetOrders(Guid userId)
    {
        var result = await mediator.Send(new GetOrdersQuery(userId));

        return result.Match<ActionResult>(
            succ => Ok(succ.MapToResponse()),
            err => NotFound(err.Message));
    }
}