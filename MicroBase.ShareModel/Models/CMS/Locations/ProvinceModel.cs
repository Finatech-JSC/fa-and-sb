using MicroBase.Share.Models.CMS.RoboForm.UI;

namespace MicroBase.Share.Models.CMS.Locations
{
    public class ProvinceModel : BaseModel
    {
        [RoboText(LabelText = "Full Name", Name = "FullName", MaxLength = 512, IsRequired = true, Cols = 12, Order = 2)]
        public string FullName { get; set; }

        [RoboText(LabelText = "Short Name", Name = "ShortName", MaxLength = 255, Cols = 12, Order = 3)]
        public string ShortName { get; set; }

        [RoboCheckbox(LabelText = "Enabled", Name = "Enabled", Cols = 12, Order = 4)]
        public bool Enabled { get; set; }
    }
}
