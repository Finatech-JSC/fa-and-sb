using MicroBase.Entity.Accounts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Notifications
{
    [Table("Notification_Users")]
    public class NotificationUser : BaseEntity<Guid>
    {
        [Required, MaxLength(255)]
        public string ConnectionId { get; set; }

        public Guid IdentityUserId { get; set; }

        [MaxLength(255)]
        public string DeviceId { get; set; }

        [ForeignKey("IdentityUserId")]
        public virtual IdentityUser IdentityUser { get; set; }
    }
}