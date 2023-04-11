using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MicroBase.Entity.Accounts
{
    [Table("TwoFactorAuthens")]
    public class TwoFactorAuthen : BaseEntity<Guid>
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