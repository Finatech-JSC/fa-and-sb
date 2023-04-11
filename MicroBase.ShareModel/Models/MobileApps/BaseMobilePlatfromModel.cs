namespace MicroBase.Share.Models.MobileApps
{
    public interface BaseMobilePlatfromModel
    {
        public bool IsAllowIos { get; set; }

        public bool IsAllowAndroid { get; set; }

        public bool IsAllowWeb { get; set; }
    }
}