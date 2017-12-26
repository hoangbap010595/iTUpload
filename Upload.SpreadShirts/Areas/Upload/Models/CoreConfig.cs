using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PartialView.Areas.PartialView.Models;

namespace Upload.SpreadShirts.Areas.Upload.Models
{
    public static class CoreConfig
    {
        //Server
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
        //User
        public static string Origin = "https://partner.spreadshirt.com";
        public static string encodeURL(string url, string defaultParam = "", string method = "POST", string locale = "", string mediaType = "", string sessionId = "", string time = "")
        {
            string t_url = url.Split('?')[0];
            string t_time = time == "" ? ApplicationLibary.GetTimeStamp() : time;
            string t_data = method + " " + t_url + " " + t_time;
            //string t_sig = GetSHA1HashData(t_data);
            string t_sig = ApplicationLibary.GetHMACSHA1HasData(t_data, API_SECRET);

            int index = t_url.IndexOf('?');
            var newUrl = "";// t_url;
            if (index == -1)
                newUrl += "?";
            else
                newUrl += "&";
            newUrl += defaultParam == "" ? "" : defaultParam + "&";
            newUrl += "apiKey=" + API_KEY;
            if (locale != "")
                newUrl += "&locale=" + locale;//us_US
            if (mediaType != "")
                newUrl += "&mediaType=" + mediaType;//json
            newUrl += "&sig=" + t_sig + "&time=" + t_time;
            if (sessionId != "")
                newUrl += "&sessionId=" + sessionId;
            return t_url + newUrl;
        }
    }
}