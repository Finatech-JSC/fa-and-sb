using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("IdentityUser_AC_RoleGroup_Maps")]
    public class IdentityUserRoleGroupMap : BaseEntity<Guid>
    {
        [Required]
        public Guid RoleGroupId { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual IdentityUserRole IdentityUserRole { get; set; }

        [ForeignKey("RoleGroupId")]
        public virtual IdentityUserRoleGroup IdentityUserRoleGroup { get; set; }
    }
}