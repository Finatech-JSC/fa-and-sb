using System.Collections.Generic;

namespace MicroBase.Share.Models.CMS.Permissions
{
    public class PermissionGroupResponse : BaseModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool AllowFullAccess { get; set; } = false;

        public int Order { get; set; }

        public List<PermissionInfoResponse> PermissionInfos { get; set; }
    }
}