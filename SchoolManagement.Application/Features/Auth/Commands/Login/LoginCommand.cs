using MediatR;
using SchoolManagement.Application.Features.Auth.DTOs;

namespace SchoolManagement.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;
