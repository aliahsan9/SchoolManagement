using SchoolManagement.Domain.Common;
namespace SchoolManagement.Domain.Entities
{
    public class StudentAttendance : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public DateTime Date { get; set; }
        public string Status { get; set; } = null!;

        public Guid? MarkedByTeacherId { get; set; }
    }
}
