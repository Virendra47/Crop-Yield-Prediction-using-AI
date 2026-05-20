using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CropYieldPredictor.ViewModels
{
    public class DashboardViewModel
    {
        public string LocationQuery { get; set; } = "Pune, Maharashtra, India";
        public string ResolvedLocation { get; set; } = "Pune, Maharashtra, India";
        public string Country { get; set; } = "India";

        [Display(Name = "Annual Rainfall (mm)")]
        public float Rainfall { get; set; } = 800f;

        [Display(Name = "Average Temperature (°C)")]
        public float Temperature { get; set; } = 26f;

        [Range(0, 200)]
        [Display(Name = "Nitrogen (N)")]
        public float Nitrogen { get; set; } = 80f;

        [Range(0, 200)]
        [Display(Name = "Phosphorus (P)")]
        public float Phosphorus { get; set; } = 24f;

        [Range(0, 200)]
        [Display(Name = "Potassium (K)")]
        public float Potassium { get; set; } = 20f;

        [Range(0, 200)]
        [Display(Name = "Fertilizer")]
        public float Fertilizer { get; set; } = 80f;

        // Yield Predictions for each crop type
        public Dictionary<string, float> CropYields { get; set; } = new Dictionary<string, float>();
    }
}
