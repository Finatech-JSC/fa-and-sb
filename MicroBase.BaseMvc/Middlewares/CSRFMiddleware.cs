using System.Net;
using System.Text;
using System.Web;
using MicroBase.Service;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models;
using MicroBase.Share.Models.SiteSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static MicroBase.Share.Constants.Constants;

namespace MicroBase.BaseMvc.Middlewares
{
    public class CSRFMiddleware : IAsyncActionFilter
    {
        private readonly ILogger<CSRFMiddleware> logger;
        private readonly IConfiguration configuration;
        private readonly ISiteSettingService siteSettingService;

        public CSRFMiddleware(ILogger<CSRFMiddleware> logger,
            IConfiguration configuration,
            ISiteSettingService siteSettingService)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.siteSettingService = siteSettingService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string secretKey = string.Empty;
            bool csrfApiConfigEnabled = false;
            string csrfToken = string.Empty,
                accessTimestamp = string.Empty;

            var config = await GetSiteConfigAsync();
            if (config != null)
            {
                csrfApiConfigEnabled = config.Enabled;
                secretKey = config.SecretKey;
            }

            if (csrfApiConfigEnabled)
            {
                if (!ValidateRequestHeader(context.HttpContext, ref csrfToken, ref accessTimestamp))
                {
                    logger.LogError($"=================ValidateRequestHeader FAILED=====================");

                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Result = new JsonResult("BadRequest")
                    {
                        Value = new BaseResponse<object>
                        {
                            Success = false,
                            Message = "BadRequest"
                        },
                    };
                    return;
                }

                if (!ValidateAllowTimestamp(accessTimestamp, config.MinTime, config.MaxTime))
                {
                    logger.LogError($"=================ValidateAllowTimestamp FAILED=====================");
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Result = new JsonResult("BadRequest")
                    {
                        Value = new BaseResponse<object>
                        {
                            Success = false,
                            Message = "BadRequest"
                        },
                    };
                    return;
                }

                if (!(await ValidateRequestSignAsync(context.HttpContext, secretKey, csrfToken, accessTimestamp)))
                {
                    logger.LogError($"=================ValidateRequestSignAsync FAILED=====================");

                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Result = new JsonResult("BadRequest")
                    {
                        Value = new BaseResponse<object>
                        {
                            Success = false,
                            Message = "BadRequest"
                        },
                    };
                    return;
                }
            }

            await next();
        }

        private bool ValidateRequestHeader(HttpContext httpContext,
            ref string csrfToken,
            ref string accessTimestamp)
        {
            if (httpContext == null)
            {
                logger.LogError("httpContext null");
                return false;
            }

            csrfToken = httpContext.Request.Headers[HttpHeaderKey.CSRF_TOKEN];
            if (string.IsNullOrWhiteSpace(csrfToken))
            {
                logger.LogError("csrfToken null");
                return false;
            }

            accessTimestamp = httpContext.Request.Headers[HttpHeaderKey.TIMESTAMP];
            if (string.IsNullOrWhiteSpace(accessTimestamp))
            {
                logger.LogError("accessTimestamp null");
                return false;
            }

            return true;
        }

        private bool ValidateAllowTimestamp(string accessTimestamp, int minTime, int maxTime)
        {
            var utcNow = DateTime.UtcNow;
            var minTimestamp = utcNow.AddSeconds(minTime);
            var maxTimestamp = utcNow.AddSeconds(maxTime);
            var timestamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(int.Parse(accessTimestamp)).ToLocalTime();
            logger.LogError($"ValidateAllowTimestamp - minTimestamp: {minTimestamp}");
            logger.LogError($"ValidateAllowTimestamp - maxTimestamp: {maxTimestamp}");
            logger.LogError($"ValidateAllowTimestamp - timestamp: {timestamp}");
            logger.LogError($"ValidateAllowTimestamp - compareTime: {timestamp >= minTimestamp && timestamp <= maxTimestamp}");

            if (timestamp >= minTimestamp && timestamp <= maxTimestamp)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> ValidateRequestSignAsync(HttpContext httpContext, string secretKey, string csrfToken, string accessTimestamp)
        {
            var req = httpContext.Request;
            var fullUrl = $"https://{req.Host.Value}{req.Path}";
            var requestUri = HttpUtility.UrlEncode(fullUrl.ToLower());
            var requestBody = await ReadBodyAsStringAsync(req);

            var dataRaw = accessTimestamp + req.Method + requestUri + requestBody;
            var hmas = CryptographyProviderService.GetHmacInHex(secretKey, dataRaw);
            logger.LogError($"ValidateRequestSignAsync - fullUrl: {fullUrl}");
            logger.LogError($"ValidateRequestSignAsync - requestUri: {requestUri}");
            logger.LogError($"ValidateRequestSignAsync - requestBody: {requestBody}");
            logger.LogError($"ValidateRequestSignAsync - dataRaw: {dataRaw}");
            logger.LogError($"ValidateRequestSignAsync - hmas: {hmas}");

            var valid = csrfToken.Equals(hmas, StringComparison.Ordinal);
            if (!valid)
            {
                logger.LogError($"Signature validate failed API Signature {hmas} client Signature {csrfToken}");
            }

            return valid;
        }

        private async Task<string> ReadBodyAsStringAsync(HttpRequest request)
        {
            var initialBody = request.Body;
            string body = string.Empty;
            int count = 0;
            while (string.IsNullOrEmpty(body) && count < 100)
            {
                try
                {
                    HttpRequestRewindExtensions.EnableBuffering(request);
                    using (StreamReader reader = new StreamReader(
                        request.Body,
                        Encoding.UTF8,
                        detectEncodingFromByteOrderMarks: false,
                        leaveOpen: true))
                    {
                        body = await reader.ReadToEndAsync();

                        // IMPORTANT: Reset the request body stream position so the next middleware can read it
                        request.Body.Position = 0;
                    }
                }
                catch (Exception e)
                {
                    logger.LogError("Exception e:", e);
                }
                finally
                {
                    // Workaround so MVC action will be able to read body as well
                    request.Body = initialBody;
                    count++;
                }
            }
            return body;
        }

        private async Task<CSRFMiddlewareModel> GetSiteConfigAsync()
        {
            var configKey = configuration.GetValue<string>("CSRFApi:ConfigKey");
            var config = await siteSettingService.GetByKeyAsync<CSRFMiddlewareModel>(configKey, SiteSettings.Fields.StringValue, false);

            return config;
        }
    }
}