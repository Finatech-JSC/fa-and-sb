using MicroBase.FileStorage.Models;
using MicroBase.Share.Models;
using Microsoft.AspNetCore.Http;

namespace MicroBase.FileStorage
{
    public interface IFileUploadService
    {
        Task<BaseResponse<FileUploadResponse>> SaveFileFormUrlAsync(string fileUrl, string fileName, string prefix);

        Task<BaseResponse<FileUploadResponse>> UploadImageAsync(IFormFile formFile,
            string prefixPath = "",
            bool isMakePublic = true,
            bool isGeneratePathByDate = true);

        Task<BaseResponse<object>> DeleteImageAsync(string fileName);

        Task<BaseResponse<FileUploadResponse>> UploadFileAsync(Stream stream,
            string fileName = "",
            string prefixPath = "",
            bool isMakePublic = true,
            bool isGeneratePathByDate = true);
    }
}