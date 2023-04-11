using System;
using System.Collections.Generic;

namespace MicroBase.Share.Models.CMS.Accounts
{
    public class SystemAccountResponse : BaseModel
    {
        public bool IsDefaultPassword { get; set; }

        public string FullName { get; set; }
        
        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string RoleGroupsName { get; set; }

        public List<NameValueModel<Guid>> RoleGroups { get; set; }
    }
}