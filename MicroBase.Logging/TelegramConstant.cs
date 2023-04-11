using System.Collections.Generic;

namespace MicroBase.Logging
{
    public static class TelegramConstant
    {
        public static class BotConstant
        {
            public enum BotChannel
            {
                /// <summary>
                /// Thông báo lỗi hệ thống
                /// </summary>
                EXCEPTION,

                /// <summary>
                /// Thông báo vận hành
                /// </summary>
                OPERATION
            }
        }

        public class BotConnectorModel
        {
            /// <summary>
            /// Token của bot telegram
            /// </summary>
            public string BotToken { get; set; }

            public IEnumerable<TelegramGroupModel> TelegramGroups { get; set; }
        }

        public class TelegramGroupModel
        {
            /// <summary>
            /// Group thông báo
            /// </summary>
            public string BotChannel { get; set; }

            /// <summary>
            /// Id của telegram
            /// </summary>
            public string GroupId { get; set; }
        }
    }
}