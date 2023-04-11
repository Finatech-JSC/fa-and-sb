using MicroBase.Entity;
using MicroBase.Entity.Accounts;
using MicroBase.Entity.Repositories;
using MicroBase.RedisProvider;
using MicroBase.Share.Constants;
using MicroBase.Share.DataAccess;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models;
using MicroBase.Share.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace MicroBase.Service.Accounts
{
    public interface ITwoFactorAuthenService : IGenericService<TwoFactorAuthen, Guid>
    {
        Task<BaseResponse<object>> Enable2FAGoogleAuthenAsync(Guid accountId,
            string secretKey,
            string verifyCode,
            string via,
            IEnumerable<TwoFaRequestModel> twoFaRequestModels);

        Task<BaseResponse<object>> Disable2FAGoogleAuthenAsync(Guid accountId,
            string verifyCode,
            string via,
            IEnumerable<TwoFaRequestModel> twoFaRequestModels);

        Task<BaseResponse<RegisterTwoFaEmailResponseModel>> Register2FAEmailAsync(RegisterTwoFaEmailModel model, Guid accountId);

        Task<BaseResponse<object>> Enable2FAEmailAsync(Guid accountId,
            string validateToken,
            string otp,
            string sessionId,
            string via,
            IEnumerable<TwoFaRequestModel> twoFaRequestModels);

        Task<BaseResponse<RegisterTwoFaEmailResponseModel>> GetSession2FAEmailAsync(Guid accountId);

        Task<BaseResponse<object>> Disable2FAEmailAsync(Guid accountId,
            string validateToken,
            string otp,
            string sessionId,
            string via,
            IEnumerable<TwoFaRequestModel> twoFaRequestModels);

        Task<BaseResponse<GoogleAuthenConfigResponse>> GetGoogleAuthenSettingAsync(Guid accountId);
    }

    public class TwoFactorAuthenService : GenericService<TwoFactorAuthen, Guid>, ITwoFactorAuthenService
    {
        private readonly IAccountManageService accountManageService;
        private readonly ISiteSettingService siteSettingService;
        private readonly IGoogleAuthenticatorService googleAuthenticatorService;
        private readonly IRepository<IdentityUserActivity, Guid> identityUserActivityRepo;
        private readonly IOtpTokenService otpTokenService;
        private readonly ILogger<TwoFactorAuthenService> logger;
        private readonly IRedisStogare redisStogare;
        private readonly IRepository<TwoFactorAuthen, Guid> repository;
        private readonly MicroDbContext microDbContext;

        public TwoFactorAuthenService(IRepository<TwoFactorAuthen,
            Guid> repository,
            IAccountManageService accountManageService,
            ISiteSettingService siteSettingService,
            IGoogleAuthenticatorService googleAuthenticatorService,
            IRepository<IdentityUserActivity, Guid> identityUserActivityRepo,
            IOtpTokenService otpTokenService,
            ILogger<TwoFactorAuthenService> logger,
            IRedisStogare redisStogare,
            MicroDbContext microDbContext)
            : base(repository)
        {
            this.accountManageService = accountManageService;
            this.siteSettingService = siteSettingService;
            this.googleAuthenticatorService = googleAuthenticatorService;
            this.identityUserActivityRepo = identityUserActivityRepo;
            this.otpTokenService = otpTokenService;
            this.logger = logger;
            this.redisStogare = redisStogare;
            this.repository = repository;
            this.microDbContext = microDbContext;
        }

        protected override void ApplyDefaultSort(FindOptions<TwoFactorAuthen> findOptions)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse<object>> Enable2FAGoogleAuthenAsync(Guid accountId,
            string secretKey,
            string verifyCode,
            string via,
            IEnumerable<TwoFaRequestModel> twoFaRequestModels)
        {
            try
            {
                var account = await GetAccountInfoAsync(accountId);
                if (account == null)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.ACCOUNT_DOES_NOT_EXISTS,
                        MessageCode = nameof(CommonMessage.Account.ACCOUNT_DOES_NOT_EXISTS)
                    };
                }

                var config = await siteSettingService
                    .GetByKeyAsync<GoogleAuthenticatorModel>(key: Constants.SiteSettings.Keys.GOOGLE_AUTHENTICATOR,
                        fieldData: Constants.SiteSettings.Fields.StringValue,
                        isFromDb: true);
                if (config == null)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.RECORD_NOT_FOUND,
                        MessageCode = nameof(CommonMessage.RECORD_NOT_FOUND)
                    };
                }

                var twoFaSettings = account.IdentityUserTwoFAs.Select(s => new TwoFaSetting
                {
                    Setting = s.Setting,
                    TwoFaServiceCode = s.TwoFactorService
                });

                var check2FA = await CheckOther2FAActiveForEnable2FAService(twoFaSettings: twoFaSettings,
                    twoFaRequestModels: twoFaRequestModels,
                    enableServiceCode: Constants.Account.TwoFAService.GOOGLE_AUTHENTICATOR.ToString());
                if (!check2FA.Success)
                {
                    return check2FA;
                }

                var isCodeValid = googleAuthenticatorService.VerifyCode(secretKey, verifyCode);
                if (!isCodeValid)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.TWO_FA_CODE_INVALID,
                        MessageCode = nameof(CommonMessage.Account.TWO_FA_CODE_INVALID)
                    };
                }

                var twoFactorAuthens = new AccountGoogleAuthenticatorModel
                {
                    Issuer = config.Issuer,
                    AccountTitleNoSpaces = account.UserName.ToLower(),
                    SecretKey = secretKey
                };

                using (var scope = new TransactionScope())
                {
                    Repository.Insert(new TwoFactorAuthen
                    {
                        Id = Guid.NewGuid(),
                        IdentityUserId = accountId,
                        TwoFactorService = Constants.Account.TwoFAService.GOOGLE_AUTHENTICATOR.ToString(),
                        Setting = twoFactorAuthens.JsonSerialize(),
                        CreatedDate = DateTime.UtcNow.UtcToVietnamTime(),
                    });

                    identityUserActivityRepo.Insert(new IdentityUserActivity
                    {
                        Id = Guid.NewGuid(),
                        Action = "Enable 2FA",
                        Description = "Enable 2FA by Google Authenticator",
                        CreatedDate = DateTime.UtcNow,
                        IdentityUserId = accountId,
                        Via = via
                    });

                    scope.Complete();
                }

                return new BaseResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.Account.TWO_FA_GOOGLE_ENABLE_SUCCESSFULLY,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_GOOGLE_ENABLE_SUCCESSFULLY)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseResponse<object>> Disable2FAGoogleAuthenAsync(Guid accountId,
            string verifyCode,
            string via,
            IEnumerable<TwoFaRequestModel> twoFaRequestModels)
        {
            try
            {
                var account = await accountManageService.GetAccountByIdAsync(accountId);
                if (account == null)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.UN_AUTHORIZE,
                        MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                    };
                }

                var enable2FAService = Constants.Account.TwoFAService.GOOGLE_AUTHENTICATOR.ToString();
                var check2FA = await CheckOther2FAActiveForDisable2FAService(account.TwoFASettings, twoFaRequestModels, enable2FAService);
                if (!check2FA.Success)
                {
                    return check2FA;
                }

                var config2FA = account.TwoFASettings.FirstOrDefault(s => s.TwoFaServiceCode == enable2FAService);
                if (config2FA == null)
                {
                    logger.LogError("Disable2FAGoogleAuthenAsync dont get config");
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.NOT_FOUND,
                        MessageCode = nameof(CommonMessage.Account.NOT_FOUND)
                    };
                }

                var setting2FA = JsonExtensions.JsonDeserialize<AccountGoogleAuthenticatorModel>(config2FA.Setting);
                var isCodeValid = googleAuthenticatorService.VerifyCode(setting2FA.SecretKey, verifyCode);
                if (!isCodeValid)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.TWO_FA_CODE_INVALID,
                        MessageCode = nameof(CommonMessage.Account.TWO_FA_CODE_INVALID)
                    };
                }

                var twoFAEntity = await Repository.FindOneAsync(s => s.IdentityUserId == accountId && s.TwoFactorService == enable2FAService);
                using (var scope = new TransactionScope())
                {
                    if (twoFAEntity != null)
                    {
                        Repository.Delete(twoFAEntity);
                    }

                    identityUserActivityRepo.Insert(new IdentityUserActivity
                    {
                        Id = Guid.NewGuid(),
                        Action = "Disable 2FA",
                        Description = "Disable 2FA by Google Authenticator",
                        CreatedDate = DateTime.UtcNow.UtcToVietnamTime(),
                        IdentityUserId = accountId,
                        Via = via
                    });

                    scope.Complete();
                }

                return new BaseResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.Account.TWO_FA_GOOGLE_DISABLED_SUCCESSFULLY,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_GOOGLE_DISABLED_SUCCESSFULLY)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseResponse<RegisterTwoFaEmailResponseModel>> Register2FAEmailAsync(RegisterTwoFaEmailModel model, Guid accountId)
        {
            var account = await accountManageService.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return new BaseResponse<RegisterTwoFaEmailResponseModel>
                {
                    Success = false,
                    Message = CommonMessage.Account.UN_AUTHORIZE,
                    MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                };
            }

            var twoFAService = account.TwoFASettings.FirstOrDefault(s => s.TwoFaServiceCode == Constants.Account.TwoFAService.EMAIL.ToString());
            if (twoFAService != null)
            {
                return new BaseResponse<RegisterTwoFaEmailResponseModel>
                {
                    Success = false,
                    Message = CommonMessage.Account.TWO_FA_EMAIL_ALREADY_ENABLE,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_EMAIL_ALREADY_ENABLE)
                };
            }

            var checkExist = await repository.FindOneAsync(s => !s.IsDelete
                && s.Setting == model.Email.ToLower()
                && s.TwoFactorService == Constants.Account.TwoFAService.EMAIL.ToString());
            if (checkExist != null)
            {
                return new BaseResponse<RegisterTwoFaEmailResponseModel>
                {
                    Success = false,
                    Message = CommonMessage.Account.TWO_FA_EMAIL_DUPLICATE,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_EMAIL_DUPLICATE)
                };
            }

            var otp = await otpTokenService.SendOtp2FAAsync(accountId, model.Email);
            if (!otp.Success)
            {
                return new BaseResponse<RegisterTwoFaEmailResponseModel>
                {
                    Success = false,
                    Message = otp.Message,
                    MessageCode = otp.MessageCode,
                    MsgParams = otp.MsgParams
                };
            }

            var twoSessionId = Guid.NewGuid().ToString();
            var cache2FARequest = new RegisterTwoFaEmailToCacheModel
            {
                Email = model.Email,
                AccountId = accountId
            };

            await redisStogare.SetAsync(twoSessionId, cache2FARequest, TimeSpan.FromMinutes(5));

            return new BaseResponse<RegisterTwoFaEmailResponseModel>
            {
                Success = true,
                Data = new RegisterTwoFaEmailResponseModel
                {
                    SessionId = twoSessionId,
                    Token = otp.Data.Token,
                    OtpType = otp.Data.OtpType,
                    SendTo = otp.Data.SendTo,
                    EmailRegistration = StringExtension.MaskEmail(model.Email)
                }
            };
        }

        public async Task<BaseResponse<object>> Enable2FAEmailAsync(Guid accountId,
            string validateToken,
            string otp,
            string sessionId,
            string via,
            IEnumerable<TwoFaRequestModel> twoFaRequestModels)
        {
            try
            {
                var account = await accountManageService.GetAccountByIdAsync(accountId);
                if (account == null)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.UN_AUTHORIZE,
                        MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                    };
                }

                var enable2FAEmailCache = await redisStogare.GetAsync<RegisterTwoFaEmailToCacheModel>(sessionId);
                if (enable2FAEmailCache == null)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.TWO_FA_SESSION_EXPIRED,
                        MessageCode = nameof(CommonMessage.Account.TWO_FA_SESSION_EXPIRED),
                        Code = (int)Constants.Account.ResponseCode.TwoFASessionExpired
                    };
                }

                var check2FA = await CheckOther2FAActiveForEnable2FAService(account.TwoFASettings, twoFaRequestModels, Constants.Account.TwoFAService.EMAIL.ToString());
                if (!check2FA.Success)
                {
                    return check2FA;
                }

                var isCodeValid = await otpTokenService.VerifyOtpAsync(otp, validateToken, Constants.OtpType.TWO_FA_CODE, true, enable2FAEmailCache.Email);
                if (isCodeValid == null || !isCodeValid.Success)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = isCodeValid.Message,
                        MessageCode = isCodeValid.MessageCode
                    };
                }

                using (var scope = new TransactionScope())
                {
                    Repository.Insert(new TwoFactorAuthen
                    {
                        Id = Guid.NewGuid(),
                        IdentityUserId = accountId,
                        TwoFactorService = Constants.Account.TwoFAService.EMAIL.ToString(),
                        Setting = enable2FAEmailCache.Email.ToLower(),
                        CreatedDate = DateTime.UtcNow.UtcToVietnamTime()
                    });

                    identityUserActivityRepo.Insert(new IdentityUserActivity
                    {
                        Id = Guid.NewGuid(),
                        Action = "Enable 2FA",
                        Description = "Enable 2FA by Email",
                        CreatedDate = DateTime.UtcNow.UtcToVietnamTime(),
                        IdentityUserId = accountId,
                        Via = via
                    });

                    scope.Complete();
                }

                await redisStogare.KeyDelAsync(sessionId);

                return new BaseResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.Account.TWO_FA_EMAIL_ENABLE_SUCCESSFULLY,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_EMAIL_ENABLE_SUCCESSFULLY)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponse<RegisterTwoFaEmailResponseModel>> GetSession2FAEmailAsync(Guid accountId)
        {
            var userCache = await accountManageService.GetAccountByIdAsync(accountId);
            if (userCache == null)
            {
                return new BaseResponse<RegisterTwoFaEmailResponseModel>
                {
                    Success = false,
                    Message = CommonMessage.Account.UN_AUTHORIZE,
                    MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE)
                };
            }

            var email2FaService = userCache.TwoFASettings
                .FirstOrDefault(s => s.TwoFaServiceCode == Constants.Account.TwoFAService.EMAIL.ToString());
            if (email2FaService == null)
            {
                return new BaseResponse<RegisterTwoFaEmailResponseModel>
                {
                    Success = false,
                    Message = CommonMessage.Account.TWO_FA_EMAIL_NOT_ENABLE,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_EMAIL_NOT_ENABLE)
                };
            }

            var otp = await otpTokenService.SendOtp2FAAsync(accountId, email2FaService.Setting);
            if (!otp.Success)
            {
                return new BaseResponse<RegisterTwoFaEmailResponseModel>
                {
                    Success = false,
                    Message = otp.Message,
                    MessageCode = otp.MessageCode,
                    MsgParams = otp.MsgParams
                };
            }

            var twoSessionId = Guid.NewGuid().ToString();
            var cache2FARequest = new RegisterTwoFaEmailToCacheModel
            {
                Email = email2FaService.Setting,
                AccountId = accountId
            };

            await redisStogare.SetAsync(twoSessionId, cache2FARequest, TimeSpan.FromMinutes(5));

            return new BaseResponse<RegisterTwoFaEmailResponseModel>
            {
                Success = true,
                Data = new RegisterTwoFaEmailResponseModel
                {
                    SessionId = twoSessionId,
                    Token = otp.Data.Token,
                    OtpType = otp.Data.OtpType,
                    SendTo = otp.Data.SendTo
                }
            };
        }

        public async Task<BaseResponse<object>> Disable2FAEmailAsync(Guid accountId,
            string validateToken,
            string otp,
            string sessionId,
            string via,
            IEnumerable<TwoFaRequestModel> twoFaRequestModels)
        {
            try
            {
                var account = await accountManageService.GetAccountByIdAsync(accountId);
                if (account == null)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.NOT_FOUND,
                        MessageCode = nameof(CommonMessage.Account.NOT_FOUND)
                    };
                }

                var enable2FAEmailCache = await redisStogare.GetAsync<RegisterTwoFaEmailToCacheModel>(sessionId);
                if (enable2FAEmailCache == null)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.TWO_FA_SESSION_EXPIRED,
                        MessageCode = nameof(CommonMessage.Account.TWO_FA_SESSION_EXPIRED),
                        Code = (int)Constants.Account.ResponseCode.TwoFASessionExpired
                    };
                }

                var enable2FAService = Constants.Account.TwoFAService.EMAIL.ToString();
                var check2FA = await CheckOther2FAActiveForDisable2FAService(account.TwoFASettings, twoFaRequestModels, enable2FAService);
                if (!check2FA.Success)
                {
                    return check2FA;
                }

                var twoFAEmailService = account.TwoFASettings.FirstOrDefault(s => s.TwoFaServiceCode == enable2FAService);
                var verifyOtpRes = await otpTokenService.VerifyOtpAsync(otp: otp,
                    validateToken: validateToken,
                    otpType: Constants.OtpType.TWO_FA_CODE,
                    removeAfterVerify: true,
                    emailRegistration: twoFAEmailService?.Setting);
                if (verifyOtpRes == null || !verifyOtpRes.Success)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = verifyOtpRes == null ? CommonMessage.Account.TWO_FA_CODE_INVALID : verifyOtpRes.Message,
                        MessageCode = verifyOtpRes == null ? nameof(CommonMessage.Account.TWO_FA_CODE_INVALID) : verifyOtpRes.MessageCode
                    };
                }

                var twoFAEntity = await Repository
                    .FindOneAsync(s => s.IdentityUserId == accountId && s.TwoFactorService == enable2FAService);
                using (var scope = new TransactionScope())
                {
                    if (twoFAEntity != null)
                    {
                        Repository.Delete(twoFAEntity);
                    }

                    identityUserActivityRepo.Insert(new IdentityUserActivity
                    {
                        Id = Guid.NewGuid(),
                        Action = "Disable 2FA",
                        Description = "Disable 2FA by Email",
                        CreatedDate = DateTime.UtcNow.UtcToVietnamTime(),
                        IdentityUserId = accountId,
                        Via = via
                    });

                    scope.Complete();
                }

                await redisStogare.KeyDelAsync(sessionId);
                return new BaseResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.Account.TWO_FA_EMAIL_DISABLED_SUCCESSFULLY,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_EMAIL_DISABLED_SUCCESSFULLY)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseResponse<GoogleAuthenConfigResponse>> GetGoogleAuthenSettingAsync(Guid accountId)
        {
            try
            {
                var account = await accountManageService.GetByIdAsync(accountId);
                if (account == null)
                {
                    return new BaseResponse<GoogleAuthenConfigResponse>
                    {
                        Success = false,
                        Message = CommonMessage.Account.NOT_FOUND,
                        MessageCode = nameof(CommonMessage.Account.NOT_FOUND)
                    };
                }

                var config = await siteSettingService.GetByKeyAsync<GoogleAuthenticatorModel>(key: Constants.SiteSettings.Keys.GOOGLE_AUTHENTICATOR,
                    fieldData: Constants.SiteSettings.Fields.StringValue,
                    isFromDb: true);
                if (config == null)
                {
                    return new BaseResponse<GoogleAuthenConfigResponse>
                    {
                        Success = false,
                        Message = CommonMessage.RECORD_NOT_FOUND,
                        MessageCode = nameof(CommonMessage.RECORD_NOT_FOUND)
                    };
                }

                var secretKey = otpTokenService.GenerateRandomCharacter(len: 16);
                var setup = googleAuthenticatorService.GetSetupData(secretKey, config.Issuer, account.NormalizedEmail.ToLower());

                return setup;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<BaseResponse<object>> CheckOther2FAActiveForEnable2FAService(IEnumerable<TwoFaSetting> twoFaSettings,
            IEnumerable<TwoFaRequestModel> twoFaRequestModels,
            string enableServiceCode)
        {
            if (twoFaSettings == null || !twoFaSettings.Any())
            {
                return new BaseResponse<object>
                {
                    Success = true
                };
            }

            var twoFAService = twoFaSettings.FirstOrDefault(s => s.TwoFaServiceCode == enableServiceCode);
            if (twoFAService != null)
            {
                string message = string.Empty,
                    messageCode = string.Empty;

                if (enableServiceCode == Constants.Account.TwoFAService.EMAIL.ToString())
                {
                    message = CommonMessage.Account.TWO_FA_EMAIL_ALREADY_ENABLE;
                    messageCode = nameof(CommonMessage.Account.TWO_FA_EMAIL_ALREADY_ENABLE);
                }
                else if (enableServiceCode == Constants.Account.TwoFAService.GOOGLE_AUTHENTICATOR.ToString())
                {
                    message = CommonMessage.Account.TWO_FA_GOOGLE_ALREADY_ENABLE;
                    messageCode = nameof(CommonMessage.Account.TWO_FA_GOOGLE_ALREADY_ENABLE);
                }
                else if (enableServiceCode == Constants.Account.TwoFAService.SMS.ToString())
                {
                    message = CommonMessage.Account.TWO_FA_SMS_ALREADY_ENABLE;
                    messageCode = nameof(CommonMessage.Account.TWO_FA_SMS_ALREADY_ENABLE);
                }

                return new BaseResponse<object>
                {
                    Success = false,
                    Message = message,
                    MessageCode = messageCode
                };
            }

            var new2FAService = twoFaSettings.Where(s => s.TwoFaServiceCode != enableServiceCode).ToList();
            if (!new2FAService.Any() && (twoFaRequestModels == null || !twoFaRequestModels.Any()))
            {
                return new BaseResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.Account.TWO_FA_CODE_VALID,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_CODE_VALID),
                };
            }

            if ((!new2FAService.Any() && twoFaRequestModels.Any()) || (new2FAService.Any() && !twoFaRequestModels.Any()))
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.TWO_FA_CODE_INVALID,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_CODE_INVALID),
                };
            }

            var verifyOtp = await otpTokenService.VerifyOtp2FAAsync(new2FAService, twoFaRequestModels, true);

            return new BaseResponse<object>
            {
                Success = verifyOtp.Success,
                Message = verifyOtp.Message,
                MessageCode = verifyOtp.MessageCode,
                MsgParams = verifyOtp.MsgParams
            };
        }

        private async Task<BaseResponse<object>> CheckOther2FAActiveForDisable2FAService(IEnumerable<TwoFaSetting> twoFaSettings,
            IEnumerable<TwoFaRequestModel> twoFaRequestModels,
            string enableService)
        {
            TwoFaSetting twoFAService = null;
            if (twoFaSettings != null && twoFaSettings.Any())
            {
                twoFAService = twoFaSettings.FirstOrDefault(s => s.TwoFaServiceCode == enableService);
            }

            if (twoFaSettings == null || !twoFaSettings.Any() || twoFAService == null)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.TWO_FA_NOT_ENABLE,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_NOT_ENABLE)
                };
            }

            var other2FAService = twoFaSettings.Where(s => s.TwoFaServiceCode != enableService).ToList();
            if (!other2FAService.Any() && (twoFaRequestModels == null || !twoFaRequestModels.Any()))
            {
                return new BaseResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.Account.TWO_FA_CODE_VALID,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_CODE_VALID),
                };
            }

            if ((!other2FAService.Any() && twoFaRequestModels.Any()) || (other2FAService.Any() && !twoFaRequestModels.Any()))
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.TWO_FA_CODE_INVALID,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_CODE_INVALID),
                };
            }

            var verifyOtp = await otpTokenService.VerifyOtp2FAAsync(other2FAService, twoFaRequestModels);

            return new BaseResponse<object>
            {
                Success = verifyOtp.Success,
                Message = verifyOtp.Message,
                MessageCode = verifyOtp.MessageCode,
                MsgParams = verifyOtp.MsgParams
            };
        }

        private async Task<IdentityUser> GetAccountInfoAsync(Guid accountId)
        {
            var account = await microDbContext.Set<IdentityUser>()
                .Include(s => s.IdentityUserTwoFAs.Where(t => !t.IsDelete))
                .FirstOrDefaultAsync(s => s.Id == accountId);

            return account;
        }
    }
}