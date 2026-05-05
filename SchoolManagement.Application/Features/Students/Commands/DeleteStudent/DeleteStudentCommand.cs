using MediatR;
namespace SchoolManagement.Application.Features.Students.Commands.DeleteStudent
{
    public class DeleteStudentCommand(Guid id) : IRequest<bool>
    {
        public Guid Id { get; set; } = id;
    }
}
