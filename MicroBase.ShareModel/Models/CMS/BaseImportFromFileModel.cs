using MicroBase.Share.Constants;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MicroBase.Share.Models.CMS
{
    public class BaseImportFromFileModel
    {
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        public IFormFile FileUpload { get; set; }
    }
}