using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Students.Commands.EnrollStudent;

public sealed class EnrollStudentCommandValidator : AbstractValidator<EnrollStudentCommand>
{
    public EnrollStudentCommandValidator(IApplicationDbContext context, ICurrentTenantContext tenant)
    {
        RuleFor(x => x.RollNumber).NotEmpty().MaximumLength(30);

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
                await context.Students.AnyAsync(s => s.Id == cmd.StudentId, ct))
            .WithMessage("Student not found.");

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
                await context.Classes.AnyAsync(c => c.Id == cmd.ClassId, ct))
            .WithMessage("Class not found.");

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
                await context.Sections.AnyAsync(s => s.Id == cmd.SectionId && s.ClassId == cmd.ClassId, ct))
            .WithMessage("Section not found for this class.");

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
                await context.AcademicYears.AnyAsync(y => y.Id == cmd.AcademicYearId, ct))
            .WithMessage("Academic year not found.");

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
            {
                if (!tenant.HasTenant || tenant.SchoolId is null) return false;
                var ok = await context.Students.AnyAsync(s => s.Id == cmd.StudentId && s.SchoolId == tenant.SchoolId, ct);
                return ok;
            })
            .WithMessage("Student is not in the current school.");
    }
}
