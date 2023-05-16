using AdaletApp.Entities;

namespace AdaletApp.WEBAPI.ViewModels
{
    public class UpdateArticleDTO : Article
    {
        public IFormFile? FileInput { get; set; }
    }
}
