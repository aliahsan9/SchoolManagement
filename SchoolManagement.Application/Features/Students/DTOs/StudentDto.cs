namespace SchoolManagement.Application.Features.Students.DTOs;

public sealed class StudentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SchoolId { get; set; }
    public string AdmissionNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public DateTime DOB { get; set; }
    public string Gender { get; set; } = null!;
    public string? BloodGroup { get; set; }
    public string Address { get; set; } = null!;
}
