using System.Collections.Generic;

namespace MicroBase.Share.Models.Emails
{
    public class EmailFilterCacheModel
    {
        public IEnumerable<string> BlackList { get; set; }

        public IEnumerable<string> WhiteList { get; set; }
    }
}