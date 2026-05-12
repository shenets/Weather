using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public enum MeasuringTypes : byte
    {
        Temperature = 1,
        Description = 2
    }

    public class Measuring
    {
        public int Id { get; set; }
        public MeasuringTypes Type { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
