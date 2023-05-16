using AdaletApp.DAL.Abstract;
using AdaletApp.Entities;
using AdaletApp.WEBAPI.Abstract;
using AdaletApp.WEBAPI.Utilities;
using AdaletApp.WEBAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AdaletApp.WEBAPI.Controllers
{
    [Route("api/[controller]")]


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


        [HttpGet("GetCategory/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var entity = HttpContext.Items["entity"] as Category;
            this.responseResult.Entity = entity;
            return Ok(this.responseResult);
        }
        [HttpGet("GetCategoryBySlug/{slug}")]
        public async Task<IActionResult> GetCategoryBySlug(string slug)
        {
            this.responseResult.Entity = await _categoryRepository.Get(a => a.SeoUrl == slug);
            this.responseResult.TotalCount = await _categoryRepository.GetEntityCount(a => a.SeoUrl == slug);
            return Ok(this.responseResult);
        }

        [HttpPost("GetCategory")]
        public async Task<IActionResult> GetCategory([FromBody] CategoryDTO model)
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


        [HttpGet("GetCategoryList")]
        public async Task<IActionResult> GetCategoryList()
        {
            this.responseResult.Entities = await _categoryRepository.GetAll();
            this.responseResult.TotalCount = await _categoryRepository.GetEntityCount();
            return Ok(this.responseResult);
        }

        [HttpGet("GetCategoryListMainPage")]
        public async Task<IActionResult> GetMainPageCategories()
        {
            this.responseResult.Entities = await _categoryRepository.GetMainPageCategoriesWithArticles();
            this.responseResult.TotalCount = this.responseResult.Entities.Count();
            return Ok(this.responseResult);
        }

        [HttpGet("GetMainPageTopSixCategories")]
        public async Task<IActionResult> GetMainPageTopSixCategories()
        {
            this.responseResult.Entities = await _categoryRepository.GetMainPageTopSixCategories();
            this.responseResult.TotalCount = this.responseResult.Entities.Count();
            return Ok(this.responseResult);
        }

        [HttpGet("GetCategoryListWithArticleCount")]
        public async Task<IActionResult> GetCategoryListWithArticleCount()
        {
            this.responseResult.Entities = await _categoryRepository.GetActiveCategoryListWithArticleCount();
            this.responseResult.TotalCount = this.responseResult.Entities.Count();
            return Ok(this.responseResult);
        }

        [HttpGet("GetCategoryListByPagination/{pageSize}/{limitCount}")]
        public async Task<IActionResult> GetCategoryList(int pageSize, int limitCount)
        {
            this.responseResult.Entities = await _categoryRepository.GetEntitesByPagination(predicate: null, pageSize, limitCount);
            this.responseResult.TotalCount = await _categoryRepository.GetEntityCount();
            return Ok(this.responseResult);
        }


        [CustomAuthorize("RootAdmin")]
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            category.SeoUrl = "/" + Helper.Helper.KarakterDuzelt(category.CategoryName + " haberleri");
            this.responseResult.Entity = await _categoryRepository.Add(category);
            return Ok(responseResult);
        }
        [CustomAuthorize("RootAdmin")]
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

        [CustomAuthorize("RootAdmin")]
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
