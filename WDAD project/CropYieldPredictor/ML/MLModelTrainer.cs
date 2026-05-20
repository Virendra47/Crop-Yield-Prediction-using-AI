using System;
using System.IO;
using Microsoft.ML;
using CropYieldPredictor.Models;

namespace CropYieldPredictor.ML
{
    public static class MLModelTrainer
    {
        private static readonly string ModelPath = Path.Combine(Environment.CurrentDirectory, "MLModel.zip");
        private static readonly string DataPath = Path.Combine(Environment.CurrentDirectory, "crop_yield_enriched.csv");

        public static void TrainAndSaveModel(bool forceRetrain = false)
        {
            if (File.Exists(ModelPath) && !forceRetrain)
            {
                return; // Model already trained
            }

            if (!File.Exists(DataPath))
            {
                Console.WriteLine($"Data file not found at {DataPath}. Please ensure crop_yield_enriched.csv is in the project root.");
                return;
            }

            MLContext mlContext = new MLContext(seed: 0);

            // 1. Load Enriched Data
            IDataView dataView = mlContext.Data.LoadFromTextFile<CropData>(DataPath, hasHeader: true, separatorChar: ',');

            // Split data into train and test sets
            var split = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            // 2. Build Pipeline with Categorical OneHotEncoding for CropType
            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding("CropTypeEncoded", nameof(CropData.CropType))
                .Append(mlContext.Transforms.Concatenate("Features", 
                    "CropTypeEncoded",
                    nameof(CropData.Rainfall), 
                    nameof(CropData.Fertilizer), 
                    nameof(CropData.Temperature), 
                    nameof(CropData.Nitrogen), 
                    nameof(CropData.Phosphorus), 
                    nameof(CropData.Potassium)))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.Regression.Trainers.FastTree(labelColumnName: nameof(CropData.Yield), featureColumnName: "Features"));

            // 3. Train Model
            Console.WriteLine("Training upgraded multi-crop model...");
            var model = pipeline.Fit(split.TrainSet);

            // 4. Evaluate Model
            var predictions = model.Transform(split.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: nameof(CropData.Yield));
            
            Console.WriteLine($"Upgraded Model Evaluation Metrics:");
            Console.WriteLine($"* R-Squared: {metrics.RSquared:0.###}");
            Console.WriteLine($"* Root Mean Squared Error: {metrics.RootMeanSquaredError:0.###}");

            // 5. Save Model (forces overwrite if it exists)
            mlContext.Model.Save(model, dataView.Schema, ModelPath);
            Console.WriteLine($"Upgraded model saved to {ModelPath}");
        }

        public static PredictionEngine<CropData, CropPrediction> GetPredictionEngine()
        {
            MLContext mlContext = new MLContext(seed: 0);
            ITransformer loadedModel = mlContext.Model.Load(ModelPath, out var modelInputSchema);
            return mlContext.Model.CreatePredictionEngine<CropData, CropPrediction>(loadedModel);
        }
    }
}
