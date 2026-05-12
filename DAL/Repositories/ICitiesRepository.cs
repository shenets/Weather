using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = DTO.City;


namespace DAL.Repositories
{
    public interface ICitiesRepository : IRepository<Model>
    {
        List<Model> Get(IEnumerable<string> names);

        List<Model> AddRange(IEnumerable<string> names);
    }
}
