using AdaletApp.DAL.Abstract;
using AdaletApp.Entities;
using AdaletApp.WEBAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdaletApp.WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ServiceFilter(typeof(CustomFilterAttribute<Category>))]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ResponseResult<Category> responseResult;
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
            responseResult = new ResponseResult<Category>();
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
        [HttpGet("GetCategoryWithCategorySourceList/{id}")]

        public async Task<IActionResult> GetCategoryWithCategorySourceList(int id)
        {
            var entity = HttpContext.Items["entity"] as Category;
            this.responseResult.Entity = await _categoryRepository.GetCategoryWithCategorySourceList(id);
            return Ok(this.responseResult);
        }
        [AllowAnonymous]
        [HttpGet("GetCategoryCount")]

        public async Task<IActionResult> GetCategoryCount()
        {

            return Ok(await _categoryRepository.GetAllCategoryCount());
        }

        [AllowAnonymous]
        [HttpGet("GetCategoryList")]
        public async Task<IActionResult> GetCategoryList()
        {
            this.responseResult.Entities = await _categoryRepository.GetAll();
            return Ok(this.responseResult);
        }

        [AllowAnonymous]
        [HttpGet("GetMainPageCategories")]
        public async Task<IActionResult> GetMainPageCategories()
        {
            this.responseResult.Entities = await _categoryRepository.GetMainPageCategories();
            return Ok(this.responseResult);
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            category.SeoUrl = FriendlySeoUrl.KarakterDuzelt(category.CategoryName);
            this.responseResult.Entity = await _categoryRepository.Add(category);
            return Ok(responseResult);
        }

        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {

            var entity = HttpContext.Items["entity"] as Category;
            entity.Explanation = category.Explanation;
            entity.SeoUrl = category.SeoUrl;
            entity.UpdateDate = DateTime.Now;
            entity.SeoKeywords = category.SeoKeywords;
            entity.Active = category.Active;
            entity.CategoryName = category.CategoryName;
            entity.MainPageCategory = category.MainPageCategory;
            entity.SeoDescription = category.SeoDescription;
            entity.SeoTitle = category.SeoTitle;
            entity.SeoUrl = FriendlySeoUrl.KarakterDuzelt(category.CategoryName);

            this.responseResult.Entity = await _categoryRepository.Update(entity);
            return Ok(responseResult);
        }


        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var entity = HttpContext.Items["entity"] as Category;
            this.responseResult.Entity = await _categoryRepository.Delete(entity);
            return Ok(this.responseResult);
        }


    }
}
