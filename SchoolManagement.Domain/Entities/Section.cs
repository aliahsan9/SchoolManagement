using SchoolManagement.Domain.Common;
namespace SchoolManagement.Domain.Entities
{
    public class Section : BaseEntity
    {
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }

        public Guid ClassId { get; set; }
        public Classes Classes { get; set; } = null!;
    }
}
