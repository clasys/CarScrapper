using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace CarScrapper.Core
{
    public class DealerInspireSelector : BaseSelector
    {
        private readonly List<DealerInfo> _dealers;

        public DealerInspireSelector(string make, string model) : base(make, model)
        {
            _dealers = base.DealersBase.Where(a => a.Type == "DealerInspire").ToList();
        }

        public override string GetBodyStyleIdentifier()
        {
            throw new NotImplementedException();
        }

        public override string GetCarUrlIdentifier(){ return ".//a"; }

        public override List<Tuple<string, string>> GetCleanupMap()
        {
            return new List<Tuple<string, string>>
            {
                new Tuple<string, string>("DriveType", "\n"),
                new Tuple<string, string>("DriveType", "\t"),
                new Tuple<string, string>("ExteriorColor", "\n"),
                new Tuple<string, string>("ExteriorColor", "\t"),
                new Tuple<string, string>("ExteriorColor", "Exterior:"),
                new Tuple<string, string>("InteriorColor", "\n"),
                new Tuple<string, string>("InteriorColor", "\t"),
                new Tuple<string, string>("InteriorColor", "Interior:"),
                new Tuple<string, string>("Transmission", "\n"),
                new Tuple<string, string>("Transmission", "\t"),
                new Tuple<string, string>("Model","&reg;"),
                new Tuple<string, string>("Transmission","Trans:"),
                new Tuple<string, string>("DriveType","Drivetrain:"),
                new Tuple<string, string>("StockNumber", "Stock #:"),
            };
        }

        public override string GetDriveTypeIdentifier() { return ".//span[@class='detail-label']"; }

        public override string GetEngineIdentifier()
        {
            throw new NotImplementedException();
        }

        public override string GetExtColorIdentifier(){ return ".//span[@class='detail-label']"; }

        public override string[] GetInfoSeparators()
        {
            //return new[] { "\n", "\t" };
            return new[] { "\n" };
        }

        public override string GetIntColorIdentifier(){ return ".//span[@class='detail-label']"; }

        public override string GetMakeIdentifier()
        {
            return Make;
        }

        public override string GetModelCodeIdentifier()
        {
            throw new NotImplementedException();
        }

        public override string GetModelIdentifier()
        {
            return Model;
        }

        public override string GetMsrpIdentifier()
        {
            return ".//span[@class='price']";
        }

        public override string[] GetRowSelectors()
        {
            return new[]
            {
                ".//div[@class='vehicle-overview']",
                ".//div[@class='vehicle-wrap']"
            };
        }

        public override string GetStockNumberIdentifier() { return ".//span[@class='stock-label']"; }

        public override string GetTransmissionIdentifier() { return ".//span[@class='detail-label']"; }

        public override string GetUrlDetails()
        {
            return string.Format("/new-vehicles/{0}/#action=im_ajax_call&perform=get_results&vrp_view=listview&page=1", GetModelIdentifier());
        }

        public override string GetVinIdentifier() { return "VIN:"; }

        public override List<Tuple<string, Regex>> GetRegexMap()
        {
            return new List<Tuple<string, Regex>>
            {
                new Tuple<string, Regex>("InteriorColor", new Regex("[ ]{2,}", RegexOptions.None)),
                new Tuple<string, Regex>("ExteriorColor", new Regex("[ ]{2,}", RegexOptions.None)),
                new Tuple<string, Regex>("DriveType", new Regex("[ ]{2,}", RegexOptions.None)),
                new Tuple<string, Regex>("Transmission", new Regex("[ ]{2,}", RegexOptions.None))
            };
        }

        public override PagingInfo GetPagingInfo(HtmlDocument htmlDocument)
        {
            //no paging logic yet, return original URL for scrapping
            return new PagingInfo
            {
                IsEnabled = true,
                PagedUrls = new List<string>() { GetUrlDetails() }
            };
        }

        public override CarInfo ParseHtmlIntoCarInfo(HtmlNode node, DealerInfo dealer)
        {
            var entries = node.InnerText?.Split(GetInfoSeparators(), StringSplitOptions.RemoveEmptyEntries);
            return new CarInfo
            {
                Make = GetMakeIdentifier(),
                Model = GetModel(node),
                MSRP = GetMSRP(node),
                InteriorColor = GetIntColor(node),
                ExteriorColor = GetExtColor(node),
                DriveType = node.SelectNodes(GetDriveTypeIdentifier())?.Where(a => a.InnerText.ToLower().Contains("drivetrain")).SingleOrDefault()?.ParentNode.InnerText.Trim(),
                Transmission = node.SelectNodes(GetTransmissionIdentifier())?.Where(a => a.InnerText.ToLower().Contains("trans")).SingleOrDefault()?.ParentNode.InnerText.Trim(),
                StockNumber = GetStock(node),
                VIN = GetVin(entries, node),
                URL = node.Descendants().Where(a => a.Name == "a" && a.OuterHtml.Contains("http") && !a.OuterHtml.Contains("javascript")).FirstOrDefault()?.Attributes.Where(a => a.Name == "href").FirstOrDefault()?.Value

                ////WebSite = URL, do it on a higher level

                //Engine = entries.Where(a => a.Contains(GetEngineIdentifier())).FirstOrDefault()?.Replace(GetEngineIdentifier(), "").Trim(),
                //BodyStyle = entries.Where(a => a.Contains(GetBodyStyleIdentifier())).FirstOrDefault()?.Replace(GetBodyStyleIdentifier(), "").Trim(),
                //ModelCode = entries.Where(a => a.Contains(GetModelCodeIdentifier())).FirstOrDefault()?.Replace(GetModelCodeIdentifier(), "").Trim(),

            };
        }

        public override List<DealerInfo> GetDealers()
        {
            return _dealers;
        }

        private string GetVin(string[] entries, HtmlNode node)
        {
            var vin = entries.Where(a => a.Contains(GetVinIdentifier())).FirstOrDefault()?.Replace(GetVinIdentifier(), "").Trim();

            if (string.IsNullOrEmpty(vin))
            {
                var data = GetDataFromAttributes(node);

                if (data != null)
                    vin = data.Where(a => a.Contains("vin")).FirstOrDefault()?.Replace("\"vin\":", "").Replace("\"", "");
            }

            return vin;
        }

        private string GetStock(HtmlNode node)
        {
            var stock = node.SelectSingleNode(GetStockNumberIdentifier())?.InnerText.Trim();

            if (string.IsNullOrEmpty(stock))
            {
                var data = GetDataFromAttributes(node);

                if (data != null)
                    stock = data.Where(a => a.Contains("stock")).FirstOrDefault()?.Replace("\"stock\":", "").Replace("\"", "");
            }

            return stock;
        }

        private string GetExtColor(HtmlNode node)
        {
            var extColor = node.SelectNodes(GetExtColorIdentifier())?.Where(a => a.InnerText.ToLower().Contains("exterior")).SingleOrDefault()?.ParentNode.InnerText.Trim();

            if (string.IsNullOrEmpty(extColor))
            {
                var data = GetDataFromAttributes(node);

                if (data != null)
                    extColor = data.Where(a => a.Contains("ext_color")).FirstOrDefault()?.Replace("\"ext_color\":", "").Replace("\"", "");
            }

            return extColor;
        }

        private string GetIntColor(HtmlNode node)
        {
            var color = node.SelectNodes(GetIntColorIdentifier())?.Where(a => a.InnerText.ToLower().Contains("interior")).SingleOrDefault()?.ParentNode.InnerText.Trim();

            if (string.IsNullOrEmpty(color))
            {
                var data = GetDataFromAttributes(node);

                if (data != null)
                    color = data.Where(a => a.Contains("int_color")).FirstOrDefault()?.Replace("\"int_color\":", "").Replace("\"", "");
            }

            return color;
        }

        private string GetMSRP(HtmlNode node)
        {
            var msrp = node.SelectNodes(GetMsrpIdentifier()).FirstOrDefault()?.InnerText?.Trim();

            //No MSRP info in attributes
            //if (string.IsNullOrEmpty(msrp))
            //{
            //    var data = GetDataFromAttributes(node);

            //    if (data != null)
            //        msrp = data.Where(a => a.Contains("trim")).FirstOrDefault()?.Replace("\"trim\":", "").Replace("\"", "");
            //}

            return msrp;
        }

        private string GetModel(HtmlNode node)
        {
            var model = node.Descendants().Where(a => a.OuterHtml.ToLower().Contains(GetModelIdentifier().ToLower()) && a.Name == "a" && a.Attributes.Count() == 1 && !string.IsNullOrEmpty(a.InnerText.Trim())).FirstOrDefault()?.InnerText;

            if (string.IsNullOrEmpty(model))
            {
                var data = GetDataFromAttributes(node);

                if (data != null)
                    model = data.Where(a => a.Contains("trim")).FirstOrDefault()?.Replace("\"trim\":", "").Replace("\"", "");
            }

            return model;
        }

        private string[] GetDataFromAttributes(HtmlNode node)
        { 
            return node.Descendants("a").Where(a => a.Attributes.Contains("data-vehicle")).FirstOrDefault()?.Attributes["data-vehicle"].Value?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
