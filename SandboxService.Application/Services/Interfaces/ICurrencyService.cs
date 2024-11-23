using SandboxService.Core.Extensions;

namespace SandboxService.Application.Services.Interfaces;

public interface ICurrencyService
{
    Task<IEnumerable<CurrencyExtensions.CurrencyReadDto>> GetAllCurrencies();
}