using AdaletApp.Entities;

namespace AdaletApp.DAL.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        public Task<List<Category>> GetMainPageCategories();
        public Task<Category> GetCategoryWithCategorySourceList(int CategoryID);
        public Task<Category> GetCategoryBySlug(string slug);

    }
}
