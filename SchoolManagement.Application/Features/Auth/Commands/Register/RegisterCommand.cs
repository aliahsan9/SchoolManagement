using MediatR;
using SchoolManagement.Application.Features.Auth.DTOs;

namespace SchoolManagement.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string Password,
    string RoleName) : IRequest<AuthResponseDto>;
