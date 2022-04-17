using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.Entities
{
    public class Article : BaseEntity
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string? PictureBig { get; set; }
        public string? PictureMedium { get; set; }
        public string? PictureSmall { get; set; }
        public string SourceUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public SourceList Source { get; set; }
        public string SeoUrl { get; set; }
        public int ReadCount { get; set; } = 1;
    }
}
