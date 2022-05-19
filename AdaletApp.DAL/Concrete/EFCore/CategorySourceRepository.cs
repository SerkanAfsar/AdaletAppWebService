using AdaletApp.DAL.Abstract;
using AdaletApp.DAL.Abstract.NewsWebSites;
using AdaletApp.Entities;
using Microsoft.EntityFrameworkCore;
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
        private readonly IAdaletMedyaRepository adaletMedyaRepository;
        public CategorySourceRepository(IAdaletBizRepository adaletBizRepository,
            IHukukiHaberRepository hukukiHaberRepository,
            IAdaletMedyaRepository adaletMedyaRepository)
        {
            this.adaletBizRepository = adaletBizRepository;
            this.hukukiHaberRepository = hukukiHaberRepository;
            this.adaletMedyaRepository = adaletMedyaRepository;

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
                    case SourceList.ADALETMEDYA:
                        {
                            await this.adaletMedyaRepository.ArticleSourceList(item.SourceUrl, item.CategoryId);
                            break;
                        }
                    default:
                        break;
                }
            }

        }
    }
}
