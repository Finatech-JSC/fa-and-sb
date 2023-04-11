using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("IdentityUser_TwoFA")]
    public class IdentityUserTwoFA : BaseEntity<Guid>
    {
        [Required, MaxLength(255)]
        public string TwoFactorService { get; set; }

        [MaxLength(5000)]
        public string Setting { get; set; }

        [Required]
        public Guid IdentityUserId { get; set; }

        [ForeignKey("IdentityUserId")]
        public virtual IdentityUser IdentityUser { get; set; }
    }
}