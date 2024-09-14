using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Weather_App.BackgroundServices
{
    public class WeatherUpdateService : BackgroundService
    {
        private readonly ILogger<WeatherUpdateService> _logger;

        public WeatherUpdateService(ILogger<WeatherUpdateService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("WeatherUpdateService is running.");

                // Hava durumu verilerini güncelleme kodu burada

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
