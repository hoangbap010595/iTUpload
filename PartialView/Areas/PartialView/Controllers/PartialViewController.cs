using PartialView.Areas.PartialView.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PartialView.Areas.PartialView.Controllers
{
    public class PartialViewController : Controller
    {
        private v2SqlHelper v2Sql = new v2SqlHelper();
        private v2Convert v2Con = new v2Convert();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LoadData(DataConfig d)
        {
            try
            {
                DataSet ds = GetDataSet(d);
                int n = ds.Tables.Count;
                if (n > 1)
                {
                    List<List<Dictionary<string, object>>> list1 = new List<List<Dictionary<string, object>>>();
                    foreach (DataTable dt in ds.Tables)
                    {
                        List<Dictionary<string, object>> l = v2Con.ConvertDataTableToListDictionary(dt);
                        list1.Add(l);
                    }
                    return v2Con.ConvertListToJson(list1);
                }
                else
                {
                    List<Dictionary<string, object>> list2 = new List<Dictionary<string, object>>();
                    list2 = v2Con.ConvertDataTableToListDictionary(ds.Tables[0]);
                    return v2Con.ConvertListToJson(list2);
                }

            }
            catch (Exception err)
            {
                JsonResult jsonErr = Json(d.SPName + ": " + err.Message, JsonRequestBehavior.AllowGet);
                jsonErr.MaxJsonLength = int.MaxValue;
                return jsonErr;
            }
            
        }

        [HttpPost]
        public DataSet GetDataSet(DataConfig d)
        {
            DataSet ds = null;
            if (String.IsNullOrEmpty(d.JsonFilter))
            {
                d.JsonFilter = "";
            }
            Dictionary<string, object> filters = v2Con.ConvertJSonToObject<Dictionary<string, object>>(d.JsonFilter);
            if (filters != null)
            {
                SqlParameter[] para = new SqlParameter[filters.Keys.Count];
                int i = 0;
                foreach (string key in filters.Keys)
                {
                    para[i] = new SqlParameter("@"+ key, filters[key]);
                    i++;
                }
                ds = v1SqlHelper.ExecuteDataset(d.ConnectionString, CommandType.StoredProcedure, d.SPName, para);
            }
            else
            {
                ds = v1SqlHelper.ExecuteDataset(d.ConnectionString, CommandType.StoredProcedure, d.SPName);
            }
            return ds;
        }
    }
}