using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace CarScrapper.Core
{
    public class DealerInspireSelector : ISelector
    {
        private readonly string _make;
        private readonly string _model;

        public DealerInspireSelector(string make, string model)
        {
            _make = make;
            _model = model;
        }

        public string GetBodyStyleIdentifier()
        {
            throw new NotImplementedException();
        }

        public string GetCarUrlIdentifier(){ return ".//a"; }

        public List<Tuple<string, string>> GetCleanupMap()
        {
            return new List<Tuple<string, string>>
            {
                new Tuple<string, string>("DriveType", "\n"),
                new Tuple<string, string>("DriveType", "\t"),
                new Tuple<string, string>("ExteriorColor", "\n"),
                new Tuple<string, string>("ExteriorColor", "\t"),
                new Tuple<string, string>("InteriorColor", "\n"),
                new Tuple<string, string>("InteriorColor", "\t"),
                new Tuple<string, string>("Transmission", "\n"),
                new Tuple<string, string>("Transmission", "\t"),
                new Tuple<string, string>("Model","&reg;")
            };
        }

        public string GetDriveTypeIdentifier() { return ".//span[@class='detail-label']"; }

        public string GetEngineIdentifier()
        {
            throw new NotImplementedException();
        }

        public string GetExtColorIdentifier(){ return ".//span[@class='detail-label']"; }

        public string[] GetInfoSeparator()
        {
            //return new[] { "\n", "\t" };
            return new[] { "\n" };
        }

        public string GetIntColorIdentifier(){ return ".//span[@class='detail-label']"; }

        public string GetMakeIdentifier()
        {
            return _make;
        }

        public string GetModelCodeIdentifier()
        {
            throw new NotImplementedException();
        }

        public string GetModelIdentifier()
        {
            return _model;
        }

        public string GetMsrpIdentifier()
        {
            return ".//span[@class='price']";
        }

        public string[] GetRowSelectors()
        {
            return new[]
            {
                ".//div[@class='vehicle-overview']",
                ".//div[@class='vehicle-wrap']"
            };
        }

        public string GetStockNumberIdentifier() { return ".//span[@class='stock-label']"; }

        public string GetTransmissionIdentifier() { return ".//span[@class='detail-label']"; }

        public string GetUriDetails()
        {
            return string.Format("/new-vehicles/{0}/", GetModelIdentifier());
        }

        public string GetVinIdentifier() { return "VIN:"; }

        public CarInfo ParseHtmlIntoCarInfo(HtmlNode node)
        {
            var entries = node.InnerText?.Split(GetInfoSeparator(), StringSplitOptions.RemoveEmptyEntries);
            return new CarInfo
            {
                Make = GetMakeIdentifier(),
                //Model = entries.Where(a => a.Contains(GetMakeIdentifier())).FirstOrDefault()?.Trim(),
                Model = node.Descendants().Where(a => a.OuterHtml.ToLower().Contains(GetModelIdentifier().ToLower()) && a.Name == "a" && a.Attributes.Count() == 1 && !string.IsNullOrEmpty(a.InnerText.Trim())).FirstOrDefault()?.InnerText,
                MSRP = node.SelectNodes(GetMsrpIdentifier()).FirstOrDefault()?.InnerText?.Trim(),
                InteriorColor = node.SelectNodes(GetIntColorIdentifier())?.Where(a => a.InnerText.ToLower().Contains("interior")).SingleOrDefault()?.ParentNode.InnerText.Trim(),
                ExteriorColor = node.SelectNodes(GetExtColorIdentifier())?.Where(a => a.InnerText.ToLower().Contains("exterior")).SingleOrDefault()?.ParentNode.InnerText.Trim(),
                DriveType = node.SelectNodes(GetDriveTypeIdentifier())?.Where(a => a.InnerText.ToLower().Contains("drivetrain")).SingleOrDefault()?.ParentNode.InnerText.Trim(),
                Transmission = node.SelectNodes(GetTransmissionIdentifier())?.Where(a => a.InnerText.ToLower().Contains("trans")).SingleOrDefault()?.ParentNode.InnerText.Trim(),
                StockNumber = node.SelectSingleNode(GetStockNumberIdentifier())?.InnerText.Trim(),
                VIN = entries.Where(a => a.Contains(GetVinIdentifier())).FirstOrDefault()?.Replace(GetVinIdentifier(), "").Trim(),
                //URL = node.SelectNodes(GetCarUrlIdentifier()).FirstOrDefault()?.Attributes.Where(a => a.Name == "href").FirstOrDefault()?.Value
                URL = node.Descendants().Where(a => a.Name == "a" && a.OuterHtml.Contains("http") && !a.OuterHtml.Contains("javascript")).FirstOrDefault()?.Attributes.Where(a => a.Name == "href").FirstOrDefault()?.Value

                ////WebSite = URL, do it on a higher level

                //Engine = entries.Where(a => a.Contains(GetEngineIdentifier())).FirstOrDefault()?.Replace(GetEngineIdentifier(), "").Trim(),
                //BodyStyle = entries.Where(a => a.Contains(GetBodyStyleIdentifier())).FirstOrDefault()?.Replace(GetBodyStyleIdentifier(), "").Trim(),
                //ModelCode = entries.Where(a => a.Contains(GetModelCodeIdentifier())).FirstOrDefault()?.Replace(GetModelCodeIdentifier(), "").Trim(),

            };
        }
    }
}
