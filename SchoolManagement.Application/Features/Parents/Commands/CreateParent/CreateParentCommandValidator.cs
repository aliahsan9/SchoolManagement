using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Parents.Commands.CreateParent;

public sealed class CreateParentCommandValidator : AbstractValidator<CreateParentCommand>
{
    public CreateParentCommandValidator(IApplicationDbContext context, ICurrentTenantContext tenant)
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (email, ct) =>
            {
                var n = email.Trim().ToLowerInvariant();
                return !await context.Users.AnyAsync(u => string.Equals(u.Email, n, StringComparison.Ordinal), ct);
            })
            .WithMessage("Email already in use.");

        RuleFor(x => x.PhoneNumber).MaximumLength(20);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.Occupation).MaximumLength(150);

        RuleFor(_ => tenant.HasTenant)
            .Equal(true)
            .WithMessage("Tenant is required.");
    }
}
