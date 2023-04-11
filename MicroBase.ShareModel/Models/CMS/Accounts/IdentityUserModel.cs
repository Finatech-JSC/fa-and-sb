using MicroBase.Share.Models.CMS.RoboForm.UI;
using System;

namespace MicroBase.Share.Models.CMS.Accounts
{
    public class IdentityUserCmsModel : BaseModel
    {
        [RoboText(LabelText = "Full name", Name = "UserName", MaxLength = 255, IsRequired = true, Cols = 12, Order = 1)]
        public string UserName { get; set; }

        [RoboText(LabelText = "Full name (Kana)", Name = "UserNameKana", MaxLength = 255, IsRequired = true, Cols = 12, Order = 2)]
        public string UserNameKana { get; set; }

        [RoboText(IsHidden = true, Type = RoboTextType.Email, LabelText = "User Email Address", Name = "Email", MaxLength = 255, IsRequired = true, Cols = 12, Order = 3)]
        public string Email { get; set; }

        [RoboText(IsHidden = true, LabelText = "Phone Number", Name = "PhoneNumber", MaxLength = 255, Cols = 12, Order = 4)]
        public string PhoneNumber { get; set; }

        [RoboDropDown(IsHidden = true, LabelText = "City", FirstOptionLabel = "Choose City", Name = "ProvinceId", Order = 6)]
        public Guid? ProvinceId { get; set; }

        public string ProvinceName { get; set; }

        [RoboText(IsHidden = true, LabelText = "Post code", Name = "PostCode", MaxLength = 255, Cols = 12, Order = 7)]
        public string PostCode { get; set; }

        [RoboText(IsHidden = true, Type = RoboTextType.MultiText, LabelText = "Address", Name = "Address", MaxLength = 512, Cols = 12, Order = 8)]
        public string Address { get; set; }

        [RoboText(IsHidden = true, Type = RoboTextType.MultiText, LabelText = "Address2", Name = "Address2", MaxLength = 512, Cols = 12, Order = 9)]
        public string Address2 { get; set; }

        [RoboText(IsHidden = true, Type = RoboTextType.MultiText, LabelText = "Address3", Name = "Address3", MaxLength = 512, Cols = 12, Order = 10)]
        public string Address3 { get; set; }
    }

    public class IdentityUserCmsResponse : IdentityUserCmsModel
    {
        public NameValueModel<Guid> Province { get; set; }
    }
}