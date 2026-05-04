using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class StudentParent : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public Guid ParentId { get; set; }
        public Parent Parent { get; set; } = null!;

        public string Relation { get; set; } = null!;
    }
}
