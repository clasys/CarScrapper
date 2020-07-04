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
            var preferences = new ProcessingPreferences(new DealerOnSelector("BMW", "X2", InventoryType.New));
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
            var preferences = new ProcessingPreferences(new DealerInspireSelector("BMW", "X1", InventoryType.New));
            var processor = new Processor(preferences);
            var result = processor.Scrap();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void TestScrapDealerCom()
        {
            var preferences = new ProcessingPreferences(new DealerComSelector("Volvo", "XC60", InventoryType.Loaner));
            var processor = new Processor(preferences);
            var result = processor.Scrap();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
    }
}
