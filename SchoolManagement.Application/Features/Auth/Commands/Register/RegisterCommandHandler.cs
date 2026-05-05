using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Auth.DTOs;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Constants;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IApplicationDbContext context,
    IPasswordHasherService passwordHasher,
    IAuthenticationTokenService authenticationTokenService,
    ICurrentTenantContext tenant)
    : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (!tenant.HasTenant || tenant.SchoolId is null)
            throw new InvalidOperationException("Tenant context is missing.");

        var canonicalRole = RoleNames.All.First(r =>
            r.Equals(request.RoleName.Trim(), StringComparison.OrdinalIgnoreCase));
        var role = await context.Roles
            .FirstAsync(r => r.Name == canonicalRole, cancellationToken);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? string.Empty : request.PhoneNumber.Trim(),
            PasswordHash = string.Empty,
            IsActive = true,
            SchoolId = tenant.SchoolId
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        await context.Users.AddAsync(user, cancellationToken);

        await context.UserRoles.AddAsync(new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = role.Id
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        var roleNames = new[] { role.Name };
        return await authenticationTokenService.IssueTokensAsync(user, roleNames, cancellationToken);
    }
}
