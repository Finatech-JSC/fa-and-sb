namespace MicroBase.Share.Models.MobileApps
{
    public class MobileMenuResponse : BaseMobilePlatfromResponse
    {
        public string ThumbnailUrl { get; set; }

        public string Name { get; set; }

        public string ActionType { get; set; }

        public string ActionTo { get; set; }

        public string Tooltip { get; set; }

        public string TooltipColor { get; set; }

        public int DisplayOrder { get; set; }

        public bool RequiredLogin { get; set; }

        public string MenuType { get; set; }
    }
}