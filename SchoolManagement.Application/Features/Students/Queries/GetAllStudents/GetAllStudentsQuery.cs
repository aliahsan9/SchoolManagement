using MediatR;
using SchoolManagement.Application.Features.Students.DTOs;

namespace SchoolManagement.Application.Features.Students.Queries.GetAllStudents
{
    public class GetAllStudentsQuery : IRequest<List<StudentDto>>
    {
    }
}
