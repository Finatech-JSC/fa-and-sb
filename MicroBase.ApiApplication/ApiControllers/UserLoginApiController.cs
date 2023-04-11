using MicroBase.BaseMvc.Middlewares;
using MicroBase.Service.Accounts;
using MicroBase.Share.Models.Accounts;
using MicroBase.Share.Models;
using Microsoft.AspNetCore.Mvc;
using MicroBase.Share.Extensions;
using MicroBase.BaseMvc.Apis;
using MicroBase.Service.Localizations;
using MicroBase.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MicroBase.Share.Constants;

namespace MicroBase.ApiApplication.ApiControllers
{
    [ApiController]
    [Route("user-login-api")]
    public class UserLoginApiController : BaseApiController
    {
        private readonly IAccountLoginService accountLoginService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<UserLoginApiController> logger;
        private readonly IJwtService jwtService;

        public UserLoginApiController(ILoggerFactory loggerFactory,
            ILocalizationService localizationService,
            IHttpContextAccessor httpContextAccessor,
            IExceptionMonitorService exceptionMonitorService,
            IAccountLoginService accountLoginService,
            ILogger<UserLoginApiController> logger,
            IJwtService jwtService)
            : base(loggerFactory, localizationService, httpContextAccessor, exceptionMonitorService)
        {
            this.accountLoginService = accountLoginService;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
            this.jwtService = jwtService;
        }

        [HttpPost("login")]
        [TypeFilter(typeof(CSRFMiddleware), Arguments = new object[] { })]
        public async Task<BaseResponse<LoginResponse>> Login(LoginModel model)
        {
            model.Via = httpContextAccessor.HttpContext.GetVia();
            var cultureCode = httpContextAccessor.HttpContext.GetRequestCultureCode();

            var res = await accountLoginService.LoginAsync(model, cultureCode);
            return LocalizationBaseResponse(res);
        }

        [HttpPost("validate-two-fa-token/{twoSessionId}")]
        public async Task<BaseResponse<LoginResponse>> ValidateLoginTwoFaAsync(string twoSessionId,
            IEnumerable<TwoFaRequestModel> twoFaRequests)
        {
            var res = await accountLoginService.ValidateLoginTwoFaAsync(twoSessionId, twoFaRequests);
            return LocalizationBaseResponse(res);
        }

        [HttpGet("validate-login-token")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<UserTokenModel> ValidateLoginToken()
        {
            var token = httpContextAccessor.HttpContext.GetBearerToken();
            var isValid = jwtService.ValidateToken(token);
            if (!isValid)
            {
                logger.LogError("ValidateLoginToken invalid token");
                return new UserTokenModel
                {
                    IsTokenValid = isValid,
                    UserInfo = null
                };
            }

            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
            var via = httpContextAccessor.HttpContext.User.GetVia();
            if (!accountId.HasValue || string.IsNullOrWhiteSpace(via))
            {
                logger.LogError($"ValidateLoginToken accountId: {accountId} via: {via}");
                return new UserTokenModel
                {
                    IsTokenValid = isValid,
                    UserInfo = null
                };
            }

            var accountType = httpContextAccessor.HttpContext.User.GetAccountType();
            var userName = httpContextAccessor.HttpContext.User.GetUserName();

            return new UserTokenModel
            {
                IsTokenValid = true,
                UserInfo = new LoginResponse
                {
                    Id = accountId.Value,
                    UserName = userName,
                    AccountType = accountType,
                    Via = via
                }
            };
        }

        [HttpPost("logout")]
        public async Task<object> Logout()
        {
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
            var res = await accountLoginService.LogoutAsync(accountId.Value);

            return LocalizationBaseResponse(res);
        }

        [HttpPost("change-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<BaseResponse<object>> ChangePassword(ChangePasswordModel model)
        {
            model.Via = httpContextAccessor.HttpContext.GetVia();
            model.Action = Constants.Account.ActivityAction.ChangePassword;

            var userName = httpContextAccessor.HttpContext.User.GetUserName();
            var res = await accountLoginService.ChangePasswordAsync(userName, model);

            return LocalizationBaseResponse(res);
        }

        [HttpPost("reset-password")]
        public async Task<BaseResponse<OtpResponse>> ResetPassword(ResetPasswordModel model)
        {
            model.Via = httpContextAccessor.HttpContext.GetVia();
            var res = await accountLoginService.ResetPasswordAsync(model);

            return LocalizationBaseResponse(res);
        }

        [HttpPost("confirm-reset-password")]
        public async Task<BaseResponse<Guid>> ConfirmResetChangePassword(ConfirmPasswordModel model)
        {
            model.Via = httpContextAccessor.HttpContext.GetVia();
            model.Action = Constants.Account.ActivityAction.ConfirmResetPassword;

            var res = await accountLoginService.ConfirmResetPasswordAsync(model);
            return LocalizationBaseResponse(res);
        }

        [HttpPost("update-user-profile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<BaseResponse<Guid>>> UpdateUserProfile([FromForm] UpdateAccountProfileModel model)
        {
            model.Via = httpContextAccessor.HttpContext.GetVia();
            var userName = httpContextAccessor.HttpContext.User.GetUserName();
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();

            var res = await accountLoginService.UpdateAccountProfileV1Async(userName, model);
            if (res.Success)
            {
                
            }

            return LocalizationBaseResponse(res);
        }
    }
}