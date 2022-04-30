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
        private readonly IAdaletBizRepository adaletBizRepository;
        private readonly IHukukiHaberRepository hukukiHaberRepository;
        public CategorySourceRepository(IAdaletBizRepository adaletBizRepository, IHukukiHaberRepository hukukiHaberRepository)
        {
            this.adaletBizRepository = adaletBizRepository;
            this.hukukiHaberRepository = hukukiHaberRepository;
        }
        public async Task SaveAllNews()
        {
            var list = new List<string>();
            var result = await this.GetAll(null);
            foreach (var item in result)
            {
                switch (item.Source)
                {
                    case SourceList.HUKUKHABERLERI:
                        {
                            await this.hukukiHaberRepository.ArticleSourceList(item.SourceUrl, item.CategoryId);
                            break;
                        }
                    case SourceList.ADALETBIZ:
                        {
                            await this.adaletBizRepository.ArticleSourceList(item.SourceUrl, item.CategoryId);
                            break;
                        }
                    default:
                        break;
                }
            }

        }
    }
}
