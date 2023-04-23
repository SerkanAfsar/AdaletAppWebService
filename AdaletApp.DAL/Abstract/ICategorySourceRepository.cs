using AdaletApp.Entities;

namespace AdaletApp.DAL.Abstract
{
    public interface ICategorySourceRepository : IRepository<CategorySource>
    {
        public Task SaveAllNews();
        Task<List<CategorySource>> GetCategorySourceList(int? CategoryID = null, int pageNumber = 1, int limitCount = 10);
        Task<CategorySource> GetCategorySourceIncludeCategoryById(int CategorySourceId);




    }
}
