using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RentACarAPI.Contexts;
using RentACarAPI.Models;

namespace RentACarAPI.Services
{
    public class RentingEventsBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval;
        private readonly ILogger<RentingEventsBackgroundService> _logger;
        private static readonly Random _random = new Random();

        public RentingEventsBackgroundService(IServiceProvider serviceProvider, ILogger<RentingEventsBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _checkInterval = TimeSpan.FromMinutes(1); // Set desired interval
            _logger = logger;
        }

        private static double GenerateRandomCoordinate(double min, double max)
        {
            return _random.NextDouble() * (max - min) + min;
        }

        public static (double latitude, double longitutde) GenerateRandomCoordinates()
        {
            // Latitude and longitude boundaries for Cluj-Napoca
            const double minLatitude = 46.70;
            const double maxLatitude = 46.80;
            const double minLongitude = 23.50;
            const double maxLongitude = 23.70;

            double latitude = GenerateRandomCoordinate(minLatitude, maxLatitude);
            double longitude = GenerateRandomCoordinate(minLongitude, maxLongitude);

            return (latitude, longitude);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Renting Background starting..");

            while(!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var currentTime = DateTime.Now;

                    try
                    {
                        // Query for renting events that have just ended within last 1 minute
                        var endedRentingEvents = await _dataContext.RentingEvents
                            .Include(re => re.Car)
                            .Where(re => re.RentalEndDate > currentTime.AddMinutes(-1) && re.RentalEndDate <= currentTime)
                            .ToListAsync();

                        if(!endedRentingEvents.Any())
                        {
                            _logger.LogWarning("Not renting ended in the last x minutes");
                        }

                        foreach (var rentingEvent in endedRentingEvents)
                        {
                            var position = await _dataContext.Positions.SingleOrDefaultAsync(p => p.Id == rentingEvent.Car.PositionId);

                            if (position == null)
                            {
                                _logger.LogWarning("No position detected!");
                                continue;
                            }

                            (position.Latitude, position.Longitude) = GenerateRandomCoordinates();

                            _logger.LogCritical("New positions: " + JsonConvert.SerializeObject(position, Formatting.Indented) + "\n From Renting event: " + JsonConvert.SerializeObject(rentingEvent, Formatting.Indented));

                            await _dataContext.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occured in Renting Events Background service");
                    }
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

    }
}
