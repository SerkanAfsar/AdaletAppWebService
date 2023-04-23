using AdaletApp.DAL.Abstract;
using AdaletApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdaletApp.DAL.Concrete.EFCore
{
    public class ArticleRepository : Repository<AppDbContext, Article>, IArticleRepository
    {
        public async Task<List<Article>> GetAllNewsOrderByIdDescending()
        {
            using (var db = new AppDbContext())
            {
                return await db.Articles.OrderByDescending(a => a.Id).Select(a => new Article()
                {
                    Id = a.Id,
                    Title = a.Title,
                    PictureUrl = a.PictureUrl,
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

        public async Task<List<Article>> GetArticlesByCategorySlugLimit(string slug, int pageNumber, int limit)
        {
            using (var db = new AppDbContext())
            {
                return await db.Articles.Where(a => a.Category.SeoUrl == slug).Select(a => new Article()
                {
                    Id = a.Id,
                    Title = a.Title,
                    SeoUrl = a.SeoUrl,
                    PictureUrl = a.PictureUrl,
                    SubTitle = a.SubTitle,
                    CreateDate = a.CreateDate,
                }).OrderByDescending(a => a.CreateDate).Skip((pageNumber - 1) * limit).Take(limit).ToListAsync();

            }
        }

        public async Task<List<Article>> GetArticlesByPagination(int pageNumber = 1, int limitCount = 10)
        {
            using (var db = new AppDbContext())
            {
                return await db.Articles.OrderByDescending(a => a.CreateDate).Skip((pageNumber - 1) * limitCount).Select(a => new Article()
                {
                    Id = a.Id,
                    Title = a.Title,
                    PictureUrl = a.PictureUrl,
                    Category = new Category()
                    {
                        CategoryName = a.Category.CategoryName
                    },
                    Source = a.Source


                }).Take(limitCount).ToListAsync();
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
