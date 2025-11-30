using System.Text.Json.Serialization;

namespace weather_app
{
    public class WeatherData
    {
        [JsonPropertyName("location")]
        public Location? Location { get; set; }

        [JsonPropertyName("current")]
        public Current? Current { get; set; }

        [JsonPropertyName("forecast")]
        public Forecast? Forecast { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("localtime")]
        public string? LocalTime { get; set; }
    }

    public class Current
    {
        [JsonPropertyName("temp_c")]
        public double TempC { get; set; }

        [JsonPropertyName("condition")]
        public Condition? Condition { get; set; }

        [JsonPropertyName("wind_kph")]
        public double WindKph { get; set; }

        [JsonPropertyName("wind_degree")]
        public int WindDegree { get; set; }

        [JsonPropertyName("wind_dir")]
        public string? WindDir { get; set; }

        [JsonPropertyName("gust_kph")]
        public double GustKph { get; set; }

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

        [JsonPropertyName("precip_mm")]
        public double PrecipMm { get; set; }

        [JsonPropertyName("cloud")]
        public int Cloud { get; set; }

        [JsonPropertyName("dewpoint_c")]
        public double DewpointC { get; set; }

        [JsonPropertyName("heatindex_c")]
        public double HeatindexC { get; set; }

        [JsonPropertyName("windchill_c")]
        public double WindchillC { get; set; }
    }

    public class Condition
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }

    public class Forecast
    {
        [JsonPropertyName("forecastday")]
        public List<ForecastDay>? ForecastDays { get; set; }
    }

    public class ForecastDay
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("day")]
        public Day? Day { get; set; }

        [JsonPropertyName("astro")]
        public Astro? Astro { get; set; }
    }

    public class Day
    {
        [JsonPropertyName("maxtemp_c")]
        public double MaxTempC { get; set; }

        [JsonPropertyName("mintemp_c")]
        public double MinTempC { get; set; }

        [JsonPropertyName("avgtemp_c")]
        public double AvgTempC { get; set; }

        [JsonPropertyName("maxwind_kph")]
        public double MaxWindKph { get; set; }

        [JsonPropertyName("totalprecip_mm")]
        public double TotalPrecipMm { get; set; }

        [JsonPropertyName("avghumidity")]
        public double AvgHumidity { get; set; }

        [JsonPropertyName("daily_chance_of_rain")]
        public int DailyChanceOfRain { get; set; }

        [JsonPropertyName("daily_chance_of_snow")]
        public int DailyChanceOfSnow { get; set; }

        [JsonPropertyName("condition")]
        public Condition? Condition { get; set; }

        [JsonPropertyName("uv")]
        public double UV { get; set; }
    }

    public class Astro
    {
        [JsonPropertyName("sunrise")]
        public string? Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public string? Sunset { get; set; }

        [JsonPropertyName("moonrise")]
        public string? Moonrise { get; set; }

        [JsonPropertyName("moonset")]
        public string? Moonset { get; set; }

        [JsonPropertyName("moon_phase")]
        public string? MoonPhase { get; set; }

        [JsonPropertyName("moon_illumination")]
        public int MoonIllumination { get; set; }
    }
}