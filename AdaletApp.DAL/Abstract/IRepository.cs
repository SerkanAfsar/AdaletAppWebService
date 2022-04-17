using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.DAL.Abstract
{
    public interface IRepository<T> where T : class
    {
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(T entity);
        Task<T> Get(int id);
        Task<List<T>> GetAll(Expression<Func<T, bool>> predicate = null);
        Task DeleteAll(Expression<Func<T, bool>> predicate = null);
    }
}
