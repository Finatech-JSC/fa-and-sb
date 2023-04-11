using MicroBase.Share.Extensions;
using MicroBase.RedisProvider;
using MicroBase.Service.Accounts;
using MicroBase.Share.Models;
using MicroBase.Share.Models.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using MicroBase.BaseMvc.Extensions;
using MicroBase.Share.Constants;

namespace MicroBase.BaseMvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ServiceAuthorization : Attribute, IAsyncAuthorizationFilter
    {
        public bool AuthenRequired { get; set; } = true;

        private static HttpClient httpClient = null;

        public ServiceAuthorization()
        {
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
        {
            if (filterContext == null)
            {
                return;
            }

            var services = filterContext.HttpContext.RequestServices;
            if (httpClient == null)
            {
                CreateHttpClient(services);
            }

            var loggerFactory = services.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<ServiceAuthorization>();
            var jwtService = services.GetService<IJwtService>();

            try
            {
                var tokenResData = GetBearerAuthenToken(filterContext, logger);
                if (!tokenResData.Success)
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    filterContext.Result = new JsonResult("Unauthorized")
                    {
                        Value = new BaseResponse<string>
                        {
                            Success = false,
                            Message = "Unauthorized"
                        },
                    };

                    return;
                }

                if (httpClient.BaseAddress == null)
                {
                    CreateHttpClient(services);
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResData.Data);

                var urlPath = "api-v1/accounts/validate-login-token";
                var tokenRes = await httpClient.GetRequestAsync<UserTokenModel>(urlPath);
                if (tokenRes.Success == false || !tokenRes.Data.IsTokenValid)
                {
                    if (AuthenRequired)
                    {
                        logger.LogError($"ServiceAuthorization Bearer token invalid {tokenRes.JsonSerialize()}");

                        if (tokenRes.MessageCode == nameof(CommonMessage.UN_DETECTED_ERROR))
                        {
                            FallBackValidateToken(filterContext, jwtService, logger);
                            return;
                        }

                        filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        filterContext.Result = new JsonResult("Unauthorized")
                        {
                            Value = new BaseResponse<object>
                            {
                                Success = false,
                                Message = "Unauthorized"
                            },
                        };
                    }

                    return;
                }

                var redisService = services.GetService<IRedisStogare>();
                var key = UserExtensions.GetBlacklistKey(tokenRes.Data.UserInfo.Id.ToString());
                var isBlacklisted = await redisService.GetAsync<string>(key);

                if (!string.IsNullOrWhiteSpace(isBlacklisted))
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    filterContext.Result = new JsonResult("Unauthorized")
                    {
                        Value = new BaseResponse<object>
                        {
                            Success = false,
                            Message = "Unauthorized"
                        },
                    };
                }

                filterContext.SetClaims(tokenRes.Data.UserInfo);
                return;
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception {ex}");

                FallBackValidateToken(filterContext, jwtService, logger);
                return;
            }
        }

        private void FallBackValidateToken(AuthorizationFilterContext filterContext,
            IJwtService jwtService,
            ILogger<ServiceAuthorization> logger)
        {
            var tokenResData = GetBearerAuthenToken(filterContext, logger);
            if (!tokenResData.Success)
            {
                logger.LogError($"FallBackValidateToken get jwt token faield {tokenResData.JsonSerialize()} bearer {tokenResData.Data}");

                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                filterContext.Result = new JsonResult("Unauthorized")
                {
                    Value = new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Unauthorized"
                    },
                };

                return;
            }

            var tokenData = jwtService.ValidateJwtToken(tokenResData.Data);
            if (!tokenData.IsTokenValid)
            {
                logger.LogError($"FallBackValidateToken jwt token invalid {tokenResData.JsonSerialize()} bearer {tokenResData.Data}");
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                filterContext.Result = new JsonResult("Unauthorized")
                {
                    Value = new BaseResponse<object>
                    {
                        Success = false,
                        Message = "Unauthorized"
                    },
                };
            }

            filterContext.SetClaims(tokenData.UserInfo);
        }

        private static void CreateHttpClient(IServiceProvider services)
        {
            var configuration = services.GetService<IConfiguration>();
            var httpClientFactory = services.GetService<IHttpClientFactory>();
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(configuration.GetValue<string>("IdentityServer:EndPoint"));
        }

        private BaseResponse<string> GetBearerAuthenToken(AuthorizationFilterContext filterContext, ILogger<ServiceAuthorization> logger)
        {
            try
            {
                var token = string.Empty;
                if (filterContext.HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    token = filterContext.HttpContext.Request.Headers["Authorization"]
                        .ToString()
                        .Replace("Bearer ", "");
                }

                if (string.IsNullOrWhiteSpace(token) && AuthenRequired)
                {
                    logger.LogError("Bearer token null");
                    return new BaseResponse<string>
                    {
                        Success = false
                    };
                }

                return new BaseResponse<string>
                {
                    Success = true,
                    Data = token
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return new BaseResponse<string>
                {
                    Success = false
                };
            }
        }
    }
}