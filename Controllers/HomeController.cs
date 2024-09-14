using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Weather_App.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace Weather_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _apiKey = "0fe12a9337bb8b7c15bee01ae9bcca92"; // OpenWeatherMap API anahtar�

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // GET: /Home/CityWeather
        public async Task<IActionResult> CityWeather()
        {
            // Sabit �ehir listesi
            var cities = new List<string> { "Istanbul", "Ankara", "Izmir", "Bursa", "Gaziantep" };

            var weatherDataList = new List<Weather>();

            // Her �ehir i�in hava durumu verisini API'den al
            foreach (var cityName in cities)
            {
                var weatherData = await GetWeatherFromApi(cityName);
                if (weatherData != null)
                {
                    var weather = new Weather
                    {
                        City = weatherData.name,
                        Date = DateTime.Now,
                        Temperature = (int)weatherData.main.temp,
                        State = weatherData.weather[0].description
                    };

                    weatherDataList.Add(weather);
                }
            }

            // �ehirlerin hava durumu verilerini View'e g�nder
            return View("WeatherResults", weatherDataList);
        }

        // Hava durumu API'den veri �ekme
        private async Task<dynamic> GetWeatherFromApi(string city)
        {
            using (var client = new HttpClient())
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<dynamic>(jsonResponse); // Dinamik veri olarak ��z�mle
                }

                return null; // E�er API ba�ar�s�zsa null d�n
            }
        }

  
        public IActionResult Index()
        {
            return RedirectToAction("CityWeather"); // Ana sayfaya y�nlendir
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
