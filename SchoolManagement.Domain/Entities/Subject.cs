using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class Subject : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
