using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Localtions
{
    [Table("Districts")]
    public class District : BaseEntity<Guid>
    {
        [Required]
        public Guid ProvinceId { get; set; }

        [Required, MaxLength(255)]
        public string FullName { get; set; }

        [MaxLength(255)]
        public string ShortName { get; set; }

        public bool Enabled { get; set; }

        [ForeignKey("ProvinceId")]
        public virtual Province Province { get; set; }
    }
}