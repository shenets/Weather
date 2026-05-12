using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Common.Logging;

using SimpleInjector;

using BLL;

using DAL.Repositories;

using Grabber.Parsers;


namespace Grabber
{
    static class Program
    {
        static readonly Container container = null;

        static readonly ILog log = LogManager.GetLogger("main");

        static Program()
        {
            container = new Container();
            container.Register<ICitiesRepository, CitiesRepository>();
            container.Register<IForecastRepository, ForecastRepository>();
            container.Register<IParser>(() => new GismeteoParser(log));
            container.Register<IForecastSaving, ForecastSaving>();
            container.Register<IForecastRetrieving, ForecastRetrieving>();

            container.Verify();
        }

        static void Main(string[] args)
        {
            try
            {
                log.Info("Begin grab");

                

                log.Info("End grab");
            }
            catch (Exception exception)
            {
                log.Error(exception);

#if DEBUG
                Console.WriteLine(exception);
#endif
            }
        }

        //        static void Main(string[] args)
        //        {
        //            try
        //            {
        //                log.Info("Begin grab");

        //                IParser parser = container.GetInstance<IParser>();
        //                IForecastSaving saving = container.GetInstance<IForecastSaving>();
        //                IForecastRetrieving retrieving = container.GetInstance<IForecastRetrieving>();

        //                Processor processor = new Processor(parser, saving, retrieving);
        //                processor.Process();

        //                log.Info("End grab");
        //            }
        //            catch (Exception exception)
        //            {
        //                log.Error(exception);

        //#if DEBUG
        //                Console.WriteLine(exception);
        //#endif
        //            }
        //        }
    }
}
