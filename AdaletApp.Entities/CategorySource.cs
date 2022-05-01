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
        public Category Category { get; set; }
        public SourceList Source { get; set; }
        public string SourceUrl { get; set; }
    }
}
