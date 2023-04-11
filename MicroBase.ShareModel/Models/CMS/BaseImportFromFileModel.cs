using MicroBase.Share.Models.CMS.RoboForm.UI;
using Microsoft.AspNetCore.Http;

namespace MicroBase.Share.Models.CMS
{
    public class BaseImportFromFileModel
    {
        [RoboText(Type = RoboTextType.HyperLink, LabelText = "Tải file mẫu", Name = "FileTemplate", MaxLength = 255, Cols = 12, IsRequired = true, Order = 1)]
        public string FileTemplate { get; set; }

        [RoboFileUpload(LabelText = "Chọn file", Name = "FileUpload", IsRequired = true, Order = 2)]
        public IFormFile FileUpload { get; set; }
    }
}