using MicroBase.Entity;
using MicroBase.Entity.Repositories;
using MicroBase.Share.Extensions;
using MicroBase.NoDependencyService;
using MicroBase.RedisProvider;
using MicroBase.Share;
using MicroBase.Share.DataAccess;
using MicroBase.Share.Models;
using MicroBase.Share.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using IdentityUser = MicroBase.Entity.Accounts.IdentityUser;
using System.Linq.Expressions;
using MicroBase.Entity.Accounts;
using MicroBase.FileStorage;
using MicroBase.RabbitMQ;
using System.Data;
using MicroBase.Service.Localizations;
using System.Transactions;
using MicroBase.FileStorage.Models;
using MicroBase.Share.Constants;

namespace MicroBase.Service.Accounts
{
    public interface IAccountLoginService : IGenericService<IdentityUser, Guid>
    {
        Task<BaseResponse<LoginResponse>> LoginAsync(LoginModel model, string cultureCode);

        Task<BaseResponse<AccountRegisterResponse>> RegisterUserExistAsync(Constants.Account.Type type,
            RegisterModel model);

        Task<BaseResponse<AccountRegisterResponse>> RegisterAsync(Constants.Account.Type type,
            RegisterModel model,
            AccountTrackingModel trackingModel,
            IReadOnlyCollection<Constants.Account.RegisterAction> actions,
            Guid? accountId = null,
            string cultureCode = null);

        Task<BaseResponse<LoginResponse>> Web3LoginAsync(Web3RegisterModel model,
            BaseActivityTrackingModel trackingModel);

        Task<BaseResponse<LoginResponse>> ValidateLoginTwoFaAsync(string twoSessionId,
            IEnumerable<TwoFaRequestModel> twoFaRequests);

        Task<bool> VerifyAccountPasswordAsync(Guid accountId, string password);

        Task<BaseResponse<object>> LogoutAsync(Guid accountId);

        Task<BaseResponse<Guid>> UpdateAccountProfileV1Async(string userName, UpdateAccountProfileModel model);

        Task<BaseResponse<Guid>> UpdateAccountProfileAsync(string userName, UpdateAccountProfileModel model);

        Task<BaseResponse<OtpResponse>> ResetPasswordAsync(ResetPasswordModel model);

        Task<BaseResponse<object>> ChangePasswordAsync(string userName, ChangePasswordModel model);

        Task<BaseResponse<Guid>> ConfirmResetPasswordAsync(ConfirmPasswordModel model);

        Task<LoginResponse> GetLoggedUserAsync(Guid accountId, Constants.Via via);

        Task<ExternalAccount> GetExternalAccountByExternalId(string externalAccountId);

        Task<BaseResponse<object>> ChangeDefaultPasswordAsync(string userName, ChangePassowrdModel model);

        Task<BaseResponse<object>> UpdateAccountAsync(Guid accountId, RegisterModel model);

        Task<IdentityUser> GetAccountAysnc(Expression<Func<IdentityUser, bool>> predicate);
    }

    public class AccountLoginService : GenericService<IdentityUser, Guid>, IAccountLoginService
    {
        private readonly ILogger<AccountLoginService> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IJwtService jwtService;
        private readonly IConfiguration configuration;
        private readonly IRedisStogare redisStogare;
        private readonly MicroDbContext microDbContext;
        private readonly IRandomService randomService;
        private readonly IOtpTokenService otpTokenService;
        private readonly IRepository<IdentityUserMetaData, Guid> identityUserMetaDataRepo;
        private readonly IRepository<ExternalAccount, Guid> externalAccountRepo;
        private readonly IFileUploadService fileUploadService;
        private readonly IMessageBusService messageBusService;
        private readonly ILocalizationService localizationService;

        private readonly string connectionString = string.Empty;

        public AccountLoginService(IRepository<IdentityUser, Guid> repository,
            ILogger<AccountLoginService> logger,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IJwtService jwtService,
            IConfiguration configuration,
            IRedisStogare redisStogare,
            MicroDbContext microDbContext,
            IRandomService randomService,
            IOtpTokenService otpTokenService,
            IRepository<IdentityUserMetaData, Guid> identityUserMetaDataRepo,
            IRepository<ExternalAccount, Guid> externalAccountRepo,
            FileUploadFactory fileUploadFactory,
            IMessageBusService messageBusService,
            ILocalizationService localizationService)
            : base(repository)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtService = jwtService;
            this.redisStogare = redisStogare;
            this.microDbContext = microDbContext;
            this.randomService = randomService;
            this.otpTokenService = otpTokenService;
            this.identityUserMetaDataRepo = identityUserMetaDataRepo;
            this.externalAccountRepo = externalAccountRepo;
            this.messageBusService = messageBusService;
            this.localizationService = localizationService;

            var uploadServiceName = configuration.GetValue<string>("FileManage:EnableService");
            this.fileUploadService = fileUploadFactory.GetServiceByName(uploadServiceName);

            connectionString = microDbContext.Database.GetDbConnection().ConnectionString;
        }

        protected override void ApplyDefaultSort(FindOptions<IdentityUser> findOptions)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse<LoginResponse>> LoginAsync(LoginModel model, string cultureCode)
        {
            try
            {
                var trackingMessage = new AccountTrackingModel
                {
                    Action = Constants.Account.ActivityAction.Login,
                    IpAddress = model.IpAddress,
                    Location = model.Location,
                    UserAgent = model.UserAgent,
                    Via = model.Via
                };

                var maxFailedAccessAttempts = configuration.GetValue<int>("IdentityUser:MaxFailedAccessAttempts");
                var lockInMinute = configuration.GetValue<int>("IdentityUser:LockInMinute");
                var signInResult = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, true);
                if (!signInResult.Succeeded)
                {
                    if (signInResult.IsLockedOut)
                    {
                        var resLocked = new BaseResponse<LoginResponse>
                        {
                            Success = false,
                            Code = (int)Constants.Account.ResponseCode.LockedOut,
                            Message = CommonMessage.Account.LOGIN_ACCOUNT_LOCKED_ACCESS_FAILED_COUNT,
                            MessageCode = nameof(CommonMessage.Account.LOGIN_ACCOUNT_LOCKED_ACCESS_FAILED_COUNT),
                            MsgParams = new List<string>
                            {
                                maxFailedAccessAttempts.ToString(),
                                lockInMinute.ToString()
                            }
                        };

                        return resLocked;
                    }

                    return new BaseResponse<LoginResponse>
                    {
                        Success = false,
                        Message = CommonMessage.Account.LOGIN_FAILED,
                        MessageCode = nameof(CommonMessage.Account.LOGIN_FAILED),
                        MsgParams = new List<string> { maxFailedAccessAttempts.ToString() }
                    };
                }

                var defaultLanguage = string.IsNullOrEmpty(cultureCode)
                    ? Constants.CultureCode.UnitedStates
                    : cultureCode.ToUpper();

                var userEntity = await GetUserByEmailAsync(model.UserName);
                if(userEntity.IsDelete == true)
                {
                    return new BaseResponse<LoginResponse>
                    {
                        Success = false,
                        Message = CommonMessage.Account.ACCOUNT_DISABLEB,
                        MessageCode = nameof(CommonMessage.Account.ACCOUNT_DISABLEB),
                        MsgParams = new List<string> { maxFailedAccessAttempts.ToString() }
                    };
                }    
                var requiredConfirmEmailOrPhone = configuration.GetValue<bool>("AppConfig:RequiredConfirmEmailOrPhone");
                if (requiredConfirmEmailOrPhone && !userEntity.EmailConfirmed && !userEntity.PhoneNumberConfirmed)
                {
                    trackingMessage.UserName = model.UserName;
                    trackingMessage.IdentityUserId = userEntity.Id;
                    trackingMessage.Description = CommonMessage.Account.LOGIN_FAILED_ACCOUNT_NOT_ALLOW;
                    await messageBusService.SendAsync(QueuesConstants.UserTrackingActivity, trackingMessage);

                    return new BaseResponse<LoginResponse>
                    {
                        Success = false,
                        Message = CommonMessage.Account.LOGIN_FAILED_ACCOUNT_NOT_ALLOW,
                        MessageCode = nameof(CommonMessage.Account.LOGIN_FAILED_ACCOUNT_NOT_ALLOW),
                        Code = (int)Constants.Account.ResponseCode.NotAllow
                    };
                }

                var checkRequired2FA = await CheckRequired2FAAsync(userEntity);
                if (!checkRequired2FA.Success)
                {
                    return new BaseResponse<LoginResponse>
                    {
                        Success = true,
                        Message = CommonMessage.Account.LOGIN_SUCCESS_REQUIRED_2FA,
                        MessageCode = nameof(CommonMessage.Account.LOGIN_SUCCESS_REQUIRED_2FA),
                        Code = (int)Constants.Account.ResponseCode.Required2FA,
                        Data = new LoginResponse
                        {
                            SessionId = checkRequired2FA.Data,
                            TwoFactorServices = userEntity.IdentityUserTwoFAs.Select(s => new TwoFaResponse
                            {
                                EmailRegistration = s.TwoFactorService == Constants.Account.TwoFAService.EMAIL.ToString()
                                    ? StringExtension.MaskEmail(s.Setting)
                                    : "",
                                TwoFaServiceCode = s.TwoFactorService,
                                TwoFaServiceName = Constants.Account.TwoFAServiceMaps[s.TwoFactorService]
                            })
                        }
                    };
                }

                return await LoginSuccessAsync(trackingMessage, userEntity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return new BaseResponse<LoginResponse>
                {
                    Success = false,
                    Message = CommonMessage.Account.UN_AUTHORIZE,
                    MessageCode = nameof(CommonMessage.Account.UN_AUTHORIZE),
                };
            }
        }

        private async Task<BaseResponse<LoginResponse>> LoginSuccessAsync(AccountTrackingModel trackingMessage,
            IdentityUser userEntity)
        {
            var res = BuildJwtLoginToken(ToLoginResponse(userEntity));

            trackingMessage.UserName = userEntity.UserName;
            trackingMessage.IdentityUserId = userEntity.Id;
            trackingMessage.Description = CommonMessage.Account.LOGIN_SUCCESS;
            await messageBusService.SendAsync(QueuesConstants.UserTrackingActivity, trackingMessage);

            return new BaseResponse<LoginResponse>
            {
                Success = true,
                Message = CommonMessage.Account.LOGIN_SUCCESS,
                MessageCode = nameof(CommonMessage.Account.LOGIN_SUCCESS),
                Data = res
            };
        }

        public async Task<BaseResponse<AccountRegisterResponse>> RegisterUserExistAsync(Constants.Account.Type type, RegisterModel model)
        {
            var account = await microDbContext.Set<IdentityUser>().FirstOrDefaultAsync(s => s.Email == model.UserName);

            if (account != null && account.IsDelete == true)
            {
                account.IsDelete = false;
                account.CreatedDate = DateTime.UtcNow;
                account.ModifiedDate = DateTime.UtcNow;
                account.FullName = model.FullName;
                account.AccountType = type.ToString();

                ChangePasswordModel changePassword = new ChangePasswordModel();
                changePassword.Password = model.Password;
                changePassword.RePassword = model.RePassword;

                await ChangePasswordAsync(account.Email, changePassword);

                Repository.Update(account);

                using (var scopre = new TransactionScope())
                {
                    Repository.Update(account);

                    scopre.Complete();
                }
            }

            return new BaseResponse<AccountRegisterResponse>
            {
                Success = true,
                Message = CommonMessage.Account.REGISTER_SUCCESSFUL,
                MessageCode = nameof(CommonMessage.Account.REGISTER_SUCCESSFUL),
                Data = new AccountRegisterResponse
                {
                    AccountId = account.Id,
                }
            };

        }

        public async Task<BaseResponse<AccountRegisterResponse>> RegisterAsync(Constants.Account.Type type,
            RegisterModel model,
            AccountTrackingModel trackingModel,
            IReadOnlyCollection<Constants.Account.RegisterAction> actions,
            Guid? accountId = null,
            string cultureCode = null)
        {
            var userNameIsEmail = UserExtensions.IsEmailAddress(model.UserName);
            var userNameIsPhone = UserExtensions.IsPhoneNumber(model.UserName, "SG");
            if (!userNameIsEmail && !userNameIsPhone)
            {
                return new BaseResponse<AccountRegisterResponse>
                {
                    Success = false,
                    Message = CommonMessage.Account.REGISTER_INVALID_USERNAME,
                    MessageCode = nameof(CommonMessage.Account.REGISTER_INVALID_USERNAME)
                };
            }

            var account = await microDbContext.Set<IdentityUser>().FirstOrDefaultAsync(s => s.Email == model.UserName);
            if (account != null && account.IsDelete == true)
            {
                await RegisterUserExistAsync(type, model);

                return new BaseResponse<AccountRegisterResponse>
                {
                    Success = true,
                    Message = CommonMessage.Account.REGISTER_SUCCESSFUL,
                    MessageCode = nameof(CommonMessage.Account.REGISTER_SUCCESSFUL),
                    Data = new AccountRegisterResponse
                    {
                        AccountId = account.Id,
                    }
                };
            }

            var id = accountId.HasValue ? accountId.Value : Guid.NewGuid();
            var accountEntity = new IdentityUser
            {
                Id = id,
                UserName = model.UserName.ToLowerInvariant(),
                Email = model.UserName,
                EmailConfirmed = false,
                Via = trackingModel.Via,
                PhoneNumber = userNameIsPhone ? model.UserName.ToLowerInvariant() : null,
                PhoneNumberConfirmed = false,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                FullName = model.FullName,
                AccountType = type.ToString()
            };

            var result = await userManager.CreateAsync(accountEntity, model.Password);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(s => s.Code.Equals("DuplicateEmail")))
                {
                    return new BaseResponse<AccountRegisterResponse>
                    {
                        Success = false,
                        Message = userNameIsEmail
                            ? CommonMessage.Account.EMAIL_ALREADY_EXISTS
                            : CommonMessage.Account.PHONENUMER_ALREADY_EXISTS,
                        MessageCode = userNameIsEmail
                            ? nameof(CommonMessage.Account.EMAIL_ALREADY_EXISTS)
                            : nameof(CommonMessage.Account.PHONENUMER_ALREADY_EXISTS)
                    };
                }

                return new BaseResponse<AccountRegisterResponse>
                {
                    Success = false,
                    Message = CommonMessage.Account.REGISTER_INVALID_EXCEPTION,
                    MessageCode = nameof(CommonMessage.Account.REGISTER_INVALID_EXCEPTION)
                };
            }

            var otpRes = await otpTokenService.ProcessSendOtpAsync(accountEntity.Id, model.UserName, Constants.OtpType.REGISTER_OTP);
            return new BaseResponse<AccountRegisterResponse>
            {
                Success = true,
                Message = CommonMessage.Account.REGISTER_SUCCESSFUL,
                MessageCode = nameof(CommonMessage.Account.REGISTER_SUCCESSFUL),
                Data = new AccountRegisterResponse
                {
                    AccountId = id,
                    OtpResponse = otpRes.Data
                }
            };
        }

        public async Task<BaseResponse<LoginResponse>> Web3LoginAsync(Web3RegisterModel model,
            BaseActivityTrackingModel trackingModel)
        {
            var trackingMessage = new AccountTrackingModel
            {
                Action = Constants.Account.ActivityAction.Login,
                IpAddress = trackingModel.IpAddress,
                Location = trackingModel.Location,
                UserAgent = trackingModel.UserAgent,
                Via = trackingModel.Via
            };

            var userEntity = await GetWeb3AccountAsync(model.WalletAddress);
            if (userEntity != null)
            {
                return await LoginSuccessAsync(trackingMessage, userEntity);
            }

            var newIdentityUserId = Guid.NewGuid();
            var referralId = await GenerateReferralCodeAsync();
            var accountEntity = new IdentityUser
            {
                Id = newIdentityUserId,
                UserName = model.WalletAddress,
                NormalizedUserName = model.WalletAddress.ToUpper(),
                Email = referralId.GenerateSystemEmail(),
                NormalizedEmail = referralId.GenerateSystemEmail().ToUpper(),
                Via = trackingModel.Via,
                CreatedDate = DateTime.UtcNow,
                AccountType = Constants.Account.Type.Normal.ToString(),
                IsDefaultPassword = true
            };

            var result = await userManager.CreateAsync(accountEntity, Guid.NewGuid().ToString());
            if (!result.Succeeded)
            {
                if (result.Errors.Any(s => s.Code.Equals("DuplicateEmail") || s.Code.Equals("DuplicateUserName")))
                {
                    userEntity = await GetWeb3AccountAsync(model.WalletAddress);
                    return await LoginSuccessAsync(trackingMessage, userEntity);
                }

                return new BaseResponse<LoginResponse>
                {
                    Success = false,
                    Message = CommonMessage.Account.REGISTER_WEB3_INVALID_EXCEPTION,
                    MessageCode = nameof(CommonMessage.Account.REGISTER_WEB3_INVALID_EXCEPTION)
                };
            }

            var externalAccount = new ExternalAccount
            {
                Id = Guid.NewGuid(),
                ExternalAccountId = model.WalletAddress,
                IdentityUserId = newIdentityUserId,
                ProviderName = model.WalletProvider,
                Platform = model.Network
            };

            await externalAccountRepo.InsertAsync(externalAccount);

            userEntity = await GetWeb3AccountAsync(model.WalletAddress);
            return await LoginSuccessAsync(trackingMessage, userEntity);
        }

        public async Task<BaseResponse<LoginResponse>> ValidateLoginTwoFaAsync(string twoSessionId,
            IEnumerable<TwoFaRequestModel> twoFaRequests)
        {
            if (!twoFaRequests.Any())
            {
                return new BaseResponse<LoginResponse>
                {
                    Success = false,
                    Message = CommonMessage.MODEL_STATE_INVALID,
                    MessageCode = nameof(CommonMessage.MODEL_STATE_INVALID)
                };
            }

            var userLoginRes = await redisStogare.GetAsync<LoginResponse>(twoSessionId);
            if (userLoginRes == null)
            {
                return new BaseResponse<LoginResponse>
                {
                    Success = false,
                    Message = CommonMessage.Account.TWO_FA_SESSION_EXPIRED,
                    MessageCode = nameof(CommonMessage.Account.TWO_FA_SESSION_EXPIRED),
                    Code = (int)Constants.Account.ResponseCode.TwoFASessionExpired
                };
            }

            var verifyTwoFaRes = await otpTokenService.VerifyOtp2FAAsync(userLoginRes.TwoFASettings, twoFaRequests);
            if (!verifyTwoFaRes.Success)
            {
                return new BaseResponse<LoginResponse>
                {
                    Success = false,
                    Message = verifyTwoFaRes.Message,
                    MessageCode = verifyTwoFaRes.MessageCode,
                    MsgParams = verifyTwoFaRes.MsgParams
                };
            }

            var isFirstTimeLogin = false;
            var entity = await userManager.FindByIdAsync(userLoginRes.Id.ToString());

            if (entity.LastLoginTime == null)
            {
                isFirstTimeLogin = true;
            }
            userLoginRes.IsFirstTimeLogin = isFirstTimeLogin;

            entity.LastLoginTime = DateTime.UtcNow;
            await userManager.UpdateAsync(entity);

            await redisStogare.KeyDelAsync(twoSessionId);

            var jwtLoginToken = BuildJwtLoginToken(userLoginRes);
            return new BaseResponse<LoginResponse>
            {
                Success = true,
                Message = CommonMessage.Account.LOGIN_SUCCESS,
                MessageCode = nameof(CommonMessage.Account.LOGIN_SUCCESS),
                Data = userLoginRes
            };
        }

        public async Task<BaseResponse<object>> LogoutAsync(Guid accountId)
        {
            string cacheKey;
            foreach (int via in Enum.GetValues(typeof(Constants.Via)))
            {
                cacheKey = GetUserSessionCacheKey(accountId, (Constants.Via)via);
                await redisStogare.KeyDelAsync(cacheKey);
            }

            cacheKey = GetUserSessionCacheKey(accountId, 0);
            await redisStogare.KeyDelAsync(cacheKey);

            return new BaseResponse<object>
            {
                Success = true,
                Message = CommonMessage.Account.LOGOUT_SUCCESS,
                MessageCode = nameof(CommonMessage.Account.LOGOUT_SUCCESS)
            };
        }

        public async Task<bool> VerifyAccountPasswordAsync(Guid accountId, string password)
        {
            var user = await userManager.FindByIdAsync(accountId.ToString());
            if (user == null)
            {
                return false;
            }

            var isValidOldPass = await userManager.CheckPasswordAsync(user, password);
            if (!isValidOldPass)
            {
                return false;
            }

            return true;
        }

        public async Task<ExternalAccount> GetExternalAccountByExternalId(string externalAccountId)
        {
            var account = await microDbContext.Set<ExternalAccount>()
                .Include(s => s.IdentityUser)
                .FirstOrDefaultAsync(s => s.ExternalAccountId == externalAccountId);

            return account;
        }

        private async Task<BaseResponse<Guid>> GetOrAddExternalAccountAsync(SocialLoginModel model)
        {
            var externalEntity = await GetExternalAccountByExternalId(model.ExternalId);
            if (externalEntity != null)
            {
                return new BaseResponse<Guid>
                {
                    Success = true,
                    Message = CommonMessage.Account.REGISTER_SUCCESSFUL,
                    MessageCode = nameof(CommonMessage.Account.REGISTER_SUCCESSFUL),
                    Data = externalEntity.IdentityUserId
                };
            }

            var externalUserName = string.Empty;
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                externalUserName = model.Email;
            }
            else if (!string.IsNullOrWhiteSpace(model.ExternalId))
            {
                externalUserName = model.ExternalId;
            }

            var userNameIsEmail = UserExtensions.IsEmailAddress(externalUserName);
            var userNameIsPhone = UserExtensions.IsPhoneNumber(externalUserName, "SG");
            var identityUser = new IdentityUser
            {
                Id = Guid.NewGuid(),
                UserName = externalUserName.ToLowerInvariant(),
                Email = userNameIsEmail ? externalUserName.ToLowerInvariant() : externalUserName.GenerateSystemEmail(),
                EmailConfirmed = userNameIsEmail ? true : false,
                Via = model.Via,
                PhoneNumber = userNameIsPhone ? externalUserName.ToLowerInvariant() : null,
                PhoneNumberConfirmed = userNameIsPhone ? true : false,
                CreatedDate = DateTime.UtcNow,
                IsDefaultPassword = true,
                AccountType = Constants.Account.Type.Normal.ToString(),
                LastLoginTime = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(identityUser, Guid.NewGuid().ToString());
            if (!result.Succeeded)
            {
                if (result.Errors.Any(s => s.Code.Equals("DuplicateEmail")))
                {
                    return new BaseResponse<Guid>
                    {
                        Success = false,
                        Message = userNameIsEmail ? CommonMessage.Account.EMAIL_ALREADY_EXISTS
                            : CommonMessage.Account.PHONENUMER_ALREADY_EXISTS,
                        MessageCode = userNameIsEmail
                            ? nameof(CommonMessage.Account.EMAIL_ALREADY_EXISTS)
                            : nameof(CommonMessage.Account.PHONENUMER_ALREADY_EXISTS)
                    };
                }

                return new BaseResponse<Guid>
                {
                    Success = false,
                    Message = CommonMessage.Account.REGISTER_INVALID_EXCEPTION,
                    MessageCode = nameof(CommonMessage.Account.REGISTER_INVALID_EXCEPTION)
                };
            }

            return new BaseResponse<Guid>
            {
                Success = true,
                Message = CommonMessage.Account.REGISTER_SUCCESSFUL,
                MessageCode = nameof(CommonMessage.Account.REGISTER_SUCCESSFUL),
                Data = identityUser.Id
            };
        }

        public async Task<BaseResponse<OtpResponse>> ResetPasswordAsync(ResetPasswordModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return new BaseResponse<OtpResponse>
                {
                    Success = false,
                    Message = CommonMessage.Account.NOT_FOUND,
                    MessageCode = nameof(CommonMessage.Account.NOT_FOUND)
                };
            }

            var userNameIsEmail = UserExtensions.IsEmailAddress(model.UserName);
            var userNameIsPhone = UserExtensions.IsPhoneNumber(model.UserName, "SG");

            var res = await otpTokenService.ProcessSendOtpAsync(user.Id, model.UserName, Constants.OtpType.RESET_PASSWORD);
            if (userNameIsEmail)
            {
                res.Message = CommonMessage.Account.RESET_PASSWORD_OTP_SEND_TO_EMAIL;
                res.MessageCode = nameof(CommonMessage.Account.RESET_PASSWORD_OTP_SEND_TO_EMAIL);
            }
            else if (userNameIsPhone)
            {
                res.Message = CommonMessage.Account.RESET_PASSWORD_OTP_SEND_TO_PHONE;
                res.MessageCode = nameof(CommonMessage.Account.RESET_PASSWORD_OTP_SEND_TO_PHONE);
            }

            return res;
        }

        public async Task<BaseResponse<Guid>> UpdateAccountProfileV1Async(string userName, UpdateAccountProfileModel model)
        {
            try
            {
                var user = await GetAccountAysnc(s => s.NormalizedEmail == userName.ToUpper());
                if (user == null)
                {
                    return new BaseResponse<Guid>
                    {
                        Success = false,
                        Message = CommonMessage.Account.NOT_FOUND,
                        MessageCode = nameof(CommonMessage.Account.NOT_FOUND)
                    };
                }

                user.FullName = model.Fullname;
                if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    user.PhoneNumber = model.PhoneNumber;
                }

                var avatarPath = new BaseResponse<FileUploadResponse>();
                if (model.Avatar != null)
                {
                    var validate = FileExtensions.ValidateImageFile(model.Avatar);
                    if (validate == false)
                    {
                        return new BaseResponse<Guid>
                        {
                            Success = false,
                            Message = CommonMessage.Upload.FILE_EXTENSION_INVALID
                        };
                    }

                    avatarPath = await fileUploadService.UploadImageAsync(model.Avatar, "user/avatar");
                }

                var isAddNewMetaData = false;
                var userMetaData = user.IdentityUserMetaData;
                if (userMetaData != null)
                {
                    if (!string.IsNullOrEmpty(model.Address))
                    {
                        userMetaData.Address = model.Address;
                    }

                    if (model.ProvinceId != null)
                    {
                        userMetaData.ProvinceId = model.ProvinceId;
                    }

                    if (model.ProvinceId != null)
                    {
                        userMetaData.DistrictId = model.DistrictId;
                    }

                    if (model.DateOfBirth != null)
                    {
                        userMetaData.DateOfBirth = model.DateOfBirth;
                    }

                    userMetaData.Gender = model.Gender;

                    if (model.Avatar != null && avatarPath.Success)
                    {
                        userMetaData.Avatar = avatarPath.Data.ThumbnailUrl;
                    }
                }
                else
                {
                    isAddNewMetaData = true;
                    userMetaData = new IdentityUserMetaData
                    {
                        Id = Guid.NewGuid(),
                        Address = model.Address,
                        DistrictId = model.DistrictId,
                        ProvinceId = model.ProvinceId,
                        DateOfBirth = model.DateOfBirth,
                        IdentityUserId = user.Id,
                        CreatedBy = user.Id,
                        CreatedDate = DateTime.UtcNow,
                        IsDelete = false,
                        Gender = model.Gender
                    };

                    if (model.Avatar != null && avatarPath.Success)
                    {
                        userMetaData.Avatar = avatarPath.Data.ThumbnailUrl;
                    }
                }

                using (var scope = new TransactionScope())
                {
                    microDbContext.Set<IdentityUser>().Update(user);
                    if (isAddNewMetaData)
                    {
                        microDbContext.Set<IdentityUserMetaData>().Add(userMetaData);
                    }
                    else
                    {
                        microDbContext.Set<IdentityUserMetaData>().Update(userMetaData);
                    }

                    microDbContext.SaveChanges();
                    scope.Complete();
                }

                await messageBusService.SendAsync(QueuesConstants.UserTrackingActivity, new AccountTrackingModel
                {
                    Action = model.Action,
                    Description = model.Description,
                    IpAddress = model.IpAddress,
                    Location = model.Location,
                    UserAgent = model.UserAgent,
                    Via = model.Via.ToString(),
                    IdentityUserId = user.Id
                });

                return new BaseResponse<Guid>
                {
                    Success = true,
                    Message = CommonMessage.Account.UPDATE_PROFILE_SUCCESS,
                    MessageCode = nameof(CommonMessage.Account.UPDATE_PROFILE_SUCCESS),
                    Data = user.Id
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return new BaseResponse<Guid>
                {
                    Success = false,
                    Message = CommonMessage.UN_DETECTED_ERROR,
                    MessageCode = nameof(CommonMessage.UN_DETECTED_ERROR)
                };
            }
        }

        public async Task<BaseResponse<Guid>> UpdateAccountProfileAsync(string userName, UpdateAccountProfileModel model)
        {
            try
            {
                var user = await GetAccountAysnc(s => s.NormalizedEmail == userName.ToUpper());
                if (user == null)
                {
                    return new BaseResponse<Guid>
                    {
                        Success = false,
                        Message = CommonMessage.Account.NOT_FOUND,
                        MessageCode = nameof(CommonMessage.Account.NOT_FOUND)
                    };
                }

                user.FullName = model.Fullname;
                if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    user.PhoneNumber = model.PhoneNumber;
                }

                var avatarPath = new BaseResponse<FileUploadResponse>();
                if (model.Avatar != null)
                {
                    var validate = FileExtensions.ValidateImageFile(model.Avatar);
                    if (validate == false)
                    {
                        return new BaseResponse<Guid>
                        {
                            Success = false,
                            Message = CommonMessage.Upload.FILE_EXTENSION_INVALID
                        };
                    }

                    avatarPath = await fileUploadService.UploadImageAsync(model.Avatar, "user/avatar");
                }

                var isAddNewMetaData = false;
                var userMetaData = user.IdentityUserMetaData;
                if (userMetaData != null)
                {
                    if (!string.IsNullOrEmpty(model.Address))
                    {
                        userMetaData.Address = model.Address;
                    }

                    if (model.ProvinceId != null)
                    {
                        userMetaData.ProvinceId = model.ProvinceId;
                    }

                    if (model.ProvinceId != null)
                    {
                        userMetaData.DistrictId = model.DistrictId;
                    }

                    if (model.DateOfBirth != null)
                    {
                        userMetaData.DateOfBirth = model.DateOfBirth;
                    }

                    userMetaData.Gender = model.Gender;

                    if (model.Avatar != null && avatarPath.Success)
                    {
                        userMetaData.Avatar = avatarPath.Data.ThumbnailUrl;
                    }
                }
                else
                {
                    isAddNewMetaData = true;
                    userMetaData = new IdentityUserMetaData
                    {
                        Id = Guid.NewGuid(),
                        Address = model.Address,
                        DistrictId = model.DistrictId,
                        ProvinceId = model.ProvinceId,
                        DateOfBirth = model.DateOfBirth,
                        IdentityUserId = user.Id,
                        CreatedBy = user.Id,
                        CreatedDate = DateTime.UtcNow,
                        IsDelete = false,
                        Gender = model.Gender,
                    };

                    if (model.Avatar != null && avatarPath.Success)
                    {
                        userMetaData.Avatar = avatarPath.Data.ThumbnailUrl;
                    }
                }

                using (var scope = new TransactionScope())
                {
                    microDbContext.Set<IdentityUser>().Update(user);
                    if (isAddNewMetaData)
                    {
                        microDbContext.Set<IdentityUserMetaData>().Add(userMetaData);
                    }
                    else
                    {
                        microDbContext.Set<IdentityUserMetaData>().Update(userMetaData);
                    }

                    microDbContext.SaveChanges();
                    scope.Complete();
                }

                await messageBusService.SendAsync(QueuesConstants.UserTrackingActivity, new AccountTrackingModel
                {
                    Action = model.Action,
                    Description = model.Description,
                    IpAddress = model.IpAddress,
                    Location = model.Location,
                    UserAgent = model.UserAgent,
                    Via = model.Via.ToString(),
                    IdentityUserId = user.Id
                });

                return new BaseResponse<Guid>
                {
                    Success = true,
                    Message = CommonMessage.Account.UPDATE_PROFILE_SUCCESS,
                    MessageCode = nameof(CommonMessage.Account.UPDATE_PROFILE_SUCCESS),
                    Data = user.Id
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return new BaseResponse<Guid>
                {
                    Success = false,
                    Message = CommonMessage.UN_DETECTED_ERROR,
                    MessageCode = nameof(CommonMessage.UN_DETECTED_ERROR)
                };
            }
        }

        public async Task<BaseResponse<object>> ChangePasswordAsync(string userName, ChangePasswordModel model)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.NOT_FOUND,
                    MessageCode = nameof(CommonMessage.Account.NOT_FOUND)
                };
            }

            if (model.OldPassword != null)
            {
                var isValidOldPass = await userManager.CheckPasswordAsync(user, model.OldPassword);
                if (!isValidOldPass)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.CHANGE_PASSWORD_OLD_PASS_INVALID,
                        MessageCode = nameof(CommonMessage.Account.CHANGE_PASSWORD_OLD_PASS_INVALID)
                    };
                }
            }

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, resetToken, model.Password);

            model.Description = result.Succeeded ? "Change password successfully" : "Change password failed";
            //await messageBusService.SendAsync(QueuesConstants.UserTrackingActivity, new AccountTrackingModel
            //{
            //    Action = model.Action,
            //    Description = model.Description,
            //    IpAddress = model.IpAddress,
            //    Location = model.Location,
            //    UserAgent = model.UserAgent,
            //    Via = model.Via.ToString(),
            //    IdentityUserId = user.Id
            //});

            if (!result.Succeeded)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.CHANGE_PASSWORD_FAILED,
                    MessageCode = nameof(CommonMessage.Account.CHANGE_PASSWORD_FAILED)
                };
            }

            await LogoutAsync(user.Id);
            return new BaseResponse<object>
            {
                Success = true,
                Message = CommonMessage.Account.CHANGE_PASSWORD_SUCCESS,
                MessageCode = nameof(CommonMessage.Account.CHANGE_PASSWORD_SUCCESS)
            };
        }

        public async Task<BaseResponse<Guid>> ConfirmResetPasswordAsync(ConfirmPasswordModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return new BaseResponse<Guid>
                {
                    Success = false,
                    Message = CommonMessage.Account.NOT_FOUND,
                    MessageCode = nameof(CommonMessage.Account.NOT_FOUND)
                };
            }

            var validateOtp = await otpTokenService.VerifyOtpAsync(otp: model.Otp,
                validateToken: model.ValidateToken,
                otpType: Constants.OtpType.RESET_PASSWORD,
                removeAfterVerify: true,
                emailRegistration: null,
                accountId: user.Id.ToString());
            if (!validateOtp.Success)
            {
                return new BaseResponse<Guid>
                {
                    Success = validateOtp.Success,
                    Code = validateOtp.Code,
                    Errors = validateOtp.Errors,
                    Message = validateOtp.Message,
                    MessageCode = validateOtp.MessageCode,
                    MsgParams = validateOtp.MsgParams
                };
            }

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, resetToken, model.Password);

            model.Description = result.Succeeded ? "Reset password successfully" : "Reset password failed";
            await messageBusService.SendAsync(QueuesConstants.UserTrackingActivity, new AccountTrackingModel
            {
                Action = model.Action,
                Description = model.Description,
                IpAddress = model.IpAddress,
                Location = model.Location,
                UserAgent = model.UserAgent,
                Via = model.Via.ToString(),
                IdentityUserId = user.Id
            });

            if (!result.Succeeded)
            {
                return new BaseResponse<Guid>
                {
                    Success = false,
                    Message = CommonMessage.Account.CHANGE_PASSWORD_FAILED,
                    MessageCode = nameof(CommonMessage.Account.CHANGE_PASSWORD_FAILED)
                };
            }

            await LogoutAsync(user.Id);
            return new BaseResponse<Guid>
            {
                Success = true,
                Message = CommonMessage.Account.CHANGE_PASSWORD_SUCCESS,
                MessageCode = nameof(CommonMessage.Account.CHANGE_PASSWORD_SUCCESS),
                Data = user.Id
            };
        }

        public async Task<LoginResponse> GetLoggedUserAsync(Guid accountId, Constants.Via via)
        {
            try
            {
                var cacheKey = GetUserSessionCacheKey(accountId, via);
                var res = await redisStogare.GetAsync<LoginResponse>(cacheKey);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
                
        public async Task<BaseResponse<object>> ChangeDefaultPasswordAsync(string userName, ChangePassowrdModel model)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.NOT_FOUND,
                    MessageCode = nameof(CommonMessage.Account.NOT_FOUND)
                };
            }

            if (!user.IsDefaultPassword)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.CHANGE_DEFAULT_PASSWORD_FAILED,
                    MessageCode = nameof(CommonMessage.Account.CHANGE_DEFAULT_PASSWORD_FAILED)
                };
            }

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, resetToken, model.Password);
            if (!result.Succeeded)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.CHANGE_PASSWORD_FAILED,
                    MessageCode = nameof(CommonMessage.Account.CHANGE_PASSWORD_FAILED)
                };
            }

            user.IsDefaultPassword = false;
            await userManager.UpdateAsync(user);

            await LogoutAsync(user.Id);
            return new BaseResponse<object>
            {
                Success = true,
                Message = CommonMessage.Account.CHANGE_PASSWORD_SUCCESS,
                MessageCode = nameof(CommonMessage.Account.CHANGE_PASSWORD_SUCCESS)
            };
        }

        private string GetUserSessionCacheKey(Guid accountId, Constants.Via via)
        {
            return $"LOGIN_{accountId}:{via}";
        }

        private async Task SaveLoggedUserToCacheAsync(Guid accountId, Constants.Via via, LoginResponse res)
        {
            var cacheKey = GetUserSessionCacheKey(accountId, via);
            await redisStogare.SetAsync(cacheKey, res, TimeSpan.FromMinutes(res.ExpiredInSecond - 5 * 60));
        }

        public async Task<BaseResponse<object>> UpdateAccountAsync(Guid accountId, RegisterModel model)
        {
            var account = await Repository.GetByIdAsync(accountId);
            if (account == null)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.NOT_FOUND
                };
            }

            account.FullName = model.FullName;
            if (!string.IsNullOrWhiteSpace(model.Password) && model.Password.Equals(model.RePassword))
            {
                account.PasswordHash = userManager.PasswordHasher.HashPassword(account, model.Password);
            }

            var res = await userManager.UpdateAsync(account);
            if (res.Succeeded)
            {
                return new BaseResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.Account.UPDATE_PROFILE_SUCCESS
                };
            }

            return new BaseResponse<object>
            {
                Success = false,
                Message = CommonMessage.Account.UPDATE_PROFILE_FAILED
            };
        }

        public async Task<IdentityUser> GetAccountAysnc(Expression<Func<IdentityUser, bool>> predicate)
        {
            return await microDbContext.Set<IdentityUser>()
                .Include(s => s.IdentityUserMetaData)
                .Include(s => s.IdentityUserTwoFAs)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<string> GenerateReferralCodeAsync()
        {
            var i = 0;
            while (i <= 5)
            {
                i++;

                var referralCode = randomService.GenerateRandomCharacter(8, false, true, false);
                var exist = await redisStogare.HGetAsync<string>("USER_REFERRAL", referralCode);
                if (string.IsNullOrWhiteSpace(exist))
                {
                    return referralCode;
                }
            }

            return randomService.GenerateRandomCharacter(6, false, true, false);
        }

        private async Task<BaseResponse<string>> CheckRequired2FAAsync(LoginResponse loginRes)
        {
            if (!loginRes.TwoFactorServices.Any())
            {
                return new BaseResponse<string>
                {
                    Success = true
                };
            }

            var twoSessionId = Guid.NewGuid().ToString();
            await redisStogare.SetAsync(twoSessionId, loginRes, TimeSpan.FromMinutes(5));

            return new BaseResponse<string>
            {
                Success = false,
                Data = twoSessionId
            };
        }

        private LoginResponse BuildJwtLoginToken(LoginResponse loginRes)
        {
            var isConfirmed = loginRes.PhoneNumberConfirmed || loginRes.EmailConfirmed;
            var claims = new[]
            {
                new Claim(Constants.Jwt.ClaimKeys.Id, loginRes.Id.ToString()),
                new Claim(Constants.Jwt.ClaimKeys.UserName, loginRes.UserName),
                new Claim(Constants.Jwt.ClaimKeys.AccountType, loginRes.AccountType),
                new Claim(Constants.Jwt.ClaimKeys.Via, loginRes.Via),
                new Claim(Constants.Jwt.ClaimKeys.IsConfirmed, isConfirmed.ToString())
            };

            double expiryMinutes = 10080;
            double.TryParse(configuration.GetValue<string>("JWT:ExpiryMinutes"), out expiryMinutes);
            var jwt = jwtService.BuildToken(claims, expiryMinutes);

            loginRes.Token = jwt.Token;
            loginRes.ExpiredInSecond = jwt.ExpiredInSecond;
            loginRes.ValidateTo = DateTime.UtcNow.AddSeconds(jwt.ExpiredInSecond);

            return loginRes;
        }

        private LoginResponse ToLoginResponse(IdentityUser identity)
        {
            return new LoginResponse
            {
                AccountType = identity.AccountType,
                Avatar = identity.IdentityUserMetaData?.Avatar,
                Email = identity.Email,
                DateOfBirth = identity.IdentityUserMetaData?.DateOfBirth,
                EmailConfirmDate = identity.EmailConfirmDate,
                EmailConfirmed = identity.EmailConfirmed,
                Fullname = identity.FullName,
                Gender = identity.IdentityUserMetaData != null ? identity.IdentityUserMetaData.Gender.Value : (sbyte)Constants.Account.Gender.Other,
                Id = identity.Id,
                IsDefaultPassword = identity.IsDefaultPassword,
                LatestLoginTime = identity.LastLoginTime,
                PhoneNumber = identity.PhoneNumber,
                LastLoginIpAddress = identity.LastLoginIpAddress,
                PhoneNumberConfirmed = identity.PhoneNumberConfirmed,
                UserName = identity.UserName,
                Via = identity.Via
            };
        }

        private async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            var user = await microDbContext.Set<IdentityUser>()
                .Include(s => s.IdentityUserMetaData)
                .Include(s => s.IdentityUserTwoFAs)
                .FirstOrDefaultAsync(s => s.NormalizedEmail == email.ToUpper());

            return user;
        }

        private async Task<BaseResponse<string>> CheckRequired2FAAsync(IdentityUser userEntity)
        {
            if (!userEntity.IdentityUserTwoFAs.Any())
            {
                return new BaseResponse<string>
                {
                    Success = true
                };
            }

            var twoSessionId = Guid.NewGuid().ToString();
            await redisStogare.SetAsync(twoSessionId, userEntity.Id, TimeSpan.FromMinutes(5));

            return new BaseResponse<string>
            {
                Success = false,
                Data = twoSessionId
            };
        }

        public async Task<IdentityUser> GetWeb3AccountAsync(string address)
        {
            var user = await microDbContext.Set<IdentityUser>()
                .Include(s => s.IdentityUserMetaData)
                .FirstOrDefaultAsync(s => s.NormalizedUserName == address.ToUpper());

            return user;
        }
    }
}