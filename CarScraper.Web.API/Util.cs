using CarScraper.Web.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CarScraper.Web.API
{
    public class Util
    {
        private IConfiguration _config;
        public Util(IConfiguration config)
        {
            _config = config;
        }

        public CarScraperWebAPIContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CarScraperWebAPIContext>();
            var connectionString = _config.GetConnectionString("CarScraperWebAPIContext");
            optionsBuilder.UseSqlServer(connectionString);
            return new CarScraperWebAPIContext(optionsBuilder.Options);
        }
    }
}
