using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Students.Commands.LinkStudentParent;

public sealed class LinkStudentParentCommandValidator : AbstractValidator<LinkStudentParentCommand>
{
    public LinkStudentParentCommandValidator(IApplicationDbContext context, ICurrentTenantContext tenant)
    {
        RuleFor(x => x.Relation).NotEmpty().MaximumLength(50);

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
            {
                if (!tenant.HasTenant || tenant.SchoolId is null) return false;
                return await context.Students.AnyAsync(s => s.Id == cmd.StudentId && s.SchoolId == tenant.SchoolId, ct);
            })
            .WithMessage("Student not found in tenant.");

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
            {
                if (!tenant.HasTenant || tenant.SchoolId is null) return false;
                return await context.Parents.AnyAsync(p => p.Id == cmd.ParentId && p.SchoolId == tenant.SchoolId, ct);
            })
            .WithMessage("Parent not found in tenant.");
    }
}
