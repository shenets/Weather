using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DAL.Repositories;

using DTO;


namespace BLL
{
    public class ForecastRetrieving : IForecastRetrieving
    {
        private readonly ICitiesRepository citiesRepository = null;
        private readonly IForecastRepository forecastRepository = null;

        public ForecastRetrieving(ICitiesRepository citiesRepository, IForecastRepository forecastRepository)
        {
            this.citiesRepository = citiesRepository ?? throw new ArgumentNullException(nameof(citiesRepository));
            this.forecastRepository = forecastRepository ?? throw new ArgumentNullException(nameof(forecastRepository));
        }

        public List<City> GetCities()
        {
            List<City> cities = citiesRepository.Get().ToList();
            return cities;
        }

        public Forecast GetForecast(int cityId)
        {
            Forecast forecast = forecastRepository.GetForecast(cityId);
            return forecast;
        }
    }
}
