using AdaletApp.Entities;

namespace AdaletApp.DAL.Abstract
{
    public interface IArticleRepository : IRepository<Article>
    {
        public Task<bool> HasArticle(string title);
        public Task<List<Article>> GetArticlesByCategoryIdLimit(int CategoryID, int pageNumber, int limit);
        public Task<Article> GetArticleIncludeCategory(int NewsID);
        public Task<int> GetAllNewsCount();
        public Task<List<Article>> GetAllNewsOrderByIdDescending();
        public Task<List<Article>> GetLastFourNews();


    }
}
