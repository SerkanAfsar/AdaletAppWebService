using AdaletApp.DAL.Abstract;
using AdaletApp.DAL.Abstract.NewsWebSites;
using AdaletApp.Entities;
using Microsoft.EntityFrameworkCore;

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

        public async Task<CategorySource> GetCategorySourceIncludeCategoryById(int CategorySourceId)
        {
            using (var db = new AppDbContext())
            {
                return await db.CategorySource.Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == CategorySourceId);
            }
        }





        public async Task<List<CategorySource>> GetCategorySourceList(int? CategoryID = null, int pageNumber = 1, int limitCount = 10)
        {
            using (var db = new AppDbContext())
            {
                return CategoryID != null ?
                    await db.CategorySource.Where(a => a.CategoryId == CategoryID).Skip((pageNumber - 1) * limitCount).Take(limitCount).Include(a => a.Category).ToListAsync() :
                    await db.CategorySource.Skip((pageNumber - 1) * limitCount).Take(limitCount).Include(a => a.Category).ToListAsync();
            }
        }
        public override async Task<CategorySource> Add(CategorySource entity)
        {
            var result = await base.Add(entity);

            using (var db = new AppDbContext())
            {
                return await db.CategorySource.Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == result.Id);
            }
        }

        public override async Task<CategorySource> Update(CategorySource entity)
        {
            var result = await base.Update(entity);

            using (var db = new AppDbContext())
            {
                return await db.CategorySource.Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == result.Id);
            }
        }


        public async Task SaveAllNews()
        {
            var list = new List<string>();
            var result = await this.GetAll(null);
            foreach (var item in result)
            {
                switch (item.Source)
                {
                    case SourceList.HUKUKİHABER:
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

