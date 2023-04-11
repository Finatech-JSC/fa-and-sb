using System;

namespace MicroBase.Share.Models.CMS.Locations
{
    public class DistrictResponse : DistrictModel
    {
        public NameValueModel<Guid> Province { get; set; }
    }
}