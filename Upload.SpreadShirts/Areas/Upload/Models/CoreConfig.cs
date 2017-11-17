using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upload.SpreadShirts.Areas.Upload.Models
{
    public static class CoreConfig
    {

        public static string VERSION = "2.21.7";
        public static string API_PATH = "/api/v1";
        public static string API_KEY = "1c711bf5-b82d-40de-bea6-435b5473cf9b";
        public static string API_SECRET = "fd9f23cc-2432-4a69-9dad-bbd57b7b9fdd";
        public static bool API_RELATIVE_SELF_LINKS = false;
        public static bool FORCE_HTTPS_LINKS = true;
        public static bool HTML5_ROUTES = true;
        public static string LOGGER_SERVER_URL = "";
        public static bool LOGGER_CLIENT_LOGGING = false;
        public static string IMAGE_SERVER_PATH = "/image-server";
        public static string RAYGUN_KEY = "942bVUYrpgpw1jUqiLEx3A==";
        public static bool RAYGUN_ACTIVE = true;
        public static string LOCALE_URL = "./locales/";
        public static string LOCALE_DEFAULT = "us_US'";
        public static string ENVIRONMENT = "ops";
        public static string PLATFORM = "na";
        public static string TABLOMAT_SHOPID = "1048679";
        public static bool LIVECHAT_ACTIVE = true;
    }
}