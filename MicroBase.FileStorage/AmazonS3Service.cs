using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using MicroBase.FileStorage.Models;
using MicroBase.Share.Extensions;
using MicroBase.NoDependencyService;
using MicroBase.Share.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MicroBase.Share.Constants;

namespace MicroBase.FileStorage
{
    public class AmazonS3Service : IFileUploadService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<AmazonS3Service> logger;
        private readonly IRandomService randomService;

        public AmazonS3Service(IConfiguration configuration,
            ILogger<AmazonS3Service> logger,
            IRandomService randomService)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.randomService = randomService;
        }

        public async Task<BaseResponse<object>> DeleteImageAsync(string fileName)
        {
            try
            {
                var response = await DeleteFromS3(fileName);
                if (response != null)
                {
                    return new BaseResponse<object>
                    {
                        Success = true
                    };
                }

                return new BaseResponse<object>
                {
                    Success = false
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.UN_DETECTED_ERROR
                };
            }
        }

        public async Task<BaseResponse<FileUploadResponse>> UploadImageAsync(IFormFile formFile,
            string prefixPath,
            bool isMakePublic,
            bool isGeneratePathByDate)
        {
            var validate = FileExtensions.ValidateImageFile(formFile);
            if (!validate)
            {
                var acceptedFileExtensions = configuration.GetSection("AcceptedFileExtensions").Get<List<string>>();
                if(acceptedFileExtensions == null)
                {
                    acceptedFileExtensions = new List<string>();
                }

                return new BaseResponse<FileUploadResponse>
                {
                    Success = false,
                    Message = CommonMessage.Upload.FILE_EXTENSION_INVALID,
                    MessageCode = nameof(CommonMessage.Upload.FILE_EXTENSION_INVALID),
                    MsgParams = new List<string> { string.Join(",", acceptedFileExtensions) }
                };
            }

            var maxImageSize = configuration.GetValue<int>("FileManage:AmazonConfig:MaxImageLength");
            if (maxImageSize > 0 && formFile.Length > maxImageSize * 1048576)
            {
                return new BaseResponse<FileUploadResponse>
                {
                    Success = false,
                    Message = String.Format(CommonMessage.Upload.FILE_SIZE_GREATER_THAN_INVALID, maxImageSize),
                    MessageCode = nameof(CommonMessage.Upload.FILE_SIZE_GREATER_THAN_INVALID),
                    MsgParams = new List<string> { maxImageSize.ToString() }
                };
            }

            var fileName = formFile.FileName.ToLower().Replace(" ", "-");
            var randon = randomService.GenerateRandomCharacter(8, isNumberOnly: true);
            var filePath = $"{randon}/{fileName}";

            if (isGeneratePathByDate)
            {
                filePath = $"{DateTime.UtcNow:yyyyMMdd}/{randon}/{fileName}";
            }

            if (!string.IsNullOrWhiteSpace(prefixPath))
            {
                filePath = $"{prefixPath}/{filePath}";
            }

            try
            {
                using (var stream = formFile.OpenReadStream())
                {
                    var filePathRes = await UploadToS3Async(stream, filePath, isMakePublic);
                    if (!string.IsNullOrWhiteSpace(filePathRes))
                    {
                        return new BaseResponse<FileUploadResponse>
                        {
                            Success = true,
                            Data = new FileUploadResponse
                            {
                                FileUrl = filePathRes
                            }
                        };
                    }

                    return new BaseResponse<FileUploadResponse>
                    {
                        Success = false
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return new BaseResponse<FileUploadResponse>
                {
                    Success = false,
                    Message = CommonMessage.UN_DETECTED_ERROR
                };
            }
        }

        private async Task<string> UploadToS3Async(Stream stream, string filePath, bool makePublic)
        {
            try
            {
                var s3BasePath = configuration.GetValue<string>("FileManage:AmazonConfig:S3BasePath");
                var awsAccessKey = configuration.GetValue<string>("FileManage:AmazonConfig:AWSAccessKey");
                var awsSecretKey = configuration.GetValue<string>("FileManage:AmazonConfig:AWSSecretKey");
                var regionEndpoint = configuration.GetValue<string>("FileManage:AmazonConfig:RegionEndpoint");
                var bucketName = configuration.GetValue<string>("FileManage:AmazonConfig:BucketName");

                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = filePath,
                    InputStream = stream,
                    CannedACL = S3CannedACL.PublicRead
                };

                var awsConfig = new AmazonS3Config
                {
                    Timeout = TimeSpan.FromSeconds(600),
                    MaxErrorRetry = 5,
                    RegionEndpoint = RegionEndpoint.GetBySystemName(regionEndpoint)
                };

                using (IAmazonS3 client = new AmazonS3Client(awsAccessKey, awsSecretKey, awsConfig))
                {
                    var response = await client.PutObjectAsync(request);
                    if (response != null)
                    {
                        return string.Format(s3BasePath, filePath);
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<DeleteObjectResponse> DeleteFromS3(string filePath)
        {
            var awsAccessKey = configuration.GetValue<string>("FileManage:AmazonConfig:AWSAccessKey");
            var awsSecretKey = configuration.GetValue<string>("FileManage:AmazonConfig:AWSSecretKey");
            var regionEndpoint = configuration.GetValue<string>("FileManage:AmazonConfig:RegionEndpoint");
            var bucketName = configuration.GetValue<string>("FileManage:AmazonConfig:BucketName");

            IAmazonS3 client = new AmazonS3Client(awsAccessKey, awsSecretKey,
                new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(regionEndpoint)
                });

            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = filePath
            };

            var response = await client.DeleteObjectAsync(request);

            client.Dispose();
            return response;
        }

        public Task<BaseResponse<FileUploadResponse>> SaveFileFormUrlAsync(string fileUrl, string fileName, string prefix)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse<FileUploadResponse>> UploadFileAsync(Stream stream,
            string fileName = "",
            string prefixPath = "",
            bool isMakePublic = true,
            bool isGeneratePathByDate = true)
        {
            fileName = fileName.ToLower().Replace(" ", "-");
            var randon = randomService.GenerateRandomCharacter(8, isNumberOnly: true);
            var filePath = $"{randon}/{fileName}";

            if (isGeneratePathByDate)
            {
                filePath = $"{DateTime.UtcNow:yyyyMMdd}/{randon}/{fileName}";
            }

            if (!string.IsNullOrWhiteSpace(prefixPath))
            {
                filePath = $"{prefixPath}/{filePath}";
            }

            try
            {
                var filePathRes = await UploadToS3Async(stream, filePath, isMakePublic);
                if (!string.IsNullOrWhiteSpace(filePathRes))
                {
                    return new BaseResponse<FileUploadResponse>
                    {
                        Success = true,
                        Data = new FileUploadResponse
                        {
                            FileUrl = filePathRes
                        }
                    };
                }

                return new BaseResponse<FileUploadResponse>
                {
                    Success = false
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return new BaseResponse<FileUploadResponse>
                {
                    Success = false,
                    Message = CommonMessage.UN_DETECTED_ERROR
                };
            }
        }
    }
}