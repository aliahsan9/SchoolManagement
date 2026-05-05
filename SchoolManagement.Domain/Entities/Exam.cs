using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class Exam : BaseEntity
    {
        public string Name { get; set; } = null!;
        public Guid AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
