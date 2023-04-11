using System;
using System.Collections.Generic;

namespace MicroBase.Share.Models.CMS.Accounts
{
    public class SystemMenuResponse : BaseModel
    {
        public string Name { get; set; }

        public string FontIcon { get; set; }

        public string ImageUrlIcon { get; set; }

        public string Route { get; set; }

        public string Code { get; set; }

        public string Target { get; set; }

        public Guid? ParentId { get; set; }

        public int DisplayOrder { get; set; }

        public string CssClass { get; set; }

        public bool Enabled { get; set; }

        public List<SubSystemMenuResponse> SubMenus { get; set; }
    }

    public class SubSystemMenuResponse : SystemMenuResponse
    {

    }
}