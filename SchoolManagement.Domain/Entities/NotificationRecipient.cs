using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class NotificationRecipient : BaseEntity
    {
        public Guid NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;

        public Guid UserId { get; set; }

        public bool IsRead { get; set; }
    }
}
