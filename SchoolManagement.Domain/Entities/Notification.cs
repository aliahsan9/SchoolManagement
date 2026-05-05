using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public ICollection<NotificationRecipient> Recipients { get; set; }
            = new List<NotificationRecipient>();
    }
}
