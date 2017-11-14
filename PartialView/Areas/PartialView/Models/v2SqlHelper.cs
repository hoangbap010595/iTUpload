using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SQLRClient;

namespace PartialView.Areas.PartialView.Models
{
    public class v2SqlHelper
    {
        v2Convert v2c = new v2Convert();
        public static string Conn(string connectionString)
        {
            string strConn = "";

            if (ConfigurationManager.ConnectionStrings[connectionString] != null)
            {
                strConn = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
            }

            return strConn;
        }

        public static SqlConnection SqlConn(string connectionString)
        {
            SqlConnection sqlConn = new SqlConnection(Conn(connectionString));

            return sqlConn;
        }

        public DataSet ExecuteDataset(string spName, string connectionString, Dictionary<string, object> filter)
        {
            DataSet ds = new DataSet();
            ds = ExecuteDatasetDefault(spName, connectionString, filter);
            return ds;
        }

        public DataSet ExecuteDatasetDefault(string spName, string connectionString, Dictionary<string, object> filter)
        {
            DataSet ds = new DataSet();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = SqlConn(connectionString);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = spName;
            cmd.CommandTimeout = 60 * 3;
            if (filter != null)
            {
                foreach (KeyValuePair<string, object> item in filter)
                {
                    var value = item.Value;
                    if (value == null)
                    {
                        value = DBNull.Value;
                    }
                    SqlParameter p = new SqlParameter(item.Key, value);
                    cmd.Parameters.Add(p);
                }
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds;
        }

        public DataSet ExecuteDatasetRelay(string spName, string connectionString, Dictionary<string, object> filter)
        {
            DataSet ds = new DataSet();
            //ds = SqlHelper.ExecuteDataset(SqlHelper.getConnectionString(), "Partner.dbo.PARR_GetTest @Type=2,@Filter=''");

            SQLRelayCommand cmd = new SQLRelayCommand();
            SQLRelayConnection cnn = new SQLRelayConnection(Conn("sqlRelayString"));
            cmd.Connection = cnn;
            cmd.CommandText = "exec " + spName;
            if (filter != null)
            {
                string strPara = "";
                foreach (KeyValuePair<string, object> item in filter)
                {
                    string value = item.Value + "";
                    string aMoc = "@";
                    if (item.Key.Substring(0,1).Equals("@"))
                    {
                        aMoc = "";
                    }

                    strPara += aMoc + item.Key + "='" + value + "',";
                }
                cmd.CommandText += " " + strPara.Substring(0, strPara.Length - 1);
                cmd.CommandText = cmd.CommandText.Replace("'null'", "null");
            }
            cnn.Open();
            SQLRelayDataAdapter da = new SQLRelayDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);

            return ds;
        }

        public List<Dictionary<string, object>> ExecuteList(string spName, string connectionString, Dictionary<string, object> filter)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            DataSet ds = ExecuteDataset(spName, connectionString, filter);
            list = v2c.ConvertDataTableToListDictionary(ds.Tables[0]);

            return list;
        }

        public List<List<Dictionary<string, object>>> ExecuteMultiList(string spName, string connectionString, Dictionary<string, object> filter)
        {
            List<List<Dictionary<string, object>>> list = new List<List<Dictionary<string, object>>>();
            DataSet ds = ExecuteDataset(spName, connectionString, filter);
            foreach (DataTable dt in ds.Tables)
            {
                list.Add(v2c.ConvertDataTableToListDictionary(dt));
            }

            return list;
        }

        public string SendMail(string mailTo, string mailCC, string mailBcc, string mailSubject, string mailBody)
        {
            string strResult = "Gởi mail thành công!";
            Dictionary<string, object> filter = new Dictionary<string, object>();
            filter.Add("@profile_name", "iscsupport");
            filter.Add("@body_format", "HTML");
            filter.Add("@recipients", mailTo);
            filter.Add("@subject", mailSubject);
            filter.Add("@body", mailBody);
            if (!String.IsNullOrEmpty(mailCC))
            {
                filter.Add("@copy_recipients", mailCC);
            }
            if (!String.IsNullOrEmpty(mailBcc))
            {
                filter.Add("@blind_copy_recipients", mailBcc);
            }

            ExecuteDatasetDefault("msdb.dbo.sp_send_dbmail", "sqlConnectionRead", filter);

            return strResult;
        }
    }
}