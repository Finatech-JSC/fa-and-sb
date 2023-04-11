using MicroBase.Entity.Accounts;
using MicroBase.Entity.Repositories;
using MicroBase.Share.Extensions;
using MicroBase.RabbitMQ;
using MicroBase.RedisProvider;
using MicroBase.Share;
using MicroBase.Share.Models;
using MicroBase.Share.Models.Accounts;
using Microsoft.Extensions.Logging;
using MicroBase.Share.Constants;

namespace MicroBase.Service.Accounts
{
    public interface IOtpTokenService
    {
        Task<BaseResponse<object>> VerifyOtpAsync(string otp,
            string validateToken,
            Constants.OtpType otpType,
            bool removeAfterVerify,
            string emailRegistration = null,
            string accountId = null);

        string GenerateRandomCharacter(int len,
            bool isNumberOnly = false,
            bool isUpperLetter = false,
            bool isLowerLetter = false);

        Task<BaseResponse<OtpResponse>> GetOtpAsync(Constants.OtpType otpType,
            string userName,
            TimeSpan? timeSpan = null);

        Task<BaseResponse<OtpResponse>> SendOtp2FAAsync(Guid accountId, string userName);

        Task<BaseResponse<OtpResponse>> ProcessSendOtpAsync(Guid accountId,
            string username,
            Constants.OtpType otpType,
            TimeSpan? timeSpan = null);

        Task<BaseResponse<OtpResponse>> GetLoginOtp2FAAsync(string sessionId);

        Task<BaseResponse<OtpResponse>> GetRegisterOtp2FAAsync(string sessionId);

        Task<BaseResponse<bool>> VerifyOtp2FAAsync(IEnumerable<TwoFaSetting> twoFactorAuthens,
            IEnumerable<TwoFaRequestModel> twoFaRequests,
            bool isCheckAllActive = false);
    }

    public class OtpTokenService : IOtpTokenService
    {
        private readonly ILogger<OtpTokenService> logger;
        private readonly IRedisStogare redisStogare;
        private readonly IRepository<IdentityUser, Guid> identityUserRepo;
        private readonly IRepository<IdentityUserMetaData, Guid> identityUserMetaDataRepo;
        private readonly IMessageBusService messageBusService;
        private readonly IGoogleAuthenticatorService googleAuthenticatorService;

        public OtpTokenService(ILogger<OtpTokenService> logger,
            IRedisStogare redisStogare,
            IRepository<IdentityUser, Guid> identityUserRepo,
            IRepository<IdentityUserMetaData, Guid> identityUserMetaDataRepo,
            IMessageBusService messageBusService,
            IGoogleAuthenticatorService googleAuthenticatorService)
        {
            this.logger = logger;
            this.redisStogare = redisStogare;
            this.identityUserRepo = identityUserRepo;
            this.identityUserMetaDataRepo = identityUserMetaDataRepo;
            this.messageBusService = messageBusService;
            this.googleAuthenticatorService = googleAuthenticatorService;
        }

        /// <summary>
        /// Generate OTP token
        /// </summary>
        /// <param name="len">Length of OTP</param>
        /// <param name="saveForValidate">Save OTP has been create for valide</param>
        /// <param name="validateToken">The token to validate the OTP</param>
        /// <returns></returns>
        public async Task<string> GenerateOtpAsync(int len,
            bool saveForValidate,
            string validateToken,
            Constants.OtpType otpType,
            string email,
            Guid? accountId = null,
            TimeSpan? timeSpan = null)
        {
            var chars = "01234567890";
            var arrs = new char[len];
            var rd = new Random();

            for (int i = 0; i < arrs.Length; i++)
            {
                arrs[i] = chars[rd.Next(chars.Length)];
            }

            var otp = new string(arrs);
            if (accountId.HasValue)
            {
                otp = $"{otp}|{accountId.Value}";
            }

            if (saveForValidate)
            {
                var key = $"{otpType}:{validateToken}";
                if (otpType == Constants.OtpType.TWO_FA_CODE)
                {
                    key = $"{otpType}{email}:{validateToken}";
                }

                if (!timeSpan.HasValue)
                {
                    timeSpan = TimeSpan.FromMinutes(1);
                }

                await redisStogare.SetAsync(key, otp, timeSpan);
            }

            return new string(arrs);
        }

        public string GenerateRandomCharacter(int len,
            bool isNumberOnly = false,
            bool isUpperLetter = false,
            bool isLowerLetter = false)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (isNumberOnly)
            {
                chars = "1234567890";
            }

            if (isUpperLetter)
            {
                chars = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }

            if (isLowerLetter)
            {
                chars = "abcdefghijklmnopqrstuvwxyz1234567890";
            }

            var arrs = new char[len];
            var rd = new Random();
            for (int i = 0; i < arrs.Length; i++)
            {
                arrs[i] = chars[rd.Next(chars.Length)];
            }

            return new string(arrs);
        }

        public async Task<string> GenerateReferralCodeAsync()
        {
            var i = 0;
            while (i <= 5)
            {
                i++;

                var referralCode = GenerateRandomCharacter(8, false, true, false);
                var exist = await redisStogare.HGetAsync<string>("USER_REFERRAL", referralCode);
                if (string.IsNullOrWhiteSpace(exist))
                {
                    return referralCode;
                }
            }

            return GenerateRandomCharacter(8, false, true, false);
        }

        public async Task<bool> IsReferralCodeExistAsync(string referralCode)
        {
            var exist = await redisStogare.HExistsAsync("USER_REFERRAL", referralCode);
            return exist;
        }

        public async Task<BaseResponse<object>> VerifyOtpAsync(string otp,
            string validateToken,
            Constants.OtpType otpType,
            bool removeAfterVerify,
            string emailRegistration = null,
            string accountId = null)
        {
            var key = $"{otpType}:{validateToken}";
            if (otpType == Constants.OtpType.TWO_FA_CODE)
            {
                if (string.IsNullOrWhiteSpace(emailRegistration))
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.TWO_FA_CODE_INVALID,
                        MessageCode = nameof(CommonMessage.Account.TWO_FA_CODE_INVALID)
                    };
                }

                key = $"{otpType}{emailRegistration}:{validateToken}";
            }

            var antiBruteForceRes = await AntiBruteForceOtpAsync(key);
            if (!antiBruteForceRes.Success)
            {
                return antiBruteForceRes;
            }

            var cacheOtp = await redisStogare.GetAsync<string>(key);
            if (string.IsNullOrWhiteSpace(cacheOtp))
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Otp.VERIFY_OTP_FAILED,
                    MessageCode = nameof(CommonMessage.Otp.VERIFY_OTP_FAILED)
                };
            }

            var arrs = cacheOtp.Split('|');
            if (accountId != null && accountId != arrs[1])
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Otp.VERIFY_OTP_FAILED,
                    MessageCode = nameof(CommonMessage.Otp.VERIFY_OTP_FAILED)
                };
            }

            if (otp == arrs[0])
            {
                if (removeAfterVerify)
                {
                    await redisStogare.KeyDelAsync(key);
                }

                return new BaseResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.Otp.VERIFY_OTP_SUCCESS,
                    MessageCode = nameof(CommonMessage.Otp.VERIFY_OTP_SUCCESS),
                    Data = arrs.Length > 1 ? arrs[1] : string.Empty
                };
            }

            return new BaseResponse<object>
            {
                Success = false,
                Message = CommonMessage.Otp.VERIFY_OTP_FAILED,
                MessageCode = nameof(CommonMessage.Otp.VERIFY_OTP_FAILED)
            };
        }

        /// <summary>
        /// Avoid Brute Force attack
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<BaseResponse<object>> AntiBruteForceOtpAsync(string key)
        {
            var antiKey = $"ANTI_SCAN_OTP:{key}";
            var antiScanOtp = await redisStogare.GetAsync<DateTime>(antiKey);
            if (antiScanOtp != null && antiScanOtp != DateTime.MinValue)
            {
                var subTime = DateTime.UtcNow.Subtract(antiScanOtp).TotalSeconds;
                if (subTime < 10)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.TOO_MANY_REQUESTS,
                        MessageCode = nameof(CommonMessage.TOO_MANY_REQUESTS)
                    };
                }
            }

            await redisStogare.SetAsync(antiKey, DateTime.UtcNow, TimeSpan.FromMinutes(5));
            return new BaseResponse<object>
            {
                Success = true
            };
        }

        public async Task<BaseResponse<OtpResponse>> GetOtpAsync(Constants.OtpType otpType, string userName, TimeSpan? timeSpan = null)
        {

            var user = await identityUserRepo.FindOneAsync(s => s.NormalizedUserName == userName.ToUpper());
            if (user == null)
            {
                return new BaseResponse<OtpResponse>
                {
                    Success = false,
                    Message = CommonMessage.RECORD_NOT_FOUND,
                    MessageCode = nameof(CommonMessage.RECORD_NOT_FOUND)
                };
            }

            var userNameIsEmail = UserExtensions.IsEmailAddress(userName);
            var userNameIsPhone = UserExtensions.IsPhoneNumber(userName, "SG");
            var res = await ProcessSendOtpAsync(user.Id, userName, otpType, timeSpan);
            return res;
        }

        public async Task<BaseResponse<OtpResponse>> GetLoginOtp2FAAsync(string sessionId)
        {
            var userCache = await redisStogare.GetAsync<LoginResponse>(sessionId);
            if (userCache == null)
            {
                return new BaseResponse<OtpResponse>
                {
                    Success = false,
                    Message = CommonMessage.Account.TWO_FA_SESSION_EXPIRED,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_SESSION_EXPIRED),
                    Code = (int)Constants.Account.ResponseCode.TwoFASessionExpired
                };
            }

            var email2FaService = userCache.TwoFASettings
                .FirstOrDefault(s => s.TwoFaServiceCode == Constants.Account.TwoFAService.EMAIL.ToString());
            if (email2FaService == null)
            {
                return new BaseResponse<OtpResponse>
                {
                    Success = false,
                    Message = CommonMessage.Account.TWO_FA_EMAIL_NOT_ENABLE,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_EMAIL_NOT_ENABLE)
                };
            }

            return await SendOtp2FAAsync(userCache.Id, email2FaService.Setting);
        }

        public async Task<BaseResponse<OtpResponse>> GetRegisterOtp2FAAsync(string sessionId)
        {
            var model = await redisStogare.GetAsync<RegisterTwoFaEmailToCacheModel>(sessionId);
            if (model == null)
            {
                return new BaseResponse<OtpResponse>
                {
                    Success = false,
                    Message = CommonMessage.Account.TWO_FA_SESSION_EXPIRED,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_SESSION_EXPIRED),
                    Code = (int)Constants.Account.ResponseCode.TwoFASessionExpired
                };
            }

            return await SendOtp2FAAsync(model.AccountId, model.Email);
        }

        public async Task<BaseResponse<OtpResponse>> SendOtp2FAAsync(Guid accountId, string userName)
        {
            var user = await identityUserRepo.FindOneAsync(s => s.Id == accountId);
            if (user == null)
            {
                return new BaseResponse<OtpResponse>
                {
                    Success = false,
                    Message = CommonMessage.RECORD_NOT_FOUND,
                    MessageCode = nameof(CommonMessage.RECORD_NOT_FOUND)
                };
            }

            var otpType = Constants.OtpType.TWO_FA_CODE;
            var res = await ProcessSendOtpAsync(user.Id, userName, otpType, null);
            return res;
        }

        public async Task<BaseResponse<OtpResponse>> ProcessSendOtpAsync(Guid accountId,
            string userName,
            Constants.OtpType otpType,
            TimeSpan? timeSpan = null)
        {
            var antiDdosGetOtpRes = await AntiDdosGetOtpAsync(accountId, otpType);
            if (!antiDdosGetOtpRes.Success)
            {
                return antiDdosGetOtpRes;
            }

            var userNameIsEmail = UserExtensions.IsEmailAddress(userName);
            var userNameIsPhone = UserExtensions.IsPhoneNumber(userName, "SG");

            var userOtpTypes = new List<Constants.OtpType>
            {
                Constants.OtpType.REGISTER_OTP,
                Constants.OtpType.RESET_PASSWORD,
                Constants.OtpType.TWO_FA_CODE
            };

            var otpLen = 6;
            if (userOtpTypes.Any(s => s == otpType))
            {
                otpLen = 4;
            }

            var tokenId = Guid.NewGuid().ToString();
            var otp = await GenerateOtpAsync(otpLen, true, tokenId, otpType, userName, accountId);

            if (userNameIsEmail)
            {
                var template = string.Empty;
                if (Constants.OtpEmailMappings.ContainsKey(otpType))
                {
                    template = Constants.OtpEmailMappings[otpType];
                }

                var cultureCode = Constants.CultureCode.UnitedStates;
                var userMetaData = await identityUserMetaDataRepo.FindOneAsync(s => s.IdentityUserId == accountId);
                if (userMetaData != null && !string.IsNullOrWhiteSpace(userMetaData.DefaultLanguage))
                {
                    cultureCode = userMetaData.DefaultLanguage.ToUpper();
                }

                var token = new Dictionary<string, string>
                {
                    { Constants.EmailTemplates.EmailTokens.USERNAME, userName },
                    { Constants.EmailTemplates.EmailTokens.OPT_CODE, otp }
                };

                var message = new EmailQueueModel
                {
                    CultureCode = cultureCode,
                    EmailTemplate = template,
                    EmailTokens = token,
                    ReceivingAddress = userName
                };

                await messageBusService.SendAsync(QueuesConstants.EmailMessage, message);
            }
            else if (userNameIsPhone)
            {
                // TODO: Process for username is phone number
            }

            if (!timeSpan.HasValue)
            {
                timeSpan = TimeSpan.FromSeconds(50);
            }

            string cacheKey = $"{otpType}:{tokenId}";
            await redisStogare.SetAsync(cacheKey, DateTime.UtcNow, timeSpan.Value);
            return new BaseResponse<OtpResponse>
            {
                Success = true,
                Message = CommonMessage.Otp.SEND_OTP_SUCCESS,
                MessageCode = nameof(CommonMessage.Otp.SEND_OTP_SUCCESS),
                Data = new OtpResponse
                {
                    Token = tokenId,
                    OtpType = otpType,
                    SendTo = userNameIsEmail
                        ? (int)CommonMessage.Otp.SendTo.Email
                        : (int)CommonMessage.Otp.SendTo.Phone
                }
            };
        }

        /// <summary>
        /// Avoid DDOS get OTP
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="otpType"></param>
        /// <returns></returns>
        private async Task<BaseResponse<OtpResponse>> AntiDdosGetOtpAsync(Guid accountId, Constants.OtpType otpType)
        {
            var cacheKey = $"{accountId}:{otpType}";
            var cacheEntry = await redisStogare.GetAsync<DateTime>(cacheKey);
            if (cacheEntry != null && cacheEntry != DateTime.MinValue && 60 - (int)DateTime.UtcNow.Subtract(cacheEntry).TotalSeconds > 0)
            {
                var delay = 60 - (int)DateTime.UtcNow.Subtract(cacheEntry).TotalSeconds;
                return new BaseResponse<OtpResponse>
                {
                    Success = false,
                    Message = CommonMessage.Otp.GET_OTP_FAILED_TOO_MANY_REQUEST,
                    MessageCode = nameof(CommonMessage.Otp.GET_OTP_FAILED_TOO_MANY_REQUEST),
                    MsgParams = new List<string> { delay.ToString() }
                };
            }

            return new BaseResponse<OtpResponse>
            {
                Success = true
            };
        }

        public async Task<BaseResponse<bool>> VerifyOtp2FAAsync(IEnumerable<TwoFaSetting> twoFactorAuthens,
            IEnumerable<TwoFaRequestModel> twoFaRequests,
            bool isCheckAllActive = false)
        {
            var count2FA = twoFactorAuthens.Count();
            if (count2FA == 0)
            {
                return new BaseResponse<bool>
                {
                    Success = false,
                    Message = CommonMessage.NO_2FA_ENABLED,
                    MessageCode = nameof(CommonMessage.NO_2FA_ENABLED),
                    Data = false
                };
            }

            var countAllActive = 0;

            foreach (var item in twoFaRequests)
            {
                if (item.TwoFaService == Constants.Account.TwoFAService.EMAIL.ToString())
                {
                    var twoEmail = twoFactorAuthens.FirstOrDefault(s => s.TwoFaServiceCode == item.TwoFaService);
                    if (twoEmail == null)
                    {
                        return new BaseResponse<bool>
                        {
                            Success = false,
                            Message = CommonMessage.Account.TWO_FA_CODE_INVALID,
                            MessageCode = nameof(CommonMessage.Account.TWO_FA_CODE_INVALID),
                            Data = false
                        };
                    }

                    var isCodeValidRes = await VerifyOtpAsync(item.Otp, item.ValidateToken, Constants.OtpType.TWO_FA_CODE, true, twoEmail.Setting);
                    if (isCodeValidRes == null || !isCodeValidRes.Success)
                    {
                        return new BaseResponse<bool>
                        {
                            Success = false,
                            Message = isCodeValidRes.Message,
                            MessageCode = isCodeValidRes.MessageCode,
                            Data = false
                        };
                    }

                    countAllActive += 1;
                }
                else if (item.TwoFaService == Constants.Account.TwoFAService.GOOGLE_AUTHENTICATOR.ToString())
                {
                    var twoFaGoogle = twoFactorAuthens.FirstOrDefault(s => s.TwoFaServiceCode == item.TwoFaService);
                    if (twoFaGoogle == null)
                    {
                        return new BaseResponse<bool>
                        {
                            Success = false,
                            Message = CommonMessage.Account.TWO_FA_CODE_INVALID,
                            MessageCode = nameof(CommonMessage.Account.TWO_FA_CODE_INVALID),
                            Data = false
                        };
                    }

                    var config = JsonExtensions.JsonDeserialize<AccountGoogleAuthenticatorModel>(twoFaGoogle.Setting);
                    var isCodeValid = googleAuthenticatorService.VerifyCode(config.SecretKey, item.Otp);
                    if (!isCodeValid)
                    {
                        return new BaseResponse<bool>
                        {
                            Success = false,
                            Message = CommonMessage.Account.TWO_FA_CODE_INVALID,
                            MessageCode = nameof(CommonMessage.Account.TWO_FA_CODE_INVALID),
                            Data = false
                        };
                    }

                    countAllActive += 1;
                }
            }

            if (countAllActive == 0 || (countAllActive != count2FA && isCheckAllActive))
            {
                return new BaseResponse<bool>
                {
                    Success = false,
                    Message = CommonMessage.Account.TWO_FA_CODE_INVALID,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_CODE_INVALID),
                    Data = false
                };
            }

            return new BaseResponse<bool>
            {
                Success = true,
                Data = true,
                Message = CommonMessage.Account.TWO_FA_CODE_VALID,
                MessageCode = nameof(CommonMessage.Account.TWO_FA_CODE_VALID),
            };
        }
    }
}
