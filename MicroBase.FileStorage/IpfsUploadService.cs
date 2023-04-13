using Ipfs.Api;
using MicroBase.FileStorage.Models;
using MicroBase.Share.Constants;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace MicroBase.FileStorage
{
    public class IpfsUploadService : IFileUploadService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<IpfsUploadService> logger;
        private readonly HttpClient httpClient;

        public IpfsUploadService(IConfiguration configuration,
            ILogger<IpfsUploadService> logger,
            IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            this.logger = logger;

            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://ipfs.infura.io:5001/");
            var byteArray = Encoding.ASCII.GetBytes("2IdnjISwtbl6kyDnk5Qp6JkpZcv:c86f39f4860c28c84e07995f1896973e");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public async Task<BaseResponse<FileUploadResponse>> UploadImageAsync(IFormFile formFile,
            string prefixPath,
            bool isMakePublic,
            bool isGeneratePathByDate)
        {
            var validate = FileExtensions.ValidateImageFile(formFile);
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

            var res = await httpClient.PostFileRequestAsync<BaseResponse<object>>("api/v0/add", formFile);

            //var ipfsHost = configuration.GetValue<string>("FileManage:Ipfs:Host");
            //var ipfs = new IpfsClient(ipfsHost);

            //using (var stream = formFile.OpenReadStream())
            //{
            //    var res = await ipfs.FileSystem.AddAsync(stream, formFile.FileName);
            //    return new BaseResponse<ImageUploadResponse>
            //    {
            //        Success = false
            //    };
            //}

            var publicBaseUrl = configuration.GetValue<string>("FileManage:LocalUploadConfig:PublicBaseUrl");
            return new BaseResponse<FileUploadResponse>
            {
                Success = true,
                Data = new FileUploadResponse
                {
                    //ImageUrl = String.Format(publicBaseUrl, filePaths.Join("/")),
                    //ThumbnailUrl = String.Format(publicBaseUrl, filePaths.Join("/"))
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
