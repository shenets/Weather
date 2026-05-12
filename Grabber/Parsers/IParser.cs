using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grabber.Models;


namespace Grabber.Parsers
{
    public interface IParser
    {
        string Url { get; }

        List<City> LoadCities();
        List<Forecast> LoadForecasts(params City[] cities);
    }
}
