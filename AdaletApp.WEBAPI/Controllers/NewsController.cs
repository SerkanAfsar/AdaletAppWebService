using AdaletApp.DAL.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdaletApp.WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class NewsController : Controller
    {
        private readonly ICategorySourceRepository _categorySourceRepository;
        public NewsController(ICategorySourceRepository _categorySourceRepository)
        {
            this._categorySourceRepository = _categorySourceRepository;

        }


        [AllowAnonymous]
        [HttpGet("GetCategoryNewsByUrl")]
        public async Task<IActionResult> GetCategoryNewsByUrl()
        {

            //return Ok(await this._categorySourceRepository.GetAllNewsUrl())
            await this._categorySourceRepository.SaveAllNews();
            return Ok("Bütün Haberler Kaydedildi");
        }
    }
}
