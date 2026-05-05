using MediatR;
using SchoolManagement.Application.Features.Teachers.DTOs;

namespace SchoolManagement.Application.Features.Teachers.Queries.GetTeachers;

public sealed record GetTeachersQuery : IRequest<List<TeacherListDto>>;
