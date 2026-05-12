using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DTO;

using Model = DTO.Measuring;


namespace DAL
{
    internal partial class Measuring
    {
        public Measuring(Model model)
        {
            Id = model.Id;
            Type = (byte)model.Type;
            CreatedDate = model.CreatedDate;
        }

        public Model ToModel()
        {
            Model model = new Model();
            model.Id = Id;
            model.Type = (MeasuringTypes)Type;
            model.CreatedDate = CreatedDate;

            return model;
        }
    }
}
