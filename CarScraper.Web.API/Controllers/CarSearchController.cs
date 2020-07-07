using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CarScraper.Web.API.Models;
using CarScrapper.Core;
using System;

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
                var prefs = new List<ProcessingPreferences>();
                var prefsDealerCom = new ProcessingPreferences(new DealerComSelector(carSearch.Make, carSearch.Model, carSearch.IsLoaner ? InventoryType.Loaner : InventoryType.New));
                var prefsDealerOn = new ProcessingPreferences(new DealerOnSelector(carSearch.Make, carSearch.Model, carSearch.IsLoaner ? InventoryType.Loaner : InventoryType.New));
                var prefsDealerInspire = new ProcessingPreferences(new DealerInspireSelector(carSearch.Make, carSearch.Model, carSearch.IsLoaner ? InventoryType.Loaner : InventoryType.New));

                //TODO: implement IsLoaner option
                switch (carSearch.DealerType)
                {
                    case DealerType.DealerCom:
                        prefs.Add(prefsDealerCom);
                        break;
                    case DealerType.DealerOn:
                        prefs.Add(prefsDealerOn);
                        break;
                    case DealerType.DealerInspire:
                        prefs.Add(prefsDealerInspire);
                        break;
                    default:
                        prefs.AddRange(new[]
                        {
                            prefsDealerCom,
                            prefsDealerInspire,
                            prefsDealerOn
                        });
                        break;
                }

                var processor = new Processor(prefs.ToArray());
                return Ok(processor.Scrap().AsEnumerable());
            });
        }
    }
}
