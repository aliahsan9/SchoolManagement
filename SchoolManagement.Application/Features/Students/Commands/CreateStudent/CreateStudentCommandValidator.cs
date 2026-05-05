using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Students.Commands.CreateStudent;

public sealed class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator(IApplicationDbContext context, ICurrentTenantContext tenant)
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150)
            .MustAsync(async (email, ct) =>
            {
                var normalized = email.Trim().ToLowerInvariant();
                return !await context.Users.AnyAsync(
                    u => string.Equals(u.Email, normalized, StringComparison.Ordinal),
                    ct);
            })
            .WithMessage("Email is already in use.");

        RuleFor(x => x.PhoneNumber).MaximumLength(20);
        RuleFor(x => x.InitialPassword).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.AdmissionNumber).NotEmpty().MaximumLength(50);

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
            {
                if (!tenant.HasTenant || tenant.SchoolId is null)
                    return true;
                return !await context.Students.AnyAsync(
                    s => s.SchoolId == tenant.SchoolId.Value && s.AdmissionNumber == cmd.AdmissionNumber.Trim(),
                    ct);
            })
            .WithMessage("Admission number must be unique within this school.");

        RuleFor(x => x.Gender).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.BloodGroup).MaximumLength(10);

        RuleFor(x => x.SchoolId)
            .Must(id => !id.HasValue || !tenant.HasTenant || id.Value == tenant.SchoolId!.Value)
            .WithMessage("School must match the current tenant.")
            .MustAsync(async (id, ct) =>
                !id.HasValue || await context.Schools.AnyAsync(s => s.Id == id.Value, ct))
            .WithMessage("School was not found.");
    }
}
