# AgriPredict Intelligence: Project Summary & Contributions

This document provides a simple explanation of the **AgriPredict Intelligence** project, its architecture, APIs, Machine Learning model, and the split of contributions between the two team partners.

---

## 🌟 1. Project Purpose (Simple Overview)
**AgriPredict Intelligence** is an agricultural decision-support web dashboard. 

It helps farmers, agronomists, and researchers determine which crops are best suited for a specific region. The application predicts the expected yield (in **quintals per acre**) for four major crops:
* **Rice** 🌾
* **Wheat** 🌾
* **Maize** 🌽
* **Cotton** ☁️

Predictions are calculated using a combination of:
1. **Soil Properties**: Nitrogen (N), Phosphorus (P), Potassium (K), and total Fertilizer levels (inputted by the user via sliders).
2. **Climate Factors**: Annual Rainfall (in mm) and Average Temperature (in °C) (automatically fetched using APIs based on the user's searched city/region).

---

## 💻 2. How the System Works (Step-by-Step)
1. **User Search**: The user enters a location (e.g. *"Pune, India"*) and adjusts the soil chemical sliders.
2. **Weather API Request**: The backend C# service queries external weather APIs to fetch the region's climate history (rainfall & temperature).
3. **ML Prediction**: The combined soil and weather statistics are sent to a local Machine Learning model built with **ML.NET**.
4. **Dashboard Display**: The system calculates the estimated crop yields and updates the dashboard, displaying a comparison bar chart (using **Chart.js**) and highlighting the crop with the highest yield.

---

## 📡 3. The Web APIs Used
The application calls **two** public Web APIs provided by **Open-Meteo**:
1. **Geocoding API** (`https://geocoding-api.open-meteo.com/v1/search`):
   * *Purpose*: Converts the text name of a location (e.g., *"Pune"*) into coordinates (Latitude and Longitude).
2. **Weather Archive API** (`https://archive-api.open-meteo.com/v1/archive`):
   * *Purpose*: Takes the coordinates and fetches historical daily weather data (temperature and rain precipitation) for the region. The backend compiles this data to get annual rainfall and average temperature.
3. **Fail-safe Fallbacks**: If the API goes offline or the internet is disconnected, the service automatically falls back to default values (800mm rainfall, 26°C temperature) so the application never crashes.

---

## 🧠 4. The Machine Learning (ML) Model
* **Technology**: **ML.NET** (Microsoft's official ML framework for C# .NET Core).
* **Algorithm**: **FastTree Regression** (a high-performance decision-tree algorithm for prediction tasks).
* **Model Training (`MLModelTrainer.cs`)**: Trains on a local dataset (`crop_yield_enriched.csv`) on application startup and generates the trained model file **`MLModel.zip`**.
* **Model Prediction (`HomeController.cs`)**: Loads `MLModel.zip` into memory and runs the prediction engine when the user requests a simulation.

---

## 🧑‍🤝‍🧑 5. Team Contribution Division (2-Person Split)

To present your work to examiners, you can split your responsibilities into **Frontend UI Development** and **Backend & ML Engineering**.

### Partner A: Frontend Developer & UI Integrator
* **Interface & Layout**: Designed and developed the glassmorphic, responsive CSS layout and cards in the Razor views (**`Index.cshtml`**, **`_Layout.cshtml`**).
* **Data Visualization**: Integrated **Chart.js** library to build the double-bar/comparison chart that visually shows the comparison of predicted yields.
* **Interactive Elements**: Programmed the frontend JavaScript sliders so that changing Nitrogen, Phosphorus, and Potassium inputs dynamically updates the UI value indicators.
* **Authentication Screens**: Built and styled the Login and Registration screens (**`Login.cshtml`** and **`Register.cshtml`**).

### Partner B: Backend & Machine Learning Engineer
* **Machine Learning Pipeline**: Built the ML pipeline in **`MLModelTrainer.cs`**, setting up data schemas, category encoding (OneHotEncoding for CropType), and model training using **FastTree Regression**.
* **External API Integration**: Created the **`WeatherService.cs`** to handle network calls (`HttpClient`), parse JSON schemas, and implement fail-safe fallback values.
* **Controller Logic**: Implemented the server routing and action methods in **`HomeController.cs`** and **`AccountController.cs`**.
* **Database & Security**: Programmed the local JSON user database storage and SHA-256 secure password hashing in **`UserService.cs`** (storing credentials securely in `users.json`).
