using AdaletApp.DAL.Abstract;
using AdaletApp.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AdaletApp.DAL.Concrete.EFCore
{
    public class CategoryRepository : Repository<AppDbContext, Category>, ICategoryRepository
    {
        public async Task<List<Category>> GetActiveCategoryListWithArticleCount()
        {
            using (var db = new AppDbContext())
            {
                return await db.Categories.Where(a => a.Active == true).Select(a => new Category()
                {
                    CategoryName = a.CategoryName,
                    ArticleCount = a.Articles.Count(),
                    SeoUrl = a.SeoUrl,
                }).ToListAsync();
            }
        }

        public async Task<Category> GetCategoryIncludeNewsPictures(int CategoryID)
        {
            using (var db = new AppDbContext())
            {
                return await db.Categories.Select(a => new Category()
                {
                    Id = a.Id,
                    NewsPictures = a.Articles.Select(b => b.PictureUrl).ToList()
                }).FirstOrDefaultAsync(a => a.Id == CategoryID);
            }
        }

        public async Task<Category> GetCategoryWithCategorySourceList(int CategoryID)
        {
            using (var db = new AppDbContext())
            {
                return await db.Categories.Include(a => a.CategorySourceList).FirstOrDefaultAsync(a => a.Id == CategoryID);
            }
        }

        public async Task<Category> GetCategoryWithLatestAndPopularNews(string seoUrl, int limitCount = 10)
        {
            using (var db = new AppDbContext())
            {
                return await db.Categories.Select(a => new Category()
                {
                    CategoryName = a.CategoryName,
                    SeoTitle = a.SeoTitle,
                    SeoUrl = a.SeoUrl,
                    Explanation = a.Explanation,
                    SeoDescription = a.SeoDescription,
                    LatestNews = a.Articles.OrderByDescending(b => b.CreateDate).Select(a => new Article() { Id = a.Id, Title = a.Title, SeoUrl = a.SeoUrl, CreateDate = a.CreateDate, PictureUrl = a.PictureUrl }).Take(limitCount).ToList(),
                    PopularNews = a.Articles.OrderByDescending(b => b.ReadCount).Select(a => new Article() { Id = a.Id, Title = a.Title, SeoUrl = a.SeoUrl, CreateDate = a.CreateDate, PictureUrl = a.PictureUrl }).Take(limitCount).ToList()
                }).FirstOrDefaultAsync(a => a.SeoUrl == seoUrl);
            }
        }

        public override async Task<List<Category>> GetEntitesByPagination(Expression<Func<Category, bool>> predicate = null, int pageNumber = 1, int limitCount = 10)
        {
            using (var db = new AppDbContext())
            {
                return predicate == null ? await db.Categories.Skip((pageNumber - 1) * limitCount).Take(limitCount).Select(a => new Category()
                {
                    Id = a.Id,
                    CategoryName = a.CategoryName,
                    ArticleCount = a.Articles.Count()
                }).ToListAsync() :
                       await db.Categories.Where(predicate).Skip((pageNumber - 1) * limitCount).Take(limitCount).Select(a => new Category()
                       {
                           Id = a.Id,
                           CategoryName = a.CategoryName,
                           ArticleCount = a.Articles.Count()
                       }).ToListAsync();
            }
        }

        //public async Task<List<Category>> GetMainPageCategories()
        //{
        //    using (var db = new AppDbContext())
        //    {
        //        var list = await db.Categories
        //            .Where(a => a.MainPageCategory == true)
        //            .OrderBy(a => a.Queue).Select(a => new Category()
        //            {
        //                CategoryName = a.CategoryName,
        //                Id = a.Id,
        //                Articles = a.Articles.OrderByDescending(b => b.CreateDate).Select(article => new Article
        //                {
        //                    Id = article.Id,
        //                    Title = article.Title,
        //                    SeoUrl = article.SeoUrl,
        //                    SubTitle = article.SubTitle,
        //                    PictureUrl = article.PictureUrl,
        //                }).Take(3).ToList()
        //            }).ToListAsync();
        //        return list;
        //    }
        //}
    }
}
