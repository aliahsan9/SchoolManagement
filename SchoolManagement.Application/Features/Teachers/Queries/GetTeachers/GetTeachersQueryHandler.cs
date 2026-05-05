using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Teachers.DTOs;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Teachers.Queries.GetTeachers;

public sealed class GetTeachersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetTeachersQuery, List<TeacherListDto>>
{
    public async Task<List<TeacherListDto>> Handle(GetTeachersQuery request, CancellationToken cancellationToken)
    {
        return await context.Teachers.AsNoTracking()
            .OrderBy(t => t.EmployeeId)
            .Select(t => new TeacherListDto
            {
                Id = t.Id,
                FullName = t.User.FirstName + " " + t.User.LastName,
                Email = t.User.Email,
                EmployeeId = t.EmployeeId
            })
            .ToListAsync(cancellationToken);
    }
}
