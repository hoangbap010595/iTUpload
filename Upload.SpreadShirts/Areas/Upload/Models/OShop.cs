using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upload.SpreadShirts.Areas.Upload.Models
{
    public class OShop
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string TargetID { get; set; }
        public bool isSelected { get; set; }
    }
}