using AdaletApp.Entities;
using AdaletApp.WEBAPI.Utilities;
using AdaletApp.WEBAPI.ViewModels;

namespace AdaletApp.WEBAPI.Services
{
    public static class AddCustomFilterServices
    {
        public static void AddCustomFilterServicesTo(this IServiceCollection services)
        {
            services.AddScoped<CustomFilterAttribute<Article>>();
            services.AddScoped<CustomFilterAttribute<Category>>();
            services.AddScoped<CustomFilterAttribute<CategorySource>>();
            services.AddScoped<CustomFilterAttribute<UserLoginDTO>>();
            services.AddScoped<CustomFilterAttribute<UserRegisterDTO>>();
            services.AddScoped<CustomFilterAttribute<AppUser>>();
            services.AddScoped<CustomFilterAttribute<AppRole>>();
        }
    }
}
