using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaletApp.Entities;

namespace AdaletApp.DAL.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        public Task<List<Category>> GetMainPageCategories();
    }
}
