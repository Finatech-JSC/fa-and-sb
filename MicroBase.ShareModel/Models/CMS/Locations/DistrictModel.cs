using MicroBase.Share.Models.CMS.RoboForm.UI;
using System;

namespace MicroBase.Share.Models.CMS.Locations
{
    public class DistrictModel : BaseModel
    {
        [RoboDropDown(LabelText = "Province", FirstOptionLabel = "Choose Province", Name = "ProvinceId", IsRequired = true, Order = 1)]
        public Guid ProvinceId { get; set; }

        [RoboText(LabelText = "Full Name", Name = "FullName", MaxLength = 512, IsRequired = true, Cols = 12, Order = 2)]
        public string FullName { get; set; }

        [RoboText(LabelText = "Short Name", Name = "ShortName", MaxLength = 255, Cols = 12, Order = 3)]
        public string ShortName { get; set; }

        [RoboCheckbox(LabelText = "Enabled", Name = "Enabled", Cols = 12, Order = 4)]
        public bool Enabled { get; set; }
    }
}
