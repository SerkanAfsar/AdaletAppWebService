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
    [CustomAuthorize("RootAdmin", "Editor")]

    [ServiceFilter(typeof(CustomFilterAttribute<Article>))]
    public class NewsController : Controller
    {
        private readonly ICategorySourceRepository _categorySourceRepository;
        private readonly IArticleRepository articleRepository;
        private readonly IFileService fileService;
        private readonly ResponseResult<Article> responseResult;
        public NewsController(ICategorySourceRepository _categorySourceRepository, IArticleRepository articleRepository, IFileService fileService)
        {
            this._categorySourceRepository = _categorySourceRepository;
            this.articleRepository = articleRepository;
            this.responseResult = new ResponseResult<Article>();
            this.fileService = fileService;
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
        [HttpGet("GetAllNews")]
        public async Task<IActionResult> GetAllNews()
        {
            this.responseResult.Entities = await articleRepository.GetAllNewsOrderByIdDescending();
            return Ok(this.responseResult);
        }

        [AllowAnonymous]
        [HttpGet("GetLastFourNews")]
        public async Task<IActionResult> GetLastFourNews()
        {
            this.responseResult.Entities = await articleRepository.GetLastFourNews();
            return Ok(this.responseResult);
        }

        [AllowAnonymous]
        [HttpGet("GetNewsByCategoryId/{categoryId}")]
        public async Task<IActionResult> GetNewsByCategoryId(int categoryId)
        {
            this.responseResult.Entities = await articleRepository.GetAll(a => a.CategoryId == categoryId);
            return Ok(this.responseResult);
        }
        [AllowAnonymous]
        [HttpPost("GetNewsByPagination")]
        public async Task<IActionResult> GetNewsByPagination([FromBody] CategoryViewModel model)
        {
            this.responseResult.Entities = await articleRepository.GetArticlesByCategorySlugLimit(model.Slug, (int)model.PageSize, model.LimitCount);
            this.responseResult.TotalCount = await articleRepository.GetEntityCount(a => a.Category.SeoUrl == model.Slug);
            return Ok(this.responseResult);
        }



        [AllowAnonymous]
        [HttpGet("GetNewsByPagination/{pageCount}/{limit}")]
        public async Task<IActionResult> GetNewsByPagination(int pageCount = 1, int limit = 10)
        {
            this.responseResult.Entities = await articleRepository.GetArticlesByPagination(pageCount, limit);
            this.responseResult.TotalCount = await articleRepository.GetEntityCount();
            return Ok(this.responseResult);
        }




        [HttpPost("AddArticle")]
        public async Task<IActionResult> AddActicle([FromForm] UpdateArticleViewModel model)
        {
            var pictureUrl = string.Empty;
            var title = Helper.Helper.KarakterDuzelt(model.Title);
            if (model.FileInput != null)
            {
                var fileExt = Path.GetExtension(model.FileInput.FileName);
                var fileName = title + fileExt;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images", fileName);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.FileInput.CopyToAsync(fileStream);
                }
                pictureUrl = fileName;
            }

            var articleModel = new Article()
            {
                Title = model.Title,
                CategoryId = model.CategoryId,
                NewsContent = model.NewsContent,
                PictureUrl = !string.IsNullOrEmpty(pictureUrl) ? pictureUrl : null,
                Source = model.Source,
                SeoUrl = title,
                SubTitle = model.SubTitle,
                SourceUrl = model.SourceUrl,
            };

            var entity = await this.articleRepository.Add(articleModel);
            this.responseResult.Entity = entity;
            return Ok(this.responseResult);
        }


        [HttpPost("RecordAllNewsToDb")]
        public async Task<IActionResult> RegisterNews()
        {
            await this._categorySourceRepository.SaveAllNews();
            return Ok(this.responseResult);
        }

        [HttpPut("UpdateArticle/{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromForm] UpdateArticleViewModel model)
        {
            var entity = HttpContext.Items["entity"] as Article;

            var title = Helper.Helper.KarakterDuzelt(model.Title);

            entity.Active = model.Active;
            entity.CategoryId = model.CategoryId;
            entity.NewsContent = model.NewsContent;
            entity.PictureUrl = model.PictureUrl;
            entity.ReadCount = model.ReadCount;
            entity.SeoUrl = title;
            entity.Source = model.Source;
            entity.SourceUrl = model.SourceUrl;
            entity.SubTitle = model.SubTitle;
            entity.Title = model.Title;
            entity.UpdateDate = DateTime.Now;

            if (model.FileInput != null)
            {
                var fileExt = Path.GetExtension(model.FileInput.FileName);
                var fileName = title + fileExt;
                fileService.DeleteFile(fileName);
                var filePathNew = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images", fileName);
                using (Stream fileStream = new FileStream(filePathNew, FileMode.Create))
                {
                    await model.FileInput.CopyToAsync(fileStream);
                }
                entity.PictureUrl = fileName;
            }


            this.responseResult.Entity = await articleRepository.Update(entity);
            return Ok(this.responseResult);
        }

        [HttpDelete("DeleteArticle/{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var entity = HttpContext.Items["entity"] as Article;
            fileService.DeleteFile(entity.PictureUrl);
            this.responseResult.Entity = await articleRepository.Delete(entity);
            return Ok(this.responseResult);
        }

    }
}
