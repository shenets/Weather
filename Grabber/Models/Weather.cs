using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabber.Models
{
    public class Forecast
    {
        public string City { get; set; }
        public List<short> Temperatures { get; set; }
        public string Description { get; set; }
    }
}
