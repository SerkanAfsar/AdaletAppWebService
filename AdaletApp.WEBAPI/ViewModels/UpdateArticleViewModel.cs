using AdaletApp.Entities;

namespace AdaletApp.WEBAPI.ViewModels
{
    public class UpdateArticleViewModel : Article
    {
        public IFormFile? FileInput { get; set; }
    }
}
