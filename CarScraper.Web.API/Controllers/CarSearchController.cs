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
        [HttpGet]
        public ActionResult Get([FromBody] CarSearch carSearch, [FromServices] IConfiguration config)
        {
            //assign searchresult 
            var uniqueKey = Guid.NewGuid();

            //start async search here
            Task.Run(() => 
            { 
                var searchResults = new SearchHandler(_context).StartCarSearch(uniqueKey, carSearch);

                string data = JsonConvert.SerializeObject(searchResults, Formatting.None);
                var searchData = new SearchData
                {
                    Key = uniqueKey,
                    Data = data
                };

                var util = new Util(config);
                using (var db = util.GetDbContext())
                {
                    db.SearchData.Add(searchData);
                    db.SaveChanges();
                }
            });

            return new AcceptedResult();
        }

        // GET: api/Search
        [Route("GetResults")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarInfo>>> Get(string key)
        {
            return await Task.Run(() => { return new AcceptedResult(); });
        }
    }
}
