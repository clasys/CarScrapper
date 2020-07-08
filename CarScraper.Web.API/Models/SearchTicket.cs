namespace CarScraper.Web.API.Models
{
    public class SearchTicket
    {
        public string SearchKey { get; set; }
        public Duration RetryAfter { get; set; }
        public string KeyRetrievalEndpoint { get; set; }
    }
}
