using System.Collections.Generic;

namespace MicroBase.Share.Models
{
    public class TPaging<T>
    {
        public IEnumerable<T> Source { get; set; }

        public long TotalRecords { get; set; }

        public int Pages { get; set; }
    }
}