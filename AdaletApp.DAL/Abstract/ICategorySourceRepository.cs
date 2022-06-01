using AdaletApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.DAL.Abstract
{
    public interface ICategorySourceRepository : IRepository<CategorySource>
    {
        public Task SaveAllNews();
        Task<List<CategorySource>> GetCategorySourceListIncludeCategory(int? CategoryID = null);


    }
}
