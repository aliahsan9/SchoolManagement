using MediatR;

namespace SchoolManagement.Application.Features.Students.Commands.EnrollStudent;

public sealed record EnrollStudentCommand(
    Guid StudentId,
    Guid ClassId,
    Guid SectionId,
    Guid AcademicYearId,
    string RollNumber) : IRequest<Guid>;
