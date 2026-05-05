namespace SchoolManagement.Application.Features.Exams.DTOs;

public sealed class ExamListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string AcademicYear { get; set; } = null!;
}
