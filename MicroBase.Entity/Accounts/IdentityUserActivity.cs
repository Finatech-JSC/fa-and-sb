using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("IdentityUser_Activities")]
    public class IdentityUserActivity : BaseEntity<Guid>
    {
        [MaxLength(128)]
        public string IpAddress { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }

        [MaxLength(255)]
        public string UserAgent { get; set; }

        [Required, MaxLength(128)]
        public string Action { get; set; }

        [Required, MaxLength(128)]
        public string Via { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public Guid? IdentityUserId { get; set; }

        [MaxLength(255)]
        public string UserName { get; set; }

        [ForeignKey("IdentityUserId")]
        public virtual IdentityUser IdentityUser { get; set; }
    }
}