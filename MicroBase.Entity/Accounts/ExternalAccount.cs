using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("IdentityUser_ExternalAccounts")]
    public class ExternalAccount : BaseEntity<Guid>
    {
        [Required, MaxLength(100)]
        public string ProviderName { get; set; }

        [MaxLength(255)]
        public string ExternalAccountId { get; set; }

        [Required]
        public Guid IdentityUserId { get; set; }

        [Required, MaxLength(50)]
        public string Platform { get; set; }

        [ForeignKey("IdentityUserId")]
        public IdentityUser IdentityUser { get; set; }
    }
}