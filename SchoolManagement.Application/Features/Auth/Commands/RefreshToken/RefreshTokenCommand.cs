using MediatR;
using SchoolManagement.Application.Features.Auth.DTOs;

namespace SchoolManagement.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;
