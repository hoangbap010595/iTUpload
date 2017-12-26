using Newtonsoft.Json.Linq;
using PartialView.Areas.PartialView.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Upload.SpreadShirts.Areas.Upload.Models;

namespace Upload.SpreadShirts.Areas.Upload.Controllers
{
    public class SpreadShirtController : Controller
    {
        private static CookieContainer cookieApplication = new CookieContainer();
        private ApplicationUser User;
        private string currToken = "";
        public SpreadShirtController()
        {
            //User = new ApplicationUser();
        }

        // GET: Upload/SpreadShirt
        public ActionResult SpreadShirt()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ExecuteLogin(string username, string password)
        {
            Dictionary<string, object> lsData = new Dictionary<string, object>();
            var json = new JsonResult();
            try
            {
                var urlLogin = CoreConfig.encodeURL("https://partner.spreadshirt.com/api/v1/sessions", "", "POST", "us_US", "json", "");
                //string urlLogin = "https://www.spreadshirt.com/api/v1/sessions?mediaType=json";
                string data2Send = "{\"rememberMe\":false,\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";

                HttpWebRequest wRequestLogin = (HttpWebRequest)WebRequest.Create(urlLogin);
                wRequestLogin.Headers.Add("Accept-Language", "vi-VN,vi;q=0.8,en-US;q=0.5,en;q=0.3");
                wRequestLogin.Accept = "application/json, text/plain, */*";
                wRequestLogin.Host = "partner.spreadshirt.com";
                wRequestLogin.ContentType = "application/json;charset=utf-8";
                wRequestLogin.Referer = "https://partner.spreadshirt.com/login";
                wRequestLogin.CookieContainer = new CookieContainer();

                Dictionary<string, object> step2Login = PostDataAPI(wRequestLogin, data2Send);
                cookieApplication = (CookieContainer)step2Login["cookies"];
                var rs = step2Login["data"].ToString();
                if (int.Parse(step2Login["status"].ToString()) == -1)
                {
                    lsData.Add("message", "Tài khoản mật khẩu không đúng");
                    lsData.Add("icon", "error");
                    lsData.Add("result", -1);
                    json = Json(lsData, JsonRequestBehavior.AllowGet);
                    return json;
                }

                var obj = JObject.Parse(rs);
                User = new ApplicationUser();
                User.SESSION_ID = obj["id"].ToString();
                User.SESSION_HREF = obj["href"].ToString();
                User.IDENTITY_ID = obj["identity"]["id"].ToString();
                User.IDENTITY_HREF = obj["identity"]["href"].ToString();
                User.USER_ID = obj["user"]["id"].ToString();
                User.USER_HREF = obj["user"]["href"].ToString();

                string urlShop = User.USER_HREF + "/pointsOfSale";
                urlShop = CoreConfig.encodeURL(urlShop, "", "GET", "us_US", "json", "");

                HttpWebRequest wRequestShopping = (HttpWebRequest)WebRequest.Create(urlShop);
                wRequestShopping.Headers.Add("Accept-Language", "vi-VN,vi;q=0.8,en-US;q=0.5,en;q=0.3");
                wRequestShopping.Accept = "application/json, text/plain, */*";
                wRequestShopping.Host = "partner.spreadshirt.com";
                wRequestShopping.ContentType = "application/json;charset=utf-8";
                wRequestShopping.CookieContainer = cookieApplication;
                Dictionary<string, object> dataShop = GetDataAPI(wRequestShopping);

                JObject objShop = JObject.Parse(dataShop["data"].ToString());
                var listShop = objShop["list"].ToString();
                JArray arrShop = JArray.Parse(listShop);

                User.SHOPS = new List<OShop>();
                foreach (var item in arrShop)
                {
                    if (!item["type"].ToString().Equals("MARKETPLACE") && !item["type"].ToString().Equals("CYO"))
                    {
                        OShop o = new OShop();
                        o.Id = item["id"].ToString();
                        o.Name = item["name"].ToString();
                        o.TargetID = item["target"]["id"].ToString();
                        o.Type = item["type"].ToString();
                        User.SHOPS.Add(o);
                    }
                }
                lsData.Add("message", "Đăng nhập thành công");
                lsData.Add("icon", "success");
                lsData.Add("result", 1);
                json = Json(lsData, JsonRequestBehavior.AllowGet);
                return json;
            }
            catch (Exception ex)
            {
                lsData.Add("message", "Xảy ra lỗi. " + ex.Message);
                lsData.Add("icon", "success");
                lsData.Add("result", -1);
                json = Json(lsData, JsonRequestBehavior.AllowGet);
                return json;
            }
        }

        #region ================= Post & Get =================
        private Dictionary<string, object> PostDataAPI(HttpWebRequest wRequest, string data2Send)
        {
            Dictionary<string, object> dataReturn = new Dictionary<string, object>();
            CookieContainer cookies = new CookieContainer();
            String htmlString;
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] postDataBytes = encoding.GetBytes(data2Send);
                wRequest.Method = "POST";
                wRequest.UserAgent = ApplicationLibary.BROWSER_FIREFOX;
                wRequest.ContentLength = postDataBytes.Length;
                wRequest.Headers.Add("Origin", CoreConfig.Origin);

                using (Stream sr = wRequest.GetRequestStream())
                {
                    sr.Write(postDataBytes, 0, postDataBytes.Length);
                }

                using (HttpWebResponse wResponse = (HttpWebResponse)wRequest.GetResponse())
                {
                    foreach (Cookie cookie in wResponse.Cookies)
                    {
                        if (cookie.Name.Contains("sprd_auth_token"))
                            currToken = cookie.Value;
                        cookies.Add(cookie);
                    }
                    using (var reader = new StreamReader(wResponse.GetResponseStream()))
                    {
                        htmlString = reader.ReadToEnd();
                    }
                }

                dataReturn.Add("cookies", cookies);
                dataReturn.Add("data", htmlString);
                dataReturn.Add("status", 1);
                return dataReturn;
            }
            catch (Exception ex)
            {
                dataReturn.Add("cookies", cookies);
                dataReturn.Add("data", ex.Message);
                dataReturn.Add("status", -1);
                return dataReturn;
            }

        }
        private Dictionary<string, object> GetDataAPI(HttpWebRequest wRequest, string data2Send = "")
        {
            wRequest.Method = "GET";
            wRequest.UserAgent = ApplicationLibary.BROWSER_FIREFOX;
            wRequest.Headers.Add("Origin", CoreConfig.Origin);
            if (data2Send != "")
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] postDataBytes = encoding.GetBytes(data2Send);
                wRequest.ContentLength = postDataBytes.Length;

                using (Stream sr = wRequest.GetRequestStream())
                {
                    sr.Write(postDataBytes, 0, postDataBytes.Length);
                }
            }

            HttpWebResponse wResponse = (HttpWebResponse)wRequest.GetResponse();
            CookieContainer cookies = new CookieContainer();
            foreach (Cookie cookie in wResponse.Cookies)
            {
                if (cookie.Name.Contains("sprd_auth_token"))
                    currToken = cookie.Value;
                cookies.Add(cookie);
            }

            String htmlString;
            using (var reader = new StreamReader(wResponse.GetResponseStream()))
            {
                htmlString = reader.ReadToEnd();
            }

            Dictionary<string, object> dataReturn = new Dictionary<string, object>();
            dataReturn.Add("cookies", cookies);
            dataReturn.Add("data", htmlString);

            return dataReturn;
        }
        public static Dictionary<string, object> HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            HttpWebRequest wr = null;
            WebResponse wresp = null;
            try
            {
                Console.WriteLine(string.Format("Uploading {0} to {1}", file, url));
                string boundary = "-----------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                wr = (HttpWebRequest)WebRequest.Create(url);
                wr.Host = "partner.spreadshirt.com";
                wr.Accept = "application/json, text/plain, */*";
                wr.ContentType = "multipart/form-data; boundary=" + boundary;
                wr.Method = "POST";
                wr.Headers.Add("Accept-Language", "vi-VN,vi;q=0.8,en-US;q=0.5,en;q=0.3");
                wr.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                wr.Headers.Add("Origin", CoreConfig.Origin);
                wr.Referer = "https://partner.spreadshirt.com/designs";
                wr.UserAgent = ApplicationLibary.BROWSER_FIREFOX;
                wr.CookieContainer = cookieApplication;
                wr.KeepAlive = true;

                Stream rs = wr.GetRequestStream();
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (string key in nvc.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, nvc[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }
                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, paramName, Path.GetFileName(file), contentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                data.Add("location", wresp.Headers["Location"]);
                data.Add("data", reader2.ReadToEnd());
                data.Add("status", 1);
            }
            catch (Exception ex)
            {
                data.Add("data", ex.Message);
                data.Add("status", -1);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
            return data;
        }
        private Dictionary<string, object> PutDataAPI(HttpWebRequest wRequest, string data2Send)
        {
            Dictionary<string, object> dataReturn = new Dictionary<string, object>();
            CookieContainer cookies = new CookieContainer();
            String htmlString;
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] postDataBytes = encoding.GetBytes(data2Send);
                wRequest.Method = "PUT";
                wRequest.UserAgent = ApplicationLibary.BROWSER_FIREFOX;
                wRequest.ContentLength = postDataBytes.Length;
                wRequest.Headers.Add("Origin", CoreConfig.Origin);//chrome-extension://aejoelaoggembcahagimdiliamlcdmfm

                using (Stream sr = wRequest.GetRequestStream())
                {
                    sr.Write(postDataBytes, 0, postDataBytes.Length);
                }

                using (HttpWebResponse wResponse = (HttpWebResponse)wRequest.GetResponse())
                {
                    foreach (Cookie cookie in wResponse.Cookies)
                    {
                        if (cookie.Name.Contains("sprd_auth_token"))
                            currToken = cookie.Value;
                        cookies.Add(cookie);
                    }
                    using (var reader = new StreamReader(wResponse.GetResponseStream()))
                    {
                        htmlString = reader.ReadToEnd();
                    }
                    wResponse.Close();
                }

                dataReturn.Add("cookies", cookies);
                dataReturn.Add("data", htmlString);
                dataReturn.Add("status", 1);
                return dataReturn;
            }
            catch (Exception ex)
            {
                dataReturn.Add("cookies", cookies);
                dataReturn.Add("data", ex.Message);
                dataReturn.Add("status", -1);
                return dataReturn;
            }

        }
        #endregion
    }
}