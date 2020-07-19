using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CarScrapper.Core
{
    public class DealerOnSelector : BaseSelector
    {
        private readonly List<DealerInfo> _dealers;

        public DealerOnSelector(string make, string model, InventoryType inventoryType, Regions region) : base(make, model, inventoryType, region)
        {
            _dealers = base.DealersBase.Where(a => a.Type == "DealerOn").ToList();
        }

        public override string GetExtColorIdentifier() { return "Ext. Color:"; }
        public override string[] GetInfoSeparators() { return new[] { "\r", "VIN" }; }
        public override string GetIntColorIdentifier() { return "Int. Color:"; }
        public override string GetMakeIdentifier() { return Make; }
        public override string GetModelIdentifier() { return Model; }
        public override string GetMsrpIdentifier() { return "MSRP"; }
        public override string[] GetRowSelectors() 
        { 
            return new[] 
            { 
                ".//div[contains(@id, 'srpRow')]" 
            }; 
        }
        public override string GetUrlDetails(DealerInfo dealer) 
        {
            return string.Format("{0}/searchnew.aspx?Model={1}&pn=100&st=Price+desc", dealer.Url, GetModelIdentifier()); 
        }
        public override string GetEngineIdentifier() { return "Engine:"; }
        public override string GetDriveTypeIdentifier() { return "Drive Type:"; }
        public override string GetStockNumberIdentifier() { return "Stock #:"; }
        public override string GetVinIdentifier() { return " #:"; }
        public override string GetCarUrlIdentifier() { return ".//a[contains(@class, 'stat-text-link')]"; }
        public override string GetBodyStyleIdentifier() { return "Body Style:"; }
        public override string GetModelCodeIdentifier() { return "Model Code:"; }
        public override string GetTransmissionIdentifier() { return "Transmission:"; }
        public override List<Tuple<string, string>> GetCleanupMap()
        {
            return new List<Tuple<string, string>>
            {
                new Tuple<string, string>("MSRP", ":")
                
            };
        }

        public override PagingInfo GetPagingInfo(HtmlDocument htmlDocument, DealerInfo dealer)
        {
            //no paging logic yet, return original URL for scrapping
            return new PagingInfo
            {
                IsEnabled = true,
                PagedUrls = new List<string>() { GetUrlDetails(dealer) }
            };
        }

        public override CarInfo ParseHtmlIntoCarInfo(HtmlNode node, DealerInfo dealer)
        {
            var entries = node.InnerText?.Split(GetInfoSeparators(), StringSplitOptions.RemoveEmptyEntries);
            return new CarInfo
            {
                Make = GetMakeIdentifier(),
                Model = entries.Where(a => a.ToLower().Contains(GetMakeIdentifier().ToLower())).FirstOrDefault()?.Trim(),
                MSRP = entries.Where(a => a.Contains(GetMsrpIdentifier())).FirstOrDefault()?.Replace(GetMsrpIdentifier(), "").Trim(),
                InteriorColor = entries.Where(a => a.Contains(GetIntColorIdentifier())).FirstOrDefault()?.Replace(GetIntColorIdentifier(), "").Trim(),
                ExteriorColor = entries.Where(a => a.Contains(GetExtColorIdentifier())).FirstOrDefault()?.Replace(GetExtColorIdentifier(), "").Trim(),
                DriveType = entries.Where(a => a.Contains(GetDriveTypeIdentifier())).FirstOrDefault()?.Replace(GetDriveTypeIdentifier(), "").Trim(),
                Engine = entries.Where(a => a.Contains(GetEngineIdentifier())).FirstOrDefault()?.Replace(GetEngineIdentifier(), "").Trim(),
                StockNumber = entries.Where(a => a.Contains(GetStockNumberIdentifier())).FirstOrDefault()?.Replace(GetStockNumberIdentifier(), "").Trim(),
                VIN = entries.Where(a => a.Contains(GetVinIdentifier())).FirstOrDefault()?.Replace(GetVinIdentifier(), "").Trim(),
                URL = node.SelectNodes(GetCarUrlIdentifier()).FirstOrDefault()?.Attributes.Where(a => a.Name == "href").FirstOrDefault()?.Value,
                //WebSite = URL, do it on a higher level
                BodyStyle = entries.Where(a => a.Contains(GetBodyStyleIdentifier())).FirstOrDefault()?.Replace(GetBodyStyleIdentifier(), "").Trim(),
                ModelCode = entries.Where(a => a.Contains(GetModelCodeIdentifier())).FirstOrDefault()?.Replace(GetModelCodeIdentifier(), "").Trim(),
                Transmission = entries.Where(a => a.Contains(GetTransmissionIdentifier())).FirstOrDefault()?.Replace(GetTransmissionIdentifier(), "").Trim()
            };
        }

        public override List<Tuple<string, Regex>> GetRegexMap()
        {
            return null;
        }

        public override List<DealerInfo> GetDealers()
        {
            return _dealers;
        }
    }
}
