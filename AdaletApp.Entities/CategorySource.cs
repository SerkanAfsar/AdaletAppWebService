using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdaletApp.Entities
{
    public class CategorySource : BaseEntity
    {
        [Required(ErrorMessage = "Kategori Seçiniz")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        [Required(ErrorMessage = "Kaynak Seçiniz")]
        //[Range(1, 3, ErrorMessage = "Kaynak Seçimi Yanlış")]
        public SourceList Source { get; set; }

        [Required(ErrorMessage = "Kaynak Url Giriniz...")]
        public string SourceUrl { get; set; }
    }
}
