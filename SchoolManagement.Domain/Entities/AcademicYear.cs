using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class AcademicYear : BaseEntity
    {
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public Guid SchoolId { get; set; }
        public School School { get; set; } = null!;
    }
}
