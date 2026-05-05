using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Students.Commands.UpdateStudent;

public sealed class UpdateStudentCommandHandler(
    IApplicationDbContext context,
    IPasswordHasherService passwordHasher,
    ICurrentTenantContext tenant)
    : IRequestHandler<UpdateStudentCommand, bool>
{
    public async Task<bool> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (student is null)
            return false;

        if (tenant.HasTenant && tenant.SchoolId is Guid tid && student.SchoolId != tid)
            return false;

        var user = student.User;

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? string.Empty : request.PhoneNumber.Trim();

        if (!string.IsNullOrEmpty(request.NewPassword))
            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);

        student.AdmissionNumber = request.AdmissionNumber.Trim();
        student.DOB = request.DOB;
        student.Gender = request.Gender.Trim();
        student.Address = request.Address.Trim();
        student.BloodGroup = string.IsNullOrWhiteSpace(request.BloodGroup) ? null : request.BloodGroup.Trim();

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
