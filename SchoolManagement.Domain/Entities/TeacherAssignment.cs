using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class TeacherAssignment : BaseEntity
    {
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public Guid ClassId { get; set; }
        public Classes Classes { get; set; } = null!;

        public Guid SectionId { get; set; }
        public Section Section { get; set; } = null!;

        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
    }
}
