namespace CarScraper.Web.API.Models
{
    public enum DealerType
    { 
        DealerCom,
        DealerInspire,
        DealerOn,
        All
    }

    public enum Duration
    {
        TenSeconds,
        FifteenSeconds,
        ThirtySeconds,
        SixtySeconds
    }

    public enum SearchResultStatus
    { 
        Success,
        Failure,
        SearchInProgress
    }
}
