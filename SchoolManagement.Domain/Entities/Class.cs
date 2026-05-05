using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class Classes : BaseEntity, Common.ISchoolScoped
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public Guid SchoolId { get; set; }
        public School School { get; set; } = null!;

        public ICollection<Section> Sections { get; set; } = new List<Section>();
        public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    }
}
