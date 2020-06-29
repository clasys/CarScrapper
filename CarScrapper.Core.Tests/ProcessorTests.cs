using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CarScrapper.Core.Tests
{
    [TestClass]
    public class ProcessorTests
    {
        [TestMethod]
        public void TestScrapDealerOn()
        {
            var preferences = new ProcessingPreferences(
                new List<string>()
                {
                    "https://www.flemingtonbmw.com",
                    "https://www.bmwedison.com",
                    "https://www.bmwhudsonvalley.com/",
                    "https://www.mbofcaldwell.com/"
                },
                new DealerOnSelector("BMW", "X1"));

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
            var preferences = new ProcessingPreferences(
               new List<string>()
               {
                    "https://www.raycatenaedison.com",
                    "https://www.mbprinceton.com",
                    "https://www.raycatenaunion.com/",
               },
               new DealerInspireSelector("Mercedes-Benz", "GLC"));

            var processor = new Processor(preferences);
            var result = processor.Scrap();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
    }
}
