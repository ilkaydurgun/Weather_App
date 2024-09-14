using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Weather_App.Context;
using Weather_App.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Weather_App.Controllers
{
    public class WeathersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _apiKey = "0fe12a9337bb8b7c15bee01ae9bcca92"; // OpenWeatherMap API anahtarı
        private readonly ILogger<WeathersController> _logger;

        public WeathersController(ApplicationDbContext context, ILogger<WeathersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.WeatherApps.ToListAsync());
        }

        public async Task<IActionResult> UpdateWeatherData()
        {
            var cities = new List<string>
            {
               "Adana", "Adıyaman", "Afyonkarahisar", "Ağrı", "Aksaray", "Amasya", "Ankara", "Antalya", "Artvin", "Aydın",
                "Balıkesir", "Bartın", "Batman", "Bayburt", "Bilecik", "Bingöl", "Bitlis", "Bolu", "Burdur", "Bursa",
                "Çanakkale", "Çankırı", "Çorum", "Denizli", "Diyarbakır", "Düzce", "Edirne", "Elazığ", "Erzincan", "Erzurum",
                "Eskişehir", "Gaziantep", "Giresun", "Gümüşhane", "Hakkari", "Hatay", "Iğdır", "Isparta", "İstanbul", "İzmir",
                "Kahramanmaraş", "Karabük", "Karaman", "Kars", "Kastamonu", "Kayseri", "Kırıkkale", "Kırklareli", "Kırşehir", "Kocaeli",
                "Konya", "Kütahya", "Malatya", "Manisa", "Mardin", "Mersin", "Muğla", "Muş", "Nevşehir", "Niğde",
                "Ordu", "Osmaniye", "Rize", "Sakarya", "Samsun", "Siirt", "Sinop", "Sivas", "Şanlıurfa", "Şırnak",
                "Tekirdağ", "Tokat", "Trabzon", "Tunceli", "Uşak", "Van", "Yalova", "Yozgat", "Zonguldak"
            };

            var allWeatherData = new List<Weather>();

            foreach (var city in cities)
            {
                try
                {
                    var weatherData = await GetWeatherFromApi(city);
                    if (weatherData != null)
                    {
                        var weather = new Weather
                        {
                            City = weatherData.name,
                            Date = DateTime.Now,
                            Temperature = (int)weatherData.main.temp,
                            State = weatherData.weather[0].description
                        };

                        allWeatherData.Add(weather);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error fetching weather data for {city}: {ex.Message}");
                }
            }

            try
            {
                foreach (var weather in allWeatherData)
                {
                    var existingWeather = await _context.WeatherApps
                        .FirstOrDefaultAsync(w => w.City == weather.City && w.Date.Date == weather.Date.Date);

                    if (existingWeather != null)
                    {
                        existingWeather.Temperature = weather.Temperature;
                        existingWeather.State = weather.State;
                        _context.Update(existingWeather);
                    }
                    else
                    {
                        _context.Add(weather);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"Database update concurrency error: {ex.Message}");
               
            }

            return Json(allWeatherData);
        }

        private async Task<dynamic> GetWeatherFromApi(string city)
        {
            using (var client = new HttpClient())
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
                try
                {
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    }
                    else
                    {
                        _logger.LogWarning($"API request for {city} failed with status code {response.StatusCode}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error fetching weather data from API: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weather = await _context.WeatherApps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weather == null)
            {
                return NotFound();
            }

            return View(weather);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,City,Date,Temperature,State")] Weather weather)
        {
            if (ModelState.IsValid)
            {
                _context.Add(weather);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(weather);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weather = await _context.WeatherApps.FindAsync(id);
            if (weather == null)
            {
                return NotFound();
            }
            return View(weather);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,City,Date,Temperature,State")] Weather weather)
        {
            if (id != weather.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weather);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeatherExists(weather.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(weather);
        }

        public IActionResult Filter()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Filter(string city, DateTime startDate, DateTime endDate)
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
                return NotFound($"No weather data found for {city} between {startDate.ToShortDateString()} and {endDate.ToShortDateString()}.");
            }

            return View("WeatherResults", weatherData);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weather = await _context.WeatherApps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weather == null)
            {
                return NotFound();
            }

            return View(weather);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var weather = await _context.WeatherApps.FindAsync(id);
            _context.WeatherApps.Remove(weather);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeatherExists(int id)
        {
            return _context.WeatherApps.Any(e => e.Id == id);
        }
    }
}
