using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upload.SpreadShirts.Areas.Upload.Models
{
    public class ApplicationUser
    {
        public string USER_ID { get; set; }
        public string USER_HREF { get; set; }
        public string SESSION_ID { get; set; }
        public string SESSION_HREF { get; set; }
        public string IDENTITY_ID { get; set; }
        public string IDENTITY_HREF { get; set; }
        public List<OShop> SHOPS { get; set; }
    }
}