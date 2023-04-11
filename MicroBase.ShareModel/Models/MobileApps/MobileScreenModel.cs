using MicroBase.Share.Models.CMS.RoboForm.UI;
using System;

namespace MicroBase.Share.Models.MobileApps
{
    public class MobileScreenModel
    {
        [RoboText(IsHidden = true)]
        public Guid Id { get; set; }

        [RoboText(LabelText = "Code", Name = "Code", MaxLength = 255, Order = 1)]
        public string Code { get; set; }

        [RoboText(LabelText = "Name", Name = "Name", MaxLength = 255, Order = 2)]
        public string Name { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Description", Name = "Description", MaxLength = 255, Order = 3)]
        public string Description { get; set; }

        [RoboCheckbox(LabelText = "Enabled", Name = "Enabled", Order = 4)]
        public bool Enabled { get; set; }
    }
}