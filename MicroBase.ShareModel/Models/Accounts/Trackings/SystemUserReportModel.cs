namespace MicroBase.Share.Models.Accounts.Trackings
{
    public class SystemUserReportModel
    {
        public long DailyAttendance { get; set; }

        public long NormalRegister { get; set; }

        public long SocialRegister { get; set; }

        public long EmailConfirm { get; set; }

        public long PhoneConfirm { get; set; }
    }
}