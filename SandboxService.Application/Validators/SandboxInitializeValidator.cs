using FluentValidation;
using SandboxService.Application.Data.Dtos;

namespace SandboxService.Application.Validators;

public class SandboxInitializeValidator : AbstractValidator<SanboxInitializeRequest>
{
    public SandboxInitializeValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}
