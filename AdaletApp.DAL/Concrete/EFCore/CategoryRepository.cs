using AdaletApp.DAL.Abstract;
using AdaletApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdaletApp.DAL.Concrete.EFCore
{
    public class CategoryRepository : Repository<AppDbContext, Category>, ICategoryRepository
    {

        public async Task<Category> GetCategoryBySlug(string slug)
        {
            using (var db = new AppDbContext())
            {
                return await db.Categories.FirstOrDefaultAsync(a => a.SeoUrl == slug);
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
                    .OrderBy(a => a.Queue).Select(a => new Category()
                    {
                        CategoryName = a.CategoryName,
                        Id = a.Id,
                        Articles = a.Articles.OrderByDescending(b => b.CreateDate).Select(article => new Article
                        {
                            Id = article.Id,
                            Title = article.Title,
                            SeoUrl = article.SeoUrl,
                            SubTitle = article.SubTitle,
                            PictureUrl = article.PictureUrl,
                        }).Take(3).ToList()
                    }).ToListAsync();
                return list;
            }
        }
    }
}
