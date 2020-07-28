using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CarScrapper.Core
{
    public class CustomCaddySelector : BaseSelector
    {
        private readonly List<DealerInfo> _dealers;
        public CustomCaddySelector(string model, InventoryType inventoryType, Regions region) : base("Cadillac", model, inventoryType, region)
        {
            _dealers = base.DealersBase.Where(a => a.Type == "CustomCaddy").ToList();
        }

        public override string GetBodyStyleIdentifier(){throw new NotImplementedException();}

        public override string GetCarUrlIdentifier(){throw new NotImplementedException();}

        public override List<Tuple<string, string>> GetCleanupMap()
        {
            return null;
        }

        public override List<DealerInfo> GetDealers()
        {
            return _dealers;
        }

        public override string GetDriveTypeIdentifier(){throw new NotImplementedException();}

        public override string GetEngineIdentifier(){throw new NotImplementedException();}

        public override string GetExtColorIdentifier(){throw new NotImplementedException();}

        public override string[] GetInfoSeparators(){throw new NotImplementedException();}

        public override string GetIntColorIdentifier(){throw new NotImplementedException();}

        public override string GetMakeIdentifier(){throw new NotImplementedException();}

        public override string GetModelCodeIdentifier(){throw new NotImplementedException();}

        public override string GetModelIdentifier(){throw new NotImplementedException();}

        public override string GetMsrpIdentifier(){throw new NotImplementedException();}

        public override PagingInfo GetPagingInfo(HtmlDocument htmlDocument, DealerInfo dealer)
        {
            //TODO: look into need for paging later
            return new PagingInfo
            {
                IsEnabled = true,
                PagedUrls = new List<string>() { GetUrlDetails(dealer) }
            };
        }

        public override List<Tuple<string, Regex>> GetRegexMap()
        {
            return null;
        }

        public override string[] GetRowSelectors()
        {
            return new[] 
            {
                ".//section[contains(@id, 'card-view/card') and contains(@data-params, 'vin:')]"
            };
        }

        public override string GetStockNumberIdentifier(){throw new NotImplementedException();}

        public override string GetTransmissionIdentifier(){throw new NotImplementedException();}

        public override string GetUrlDetails(DealerInfo dealer)
        {
            return string.Format("{0}/VehicleSearchResults?search=new&model={1}&limit=100", dealer.Url, base.Model);
        }

        public override string GetVinIdentifier(){throw new NotImplementedException();}

        public override CarInfo ParseHtmlIntoCarInfo(HtmlNode node, DealerInfo dealer)
        {
            var sectionID = node.Attributes["id"]?.Value;
            var data = node.OwnerDocument.DocumentNode.SelectNodes(string.Format(".//section[@id='{0}']",sectionID))?.FirstOrDefault()?.Attributes["data-params"]?.Value?.Split(';');


            var car = new CarInfo
            {
                BodyStyle = data.Where(a => a.Contains("bodyType:")).SingleOrDefault()?.Replace("bodyType:", ""),
                ExteriorColor = data.Where(a => a.Contains("exteriorColor:")).SingleOrDefault()?.Replace("exteriorColor:", "")?.Replace("%20", " "),
                Make = base.Make,
                Model = (data.Where(a => a.Contains("year:")).SingleOrDefault()?.Replace("year:", "") +
                    data.Where(a => a.Contains("model:")).SingleOrDefault()?.Replace("model:", " ") +
                    data.Where(a => a.Contains("trim:")).SingleOrDefault()?.Replace("trim:", " "))?.Replace("%20", " "),
                StockNumber = data.Where(a => a.Contains("stockNumber:")).SingleOrDefault()?.Replace("stockNumber:", ""),
                VIN = data.Where(a => a.Contains("vin:")).SingleOrDefault()?.Replace("vin:", ""),
                URL = node.SelectNodes(".//a")?.Where(a => a.Attributes["itemprop"]?.Value.ToLower() == "url").FirstOrDefault()?.Attributes["href"].Value,
                IsLoaner = node.SelectNodes(".//img[contains(@title,'Courtesy')]")?.Count() > 0,
                MSRP = node.SelectNodes(".//span")?.Where(a => a.Attributes["itemprop"]?.Value == "price").FirstOrDefault()?.InnerText
            };

            return car;
        }
    }
}
