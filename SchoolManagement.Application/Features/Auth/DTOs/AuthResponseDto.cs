namespace SchoolManagement.Application.Features.Auth.DTOs;

public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    Guid UserId,
    string Email,
    string FullName,
    IReadOnlyList<string> Roles);
