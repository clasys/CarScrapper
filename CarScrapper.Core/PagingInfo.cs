using System.Collections.Generic;

namespace CarScrapper.Core
{
    public class PagingInfo
    {
        public bool IsEnabled { get; set; }
        public IList<string> PagedUrls { get; set; }
    }
}
