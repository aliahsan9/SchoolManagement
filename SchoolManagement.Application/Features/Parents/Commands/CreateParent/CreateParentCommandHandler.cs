using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Constants;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.Parents.Commands.CreateParent;

public sealed class CreateParentCommandHandler(
    IApplicationDbContext context,
    IPasswordHasherService passwordHasher,
    ICurrentTenantContext tenant)
    : IRequestHandler<CreateParentCommand, Guid>
{
    public async Task<Guid> Handle(CreateParentCommand request, CancellationToken cancellationToken)
    {
        if (!tenant.HasTenant || tenant.SchoolId is null)
            throw new InvalidOperationException("Tenant is required.");

        var role = await context.Roles.FirstAsync(r => r.Name == RoleNames.Parent, cancellationToken);

        var user = new User
        {
            Id = Guid.NewGuid(),
            SchoolId = tenant.SchoolId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? string.Empty : request.PhoneNumber.Trim(),
            PasswordHash = string.Empty,
            IsActive = true
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        await context.Users.AddAsync(user, cancellationToken);
        await context.UserRoles.AddAsync(new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = role.Id
        }, cancellationToken);

        var parent = new Parent
        {
            Id = Guid.NewGuid(),
            SchoolId = tenant.SchoolId.Value,
            UserId = user.Id,
            Occupation = string.IsNullOrWhiteSpace(request.Occupation) ? null : request.Occupation.Trim()
        };

        await context.Parents.AddAsync(parent, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return parent.Id;
    }
}
