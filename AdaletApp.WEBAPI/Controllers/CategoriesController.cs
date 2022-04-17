using AdaletApp.DAL.Abstract;
using AdaletApp.Entities;
using AdaletApp.WEBAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AdaletApp.WEBAPI.Controllers
{
    [Route("api/[controller]")]
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
        [HttpGet("GetCategory/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var entity = HttpContext.Items["entity"] as Category;
            this.responseResult.Entity = entity;
            return Ok(this.responseResult);
        }
        [HttpGet("GetCategoryList")]
        public async Task<IActionResult> GetCategoryList()
        {
            this.responseResult.Entities = await _categoryRepository.GetAll();
            return Ok(this.responseResult);
        }
    }
}
