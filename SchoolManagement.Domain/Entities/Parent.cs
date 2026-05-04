using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class Parent : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string? Occupation { get; set; }

        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
    }
}
