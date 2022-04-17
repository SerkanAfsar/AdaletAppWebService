using AdaletApp.DAL.Abstract;
using AdaletApp.DAL.Concrete.EFCore;
using AdaletApp.Entities;
using AdaletApp.WEBAPI.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();
builder.Services.AddSingleton<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<CustomFilterAttribute<Article>>();
builder.Services.AddScoped<CustomFilterAttribute<Category>>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
