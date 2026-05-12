using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using Common.Logging;

using DTO;

using BLL;


namespace Api
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Operations : IOperations
    {
        private readonly ILog log = LogManager.GetLogger("main");
        private readonly IForecastRetrieving bll = null;

        public Operations(IForecastRetrieving bll)
        {
            this.bll = bll ?? throw new ArgumentNullException(nameof(bll));
        }

        public List<City> GetCities()
        {
            try
            {
                List<City> cities = bll.GetCities();
                return cities;
            }
            catch (Exception exception)
            {
                log.Error(exception);

                return null;
            }
        }

        public Forecast GetForecast(int cityId)
        {
            try
            {
                Forecast forecast = bll.GetForecast(cityId);
                return forecast;
            }
            catch (Exception exception)
            {
                log.Error(exception);

                return null;
            }
        }
    }
}
