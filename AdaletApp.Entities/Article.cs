using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdaletApp.Entities
{

    public class Article : BaseEntity
    {
        [Required(ErrorMessage = "Haber Başlık Giriniz...")]
        public string Title { get; set; }
        public string? SubTitle { get; set; }
        public string? NewsContent { get; set; }
        public string? PictureUrl { get; set; }
        public string? SourceUrl { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Kategori Seçiniz")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        public SourceList Source { get; set; }
        public string? SeoUrl { get; set; }

        public int ReadCount { get; set; } = 0;
    }
}
