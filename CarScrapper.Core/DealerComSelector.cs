﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace CarScrapper.Core
{
    public class DealerComSelector : BaseSelector
    {
        private readonly List<DealerInfo> _dealers;

        public DealerComSelector(string make, string model) : base(make, model)
        {
            _dealers = base.DealersBase.Where(a => a.Type == "DealerCom").ToList();
        }

        public override string GetBodyStyleIdentifier() { return "ff_body_type"; }
        public override string GetCarUrlIdentifier()
        {
            throw new NotImplementedException();
        }
        public override List<Tuple<string, string>> GetCleanupMap() { return null; }
        public override List<DealerInfo> GetDealers(){ return _dealers; }
        public override string GetDriveTypeIdentifier(){ return "Drive Line:"; }
        public override string GetEngineIdentifier() { return "Engine:"; }
        public override string GetExtColorIdentifier() { return "Exterior Color:"; }
        public override string[] GetInfoSeparators() { return new[] { ",", "More&" }; }
        public override string GetIntColorIdentifier(){ return "Interior Color:"; }
        public override string GetMakeIdentifier()
        {
            throw new NotImplementedException();
        }
        public override string GetModelCodeIdentifier()
        {
            throw new NotImplementedException();
        }
        public override string GetModelIdentifier(){ return Model; }
        public override string GetMsrpIdentifier(){ return ".//span[contains(@class,'msrp')]"; }
        public override List<Tuple<string, Regex>> GetRegexMap() { return null; }
        public override string[] GetRowSelectors()
        {
            return new[]
            {
                ".//div[contains(@class,'hproduct ')]"
            };
        }
        public override string GetStockNumberIdentifier() { return "Stock #:"; }
        public override string GetTransmissionIdentifier(){ return "Transmission:"; }
        public override string GetUrlDetails()
        {
            return string.Format("/new-inventory/index.htm?model={0}", GetModelIdentifier());
        }
        public override string GetVinIdentifier() { return "ff_vin"; }

        public override PagingInfo GetPagingInfo(HtmlDocument htmlDocument)
        {
            var urls = new List<string>();
            urls.Add(string.Format("{0}&start=1", GetUrlDetails()));

            var entry = htmlDocument.DocumentNode.SelectSingleNode(".//span[contains(text(), 'Page')]")?.InnerText;
            if (entry != null)
            {
                var matches = Regex.Matches(entry, "\\d+");

                if (matches.Count != 2)
                    return new PagingInfo { IsEnabled = false };

                int iStart = int.Parse(matches[0].Value);
                int iEnd = int.Parse(matches[1].Value);

                for (int i = iStart; i <= iEnd; i++)
                {
                    urls.Add(string.Format("{0}&start={1}", GetUrlDetails(), int.Parse(i + "0")));
                }
            }

            return new PagingInfo
            {
                IsEnabled = true,
                PagedUrls = urls
            };
        }

        public override CarInfo ParseHtmlIntoCarInfo(HtmlNode node, DealerInfo dealer)
        {
            var description = node.Descendants("div").Where(a => a.Attributes.Contains("class") && a.Attributes["class"].Value == "description").FirstOrDefault();
            var entries = description?.InnerText.Split(GetInfoSeparators(), StringSplitOptions.RemoveEmptyEntries);
            //var anotherDiv = node.Descendants("div").Where(a => a.Attributes.Any(b => b.Value.Equals("ff_link"))).FirstOrDefault();

            var carInfo = new CarInfo();
            carInfo.Make = Make;
            carInfo.Model = GetModel(node);
            carInfo.Engine = entries.Where(a => a.Contains(GetEngineIdentifier())).FirstOrDefault()?.Replace(GetEngineIdentifier(), string.Empty).Trim();
            carInfo.Transmission = entries.Where(a => a.Contains(GetTransmissionIdentifier())).FirstOrDefault()?.Replace(GetTransmissionIdentifier(), string.Empty).Trim();
            carInfo.DriveType = entries.Where(a => a.Contains(GetDriveTypeIdentifier())).FirstOrDefault()?.Replace(GetDriveTypeIdentifier(), string.Empty).Trim();
            carInfo.ExteriorColor = entries.Where(a => a.Contains(GetExtColorIdentifier())).FirstOrDefault()?.Replace(GetExtColorIdentifier(), string.Empty).Trim();
            carInfo.InteriorColor = entries.Where(a => a.Contains(GetIntColorIdentifier())).FirstOrDefault()?.Replace(GetIntColorIdentifier(), string.Empty).Trim();
            carInfo.StockNumber = entries.Where(a => a.Contains(GetStockNumberIdentifier())).FirstOrDefault()?.Replace(GetStockNumberIdentifier(), string.Empty).Trim();
            carInfo.MSRP = node.SelectNodes(GetMsrpIdentifier())?.FirstOrDefault()?.ChildNodes[1].InnerText;
            carInfo.VIN = GetVIN(node);
            carInfo.BodyStyle = GetBodyStyle(node);
            carInfo.URL = string.Format("{0}/{1}",
                dealer.Url,
                node.SelectNodes(".//a[contains(@href,'/new/')]")?.FirstOrDefault()?.Attributes["href"].Value);

            return carInfo;
        }

        private string GetBodyStyle(HtmlNode node)
        {
            var div = node.Descendants("div").Where(a => a.Attributes.Any(b => b.Value.Equals("ff_link"))).FirstOrDefault();
            return div?.Attributes.Where(a => a.Name.Equals(GetBodyStyleIdentifier())).FirstOrDefault()?.Value;
        }

        private string GetModel(HtmlNode node)
        {
            var div = node.Descendants("div").Where(a => a.Attributes.Any(b => b.Value.Equals("ff_link"))).FirstOrDefault();
            var result = string.Format("{0} {1}",
                                div?.Attributes.Where(a => a.Name.Equals("ff_model")).FirstOrDefault()?.Value,
                                div?.Attributes.Where(a => a.Name.Equals("ff_trim")).FirstOrDefault()?.Value);

            //<a class="url" href="/new/Volvo/2020-Volvo-XC60-falls-church-va-015da5050a0e0a6b1bace8b878690fce.htm"> 2020 Volvo XC60 T5 Momentum SUV</a>
            if (string.IsNullOrEmpty(result?.Trim()))
                result = node.SelectNodes(".//a[@class='url']")?.FirstOrDefault()?.InnerText?.Trim();

            return result;
        }

        private string GetVIN(HtmlNode node)
        {
            var div = node.Descendants("div").Where(a => a.Attributes.Any(b => b.Value.Equals("ff_link"))).FirstOrDefault();
            var result = div?.Attributes.Where(a => a.Name.Equals(GetVinIdentifier())).FirstOrDefault()?.Value;
            
            //< div class="les_video" les_vin="YV4102RK6L1601526"></div>
	        //<div class="tps-roadster-btn" data-condition="new" data-vin="YV4102RK6L1601526"></div>
	        //<div class="tps-roadster-buildmydeal-btn" data-condition="new" data-vin="YV4102RK6L1601526"></div>
            
            if (string.IsNullOrEmpty(result))
                result = node.SelectNodes(".//div[@class='les_video']")?.FirstOrDefault().GetAttributeValue("les_vin", string.Empty);

            return result;
        }
    }
}
