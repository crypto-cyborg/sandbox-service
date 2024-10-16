
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;
using SandboxService.Core.Exceptions;

namespace SandboxService.API;

public class GlobalExceptionsMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (SandboxException e)
        {
            await Handle(context, e);
        }
        catch (Exception e)
        {
            await Handle(context, e);
        }
    }

    private Task Handle(HttpContext context, Exception e)
    {
        var code = HttpStatusCode.InternalServerError;

        if (e as SandboxException is not null)
        {
            var ex = e as SandboxException;

            switch (ex!.Type)
            {
                case SandboxExceptionType.RECORD_NOT_FOUND:
                    code = HttpStatusCode.NotFound;
                    break;
            }
        }

        var result = JsonSerializer.Serialize(new { code, e.Message });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}
