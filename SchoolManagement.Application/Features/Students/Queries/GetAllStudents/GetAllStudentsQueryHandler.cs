using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Students.DTOs;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Students.Queries.GetAllStudents;

public sealed class GetAllStudentsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllStudentsQuery, List<StudentDto>>
{
    public async Task<List<StudentDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
    {
        return await context.Students
            .AsNoTracking()
            .OrderBy(s => s.AdmissionNumber)
            .Select(s => new StudentDto
            {
                Id = s.Id,
                UserId = s.UserId,
                SchoolId = s.SchoolId,
                AdmissionNumber = s.AdmissionNumber,
                FullName = s.User.FirstName + " " + s.User.LastName,
                Email = s.User.Email,
                PhoneNumber = s.User.PhoneNumber,
                DOB = s.DOB,
                Gender = s.Gender,
                BloodGroup = s.BloodGroup,
                Address = s.Address
            })
            .ToListAsync(cancellationToken);
    }
}
