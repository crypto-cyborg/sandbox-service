using SandboxService.Core.Extensions;
using SandboxService.Core.Interfaces.Services;
using SandboxService.Persistence;

namespace SandboxService.Application.Services;

public class CurrencyService(UnitOfWork unitOfWork) : ICurrencyService
{
    public async Task<IEnumerable<CurrencyExtensions.CurrencyReadDto>> GetAllCurrencies()
        => (await unitOfWork.CurrencyRepository.GetAsync()).MapToResponse();

}