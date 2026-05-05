using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class FeeStructure : BaseEntity
    {
        public Guid ClassId { get; set; }
        public Classes Classes { get; set; } = null!;

        public Guid FeeTypeId { get; set; }
        public FeeType FeeType { get; set; } = null!;

        public decimal Amount { get; set; }
    }
}
