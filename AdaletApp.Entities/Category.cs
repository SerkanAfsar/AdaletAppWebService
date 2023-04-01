using System.ComponentModel.DataAnnotations;

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
    }
}
