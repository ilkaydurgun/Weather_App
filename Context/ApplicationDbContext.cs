using Microsoft.EntityFrameworkCore;
using Weather_App.Models;

namespace Weather_App.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Weather> WeatherApps { get; set; }
    }
}
