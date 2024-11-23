using SandboxService.Core.Extensions;
using SandboxService.Persistence;

namespace SandboxService.Application.Services;

public class CurrencyService(UnitOfWork unitOfWork)
{
    public async Task<IEnumerable<CurrencyExtensions.CurrencyReadDto>> GetAllCurrencies()
        => (await unitOfWork.CurrencyRepository.GetAsync()).MapToResponse();

    public async Task<CurrencyExtensions.CurrencyReadDto> CreateCurrency(CurrencyExtensions.CurrencyCreateDto dto)
    {
        
    }
}