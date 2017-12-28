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
        //private ApplicationUser User;

        private string currToken = "";
        private string dataCYOID = "{\"pointOfSale\":{\"id\":\"56963c0a59248d4dfb5c3852\",\"name\":\"CYO\",\"type\":\"CYO\",\"target\":{\"id\":\"93439\"}},\"id\":\"56963c0a59248d4dfb5c3852\"}";
        private string dataMarkID = @"{""id"":""55c864cc64c7436b464aeb7b"",""pointOfSale"":{""id"":""55c864cc64c7436b464aeb7b"",""type"":""MARKETPLACE"",""target"":{""id"":""93439""},""allowed"":true}}";
        private string dataShopID = @"{""id"":""@ShopID"",""pointOfSale"":{""id"":""@ShopID"",""name"":""@ShopName"",""type"":""SHOP"",""target"":{""id"":""@TargetID"",""currencyId"":""3""},""allowed"":true},""commission"":{""amount"":0,""currencyId"":""3""}}";


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
                ApplicationUser User = new ApplicationUser();
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

                //Set current Users
                Session["USER"] = User;

                lsData.Add("shop", dataShop["data"].ToString());
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

        [HttpPost]
        public JsonResult UploadProgress(ExecuteUpload dataUpload)
        {
            Dictionary<string, object> dResult = new Dictionary<string, object>();
            //Set current Users

            var json = new JsonResult();
            try
            {
                var folder = "AdminUpload";
                if (Session["UserName"] != null)
                    folder = Session["UserName"].ToString();
                if (Session["USER"] != null)
                {
                    ApplicationUser User = (ApplicationUser)Session["USER"];

                    string image = Path.Combine(Server.MapPath("~/Uploaded/" + folder), @dataUpload.Image); // File image
                    string title = dataUpload.Title; // Tieu de sp
                    string description = dataUpload.Description;//Phan mo ta sp
                    string tags = dataUpload.Tag;//tag1,tag2,tag3
                    string shop = dataUpload.Shop;//shop1,shop2,shop3
                    double amount = double.Parse(dataUpload.Price.ToString("N2"));
                    if (!System.IO.File.Exists(image))
                    {
                        dResult.Add("data", "File not found: " + Path.GetFileName(image));
                        json = Json(dResult, JsonRequestBehavior.AllowGet);
                        return json;
                    }
                    #region -----------Step 1: Upload Image-----------
                    //ApplicationLibary.writeLogThread(lsBoxLog, "Uploadding " + Path.GetFileName(image), 3);
                    string img_UrlUpload = User.USER_HREF + "/design-uploads";
                    var urlUploadImage = CoreConfig.encodeURL(url: img_UrlUpload, defaultParam: "createProductIdea=true", time: ApplicationLibary.GetTimeStamp());
                    NameValueCollection nvc = new NameValueCollection();
                    var data = HttpUploadFile(urlUploadImage, image, "filedata", "image/png", nvc);
                    if (int.Parse(data["status"].ToString()) == -1)
                    {
                        dResult.Add("data", "Step 1: " + data["data"].ToString());
                        json = Json(dResult, JsonRequestBehavior.AllowGet);
                        return json;
                    }
                    JObject jObj = JObject.Parse(data["data"].ToString());
                    var designId = jObj["designId"].ToString();
                    var ideaId = jObj["ideaId"].ToString();
                    var name = jObj["name"].ToString();

                    #endregion

                    #region -----------Step 2: Upload Data-----------
                    string u_method = "PUT";
                    //https://partner.spreadshirt.com/api/v1/users/302721328/ideas/5a33342faa0c6d3e511164f3?apiKey=1c711bf5-b82d-40de-bea6-435b5473cf9b&locale=us_US&mediaType=json&sig=5a88e6520a13a9aa1f7b39036a7c120cd445ccab&time=1513305661500
                    string u_urlUpload = User.USER_HREF + "/ideas/" + ideaId;
                    string u_time = ApplicationLibary.GetTimeStamp();
                    string u_dataUrl = u_method + " " + u_urlUpload + " " + u_time + "";
                    string u_sig = ApplicationLibary.GetHMACSHA1HasData(u_dataUrl, CoreConfig.API_SECRET);
                    Dictionary<string, object> dataObj = new Dictionary<string, object>();
                    dataObj.Add("amount", amount);
                    dataObj.Add("shop", shop);
                    dataObj.Add("ideaID", ideaId);
                    dataObj.Add("designID", designId);
                    dataObj.Add("title", title);
                    dataObj.Add("description", description);
                    dataObj.Add("tags", tags);
                    dataObj.Add("sig", u_sig);

                    string rs_UrlUpload = CoreConfig.encodeURL(url: u_urlUpload, method: u_method, locale: "us_US", mediaType: "json", time: u_time);
                    string rs_Data2Send = refixData2Send(User,dataObj);

                    HttpWebRequest wRequestUpload = (HttpWebRequest)WebRequest.Create(rs_UrlUpload);
                    wRequestUpload.Headers.Add("Accept-Language", "vi-VN,vi;q=0.8,en-US;q=0.5,en;q=0.3");
                    wRequestUpload.Accept = "application/json, text/plain, */*";
                    wRequestUpload.Host = "partner.spreadshirt.com";
                    wRequestUpload.ContentType = "application/json;charset=utf-8";
                    wRequestUpload.Referer = "https://partner.spreadshirt.com/designs/" + ideaId;
                    wRequestUpload.CookieContainer = cookieApplication;

                    Dictionary<string, object> step2Upload = PutDataAPI(wRequestUpload, rs_Data2Send);
                    if (int.Parse(step2Upload["status"].ToString()) == -1)
                    {
                        dResult.Add("data", "Step 2: " + step2Upload["data"].ToString());
                        json = Json(dResult, JsonRequestBehavior.AllowGet);
                        return json;
                    }
                    var objUploadEnd = JObject.Parse(step2Upload["data"].ToString());
                    var linkIdea = objUploadEnd["href"].ToString();
                    #endregion

                    #region -----------Step 3: Publish-----------
                    //https://partner.spreadshirt.com/api/v1/users/302721328/ideas/5a339e2aaa0c6d3e511e3268/publishingDetails?apiKey=1c711bf5-b82d-40de-bea6-435b5473cf9b&locale=us_US&mediaType=json&sig=ff531cb9b45015934d699c796dc013033dcff8e8&time=1513333983186
                    var urlPublish = User.USER_HREF + "/ideas/" + ideaId + "/publishingDetails";
                    string p_urlPublish = CoreConfig.encodeURL(url: urlPublish, method: "PUT", locale: "us_US", mediaType: "json");
                    var p_data2SendPublish = @"{""list"": [" + dataCYOID;
                    foreach (string str in getPublishingDetailsPublish(User,shop))
                    {
                        var x = str.Replace(@"\", "");
                        p_data2SendPublish += "," + x;
                    }
                    p_data2SendPublish = p_data2SendPublish.TrimEnd(',') + "]}";

                    HttpWebRequest wRequestPublish = (HttpWebRequest)WebRequest.Create(p_urlPublish);
                    wRequestPublish.Headers.Add("Accept-Language", "vi-VN,vi;q=0.8,en-US;q=0.5,en;q=0.3");
                    wRequestPublish.Accept = "application/json, text/plain, */*";
                    wRequestPublish.Host = "partner.spreadshirt.com";
                    wRequestPublish.ContentType = "application/json;charset=utf-8";
                    wRequestPublish.Referer = "https://partner.spreadshirt.com/designs/" + ideaId;
                    wRequestPublish.CookieContainer = cookieApplication;

                    Dictionary<string, object> step3Publish = PutDataAPI(wRequestPublish, p_data2SendPublish);
                    if (int.Parse(step3Publish["status"].ToString()) == -1)
                    {
                        dResult.Add("data", "Step 3: " + step3Publish["data"].ToString());
                        json = Json(dResult, JsonRequestBehavior.AllowGet);
                        return json;
                    }
                    #endregion
                    DeleteFileUploaded(Path.GetFileName(image));
                    dResult.Add("data", "Upload & Publish finish: " + "https://partner.spreadshirt.com/designs/" + ideaId);
                    json = Json(dResult, JsonRequestBehavior.AllowGet);
                    return json;
                }else
                {
                    dResult.Add("data", "Session Invaild!");
                    json = Json(dResult, JsonRequestBehavior.AllowGet);
                    return json;
                }
            }
            catch (Exception ex)
            {
                dResult.Add("data", "Upload error: " + ex.Message);
                json = Json(dResult, JsonRequestBehavior.AllowGet);
                return json;
            }

        }
        private void DeleteFileUploaded(string fileName)
        {
            //Delete File 
            var folder = "AdminUpload";
            if (Session["UserName"] != null)
                folder = Session["UserName"].ToString();
            var physicalPath = Path.Combine(Server.MapPath("~/Uploaded/" + folder), fileName);

            // TODO: Verify user permissions

            if (System.IO.File.Exists(physicalPath))
            {
                // The files are not actually removed in this demo
                System.IO.File.Delete(physicalPath);
            }
        }
        private List<string> getPublishingDetailsPublish(ApplicationUser User,string shopName)
        {
            List<string> data = new List<string>();
            data.Add(dataMarkID);
            var lsShop = shopName.Split(',');
            foreach (string strShop in lsShop)
            {
                if (!string.IsNullOrEmpty(strShop))
                    foreach (OShop shop in User.SHOPS)
                    {
                        if (shop.Name.Contains(strShop) || shop.TargetID.Contains(strShop))
                        {
                            string x = dataShopID.Replace("@ShopID", shop.Id).Replace("@ShopName", shop.Name).Replace("@TargetID", shop.TargetID);
                            data.Add(x);
                        }
                    }
            }

            return data;
        }
        private string refixData2Send(ApplicationUser User,Dictionary<string, object> dataObj)
        {
            var file = Server.MapPath("~/App_Data");
            var fileName = Path.Combine(file, "spread_d2s.txt");
            string data2SendFromFile = CoreConfig.readDataFromFile(fileName);
            JObject obj = JObject.Parse(data2SendFromFile);
            int time = int.Parse(ApplicationLibary.GetTimeStamp());
            //set Amount
            obj["commission"]["amount"] = double.Parse(dataObj["amount"].ToString());

            //publishingDetails
            JArray item = (JArray)obj["publishingDetails"];
            foreach (Dictionary<string, object> itemShop in getPublishingDetails(User,dataObj["shop"].ToString()))
            {
                if (itemShop["type"].ToString().Equals("MARKETPLACE"))
                {
                    item.Add(JObject.Parse(dataMarkID));
                }
                else
                {
                    var xDa = dataShopID.Replace("@ShopID", itemShop["id"].ToString()).Replace("@ShopName", itemShop["name"].ToString()).Replace("@TargetID", itemShop["targetID"].ToString());
                    item.Add(JObject.Parse(xDa));

                }
            }
            //set Time configuration
            obj["properties"]["configuration"] = time;
            //set IdeaID
            obj["id"] = dataObj["ideaID"].ToString();
            //set userId
            obj["userId"] = User.USER_ID;
            //set mainDesignId
            obj["mainDesignId"] = dataObj["designID"].ToString();
            //set name
            obj["translations"][0]["name"] = dataObj["title"].ToString();
            //set description
            obj["translations"][0]["description"] = dataObj["description"].ToString();
            //set tags
            var tags = dataObj["tags"].ToString().Split(',');
            JArray itemTag = (JArray)obj["translations"][0]["tags"];
            foreach (string str in tags)
            {
                if (!string.IsNullOrEmpty(str))
                    itemTag.Add(str);
            }
            //set dateCreated
            obj["dateCreated"] = string.Format("{0:yyyy-MM-dd'T'hh:mm:ss.fff'Z'}", DateTime.UtcNow);
            //set Amount
            obj["dateModified"] = time;

            //set href
            obj["resources"][0]["href"] = "https://image.spreadshirtmedia.com/image-server/v1/products/" + "1522508808" + "/views/1";
            //set href
            obj["resources"][1]["href"] = "https://image.spreadshirtmedia.com/image-server/v1/designs/" + dataObj["designID"].ToString();

            //set apiKey
            obj["assortment"]["reqParams"]["apiKey"] = CoreConfig.API_KEY;
            //set Amount
            obj["assortment"]["reqParams"]["sig"] = dataObj["sig"].ToString();
            //set time
            obj["assortment"]["reqParams"]["time"] = time;
            //set id

            obj["assortment"]["parentResource"]["parentResource"]["id"] = User.USER_ID;
            //set href
            obj["assortment"]["parentResource"]["parentResource"]["href"] = User.USER_HREF;
            //set ideaID
            obj["assortment"]["parentResource"]["id"] = dataObj["ideaID"].ToString();
            //set time
            obj["assortment"]["parentResource"]["href"] = User.USER_HREF + "/ideas/" + dataObj["ideaID"].ToString();
            //set backgroundColor
            obj["backgroundColor"] = "#666666";

            var data2Send = obj.ToString(Newtonsoft.Json.Formatting.None);
            return data2Send;
        }
        private List<Dictionary<string, object>> getPublishingDetails(ApplicationUser User,string shopName)
        {
            List<Dictionary<string, object>> lsData = new List<Dictionary<string, object>>();
            Dictionary<string, object> currDic = new Dictionary<string, object>();
            currDic.Add("id", "55c864cc64c7436b464aeb7b");
            currDic.Add("name", "");
            currDic.Add("targetID", "93439");
            currDic.Add("type", "MARKETPLACE");
            lsData.Add(currDic);
            var lsShop = shopName.Split(',');
            foreach (string strShop in lsShop)
            {
                if (!string.IsNullOrEmpty(strShop))
                {
                    currDic = new Dictionary<string, object>();
                    foreach (OShop shop in User.SHOPS)
                    {
                        if (shop.Name.Contains(strShop) || shop.TargetID.Contains(strShop))
                        {
                            currDic.Add("id", shop.Id);
                            currDic.Add("name", shop.Name);
                            currDic.Add("targetID", shop.TargetID);
                            currDic.Add("type", "SHOP");
                            lsData.Add(currDic);
                        }
                    }
                }
            }

            return lsData;
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