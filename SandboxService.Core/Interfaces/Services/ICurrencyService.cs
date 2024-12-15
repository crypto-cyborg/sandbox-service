using SandboxService.Core.Extensions;

namespace SandboxService.Core.Interfaces.Services;

public interface ICurrencyService
{
    Task<IEnumerable<CurrencyExtensions.CurrencyReadDto>> GetAllCurrencies();
}