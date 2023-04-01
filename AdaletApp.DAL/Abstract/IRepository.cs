using System.Linq.Expressions;

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

        Task<List<T>> GetEntitesByPagination(Expression<Func<T, bool>> predicate = null, int pageNumber = 1, int limitCount = 10);
        Task<int> GetEntityCount();
    }
}
