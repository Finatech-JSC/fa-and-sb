using Google.Authenticator;
using MicroBase.Share.Models;
using MicroBase.Share.Models.Accounts;

namespace MicroBase.Service.Accounts
{
    public interface IGoogleAuthenticatorService
    {
        BaseResponse<GoogleAuthenConfigResponse> GetSetupData(string secretKey, string issuer, string accountTitleNoSpace);

        bool VerifyCode(string secreKey, string code);
    }

    public class GoogleAuthenticatorService : IGoogleAuthenticatorService
    {
        public BaseResponse<GoogleAuthenConfigResponse> GetSetupData(string secretKey, string issuer, string accountTitleNoSpace)
        {
            var tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode(issuer, accountTitleNoSpace, secretKey, false);
            return new BaseResponse<GoogleAuthenConfigResponse>
            {
                Success = true,
                Data = new GoogleAuthenConfigResponse
                {
                    Account = accountTitleNoSpace,
                    Key = secretKey,
                    ManualEntryKey = setupInfo.ManualEntryKey,
                    QrCodeSetupImageUrl = setupInfo.QrCodeSetupImageUrl
                }
            };
        }

        public bool VerifyCode(string secreKey, string code)
        {
            var tfa = new TwoFactorAuthenticator();
            return tfa.ValidateTwoFactorPIN(secreKey, code);
        }
    }
}