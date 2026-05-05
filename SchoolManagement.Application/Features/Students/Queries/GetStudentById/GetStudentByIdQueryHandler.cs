using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Students.DTOs;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Constants;

namespace SchoolManagement.Application.Features.Students.Queries.GetStudentById;

public sealed class GetStudentByIdQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser)
    : IRequestHandler<GetStudentByIdQuery, StudentDto?>
{
    public async Task<StudentDto?> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await context.Students
            .AsNoTracking()
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (student is null)
            return null;

        var roles = currentUser.Roles;
        var isStaff = roles.Contains(RoleNames.Admin) || roles.Contains(RoleNames.Teacher);
        var isOwnerStudent = roles.Contains(RoleNames.Student)
            && currentUser.UserId == student.UserId;

        if (currentUser.SchoolId is Guid claimSchool && claimSchool != student.SchoolId)
            return null;

        if (!isStaff && !isOwnerStudent)
            return null;

        return new StudentDto
        {
            Id = student.Id,
            UserId = student.UserId,
            SchoolId = student.SchoolId,
            AdmissionNumber = student.AdmissionNumber,
            FullName = student.User.FirstName + " " + student.User.LastName,
            Email = student.User.Email,
            PhoneNumber = student.User.PhoneNumber,
            DOB = student.DOB,
            Gender = student.Gender,
            BloodGroup = student.BloodGroup,
            Address = student.Address
        };
    }
}
