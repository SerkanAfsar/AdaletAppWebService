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

        public SourceList Source { get; set; }

        [Required(ErrorMessage = "Kaynak Url Giriniz 1...")]
        public string SourceUrl { get; set; }
    }
}
