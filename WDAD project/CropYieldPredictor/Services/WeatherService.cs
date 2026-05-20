using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace CropYieldPredictor.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "CropPredictAI-App");
        }

        public async Task<(float rainfall, float temperature, string resolvedName, string country)> GetWeatherDataAsync(string locationName)
        {
            // Fallback default values
            float defaultRainfall = 800f;
            float defaultTemperature = 26f;
            string resolvedName = locationName;
            string country = "Unknown";

            try
            {
                // Step 1: Geocoding
                string geocodeUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(locationName)}&count=1&language=en&format=json";
                var geocodeResponse = await _httpClient.GetStringAsync(geocodeUrl);
                var geocodeData = JsonSerializer.Deserialize<GeocodeResponse>(geocodeResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (geocodeData?.Results == null || geocodeData.Results.Length == 0)
                {
                    return (defaultRainfall, defaultTemperature, resolvedName, country);
                }

                var result = geocodeData.Results[0];
                double lat = result.Latitude;
                double lon = result.Longitude;
                resolvedName = result.Name;
                country = result.Country ?? "Unknown";

                // Step 2: Fetch 2025 Archive Weather
                // We'll query from 2025-01-01 to 2025-12-31 to get a complete annual overview of Rainfall & Temperature
                string archiveUrl = $"https://archive-api.open-meteo.com/v1/archive?latitude={lat:0.0000}&longitude={lon:0.0000}&start_date=2025-01-01&end_date=2025-12-31&daily=temperature_2m_mean,precipitation_sum&timezone=auto";
                var weatherResponse = await _httpClient.GetStringAsync(archiveUrl);
                var weatherData = JsonSerializer.Deserialize<WeatherArchiveResponse>(weatherResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (weatherData?.Daily == null)
                {
                    return (defaultRainfall, defaultTemperature, resolvedName, country);
                }

                // Sum precipitation (annual rainfall)
                float totalRainfall = 0f;
                if (weatherData.Daily.Precipitation_Sum != null)
                {
                    totalRainfall = (float)weatherData.Daily.Precipitation_Sum.Where(x => x.HasValue).Sum(x => x.Value);
                }

                // Average temperature
                float avgTemp = defaultTemperature;
                if (weatherData.Daily.Temperature_2m_Mean != null)
                {
                    var validTemps = weatherData.Daily.Temperature_2m_Mean.Where(x => x.HasValue).Select(x => x.Value).ToList();
                    if (validTemps.Count > 0)
                    {
                        avgTemp = (float)validTemps.Average();
                    }
                }

                // Smooth out extreme values to match dataset ranges
                // Our model is trained on Rainfall (400 - 1300) and Temp (15 - 38)
                float finalRainfall = Math.Max(300f, Math.Min(1500f, totalRainfall));
                float finalTemp = Math.Max(10f, Math.Min(45f, avgTemp));

                return (finalRainfall, finalTemp, resolvedName, country);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weather data: {ex.Message}");
                return (defaultRainfall, defaultTemperature, resolvedName, country);
            }
        }
    }

    #region JSON Schema Mappings
    public class GeocodeResponse
    {
        public GeocodeResult[] Results { get; set; }
    }

    public class GeocodeResult
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Country { get; set; }
    }

    public class WeatherArchiveResponse
    {
        public DailyWeatherData Daily { get; set; }
    }

    public class DailyWeatherData
    {
        public double?[] Temperature_2m_Mean { get; set; }
        public double?[] Precipitation_Sum { get; set; }
    }
    #endregion
}
