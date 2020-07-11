using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace CarScrapper.Core
{
    public static class Util
    {
        private static string _workingDirectory = new FileInfo(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).AbsolutePath).Directory.FullName;

        public static IList<DealerInfo> GetDealers()
        {
            try
            {
                var result = new List<DealerInfo>();
                var configPath = _workingDirectory + "\\Config\\dealers.json";
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

        public static void SerializeSearchResults(IList<CarInfo> searchResults, string searchID)
        {
            if (!Directory.Exists(_workingDirectory + "\\SearchResults"))
                Directory.CreateDirectory(_workingDirectory + "\\SearchResults");

            using (StreamWriter file = File.CreateText(_workingDirectory + "\\SearchResults\\Result_" + searchID + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, searchResults);
            }
        }

        public static IList<CarInfo> DeserializeSearchResults(string searchID)
        {
            try
            {
                using (StreamReader file = File.OpenText(_workingDirectory + "\\SearchResults\\Result_" + searchID + ".json"))
                {
                    return (List<CarInfo>)new JsonSerializer().Deserialize(file, typeof(List<CarInfo>));
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public static string ConvertDateTimeToSearchID(DateTime dt)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
        }

        public static bool IsSearchCompleted(string searchID)
        {
            return File.Exists(_workingDirectory + "\\SearchResults\\Result_" + searchID + ".json");
        }
    }
}
