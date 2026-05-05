using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Constants;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.Students.Commands.CreateStudent;

public sealed class CreateStudentCommandHandler(
    IApplicationDbContext context,
    IPasswordHasherService passwordHasher,
    ICurrentTenantContext tenant)
    : IRequestHandler<CreateStudentCommand, Guid>
{
    public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        if (!tenant.HasTenant || tenant.SchoolId is null)
            throw new InvalidOperationException("Tenant context is required.");

        var schoolId = request.SchoolId ?? tenant.SchoolId.Value;
        if (schoolId != tenant.SchoolId.Value)
            throw new UnauthorizedAccessException("Students can only be created in the current school tenant.");

        var studentRole = await context.Roles
            .FirstAsync(r => r.Name == RoleNames.Student, cancellationToken);

        var user = new User
        {
            Id = Guid.NewGuid(),
            SchoolId = schoolId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? string.Empty : request.PhoneNumber.Trim(),
            PasswordHash = string.Empty,
            IsActive = true
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.InitialPassword);

        await context.Users.AddAsync(user, cancellationToken);

        await context.UserRoles.AddAsync(new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = studentRole.Id
        }, cancellationToken);

        var student = new Student
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            SchoolId = schoolId,
            AdmissionNumber = request.AdmissionNumber.Trim(),
            DOB = request.DOB,
            Gender = request.Gender.Trim(),
            Address = request.Address.Trim(),
            BloodGroup = string.IsNullOrWhiteSpace(request.BloodGroup) ? null : request.BloodGroup.Trim()
        };

        await context.Students.AddAsync(student, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return student.Id;
    }
}
