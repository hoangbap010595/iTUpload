using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upload.SpreadShirts.Areas.Upload.Models
{
    public class ExecuteUpload
    {
        public string Image { get; set; }
        public double Price { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tag { get; set; }
        public string Shop { get; set; }
    }
}