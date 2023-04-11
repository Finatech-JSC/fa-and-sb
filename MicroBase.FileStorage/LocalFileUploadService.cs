using MicroBase.FileStorage.Models;
using MicroBase.NoDependencyService;
using MicroBase.Share.Constants;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MicroBase.FileStorage
{
    public class LocalFileUploadService : IFileUploadService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<LocalFileUploadService> logger;
        private readonly IRandomService randomService;

        public LocalFileUploadService(IConfiguration configuration,
            ILogger<LocalFileUploadService> logger,
            IRandomService randomService)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.randomService = randomService;
        }

        public async Task<BaseResponse<FileUploadResponse>> UploadImageAsync(IFormFile formFile,
            string prefixPath,
            bool isMakePublic,
            bool isGeneratePathByDate)
        {
            var validate = FileExtensions.ValidateImage(formFile);
            if (!validate)
            {
                var acceptedFileExtensions = configuration.GetSection("FileManage:AcceptedFileExtensions").Get<List<string>>();
                return new BaseResponse<FileUploadResponse>
                {
                    Success = false,
                    Message = CommonMessage.Upload.FILE_EXTENSION_INVALID,
                    MessageCode = nameof(CommonMessage.Upload.FILE_EXTENSION_INVALID),
                    MsgParams = new List<string> { string.Join(",", acceptedFileExtensions) }
                };
            }

            var maxImageSize = configuration.GetValue<int>("FileManage:LocalUploadConfig:MaxImageLength");
            if (maxImageSize > 0 && formFile.Length > maxImageSize * 1048576)
            {
                return new BaseResponse<FileUploadResponse>
                {
                    Success = false,
                    Message = CommonMessage.Upload.FILE_SIZE_GREATER_THAN_INVALID,
                    MessageCode = nameof(CommonMessage.Upload.FILE_SIZE_GREATER_THAN_INVALID),
                    MsgParams = new List<string> { maxImageSize.ToString() }
                };
            }

            var filePaths = new List<string>();
            if (!string.IsNullOrWhiteSpace(prefixPath))
            {
                filePaths.Add(prefixPath);
            }

            if (isGeneratePathByDate)
            {
                filePaths.Add($"{DateTime.UtcNow:yyyyMMdd}");
            }
            else
            {
                var randon = randomService.GenerateRandomCharacter(8, isNumberOnly: true);
                filePaths.Add(randon);
            }

            var savePath = configuration.GetValue<string>("FileManage:LocalUploadConfig:SavePath");
            var localPath = $"{savePath}{Path.DirectorySeparatorChar}{FileExtensions.CombinePaths(filePaths)}";
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }

            var fileName = formFile.FileName.Replace(" ", "");
            localPath = Path.Combine(localPath, fileName);

            using (var stream = new FileStream(localPath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            var publicBaseUrl = configuration.GetValue<string>("FileManage:LocalUploadConfig:PublicBaseUrl");

            filePaths.Add(fileName);
            return new BaseResponse<FileUploadResponse>
            {
                Success = true,
                Data = new FileUploadResponse
                {
                    FileUrl = String.Format(publicBaseUrl, filePaths.Join("/")),
                    ThumbnailUrl = String.Format(publicBaseUrl, filePaths.Join("/"))
                }
            };
        }

        public Task<BaseResponse<object>> DeleteImageAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<FileUploadResponse>> SaveFileFormUrlAsync(string fileUrl, string fileName, string prefix)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<FileUploadResponse>> UploadFileAsync(Stream stream,
            string fileName = "",
            string prefixPath = "",
            bool isMakePublic = true,
            bool isGeneratePathByDate = true)
        {
            throw new NotImplementedException();
        }
    }
}