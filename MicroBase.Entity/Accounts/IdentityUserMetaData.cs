using MicroBase.Entity.Localtions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("IdentityUser_MetaData")]
    public class IdentityUserMetaData : BaseEntity<Guid>
    {
        [Required]
        public Guid IdentityUserId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(1000)]
        public string Address { get; set; }

        [MaxLength(10)]
        public string CountryCode { get; set; }

        public Guid? ProvinceId { get; set; }

        public Guid? DistrictId { get; set; }

        [MaxLength(5000)]
        public string Avatar { get; set; }

        public sbyte? Gender { get; set; }

        public int ReferralCount { get; set; }

        public int ReferralWeekCount { get; set; }

        public int ReferralMonthCount { get; set; }

        [MaxLength(10)]
        public string DefaultLanguage { get; set; }

        public bool AllowAppNotification { get; set; }

        public bool AllowEmailNotification { get; set; }

        [MaxLength(50)]
        public string PostCode { get; set; }

        [MaxLength(512)]
        public string WalletAddress { get; set; }

        [MaxLength(512)]
        public string NormalizedWalletAddress { get; set; }

        [ForeignKey("IdentityUserId")]
        public virtual IdentityUser IdentityUser { get; set; }

        [ForeignKey("ProvinceId")]
        public virtual Province Province { get; set; }
    }
}