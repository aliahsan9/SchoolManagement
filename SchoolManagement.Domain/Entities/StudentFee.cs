using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class StudentFee : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public Guid FeeStructureId { get; set; }
        public FeeStructure FeeStructure { get; set; } = null!;

        public DateTime DueDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
