using HtmlAgilityPack;
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

        public override CarInfo ParseHtmlIntoCarInfo(HtmlNode node, DealerInfo dealer)
        {
            var description = node.Descendants("div").Where(a => a.Attributes.Contains("class") && a.Attributes["class"].Value == "description").FirstOrDefault();
            var entries = description?.InnerText.Split(GetInfoSeparators(), StringSplitOptions.RemoveEmptyEntries);
            var anotherDiv = node.Descendants("div").Where(a => a.Attributes.Any(b => b.Value.Equals("ff_link"))).FirstOrDefault();

            var carInfo = new CarInfo
            {
                Make = Make,
                Model = string.Format("{0} {1}",
                    anotherDiv?.Attributes.Where(a => a.Name.Equals("ff_model")).FirstOrDefault()?.Value,
                    anotherDiv?.Attributes.Where(a => a.Name.Equals("ff_trim")).FirstOrDefault()?.Value),
                Engine = entries.Where(a => a.Contains(GetEngineIdentifier())).FirstOrDefault()?.Replace(GetEngineIdentifier(), string.Empty).Trim(),
                Transmission = entries.Where(a => a.Contains(GetTransmissionIdentifier())).FirstOrDefault()?.Replace(GetTransmissionIdentifier(), string.Empty).Trim(),
                DriveType = entries.Where(a => a.Contains(GetDriveTypeIdentifier())).FirstOrDefault()?.Replace(GetDriveTypeIdentifier(), string.Empty).Trim(),
                ExteriorColor = entries.Where(a => a.Contains(GetExtColorIdentifier())).FirstOrDefault()?.Replace(GetExtColorIdentifier(), string.Empty).Trim(),
                InteriorColor = entries.Where(a => a.Contains(GetIntColorIdentifier())).FirstOrDefault()?.Replace(GetIntColorIdentifier(), string.Empty).Trim(),
                StockNumber = entries.Where(a => a.Contains(GetStockNumberIdentifier())).FirstOrDefault()?.Replace(GetStockNumberIdentifier(), string.Empty).Trim(),
                MSRP = node.SelectNodes(GetMsrpIdentifier())?.FirstOrDefault()?.ChildNodes[1].InnerText,
                VIN = anotherDiv?.Attributes.Where(a => a.Name.Equals(GetVinIdentifier())).FirstOrDefault()?.Value,
                BodyStyle = anotherDiv?.Attributes.Where(a => a.Name.Equals(GetBodyStyleIdentifier())).FirstOrDefault()?.Value,
                URL = string.Format("{0}/{1}",
                    dealer.Url,
                    node.SelectNodes(".//a[contains(@href,'/new/')]").FirstOrDefault()?.Attributes["href"].Value)
            };

            return carInfo;
        }
    }
}
