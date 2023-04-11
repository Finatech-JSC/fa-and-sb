using MicroBase.Share.Extensions;
using MicroBase.Share.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MicroBase.Logging
{
    public interface ITelegramBotService
    {
        /// <summary>
        /// Gửi tin nhắn vào một nhóm telegram
        /// </summary>
        /// <param name="isProduction"></param>
        /// <param name="method"></param>
        /// <param name="message"></param>
        /// <param name="botChannel"></param>
        /// <returns></returns>
        Task<BaseResponse<object>> SendMessageAsync(bool isProduction,
            string method,
            string message,
            BotConstant.BotChannel botChannel);
    }

    public class TelegramBotService : ITelegramBotService
    {
        private readonly HttpClient httpClient;
        private BotConnectorModel botConnector = null;

        public TelegramBotService(IHttpClientFactory httpClientFactory,
            BotConnectorModel botConnector)
        {
            this.botConnector = botConnector;

            if (botConnector.Enabled)
            {
                httpClient = httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri($"https://api.telegram.org/bot{botConnector.BotToken}/");
            }
        }

        private TelegramGroupModel GetBotConfig(BotConstant.BotChannel botChannel)
        {
            if (botConnector == null)
            {
                return null;
            };

            return botConnector.TelegramGroups.FirstOrDefault(s => s.BotChannel == botChannel.ToString());
        }

        /// <summary>
        /// Gửi tin nhắn vào một nhóm telegram
        /// </summary>
        /// <param name="isProduction"></param>
        /// <param name="method"></param>
        /// <param name="message"></param>
        /// <param name="botChannel"></param>
        /// <returns></returns>
        public async Task<BaseResponse<object>> SendMessageAsync(bool isProduction,
            string method,
            string message,
            BotConstant.BotChannel botChannel)
        {
            try
            {
                var telegramGroup = GetBotConfig(botChannel);
                if (telegramGroup == null)
                {
                    return new BaseResponse<object>
                    {
                        Success = false
                    };
                }

                var model = new TelegramMessageModel
                {
                    ChatId = telegramGroup.GroupId,
                    Text = $"[{DateTime.UtcNow}]\n{method}\n{message}",
                    DisableNotification = false
                };

                var res = await httpClient.PostRequestAsync<TelegramMessageModel, object>("sendMessage?parse_mode=html", model);
                return res;
            }
            catch (Exception ex)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = ex.ToString()
                };
            }
        }
    }
}