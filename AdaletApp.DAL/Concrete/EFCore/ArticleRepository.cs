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
        public async Task<List<Article>> GetArticlesByCategoryIdLimit(int CategoryID, int pageNumber, int limit)
        {
            using (var db = new AppDbContext())
            {
                return await db.Articles.Where(a => a.CategoryId == CategoryID).OrderByDescending(a => a.CreateDate).Skip(pageNumber * limit).Take(limit).ToListAsync();

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
