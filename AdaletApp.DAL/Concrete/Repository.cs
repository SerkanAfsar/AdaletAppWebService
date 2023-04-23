using AdaletApp.DAL.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AdaletApp.DAL.Concrete
{
    public class Repository<Context, T> : IRepository<T> where T : class
        where Context : DbContext, new()
    {
        public virtual async Task<T> Add(T entity)
        {
            using (var db = new Context())
            {
                await db.Set<T>().AddAsync(entity);
                await db.SaveChangesAsync();
                return entity;
            }

        }

        public async Task<T> Delete(T entity)
        {
            using (var db = new Context())
            {
                db.Set<T>().Remove(entity);
                await db.SaveChangesAsync();
                return entity;
            }
        }

        public async Task DeleteAll(Expression<Func<T, bool>> predicate = null)
        {
            using (var db = new Context())
            {
                db.Set<T>().RemoveRange(await db.Set<T>().Where(predicate).ToListAsync());
                await db.SaveChangesAsync();
            }
        }

        public async Task<T> Get(int id)
        {
            using (var db = new Context())
            {
                var entity = await db.Set<T>().FindAsync(id);
                if (entity != null)
                {
                    return entity;
                }
                return null;
            }
        }

        public async Task<T> Get(Expression<Func<T, bool>> predicate = null)
        {
            using (var db = new Context())
            {
                var entity = await db.Set<T>().FirstOrDefaultAsync(predicate);
                if (entity != null)
                {
                    return entity;
                }
                return null;
            }
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            using (var db = new Context())
            {
                return predicate == null ? await db.Set<T>().ToListAsync() : await db.Set<T>().Where(predicate).ToListAsync();
            }
        }

        public virtual async Task<List<T>> GetEntitesByPagination(Expression<Func<T, bool>> predicate = null, int pageNumber = 1, int limitCount = 10)
        {
            using (var db = new Context())
            {
                return predicate == null ? await db.Set<T>().Skip((pageNumber - 1) * limitCount).Take(limitCount).ToListAsync() :
                    await db.Set<T>().Where(predicate).Skip((pageNumber - 1) * limitCount).Take(limitCount).ToListAsync();
            }
        }

        public async Task<int> GetEntityCount(Expression<Func<T, bool>> predicate = null)
        {
            using (var db = new Context())
            {
                if (predicate == null)
                {
                    return await db.Set<T>().CountAsync();
                }
                return await db.Set<T>().Where(predicate).CountAsync();

            }
        }

        public virtual async Task<T> Update(T entity)
        {
            using (var db = new Context())
            {
                db.Set<T>().Update(entity);
                await db.SaveChangesAsync();
                return entity;
            }
        }
    }
}
