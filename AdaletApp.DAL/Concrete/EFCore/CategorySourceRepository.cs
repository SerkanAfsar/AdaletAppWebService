using AdaletApp.DAL.Abstract;
using AdaletApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.DAL.Concrete.EFCore
{
    public class CategorySourceRepository : Repository<AppDbContext, CategorySource>, ICategorySourceRepository
    {
    }
}
