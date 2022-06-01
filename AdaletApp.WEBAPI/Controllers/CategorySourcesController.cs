using AdaletApp.DAL.Abstract;
using AdaletApp.Entities;
using AdaletApp.WEBAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdaletApp.WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [CustomAuthorize("RootAdmin")]
    [ServiceFilter(typeof(CustomFilterAttribute<CategorySource>))]
    public class CategorySourcesController : Controller
    {
        private readonly ICategorySourceRepository categorySourceRepository;
        public ResponseResult<CategorySource> responseResult;
        public CategorySourcesController(ICategorySourceRepository _categorySourceRepository)
        {
            categorySourceRepository = _categorySourceRepository;
            responseResult = new ResponseResult<CategorySource>();
        }

        [HttpGet("GetCategorySource/{id}")]
        public IActionResult GetCategorySource(int id)
        {
            var entity = HttpContext.Items["entity"] as CategorySource;
            this.responseResult.Entity = entity;
            return Ok(this.responseResult);
        }

        [HttpGet("GetCategorySourceList/{CategoryID}")]
        public async Task<IActionResult> GetCategorySourceList(int? CategoryID = null)
        {
            this.responseResult.Entities = await categorySourceRepository.GetCategorySourceListIncludeCategory(CategoryID);
            return Ok(this.responseResult);
        }
        [HttpPost("AddCategorySource")]
        public async Task<IActionResult> AddCategorySource([FromBody] CategorySource categorySource)
        {
            this.responseResult.Entity = await categorySourceRepository.Add(categorySource);
            return Ok(this.responseResult);
        }
        [HttpPut("UpdateCategorySource/{id}")]
        public async Task<IActionResult> UpdateCategorySource(int id, [FromBody] CategorySource model)
        {
            var entity = HttpContext.Items["entity"] as CategorySource;
            entity.UpdateDate = DateTime.Now;
            entity.Active = model.Active;
            entity.CategoryId = model.CategoryId;
            entity.Source = model.Source;
            entity.SourceUrl = model.SourceUrl;

            this.responseResult.Entity = await categorySourceRepository.Update(entity);
            return Ok(responseResult);
        }
        [HttpDelete("DeleteCategorySource/{id}")]
        public async Task<IActionResult> DeleteCategorySource(int id)
        {
            var entity = HttpContext.Items["entity"] as CategorySource;
            this.responseResult.Entity = await categorySourceRepository.Delete(entity);
            return Ok(this.responseResult);
        }
    }
}
