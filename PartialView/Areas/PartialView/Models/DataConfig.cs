using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PartialView.Areas.PartialView.Models
{
    public class DataConfig
    {
        public String ConnectionString { get; set; }
        public String SPName { get; set; }
        public String JsonFilter { get; set; }

        public DataConfig()
        {
            this.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            this.SPName = "";
            this.JsonFilter = "";
        }
    }

}