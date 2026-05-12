using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = DTO.Description;


namespace DAL
{
    internal partial class Description
    {
        public Description()
        {
        }
        public Description(Model model)
        {
            Id = model.Id;
            CityId = model.CityId;
            MeasuringId = model.MeasuringId;
            Value = model.Value;
        }

        public Model ToModel()
        {
            Model model = new Model();
            model.Id = Id;
            model.CityId = CityId;
            model.MeasuringId = MeasuringId;
            model.Value = Value;

            return model;
        }
    }
}
