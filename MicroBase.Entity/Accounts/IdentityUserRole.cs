using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("Privileges_Roles")]
    public class PrivilegesRole : IdentityRole<Guid>, IBaseEntity<Guid>
    {
        [Required, MaxLength(128)]
        public override string Name { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(128)]
        public string GroupName { get; set; }

        [MaxLength(50)]
        public string GroupCode { get; set; }

        [MaxLength(50)]
        public string HttpMethod { get; set; }

        [MaxLength(255)]
        public string Route { get; set; }

        [MaxLength(255)]
        public string BaseRoute { get; set; }

        [MaxLength(512)]
        public string Description { get; set; }

        public bool IsDelete { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public object GetIdValue()
        {
            return Id;
        }
        
        public virtual ICollection<PrivilegesUserRoleMap> PrivilegesUserGroupMaps { get; set; }

        public virtual ICollection<PrivilegesRoleGroupMap> PrivilegesRoleGroupMaps { get; set; }
    }
}