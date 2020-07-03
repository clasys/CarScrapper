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
using System.Threading;
using System.Net;

namespace CarScrapper.Core
{
    public class Processor
    {
        private ProcessingPreferences _preferences;
        //private ScrapingBrowser _browser = new ScrapingBrowser();

        public Processor(ProcessingPreferences preferences)
        {
            _preferences = preferences;
        }

        public IList<CarInfo> Scrap()
        {
#if DEBUG
            var m = DateTime.Now;
#endif
            List<CarInfo> result = new List<CarInfo>();

            foreach (var dealer in _preferences.ProcessingSelector.GetDealers())
            {
#if DEBUG
                //NLogger.Instance.Info(string.Format("Starting scrape for dealer {0}, URL {1}", dealer.Name, dealer.Url));
                var s = DateTime.Now;
#endif
                HtmlAgilityPack.HtmlDocument doc = LoadWebSiteOld(dealer.Url + _preferences.ProcessingSelector.GetUrlDetails());
                var pagingInfo = _preferences.ProcessingSelector.GetPagingInfo(doc);

                if (pagingInfo.IsEnabled)
                {
                    var multiple = ScrapMultiple(pagingInfo, dealer);
                    result.AddRange(multiple);
#if DEBUG
                    Debug.WriteLine(string.Format("***************** Dealer {0} done. {1} cars. {2} sec", dealer.Name, multiple.GroupBy(a => a.VIN).Select(a => a.First()).Count(), (DateTime.Now - s).TotalSeconds));
                    NLogger.Instance.Info(string.Format("Finish scrape for dealer {0}, URL {1} ({2} ms)", dealer.Name, dealer.Url, (DateTime.Now - s).TotalMilliseconds));
#endif
                }
            }
#if DEBUG
            Debug.WriteLine(string.Format("***************** ALL done. {0} cars. {1} sec", result.GroupBy(a => a.VIN).Select(a => a.First()).Count(), (DateTime.Now - m).TotalSeconds));
            //NLogger.Instance.Info(string.Format("Starting scrape for dealer {0}, URL {1}", dealer.Name, dealer.Url));
#endif
            //remove dupes created by different page sizes on different websites
            return result.GroupBy(a=> a.VIN).Select(a=>a.First()).ToList();
        }

        

        private List<CarInfo> ScrapMultiple(PagingInfo pagingInfo, DealerInfo dealer)
        {
            var result = new List<CarInfo>();

            foreach (var pagedUrl in pagingInfo.PagedUrls)
            {
                //HtmlAgilityPack.HtmlDocument doc = LoadWebSiteAsync(dealer.Url + pagedUrl);
                //var node = LoadWebSiteScrapySharp(dealer.Url + pagedUrl);
                HtmlDocument doc = LoadWebSiteOld(dealer.Url + pagedUrl);

                HtmlNodeCollection rows = null;
                foreach (var rowSelector in _preferences.ProcessingSelector.GetRowSelectors())
                {
                    rows = doc?.DocumentNode.SelectNodes(rowSelector);
                    if (rows != null)
                        break;
                }

                if (rows != null)
                {
                    var selector = _preferences.ProcessingSelector;

                    rows.ToList().ForEach(row =>
                    {
                        var carInfo = selector.ParseHtmlIntoCarInfo(row, dealer);
                        carInfo.WebSite = dealer.Url;
                        carInfo.DealerName = dealer.Name;

                        var map = selector.GetCleanupMap();
                        if (map != null)
                            map.ForEach(e => { carInfo.GetType().GetProperty(e.Item1).SetValue(carInfo, carInfo.GetType().GetProperty(e.Item1).GetValue(carInfo)?.ToString().Replace(e.Item2, "").Trim()); });

                        var regexMap = selector.GetRegexMap();
                        if (regexMap != null)
                        {
                            regexMap.ForEach(a =>
                            {
                                if (carInfo.GetType().GetProperty(a.Item1).GetValue(carInfo) != null)
                                    carInfo.GetType().GetProperty(a.Item1).SetValue(carInfo, a.Item2.Replace(carInfo.GetType().GetProperty(a.Item1).GetValue(carInfo)?.ToString(), " ").Trim());
                            });
                        }

                        result.Add(carInfo);
                    });
                }
            }

            return result;
        }

        private HtmlDocument LoadWebSiteOld(string url)
        {
            try
            {
                return new HtmlWeb().Load(url);
            }
            catch (WebException e)
            {
                //TODO: log it here for which website it happened and continue
                NLogger.Instance.Error(e, string.Format("Error connecting to {0}", url));
                return null;
            }
        }

        //public void Test()
        //{
        //    ScrapingBrowser browser = new ScrapingBrowser();
        //    WebPage homePage = browser.NavigateToPage(new Uri("https://www.flemingtonbmw.com/searchnew.aspx?Model=X3&pn=100"));

        //    /*PageWebForm form = homePage.FindFormById("sb_form");
        //    form["q"] = "scrapysharp";
        //    form.Method = HttpVerb.Get;
        //    WebPage resultsPage = form.Submit();*/

        //    //HtmlNode[] resultsLinks = homePage.Html.CssSelect("div.srpRow-5UXTY5C00L9B76207").ToArray();
        //    var rows = homePage.Html.SelectNodes(".//div[contains(@id, 'srpRow')]");

        //    //WebPage blogPage = resultsPage.FindLinks(By.Text("romcyber blog | Just another WordPress site")).Single().Click();
        //    Debugger.Break();
        //}

        //private HtmlNode LoadWebSiteScrapySharp(string url)
        //{
        //    _browser.IgnoreCookies = true;
        //    WebPage page = _browser.NavigateToPageAsync(new Uri(url)).Result;
        //    return page.Html;

        //    //return Task.Run(() =>
        //    //{
        //    //    _browser.IgnoreCookies = true;
        //    //    WebPage page = _browser.NavigateToPageAsync(new Uri(url));
        //    //    return page.Html;
        //    //}).Result;

        //}

        /// <summary>
        /// Loads web site with delay until all scripts are executed
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        //private HtmlAgilityPack.HtmlDocument LoadWebSite(string url)
        //{
        //    HtmlAgilityPack.HtmlDocument doc = null;

        //    var thread = new Thread(() =>
        //    {
        //        var web = new HtmlWeb();
        //        doc = web.LoadFromBrowser(url, a=> 
        //        {
        //            var browser = (WebBrowser)a;
        //            // WAIT until the dynamic text is set
        //            return !string.IsNullOrEmpty(browser.Document.Body.InnerHtml);
        //        });
        //    });

        //    thread.SetApartmentState(ApartmentState.STA);
        //    thread.Start();
        //    thread.Join();

        //    return doc;
        //}

        //public void Test()
        //{
        //    var url = "https://www.koons.com/new-inventory/index.htm?model=XC60&start=0";

        //    var web1 = new HtmlWeb();
        //    var doc1 = web1.LoadFromBrowser(url, o =>
        //    {
        //        var webBrowser = (WebBrowser)o;

        //        // WAIT until the dynamic text is set
        //        return !string.IsNullOrEmpty(webBrowser.Document.Body.InnerHtml);
        //    });

        //    Debugger.Break();
        //}
    }
}
