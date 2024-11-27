namespace SandboxService.Core.Exceptions;

public enum SandboxExceptionType
{
    RECORD_NOT_FOUND,
    RECORD_EXISTS,
    INVALID_KEYS,
    INSUFFICIENT_FUNDS,
    INVALID_ASSET,
    CURRENCY_NOT_FOUND,
    WALLET_DOES_NOT_EXIST,
    INVALID_PRICE,
    CONCURRENCY_CONFLICT,
}
