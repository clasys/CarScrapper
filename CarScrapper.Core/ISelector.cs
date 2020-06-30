using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CarScrapper.Core
{
    public interface ISelector
    {
        string[] GetRowSelectors();
        string[] GetInfoSeparator();
        string GetMsrpIdentifier();
        string GetMakeIdentifier();
        string GetExtColorIdentifier();
        string GetIntColorIdentifier();
        string GetModelIdentifier();
        string GetUrlDetails();
        string GetEngineIdentifier();
        string GetDriveTypeIdentifier();
        string GetStockNumberIdentifier();
        string GetVinIdentifier();
        string GetCarUrlIdentifier();
        string GetBodyStyleIdentifier();
        string GetModelCodeIdentifier();
        string GetTransmissionIdentifier();
        List<Tuple<string, string>> GetCleanupMap();
        List<Tuple<string, Regex>> GetRegexMap();
        CarInfo ParseHtmlIntoCarInfo(HtmlNode node);
        List<DealerInfo> GetDealers();
    }
}
