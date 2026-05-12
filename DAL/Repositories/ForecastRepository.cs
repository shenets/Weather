using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DTO;


namespace DAL.Repositories
{
    public class ForecastRepository : IForecastRepository
    {
        public Forecast GetForecast(int cityId)
        {
            if (cityId <= 0)
                throw new ArgumentNullException(nameof(cityId));

            using (WeatherDBEntities context = new WeatherDBEntities())
            {
                var measurings = from city in context.Cities
                                 join temperature in context.Temperatures
                                     on city.Id equals temperature.CityId into temperatures
                                 from temperature in temperatures.DefaultIfEmpty()
                                 join description in context.Descriptions
                                     on city.Id equals description.CityId into descriptions
                                 from description in descriptions.DefaultIfEmpty()
                                 where city.Id == cityId
                                 group new
                                 {
                                     CityId = city.Id,
                                     TemperatureMeasuringId = temperature.MeasuringId,
                                     DescriptionMeasuringId = description.MeasuringId
                                 } by new { city.Id, city.Name } into grouping
                                 select new
                                 {
                                     CityId = grouping.Key.Id,
                                     Name = grouping.Key,
                                     LastTemperatureMeasuringId = grouping.Max(item => (int?)item.TemperatureMeasuringId) ?? 0,
                                     LastDescriptionMeasuringId = grouping.Max(item => (int?)item.DescriptionMeasuringId) ?? 0
                                 };

                var query = from measuring in measurings
                            join temperature in context.Temperatures
                                on new { measuring.CityId, MeasuringId = measuring.LastTemperatureMeasuringId } equals new { temperature.CityId, temperature.MeasuringId } into temperatureGroup
                            from temperature in temperatureGroup.DefaultIfEmpty()
                            join description in context.Descriptions
                                on new { measuring.CityId, MeasuringId = measuring.LastDescriptionMeasuringId } equals new { description.CityId, description.MeasuringId } into descriptionGroup
                            from description in descriptionGroup.DefaultIfEmpty()
                            select new
                            {
                                City = measuring.Name,
                                Temperature = (short?)temperature.Value,
                                Description = description.Value
                            };


                var elements = query.ToList();

                var cityTarget = elements.Select(item => item.City).Distinct().SingleOrDefault();
                if (cityTarget == null)
                    return null;

                Forecast forecast = new Forecast();
                forecast.City = cityTarget.Name;
                forecast.Temperatures = elements.Where(item => item.Temperature != null).Select(item => item.Temperature.Value).ToList();
                forecast.Description = elements.Select(item => item.Description).Distinct().SingleOrDefault();

                return forecast;
            }
        }
        public List<Forecast> GetForecasts(List<string> cities)
        {
            if (cities == null)
                throw new ArgumentNullException(nameof(cities));

            if (cities.Count > 2000)
                throw new ArgumentOutOfRangeException(nameof(cities));

            if (!cities.Any())
                return new List<Forecast>();

            using (WeatherDBEntities context = new WeatherDBEntities())
            {
                var measurings = from city in context.Cities
                                 join temperature in context.Temperatures
                                     on city.Id equals temperature.CityId into temperatures
                                 from temperature in temperatures.DefaultIfEmpty()
                                 join description in context.Descriptions
                                     on city.Id equals description.CityId into descriptions
                                 from description in descriptions.DefaultIfEmpty()
                                 where cities.Contains(city.Name)
                                 group new
                                 {
                                     CityId = city.Id,
                                     TemperatureMeasuringId = temperature.MeasuringId,
                                     DescriptionMeasuringId = description.MeasuringId

                                 } by new { city.Id, city.Name } into grouping
                                 select new
                                 {
                                     CityId = grouping.Key.Id,
                                     Name = grouping.Key.Name,
                                     LastTemperatureMeasuringId = grouping.Max(item => (int?)item.TemperatureMeasuringId) ?? 0,
                                     LastDescriptionMeasuringId = grouping.Max(item => (int?)item.DescriptionMeasuringId) ?? 0
                                 };

                var query = from measuring in measurings
                            join temperature in context.Temperatures
                                on new { measuring.CityId, MeasuringId = measuring.LastTemperatureMeasuringId } equals new { temperature.CityId, temperature.MeasuringId } into temperatureGroup
                            from temperature in temperatureGroup.DefaultIfEmpty()
                            join description in context.Descriptions
                                on new { measuring.CityId, MeasuringId = measuring.LastDescriptionMeasuringId } equals new { description.CityId, description.MeasuringId } into descriptionGroup
                            from description in descriptionGroup.DefaultIfEmpty()
                            select new
                            {
                                City = measuring.Name,
                                Temperature = (short?)temperature.Value,
                                Description = description.Value
                            };

                var elements = query.ToList();

                IEnumerable<string> cityNames = elements.Select(item => item.City).Distinct();

                Dictionary<string, List<short>> temperaturesDictionary = elements.GroupBy(item => item.City).ToDictionary(key => key.Key, value => value.Where(item => item.Temperature != null).Select(item => item.Temperature.Value).ToList());
                Dictionary<string, string> descriptionsDictionary = elements.GroupBy(item => item.City).ToDictionary(key => key.Key, value => value.Select(item => item.Description).Distinct().SingleOrDefault());

                List<Forecast> forecasts = new List<Forecast>();

                foreach (string city in cityNames)
                {
                    Forecast target = new Forecast();
                    target.City = city;

                    temperaturesDictionary.TryGetValue(city, out List<short> temperatures);
                    target.Temperatures = temperatures;

                    descriptionsDictionary.TryGetValue(city, out string description);
                    target.Description = description;

                    forecasts.Add(target);
                }

                return forecasts;
            }
        }

        public int AddMeasuring(MeasuringTypes type)
        {
            Measuring measuring = new Measuring();
            measuring.Type = (byte)type;
            measuring.CreatedDate = DateTime.UtcNow;

            using (WeatherDBEntities context = new WeatherDBEntities())
            {
                context.Entry(measuring).State = EntityState.Added;

                context.SaveChanges();
            }

            return measuring.Id;
        }

        public void AddTemperatures(int cityId, int measuringId, List<short> values)
        {
            if (cityId <= 0)
                throw new ArgumentNullException(nameof(cityId));

            if (measuringId <= 0)
                throw new ArgumentNullException(nameof(measuringId));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            List<Temperature> temperatures = new List<Temperature>();

            foreach (short value in values)
            {
                Temperature temperature = new Temperature();
                temperature.CityId = cityId;
                temperature.MeasuringId = measuringId;
                temperature.Value = value;

                temperatures.Add(temperature);
            }

            using (WeatherDBEntities context = new WeatherDBEntities())
            {
                context.Temperatures.AddRange(temperatures);

                context.SaveChanges();
            }
        }

        public void AddDescription(int cityId, int measuringId, string value)
        {
            if (cityId <= 0)
                throw new ArgumentNullException(nameof(cityId));

            if (measuringId <= 0)
                throw new ArgumentNullException(nameof(measuringId));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Description description = new Description();
            description.CityId = cityId;
            description.MeasuringId = measuringId;
            description.Value = value;

            using (WeatherDBEntities context = new WeatherDBEntities())
            {
                context.Entry(description).State = EntityState.Added;

                context.SaveChanges();
            }
        }
    }
}
