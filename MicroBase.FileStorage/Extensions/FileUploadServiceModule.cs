using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroBase.FileStorage.Extensions
{
    public static class FileUploadServiceModule
    {
        public static void ModuleRegister(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddTransient<AmazonS3Service>();
            services.AddTransient<LocalFileUploadService>();
            services.AddTransient<FileUploadFactory>();
            services.AddTransient<IpfsUploadService>();

            services.AddTransient<FileUploadServiceResolver>(serviceProvider => key =>
            {
                if (key == FileUploadConstants.FileUploadService.AMAZON_S3_UPLOAD)
                {
                    return serviceProvider.GetService<AmazonS3Service>();
                }
                else if (key == FileUploadConstants.FileUploadService.LOCAL_UPLOAD)
                {
                    return serviceProvider.GetService<LocalFileUploadService>();
                }
                else if (key == FileUploadConstants.FileUploadService.IPFS_UPLOAD)
                {
                    return serviceProvider.GetService<IpfsUploadService>();
                }

                throw new KeyNotFoundException();
            });
        }
    }
}