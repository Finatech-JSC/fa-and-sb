using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity
{
    [Table("Localization_Keys")]
    public class LocalizationKey : BaseEntity<Guid>
    {
        [MaxLength(255)]
        public string Prefix { get; set; }

        [Required, MaxLength(255)]
        public string CultureCode { get; set; }

        [Required, MaxLength(255)]
        public string Key { get; set; }

        public string Content { get; set; }
    }
}