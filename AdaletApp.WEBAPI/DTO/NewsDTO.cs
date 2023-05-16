using System.ComponentModel.DataAnnotations;

namespace AdaletApp.WEBAPI.DTO
{
    public class NewsDTO
    {
        [Required(ErrorMessage = "News Slug Required")]
        public string Slug { get; set; }
    }
}
