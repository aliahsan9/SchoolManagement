using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Auth.DTOs;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IApplicationDbContext context,
    IPasswordHasherService passwordHasher,
    IAuthenticationTokenService authenticationTokenService)
    : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!passwordHasher.VerifyPassword(user, request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var roleNames = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        return await authenticationTokenService.IssueTokensAsync(user, roleNames, cancellationToken);
    }
}
