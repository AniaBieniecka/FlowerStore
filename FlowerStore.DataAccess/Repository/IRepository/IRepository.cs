using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlowerStore.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T: class
    {
        //T - Category or any other generic model

        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null);
        T Get(Expression<Func<T,bool>> filter, string? includeProperties = null, bool tracked=false); // zwraca jeden element używa linq expression
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);

    }
}
