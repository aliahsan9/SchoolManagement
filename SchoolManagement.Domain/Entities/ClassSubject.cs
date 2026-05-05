using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class ClassSubject : BaseEntity
    {
        public Guid ClassId { get; set; }
        public Classes Classes { get; set; } = null!;

        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
    }
}
