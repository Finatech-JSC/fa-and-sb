using MicroBase.Share.Extensions;
using MicroBase.Service.Accounts;
using MicroBase.Share.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Http;
using MicroBase.BaseMvc.Extensions;
using MicroBase.Share.Constants;

namespace MicroBase.BaseMvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class InternalAuthorization : Attribute, IAsyncAuthorizationFilter
    {
        public bool AuthenRequired { get; set; } = true;

        public InternalAuthorization()
        {
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
        {
            if (filterContext == null)
            {
                return;
            }

            var services = filterContext.HttpContext.RequestServices;
            var loggerFactory = services.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<InternalAuthorization>();
            var jwtService = services.GetService<IJwtService>();
            var httpContextAccessor = services.GetService<IHttpContextAccessor>();

            try
            {
                var token = httpContextAccessor.HttpContext.GetBearerToken();
                var userTokenModel = jwtService.ValidateJwtToken(token);

                if (!userTokenModel.IsTokenValid && AuthenRequired)
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    filterContext.Result = new JsonResult("Unauthorized")
                    {
                        Value = new BaseResponse<string>
                        {
                            Success = false,
                            Message = CommonMessage.Account.UN_AUTHORIZE
                        },
                    };

                    return;
                }

                if (userTokenModel.UserInfo == null)
                {
                    return;
                }

                filterContext.SetClaims(identityUserId: userTokenModel.UserInfo.Id,
                    userName: userTokenModel.UserInfo.UserName,
                    accountType: userTokenModel.UserInfo.AccountType,
                    via: userTokenModel.UserInfo.Via,
                    isConfirmed: userTokenModel.UserInfo.IsConfirmed);

                return;
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception {ex}");
                return;
            }
        }
    }
}