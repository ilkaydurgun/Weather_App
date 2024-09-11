using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using Weather_App.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Weather_App.Context;

namespace Weather_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;  // Veritabaný baðlamý (DbContext)

        // Constructor - DbContext injection
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;  // Veritabaný baðlamý initialize edilir
        }
        [HttpGet]
        public IActionResult CityWeather()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CityWeather(string city, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(city) || startDate == default || endDate == default)
            {
                return BadRequest("Please provide valid city name and dates.");
            }

            var weatherData = await _context.WeatherApps
                .Where(w => w.City == city && w.Date >= startDate && w.Date <= endDate)
                .ToListAsync();

            if (weatherData == null || weatherData.Count == 0)
            {
                return View(new List<Weather_App.Models.Weather>()); // Return an empty list
            }

            return View(weatherData);
        }
        // Index action - Veritabanýndan verileri çekip view'e gönderir
        public async Task<IActionResult> Index()
        {
            // Veritabanýndaki WeatherApp tablosundaki tüm verileri çekiyoruz
            var weatherData = await _context.WeatherApps.ToListAsync();

            // Verileri View'e (Index.cshtml) gönderiyoruz
            return View(weatherData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
