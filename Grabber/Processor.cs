using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using BLL;

using Grabber.Parsers;

using City = Grabber.Models.City;
using Forecast = Grabber.Models.Forecast;


namespace Grabber
{
    public class Processor
    {
        private readonly IParser parser = null;
        private readonly IForecastSaving saving = null;
        private readonly IForecastRetrieving retrieving = null;

        public Processor(IParser parser, IForecastSaving saving, IForecastRetrieving retrieving)
        {
            this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
            this.saving = saving ?? throw new ArgumentNullException(nameof(saving));
            this.retrieving = retrieving ?? throw new ArgumentNullException(nameof(retrieving));
        }

        public void Process()
        {
            // Cities
            List<City> cities = parser.LoadCities();

            List<DTO.City> existCities = retrieving.GetCities();

            List<string> cityTargets = cities.Select(item => item.Name).Except(existCities.Select(item => item.Name)).ToList();
            if (cityTargets.Any())
                saving.AddCities(cityTargets);

            // forecasts
            List<Forecast> forecasts = parser.LoadForecasts(cities.ToArray());

            List<DTO.Forecast> forecastTargets = new List<DTO.Forecast>();

            foreach (Forecast forecast in forecasts)
            {
                DTO.Forecast forecastTarget = new DTO.Forecast();
                forecastTarget.City = forecast.City;
                forecastTarget.Temperatures = forecast.Temperatures;
                forecastTarget.Description = forecast.Description;

                forecastTargets.Add(forecastTarget);
            }

            saving.AddForecasts(forecastTargets);
        }
    }
}
