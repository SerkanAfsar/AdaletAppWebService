using AdaletApp.DAL.Abstract;
using AdaletApp.Entities;
using AdaletApp.WEBAPI.Abstract;
using AdaletApp.WEBAPI.DTO;
using AdaletApp.WEBAPI.Utilities;
using AdaletApp.WEBAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdaletApp.WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(CustomFilterAttribute<Article>))]
    public class NewsController : Controller
    {
        private readonly ICategorySourceRepository _categorySourceRepository;
        private readonly IArticleRepository articleRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IFileService fileService;
        private readonly ResponseResult<Article> responseResult;
        public NewsController(ICategorySourceRepository _categorySourceRepository, IArticleRepository articleRepository, IFileService fileService, ICategoryRepository categoryRepository)
        {
            this._categorySourceRepository = _categorySourceRepository;
            this.articleRepository = articleRepository;
            this.responseResult = new ResponseResult<Article>();
            this.fileService = fileService;
            this.categoryRepository = categoryRepository;
        }

        [AllowAnonymous]
        [HttpGet("GetNewsById/{id}")]
        public async Task<IActionResult> GetNewsById(int id)
        {
            var entity = HttpContext.Items["entity"] as Article;
            this.responseResult.Entity = entity;
            return Ok(this.responseResult);
        }


        [HttpPost("GetSingleNewsBySlug")]
        public async Task<IActionResult> GetSingleNewsBySlug([FromBody] NewsDTO model)
        {
            var entity = await articleRepository.Get(a => a.SeoUrl == model.Slug);
            if (entity == null)
            {
                this.responseResult.StatusCode = System.Net.HttpStatusCode.NotFound;
                this.responseResult.IsSuccess = false;
                this.responseResult.HasError = true;
                this.responseResult.ErrorList.Add("Object Not Found");
                return NotFound(this.responseResult);
            }
            this.responseResult.Entity = entity;
            return Ok(this.responseResult);
        }


        [HttpGet("GetAllNews")]
        public async Task<IActionResult> GetAllNews()
        {
            this.responseResult.Entities = await articleRepository.GetAllNewsOrderByIdDescending();
            this.responseResult.TotalCount = this.responseResult.Entities.Count();
            return Ok(this.responseResult);
        }

        [HttpGet("GetNewsByCategoryId/{categoryId}")]
        public async Task<IActionResult> GetNewsByCategoryId(int categoryId)
        {
            this.responseResult.Entities = await articleRepository.GetAll(a => a.CategoryId == categoryId);
            return Ok(this.responseResult);
        }

        [HttpGet("GetTopReadedNewsByCategoryId/{categoryId}/{limitCount}")]
        public async Task<IActionResult> GetTopReadedNewsByCategoryId(int categoryId, int limitCount = 10)
        {

            this.responseResult.Entities = await articleRepository.GetTopReadedNewsByCategoryIdAsync(categoryId, limitCount);
            return Ok(this.responseResult);
        }

        [HttpGet("GetLatestNewsByCategoryId/{categoryId}/{limitCount}")]
        public async Task<IActionResult> GetLatestNewsByCategoryId(int categoryId, int limitCount = 10)
        {
            this.responseResult.Entities = await articleRepository.GetLatestNewsByCategoryIdAsync(categoryId, limitCount);
            return Ok(this.responseResult);
        }

        //[HttpGet("Reset")]
        //public async Task<IActionResult> Reset()
        //{

        //    var list = await articleRepository.GetAll();
        //    foreach (var item in list)
        //    {
        //        var entity = item;
        //        entity.ReadCount = 0;
        //        await articleRepository.Update(entity);
        //    }
        //    return Ok("Resetlendi");
        //}

        [HttpGet("GetMainPageTopReadedNews/{limitCount}")]
        public async Task<IActionResult> GetMainPageTopReadedNews(int limitCount = 10)
        {

            this.responseResult.Entities = await articleRepository.GetMainPageTopReadedNewsAsync(limitCount);
            return Ok(this.responseResult);
        }

        [HttpGet("GetMainPageLastAddedNews/{limitCount}")]
        public async Task<IActionResult> GetMainPagePopularNews(int limitCount = 10)
        {

            this.responseResult.Entities = await articleRepository.GetMainPageLastAddedNewsAsync(limitCount);
            return Ok(this.responseResult);
        }


        [HttpPost("GetNewsByPagination")]
        public async Task<IActionResult> GetNewsByPagination([FromBody] CategoryDTO model)
        {
            this.responseResult.Entities = await articleRepository.GetArticlesByCategorySlugLimit(model.Slug, (int)model.PageSize, model.LimitCount);
            this.responseResult.TotalCount = await articleRepository.GetEntityCount(a => a.Category.SeoUrl == model.Slug);
            this.responseResult.PaginationItemCount = this.responseResult.Entities.Count();
            return Ok(this.responseResult);
        }

        [HttpGet("GetNewsByPagination/{pageCount}/{limit}")]
        public async Task<IActionResult> GetNewsByPagination(int pageCount = 1, int limit = 10)
        {
            this.responseResult.Entities = await articleRepository.GetArticlesByPagination(pageCount, limit);
            this.responseResult.TotalCount = await articleRepository.GetEntityCount();
            return Ok(this.responseResult);
        }


        [CustomAuthorize("RootAdmin", "Editor")]
        [HttpPost("AddArticle")]
        public async Task<IActionResult> AddActicle([FromForm] UpdateArticleDTO model)
        {
            var pictureUrl = string.Empty;
            var categoryEntity = await categoryRepository.Get(a => a.Id == model.CategoryId);
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
                SeoUrl = categoryEntity.SeoUrl + "/" + title,
                SubTitle = model.SubTitle,
                SourceUrl = model.SourceUrl,
            };

            var entity = await this.articleRepository.Add(articleModel);
            this.responseResult.Entity = entity;
            return Ok(this.responseResult);
        }


        [CustomAuthorize("RootAdmin", "Editor")]
        [HttpPost("RecordAllNewsToDb")]
        public async Task<IActionResult> RegisterNews()
        {
            await this._categorySourceRepository.SaveAllNews();
            return Ok(this.responseResult);
        }

        [CustomAuthorize("RootAdmin", "Editor")]
        [HttpPut("UpdateArticle/{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromForm] UpdateArticleDTO model)
        {
            var entity = HttpContext.Items["entity"] as Article;

            var categoryEntity = await categoryRepository.Get(a => a.Id == model.CategoryId);
            var title = Helper.Helper.KarakterDuzelt(model.Title);

            entity.Active = model.Active;
            entity.CategoryId = model.CategoryId;
            entity.NewsContent = model.NewsContent;
            entity.PictureUrl = model.PictureUrl;
            entity.ReadCount = model.ReadCount;
            entity.SeoUrl = categoryEntity.SeoUrl + "/" + title;
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
        [CustomAuthorize("RootAdmin", "Editor")]

        [HttpDelete("DeleteArticle/{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var entity = HttpContext.Items["entity"] as Article;
            fileService.DeleteFile(entity.PictureUrl);
            this.responseResult.Entity = await articleRepository.Delete(entity);
            return Ok(this.responseResult);
        }
        [CustomAuthorize("RootAdmin", "Editor")]
        [HttpPut("UpdateArticleCount/{id}")]
        public async Task<IActionResult> UpdateArticleCount(int id)
        {
            var entity = HttpContext.Items["entity"] as Article;
            entity.ReadCount = entity.ReadCount + 1;

            this.responseResult.Entity = await articleRepository.Update(entity);
            return Ok(this.responseResult);
        }
    }
}
