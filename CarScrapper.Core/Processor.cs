using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrapySharp;
using ScrapySharp.Html.Forms;
using ScrapySharp.Network;
using ScrapySharp.Extensions;
using HtmlAgilityPack;
using System.Diagnostics;

namespace CarScrapper.Core
{
    public class Processor
    {
        private ProcessingPreferences _preferences;
        private ScrapingBrowser _browser = new ScrapingBrowser();

        public void Test()
        {
            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage homePage = browser.NavigateToPage(new Uri("https://www.flemingtonbmw.com/searchnew.aspx?Model=X3&pn=100"));

            /*PageWebForm form = homePage.FindFormById("sb_form");
            form["q"] = "scrapysharp";
            form.Method = HttpVerb.Get;
            WebPage resultsPage = form.Submit();*/

            //HtmlNode[] resultsLinks = homePage.Html.CssSelect("div.srpRow-5UXTY5C00L9B76207").ToArray();
            var rows = homePage.Html.SelectNodes(".//div[contains(@id, 'srpRow')]");

            //WebPage blogPage = resultsPage.FindLinks(By.Text("romcyber blog | Just another WordPress site")).Single().Click();
            Debugger.Break();
        }

        public Processor(ProcessingPreferences preferences)
        {
            _preferences = preferences;
        }

        public IList<CarInfo> Scrap()
        {
            List<CarInfo> result = new List<CarInfo>();

            foreach (var site in _preferences.Uris)
            {
                var page = _browser.NavigateToPage(new Uri(site + _preferences.ProcessingSelector.GetUriDetails()));

                HtmlNodeCollection rows = null;
                foreach (var rowSelector in _preferences.ProcessingSelector.GetRowSelectors())
                {
                    rows = page.Html.SelectNodes(rowSelector);
                    if (rows != null)
                        break;
                }

                if (rows != null)
                {
                    var selector = _preferences.ProcessingSelector;

                    rows.ToList().ForEach(row => {
                        var carInfo = selector.ParseHtmlIntoCarInfo(row);
                        carInfo.WebSite = site;

                        var map = selector.GetCleanupMap();
                        if (map != null)
                        {
                            map.ForEach(e => { carInfo.GetType().GetProperty(e.Item1).SetValue(carInfo, carInfo.GetType().GetProperty(e.Item1).GetValue(carInfo)?.ToString().Replace(e.Item2, "").Trim()); });
                        }

                        var regexMap = selector.GetRegexMap();
                        if (regexMap != null)
                        {
                            regexMap.ForEach(a => 
                            { 
                                if(carInfo.GetType().GetProperty(a.Item1).GetValue(carInfo) != null)
                                    carInfo.GetType().GetProperty(a.Item1).SetValue(carInfo, a.Item2.Replace(carInfo.GetType().GetProperty(a.Item1).GetValue(carInfo)?.ToString(), " ").Trim()); 
                            });
                        }

                        result.Add(carInfo);
                    });
                }
            }

            return result;
        }
    }
}
