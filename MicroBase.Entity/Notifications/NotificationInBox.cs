using MicroBase.Entity.Accounts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Notifications
{
    [Table("Notification_InBox")]
    public class NotificationInBox : BaseEntity<Guid>
    {
        public Guid? NotificationSettingId { get; set; }

        [Required, MaxLength(255)]
        public string Title { get; set; }

        [Required, MaxLength(512)]
        public string SubContent { get; set; }

        public string BodyDescription { get; set; }

        [MaxLength(5000)]
        public string Image { get; set; }

        [MaxLength(5000)]
        public string ExtraParams { get; set; }

        [MaxLength(5000)]
        public string RedirectTo { get; set; }

        [MaxLength(500)]
        public string RedirectType { get; set; }

        [MaxLength(5000)]
        public string Link { get; set; }

        [Required]
        public bool IsRead { get; set; }

        public int? NotiInScreen { get; set; }

        public Guid? IdentityUserId { get; set; }

        [ForeignKey("IdentityUserId")]
        public virtual IdentityUser IdentityUser { get; set; }

        [ForeignKey("NotificationSettingId")]
        public virtual NotificationSetting NotificationSetting { get; set; }
    }
}