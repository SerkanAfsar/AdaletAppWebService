using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.Entities
{
    public class Article : BaseEntity
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string NewsContent { get; set; }
        public string? PictureUrl { get; set; }

        public string SourceUrl { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public SourceList Source { get; set; }
        public string SeoUrl { get; set; }
        public int ReadCount { get; set; } = 1;
    }
}
