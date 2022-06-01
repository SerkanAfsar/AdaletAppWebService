using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.Entities
{
    public class CategorySource : BaseEntity
    {
        [Required(ErrorMessage = "Category Id is Required")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        [Required(ErrorMessage = "Source Is Required")]
        [Range(1, 3, ErrorMessage = "Kaynak Seçiniz")]
        public SourceList Source { get; set; }
        [Required(ErrorMessage = "Kaynak Url Giriniz...")]
        public string SourceUrl { get; set; }
    }
}
