using FluentValidation;

namespace SchoolManagement.Application.Features.Students.Queries.GetStudentsPaged;

public sealed class GetStudentsPagedQueryValidator : AbstractValidator<GetStudentsPagedQuery>
{
    public GetStudentsPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.SortBy)
            .Must(s => s is "AdmissionNumber" or "Name" or "DOB" or "Email")
            .WithMessage("SortBy must be AdmissionNumber, Name, DOB, or Email.");
    }
}
