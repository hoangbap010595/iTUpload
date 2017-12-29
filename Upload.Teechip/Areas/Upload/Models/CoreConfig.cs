using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Upload.Teechip.Areas.Upload.Models
{
    public static class CoreConfig
    {
        public static string Origin = "https://pro.teechip.com";
        public static string readDataFromFile(string filePath)
        {
            var data = "";
            using (StreamReader reader = new StreamReader(filePath))
            {
                data = reader.ReadToEnd();
            }
            return data;
        }
    }
}