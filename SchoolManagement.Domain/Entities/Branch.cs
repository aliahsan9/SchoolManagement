using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class Branch : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;

        public Guid SchoolId { get; set; }
        public School School { get; set; } = null!;
    }
}
