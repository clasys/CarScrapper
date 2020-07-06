using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace CarScrapper.Core
{
    public static class Util
    {
        public static IList<DealerInfo> GetDealers()
        {
            try
            {
                var result = new List<DealerInfo>();
                var configPath = new FileInfo(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).AbsolutePath).Directory.FullName + "\\Config\\dealers.json";
                var config = JObject.Parse(File.ReadAllText(configPath));
                foreach (var dealer in ((JArray)config["dealers"])) //.Where(a => ((string)a["make"]).ToLower() == Make.ToLower()))
                {
                    result.Add(new DealerInfo
                    {
                        Type = (string)dealer["dealertype"],
                        Name = (string)dealer["name"],
                        Url = (string)dealer["url"],
                        Make = (string)dealer["make"],
                        CustomUrl = dealer["customurl"] == null ? null : (string)dealer["customurl"],
                        LoanerUrl = dealer["loanerurl"] == null ? null : (string)dealer["loanerurl"]
                    });
                }

                return result;
            }
            catch(Exception e)
            {
                NLogger.Instance.Error(e, "Unable to read dealer info from json config file.");
                throw e;
            }
        }
    }
}
