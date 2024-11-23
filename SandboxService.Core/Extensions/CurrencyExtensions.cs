using SandboxService.Core.Models;

namespace SandboxService.Core.Extensions;

public static class CurrencyExtensions
{
    public record CurrencyReadDto(int Id, string Name, string Ticker);

    public record CurrencyCreateDto(string Name, string Ticker);

    public static Currency Create(string name, string ticker) => new() { Name = name, Ticker = ticker };
    public static Currency Create(CurrencyCreateDto dto) => new() { Name = dto.Name, Ticker = dto.Ticker };

    public static CurrencyReadDto MapToResponse(this Currency currency) =>
        new(currency.Id, currency.Name, currency.Ticker);

    public static IEnumerable<CurrencyReadDto> MapToResponse(this IEnumerable<Currency> currencies) =>
        currencies.Select(c => c.MapToResponse());
}