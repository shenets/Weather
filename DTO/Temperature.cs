using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Temperature
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public int MeasuringId { get; set; }
        public short Value { get; set; }
    }
}
