using NLog;

namespace CarScrapper.Core
{
    public static class NLogger
    {
        public static Logger Instance { get; private set; }

        static NLogger()
        {
            LogManager.ReconfigExistingLoggers();
            Instance = LogManager.GetCurrentClassLogger();
        }
    }
}
