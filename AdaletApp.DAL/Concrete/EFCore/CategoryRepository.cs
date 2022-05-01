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
