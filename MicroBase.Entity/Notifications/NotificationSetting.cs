using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Notifications
{
    [Table("Notification_Settings")]
    public class NotificationSetting : BaseEntity<Guid>
    {
        [Required, MaxLength(255)]
        public string Title { get; set; }

        [Required, MaxLength(512)]
        public string SubContent { get; set; }

        public string Content { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [MaxLength(255)]
        public string RedirectTo { get; set; }

        [MaxLength(255)]
        public string ExtraParams { get; set; }

        [MaxLength(5000)]
        public string Image { get; set; }

        [MaxLength(500)]
        public string RedirectType { get; set; }

        [MaxLength(5000)]
        public string Link { get; set; }

        [Required]
        public bool PushToMailBox { get; set; }

        [Required]
        public bool Enabled { get; set; }

        [Required]
        public bool IsSend { get; set; }

        public int? NotiInScreen { get; set; }

        [MaxLength(255)]
        public string Key { get; set; }

        [Required]
        public bool SendManually { get; set; }

        [Required]
        public int RepeatDay { get; set; } = 0;

        public DateTime? LatestSend { get; set; }

        public virtual ICollection<NotificationInBox> NotificationQueues { get; set; }
    }
}