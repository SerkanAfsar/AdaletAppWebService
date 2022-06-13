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
    public class ArticleRepository : Repository<AppDbContext, Article>, IArticleRepository
    {
        public async Task<int> GetAllNewsCount()
        {
            using (var db = new AppDbContext())
            {
                return await db.Articles.CountAsync();
            }
        }

        public async Task<List<Article>> GetAllNewsOrderByIdDescending()
        {
            using (var db = new AppDbContext())
            {
                return await db.Articles.OrderByDescending(a => a.Id).Select(a => new Article()
                {
                    Id = a.Id,
                    Title = a.Title,
                    PictureUrl = a.PictureUrl,
                    CategoryName = a.Category.CategoryName,
                    CategorySeoUrl = a.Category.SeoUrl,
                    SeoUrl = a.SeoUrl
                }).ToListAsync();
            }
        }

        public async Task<Article> GetArticleIncludeCategory(int NewsID)
        {
            using (var db = new AppDbContext())
            {
                return await db.Articles.Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == NewsID);
            }
        }

        public async Task<List<Article>> GetArticlesByCategoryIdLimit(int CategoryID, int pageNumber, int limit)
        {
            using (var db = new AppDbContext())
            {
                return await db.Articles.Where(a => a.CategoryId == CategoryID).OrderByDescending(a => a.CreateDate).Skip(pageNumber * limit).Take(limit).ToListAsync();

            }
        }

        public async Task<List<Article>> GetLastFourNews()
        {
            using (var db = new AppDbContext())
            {
                return await db.Articles.OrderByDescending(a => a.Id).Take(4).Select(a => new Article
                {
                    Id = a.Id,
                    CreateDate = a.CreateDate,
                    Title = a.Title,
                    SeoUrl = a.SeoUrl,
                    PictureUrl = a.PictureUrl,
                    CategoryName = a.Category.CategoryName,
                    CategorySeoUrl = a.Category.SeoUrl
                }).ToListAsync();
            }
        }

        public async Task<bool> HasArticle(string title)
        {
            using (var db = new AppDbContext())
            {
                var entity = await db.Articles.FirstOrDefaultAsync(a => a.Title == title);
                return entity == null;
            }
        }
    }
}
