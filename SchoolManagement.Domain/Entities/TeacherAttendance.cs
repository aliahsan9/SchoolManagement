using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class TeacherAttendance : BaseEntity
    {
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public DateTime Date { get; set; }
        public string Status { get; set; } = null!;
    }
}
