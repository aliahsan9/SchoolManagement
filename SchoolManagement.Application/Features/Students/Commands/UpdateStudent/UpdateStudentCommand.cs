using MediatR;

namespace SchoolManagement.Application.Features.Students.Commands.UpdateStudent;

public sealed record UpdateStudentCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string AdmissionNumber,
    DateTime DOB,
    string Gender,
    string Address,
    string? BloodGroup,
    string? NewPassword) : IRequest<bool>;
