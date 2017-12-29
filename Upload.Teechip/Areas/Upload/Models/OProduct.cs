using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upload.Teechip.Areas.Upload.Models
{
    public class OProduct
    {
        public string Id { get; set; }
        public string PrintSize { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<OColor> Colors { get; set; }
        public int Msrp { get; set; }

    }
}