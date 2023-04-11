using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("IdentityUser_AC_Groups")]
    public class IdentityUserACGroup : BaseEntity<Guid>
    {
        public Guid IdentityUserId { get; set; }

        public Guid? RoleGroupId { get; set; }

        public Guid? RoleId { get; set; }

        [ForeignKey("IdentityUserId")]
        public virtual IdentityUser IdentityUser { get; set; }

        [ForeignKey("RoleGroupId")]
        public virtual IdentityUserRoleGroup IdentityUserRoleGroup { get; set; }

        [ForeignKey("RoleId")]
        public virtual IdentityUserRole IdentityUserRole { get; set; }
    }
}