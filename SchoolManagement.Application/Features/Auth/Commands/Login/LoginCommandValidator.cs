using FluentValidation;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(ICurrentTenantContext tenant)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(_ => tenant.HasTenant)
            .Equal(true)
            .WithMessage("Tenant is required: send header X-Tenant-Subdomain (e.g. demo) or use your school subdomain.");
    }
}
