using Microsoft.EntityFrameworkCore;
using CarScraper.Web.API.Models;

namespace CarScraper.Web.API.Data
{
    public class CarScraperWebAPIContext : DbContext
    {
        public CarScraperWebAPIContext (DbContextOptions<CarScraperWebAPIContext> options)
            : base(options)
        {
        }

        public DbSet<SearchData> SearchData { get; set; }
    }
}
