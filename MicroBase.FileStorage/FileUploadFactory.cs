namespace MicroBase.FileStorage
{
    public class FileUploadFactory
    {
        private readonly FileUploadServiceResolver fileUploadServiceResolver;

        public FileUploadFactory(FileUploadServiceResolver fileUploadServiceResolver)
        {
            this.fileUploadServiceResolver = fileUploadServiceResolver;
        }

        public IFileUploadService GetServiceByName(string serviceName)
        {
            return fileUploadServiceResolver(serviceName);
        }
    }
}