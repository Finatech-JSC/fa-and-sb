using MicroBase.Share.Models.Accounts;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace MicroBase.Service.Accounts.Externals
{
    public interface IAppleProviderService
    {
        Task<SocialUserModel> GetUserFromAppleAsync(string token);
    }

    public class AppleProviderService : IAppleProviderService
    {
        private readonly HttpClient httpClient;

        public AppleProviderService()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<SocialUserModel> GetUserFromAppleAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var userToken = tokenHandler.ReadJwtToken(token);
            var emailClaim = userToken.Claims.FirstOrDefault(s => s.Type == "email");

            var account = new SocialUserModel
            {
                Email = emailClaim?.Value
            };

            return account;
        }
    }
}