using AdaletApp.DAL.Abstract;
using AdaletApp.Entities;
using AdaletApp.WEBAPI.Abstract;
using AdaletApp.WEBAPI.Utilities;
using AdaletApp.WEBAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdaletApp.WEBAPI.Controllers
{
    [Route("api/[controller]")]

    [CustomAuthorize("RootAdmin")]
    [ServiceFilter(typeof(CustomFilterAttribute<Category>))]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IArticleRepository _articleRespository;
        private readonly IFileService _fileService;
        private readonly ResponseResult<Category> responseResult;
        public CategoriesController(ICategoryRepository categoryRepository, IArticleRepository articleRespository, IFileService fileService)
        {
            _categoryRepository = categoryRepository;
            responseResult = new ResponseResult<Category>();
            _articleRespository = articleRespository;
            this._fileService = fileService;
        }

        [AllowAnonymous]
        [HttpGet("GetCategory/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var entity = HttpContext.Items["entity"] as Category;
            this.responseResult.Entity = entity;
            return Ok(this.responseResult);
        }
        [AllowAnonymous]
        [HttpPost("GetCategory")]
        public async Task<IActionResult> GetCategory([FromBody] CategoryViewModel model)
        {
            var entity = await _categoryRepository.GetCategoryWithLatestAndPopularNews(model.Slug, model.LimitCount);
            if (entity == null)
            {
                this.responseResult.HasError = true;
                this.responseResult.IsSuccess = false;
                this.responseResult.StatusCode = System.Net.HttpStatusCode.NotFound;
                this.responseResult.ErrorList.Add("Object Not Found");
                return NotFound(this.responseResult);
            }
            this.responseResult.Entity = entity;
            this.responseResult.TotalCount = await _articleRespository.GetEntityCount(a => a.Category.SeoUrl == model.Slug);
            return Ok(this.responseResult);
        }

        [AllowAnonymous]
        [HttpGet("GetCategoryList")]
        public async Task<IActionResult> GetCategoryList()
        {
            this.responseResult.Entities = await _categoryRepository.GetAll();
            this.responseResult.TotalCount = await _categoryRepository.GetEntityCount();
            return Ok(this.responseResult);
        }
        [AllowAnonymous]
        [HttpGet("GetCategoryListWithArticleCount")]
        public async Task<IActionResult> GetCategoryListWithArticleCount()
        {
            this.responseResult.Entities = await _categoryRepository.GetActiveCategoryListWithArticleCount();
            this.responseResult.TotalCount = this.responseResult.Entities.Count();
            return Ok(this.responseResult);
        }


        [AllowAnonymous]
        [HttpGet("GetCategoryListSourceList/{id}")]
        public async Task<IActionResult> GetCategoryWithCategorySourceList(int id)
        {
            var entity = HttpContext.Items["entity"] as Category;
            this.responseResult.Entity = await _categoryRepository.GetCategoryWithCategorySourceList(id);
            this.responseResult.TotalCount = await _categoryRepository.GetEntityCount(a => a.Id == id);
            return Ok(this.responseResult);
        }


        [AllowAnonymous]
        [HttpGet("GetCategoryListByPagination/{pageSize}/{limitCount}")]
        public async Task<IActionResult> GetCategoryList(int pageSize, int limitCount)
        {
            this.responseResult.Entities = await _categoryRepository.GetEntitesByPagination(predicate: null, pageSize, limitCount);
            this.responseResult.TotalCount = await _categoryRepository.GetEntityCount();
            return Ok(this.responseResult);
        }


        [AllowAnonymous]
        [HttpGet("GetCategoryBySlug/{slug}")]
        public async Task<IActionResult> GetCategoryBySlug(string slug)
        {
            this.responseResult.Entity = await _categoryRepository.Get(a => a.SeoUrl == slug);
            this.responseResult.TotalCount = await _categoryRepository.GetEntityCount(a => a.SeoUrl == slug);
            return Ok(this.responseResult);
        }

        [AllowAnonymous]
        [HttpGet("GetMainPageCategories")]
        public async Task<IActionResult> GetMainPageCategories()
        {
            var entities = await _categoryRepository.GetAll(a => a.MainPageCategory == true);
            var result = entities.OrderBy(a => a.Queue).Select(a => new Category()
            {
                CategoryName = a.CategoryName,
                Id = a.Id,
                Articles = a.Articles.OrderByDescending(b => b.CreateDate).Select(article => new Article
                {
                    Id = article.Id,
                    Title = article.Title,
                    SeoUrl = article.SeoUrl,
                    SubTitle = article.SubTitle,
                    PictureUrl = article.PictureUrl,
                }).Take(3).ToList()
            }).ToList();

            this.responseResult.Entities = result;
            this.responseResult.TotalCount = entities.Count();
            return Ok(this.responseResult);
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            category.SeoUrl = "/" + Helper.Helper.KarakterDuzelt(category.CategoryName + " haberleri");
            this.responseResult.Entity = await _categoryRepository.Add(category);
            return Ok(responseResult);
        }

        [HttpPut("UpdateCategory/{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {

            var entity = HttpContext.Items["entity"] as Category;
            var categorySeoUrl = "/" + Helper.Helper.KarakterDuzelt(category.CategoryName + " haberleri");

            if (entity.CategoryName != category.CategoryName)
            {
                var articleEntities = await _articleRespository.GetAll(a => a.CategoryId == entity.Id);
                articleEntities.ForEach(async (item) =>
                {
                    item.SeoUrl = categorySeoUrl + "/" + Helper.Helper.KarakterDuzelt(item.Title);
                    await _articleRespository.Update(item);
                });
            }

            entity.Explanation = category.Explanation;
            entity.UpdateDate = DateTime.Now;
            entity.SeoKeywords = category.SeoKeywords;
            entity.Active = category.Active;
            entity.CategoryName = category.CategoryName;
            entity.MainPageCategory = category.MainPageCategory;
            entity.SeoDescription = category.SeoDescription;
            entity.SeoTitle = category.SeoTitle;
            entity.SeoUrl = categorySeoUrl;


            this.responseResult.Entity = await _categoryRepository.Update(entity);
            return Ok(responseResult);
        }


        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var entity = await _categoryRepository.GetCategoryIncludeNewsPictures(id);

            if (entity.NewsPictures != null)
            {
                entity.NewsPictures.ForEach((item) =>
                {
                    _fileService.DeleteFile(item);
                });
            }

            this.responseResult.Entity = await _categoryRepository.Delete(entity);
            return Ok(this.responseResult);
        }
    }
}
