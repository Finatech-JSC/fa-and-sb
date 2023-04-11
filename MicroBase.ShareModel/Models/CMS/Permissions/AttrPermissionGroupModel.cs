using System.Collections.Generic;

namespace MicroBase.Share.Models.CMS.Permissions
{
    public class AttrPermissionGroupModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Route { get; set; }

        public List<AttrPermissionInfoModel> PermissionInfos { get; set; }
    }
}