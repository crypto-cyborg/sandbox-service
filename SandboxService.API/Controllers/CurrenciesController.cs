using Microsoft.AspNetCore.Mvc;
using SandboxService.Core.Extensions;
using SandboxService.Persistence;

namespace SandboxService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrenciesController(UnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCurrencies()
    {
        return Ok(await unitOfWork.CurrencyRepository.GetAsync());
    }

    [HttpPost]
    public async Task<IActionResult> AddCurrency(CurrencyExtensions.CurrencyCreateDto request)
    {
        await unitOfWork.CurrencyRepository.InsertAsync(CurrencyExtensions.Create(request));
        await unitOfWork.SaveAsync();
        
        return Ok(await unitOfWork.CurrencyRepository.GetByTickerAsync(request.Ticker));
    }
}