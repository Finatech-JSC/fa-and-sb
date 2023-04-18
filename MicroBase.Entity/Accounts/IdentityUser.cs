using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity.Accounts
{
    [Table("IdentityUsers")]
    public class IdentityUser : IdentityUser<Guid>, IBaseEntity<Guid>
    {
        [Required, MaxLength(255)]
        public override string UserName { get; set; }

        [Required, MaxLength(255)]
        public override string NormalizedUserName { get; set; }

        [Required, MaxLength(255)]
        public override string Email { get; set; }

        [Required, MaxLength(255)]
        public override string NormalizedEmail { get; set; }

        [Required, MaxLength(50)]
        public string AccountType { get; set; }

        [MaxLength(255)]
        public string FullName { get; set; }

        [MaxLength(128)]
        public virtual string LastLoginIpAddress { get; set; }

        public DateTime? LastLoginTime { get; set; }
        
        [MaxLength(50)]
        public string Via { get; set; }

        public bool IsDelete { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [Required]
        public bool IsDefaultPassword { get; set; }

        public DateTime? EmailConfirmDate { get; set; }

        public DateTime? PhoneConfirmDate { get; set; }

        public bool? IsSystemLocked { get; set; }

        [MaxLength(512)]
        public string LockedDescription { get; set; }

        public virtual IdentityUserMetaData IdentityUserMetaData { get; set; }

        public virtual ICollection<PrivilegesUserRoleMap> IdentityUserACGroups { get; set; }

        public virtual ICollection<IdentityUserTwoFA> IdentityUserTwoFAs { get; set; }

        public object GetIdValue()
        {
            return Id;
        }
    }
}