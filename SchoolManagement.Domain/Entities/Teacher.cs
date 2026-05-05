using SchoolManagement.Domain.Common;
namespace SchoolManagement.Domain.Entities
{
    public class Teacher : BaseEntity, Common.ISchoolScoped
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string EmployeeId { get; set; } = null!;
        public DateTime JoiningDate { get; set; }
        public string Qualification { get; set; } = null!;
        public int ExperienceYears { get; set; }

        public Guid SchoolId { get; set; }
        public School School { get; set; } = null!;
    }
}
