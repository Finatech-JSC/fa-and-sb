using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("Privileges_RoleGroupMaps")]
    public class PrivilegesRoleGroupMap : BaseEntity<Guid>
    {
        [Required]
        public Guid GroupId { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual PrivilegesRole PrivilegesRole { get; set; }

        [ForeignKey("GroupId")]
        public virtual PrivilegesGroup PrivilegesGroup { get; set; }
    }
}