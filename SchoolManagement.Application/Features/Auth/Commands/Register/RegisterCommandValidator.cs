using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Constants;

namespace SchoolManagement.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(IApplicationDbContext context, ICurrentTenantContext tenant)
    {
        RuleFor(_ => tenant.HasTenant)
            .Equal(true)
            .WithMessage("Tenant is required: send header X-Tenant-Subdomain (e.g. demo).");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150)
            .MustAsync(async (email, ct) =>
            {
                var normalized = email.Trim().ToLowerInvariant();
                return !await context.Users.AnyAsync(
                    u => u.Email == normalized,
                    ct);
            })
            .WithMessage("Email is already registered.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .Must(name => RoleNames.All.Any(r =>
                r.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)))
            .WithMessage($"Role must be one of: {string.Join(", ", RoleNames.All)}.")
            .MustAsync(async (roleName, ct) =>
            {
                var canonical = RoleNames.All.First(r =>
                    r.Equals(roleName.Trim(), StringComparison.OrdinalIgnoreCase));
                return await context.Roles.AnyAsync(r => r.Name == canonical, ct);
            })
            .WithMessage("Role does not exist. Ensure the database has been seeded.");
    }
}
