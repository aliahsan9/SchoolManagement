using MediatR;

namespace SchoolManagement.Application.Features.Students.Commands.LinkStudentParent;

public sealed record LinkStudentParentCommand(
    Guid StudentId,
    Guid ParentId,
    string Relation) : IRequest<bool>;
