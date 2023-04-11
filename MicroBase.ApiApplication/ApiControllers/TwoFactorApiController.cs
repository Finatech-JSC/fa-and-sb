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
    [Route("two-factor-api")]
    public class TwoFactorApiController : BaseApiController
    {
        private readonly IAccountLoginService accountLoginService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<TwoFactorApiController> logger;
        private readonly IJwtService jwtService;
        private readonly ITwoFactorAuthenService twoFactorAuthenService;

        public TwoFactorApiController(ILoggerFactory loggerFactory,
            ILocalizationService localizationService,
            IHttpContextAccessor httpContextAccessor,
            IExceptionMonitorService exceptionMonitorService,
            IAccountLoginService accountLoginService,
            ILogger<TwoFactorApiController> logger,
            IJwtService jwtService)
            : base(loggerFactory, localizationService, httpContextAccessor, exceptionMonitorService)
        {
            this.accountLoginService = accountLoginService;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
            this.jwtService = jwtService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("2fa-google-authen-config")]
        public async Task<BaseResponse<GoogleAuthenConfigResponse>> Get2FAGoogleAuthenConfig()
        {
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
            if (!accountId.HasValue)
            {
                return LocalizationBaseResponse(new BaseResponse<GoogleAuthenConfigResponse>
                {
                    Success = false,
                    Message = CommonMessage.Account.UN_AUTHORIZE,
                    MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                });
            }

            var res = await twoFactorAuthenService.GetGoogleAuthenSettingAsync(accountId.Value);
            return LocalizationBaseResponse(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("enable-2fa-google-authen")]
        public async Task<BaseResponse<object>> Enable2FAGoogleAuthen(GoogleAuthenSettingRequest request)
        {
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
            if (!accountId.HasValue)
            {
                return LocalizationBaseResponse(new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.UN_AUTHORIZE,
                    MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                });
            }

            var via = httpContextAccessor.HttpContext.User.GetVia();
            var res = await twoFactorAuthenService.Enable2FAGoogleAuthenAsync(accountId: accountId.Value,
                secretKey: request.GoogleSecretKey,
                verifyCode: request.VerifyCode,
                via: via,
                twoFaRequestModels: request.TwoFaRequestModels);
            return LocalizationBaseResponse(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("disable-2fa-google-authen")]
        public async Task<BaseResponse<object>> Disable2FAGoogleAuthen(TwoFaGoogleModel request)
        {
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
            if (!accountId.HasValue)
            {
                return LocalizationBaseResponse(new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.UN_AUTHORIZE,
                    MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                });
            }

            var via = httpContextAccessor.HttpContext.User.GetVia();
            var res = await twoFactorAuthenService.Disable2FAGoogleAuthenAsync(accountId: accountId.Value,
                verifyCode: request.VerifyCode,
                via: via,
                twoFaRequestModels: request.TwoFaRequestModels);
            return LocalizationBaseResponse(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("register-2fa-email")]
        public async Task<BaseResponse<RegisterTwoFaEmailResponseModel>> Register2faEmail(RegisterTwoFaEmailModel request)
        {
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
            if (!accountId.HasValue)
            {
                return LocalizationBaseResponse(new BaseResponse<RegisterTwoFaEmailResponseModel>
                {
                    Success = false,
                    Message = CommonMessage.Account.UN_AUTHORIZE,
                    MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                });
            }

            var via = httpContextAccessor.HttpContext.User.GetVia();
            var res = await twoFactorAuthenService.Register2FAEmailAsync(request, accountId.Value);
            return LocalizationBaseResponse(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("enable-2fa-email")]
        public async Task<BaseResponse<object>> Enable2FaEmail(TwoFaEmailModel request)
        {
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
            if (!accountId.HasValue)
            {
                return LocalizationBaseResponse(new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.UN_AUTHORIZE,
                    MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                });
            }

            var via = httpContextAccessor.HttpContext.User.GetVia();
            var res = await twoFactorAuthenService.Enable2FAEmailAsync(accountId: accountId.Value,
                validateToken: request.Token,
                otp: request.VerifyCode,
                sessionId: request.SessionId,
                via: via,
                twoFaRequestModels: request.TwoFaRequestModels);
            return LocalizationBaseResponse(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("get-session-2fa-email")]
        public async Task<BaseResponse<RegisterTwoFaEmailResponseModel>> GetSession2FaEmail()
        {
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
            if (!accountId.HasValue)
            {
                return LocalizationBaseResponse(new BaseResponse<RegisterTwoFaEmailResponseModel>
                {
                    Success = false,
                    Message = CommonMessage.Account.UN_AUTHORIZE,
                    MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                });
            }

            var res = await twoFactorAuthenService.GetSession2FAEmailAsync(accountId.Value);
            return LocalizationBaseResponse(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("disable-2fa-email")]
        public async Task<BaseResponse<object>> Disable2FAEmail(TwoFaEmailModel request)
        {
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
            if (!accountId.HasValue)
            {
                return LocalizationBaseResponse(new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.UN_AUTHORIZE,
                    MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                });
            }

            var via = httpContextAccessor.HttpContext.User.GetVia();
            var res = await twoFactorAuthenService.Disable2FAEmailAsync(accountId: accountId.Value,
                validateToken: request.Token,
                otp: request.VerifyCode,
                sessionId: request.SessionId,
                via: via,
                twoFaRequestModels: request.TwoFaRequestModels);
            return LocalizationBaseResponse(res);
        }
    }
}