using FluentValidation;

namespace SchoolManagement.Application.Features.Fees.Queries.GetStudentFees;

public sealed class GetStudentFeesQueryValidator : AbstractValidator<GetStudentFeesQuery>
{
    public GetStudentFeesQueryValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
