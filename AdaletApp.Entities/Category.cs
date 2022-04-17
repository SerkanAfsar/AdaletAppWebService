using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
        public string Explanation { get; set; }
        public bool MainPageCategory { get; set; } = false;
        public string SeoUrl { get; set; }
        public List<Article> Articles { get; set; } = new List<Article>();
    }
}
