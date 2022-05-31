using AdaletApp.DAL.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.DAL.Concrete
{
    public class Repository<Context, T> : IRepository<T> where T : class
        where Context : DbContext, new()
    {
        public async Task<T> Add(T entity)
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

        public async Task<List<T>> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            using (var db = new Context())
            {
                return predicate == null ? await db.Set<T>().ToListAsync() : await db.Set<T>().Where(predicate).ToListAsync();
            }
        }

        public async Task<T> Update(T entity)
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
