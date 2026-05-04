using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class Setting : BaseEntity
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public Guid SchoolId { get; set; }
    }
}
