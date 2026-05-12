using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Common.Logging;

using HtmlAgilityPack;

using Grabber.Models;


namespace Grabber.Parsers
{
    public class GismeteoParser : IParser
    {
        private readonly ILog log = null;

        private readonly HtmlWeb web = new HtmlWeb();

        private readonly List<Forecast> forecasts = new List<Forecast>();

        public GismeteoParser(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public string Url => "https://www.gismeteo.ru/";

        public List<City> LoadCities()
        {
            log.Info("Begin loading cities");

            HtmlDocument data = web.Load(Url);
            if (data == null)
                return new List<City>();

            HtmlNodeCollection nodes = data.DocumentNode.SelectNodes("//noscript[@id='noscript']/a");
            if (nodes == null)
                return new List<City>();

            List<City> targets = new List<City>();

            foreach (var node in nodes)
            {
                HtmlAttribute name = node.Attributes["data-name"];
                if (name == null)
                    continue;

                HtmlAttribute href = node.Attributes["href"];
                if (href == null)
                    continue;

                City target = new City();
                target.Name = name.Value;
                target.Href = (href.Value[0] == '/') ? $"{Url}{href.Value.Remove(0, 1)}" : $"{Url}{href.Value}";

                targets.Add(target);
            }

            log.Info("End loading cities");

            return targets;
        }

        public List<Forecast> LoadForecasts(params City[] cities)
        {
            lock (forecasts)
            {
                forecasts.Clear();
            }

            List<Task> tasks = cities.Select(city => Task.Run(() => { LoadForecast(city); })).ToList();
            Task.WaitAll(tasks.ToArray());

            lock (forecasts)
            {
                return forecasts;
            }
        }

        private void LoadForecast(City city)
        {
            log.Info($"Begin loading forecast of {city.Name}");

            HtmlDocument data = web.Load(city.Href + "tomorrow/");
            if (data == null)
                return;

            List<short> temperatures = new List<short>();

            HtmlNode root = data.DocumentNode.SelectSingleNode("//div[@class='tab  tooltip']");

            // Temperature
            HtmlNodeCollection nodes = root.SelectNodes("div/div/div/div/div/div/div/span[@class='unit unit_temperature_c']");
            if (nodes.Count == 2)
            {
                string min = nodes[0].InnerText.Replace("&minus;", "-");
                string max = nodes[1].InnerText.Replace("&minus;", "-");

                temperatures.Add(short.Parse(min));
                temperatures.Add(short.Parse(max));
            }
            else if (nodes.Count == 1)
            {
                string value = nodes[0].InnerText.Replace("&minus;", "-");

                temperatures.Add(short.Parse(value));
            }
            else
            {
                throw new NotImplementedException($"{city.Name}");
            }

            // Description
            HtmlAttribute description = root.Attributes["data-text"];

            lock (forecasts)
            {
                forecasts.Add(new Forecast() { City = city.Name, Temperatures = temperatures, Description = description?.Value });
            }

            log.Info($"End loading forecast of {city.Name}");
        }
    }
}
