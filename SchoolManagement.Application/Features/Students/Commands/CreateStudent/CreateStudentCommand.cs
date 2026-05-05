using MediatR;

namespace SchoolManagement.Application.Features.Students.Commands.CreateStudent;

public sealed record CreateStudentCommand(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string InitialPassword,
    string AdmissionNumber,
    DateTime DOB,
    string Gender,
    string Address,
    string? BloodGroup,
    Guid? SchoolId) : IRequest<Guid>;
