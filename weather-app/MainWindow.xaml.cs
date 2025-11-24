using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace weather_app
{
    public partial class MainWindow : Window
    {
        private string ApiKey;


        public MainWindow()
        {
            InitializeComponent();

            ApiKey = Environment.GetEnvironmentVariable("WAPI_KEY");

            if (string.IsNullOrEmpty(ApiKey)) {
                MessageBox.Show("API key is required for application.");
                Close();
                return;
            }


            LoadWeather("Новосибирск");
        }

        private async void LoadWeather(string city)
        {
            try
            {
                StatusTextBlock.Text = "Загрузка...";
                SearchButton.IsEnabled = false;

                using (var client = new HttpClient())
                {
                    string url = $"http://api.weatherapi.com/v1/current.json?key={ApiKey}&q={city}&lang=ru";

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    WeatherData data = JsonSerializer.Deserialize<WeatherData>(json);

                    CityTextBlock.Text = data.Location.Name;
                    CountryTextBlock.Text = data.Location.Country;
                    TimeTextBlock.Text = data.Location.LocalTime;
                    TempTextBlock.Text = $"{data.Current.TempC:0}°C";
                    FeelsLikeTextBlock.Text = $"Ощущается как: {data.Current.FeelsLikeC:0}°C";
                    DescTextBlock.Text = data.Current.Condition.Text;
                    HumidityTextBlock.Text = $"{data.Current.Humidity}%";
                    PressureTextBlock.Text = $"{data.Current.PressureMb} hPa";
                    WindTextBlock.Text = $"{data.Current.WindKph} км/ч";
                    VisibilityTextBlock.Text = $"{data.Current.VisibilityKm} км";
                    UVTextBlock.Text = data.Current.UV.ToString();

                    if (!string.IsNullOrEmpty(data.Current.Condition.Icon))
                    {
                        string iconUrl = data.Current.Condition.Icon;
                        if (iconUrl.StartsWith("//"))
                        {
                            iconUrl = "https:" + iconUrl;
                        }
                        WeatherIcon.Source = new System.Windows.Media.Imaging.BitmapImage(
                            new Uri(iconUrl));
                    }

                    StatusTextBlock.Text = $"Обновлено: {DateTime.Now:HH:mm}";
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Ошибка: {ex.Message}";
                ClearWeatherData();
            }
            finally
            {
                SearchButton.IsEnabled = true;
            }
        }

        private void ClearWeatherData()
        {
            CityTextBlock.Text = "";
            CountryTextBlock.Text = "";
            TimeTextBlock.Text = "";
            TempTextBlock.Text = "";
            FeelsLikeTextBlock.Text = "";
            DescTextBlock.Text = "";
            HumidityTextBlock.Text = "";
            PressureTextBlock.Text = "";
            WindTextBlock.Text = "";
            VisibilityTextBlock.Text = "";
            UVTextBlock.Text = "";
            WeatherIcon.Source = null;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string city = CityTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(city))
            {
                LoadWeather(city);
            }
        }

        private void CityTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string city = CityTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(city))
                {
                    LoadWeather(city);
                }
            }
        }
    }
}