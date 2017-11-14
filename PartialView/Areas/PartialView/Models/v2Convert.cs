using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace PartialView.Areas.PartialView.Models
{
    public class v2Convert : Controller
    {
        public static string SQLFormat = "yyyy-MM-dd HH:mm:ss";
        private static string key = "insidenew";

        #region For Export Excel
        public DataTable ConvertJsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            jsonString = HttpUtility.HtmlDecode(jsonString);
            JsonSerializerSettings nts = new JsonSerializerSettings();
            nts.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            nts.DateTimeZoneHandling = DateTimeZoneHandling.Local;

            dt = JsonConvert.DeserializeObject<DataTable>(jsonString, nts);

            return dt;
        }

        public string RemoveUnicode(string text)
        {
            for (int i = 33; i < 48; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }

            for (int i = 58; i < 65; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }

            for (int i = 91; i < 97; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }
            for (int i = 123; i < 127; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }

            text = text.Replace(" ", "_");
            text = text.Replace("__", "_");
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string strFormD = text.Normalize(System.Text.NormalizationForm.FormD).ToLower();

            return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        #endregion

        #region Convert From JsonString
        public string ConvertObjectToJSon<T>(T obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj);

            return jsonString;
        }

        public T ConvertJSonToObject<T>(string jsonString)
        {
            jsonString = HttpUtility.HtmlDecode(jsonString);
            JsonSerializerSettings nts = new JsonSerializerSettings();
            nts.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            nts.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            T obj = JsonConvert.DeserializeObject<T>(jsonString, nts);

            return obj;
        }
        #endregion

        #region JsonResult
        public List<T> ConvertDataTableToList<T>(DataTable dt, params object[] prObjs)
        {
            #region First Config
            List<T> list = new List<T>();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataColumnCollection columns = dt.Columns;

            Int32 dfInt = 0;
            Double dfDouble = -9999;
            DateTime dfDateTime = DateTime.MinValue;
            Decimal dDecimal = 0;
            if (prObjs.Length > 0)
            {
                try
                {
                    Int32.TryParse(prObjs[0] + "", out dfInt);
                    Double.TryParse(prObjs[1] + "", out dfDouble);
                    DateTime.TryParse(prObjs[2] + "", out dfDateTime);
                    Decimal.TryParse(prObjs[3] + "", out dDecimal);
                }
                catch (Exception)
                {
                }
            }
            #endregion

            #region Loop
            foreach (DataRow dr in dt.Rows)
            {
                T newT = Activator.CreateInstance<T>();
                foreach (PropertyDescriptor prop in props)
                {
                    string pName = prop.Name;
                    string pType = prop.PropertyType.FullName;

                    if (columns.Contains(pName))
                    {
                        var value = dr[pName];
                        if (value == DBNull.Value)
                        {
                            if (pType.Contains("Int32"))
                            {
                                value = dfInt;
                            }
                            if (pType.Contains("Double"))
                            {
                                value = dfDouble;
                            }
                            if (pType.Contains("DateTime"))
                            {
                                value = dfDateTime;
                            }
                            if (pType.Contains("Decimal"))
                            {
                                value = dDecimal;
                            }
                            if (pType.Contains("String"))
                            {
                                value = "";
                            }
                        }
                        prop.SetValue(newT, value);
                    }
                }
                list.Add(newT);
            }
            #endregion

            return list;
        }

        public JsonResult ConvertListToJson<T>(List<T> list)
        {
            JsonResult jsonReturn = Json(list, JsonRequestBehavior.AllowGet);
            jsonReturn.MaxJsonLength = int.MaxValue;

            return jsonReturn;
        }

        public JsonResult ConvertDataTableToJson<T>(DataTable dt, params object[] prObjs)
        {
            List<T> list = ConvertDataTableToList<T>(dt, prObjs);
            JsonResult jsonReturn = ConvertListToJson<T>(list);

            return jsonReturn;
        }

        public List<Dictionary<string, object>> ConvertDataTableToListDictionary(DataTable dt)
        {
            #region First Config
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            #endregion

            #region Loop
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> dictRow = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    string DataType = col.DataType + "";
                    var value = dr[col];
                    if (value == DBNull.Value)
                    {
                        if (DataType.Contains("Int32") && col.ColumnName == "ParentID")
                        {
                            value = -1;
                        }
                        if (DataType.Contains("String"))
                        {
                            value = "";
                        }
                    }
                    dictRow.Add(col.ColumnName, value);
                }
                list.Add(dictRow);
            }
            #endregion

            return list;
        }
        #endregion

        #region EnCrypt and DeCrypt
        public string EncryptText(string input)
        {
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);
            string result = Convert.ToBase64String(bytesEncrypted);
            result = result.Replace("+", "-");

            return result;
        }

        private byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public string DecryptText(string input)
        {
            input = input.Replace("-", "+");
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);
            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }

        private byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
        #endregion

        public string GetClientIP(HttpRequestBase r)
        {
            string ip = r.ServerVariables["HTTP_X_FORWARDED_FOR"];
            try
            {
                if (string.IsNullOrEmpty(ip))
                {
                    ip = r.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    ip = ip.Split(',')[0];
                }
            }
            catch (Exception)
            {
                ip = r.ServerVariables["REMOTE_ADDR"];
            }

            return ip;

        }
    }
}