using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace weather_app
{
    public partial class MainWindow : Window
    {
        private string? ApiKey;
        private DatabaseHelper? _dbHelper;
        private string _currentCity = "Новосибирск";

        public MainWindow()
        {
            InitializeComponent();

            ApiKey = Environment.GetEnvironmentVariable("WAPI_KEY");

            if (string.IsNullOrEmpty(ApiKey)) {
                MessageBox.Show("API key is required for application.");
                Close();
                return;
            }

            try
            {
                _dbHelper = new DatabaseHelper();
                string? savedCity = _dbHelper.GetDefaultCity();
                if (!string.IsNullOrEmpty(savedCity))
                {
                    _currentCity = savedCity;
                    CityTextBox.Text = savedCity;
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Ошибка БД: {ex.Message}";
            }

            LoadWeather(_currentCity);
        }

        private async void LoadWeather(string city)
        {
            try
            {
                StatusTextBlock.Text = "Загрузка...";
                SearchButton.IsEnabled = false;
                SaveCityButton.IsEnabled = false;

                using (var client = new HttpClient())
                {
                    string url = $"http://api.weatherapi.com/v1/forecast.json?key={ApiKey}&q={city}&days=3&lang=ru";
                    
                    HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(true);
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                    WeatherData? data = JsonSerializer.Deserialize<WeatherData>(json);

                    if (data == null || data.Location == null || data.Current == null)
                    {
                        await Dispatcher.InvokeAsync(() =>
                        {
                            StatusTextBlock.Text = "Ошибка: данные не получены";
                        });
                        return;
                    }

                    _currentCity = data.Location.Name ?? city;

                    await Dispatcher.InvokeAsync(() =>
                    {
                        CityTextBlock.Text = data.Location.Name ?? "";
                        CountryTextBlock.Text = data.Location.Country ?? "";
                        TimeTextBlock.Text = data.Location.LocalTime ?? "";

                        TempTextBlock.Text = $"{data.Current.TempC:0}°C";
                        FeelsLikeTextBlock.Text = $"Ощущается как: {data.Current.FeelsLikeC:0}°C";
                        DescTextBlock.Text = data.Current.Condition?.Text ?? "";

                        HumidityTextBlock.Text = $"{data.Current.Humidity}%";
                        PressureTextBlock.Text = $"{data.Current.PressureMb} hPa";
                        VisibilityTextBlock.Text = $"{data.Current.VisibilityKm} км";
                        CloudTextBlock.Text = $"{data.Current.Cloud}%";

                        WindTextBlock.Text = $"{data.Current.WindKph} км/ч";
                        WindDirTextBlock.Text = $"{data.Current.WindDir ?? ""} {data.Current.WindDegree}°";
                        GustTextBlock.Text = $"{data.Current.GustKph} км/ч";
                        WindchillTextBlock.Text = $"{data.Current.WindchillC:0}°C";

                        PrecipTextBlock.Text = $"{data.Current.PrecipMm} мм";
                        DewpointTextBlock.Text = $"{data.Current.DewpointC:0}°C";
                        HeatindexTextBlock.Text = $"{data.Current.HeatindexC:0}°C";
                        UVTextBlock.Text = data.Current.UV.ToString();

                        if (!string.IsNullOrEmpty(data.Current.Condition?.Icon))
                        {
                            string iconUrl = data.Current.Condition.Icon;
                            if (iconUrl.StartsWith("//"))
                            {
                                iconUrl = "https:" + iconUrl;
                            }
                            WeatherIcon.Source = new System.Windows.Media.Imaging.BitmapImage(
                                new Uri(iconUrl));
                        }

                        if (data.Forecast?.ForecastDays != null && data.Forecast.ForecastDays.Count > 0)
                        {
                            var todayAstro = data.Forecast.ForecastDays[0].Astro;
                            if (todayAstro != null)
                            {
                                SunriseTextBlock.Text = todayAstro.Sunrise ?? "--:--";
                                SunsetTextBlock.Text = todayAstro.Sunset ?? "--:--";
                                MoonriseTextBlock.Text = todayAstro.Moonrise ?? "--:--";
                                MoonsetTextBlock.Text = todayAstro.Moonset ?? "--:--";
                                MoonPhaseTextBlock.Text = TranslateMoonPhase(todayAstro.MoonPhase);
                                MoonIllumTextBlock.Text = $"{todayAstro.MoonIllumination}%";
                            }

                            UpdateForecastPanel(data.Forecast.ForecastDays);
                        }

                        StatusTextBlock.Text = $"Обновлено: {DateTime.Now:HH:mm}";
                    });
                }
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    StatusTextBlock.Text = $"Ошибка: {ex.Message}";
                    ClearWeatherData();
                });
            }
            finally
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    SearchButton.IsEnabled = true;
                    SaveCityButton.IsEnabled = true;
                });
            }
        }

        private void UpdateForecastPanel(List<ForecastDay> forecastDays)
        {
            ForecastPanel.Children.Clear();

            foreach (var day in forecastDays)
            {
                var card = CreateForecastCard(day);
                ForecastPanel.Children.Add(card);
            }
        }

        private Border CreateForecastCard(ForecastDay forecastDay)
        {
            var border = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e3f2fd")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#90caf9")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15),
                Margin = new Thickness(5),
                MinWidth = 170
            };

            var stack = new StackPanel();

            var dateText = new TextBlock
            {
                Text = forecastDay.Date ?? "",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1565c0")),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            stack.Children.Add(dateText);

            if (forecastDay.Day?.Condition?.Icon != null)
            {
                string iconUrl = forecastDay.Day.Condition.Icon;
                if (iconUrl.StartsWith("//"))
                {
                    iconUrl = "https:" + iconUrl;
                }
                var icon = new Image
                {
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(iconUrl)),
                    Width = 48,
                    Height = 48,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                stack.Children.Add(icon);
            }

            var conditionText = new TextBlock
            {
                Text = forecastDay.Day?.Condition?.Text ?? "",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#424242")),
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            stack.Children.Add(conditionText);

            var tempText = new TextBlock
            {
                Text = $"🌡️ {forecastDay.Day?.MinTempC:0}°C / {forecastDay.Day?.MaxTempC:0}°C",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#212529")),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5)
            };
            stack.Children.Add(tempText);

            var avgTempText = new TextBlock
            {
                Text = $"Средняя: {forecastDay.Day?.AvgTempC:0}°C",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6c757d")),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 8)
            };
            stack.Children.Add(avgTempText);

            var humidityText = new TextBlock
            {
                Text = $"💧 Влажность: {forecastDay.Day?.AvgHumidity}%",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#495057")),
                Margin = new Thickness(0, 0, 0, 3)
            };
            stack.Children.Add(humidityText);

            var windText = new TextBlock
            {
                Text = $"💨 Ветер: {forecastDay.Day?.MaxWindKph} км/ч",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#495057")),
                Margin = new Thickness(0, 0, 0, 3)
            };
            stack.Children.Add(windText);

            var precipText = new TextBlock
            {
                Text = $"🌧️ Осадки: {forecastDay.Day?.TotalPrecipMm} мм",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#495057")),
                Margin = new Thickness(0, 0, 0, 3)
            };
            stack.Children.Add(precipText);

            var rainText = new TextBlock
            {
                Text = $"🌧 Дождь: {forecastDay.Day?.DailyChanceOfRain}%",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#495057")),
                Margin = new Thickness(0, 0, 0, 3)
            };
            stack.Children.Add(rainText);

            var snowText = new TextBlock
            {
                Text = $"❄️ Снег: {forecastDay.Day?.DailyChanceOfSnow}%",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#495057")),
                Margin = new Thickness(0, 0, 0, 3)
            };
            stack.Children.Add(snowText);

            var uvText = new TextBlock
            {
                Text = $"☀️ УФ: {forecastDay.Day?.UV}",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#495057")),
                Margin = new Thickness(0, 0, 0, 8)
            };
            stack.Children.Add(uvText);

            if (forecastDay.Astro != null)
            {
                var astroHeader = new TextBlock
                {
                    Text = "─── Восход/Закат ───",
                    FontSize = 10,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#90caf9")),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                stack.Children.Add(astroHeader);

                var sunriseText = new TextBlock
                {
                    Text = $"🌅 {forecastDay.Astro.Sunrise}",
                    FontSize = 10,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6c757d"))
                };
                stack.Children.Add(sunriseText);

                var sunsetText = new TextBlock
                {
                    Text = $"🌇 {forecastDay.Astro.Sunset}",
                    FontSize = 10,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6c757d"))
                };
                stack.Children.Add(sunsetText);
            }

            border.Child = stack;
            return border;
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
            CloudTextBlock.Text = "";
            WindDirTextBlock.Text = "";
            GustTextBlock.Text = "";
            WindchillTextBlock.Text = "";
            PrecipTextBlock.Text = "";
            DewpointTextBlock.Text = "";
            HeatindexTextBlock.Text = "";
            SunriseTextBlock.Text = "--:--";
            SunsetTextBlock.Text = "--:--";
            MoonriseTextBlock.Text = "--:--";
            MoonsetTextBlock.Text = "--:--";
            MoonPhaseTextBlock.Text = "--";
            MoonIllumTextBlock.Text = "0%";
            WeatherIcon.Source = null;
            ForecastPanel.Children.Clear();
        }

        private string TranslateMoonPhase(string? moonPhase)
        {
            return moonPhase?.ToLower() switch
            {
                "new moon" => "Новолуние",
                "waxing crescent" => "Растущий серп",
                "first quarter" => "Первая четверть",
                "waxing gibbous" => "Растущая луна",
                "full moon" => "Полнолуние",
                "waning gibbous" => "Убывающая луна",
                "last quarter" => "Последняя четверть",
                "waning crescent" => "Убывающий серп",
                _ => moonPhase ?? "--"
            };
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string city = CityTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(city))
            {
                LoadWeather(city);
            }
        }

        private void SaveCityButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dbHelper != null && !string.IsNullOrEmpty(_currentCity))
                {
                    _dbHelper.SaveDefaultCity(_currentCity);
                    MessageBox.Show($"Город \"{_currentCity}\" сохранен как избранный!", "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusTextBlock.Text = $"Город \"{_currentCity}\" сохранен";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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