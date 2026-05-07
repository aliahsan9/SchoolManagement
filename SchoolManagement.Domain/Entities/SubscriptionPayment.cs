using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class SubscriptionPayment : BaseEntity
    {
        public Guid SchoolSubscriptionId { get; set; }
        public SchoolSubscription SchoolSubscription { get; set; } = null!;

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "PKR";
        public string PaymentMethod { get; set; } = null!;
        public string? ExternalTransactionId { get; set; }
        public DateTime PaidAtUtc { get; set; }
        public string? Notes { get; set; }
    }
}