namespace MicroBase.Share.Models.CMS.RoboForm.UI
{
    public class RoboTableAttribute : RoboControlAttribute
    {
        public RoboTableAttribute()
        {
            Type = RoboTextType.Table;
        }

        public RoboTableAttribute(RoboTextType type)
        {
            Type = type;
        }

        public RoboTextType Type { get; set; }
    }
}