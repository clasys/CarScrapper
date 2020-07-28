using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace CarScrapper.Core.Tests
{
    [TestClass]
    public class ProcessorTests
    {
        [TestMethod]
        public void TestScrapDealerOn()
        {
            var preferences = new[] { new ProcessingPreferences(new DealerOnSelector("BMW", "X2", InventoryType.New, Regions.Northeast)) };
            var processor = new Processor(preferences);
            var start = DateTime.Now;
            var result = processor.Scrap();
            Debug.WriteLine("Scraping took " + (DateTime.Now - start).TotalMilliseconds + " ms");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void TestScrapDealerInspire()
        {
            var preferences = new[] { new ProcessingPreferences(new DealerInspireSelector("BMW", "X1", InventoryType.New, Regions.Northeast)) };
            var processor = new Processor(preferences);
            var result = processor.Scrap();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void TestScrapDealerCom()
        {
            var preferences = new[] { new ProcessingPreferences(new DealerComSelector("BMW", "X3", InventoryType.New, Regions.Northeast)) };
            var processor = new Processor(preferences);
            var result = processor.Scrap();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void TestScrapCustomCaddy()
        {
            var preferences = new[] { new ProcessingPreferences(new CustomCaddySelector("XT5", InventoryType.New, Regions.All)) };
            var processor = new Processor(preferences);
            var result = processor.Scrap();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void TestScrapToFile()
        {
            //var preferences = new ProcessingPreferences(new DealerOnSelector("BMW", "X2", InventoryType.New));
            //var processor = new Processor(preferences);

            //var searchID = Util.ConvertDateTimeToSearchID(DateTime.Now);
            //processor.ScrapToFile(searchID);
            Assert.IsTrue(true);
        }
    }

    [TestClass]
    public class UtilTests
    {
        [TestMethod]
        public void TestSerializeDeserializeSearchResults()
        {
            var searchResults = new List<CarInfo>
            {
                new CarInfo
                {
                    BodyStyle = "SUV",
                    DealerName = "Edison BMW",
                    DriveType = "AWD",
                    Engine = "4 CYL",
                    ExteriorColor = "White",
                    InteriorColor = "Mocha",
                    IsLoaner = false,
                    Make = "BMW",
                    Model = "X1 xDrive328i",
                    MSRP = "$44,587",
                    Transmission = "Automatic",
                    URL = "www.testurl.com",
                    VIN = "VWER3454545454",
                    WebSite = "www.testurl.com",
                    IPacket = "www.ipacket.com",
                    ModelCode = "2X",
                    StockNumber = "1CDFG"
                },
                new CarInfo
                {
                    BodyStyle = "SUV",
                    DealerName = "BMW of Roschester",
                    DriveType = "FWD",
                    Engine = "4 CYL",
                    ExteriorColor = "Black",
                    InteriorColor = "Oister",
                    IsLoaner = false,
                    Make = "BMW",
                    Model = "X1 sDrive328i",
                    MSRP = "$39,500",
                    Transmission = "Automatic",
                    URL = "www.testurl.com",
                    VIN = "VWER345467878",
                    WebSite = "www.testurl.com",
                    IPacket = "www.ipacket.com",
                    ModelCode = "2X",
                    StockNumber = "1CDLKJ"
                }
            };

            var id = Util.ConvertDateTimeToSearchID(DateTime.Now);
            
            Util.SerializeSearchResults(searchResults, id);

            var desResults = Util.DeserializeSearchResults(id);
            
            Assert.IsNotNull(desResults);
            Assert.IsTrue(desResults.Count() > 0);
        }
    }
}
