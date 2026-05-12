using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = DTO.City;


namespace DAL
{
    internal partial class City
    {
        public City(Model model)
        {
            Id = model.Id;
            Name = model.Name;
            CreatedDate = model.CreatedDate;
        }

        public Model ToModel()
        {
            Model model = new Model();
            model.Id = Id;
            model.Name = Name;
            model.CreatedDate = CreatedDate;

            return model;
        }
    }
}
