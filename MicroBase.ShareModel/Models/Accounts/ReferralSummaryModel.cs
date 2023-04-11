namespace MicroBase.Share.Models.Accounts
{
    public class ReferralSummaryModel : BaseModel
    {
        public string ReferralId { get; set; }

        public string ReferralLink { get; set; }

        /// <summary>
        /// DeepLink mobile app
        /// </summary>
        public string DeepLink { get; set; }

        public int SumOfInvitedUser { get; set; }

        public int SumOfInvitedUserInWeek { get; set; }

        public int SumOfInvitedUserInMonth { get; set; }

        public int NoOfWinInWeek { get; set; }

        public int NoOfWinInMonth { get; set; }

        public string InvitationContent { get; set; }
    }
}