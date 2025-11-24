using System.Text.Json.Serialization;

namespace weather_app
{
    public class WeatherData
    {
        [JsonPropertyName("location")]
        public Location Location { get; set; }

        [JsonPropertyName("current")]
        public Current Current { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("localtime")]
        public string LocalTime { get; set; }
    }

    public class Current
    {
        [JsonPropertyName("temp_c")]
        public double TempC { get; set; }

        [JsonPropertyName("condition")]
        public Condition Condition { get; set; }

        [JsonPropertyName("wind_kph")]
        public double WindKph { get; set; }

        [JsonPropertyName("pressure_mb")]
        public double PressureMb { get; set; }

        [JsonPropertyName("humidity")]
        public double Humidity { get; set; }

        [JsonPropertyName("feelslike_c")]
        public double FeelsLikeC { get; set; }

        [JsonPropertyName("vis_km")]
        public double VisibilityKm { get; set; }

        [JsonPropertyName("uv")]
        public double UV { get; set; }
    }

    public class Condition
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }
    }
}