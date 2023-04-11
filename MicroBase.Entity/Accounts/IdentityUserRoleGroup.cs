using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("IdentityUser_AC_RoleGroups")]
    public class IdentityUserRoleGroup : BaseEntity<Guid>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsDefault { get; set; } = false;

        [Required]
        public bool AllowFullAccess { get; set; } = false;

        [Required]
        public bool Enabled { get; set; } = false;

        public virtual ICollection<IdentityUserRole> IdentityUserRoles { get; set; }

        public virtual ICollection<IdentityUserRoleGroupMap> IdentityUserRoleGroupMaps { get; set; }
    }
}