using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid StudentFeeId { get; set; }
        public StudentFee StudentFee { get; set; } = null!;

        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? TransactionId { get; set; }
    }
}
