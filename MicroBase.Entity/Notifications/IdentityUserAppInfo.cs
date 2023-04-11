using MicroBase.Entity.Accounts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Notifications
{
    [Table("IdentityUser_AppInfos")]
    public class IdentityUserAppInfo : BaseEntity<Guid>
    {
        [Required]
        public Guid AccountId { get; set; }

        [MaxLength(255)]
        public string Platform { get; set; }

        [MaxLength(255)]
        public string Version { get; set; }

        public int IntVersion { get; set; }

        [MaxLength(255)]
        public string DeviceId { get; set; }

        [ForeignKey("AccountId")]
        public virtual IdentityUser IdentityUser { get; set; }
    }
}