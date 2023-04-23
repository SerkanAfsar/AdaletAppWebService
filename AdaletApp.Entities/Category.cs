using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdaletApp.Entities
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "Category Name Required")]
        public string CategoryName { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string? SeoKeywords { get; set; }
        public string? Explanation { get; set; }
        public bool MainPageCategory { get; set; } = false;
        public string? SeoUrl { get; set; }
        public List<CategorySource>? CategorySourceList { get; set; } = new List<CategorySource>();
        public List<Article>? Articles { get; set; } = new List<Article>();
        [NotMapped]
        public int? ArticleCount { get; set; }
        [NotMapped]
        public List<string> NewsPictures { get; set; } = new List<string>();

        [NotMapped]
        public List<Article> LatestNews { get; set; } = new List<Article>();
        [NotMapped]
        public List<Article> PopularNews { get; set; } = new List<Article>();
    }
}
