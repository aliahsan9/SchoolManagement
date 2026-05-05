using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class SchoolSubscription : BaseEntity, Common.ISchoolScoped
{
    public Guid SchoolId { get; set; }
    public School School { get; set; } = null!;

    public SubscriptionPlanType Plan { get; set; }
    public SubscriptionStatus Status { get; set; }

    public DateTime TrialEndsAtUtc { get; set; }
    public DateTime? CurrentPeriodEndUtc { get; set; }

    public decimal MonthlyPriceSnapshot { get; set; }
    public decimal YearlyPriceSnapshot { get; set; }

    public ICollection<SubscriptionPayment> Payments { get; set; } = new List<SubscriptionPayment>();
}
