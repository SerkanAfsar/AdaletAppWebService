using AdaletApp.DAL.Abstract;
using AdaletApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.DAL.Concrete.EFCore
{
    public class CategoryRepository : Repository<AppDbContext, Category>, ICategoryRepository
    {
        public async Task<int> GetAllCategoryCount()
        {
            using (var db = new AppDbContext())
            {
                return await db.Categories.CountAsync();
            }
        }

        public async Task<Category> GetCategoryWithCategorySourceList(int CategoryID)
        {
            using (var db = new AppDbContext())
            {
                return await db.Categories.Include(a => a.CategorySourceList).FirstOrDefaultAsync(a => a.Id == CategoryID);
            }
        }

        public async Task<List<Category>> GetMainPageCategories()
        {
            using (var db = new AppDbContext())
            {
                var list = await db.Categories
                    .Where(a => a.MainPageCategory == true)
                    .OrderBy(a => a.Queue).Select(a => new Category() { Articles = a.Articles.OrderByDescending(b => b.CreateDate).Take(5).ToList() }).ToListAsync();
                return list;
            }
        }
    }
}
