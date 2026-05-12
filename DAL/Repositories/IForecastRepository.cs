using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DTO;


namespace DAL.Repositories
{
    public interface IForecastRepository
    {
        Forecast GetForecast(int cityId);
        List<Forecast> GetForecasts(List<string> cities);

        int AddMeasuring(MeasuringTypes temperature);

        void AddTemperatures(int cityId, int measuringId, List<short> values);

        void AddDescription(int cityId, int measuringId, string value);
    }
}
