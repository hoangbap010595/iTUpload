using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upload.Teechip.Areas.Upload.Models
{
    public class ExecuteUpload
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public List<string> LineID { get; set; }
        public List<string> RetailID { get; set; }
    }
}