using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Students.Commands.UpdateStudent;

public sealed class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator(IApplicationDbContext context, ICurrentTenantContext tenant)
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
        RuleFor(x => x.AdmissionNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Gender).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.BloodGroup).MaximumLength(10);

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
            {
                if (!tenant.HasTenant || tenant.SchoolId is null)
                    return true;
                return !await context.Students.AnyAsync(
                    s => s.SchoolId == tenant.SchoolId.Value &&
                         s.AdmissionNumber == cmd.AdmissionNumber.Trim() &&
                         s.Id != cmd.Id,
                    ct);
            })
            .WithMessage("Admission number must be unique within this school.");

        RuleFor(x => x.NewPassword)
            .Must(p => string.IsNullOrEmpty(p) || (p.Length >= 8 && p.Length <= 128))
            .WithMessage("Password must be between 8 and 128 characters when provided.");
    }
}
