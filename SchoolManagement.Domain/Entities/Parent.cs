using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class Parent : BaseEntity, ISchoolScoped
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid SchoolId { get; set; }
        public School School { get; set; } = null!;

        public string? Occupation { get; set; }

        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
    }
} 