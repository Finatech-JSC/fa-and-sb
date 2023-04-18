using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("Privileges_UserRoleMaps")]
    public class PrivilegesUserRoleMap : BaseEntity<Guid>
    {
        public Guid IdentityUserId { get; set; }

        public Guid? RoleGroupId { get; set; }

        public Guid? RoleId { get; set; }

        [ForeignKey("IdentityUserId")]
        public virtual IdentityUser IdentityUser { get; set; }

        [ForeignKey("RoleGroupId")]
        public virtual PrivilegesGroup PrivilegesGroup { get; set; }

        [ForeignKey("RoleId")]
        public virtual PrivilegesRole PrivilegesRole { get; set; }
    }
}