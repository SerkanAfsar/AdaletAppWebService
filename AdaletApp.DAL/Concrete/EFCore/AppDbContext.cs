using AdaletApp.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AdaletApp.DAL.Concrete.EFCore
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new ConfigurationBuilder().
                SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build().
                GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategorySource> CategorySource { get; set; }
        public DbSet<Article> Articles { get; set; }
    }
}
