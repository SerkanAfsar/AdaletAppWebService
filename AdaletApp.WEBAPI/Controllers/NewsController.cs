using AdaletApp.DAL.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdaletApp.WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class NewsController : Controller
    {
        private readonly IHukukiHaberRepository hukukiHaberRepository;
        public NewsController(IHukukiHaberRepository hukukiHaberRepository)
        {
            this.hukukiHaberRepository = hukukiHaberRepository;
        }
        [HttpGet("GetCategoryNewsByUrl/{categoryUrl}")]
        public async Task<IActionResult> GetCategoryNewsByUrl(string categoryUrl)
        {
            return Ok(await this.hukukiHaberRepository.ArticleSourceList("deneme"));
        }
    }
}
