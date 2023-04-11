using System.Collections.Generic;

namespace MicroBase.Share.Models.Accounts
{
    public class ExternalAccountProviderModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IDictionary<string, string> ClientIds { get; set; }
    }
}