namespace MicroBase.Share.Models.CMS.RoboForm.UI
{
    public class RoboFileUploadAttribute : RoboControlAttribute
    {
        public bool IsShowPreview { get; set; }

        public string ThumbnailField { get; set; }

        /// <summary>
        /// Độ cao của anh preview
        /// </summary>
        public int PreviewHeight { get; set; }

        /// <summary>
        /// Độ rộng của ảnh preview
        /// </summary>
        public int PreviewWidth { get; set; }

        public string FileTemplate { get; set; }
    }
}