using MicroBase.Share.Models.CMS.RoboForm.UI;

namespace MicroBase.Share.Models.Emails
{
    public class SmtpSettingRequest
    {
        [RoboText(LabelText = "Display Name", Name = "DisplayName", MaxLength = 255, IsRequired = true, Cols = 12, Order = 1)]
        public string DisplayName { get; set; }

        [RoboText(LabelText = "From Email Address", Name = "FromEmailAddress", MaxLength = 255, IsRequired = true, Cols = 12, Order = 2)]
        public string FromEmailAddress { get; set; }

        [RoboText(LabelText = "Account", Name = "Account", MaxLength = 255, IsRequired = true, Cols = 12, Order = 3)]
        public string Account { get; set; }

        [RoboText(LabelText = "Password", Name = "Password", Type = RoboTextType.Password, MaxLength = 255, IsRequired = true, Cols = 12, Order = 4)]
        public string Password { get; set; }

        [RoboText(LabelText = "Host", Name = "Host", MaxLength = 255, IsRequired = true, Cols = 12, Order = 5)]
        public string Host { get; set; }

        [RoboText(LabelText = "Port", Name = "Port", MaxLength = 255, IsRequired = true, Cols = 12, Order = 6)]
        public int Port { get; set; }

        [RoboCheckbox(LabelText = "Enable SSL", Name = "EnableSsl", Cols = 12, Order = 7)]
        public bool EnableSsl { get; set; }

        [RoboCheckbox(IsHidden = true, LabelText = "Enabled", Name = "Enabled", Cols = 12, Order = 8)]
        public bool Enabled { get; set; }
    }
}