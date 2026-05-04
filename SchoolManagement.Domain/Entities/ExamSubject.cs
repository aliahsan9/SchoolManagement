using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class ExamSubject : BaseEntity
    {
        public Guid ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
    }
}
