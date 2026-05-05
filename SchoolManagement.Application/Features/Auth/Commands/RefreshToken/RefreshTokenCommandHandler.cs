using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Auth.DTOs;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IApplicationDbContext context,
    IJwtTokenService jwtTokenService,
    IAuthenticationTokenService authenticationTokenService)
    : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var hash = jwtTokenService.HashRefreshToken(request.RefreshToken);

        var existing = await context.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt => rt.TokenHash == hash, cancellationToken);

        if (existing is null || !existing.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        existing.RevokedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        var user = existing.User;
        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is disabled.");

        var roleNames = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        return await authenticationTokenService.IssueTokensAsync(user, roleNames, cancellationToken);
    }
}
