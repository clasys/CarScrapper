using CarScrapper.Core;
using System.Collections.Generic;

namespace CarScraper.Web.API.Models
{
    public class SearchResult
    {
        public decimal DurationInSeconds { get; set; }
        public SearchResultStatus Status { get; set; }
        public string Error { get; set; }
        public int Count { get; set; }
        public IList<CarInfo> Results { get; set; }

    }
}
