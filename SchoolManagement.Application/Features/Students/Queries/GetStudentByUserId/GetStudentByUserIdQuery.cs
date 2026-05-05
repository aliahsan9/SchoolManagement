using MediatR;
using SchoolManagement.Application.Features.Students.DTOs;

namespace SchoolManagement.Application.Features.Students.Queries.GetStudentByUserId;

public sealed record GetStudentByUserIdQuery(Guid UserId) : IRequest<StudentDto?>;
