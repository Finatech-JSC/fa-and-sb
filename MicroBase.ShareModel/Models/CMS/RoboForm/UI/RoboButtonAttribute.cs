namespace MicroBase.Share.Models.CMS.RoboForm.UI
{
    public class RoboButtonAttribute : RoboControlAttribute
    {
        public RoboButtonAttribute()
        {
            Type = RoboTextType.Buttons;
        }

        public RoboButtonAttribute(RoboTextType type)
        {
            Type = type;
        }
    }
}