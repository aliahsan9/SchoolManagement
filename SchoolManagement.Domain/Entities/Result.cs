using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class Result : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public Guid ExamSubjectId { get; set; }
        public ExamSubject ExamSubject { get; set; } = null!;

        public int MarksObtained { get; set; }
        public string Grade { get; set; } = null!;
        public string? Remarks { get; set; }
    }
}
