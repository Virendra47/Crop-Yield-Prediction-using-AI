using Microsoft.ML.Data;

namespace CropYieldPredictor.Models
{
    public class CropData
    {
        [LoadColumn(0)]
        public string CropType { get; set; }

        [LoadColumn(1)]
        public float Rainfall { get; set; }

        [LoadColumn(2)]
        public float Fertilizer { get; set; }

        [LoadColumn(3)]
        public float Temperature { get; set; }

        [LoadColumn(4)]
        public float Nitrogen { get; set; }

        [LoadColumn(5)]
        public float Phosphorus { get; set; }

        [LoadColumn(6)]
        public float Potassium { get; set; }

        [LoadColumn(7)]
        public float Yield { get; set; }
    }

    public class CropPrediction
    {
        [ColumnName("Score")]
        public float PredictedYield { get; set; }
    }
}
