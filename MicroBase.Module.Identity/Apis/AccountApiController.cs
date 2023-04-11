using MicroBase.BaseMvc.Apis;
using MicroBase.BaseMvc.Middlewares;
using MicroBase.RabbitMQ;
using MicroBase.Service.Accounts;
using MicroBase.Service.Emails;
using MicroBase.Service.Localizations;
using MicroBase.Share;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models;
using MicroBase.Share.Models.Accounts;
using MicroBase.Share.Models.Accounts.Trackings;
using MicroBase.Telegram;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MicroBase.Module.IdentityApi.Apis
{
    [ApiController]
    [Route("api-v1/accounts")]
    public class AccountApiController : BaseApiController
    {
        private readonly IAccountLoginService accountLoginService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IJwtService jwtService;
        private readonly ISocialAccountService socialAccountService;
        private readonly IConfiguration configuration;
        private readonly IMessageBusService messageBusService;
        private readonly IEmailFilterService emailFilterService;
        private readonly IExceptionMonitorService exceptionMonitorService;
        private readonly ILogger<AccountApiController> logger;

        public AccountApiController(ILoggerFactory loggerFactory,
            ILocalizationService localizationService,
            IHttpContextAccessor httpContextAccessor,
            IExceptionMonitorService exceptionMonitorService,
            IAccountLoginService accountService,
            IJwtService jwtService,
            ISocialAccountService socialAccountService,
            IConfiguration configuation,
            IMessageBusService messageBusService,
            IEmailFilterService emailFilterService,
            IHttpClientFactory httpClientFactory,
            ILogger<AccountApiController> logger)
            : base(loggerFactory, localizationService, httpContextAccessor, exceptionMonitorService)
        {
            this.accountLoginService = accountService;
            this.httpContextAccessor = httpContextAccessor;
            this.jwtService = jwtService;
            this.socialAccountService = socialAccountService;
            this.configuration = configuation;
            this.messageBusService = messageBusService;
            this.emailFilterService = emailFilterService;
            this.exceptionMonitorService = exceptionMonitorService;
            this.logger = logger;
        }

        [HttpPost("register")]
        [TypeFilter(typeof(CSRFMiddleware), Arguments = new object[] { })]
        public async Task<BaseResponse<OtpResponse>> Register(RegisterModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.UserName)
                && model.UserName.IsEmailAddress()
                && model.UserName.Contains("+"))
            {
                return LocalizationBaseResponse(new BaseResponse<OtpResponse>
                {
                    Success = false,
                    Message = CommonMessage.EMAIL_ADDRESS_INVALID,
                    MessageCode = nameof(CommonMessage.EMAIL_ADDRESS_INVALID)
                });
            }

            var isAcceptEmail = await emailFilterService.IsAcceptEmailAsync(model.UserName);
            if (!isAcceptEmail)
            {
                return LocalizationBaseResponse(new BaseResponse<OtpResponse>
                {
                    Success = false,
                    Message = CommonMessage.EMAIL_ADDRESS_INVALID,
                    MessageCode = nameof(CommonMessage.EMAIL_ADDRESS_INVALID)
                });
            }

            var actions = new List<Constants.Account.RegisterAction>
            {
                Constants.Account.RegisterAction.SendEmailAfterRegister
            };

            var requiredConfirmEmailOrPhone = configuration.GetValue<bool>("AppConfig:RequiredConfirmEmailOrPhone");
            if (requiredConfirmEmailOrPhone)
            {
                actions.Add(Constants.Account.RegisterAction.MustConfirmEmailOrPhone);
            }

            var trackingMode = new AccountTrackingModel
            {
                Via = httpContextAccessor.HttpContext.GetVia()
            };

            var res = await accountLoginService.RegisterAsync(Constants.Account.Type.Normal, model, trackingMode, actions);
            if (res.Success)
            {
                var eventMessage = new SystemUserReportModel
                {
                    DailyAttendance = 0,
                    NormalRegister = 1,
                    SocialRegister = 0,
                    EmailConfirm = 0,
                    PhoneConfirm = 0
                };

                await messageBusService.SendAsync(QueuesConstants.UpdateSystemUserReport, eventMessage);
            }

            return LocalizationBaseResponse(new BaseResponse<OtpResponse>
            {
                Success = res.Success,
                Code = res.Code,
                Data = res.Data?.OtpResponse,
                Message = res.Message,
                MessageCode = res.MessageCode,
                Errors = res.Errors
            });
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

        [HttpPost("web3-login")]
        [TypeFilter(typeof(CSRFMiddleware), Arguments = new object[] { })]
        public async Task<BaseResponse<LoginResponse>> Web3Login(Web3RegisterModel model)
        {
            var tracking = GetTracking(Constants.Account.ActivityAction.Web3Login.ToString(), string.Empty);

            var res = await accountLoginService.Web3LoginAsync(model, tracking);
            return LocalizationBaseResponse(res);
        }

        [HttpPost("validate-two-fa-token/{twoSessionId}")]
        public async Task<BaseResponse<LoginResponse>> ValidateLoginTwoFaAsync(string twoSessionId,
            IEnumerable<TwoFaRequestModel> twoFaRequests)
        {
            var res = await accountLoginService.ValidateLoginTwoFaAsync(twoSessionId, twoFaRequests);
            return LocalizationBaseResponse(res);
        }

        [HttpPost("social-login")]
        [TypeFilter(typeof(CSRFMiddleware), Arguments = new object[] { })]
        public async Task<BaseResponse<LoginResponse>> SocialLogin(SocialLoginModel model)
        {
            model.Via = httpContextAccessor.HttpContext.GetVia();
            var res = await socialAccountService.VerifyLoginTokenAsync(model);
            if (!res.Success
                || (!string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(res.Data.Email) && model.Email != res.Data.Email))
            {
                return LocalizationBaseResponse(new BaseResponse<LoginResponse>
                {
                    Success = false,
                    Message = CommonMessage.Account.LOGIN_FAILED,
                    MessageCode = nameof(CommonMessage.Account.LOGIN_FAILED)
                });
            }

            if (!string.IsNullOrWhiteSpace(res.Data.Email))
            {
                model.Email = res.Data.Email;
            }

            if (!string.IsNullOrWhiteSpace(res.Data.Avatar))
            {
                model.Avatar = res.Data.Avatar;
            }

            var loginRes = await accountLoginService.LoginByExternalAccountAsync(model);
            return LocalizationBaseResponse(loginRes);
        }

        [HttpGet("validate-login-token")]
        [Authorize(Policy = "WhitelistPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            if (!accountId.HasValue || !string.IsNullOrWhiteSpace(via))
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
            var isConfirmed = httpContextAccessor.HttpContext.User.IsAccountConfirmed();

            return new UserTokenModel
            {
                IsTokenValid = true,
                UserInfo = new LoginResponse
                {
                    Id = accountId.Value,
                    UserName = userName,
                    AccountType = accountType,
                    Via = via,
                    IsConfirmed = isConfirmed
                }
            };
        }

        [HttpPost("logout")]
        [Authorize(Policy = "WhitelistPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<object> Logout()
        {
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
            var res = await accountLoginService.LogoutAsync(accountId.Value);

            return LocalizationBaseResponse(res);
        }

        [HttpPost("change-password")]
        [Authorize(Policy = "WhitelistPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [Authorize(Policy = "WhitelistPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<BaseResponse<Guid>>> UpdateUserProfileV1([FromForm] UpdateAccountProfileModel model)
        {
            model.Via = httpContextAccessor.HttpContext.GetVia();
            var userName = httpContextAccessor.HttpContext.User.GetUserName();
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();

            var res = await accountLoginService.UpdateAccountProfileV1Async(userName, model);
            return LocalizationBaseResponse(res);
        }

        [HttpPost("/api-v2/accounts/update-user-profile")]
        [Authorize(Policy = "WhitelistPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<BaseResponse<Guid>>> UpdateUserProfile([FromForm] UpdateAccountProfileModel model)
        {
            model.Via = httpContextAccessor.HttpContext.GetVia();
            var userName = httpContextAccessor.HttpContext.User.GetUserName();
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();

            var res = await accountLoginService.UpdateAccountProfileAsync(userName, model);
            return LocalizationBaseResponse(res);
        }

        [HttpGet("user-profile")]
        [Authorize(Policy = "WhitelistPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<BaseResponse<AccountProfileModel>> GetUserProfile()
        {
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
            var cultureCode = httpContextAccessor.HttpContext.GetRequestCultureCode();

            var res = await accountLoginService.GetAccountProfileAsync(accountId.Value, cultureCode);
            return LocalizationBaseResponse(res);
        }

        [HttpGet("user-profile/{referralId}")]
        public async Task<BaseResponse<AccountProfileModel>> GetUserProfile(string referralId)
        {
            if (string.IsNullOrWhiteSpace(referralId))
            {
                return LocalizationBaseResponse(new BaseResponse<AccountProfileModel>
                {
                    Success = false,
                    Message = CommonMessage.Account.UN_AUTHORIZE,
                    MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                });
            }

            var res = await accountLoginService.GetAccountByReferralIdAsync(referralId);
            return LocalizationBaseResponse(res);
        }

        [HttpPost("change-default-password")]
        [Authorize(Policy = "WhitelistPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<BaseResponse<object>>> ChangeDefaultPassword(ChangePassowrdModel model)
        {
            model.Via = httpContextAccessor.HttpContext.GetVia();
            var userName = httpContextAccessor.HttpContext.User.GetUserName();

            var res = await accountLoginService.ChangeDefaultPasswordAsync(userName, model);

            var activityDesc = res.Success ? "Change default password successfully" : "Change default password failed";
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();

            return LocalizationBaseResponse(res);
        }

        //[HttpGet("get-user-referral-summary")]
        //[Authorize(Policy = "WhitelistPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[AllowAnonymous]
        //public async Task<BaseResponse<ReferralSummaryModel>> GetUserReferralSummary()
        //{
        //    var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
        //    if (!accountId.HasValue)
        //    {
        //        return LocalizationBaseResponse(new BaseResponse<ReferralSummaryModel>
        //        {
        //            Success = false,
        //            Message = CommonMessage.Account.UN_AUTHORIZE,
        //            MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
        //        });
        //    }

        //    var res = await accountLoginService.GetReferralSummaryAsync(accountId.Value);
        //    return LocalizationBaseResponse(res);
        //}

        //[HttpGet("/api-v2/accounts/get-user-referral-summary")]
        //[Authorize(Policy = "WhitelistPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[AllowAnonymous]
        //public async Task<BaseResponse<ReferralSummaryModel>> GetUserReferralSummaryV2()
        //{
        //    try
        //    {
        //        var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
        //        if (!accountId.HasValue)
        //        {
        //            return LocalizationBaseResponse(new BaseResponse<ReferralSummaryModel>
        //            {
        //                Success = false,
        //                Message = CommonMessage.Account.UN_AUTHORIZE,
        //                MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
        //            });
        //        }

        //        var res = await accountLoginService.GetReferralSummaryV2Async(accountId.Value);
        //        return LocalizationBaseResponse(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        await exceptionMonitorService.SendExceptionNotiAsync("/api-v2/accounts/get-user-referral-summary", ex);
        //        return new BaseResponse<ReferralSummaryModel>
        //        {
        //            Success = false,
        //            Message = CommonMessage.UN_DETECTED_ERROR,
        //            MessageCode = nameof(CommonMessage.UN_DETECTED_ERROR)
        //        };
        //    }
        //}

        [HttpGet("get-invited-user")]
        [Authorize(Policy = "WhitelistPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<BaseResponse<List<InvitedUserModel>>>> GetInvitedUser(int pageIndex = 1, int pageSize = 10)
        {
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();
            if (!accountId.HasValue)
            {
                return LocalizationBaseResponse(new BaseResponse<List<InvitedUserModel>>
                {
                    Success = false,
                    Message = CommonMessage.Account.UN_AUTHORIZE,
                    MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                });
            }

            var res = await accountLoginService.GetInvitedUserAsync(accountId.Value, pageIndex, pageSize);
            return LocalizationBaseResponse(res);
        }

        [HttpPost("notification-setting")]
        [Authorize(Policy = "WhitelistPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<BaseResponse<object>>> AccountNotificationSetting(AccountNotificationSettingModel model)
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

            var res = await accountLoginService.AccountNotificationSettingAsync(accountId.Value, model);
            return LocalizationBaseResponse(res);
        }
    }
}