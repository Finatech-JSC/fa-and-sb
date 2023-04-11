using System;

namespace MicroBase.Share.Models.CMS.Permissions
{
    public class AttrPermissionInfoModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string HttpMethod { get; set; }

        public string Route { get; set; }

        public string BaseRoute { get; set; }

        public string GroupCode { get; set; }
    }

    public class IdentityUserPermissionInfo : AttrPermissionInfoModel
    {
        public Guid IdentityUserId { get; set; }
    }

    public class PermissionInfoResponse : AttrPermissionInfoModel
    {
        public Guid Id { get; set; }
    }
}