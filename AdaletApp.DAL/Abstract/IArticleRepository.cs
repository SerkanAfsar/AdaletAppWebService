using AdaletApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.DAL.Abstract
{
    public interface IArticleRepository : IRepository<Article>
    {
        public Task<bool> HasArticle(string title);
        public Task<List<Article>> GetArticlesByCategoryIdLimit(int CategoryID, int pageNumber, int limit);
        public Task<Article> GetArticleIncludeCategory(int NewsID);
        public Task<int> GetAllNewsCount();
    }
}
