using AdaletApp.DAL.Abstract;
using AdaletApp.DAL.Abstract.NewsWebSites;
using AdaletApp.DAL.Concrete;
using AdaletApp.DAL.Concrete.EFCore;
using AdaletApp.DAL.Concrete.NewsWebSites;
using AdaletApp.WEBAPI.Abstract;
using AdaletApp.WEBAPI.Concrete;

namespace AdaletApp.WEBAPI.Services
{
    public static class AddRepositoryServices
    {
        public static void AddRepositoyServices(this IServiceCollection services)
        {
            services.AddSingleton<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<IArticleRepository, ArticleRepository>();
            services.AddSingleton<ICategorySourceRepository, CategorySourceRepository>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IFileService, FileService>();
            services.AddSingleton<IHukukiHaberRepository, HukukiHaberRepository>();
            services.AddSingleton<IAdaletBizRepository, AdaletBizRepository>();
            services.AddSingleton<IAdaletMedyaRepository, AdaletMedyaRepository>();
        }
    }
}
