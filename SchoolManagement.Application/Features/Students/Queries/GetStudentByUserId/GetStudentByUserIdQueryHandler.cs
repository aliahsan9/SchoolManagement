using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Students.DTOs;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Students.Queries.GetStudentByUserId;

public sealed class GetStudentByUserIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetStudentByUserIdQuery, StudentDto?>
{
    public async Task<StudentDto?> Handle(GetStudentByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await context.Students
            .AsNoTracking()
            .Where(s => s.UserId == request.UserId)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
