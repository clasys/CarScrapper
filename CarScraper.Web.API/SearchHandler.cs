using CarScraper.Web.API.Data;
using CarScraper.Web.API.Models;
using CarScrapper.Core;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CarScraper.Web.API
{
    public class SearchHandler
    {
        private readonly CarScraperWebAPIContext _dataContext;

        public SearchHandler(CarScraperWebAPIContext context)
        {
            _dataContext = context;
        }

        /// <summary>
        /// Starts search and stores results using unique key for future retrieval
        /// </summary>
        /// <param name="carSearch"></param>
        public ScrapeResult StartCarSearch(CarSearch carSearch)
        {
            var start = DateTime.Now;

            try
            {
                var prefs = new List<ProcessingPreferences>();
                var prefsDealerCom = new ProcessingPreferences(new DealerComSelector(carSearch.Make, carSearch.Model, carSearch.IsLoaner ? InventoryType.Loaner : InventoryType.New, carSearch.Region));
                var prefsDealerOn = new ProcessingPreferences(new DealerOnSelector(carSearch.Make, carSearch.Model, carSearch.IsLoaner ? InventoryType.Loaner : InventoryType.New, carSearch.Region));
                var prefsDealerInspire = new ProcessingPreferences(new DealerInspireSelector(carSearch.Make, carSearch.Model, carSearch.IsLoaner ? InventoryType.Loaner : InventoryType.New, carSearch.Region));

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
                var scrapeResults = processor.Scrap();

                return new ScrapeResult
                {
                    ScrapeResults = scrapeResults,
                    RuntimeException = null,
                    DurationInSeconds = (decimal)(DateTime.Now - start).TotalSeconds
                };
            }
            catch (Exception e)
            {
                return new ScrapeResult
                {
                    ScrapeResults = new List<CarInfo> { new CarInfo { Make = "Search Error" } },
                    RuntimeException = e,
                    DurationInSeconds = (decimal)(DateTime.Now - start).TotalSeconds
                };
            }
        }

        private void SaveToDb(IList<CarInfo> searchResults, Guid uniqueKey)
        {
            string data = JsonConvert.SerializeObject(searchResults, Formatting.Indented);
            var searchData = new SearchData
            {
                Key = uniqueKey,
                Data = data
            };
            
            _dataContext.Entry(searchData).State = EntityState.Modified;
            _dataContext.SaveChangesAsync();
        }
    }
}
