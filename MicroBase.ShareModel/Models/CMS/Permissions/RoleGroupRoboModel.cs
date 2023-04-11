using MicroBase.Share.Models.CMS.RoboForm.UI;

namespace MicroBase.Share.Models.CMS.Permissions
{
    public class RoleGroupRoboModel : BaseModel
    {
        [RoboText(LabelText = "Name", Name = "Name", MaxLength = 512, IsRequired = true, Cols = 12, Order = 1)]
        public string Name { get; set; }

        [RoboCheckbox(LabelText = "Is Default", Name = "IsDefault", Cols = 12, Order = 2, IsHidden = true)]
        public bool IsDefault { get; set; }

        [RoboCheckbox(LabelText = "Allow Full Access", Name = "AllowFullAccess", Cols = 12, Order = 3)]
        public bool AllowFullAccess { get; set; }

        [RoboCheckbox(LabelText = "Enabled", Name = "Enabled", Cols = 12, Order = 4, IsHidden = true)]
        public bool Enabled { get; set; }
    }
}