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

        public Guid? ProvinceId { get; set; }

        public Guid? DistrictId { get; set; }

        [MaxLength(5000)]
        public string Avatar { get; set; }

        public sbyte? Gender { get; set; }

        [MaxLength(50)]
        public string PostCode { get; set; }

        [ForeignKey("IdentityUserId")]
        public virtual IdentityUser IdentityUser { get; set; }

        [ForeignKey("ProvinceId")]
        public virtual Province Province { get; set; }
    }
}