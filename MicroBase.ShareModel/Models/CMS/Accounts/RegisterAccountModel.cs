using MicroBase.Share.Constants;
using MicroBase.Share.Models.CMS.RoboForm.UI;
using System;
using System.ComponentModel.DataAnnotations;

namespace MicroBase.Share.Models.CMS.Accounts
{
    public class BaseRegisterAccountModel : BaseModel
    {
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        [RoboText(LabelText = "Full name", Name = "FullName", IsRequired = true, MaxLength = 255, Cols = 12, Order = 1)]
        public virtual string FullName { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        [EmailAddress(ErrorMessage = CommonMessage.EMAIL_ADDRESS_INVALID)]
        [RoboText(Type = RoboTextType.Email, LabelText = "Mail address", Name = "Email", IsRequired = true, MaxLength = 255, Cols = 12, Order = 2)]
        public virtual string Email { get; set; }

        [RoboDropDown(LabelText = "Role", FirstOptionLabel = "Choose role", Name = "RoleGroupId", IsRequired = true, Order = 3)]
        public Guid RoleGroupId { get; set; }

        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        [RoboText(Type = RoboTextType.Password, LabelText = "Password", Name = "Password", IsRequired = true, MaxLength = 255, Cols = 12, Order = 4)]
        public virtual string Password { get; set; }

        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        [Compare("Password")]
        [RoboText(Type = RoboTextType.Password, LabelText = "Re Password", Name = "RePassword", EqualTo = "Password", IsRequired = true, MaxLength = 255, Cols = 12, Order = 5)]
        public virtual string RePassword { get; set; }
    }

    public class RegisterAccountModel : BaseRegisterAccountModel
    {

    }

    public class UpdateAccountModel : BaseRegisterAccountModel
    {
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        [EmailAddress(ErrorMessage = CommonMessage.EMAIL_ADDRESS_INVALID)]
        [RoboText(IsReadOnly = true, Type = RoboTextType.Email, LabelText = "Mail address", Name = "Email", MaxLength = 255, Cols = 12, Order = 2)]
        public override string Email { get; set; }

        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        [RoboText(Type = RoboTextType.Password, LabelText = "Password", Name = "Password", MaxLength = 255, Cols = 12, Order = 4)]
        public override string Password { get; set; }

        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        [Compare("Password")]
        [RoboText(Type = RoboTextType.Password, LabelText = "Re Password", Name = "RePassword", EqualTo = "Password", MaxLength = 255, Cols = 12, Order = 5)]
        public override string RePassword { get; set; }
    }

    public class AccountViewDetailModel : BaseRegisterAccountModel
    {
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        [Compare("Password")]
        [RoboText(IsHidden = true, Type = RoboTextType.Password, LabelText = "Re Password", Name = "RePassword", EqualTo = "Password", MaxLength = 255, Cols = 12, Order = 4)]
        public override string RePassword { get; set; }
    }
}