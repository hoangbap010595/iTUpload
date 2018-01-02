using Newtonsoft.Json;
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
using Upload.Teechip.Areas.Upload.Models;

namespace Upload.Teechip.Areas.Upload.Controllers
{
    public class TeechipController : Controller
    {
        private CookieContainer cookieApplication = new CookieContainer();
        private ApplicationUser User = new ApplicationUser();
        private string currToken = "";

        private string POSTER = "{\"designId\":\"{0}\",\"entityId\":\"{1}\",\"printSize\":\"poster-standard\",\"id\":\"{2}-36710\",\"sides\":{\"front\":{\"artworkId\":\"{3}\",\"position\":{\"vertical\":{\"origin\":\"C\",\"offset\":0},\"horizontal\":{\"origin\":\"C\",\"offset\":0}},\"size\":{\"width\":0.85,\"unit\":\"percentage\"}}},\"handling\":\"default\"}";
        private string CASE = "{\"designId\":\"{0}\",\"entityId\":\"{1}\",\"printSize\":\"case-standard\",\"id\":\"{2}-38916\",\"sides\":{\"front\":{\"artworkId\":\"{3}\",\"position\":{\"vertical\":{\"origin\":\"C\",\"offset\":-0.002962962962962945},\"horizontal\":{\"origin\":\"C\",\"offset\":-0.00666666666666671}},\"size\":{\"width\":0.46020776295794047,\"unit\":\"percentage\"}}},\"handling\":\"default\"}";
        private string GENERAL_SLIM = "{\"designId\":\"{0}\",\"entityId\":\"{1}\",\"printSize\":\"general-slim\",\"id\":\"{2}-7838\",\"sides\":{\"right\":{\"artworkId\":\"{3}\",\"position\":{\"vertical\":{\"origin\":\"T\",\"offset\":0},\"horizontal\":{\"origin\":\"C\",\"offset\":0}},\"size\":{\"width\":4,\"unit\":\"inch\"}},\"left\":{\"artworkId\":\"@ArtworkID\",\"position\":{\"vertical\":{\"origin\":\"T\",\"offset\":0},\"horizontal\":{\"origin\":\"C\",\"offset\":0}},\"size\":{\"width\":4,\"unit\":\"inch\"}}},\"handling\":\"default\"}";
        private string MUG = "{\"designId\":\"{0}\",\"entityId\":\"{1}\",\"printSize\":\"mug-standard\",\"id\":\"{2}-67858\",\"sides\":{\"front\":{\"artworkId\":\"{3}\",\"position\":{\"vertical\":{\"origin\":\"C\",\"offset\":0},\"horizontal\":{\"origin\":\"C\",\"offset\": 0}},\"size\":{\"width\":0.6855757689683196,\"unit\":\"percentage\"}}},\"handling\":\"default\"}";
        private string GENRAL = "{\"designId\":\"{0}\",\"entityId\":\"{1}\",\"printSize\":\"general-standard\",\"id\":\"{2}-75642\",\"sides\":{\"front\":{\"artworkId\":\"{3}\",\"position\":{\"vertical\":{\"origin\":\"T\",\"offset\":2},\"horizontal\":{\"origin\":\"C\",\"offset\":0}},\"size\":{\"width\":14,\"unit\":\"inch\"}}},\"handling\":\"default\"}";
        private string HAT = "{\"designId\":\"{0}\",\"entityId\":\"{1}\",\"printSize\":\"hat-standard\",\"id\":\"{2}-21944\",\"sides\":{\"front\":{\"artworkId\":\"{3}\",\"position\":{\"vertical\":{\"origin\":\"T\",\"offset\":0},\"horizontal\":{\"origin\":\"C\",\"offset\":0}},\"size\":{\"width\":2.8083610329838917,\"unit\":\"inch\"}}},\"handling\":\"default\"}";
        private string GENERAL_REDUCED = "{\"designId\":\"{0}\",\"entityId\":\"{1}}\",\"id\":\"{2}-94026\",\"printSize\":\"general-reduced\",\"sides\":{\"front\":{\"artworkId\":\"{3}\",\"position\":{\"vertical\":{\"origin\":\"T\",\"offset\":0},\"horizontal\":{\"origin\":\"C\",\"offset\":0}},\"size\":{\"width\":4,\"unit\":\"inch\"}}},\"handling\":\"default\"}";


        // GET: Upload/Teechip
        public ActionResult Teechip()
        {
            if (Session["USER_TEECHIP"] != null)
            {
                ApplicationUser User = (ApplicationUser)Session["USER_TEECHIP"];
                ViewBag.Email = User.Email;
                ViewData["Product"] = User.Products;
                ViewBag.Login = true;
            }
            else
            {
                ViewBag.Login = false;
            }

            return View();
        }

        [HttpGet]
        public JsonResult GetCategory()
        {
            List<string> lsCate = new List<string>();
            var file = Server.MapPath("~/App_Data");
            var fileName = Path.Combine(file, "teechip_category.txt");
            string rs = CoreConfig.readDataFromFile(fileName);
            JArray jArray = JArray.Parse(rs);
            foreach (var item in jArray)
            {
                lsCate.Add(item["name"].ToString());
            }
            var json = Json(lsCate, JsonRequestBehavior.AllowGet);
            return json;
        }

        [HttpPost]
        public JsonResult ExecuteLogin(string username, string password)
        {
            Dictionary<string, object> lsData = new Dictionary<string, object>();
            var json = new JsonResult();
            try
            {
                var urlLogin = "https://pro.teechip.com/manager/auth/login";
                var data2Send = "{\"email\":\"" + username + "\",\"password\":\"" + password + "\"}";
                //Step 1
                HttpWebRequest wRequest = (HttpWebRequest)WebRequest.Create(urlLogin);
                wRequest.Host = "pro.teechip.com";
                wRequest.CookieContainer = new CookieContainer();
                Dictionary<string, object> stepLogin = GetDataAPI(wRequest);
                cookieApplication = (CookieContainer)stepLogin["cookies"];
                //Step 2
                HttpWebRequest wRequestLogin = (HttpWebRequest)WebRequest.Create(urlLogin);
                wRequestLogin.Referer = "https://pro.teechip.com/manager/auth/login";
                wRequestLogin.ContentType = "application/json";
                wRequestLogin.Host = "pro.teechip.com";
                wRequestLogin.CookieContainer = cookieApplication;
                wRequestLogin.Headers.Add("x-xsrf-token", currToken);
                Dictionary<string, object> step2Login = PostDataAPI(wRequestLogin, data2Send);
                cookieApplication = (CookieContainer)step2Login["cookies"];
                var rs = step2Login["data"].ToString();
                var status = step2Login["status"].ToString();
                if (int.Parse(status) == -1)
                {
                    lsData.Add("message", "Tài khoản mật khẩu không đúng");
                    lsData.Add("icon", "error");
                    lsData.Add("result", -1);
                    json = Json(lsData, JsonRequestBehavior.AllowGet);
                    return json;
                }
                User = new ApplicationUser();
                var obj = JObject.Parse(rs);
                User.UserID = obj["_id"].ToString();
                User.Email = obj["email"].ToString();
                //User.Code = obj["referralCode"].ToString();
                User.ApiKey = obj["apiKey"].ToString();
                //User.ViewOnlyApiKey = obj["viewOnlyApiKey"].ToString();
                User.GroupID = obj["groupId"].ToString();
                User.EntityID = obj["entities"][0]["entityId"].ToString();
                //User.PayableId = obj["payable"]["payableId"].ToString();
                User.Authorization = "Basic " + ApplicationLibary.Base64Encode(":" + User.ApiKey);
                User.UnAuthorization = "Basic " + ApplicationLibary.Base64Encode("undefined:" + User.ApiKey);


                var data2SendBulkCode = "[\"TC0\",\"TC1\",\"TC6\",\"TC1001\",\"TC5\",\"TC2\",\"TC15\",\"TC1000\",\"TC4\",\"TC12\",\"TC7\",\"TC10\",\"TC13\",\"TC17\",\"TC2000\",\"TC2001\",\"TC2002\",\"TC30\",\"TC3001\",\"TC1002\",\"TC25\",\"TC3002\",\"TC4001\",\"TC4002\"]";
                var urlBulkCode = "https://api.scalablelicensing.com/rest/products/bulkByCode";

                HttpWebRequest wRequestBulkCode = (HttpWebRequest)WebRequest.Create(urlBulkCode);
                wRequestBulkCode.Referer = "https://pro.teechip.com/manager/auth/login";
                wRequestBulkCode.ContentType = "application/json";
                wRequestBulkCode.Host = "api.scalablelicensing.com";
                wRequestBulkCode.Headers.Add("x-xsrf-token", currToken);
                wRequestBulkCode.Referer = "https://pro.teechip.com/manager/campaigns/create/product?picker=true";
                Dictionary<string, object> dataBulkCode = PostDataAPI(wRequestBulkCode, data2SendBulkCode);
                JArray objProduct = JArray.Parse(dataBulkCode["data"].ToString());

                User.Products = new List<OProduct>();
                for (int i = 0; i < objProduct.Count; i++)
                {
                    OProduct p = new OProduct();
                    p.Id = objProduct[i]["_id"].ToString();
                    p.PrintSize = objProduct[i]["printSize"].ToString();
                    p.Type = objProduct[i]["type"].ToString();
                    p.Category = objProduct[i]["category"].ToString();
                    p.Code = objProduct[i]["code"].ToString();
                    p.Name = objProduct[i]["name"].ToString();
                    p.Msrp = int.Parse(objProduct[i]["msrp"].ToString());
                    p.Colors = new List<OColor>();
                    var listColor = objProduct[i]["colors"].ToString();
                    JArray arrColor = JArray.Parse(listColor);
                    foreach (var item in arrColor)
                    {
                        OColor c = new OColor();
                        c.Name = item["name"].ToString();
                        c.Hex = item["hex"].ToString();
                        c.Image = item["image"].ToString();
                        p.Colors.Add(c);
                    }
                    User.Products.Add(p);
                }

                //Set current Users
                Session["USER_TEECHIP"] = User;

                lsData.Add("email", User.Email);
                lsData.Add("product", User.Products);
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
        //1.Upload From FormData
        [HttpPost]
        public JsonResult UploadProgress(ExecuteUpload dataUploadTC)
        {
            Dictionary<string, object> dResult = new Dictionary<string, object>();
            //Set current Users
            var json = new JsonResult();
            try
            {
                var folder = "AdminUpload";
                if (Session["UserName"] != null)
                    folder = Session["UserName"].ToString();
                if (Session["USER_TEECHIP"] != null)
                {
                    ApplicationUser User = (ApplicationUser)Session["USER_TEECHIP"];

                    //Tiêu đề dùng chung
                    string uImage = Path.Combine(Server.MapPath("~/Uploaded/" + folder), @dataUploadTC.Image); // File image
                    string text_All = Path.GetFileName(uImage).Split('.')[0].ToString();
                    //cấu hình dữ liệu
                    var uTitle = dataUploadTC.Title.Replace("$name", text_All); //txtTitle.Text
                    var uDescription = @"<div>" + dataUploadTC.Description.Replace("$name", text_All) + "</div>"; //memoDescription.Text.Trim()
                                                                                                                  //var uCategory = ApplicationLibary.convertStringToJson(memoCategory.Text);
                    var uCategory = ApplicationLibary.convertStringToJson(getStringCategory(dataUploadTC.Category));
                    var uUrl = dataUploadTC.Url.ToLower();
                    if (string.IsNullOrEmpty(uUrl) || uUrl == "$url")
                        if (uUrl.Length <= 16)
                            uUrl = text_All.Replace(" ", "").Trim();
                        else
                            uUrl = text_All.Replace(" ", "").Trim().Substring(0, 16);
                    else
                        uUrl = uUrl.Replace(" ", "").Trim().Replace("$url", text_All);
                    uUrl += DateTime.Now.ToString("mmssfff");

                    var urlUploadImage = "https://scalable-licensing.s3.amazonaws.com/";
                    if (!System.IO.File.Exists(uImage))
                    {
                        dResult.Add("data", "File not found: " + Path.GetFileName(uImage));
                        json = Json(dResult, JsonRequestBehavior.AllowGet);
                        return json;
                    }

                    var imgDessign = Path.GetFileName(uImage);
                    #region ============== Upload Image & Get AtWork==================
                    string fileUpload = "uploads/" + DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.ToString("MM") + "/" + DateTime.Now.ToString("dd") + "/" + DateTime.Now.Ticks.ToString("x") + ".png";
                    NameValueCollection nvc = new NameValueCollection();
                    nvc.Add("key", fileUpload);
                    nvc.Add("bucket", "scalable-licensing");
                    nvc.Add("AWSAccessKeyId", "AKIAJE4QLGLTY4DH4WRA");
                    nvc.Add("Policy", "eyJleHBpcmF0aW9uIjoiMzAwMC0wMS0wMVQwMDowMDowMFoiLCJjb25kaXRpb25zIjpbeyJidWNrZXQiOiJzY2FsYWJsZS1saWNlbnNpbmcifSxbInN0YXJ0cy13aXRoIiwiJGtleSIsInVwbG9hZHMvIl0seyJhY2wiOiJwdWJsaWMtcmVhZCJ9XX0=");
                    nvc.Add("Signature", "4yVrFVzCgzWg2BH8RkrI6LVi11Y=");
                    nvc.Add("acl", "public-read");
                    Dictionary<string, object> data = HttpUploadFile(urlUploadImage, uImage, "file", "image/png", nvc);
                    if (int.Parse(data["status"].ToString()) == -1)
                    {
                        dResult.Add("data", "Step 0: Upload Image - " + data["data"].ToString());
                        json = Json(dResult, JsonRequestBehavior.AllowGet);
                        return json;
                    }
                    var urlImage = HttpUtility.UrlDecode(data["data"].ToString());

                    var data2Send = "{\"artwork\":\"" + urlImage + "\",\"AB\":{\"ab-use-dpi\":false}}";
                    HttpWebRequest wAtWork = (HttpWebRequest)WebRequest.Create("https://api.scalablelicensing.com/rest/artworks");
                    wAtWork.Host = "api.scalablelicensing.com";
                    wAtWork.Accept = "application/json, text/plain, */*";
                    wAtWork.ContentType = "application/json";

                    Dictionary<string, object> dataAtwork = PostDataAPI(wAtWork, data2Send);
                    var rs = dataAtwork["data"].ToString();
                    var obj = JObject.Parse(rs);
                    var atworkID = obj["artworkId"].ToString();
                    #endregion

                    #region ===============Step 1: Create Design & Get ID Design=============
                    var data2SendUpload = "{\"name\":\"" + uTitle + "\",\"entityId\":\"" + User.EntityID + "\",\"tags\":{\"style\":[" + uCategory + "]}}";

                    HttpWebRequest wCost = (HttpWebRequest)WebRequest.Create("https://api.scalablelicensing.com/rest/designs");
                    wCost.Accept = "application/json, text/plain, */*";
                    wCost.ContentType = "application/json";
                    wCost.PreAuthenticate = true;
                    wCost.Headers.Add("Authorization", User.Authorization);

                    Dictionary<string, object> dataUpload = PostDataAPI(wCost, data2SendUpload);
                    var rsUpload = dataUpload["data"].ToString();
                    var statusUpload = int.Parse(dataUpload["status"].ToString());
                    if (statusUpload == -1)
                    {
                        dResult.Add("data", "Step 1: " + rsUpload);
                        json = Json(dResult, JsonRequestBehavior.AllowGet);
                        return json;
                    }
                    var objUpload = JObject.Parse(rsUpload);
                    var _IDDesign = objUpload["_id"].ToString();
                    #endregion
                    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    var data2SendLineIDPOSTER = @POSTER.Replace("{0}", _IDDesign).Replace("{1}", User.EntityID).Replace("{2}", unixTimestamp.ToString()).Replace("{3}", atworkID);
                    var data2SendLineIDCASE = @CASE.Replace("{0}", _IDDesign).Replace("{1}", User.EntityID).Replace("{2}", unixTimestamp.ToString()).Replace("{3}", atworkID);
                    var data2SendLineIDGENERAL_SLIM = @GENERAL_SLIM.Replace("{0}", _IDDesign).Replace("{1}", User.EntityID).Replace("{2}", unixTimestamp.ToString()).Replace("{3}", atworkID);
                    var data2SendLineIDHAT = @HAT.Replace("{0}", _IDDesign).Replace("{1}", User.EntityID).Replace("{2}", unixTimestamp.ToString()).Replace("{3}", atworkID);
                    var data2SendLineIDGENRAL = @GENRAL.Replace("{0}", _IDDesign).Replace("{1}", User.EntityID).Replace("{2}", unixTimestamp.ToString()).Replace("{3}", atworkID);
                    var data2SendLineIDMUG = @MUG.Replace("{0}", _IDDesign).Replace("{1}", User.EntityID).Replace("{2}", unixTimestamp.ToString()).Replace("{3}", atworkID);
                    var data2SendLineIDREDUCED = GENERAL_REDUCED.Replace("{0}", _IDDesign).Replace("{1}", User.EntityID).Replace("{2}", unixTimestamp.ToString()).Replace("{3}", atworkID);

                    #region ===============Step 2: Create Design Line & Get DesignLine ID===============
                    Dictionary<string, object> lineID = new Dictionary<string, object>();
                    foreach (string item in dataUploadTC.LineID)
                    {
                        switch (item)
                        {
                            case "mug-standard":
                                lineID.Add("LineIDMUG", getDesignLineID(User, data2SendLineIDMUG));
                                break;
                            case "poster-standard":
                                lineID.Add("LineIDPOSTER", getDesignLineID(User, data2SendLineIDPOSTER));
                                break;
                            case "case-standard":
                                lineID.Add("LineIDCASE", getDesignLineID(User, data2SendLineIDCASE));
                                break;
                            case "general-slim":
                                lineID.Add("LineIDGENERAL_SLIM", getDesignLineID(User, data2SendLineIDGENERAL_SLIM));
                                break;
                            case "hat-standard":
                                lineID.Add("LineIDHAT", getDesignLineID(User, data2SendLineIDHAT));
                                break;
                            case "general-reduced":
                                lineID.Add("LineIDREDUCED", getDesignLineID(User, data2SendLineIDREDUCED));
                                break;
                            default:
                                lineID.Add("LineIDGENRAL", getDesignLineID(User, data2SendLineIDGENRAL));
                                break;
                        }
                    }
                    #endregion

                    //Step 3 -- Tham số cần truyền: 
                    //      1. productId, color, price: người dùng chọn
                    var objIDReail = getAllRetailIDFromDesignID(User, dataUploadTC.RetailID, lineID);

                    //Step 4 -- Nhận giá trị 1 mảng _IDDesignRetail từ Step 3
                    var data2SendCampaigns = "{\"url\":\"" + uUrl + "\",\"title\":\"" + uTitle + "\",\"description\":\"" + uDescription + "\",\"duration\":24,\"policies\":{\"forever\":true,\"fulfillment\":24,\"private\":false,\"checkout\":\"direct\"},\"social\":{\"trackingTags\":{}},\"entityId\":\"" + User.EntityID + "\",\"upsells\":[],\"tags\":{\"style\":[" + uCategory + "]},\"related\": " + objIDReail + "}";

                    HttpWebRequest wCampaigns = (HttpWebRequest)WebRequest.Create("https://api.scalablelicensing.com/rest/campaigns");
                    wCampaigns.Accept = "application/json, text/plain, */*";
                    wCampaigns.ContentType = "application/json";
                    wCampaigns.PreAuthenticate = true;
                    wCampaigns.Headers.Add("Authorization", User.Authorization);

                    Dictionary<string, object> dataUploadCampaigns = PostDataAPI(wCampaigns, data2SendCampaigns);
                    var rsUploadCampaigns = dataUploadCampaigns["data"].ToString();
                    var statusCampaigns = int.Parse(dataUploadCampaigns["status"].ToString());
                    if (statusCampaigns == -1)
                    {
                        dResult.Add("data", "Step 4: " + rsUploadCampaigns);
                        json = Json(dResult, JsonRequestBehavior.AllowGet);
                        return json;
                    }
                    var objUploadCampaigns = JObject.Parse(rsUploadCampaigns);
                    var titleCampaigns = objUploadCampaigns["title"].ToString();
                    var urlCampaigns = "https://pro.teechip.com/" + objUploadCampaigns["url"].ToString();

                    DeleteFileUploaded(uImage);
                    dResult.Add("data", "Upload finish: " + titleCampaigns + ", Link:" + urlCampaigns);
                    json = Json(dResult, JsonRequestBehavior.AllowGet);
                    return json;
                }
                else
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

        //Step 2: Get create Design Line & Get DesignLine ID
        private string getDesignLineID(ApplicationUser user, string data2Send)
        {
            HttpWebRequest wLines = (HttpWebRequest)WebRequest.Create("https://api.scalablelicensing.com/rest/design-lines");
            wLines.Accept = "application/json, text/plain, */*";
            wLines.ContentType = "application/json";
            wLines.PreAuthenticate = true;
            wLines.Headers.Add("Authorization", user.Authorization);

            Dictionary<string, object> dataUploadLines = PostDataAPI(wLines, data2Send);
            var rsUploadLines = dataUploadLines["data"].ToString();
            var statusLines = int.Parse(dataUploadLines["status"].ToString());
            if (statusLines == -1)
            {
                return "";
            }
            var objUploadLines = JObject.Parse(rsUploadLines);
            return objUploadLines["_id"].ToString();
        }
        //Step 3: Get Retail ID
        private string getAllRetailIDFromDesignID(ApplicationUser user, List<string> themes, Dictionary<string, object> dataDesignID)
        {
            List<Dictionary<string, object>> lsData = new List<Dictionary<string, object>>();
            Dictionary<string, object> data;
            List<string> lsCommand = new List<string>();

            foreach (string item in themes)
            {
                var designID = "";
                var objThemes = JObject.Parse(item);
                var t_printSize = objThemes["designLineId"].ToString();
                switch (t_printSize)
                {
                    case "@mug-standard":
                        designID = dataDesignID["LineIDMUG"].ToString();
                        var x1 = item.Replace(t_printSize, designID);
                        lsCommand.Add(x1);
                        break;
                    case "@poster-standard":
                        designID = dataDesignID["LineIDPOSTER"].ToString();
                        var x2 = item.Replace(t_printSize, designID);
                        lsCommand.Add(x2);
                        break;
                    case "@case-standard":
                        designID = dataDesignID["LineIDCASE"].ToString();
                        var x3 = item.Replace(t_printSize, designID);
                        lsCommand.Add(x3);
                        break;
                    case "@general-slim":
                        designID = dataDesignID["LineIDGENERAL_SLIM"].ToString();
                        var x4 = item.Replace(t_printSize, designID);
                        lsCommand.Add(x4);
                        break;
                    case "@hat-standard":
                        designID = dataDesignID["LineIDHAT"].ToString();
                        var x5 = item.Replace(t_printSize, designID);
                        lsCommand.Add(x5);
                        break;
                    case "@general-reduced":
                        designID = dataDesignID["LineIDREDUCED"].ToString();
                        var x6 = item.Replace(t_printSize, designID);
                        lsCommand.Add(x6);
                        break;
                    default:
                        designID = dataDesignID["LineIDGENRAL"].ToString();
                        var x7 = item.Replace(t_printSize, designID);
                        lsCommand.Add(x7);
                        break;
                }
            }
            foreach (var item2 in lsCommand)
            {
                try
                {
                    data = new Dictionary<string, object>();
                    HttpWebRequest wRetail = (HttpWebRequest)WebRequest.Create("https://api.scalablelicensing.com/rest/retail-products");
                    wRetail.Accept = "application/json, text/plain, */*";
                    wRetail.ContentType = "application/json";
                    wRetail.PreAuthenticate = true;
                    wRetail.Headers.Add("Authorization", user.Authorization);

                    Dictionary<string, object> dataUploadRetail = PostDataAPI(wRetail, item2);
                    var rsUploadRetail = dataUploadRetail["data"].ToString();
                    var statusRetail = int.Parse(dataUploadRetail["status"].ToString());
                    if (statusRetail == -1)
                    {
                        //ApplicationLibary.writeLogThread(lsBoxLog, "Step 3: " + rsUploadRetail, 2);
                        continue;
                    }
                    else
                    {
                        var objUploadRetail = JObject.Parse(rsUploadRetail);
                        data.Add("id", objUploadRetail["_id"].ToString());
                        data.Add("price", int.Parse(objUploadRetail["price"].ToString()));
                        if (lsData.Count == 0)
                            data.Add("default", true);
                        lsData.Add(data);
                    }
                }
                catch (Exception ex)
                {
                    //ApplicationLibary.writeLogThread(lsBoxLog, ex.Message, 2);
                    continue;
                }
            }
            var jsData = JsonConvert.SerializeObject(lsData);
            return jsData;
        }
        private string getStringCategory(string text)
        {
            string kq = "";
            var file = Server.MapPath("~/App_Data");
            var fileName = Path.Combine(file, "teechip_category.txt");
            string rs = CoreConfig.readDataFromFile(fileName);
            JArray jArray = JArray.Parse(rs);
            foreach (var itemText in text.Split(','))
            {
                foreach (var item in jArray)
                {
                    if (item["name"].ToString().ToLower().IndexOf(itemText.ToLower()) > -1 || item["fullName"].ToString().ToLower().IndexOf(itemText.ToLower()) > -1
                        || item["slug"].ToString().ToLower().IndexOf(itemText.ToLower()) > -1)
                    {
                        kq += item["fullName"].ToString();
                    }
                    JArray jArray2 = JArray.Parse(item["children"].ToString());
                    if (jArray2.Count > 0)
                    {
                        foreach (var item2 in jArray2)
                        {
                            if (item2["slug"].ToString().ToLower().IndexOf(itemText.ToLower()) > -1)
                            {
                                foreach (var item3 in jArray)
                                {
                                    if (item2["tagId"].ToString().ToLower().Equals(item3["_id"].ToString().ToLower()))
                                    {
                                        kq += item3["fullName"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var currKQ = kq.Replace("(", "|").Replace(")", "|");
            var x = currKQ.Split('|');
            string ressult = "";
            int i = 0;
            foreach (string item in x)
            {
                if (i >= 200)
                    break;
                var crrText = item.Trim();
                if (crrText != "" && crrText != " " && ressult.IndexOf(crrText) == -1)
                    ressult += crrText.Trim() + ",";
                i++;
            }
            return ressult;
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
        #region ========== Post & Get Data ==========
        public static Dictionary<string, object> HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            HttpWebRequest wr = null;
            WebResponse wresp = null;
            try
            {
                Console.WriteLine(string.Format("Uploading {0} to {1}", file, url));
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                wr = (HttpWebRequest)WebRequest.Create(url);
                wr.Host = "scalable-licensing.s3.amazonaws.com";
                wr.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:49.0) Gecko/20100101 Firefox/49.0";
                wr.Accept = "application/json";
                wr.ContentType = "multipart/form-data; boundary=" + boundary;
                wr.Method = "POST";
                wr.Headers.Add("Origin", "https://pro.teechip.com");
                wr.Headers.Add("Accept-Language", "vi-VN,vi;q=0.8,en-US;q=0.5,en;q=0.3");
                wr.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                wr.KeepAlive = true;
                wr.ServicePoint.Expect100Continue = false;
                wr.ProtocolVersion = HttpVersion.Version11;
                wr.Timeout = 90000;
                wr.ReadWriteTimeout = 90000;

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
                string header = string.Format(headerTemplate, paramName, file, contentType);
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

                data.Add("data", wresp.Headers["Location"]);
                data.Add("status", 1);
                wresp.Close();
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
        private Dictionary<string, object> PostDataAPI(HttpWebRequest wRequest, string data2Send)
        {
            Dictionary<string, object> dataReturn = new Dictionary<string, object>();
            CookieContainer cookies = new CookieContainer();
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] postDataBytes = encoding.GetBytes(data2Send);

                wRequest.Method = "POST";
                wRequest.UserAgent = ApplicationLibary.BROWSER_FIREFOX;
                wRequest.ContentLength = postDataBytes.Length;
                wRequest.Headers.Add("Origin", CoreConfig.Origin);
                wRequest.ServicePoint.Expect100Continue = false;
                wRequest.ProtocolVersion = HttpVersion.Version11;
                wRequest.Timeout = 90000;
                wRequest.ReadWriteTimeout = 90000;
                wRequest.KeepAlive = true;

                using (Stream sr = wRequest.GetRequestStream())
                {
                    sr.Write(postDataBytes, 0, postDataBytes.Length);
                }

                HttpWebResponse wResponse = (HttpWebResponse)wRequest.GetResponse();
                foreach (Cookie cookie in wResponse.Cookies)
                {
                    if (cookie.Name.Contains("x-xsrf-token") || cookie.Name.Contains("XSRF-TOKEN"))
                        currToken = cookie.Value;
                    cookies.Add(cookie);
                } 

                String htmlString;
                using (var reader = new StreamReader(wResponse.GetResponseStream()))
                {
                    htmlString = reader.ReadToEnd();
                }
                wResponse.Close();
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
            if (data2Send != "")
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] postDataBytes = encoding.GetBytes(data2Send);
                wRequest.ContentLength = postDataBytes.Length;
                wRequest.Headers.Add("Origin", CoreConfig.Origin);
                wRequest.ServicePoint.Expect100Continue = false;
                wRequest.ProtocolVersion = HttpVersion.Version11;
                wRequest.Timeout = 90000;
                wRequest.ReadWriteTimeout = 90000;
                wRequest.KeepAlive = true;

                using (Stream sr = wRequest.GetRequestStream())
                {
                    sr.Write(postDataBytes, 0, postDataBytes.Length);
                }
            }

            HttpWebResponse wResponse = (HttpWebResponse)wRequest.GetResponse();
            CookieContainer cookies = new CookieContainer();
            foreach (Cookie cookie in wResponse.Cookies)
            {
                if (cookie.Name.Contains("x-xsrf-token") || cookie.Name.Contains("XSRF-TOKEN"))
                    currToken = cookie.Value;
                cookies.Add(cookie);
            }

            String htmlString;
            using (var reader = new StreamReader(wResponse.GetResponseStream()))
            {
                htmlString = reader.ReadToEnd();
            }

            wResponse.Close();
            Dictionary<string, object> dataReturn = new Dictionary<string, object>();
            dataReturn.Add("cookies", cookies);
            dataReturn.Add("data", htmlString);

            return dataReturn;
        }
        #endregion
    }
}