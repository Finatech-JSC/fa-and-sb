using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MicroBase.Share.Constants;

namespace MicroBase.Share.Models.CMS.Permissions
{
    public class RoleGroupRequest : BaseModel
    {
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        public string Name { get; set; }

        [Required]
        public bool IsDefault { get; set; } = false;

        [Required]
        public bool AllowFullAccess { get; set; } = false;

        [Required]
        public bool Enabled { get; set; } = true;

        public List<PermissionGroupResponse> PermissionGroups { get; set; }

        public List<PermissionResponse> AccountPermissions { get; set; }
    }
}