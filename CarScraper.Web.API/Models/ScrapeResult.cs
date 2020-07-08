using CarScrapper.Core;
using System;
using System.Collections.Generic;

namespace CarScraper.Web.API.Models
{
    public class ScrapeResult
    {
        public IList<CarInfo> ScrapeResults { get; set; }
        public Exception RuntimeException { get; set; }
        public decimal DurationInSeconds { get; set; }
    }
}
