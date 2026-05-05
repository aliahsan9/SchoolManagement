using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.Students.Commands.LinkStudentParent;

public sealed class LinkStudentParentCommandHandler(IApplicationDbContext context)
    : IRequestHandler<LinkStudentParentCommand, bool>
{
    public async Task<bool> Handle(LinkStudentParentCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.StudentParents.AnyAsync(
            sp => sp.StudentId == request.StudentId && sp.ParentId == request.ParentId,
            cancellationToken);

        if (exists)
            return true;

        await context.StudentParents.AddAsync(new StudentParent
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            ParentId = request.ParentId,
            Relation = request.Relation.Trim()
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
