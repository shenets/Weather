using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DTO;

using DAL.Repositories;


namespace BLL
{
    public class ForecastSaving : IForecastSaving
    {
        private readonly ICitiesRepository citiesRepository = null;
        private readonly IForecastRepository forecastRepository = null;

        public ForecastSaving(ICitiesRepository citiesRepository, IForecastRepository forecastRepository)
        {
            this.citiesRepository = citiesRepository ?? throw new ArgumentNullException(nameof(citiesRepository));
            this.forecastRepository = forecastRepository ?? throw new ArgumentNullException(nameof(forecastRepository));
        }

        public void AddCities(IEnumerable<string> names)
        {
            // уникальность по имени в бд добавить

            if (names == null)
                throw new ArgumentNullException(nameof(names));

            citiesRepository.AddRange(names);
        }

        public void AddForecasts(List<Forecast> forecasts)
        {
            if (forecasts == null)
                throw new ArgumentNullException(nameof(forecasts));

            // City
            List<string> cityNames = forecasts.Select(item => item.City).Distinct().ToList();

            List<City> cities = citiesRepository.Get(cityNames);

            List<string> exceptions = cityNames.Except(cities.Select(item => item.Name)).ToList();
            if (exceptions.Any())
            {
                var news = citiesRepository.AddRange(exceptions);

                cities.AddRange(news);
            }

            List<Forecast> measurings = forecastRepository.GetForecasts(cities.Select(item => item.Name).ToList());

            // Temperature
            List<Forecast> temperatureTargets = new List<Forecast>();

            foreach (Forecast bunch in forecasts)
            {
                Forecast measuring = measurings.SingleOrDefault(item => item.City == bunch.City);
                if (measuring != null)
                {
                    IEnumerable<short> excepts = measuring.Temperatures.Except(bunch.Temperatures);
                    if (excepts.Any())
                        temperatureTargets.Add(bunch);
                }
                else
                {
                    temperatureTargets.Add(bunch);
                }
            }

            if (temperatureTargets.Any())
            {
                int measuringId = forecastRepository.AddMeasuring(MeasuringTypes.Temperature);

                foreach (Forecast target in temperatureTargets)
                {
                    City city = cities.Single(item => item.Name == target.City);

                    forecastRepository.AddTemperatures(city.Id, measuringId, target.Temperatures);
                }
            }

            // Description
            List<Forecast> descriptionTargets = new List<Forecast>();

            foreach (Forecast forecast in forecasts)
            {
                Forecast measuring = measurings.SingleOrDefault(item => item.City == forecast.City);
                if (measuring != null)
                {
                    if (forecast.Description != null && !forecast.Description.Equals(measuring.Description, StringComparison.InvariantCultureIgnoreCase))
                        descriptionTargets.Add(forecast);
                }
                else
                {
                    descriptionTargets.Add(forecast);
                }
            }

            if (descriptionTargets.Any())
            {
                int measuringId = forecastRepository.AddMeasuring(MeasuringTypes.Description);

                foreach (Forecast target in descriptionTargets)
                {
                    City city = cities.Single(item => item.Name == target.City);

                    forecastRepository.AddDescription(city.Id, measuringId, target.Description);
                }
            }
        }
    }
}
