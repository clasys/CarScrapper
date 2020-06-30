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
    public class DealerOnSelector : ISelector
    {
        private readonly string _make;
        private readonly string _model;

        public DealerOnSelector(string make, string model)
        {
            _make = make;
            _model = model;
        }

        public string GetExtColorIdentifier() { return "Ext. Color:"; }
        public string[] GetInfoSeparator() { return new[] { "\r", "VIN" }; }
        public string GetIntColorIdentifier() { return "Int. Color:"; }
        public string GetMakeIdentifier() { return _make; }
        public string GetModelIdentifier() { return _model; }
        public string GetMsrpIdentifier() { return "MSRP"; }
        public string[] GetRowSelectors() 
        { 
            return new[] 
            { 
                ".//div[contains(@id, 'srpRow')]" 
            }; 
        }
        public string GetUriDetails() { return string.Format("/searchnew.aspx?Model={0}&pn=100", GetModelIdentifier()); }
        public string GetEngineIdentifier() { return "Engine:"; }
        public string GetDriveTypeIdentifier() { return "Drive Type:"; }
        public string GetStockNumberIdentifier() { return "Stock #:"; }
        public string GetVinIdentifier() { return " #:"; }
        public string GetCarUrlIdentifier() { return ".//a[contains(@class, 'stat-text-link')]"; }
        public string GetBodyStyleIdentifier() { return "Body Style:"; }
        public string GetModelCodeIdentifier() { return "Model Code:"; }
        public string GetTransmissionIdentifier() { return "Transmission:"; }
        public List<Tuple<string, string>> GetCleanupMap()
        {
            return new List<Tuple<string, string>>
            {
                new Tuple<string, string>("MSRP", ":")
                
            };
        }

        public CarInfo ParseHtmlIntoCarInfo(HtmlNode node)
        {
            var entries = node.InnerText?.Split(GetInfoSeparator(), StringSplitOptions.RemoveEmptyEntries);
            return new CarInfo
            {
                Make = GetMakeIdentifier(),
                Model = entries.Where(a => a.Contains(GetMakeIdentifier())).FirstOrDefault()?.Trim(),
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

        public List<Tuple<string, Regex>> GetRegexMap()
        {
            return null;
        }
    }
}
