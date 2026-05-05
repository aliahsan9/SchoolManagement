using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Features.Billing.DTOs;

public sealed class SchoolSubscriptionDto
{
    public Guid Id { get; set; }
    public Guid SchoolId { get; set; }
    public SubscriptionPlanType Plan { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime TrialEndsAtUtc { get; set; }
    public DateTime? CurrentPeriodEndUtc { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
}
