using System.Collections.Generic;

namespace CarScrapper.Core
{
    public class ProcessingPreferences
    {
        public ProcessingPreferences(ISelector processingSelector)
        {
            ProcessingSelector = processingSelector;
        }

        public ISelector ProcessingSelector { get; }
    }
}
