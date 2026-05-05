using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class Timetable : BaseEntity
    {
        public Guid ClassId { get; set; }
        public Classes Classes { get; set; } = null!;

        public Guid SectionId { get; set; }
        public Section Section { get; set; } = null!;

        public DayOfWeek DayOfWeek { get; set; }
        public int PeriodNumber { get; set; }

        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
