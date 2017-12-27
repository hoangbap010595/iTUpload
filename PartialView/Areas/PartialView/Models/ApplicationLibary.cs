using ExcelDataReader;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PartialView.Areas.PartialView.Models
{
    public class ApplicationLibary
    {
        public static string BROWSER_FIREFOX = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36";
        public static string ConvertImageToBase64(string path)
        {
            string base64String = string.Empty;
            if (!File.Exists(path))
                return base64String;
            // Convert Image to Base64
            using (Image img = Image.FromFile(path)) // Image Path from File Upload Controller
            {
                using (var memStream = new MemoryStream())
                {
                    img.Save(memStream, img.RawFormat);
                    byte[] imageBytes = memStream.ToArray();
                    // Convert byte[] to Base64 String
                    base64String = Convert.ToBase64String(imageBytes);
                }
                img.Dispose();
            }
            return base64String;
        }
        public static DataTable getDataExcelFromFileToDataTable(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (data) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    //Get all Table
                    DataTableCollection tables = result.Tables;
                    DataTable dt = tables[0];
                    return dt;
                }

            }
        }

        private static DataTable getDataExcelFromFileCSVToDataTable(string csv_file_path)

        {

            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return csvData;

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch
            {
                throw new Exception("Something's wrong. Please try again !");
            }
        }
        public static string readDataFromFile(string filePath)
        {
            var data = ""; 
            using (StreamReader reader = new StreamReader(filePath))
            {
                data = reader.ReadToEnd();
            }
            return data;
        }

        public static string convertStringToJson(string text)
        {
            var result = "";
            var value = text.Split(',');
            foreach (string item in value)
            {
                if (item != "")
                    result += "\"" + item + "\",";
            }
            return result.TrimEnd(',');
        }

        /// <summary>
        /// take any string and encrypt it using SHA1 then
        /// return the encrypted data
        /// </summary>
        /// <param name="data">input text you will enterd to encrypt it</param>
        /// <returns>return the encrypted text as hexadecimal string</returns>
        public static string GetSHA1HashData(string data, string secret)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] keyByte = Encoding.UTF8.GetBytes(secret);
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public static string GetHMACSHA1HasData(string data, string secretAccessKey)
        {
            String rs = "";
            byte[] keyByte = Encoding.UTF8.GetBytes(secretAccessKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(data);
            using (var hmacsha256 = new HMACSHA1(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                for (int i = 0; i < hashmessage.Length; i++)
                {
                    byte bit = hashmessage[i];
                    rs += bit.ToString("x2");
                }
            }
            return rs;
        }
        public static string GetTimeStamp()
        {
            int unixTimestamp = (int)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return (unixTimestamp * 1000).ToString();
        }

    }
}