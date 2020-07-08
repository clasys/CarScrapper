using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarScraper.Web.API.Models
{
    public class SearchData
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public string Data { get; set; }
        public string Exception { get; set; }
        public decimal SearchDurationInSec { get; set; }
    }
}
