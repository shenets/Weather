using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DTO;


namespace BLL
{
    public interface IForecastSaving
    {
        void AddCities(IEnumerable<string> names);

        void AddForecasts(List<Forecast> forecasts);
    }
}
