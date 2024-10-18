namespace SandboxService.Core.Exceptions;

public class SandboxException(string message, SandboxExceptionType type) : Exception(message)
{
    public SandboxExceptionType Type { get; set; } = type;
}
