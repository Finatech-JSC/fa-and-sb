using MicroBase.Entity.Accounts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Localtions
{
    [Table("Provinces")]
    public class Province : BaseEntity<Guid>
    {
        [Required, MaxLength(512)]
        public string FullName { get; set; }

        [MaxLength(255)]
        public string ShortName { get; set; }

        public bool Enabled { get; set; }

        [MaxLength(10)]
        public string CountryCode { get; set; }

        public int Order { get; set; }

        public virtual ICollection<District> Districts { get; set; }
        
        public virtual ICollection<IdentityUserMetaData> IdentityUserMetaDatas { get; set; }
    }
}