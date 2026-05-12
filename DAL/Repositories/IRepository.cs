using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IRepository<T> : IReadOnlyRepository<T>
    {
        int Add(T entity);
        int Update(T entity);
        int Delete(T entity);
    }
}
