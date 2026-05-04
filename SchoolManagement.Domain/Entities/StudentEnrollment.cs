using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class StudentEnrollment : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public Guid ClassId { get; set; }
        public Class Class { get; set; } = null!;

        public Guid SectionId { get; set; }
        public Section Section { get; set; } = null!;

        public Guid AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; } = null!;

        public string RollNumber { get; set; } = null!;
    }
}
