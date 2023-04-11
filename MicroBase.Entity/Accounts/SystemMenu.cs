using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("System_Menus")]
    public class SystemMenu : BaseEntity<Guid>
    {
        [Required, MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Code { get; set; }

        [MaxLength(255)]
        public string FontIcon { get; set; }

        [MaxLength(255)]
        public string ImageUrlIcon { get; set; }

        [MaxLength(255)]
        public string Route { get; set; }

        [MaxLength(255)]
        public string Target { get; set; }

        [MaxLength(255)]
        public string CssClass { get; set; }

        public Guid? ParentId { get; set; }

        public int DisplayOrder { get; set; }

        public bool Enabled { get; set; }

        [ForeignKey("ParentId")]
        public virtual SystemMenu Parent { get; set; }
    }
}