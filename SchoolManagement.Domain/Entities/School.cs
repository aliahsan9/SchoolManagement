using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class School : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Subdomain { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;

        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
        public ICollection<AcademicYear> AcademicYears { get; set; } = new List<AcademicYear>();
        public ICollection<SchoolSubscription> Subscriptions { get; set; } = new List<SchoolSubscription>();
    }
}
