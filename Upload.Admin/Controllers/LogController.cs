using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Upload.Admin.Models;

namespace Upload.Admin.Controllers
{
    public class LogController : Controller
    {
        //public JsonResult 
        ApplicationContext context = new ApplicationContext();

        
        [HttpPost]
        public JsonResult InsertLog(LogConnected log)
        {
            Dictionary<string, object> lsData = new Dictionary<string, object>();
            var json = new JsonResult();
            try
            {
                context.LogConnecteds.Add(log);
                context.SaveChanges();
                lsData.Add("result", 1);
                lsData.Add("data", "Success!");
                json = Json(lsData, JsonRequestBehavior.AllowGet);
                return json;
            }
            catch (Exception ex){
                lsData.Add("result", -1);
                lsData.Add("data", "Error: " + ex.Message);
                json = Json(lsData, JsonRequestBehavior.AllowGet);
                return json;
            }

        }
    }
}
