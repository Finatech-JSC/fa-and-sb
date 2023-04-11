namespace MicroBase.Share.Models.CMS.RoboForm.UI
{
    public class RoboTextAttribute : RoboControlAttribute
    {
        public RoboTextAttribute()
        {
            Type = RoboTextType.TextBox;
        }

        public RoboTextAttribute(RoboTextType type)
        {
            Type = type;
        }

        public RoboTextType Type { get; set; }

        public int MinLength { get; set; }

        public int MaxLength { get; set; }

        public string EqualTo { get; set; }

        public string RegexPattern { get; set; }

        public string RegexValue { get; set; }

        /// <summary>
        /// Apply when Type = RoboTextType.FileUpload
        /// </summary>
        public bool IsShowPreview { get; set; }

        public int Height { get; set; }
    }
}