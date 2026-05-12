using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = DTO.City;
using Entity = DAL.City;


namespace DAL.Repositories
{
    public sealed class CitiesRepository : ICitiesRepository
    {
        public IEnumerable<Model> Get()
        {
            using (WeatherDBEntities context = new WeatherDBEntities())
            {
                IEnumerable<Model> models = context.Cities.ToList().Select(item => item.ToModel());
                return models;
            }
        }
        public Model Get(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            using (WeatherDBEntities context = new WeatherDBEntities())
            {
                Entity entity = context.Cities.Find(id);
                return entity?.ToModel();
            }
        }
        public int Add(Model model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            Entity entity = new Entity(model);

            using (WeatherDBEntities context = new WeatherDBEntities())
            {
                context.Entry(entity).State = EntityState.Added;
                context.SaveChanges();
            }

            return entity.Id;
        }

        public int Update(Model model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.Id <= 0)
                throw new ArgumentOutOfRangeException(nameof(model.Id));

            Entity entity = new Entity(model);

            using (WeatherDBEntities context = new WeatherDBEntities())
            {
                context.Cities.AddOrUpdate(entity);
                context.SaveChanges();
            }

            return entity.Id;
        }
        public int Delete(Model model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.Id <= 0)
                throw new ArgumentOutOfRangeException(nameof(model.Id));

            Entity entity = new Entity(model);

            using (WeatherDBEntities context = new WeatherDBEntities())
            {
                context.Cities.Attach(entity);

                context.Entry(entity).State = EntityState.Deleted;
                context.SaveChanges();
            }

            return entity.Id;
        }

        public List<Model> Get(IEnumerable<string> names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));

            using (WeatherDBEntities context = new WeatherDBEntities())
            {
                List<Model> cities = context.Cities.Where(item => names.Contains(item.Name)).ToList().Select(item => item.ToModel()).ToList();
                return cities;
            }
        }

        public List<Model> AddRange(IEnumerable<string> names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));

            List<Model> targets = new List<Model>();

            DateTime now = DateTime.UtcNow;

            using (WeatherDBEntities context = new WeatherDBEntities())
            {
                foreach (string name in names)
                {
                    Entity entity = new Entity();
                    entity.Name = name;
                    entity.CreatedDate = now; 

                    context.Entry(entity).State = EntityState.Added;

                    targets.Add(entity.ToModel());
                }

                context.SaveChanges();
            }

            return targets;
        }
    }
}
