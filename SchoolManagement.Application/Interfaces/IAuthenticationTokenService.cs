using SchoolManagement.Application.Features.Auth.DTOs;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Interfaces;

public interface IAuthenticationTokenService
{
    Task<AuthResponseDto> IssueTokensAsync(
        User user,
        IReadOnlyCollection<string> roleNames,
        CancellationToken cancellationToken);
}
