using AdaletApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.DAL.Abstract
{
    public interface ISourceRepository
    {
        Task ArticleSourceList(string categorySourceUrl,int CategoryID);
        Task AddArticleToDb(string articleSourceUrl, int CategoryID);

    }
}
