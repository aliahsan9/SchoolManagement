using MediatR;
using SchoolManagement.Application.Features.Students.DTOs;

namespace SchoolManagement.Application.Features.Students.Queries.GetStudentById
{


    public class GetStudentByIdQuery : IRequest<StudentDto?>
    {
        public Guid Id { get; set; }

        public GetStudentByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
