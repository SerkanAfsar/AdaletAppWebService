using AdaletApp.Entities;

namespace AdaletApp.DAL.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category> GetCategoryWithCategorySourceList(int CategoryID);
        Task<Category> GetCategoryIncludeNewsPictures(int CategoryID);
        Task<List<Category>> GetActiveCategoryListWithArticleCount();
        Task<List<Category>> GetMainPageCategoriesWithArticles();

        Task<Category> GetCategoryWithLatestAndPopularNews(string seoUrl, int limitCount = 10);


    }
}
