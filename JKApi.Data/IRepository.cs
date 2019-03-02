using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Data
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Func<T, bool> predicate = null);
        T Get(Func<T, bool> predicate);
        bool Add(T entity);
        bool Attach(T entity);
        bool Delete(T entity);
    }
}
