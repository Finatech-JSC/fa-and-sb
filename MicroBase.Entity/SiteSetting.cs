using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity
{
    [Table("SiteSettings")]
    public class SiteSetting : BaseEntity<Guid>
    {
        [Required, MaxLength(255)]
        public string Key { get; set; }
        
        [MaxLength(512)]
        public string ModelField { get; set; }

        public bool ModelFieldIsArray { get; set; }

        [MaxLength(255)]
        public string GroupKey { get; set; }

        public string StringValue { get; set; }

        public bool? BoolValue { get; set; }

        public decimal? NumberValue { get; set; }

        public int Order { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// Set secret parameters, do not allow to retrieve from API
        /// </summary>
        public bool IsSecret { get; set; } = false;
    }
}