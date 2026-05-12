using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Client.Api;


namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IOperations api = null;

        public MainWindow()
        {
            api = new OperationsClient();

            InitializeComponent();

            HideForecast();
            Localize();
        }

        private void Localize()
        {
            Title = Properties.Resources.Forecast;
            TemperatureTitle.Content = Properties.Resources.Temperature;
            DescriptionTitle.Content = Properties.Resources.Description;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            FillCities();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            Cities.SelectionChanged += Cities_SelectionChanged;
        }

        protected override void OnDeactivated(EventArgs e)
        {
            Cities.SelectionChanged -= Cities_SelectionChanged;

            base.OnDeactivated(e);
        }

        private void Cities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;

            FillForecast((int)selectedItem.Tag);
        }

        private async void FillCities()
        {
            City[] cities = await api.GetCitiesAsync();
            if (cities == null)
                return;

            if (cities.Any())
            {
                foreach (var city in cities)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Tag = city.Id;
                    item.Content = city.Name;

                    Cities.Items.Add(item);
                }

                Cities.SelectedIndex = 0;
            }

            ShowForecast();
        }
        private async void FillForecast(int cityId)
        {
            Forecast forecast = await api.GetForecastAsync(cityId);

            string temperature = null;
            string description = null;

            if (forecast != null)
            {
                temperature = string.Join(" ", forecast.Temperatures.Distinct());
                description = forecast.Description;
            }

            Temperature.Text = (!string.IsNullOrWhiteSpace(temperature)) ? temperature : Properties.Resources.DataLack;
            Description.Text = (!string.IsNullOrWhiteSpace(description)) ? description : Properties.Resources.DataLack;
        }

        private void ShowForecast()
        {
            Cities.Visibility = Visibility.Visible;
            TemperatureTitle.Visibility = Visibility.Visible;
            Temperature.Visibility = Visibility.Visible;
            DescriptionTitle.Visibility = Visibility.Visible;
            Description.Visibility = Visibility.Visible;
        }
        private void HideForecast()
        {
            Cities.Visibility = Visibility.Hidden;
            TemperatureTitle.Visibility = Visibility.Hidden;
            Temperature.Visibility = Visibility.Hidden;
            DescriptionTitle.Visibility = Visibility.Hidden;
            Description.Visibility = Visibility.Hidden;
        }
    }
}
