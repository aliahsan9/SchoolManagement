using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateAccessToken(
        User user,
        IReadOnlyCollection<string> roleNames,
        Guid? schoolId,
        string? schoolSubdomain);
    (string PlainToken, DateTime ExpiresAtUtc) CreateRefreshTokenValue();
    string HashRefreshToken(string plainToken);
}
