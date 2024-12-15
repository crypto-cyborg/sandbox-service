using FluentValidation;
using SandboxService.Core.Interfaces.Services;

namespace SandboxService.Application.Validators;

public class SandboxInitializeValidator : AbstractValidator<SandboxInitializeRequest>
{
    public SandboxInitializeValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}
