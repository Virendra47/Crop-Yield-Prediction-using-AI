using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CropYieldPredictor.Models;
using CropYieldPredictor.ViewModels;
using CropYieldPredictor.Services;
using Microsoft.ML;

namespace CropYieldPredictor.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly PredictionEngine<CropData, CropPrediction> _predictionEngine;
    private readonly WeatherService _weatherService;

    public HomeController(PredictionEngine<CropData, CropPrediction> predictionEngine, WeatherService weatherService)
    {
        _predictionEngine = predictionEngine;
        _weatherService = weatherService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            LocationQuery = "Pune, Maharashtra, India",
            ResolvedLocation = "Pune, Maharashtra, India",
            Country = "India",
            Nitrogen = 80,
            Phosphorus = 24,
            Potassium = 20,
            Fertilizer = 80
        };

        // Get initial weather for Pune
        var (rainfall, temp, resolved, country) = await _weatherService.GetWeatherDataAsync(model.LocationQuery);
        model.Rainfall = rainfall;
        model.Temperature = temp;
        model.ResolvedLocation = resolved;
        model.Country = country;

        // Perform initial multi-crop prediction
        CalculateYields(model);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(DashboardViewModel model, string actionType)
    {
        if (actionType == "search" && !string.IsNullOrWhiteSpace(model.LocationQuery))
        {
            var (rainfall, temp, resolved, country) = await _weatherService.GetWeatherDataAsync(model.LocationQuery);
            model.Rainfall = rainfall;
            model.Temperature = temp;
            model.ResolvedLocation = resolved;
            model.Country = country;
        }

        // Calculate yields for all crops
        CalculateYields(model);

        return View(model);
    }

    private void CalculateYields(DashboardViewModel model)
    {
        var crops = new[] { "Rice", "Wheat", "Maize", "Cotton" };
        foreach (var crop in crops)
        {
            var input = new CropData
            {
                CropType = crop,
                Rainfall = model.Rainfall,
                Fertilizer = model.Fertilizer,
                Temperature = model.Temperature,
                Nitrogen = model.Nitrogen,
                Phosphorus = model.Phosphorus,
                Potassium = model.Potassium
            };

            var prediction = _predictionEngine.Predict(input);
            // Ensure predicted yield is not negative
            model.CropYields[crop] = System.Math.Max(0f, prediction.PredictedYield);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
