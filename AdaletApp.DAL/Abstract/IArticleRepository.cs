using AdaletApp.Entities;

namespace AdaletApp.DAL.Abstract
{
    public interface IArticleRepository : IRepository<Article>
    {
        Task<bool> HasArticle(string title);
        Task<List<Article>> GetArticlesByCategorySlugLimit(string slug, int pageNumber, int limit);
        Task<Article> GetArticleIncludeCategory(int NewsID);

        Task<List<Article>> GetAllNewsOrderByIdDescending();
        Task<List<Article>> GetLastFourNews();
        Task<List<Article>> GetArticlesByPagination(int pageNumber = 1, int limitCount = 10);



    }
}
