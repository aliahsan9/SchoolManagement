using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class Student : BaseEntity, Common.ISchoolScoped
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string AdmissionNumber { get; set; } = null!;
        public DateTime DOB { get; set; }
        public string Gender { get; set; } = null!;
        public string? BloodGroup { get; set; }
        public string Address { get; set; } = null!;

        public Guid SchoolId { get; set; }
        public School School { get; set; } = null!;

        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
        public ICollection<StudentEnrollment> Enrollments { get; set; } = new List<StudentEnrollment>();
    }
}
