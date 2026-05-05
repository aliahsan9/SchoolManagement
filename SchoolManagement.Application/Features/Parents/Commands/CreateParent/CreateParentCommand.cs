using MediatR;

namespace SchoolManagement.Application.Features.Parents.Commands.CreateParent;

public sealed record CreateParentCommand(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string Password,
    string? Occupation) : IRequest<Guid>;
