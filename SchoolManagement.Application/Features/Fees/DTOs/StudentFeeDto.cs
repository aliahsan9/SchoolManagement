namespace SchoolManagement.Application.Features.Fees.DTOs;

public sealed class StudentFeeDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalPaid { get; set; }
}
