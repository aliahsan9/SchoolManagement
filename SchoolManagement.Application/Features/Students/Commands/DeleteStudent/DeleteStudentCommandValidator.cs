using FluentValidation;

namespace SchoolManagement.Application.Features.Students.Commands.DeleteStudent;

public sealed class DeleteStudentCommandValidator : AbstractValidator<DeleteStudentCommand>
{
    public DeleteStudentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
