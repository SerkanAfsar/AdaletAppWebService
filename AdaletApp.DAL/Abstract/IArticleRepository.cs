using AdaletApp.Entities;

namespace AdaletApp.DAL.Abstract
{
    public interface IArticleRepository : IRepository<Article>
    {
        Task<bool> HasArticle(string title);
        Task<List<Article>> GetArticlesByCategorySlugLimit(string slug, int pageNumber, int limit);
        Task<Article> GetArticleIncludeCategory(int NewsID);

        Task<List<Article>> GetAllNewsOrderByIdDescending();

        Task<List<Article>> GetArticlesByPagination(int pageNumber = 1, int limitCount = 10);
        Task<List<Article>> GetTopReadedNewsByCategoryIdAsync(int categoryId, int limitCount = 10);
        Task<List<Article>> GetLatestNewsByCategoryIdAsync(int categoryId, int limitCount = 10);
        Task<List<Article>> GetMainPageTopReadedNewsAsync(int limitCount = 10);
        Task<List<Article>> GetMainPageLastAddedNewsAsync(int limitCount = 10);


    }
}
