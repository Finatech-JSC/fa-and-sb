using Microsoft.AspNetCore.Http;
using NLog;
using System.Text;

namespace MicroBase.BaseMvc.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Logger requestLog = LogManager.GetLogger("requestLog");

        public RequestLoggingMiddleware(RequestDelegate _next)
        {
            this._next = _next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                StringBuilder sbLog = new StringBuilder();
                sbLog.AppendLine($"Request {context.Request?.Method} {context.Request?.Path.Value}{context.Request?.QueryString} => statusCode: {context.Response?.StatusCode}");

                sbLog.AppendLine("Request Headers:");
                foreach (var item in context.Request.Headers)
                {
                    sbLog.AppendLine($"     {item.Key}:{item.Value}");
                }

                sbLog.AppendLine("=============");
                sbLog.AppendLine("Request Body:");

                // Log Request
                using (var reader = new StreamReader(context.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    sbLog.AppendLine($"     {requestBody}");
                }

                sbLog.AppendLine("=============");
                sbLog.AppendLine("Response Headers:");
                foreach (var item in context.Response.Headers)
                {
                    sbLog.AppendLine($"     {item.Key}:{item.Value}");
                }

                sbLog.AppendLine("=============");
                sbLog.AppendLine("Response Body:");

                // Log Response
                string responseBody = string.Empty;
                using (var swapStream = new MemoryStream())
                {
                    var originalResponseBody = context.Response.Body;
                    context.Response.Body = swapStream;
                    await _next(context);
                    swapStream.Seek(0, SeekOrigin.Begin);
                    responseBody = new StreamReader(swapStream).ReadToEnd();
                    swapStream.Seek(0, SeekOrigin.Begin);
                    await swapStream.CopyToAsync(originalResponseBody);
                    context.Response.Body = originalResponseBody;
                }

                sbLog.AppendLine($"     {responseBody}");
                sbLog.AppendLine("=============");

                requestLog.Trace(sbLog.ToString());
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}