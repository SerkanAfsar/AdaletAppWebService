using AdaletApp.DAL.Abstract;
using AdaletApp.DAL.Utilites;
using AdaletApp.Entities;
using AdaletApp.WEBAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdaletApp.WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ServiceFilter(typeof(CustomFilterAttribute<Article>))]
    public class NewsController : Controller
    {
        private readonly ICategorySourceRepository _categorySourceRepository;
        private readonly IArticleRepository articleRepository;
        private readonly ResponseResult<Article> responseResult;
        public NewsController(ICategorySourceRepository _categorySourceRepository, IArticleRepository articleRepository)
        {
            this._categorySourceRepository = _categorySourceRepository;
            this.articleRepository = articleRepository;
            this.responseResult = new ResponseResult<Article>();

        }

        [AllowAnonymous]
        [HttpGet("GetNewsById/{id}")]
        public async Task<IActionResult> GetNewsById(int id)
        {
            var entity = HttpContext.Items["entity"] as Article;
            this.responseResult.Entity = entity;
            return Ok(this.responseResult);
        }

        [AllowAnonymous]
        [HttpGet("GetNewsByCategoryID/{categoryId}")]
        public async Task<IActionResult> GetNewsByCategoryID(int categoryId)
        {
            this.responseResult.Entities = await articleRepository.GetAll(a => a.CategoryId == categoryId);
            return Ok(this.responseResult);
        }
        [AllowAnonymous]
        [HttpGet("GetNewsCount")]
        public async Task<IActionResult> GetNewsCount()
        {
            return Ok(await articleRepository.GetAllNewsCount());
        }
        [AllowAnonymous]
        [HttpGet("GetNewsByCategoryIDPager/{categoryId}/{pageCount}/{limit}")]
        public async Task<IActionResult> GetNewsByCategoryID(int categoryId, int pageCount, int limit)
        {
            this.responseResult.Entities = await articleRepository.GetArticlesByCategoryIdLimit(categoryId, pageCount, limit);
            return Ok(this.responseResult);
        }
        [HttpPost("AddArticle")]
        public async Task<IActionResult> AddActicle([FromBody] Article model)
        {
            var entity = await this.articleRepository.Add(model);
            this.responseResult.Entity = entity;
            return Ok(this.responseResult);
        }

        [AllowAnonymous]
        [HttpPost("RecordAllNewsToDb")]
        public async Task<IActionResult> RegisterNews()
        {
            await this._categorySourceRepository.SaveAllNews();
            return Ok(this.responseResult);
        }

        [HttpPut("UpdateArticle/{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] Article model)
        {
            var entity = HttpContext.Items["entity"] as Article;

            entity.Active = model.Active;
            entity.CategoryId = model.CategoryId;
            entity.NewsContent = model.NewsContent;
            entity.PictureUrl = model.PictureUrl;

            entity.ReadCount = model.ReadCount;
            entity.SeoUrl = Helper.KarakterDuzelt(model.Title);
            entity.Source = model.Source;
            entity.SourceUrl = model.SourceUrl;
            entity.SubTitle = model.SubTitle;
            entity.Title = model.Title;
            entity.UpdateDate = DateTime.Now;

            this.responseResult.Entity = await articleRepository.Update(entity);
            return Ok(this.responseResult);
        }

        [HttpDelete("DeleteArticle/{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var entity = HttpContext.Items["entity"] as Article;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images", entity.PictureUrl);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);

            }
            this.responseResult.Entity = await articleRepository.Delete(entity);
            return Ok(this.responseResult);
        }

    }
}
