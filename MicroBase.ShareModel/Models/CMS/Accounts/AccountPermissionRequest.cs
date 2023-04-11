using MicroBase.Share.Models.CMS.Permissions;
using System;
using System.Collections.Generic;

namespace MicroBase.Share.Models.CMS.Accounts
{
    public class AccountPermissionRequest
    {
        public IEnumerable<RoleGroupRoboModel> RoleGroups { get; set; }

        public List<AccountPerrmissionResponse> AccountPermissions { get; set; }

        public List<PermissionGroupResponse> SystemPermissions { get; set; }
    }

    public class AccountPerrmissionResponse
    {
        public Guid? RoleGroupId { get; set; }

        public Guid? PermissionId { get; set; }
    }
}
