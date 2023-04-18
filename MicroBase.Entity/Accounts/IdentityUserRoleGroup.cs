using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("Privileges_Groups")]
    public class PrivilegesGroup : BaseEntity<Guid>
    {
        [Required, MaxLength(128)]
        public string Name { get; set; }

        [Required]
        public bool IsDefault { get; set; } = false;

        [Required]
        public bool AllowFullAccess { get; set; } = false;

        [Required]
        public bool Enabled { get; set; } = false;

        public virtual ICollection<PrivilegesRole> PrivilegesRoles { get; set; }

        public virtual ICollection<PrivilegesRoleGroupMap> PrivilegesRoleGroupMaps { get; set; }
    }
}