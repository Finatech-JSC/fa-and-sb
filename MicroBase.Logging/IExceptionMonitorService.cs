using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroBase.Logging
{
    public interface IExceptionMonitorService
    {
        /// <summary>
        /// Gửi thông báo lỗi hệ thống
        /// </summary>
        /// <param name="method"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        Task SendExceptionNotiAsync(string method, Exception ex);

        /// <summary>
        /// Gửi các thông báo vận hành
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendOperationMessageAsync(string taskName, string message);
    }

    public class ExceptionMonitorService : IExceptionMonitorService
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger<ExceptionMonitorService> logger;

        public ExceptionMonitorService(IHostingEnvironment hostingEnvironment,
            ILogger<ExceptionMonitorService> logger)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }

        /// <summary>
        /// Gửi thông báo lỗi hệ thống
        /// </summary>
        /// <param name="method"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public Task SendExceptionNotiAsync(string method, Exception ex)
        {
            logger.LogError(ex.ToString());

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gửi các thông báo vận hành
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendOperationMessageAsync(string taskName, string message)
        {
            return Task.CompletedTask;
        }
    }
}