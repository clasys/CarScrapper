using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CarScraper.Web.API.Models;
using CarScrapper.Core;
using System;
using CarScraper.Web.API.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CarScraper.Web.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CarSearchController : ControllerBase
    {
        private readonly CarScraperWebAPIContext _context;

        public CarSearchController(CarScraperWebAPIContext context)
        {
            _context = context;
        }

        [Route("StartSearch")]
        [HttpPost]
        public ActionResult<SearchTicket> Post([FromBody] CarSearch carSearch, [FromServices] IConfiguration config)
        {
            //assign searchresult 
            var uniqueKey = Guid.NewGuid();

            //start async search here
            Task.Run(() => 
            { 
                var results = new SearchHandler(_context).StartCarSearch(uniqueKey, carSearch);
                string data = JsonConvert.SerializeObject(results.ScrapeResults, Formatting.None);
                
                var searchData = new SearchData
                {
                    Key = uniqueKey,
                    Data = data,
                    Exception = results.RuntimeException?.Message,
                    SearchDurationInSec = results.DurationInSeconds
                };

                var util = new Util(config);
                using (var db = util.GetDbContext())
                {
                    db.SearchData.Add(searchData);
                    db.SaveChanges();
                }
            });

            var returnTicket = new SearchTicket
            {
                SearchKey = uniqueKey.ToString(),
                RetryAfter = Duration.ThirtySeconds,
                KeyRetrievalEndpoint = string.Format("{0}://{1}{2}{3}", Request.Scheme, Request.Host, Request.Path, Request.QueryString)?.ToLower().Replace("startsearch", "getresults")
            };

            return new AcceptedResult("", returnTicket);
        }

        // GET: api/Search
        [Route("GetResults")]
        [HttpGet]
        public ActionResult<SearchResult> Get(string searchKey, [FromServices] IConfiguration config)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                var util = new Util(config);
                using (var db = util.GetDbContext())
                {
                    var result = db.SearchData.Where(a => a.Key.ToString() == searchKey).SingleOrDefault();

                    if (result != null)
                    {
                        if (!string.IsNullOrEmpty(result.Exception))
                            return new OkObjectResult(
                                new SearchResult 
                                { 
                                    Error = result.Exception?.Length > 64 ? result.Exception?.Substring(0,64) : result.Exception,
                                    Status = SearchResultStatus.Failure,
                                    DurationInSeconds = result.SearchDurationInSec,
                                    Count = 0
                                });

                        var carInfos = (IList<CarInfo>)JsonConvert.DeserializeObject(result.Data, typeof(IList<CarInfo>));

                        return new OkObjectResult(
                            new SearchResult
                            {
                                Results = carInfos,
                                Status = SearchResultStatus.Success,
                                DurationInSeconds = result.SearchDurationInSec,
                                Count = carInfos.Count()
                            });
                    }

                    return new AcceptedResult("", new SearchResult { Status = SearchResultStatus.SearchInProgress });
                }
            }

            return new NotFoundResult();
        }

        [Route("GetAvailableDealers")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetAvailableDealers()
        {
            try
            {
                var dealerNames = CarScrapper.Core.Util.GetDealers().OrderBy(a => a.Name).Select(a => a.Name).Distinct();
                return Ok(dealerNames);
            }
            catch
            {
                return new NotFoundResult();
            }
        }

        [Route("GetAvailableMakes")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetAvailableMakes()
        {
            try
            {
                return Ok(CarScrapper.Core.Util.GetDealers().OrderBy(a => a.Make).Select(a => a.Make).Distinct());
            }
            catch
            {
                return new NotFoundResult();
            }
        }
    }
}
