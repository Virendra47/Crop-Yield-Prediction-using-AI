using System.ComponentModel.DataAnnotations;

namespace CropYieldPredictor.ViewModels
{
    public class YieldPredictionViewModel
    {
        [Required]
        [Range(0, 5000, ErrorMessage = "Rainfall must be between 0 and 5000 mm.")]
        [Display(Name = "Rainfall (mm)")]
        public float Rainfall { get; set; }

        [Required]
        [Range(0, 200, ErrorMessage = "Fertilizer amount is invalid.")]
        [Display(Name = "Fertilizer")]
        public float Fertilizer { get; set; }

        [Required]
        [Range(-10, 60, ErrorMessage = "Temperature must be between -10 and 60 °C.")]
        [Display(Name = "Temperature (°C)")]
        public float Temperature { get; set; }

        [Required]
        [Range(0, 200, ErrorMessage = "Nitrogen level is invalid.")]
        [Display(Name = "Nitrogen (N)")]
        public float Nitrogen { get; set; }

        [Required]
        [Range(0, 200, ErrorMessage = "Phosphorus level is invalid.")]
        [Display(Name = "Phosphorus (P)")]
        public float Phosphorus { get; set; }

        [Required]
        [Range(0, 200, ErrorMessage = "Potassium level is invalid.")]
        [Display(Name = "Potassium (K)")]
        public float Potassium { get; set; }

        public float? PredictedYield { get; set; }
    }
}
