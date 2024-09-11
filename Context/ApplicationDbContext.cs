using Microsoft.EntityFrameworkCore;
using Weather_App.Models;

namespace Weather_App.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        // Weather_App is assumed to be a model class. Make sure it's defined correctly.
        public DbSet<Weather> WeatherApps { get; set; }
    }
}
