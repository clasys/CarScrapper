using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarScrapper.Core
{
    public class ProcessingPreferences
    {
        public ProcessingPreferences(IList<string> uris, ISelector processingSelector)
        {
            Uris = uris;
            ProcessingSelector = processingSelector;
        }

        public ISelector ProcessingSelector { get; }

        public IList<string> Uris { get; }
    }
}
