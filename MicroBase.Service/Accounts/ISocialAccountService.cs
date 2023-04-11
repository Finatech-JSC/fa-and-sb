using MicroBase.Service.Accounts.Externals;
using MicroBase.Share.Constants;
using MicroBase.Share.Models;
using MicroBase.Share.Models.Accounts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace MicroBase.Service.Accounts
{
    public interface ISocialAccountService
    {
        Task<BaseResponse<SocialAccountInfo>> VerifyLoginTokenAsync(SocialLoginModel model);
    }

    public class SocialAccountService : ISocialAccountService
    {
        private readonly IOptions<List<ExternalAccountProviderModel>> externalAccProvidersOption;
        private readonly ILogger logger;
        private readonly IFacebookProviderService facebookProviderService;
        private readonly IAppleProviderService appleProviderService;

        public SocialAccountService(IOptions<List<ExternalAccountProviderModel>> externalAccProvidersOption,
            ILogger<SocialAccountService> logger,
            IFacebookProviderService facebookProviderService,
            IAppleProviderService appleProviderService)
        {
            this.externalAccProvidersOption = externalAccProvidersOption;
            this.logger = logger;
            this.facebookProviderService = facebookProviderService;
            this.appleProviderService = appleProviderService;
        }

        public async Task<BaseResponse<SocialAccountInfo>> VerifyLoginTokenAsync(SocialLoginModel model)
        {
            var provider = externalAccProvidersOption.Value.FirstOrDefault(s => s.Name == model.ProviderName);
            if (provider == null)
            {
                return new BaseResponse<SocialAccountInfo>
                {
                    Success = false,
                    Message = CommonMessage.Account.LOGIN_SOCIAL_NOT_SUPPORTED,
                    MessageCode = nameof(CommonMessage.Account.LOGIN_SOCIAL_NOT_SUPPORTED),
                    MsgParams = new List<string> { model.ProviderName }
                };
            }

            var res = new BaseResponse<SocialAccountInfo>();
            var platform = model.Via.ToString();
            switch (model.ProviderName)
            {
                case Constants.ExternalAccountProvider.GOOGLE:
                    return await AuthByGoogleAsync(platform, model.Token, provider);
                case Constants.ExternalAccountProvider.FACEBOOK:
                    return await AuthByFacebookAsync(model.Token, provider);
                case Constants.ExternalAccountProvider.APPLE:
                    return await AuthByAppleAsync(model.Token, provider);
                default:
                    return new BaseResponse<SocialAccountInfo>
                    {
                        Success = false,
                        Message = CommonMessage.Account.LOGIN_SOCIAL_NOT_SUPPORTED,
                        MessageCode = nameof(CommonMessage.Account.LOGIN_SOCIAL_NOT_SUPPORTED),
                        MsgParams = new List<string> { model.ProviderName }
                    };
            }
        }

        public async Task<BaseResponse<SocialAccountInfo>> AuthByGoogleAsync(string platform, 
            string token, 
            ExternalAccountProviderModel provider)
        {
            try
            {
                var clientId = provider.ClientIds.ContainsKey(platform) ? provider.ClientIds[platform] : string.Empty;
                var verify = await ValidateAsync(token);

                return new BaseResponse<SocialAccountInfo>
                {
                    Success = true,
                    Message = CommonMessage.Account.LOGIN_SOCIAL_VERIRY_SUCCESS,
                    Data = new SocialAccountInfo
                    {
                        Email = verify.Email,
                        FullName = $"{verify.GivenName} {verify.FamilyName}",
                        Avatar = verify.Picture
                    }
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.Message}:{Environment.NewLine} {ex.StackTrace}");
                return new BaseResponse<SocialAccountInfo>
                {
                    Success = false,
                    Message = CommonMessage.Account.LOGIN_SOCIAL_VERIRY_FAILED
                };
            }
        }

        public async Task<BaseResponse<SocialAccountInfo>> AuthByFacebookAsync(string token, ExternalAccountProviderModel provider)
        {
            try
            {
                var res = await facebookProviderService.GetUserFromFacebookAsync(token);
                return new BaseResponse<SocialAccountInfo>
                {
                    Success = true,
                    Message = CommonMessage.Account.LOGIN_SOCIAL_VERIRY_SUCCESS,
                    Data = new SocialAccountInfo
                    {
                        Email = res.Email,
                        FullName = res.FullName,
                        ExternalAccountId = res.Id,
                        Avatar = res.Picture
                    }
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.Message}:{Environment.NewLine} {ex.StackTrace}");
                return new BaseResponse<SocialAccountInfo>
                {
                    Success = false,
                    Message = CommonMessage.Account.LOGIN_SOCIAL_VERIRY_FAILED
                };
            }
        }

        public async Task<BaseResponse<SocialAccountInfo>> AuthByAppleAsync(string token, ExternalAccountProviderModel provider)
        {
            try
            {
                var res = await appleProviderService.GetUserFromAppleAsync(token);
                return new BaseResponse<SocialAccountInfo>
                {
                    Success = true,
                    Message = CommonMessage.Account.LOGIN_SOCIAL_VERIRY_SUCCESS,
                    Data = new SocialAccountInfo
                    {
                        Email = res.Email,
                        ProviderName = provider.Name,
                        FullName = res.Email
                    }
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.Message}:{Environment.NewLine} {ex.StackTrace}");
                return new BaseResponse<SocialAccountInfo>
                {
                    Success = false,
                    Message = CommonMessage.Account.LOGIN_SOCIAL_VERIRY_FAILED
                };
            }
        }
    }
}