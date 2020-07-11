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

        public DealerComSelector(string make, string model, InventoryType inventoryType) : base(make, model, inventoryType)
        {
            _dealers = base.DealersBase.Where(a => a.Type == "DealerCom").ToList();
        }

        public override string GetBodyStyleIdentifier() { return "ff_body_type"; }
        public override string GetCarUrlIdentifier()
        {
            throw new NotImplementedException();
        }
        public override List<Tuple<string, string>> GetCleanupMap() {
            return new List<Tuple<string, string>>
            {
                new Tuple<string, string>("BodyStyle","\"")
            };
        }
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
                ".//div[contains(@class,'hproduct ')]",
                ".//li[contains(@class, 'vehicle-card ')]",
                ".//div[contains(@class, 'browse-row ')]"
            };
        }
        public override string GetStockNumberIdentifier() { return "Stock #:"; }
        public override string GetTransmissionIdentifier(){ return "Transmission:"; }
        public override string GetUrlDetails(DealerInfo dealer)
        {
            //return string.Format("/new-inventory/index.htm?model={0}", GetModelIdentifier());

            var url = string.Format("{0}/new-inventory/index.htm?model={1}", dealer.Url, GetModelIdentifier());

            if (!string.IsNullOrEmpty(dealer.CustomUrl))
                url = string.Format(dealer.CustomUrl, GetModelIdentifier());
            
            return url;
        }
        public override string GetVinIdentifier() { return "ff_vin"; }

        public override PagingInfo GetPagingInfo(HtmlDocument htmlDocument, DealerInfo dealer)
        {
            var urls = new List<string>();
            var url = string.Format("{0}", GetUrlDetails(dealer));
            var isStandardUrl = string.IsNullOrEmpty(dealer.CustomUrl);

            //add pagination only to standard URLs
            if (isStandardUrl)
                url += "&start=0";

            urls.Add(url);

            if (isStandardUrl)
            {
                var entry = htmlDocument.DocumentNode.SelectSingleNode(".//span[contains(text(), 'Page')]")?.InnerText;
                if (entry != null)
                {
                    var matches = Regex.Matches(entry, "\\d+");

                    //Rather than disabling it outright, return collection with 1st paged URL so at least that can be scraped
                    //if (matches.Count != 2)
                    //    return new PagingInfo { IsEnabled = false };

                    if (matches.Count == 2)
                    {
                        int iStart = int.Parse(matches[0].Value);
                        int iEnd = int.Parse(matches[1].Value);

                        for (int i = iStart; i <= iEnd; i++)
                        {
                            urls.Add(string.Format("{0}&start={1}", GetUrlDetails(dealer), int.Parse(i + "0")));
                        }
                    }
                }
            }

            return new PagingInfo
            {
                IsEnabled = true,
                PagedUrls = urls.GroupBy(a => a.Trim()).Select(a => a.First()).ToList() //remove duplicate entries
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
            carInfo.Engine = GetEngine(entries, node);
            carInfo.Transmission = GetTransmission(entries, node);
            carInfo.DriveType = GetDriveType(entries, node);
            carInfo.ExteriorColor = GetExtColor(entries, node);
            carInfo.InteriorColor = GetIntColor(entries, node);
            carInfo.StockNumber = GetStockNumber(entries, node);
            carInfo.MSRP = GetMSRP(node);
            carInfo.VIN = GetVIN(node);
            carInfo.BodyStyle = GetBodyStyle(node);
            carInfo.URL = GetStockUrl(node, dealer);
            carInfo.IsLoaner = IsThisLoaner(node);
            carInfo.IPacket = GetIPacket(node, GetVIN(node));
            carInfo.Packages = GetPackages(node);

            return carInfo;
        }

        private string GetPackages(HtmlNode node)
        {
            var spans = node.SelectNodes(".//div[contains(@class,'packages')]")?.Elements("span");
            return spans == null ? null : string.Join(", ", spans.Where(a => !string.IsNullOrEmpty(a.InnerText?.Replace(",", "").Trim())).Select(a => a.InnerText).ToArray());
        }

        private static string GetStockUrl(HtmlNode node, DealerInfo dealer)
        {
            var url = node.SelectNodes(".//input[contains(@value,'/new-inventory')]")?.FirstOrDefault()?.Attributes["value"]?.Value;

            if (!IsEmpty(url))
                return url;

            return string.Format("{0}/{1}",
                            dealer.Url, node.SelectNodes(".//a[contains(@href,'/new/') or contains(@href, '-new')]")?.FirstOrDefault()?.Attributes["href"].Value);
        }

        private string GetStockNumber(string[] entries, HtmlNode node)
        {
            var result = entries?.Where(a => a.Contains(GetStockNumberIdentifier())).FirstOrDefault()?.Replace(GetStockNumberIdentifier(), string.Empty)?.Trim();

            if (IsEmpty(result))
                result = node.SelectNodes(".//span[@class='field-value']")?.Where(a => a.PreviousSibling?.InnerText?.ToLower() == "stock number:").FirstOrDefault()?.InnerText;

            return result;
        }

        private string GetTransmission(string[] entries, HtmlNode node)
        {
            return entries?.Where(a => a.Contains(GetTransmissionIdentifier())).FirstOrDefault()?.Replace(GetTransmissionIdentifier(), string.Empty)?.Trim();
        }

        private string GetEngine(string[] entries, HtmlNode node)
        {
            var result = entries?.Where(a => a.Contains(GetEngineIdentifier())).FirstOrDefault()?.Replace(GetEngineIdentifier(), string.Empty)?.Trim();

            if (IsEmpty(result))
                result = node.SelectNodes(".//span[@class='field-value']")?.Where(a => a.PreviousSibling?.InnerText?.ToLower() == "engine:").FirstOrDefault()?.InnerText;

            return result;
        }

        private string GetDriveType(string[] entries, HtmlNode node)
        {
            var result = entries?.Where(a => a.Contains(GetDriveTypeIdentifier())).FirstOrDefault()?.Replace(GetDriveTypeIdentifier(), string.Empty)?.Trim();

            if (IsEmpty(result))
                result = (node.SelectNodes(".//span[@class='content']")?.FirstOrDefault()?.InnerText?.ToLower()?.Contains("fwd")).HasValue ? "FWD" : null;

            if (IsEmpty(result))
                result = (node.SelectNodes(".//span[@class='content']")?.FirstOrDefault()?.InnerText?.ToLower()?.Contains("awd")).HasValue ? "AWD" : null;

            if (IsEmpty(result))
                result = node.SelectNodes(".//a[contains(@title, 'sDrive')]")?.Count > 0 ? "RWD" : null;

            if (IsEmpty(result))
                result = node.SelectNodes(".//a[contains(@title, 'xDrive')]")?.Count > 0 ? "AWD" : null;

            if (IsEmpty(result))
                result = node.SelectNodes(".//*[contains(.,'sDrive')]")?.Count() > 0 ? "RWD" : null;

            if (IsEmpty(result))
                result = node.SelectNodes(".//*[contains(.,'xDrive')]")?.Count() > 0 ? "AWD" : null;

            return result;
        }

        private static bool IsEmpty(string result)
        {
            return string.IsNullOrEmpty(result?.Trim());
        }

        private string GetIPacket(HtmlNode node, string vin)
        {
            var xpath = string.Format(".//a[contains(@href, '{0}') and contains(@href, 'ipacket')]", vin);
            var result = node.OwnerDocument?.DocumentNode?.SelectNodes(xpath)?.FirstOrDefault()?.Attributes["href"].Value;
            return result;
        }

        private bool IsThisLoaner(HtmlNode node)
        {
            var result = node.SelectNodes(".//img[contains(@title,'Loaner/Demo')]") != null;

            return result;
        }

        private string GetMSRP(HtmlNode node)
        {
            var result = node.SelectNodes(GetMsrpIdentifier())?.FirstOrDefault()?.ChildNodes[1].InnerText;

            if (IsEmpty(result))
                result = node.SelectNodes(".//dt[@class='final-price msrp']")?.FirstOrDefault()?.NextSibling?.InnerText;

            if (IsEmpty(result))
                result = node.SelectNodes(".//span[contains(@class, 'internetPrice')]")?.FirstOrDefault()?.ChildNodes?.Where(a => a.Attributes["class"].Value == "value").FirstOrDefault()?.InnerText;

            if(IsEmpty(result))
                result = node.SelectNodes(".//span[@class='price-value']")?.Where(a => a.PreviousSibling?.InnerText?.ToLower() == "msrp:").FirstOrDefault()?.InnerText;

            return result;
        }

        private string GetIntColor(string[] entries, HtmlNode node)
        {
            var result = entries?.Where(a => a.Contains(GetIntColorIdentifier())).FirstOrDefault()?.Replace(GetIntColorIdentifier(), string.Empty)?.Trim();

            if(IsEmpty(result))
                result = node.SelectNodes(".//li[contains(@class, 'interiorColor')]")?.FirstOrDefault()?.InnerText;

            if (IsEmpty(result))
                result = node.SelectNodes(".//span[@class='field-value']")?.Where(a => a.PreviousSibling?.InnerText?.ToLower() == "interior color:").FirstOrDefault()?.InnerText;

            return result;
        }

        private string GetExtColor(string[] entries, HtmlNode node)
        {
            var result = entries?.Where(a => a.Contains(GetExtColorIdentifier())).FirstOrDefault()?.Replace(GetExtColorIdentifier(), string.Empty)?.Trim();

            if (IsEmpty(result))
                result = node.SelectNodes(".//li[contains(@class, 'exteriorColor')]")?.FirstOrDefault()?.InnerText;

            if (IsEmpty(result))
                result = node.SelectNodes(".//span[@class='field-value']")?.Where(a => a.PreviousSibling?.InnerText?.ToLower() == "exterior color:").FirstOrDefault()?.InnerText;

            return result;
        }

        private string GetBodyStyle(HtmlNode node)
        {
            var div = node.Descendants("div").Where(a => a.Attributes.Any(b => b.Value.Equals("ff_link"))).FirstOrDefault();
            var result = div?.Attributes.Where(a => a.Name.Equals(GetBodyStyleIdentifier())).FirstOrDefault()?.Value;

            if (IsEmpty(result) && node.OuterHtml.Contains("data-bodystyle"))
                result = node.OuterHtml.Substring(node.OuterHtml.IndexOf("data-bodystyle") + 16, 5);

            return result;
        }

        private string GetModel(HtmlNode node)
        {
            var div = node.Descendants("div").Where(a => a.Attributes.Any(b => b.Value.Equals("ff_link"))).FirstOrDefault();
            var result = string.Format("{0} {1}",
                                div?.Attributes.Where(a => a.Name.Equals("ff_model")).FirstOrDefault()?.Value,
                                div?.Attributes.Where(a => a.Name.Equals("ff_trim")).FirstOrDefault()?.Value);

            if (IsEmpty(result))
                result = node.SelectNodes(".//a[contains(@href, '/new/') or contains(@href, '/commercial-new/')]")?.Where(a => !string.IsNullOrEmpty(a.InnerText) && a.InnerText.Contains(GetModelIdentifier()))?.FirstOrDefault()?.InnerText;

            if (IsEmpty(result))
                result = node.SelectNodes(".//h4[@class='hidden-xs']")?.FirstOrDefault()?.InnerText;

            return result;
        }

        private string GetVIN(HtmlNode node)
        {
            var div = node.Descendants("div").Where(a => a.Attributes.Any(b => b.Value.Equals("ff_link"))).FirstOrDefault();
            var result = div?.Attributes.Where(a => a.Name.Equals(GetVinIdentifier())).FirstOrDefault()?.Value;
            
            if (IsEmpty(result))
                result = node.SelectNodes(".//div[@class='les_video']")?.FirstOrDefault().GetAttributeValue("les_vin", string.Empty);

            if (IsEmpty(result))
                result = node.SelectSingleNode(".//dl[@class='vin']")?.ChildNodes[1].InnerText;

            //<a class="btn btn-primary price-btn btn-block" href="/external-catalog-services/rest/monroney/windowsticker?vin=KMTG44LA5KU025195&status=2&category=AUTO&vehicleId=cb5af8c20a0e0ae9037bd50ce5022fec" target="_self" data-location='vehicle-window-sticker-button'>
            if (IsEmpty(result))
                result = node.SelectSingleNode(".//a[contains(@href, 'vin=')]")?.Attributes["href"].Value?.Split(new[] { '&', '?' }).ToList().Where(a => a.Contains("vin=")).FirstOrDefault()?.Replace("vin=", "").Trim();

            if (IsEmpty(result) && node.OuterHtml.Contains("data-vin"))
                result = node.OuterHtml.Substring(node.OuterHtml.IndexOf("data-vin") + 10, 17);

            if (IsEmpty(result))
                result = node.SelectNodes(".//span[@class='field-value']")?.Where(a => a.PreviousSibling?.InnerText?.ToLower() == "vin:").FirstOrDefault()?.InnerText;

            return result;
        }
    }
}
