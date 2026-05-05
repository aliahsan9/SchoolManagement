using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public Guid? SchoolId { get; set; }
        public Guid? UserId { get; set; }
        public string Action { get; set; } = null!;
        public string TableName { get; set; } = null!;
        public Guid RecordId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
