using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CarScraper.Web.API.Models;
using CarScrapper.Core;

namespace CarScraper.Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarSearchController : ControllerBase
    {
        public CarSearchController()
        {
        }

        // GET: api/Search
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarInfo>>> Get([FromBody] CarSearch carSearch)
        {
            var p = carSearch;

            return await Task.Run(() => 
            {
                ProcessingPreferences prefs = null;

                switch (carSearch.DealerType)
                {
                    case DealerType.DealerCom:
                        prefs = new ProcessingPreferences(new DealerComSelector(carSearch.Make, carSearch.Model, InventoryType.New));
                        break;
                    case DealerType.DealerOn:
                        prefs = new ProcessingPreferences(new DealerOnSelector(carSearch.Make, carSearch.Model, InventoryType.New));
                        break;
                    case DealerType.DealerInspire:
                        prefs = new ProcessingPreferences(new DealerInspireSelector(carSearch.Make, carSearch.Model, InventoryType.New));
                        break;
                }

                var processor = new Processor(new[] { prefs });
                return Ok(processor.Scrap().AsEnumerable());
            });
        }
    }
}
