using System;
using System.Collections.Generic;

namespace MicroBase.Share.Models
{
    public class EmailQueueModel
    {
        public Guid? AccountId { get; set; }

        public string EmailTemplate { get; set; }

        public string ReceivingAddress { get; set; }

        public string CultureCode { get; set; }

        public Dictionary<string, string> EmailTokens { get; set; }
    }
}