using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Auth.DTOs;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Services
{
    public sealed class AuthenticationTokenService(
        IApplicationDbContext context,
        IJwtTokenService jwtTokenService)
        : IAuthenticationTokenService
    {
        public async Task<AuthResponseDto> IssueTokensAsync(
            User user,
            IReadOnlyCollection<string> roleNames,
            CancellationToken cancellationToken)
        {
            string? subdomain = null;
            if (user.SchoolId is Guid schoolPk)
            {
                subdomain = await context.Schools.AsNoTracking()
                    .Where(s => s.Id == schoolPk)
                    .Select(s => s.Subdomain)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var (accessToken, accessExpires) = jwtTokenService.CreateAccessToken(
                user,
                roleNames,
                user.SchoolId,
                subdomain);
            var (plainRefresh, refreshExpires) = jwtTokenService.CreateRefreshTokenValue();
            var hash = jwtTokenService.HashRefreshToken(plainRefresh);

            await context.RefreshTokens.AddAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = hash,
                ExpiresAt = refreshExpires
            }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto(
                accessToken,
                plainRefresh,
                accessExpires,
                user.Id,
                user.Email,
                $"{user.FirstName} {user.LastName}".Trim(),
                roleNames.ToList());
        }
    }
}