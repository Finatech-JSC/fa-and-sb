namespace MicroBase.Share.Models.Accounts
{
    public class TopReferralModel
    {
        public int CountTopReferralInWeek { get; set; }

        public int CountTopReferralInMonth { get; set; }

        public string TopUserReferralInWeek { get; set; } = "";

        public string TopUserReferralInMonth { get; set; } = "";
    }
}
