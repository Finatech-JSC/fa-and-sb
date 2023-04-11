using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroBase.Logging
{
    public static class BotConstant
    {
        public enum BotChannel
        {
            /// <summary>
            /// System message
            /// </summary>
            EXCEPTION,

            /// <summary>
            /// For operation
            /// </summary>
            OPERATION
        }
    }

    public class BotConnectorModel
    {
        /// <summary>
        /// Telegram token
        /// </summary>
        public string BotToken { get; set; }

        public IEnumerable<TelegramGroupModel> TelegramGroups { get; set; }

        public bool Enabled { get; set; }
    }

    public class TelegramGroupModel
    {
        /// <summary>
        /// BotConstant.BotChannel
        /// </summary>
        public string BotChannel { get; set; }

        /// <summary>
        /// Telegram Group Id
        /// </summary>
        public string GroupId { get; set; }
    }
}