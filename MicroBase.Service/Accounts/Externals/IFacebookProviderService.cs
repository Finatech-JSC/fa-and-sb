using MicroBase.FileStorage;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models.Accounts;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace MicroBase.Service.Accounts.Externals
{
    public interface IFacebookProviderService
    {
        Task<SocialUserModel> GetUserFromFacebookAsync(string facebookToken);
    }

    public class FacebookProviderService : IFacebookProviderService
    {
        private readonly HttpClient httpClient;
        private readonly IFileUploadService fileUploadService;

        public FacebookProviderService(IConfiguration configuration,
            FileUploadFactory fileUploadFactory)
        {
            var uploadServiceName = configuration.GetValue<string>("FileManage:EnableService");
            this.fileUploadService = fileUploadFactory.GetServiceByName(uploadServiceName);

            httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com/v2.8/")
            };

            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<SocialUserModel> GetUserFromFacebookAsync(string facebookToken)
        {
            var result = await GetAsync<FacebookUserModel>(facebookToken, "me", "fields=id,name,first_name,last_name,email,picture.width(500).height(500)");
            if (result == null)
            {
                throw new Exception("User from this token not exist");
            }

            var avatar = string.Empty;
            if (!string.IsNullOrWhiteSpace(result.Picture?.Data?.Url))
            {
                var res = await fileUploadService.SaveFileFormUrlAsync(result.Picture?.Data?.Url, $"{Guid.NewGuid()}.jpeg", string.Empty);
                if (res.Success)
                {
                    avatar = res.Data.ThumbnailUrl;
                }
            }

            var account = new SocialUserModel
            {
                Id = result.Id,
                Email = result.Id.GenerateSystemEmail(),
                FirstName = result.FirstName,
                LastName = result.LastName,
                FullName = result.Name,
                Picture = avatar
            };

            return account;
        }

        private async Task<T> GetAsync<T>(string accessToken, string endpoint, string args = null)
        {
            var response = await httpClient.GetAsync($"{endpoint}?access_token={accessToken}&{args}");
            if (!response.IsSuccessStatusCode)
            {
                return default(T);
            }

            var result = await response.Content.ReadAsStringAsync();

            return JsonExtensions.JsonDeserialize<T>(result);
        }
    }
}