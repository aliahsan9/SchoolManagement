namespace SchoolManagement.Application.Features.Teachers.DTOs;

public sealed class TeacherListDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string EmployeeId { get; set; } = null!;
}
