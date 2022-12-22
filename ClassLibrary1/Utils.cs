﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public static class Utils
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        private const int MAX_FILE_SIZE = 4194304; // 4 MBytes

        public static string ToCamelCase(this string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        public static string SplitCamelCase(this string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        public static string TrimStart(this string value, string toTrim)
        {
            if (value.StartsWith(toTrim))
            {
                int startIndex = toTrim.Length;
                return value.Substring(startIndex);
            }
            return value;
        }

        public static string TrimEnd(this string value, string toTrim)
        {
            if (value.EndsWith(toTrim))
            {
                int startIndex = toTrim.Length;
                return value.Substring(0, value.Length - startIndex);
            }
            return value;
        }

        public static string GetString(object val, bool forSql = false, string defaultValue = "")
        {
            string result = defaultValue;

            if (val != null)
            {
                result = forSql ? val.ToString().Replace("'", "''").Trim() : val.ToString().Trim();
            }

            return result;
        }

        public static string GetStringMaxChars(object val, int maxCharCount, bool forSql = false)
        {
            string value = GetString(val, false); // First get string

            value = value.Substring(0, Math.Min(value.Length, maxCharCount));// Then cut characters if needed

            // If for SQL, replace quotes
            return forSql ? GetString(value, true) : value;
        }

        public static int GetInt(object val, int defval = -1)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return defval;

            try
            {
                return Convert.ToInt32(val);
            }
            catch
            {
                return defval;
            }
        }


        public static int? GetNullInt(object val, int? defval = null)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return null;

            try
            {
                return (int?)Convert.ToInt32(val);
            }
            catch
            {
                return defval;
            }
        }

        public static Int64 GetLong(object val, long defval = -1)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return defval;

            try
            {
                return Convert.ToInt64(val);
            }
            catch
            {
                return -1;
            }
        }

        public static long? GetNullLong(object val, long? defval = null)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return null;

            try
            {
                return (long?)Convert.ToInt64(val);
            }
            catch
            {
                return defval;
            }
        }

        public static DateTime? GetDate(object val, string format)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return null;

            try
            {
                return DateTime.ParseExact(GetString(val), format, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? GetDate(object val)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return null;

            try
            {
                return DateTime.Parse(GetString(val));
            }
            catch
            {
                return null;
            }
        }

        public static DateTime GetDate(object val, DateTime defaultValue)
        {
            try
            {
                return DateTime.Parse(GetString(val));
            }
            catch
            {
                return defaultValue;
            }
        }

        public static float GetFloat(object val, int defval = -1)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return defval;

            try
            {
                return float.Parse(val.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
            }
            catch
            {
                return defval;
            }
        }

        public static float? GetNullFloat(object val, float? defval = null)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return null;

            try
            {
                return (float?)float.Parse(GetString(val).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
            }
            catch
            {
                return defval;
            }
        }

        public static double GetDouble(object val, int defval = -1)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return defval;

            try
            {
                return double.Parse(val.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
            }
            catch
            {
                return defval;
            }
        }

        public static double? GetNullDouble(object val, double? defval = null)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return null;

            try
            {
                return (double?)double.Parse(GetString(val).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
            }
            catch
            {
                return defval;
            }
        }

        public static decimal GetDecimal(object val, decimal defval = -1)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return defval;

            try
            {
                decimal value = decimal.Parse(val.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
                return value;
            }
            catch
            {
                return defval;
            }
        }

        public static decimal? GetNullDecimal(object val, decimal? defval = null)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return null;

            try
            {
                return decimal.Parse(GetString(val).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
            }
            catch
            {
                return defval;
            }
        }

        public static bool GetBool(object val, bool defaultValue = false)
        {
            bool result = defaultValue;
            try
            {
                if (GetString(val).Equals("1") || GetString(val).ToLower().Equals("true"))
                    result = true;
                else if (val == null || GetString(val).Equals("0") || GetString(val).ToLower().Equals("false"))
                    result = false;
                else
                    result = bool.Parse(val.ToString());
            }
            catch { }

            return result;
        }

        public static bool? GetNullBool(object val, bool? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(GetString(val)))
                return null;

            bool? result = defaultValue;
            try
            {
                if (GetString(val).Equals("1") || GetString(val).ToLower().Equals("true"))
                    result = (bool?)true;
                else if (GetString(val).Equals("0") || GetString(val).ToLower().Equals("false"))
                    result = (bool?)false;
                else
                    result = (bool?)bool.Parse(val.ToString());
            }
            catch { }

            return result;
        }

        //public static void Log(string msg, string fileName = null, bool fromWebAPI = true)
        //{
        //    try
        //    {
        //        string final_path = null;
        //        // Log is by default stored in server files under Logs folder.
        //        if (fromWebAPI)
        //        {
        //            final_path = HttpContext.Current.Server.MapPath("~\\Logs\\");
        //            final_path += fileName;
        //        }
        //        else
        //            final_path = Directory.GetCurrentDirectory() + "\\" + fileName;
        //        EnsureFileSize(final_path);
        //        StreamWriter sw = new StreamWriter(final_path, true);
        //        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + msg);
        //        sw.Close();
        //    }
        //    catch (Exception ex) { }
        //}

        //public static void LogQuery(string msg, string path = null)
        //{
        //    if (Globals.Debug)
        //    {
        //        try
        //        {
        //            EnsureFileSize(path != null ? path : Globals.LogPath);

        //            StreamWriter sw = new StreamWriter(path != null ? path : Globals.LogPath, true);
        //            sw.WriteLine(msg);
        //            sw.Close();
        //        }
        //        catch { }
        //    }
        //}

        private static void EnsureFileSize(string fname)
        {
            FileInfo finfo = null;
            try
            {
                finfo = new System.IO.FileInfo(fname);
                long len = finfo.Length;
                if (len < MAX_FILE_SIZE)
                    return;
                string ext = string.Format(".{0}", Path.GetExtension(fname));
                string newFName = fname.Replace(ext, string.Format("{0}{1}", DateTime.Now.ToString("_yy_MM_dd_HH_mm_ss_"), ext));
                File.Move(fname, newFName);
            }
            catch
            {

            }
        }

        //public static void LogError(Exception ex, string label = "")
        //{
        //    try
        //    {
        //        EnsureFileSize(Globals.LogErrorPath);
        //        StreamWriter sw = new StreamWriter(Globals.LogErrorPath, true);
        //        sw.WriteLine(string.Format("{1} : {2}{0}{3}{0}{4}", Environment.NewLine,
        //            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //            label, ex.Message, ex.StackTrace));
        //        sw.Close();

        //        if (ex.InnerException != null)
        //            LogError(ex.InnerException);
        //    }
        //    catch { }
        //}

        //public static void LogError(string msg)
        //{
        //    try
        //    {
        //        EnsureFileSize(Globals.LogErrorPath);
        //        StreamWriter sw = new StreamWriter(Globals.LogErrorPath, true);
        //        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + msg);
        //        sw.Close();
        //    }
        //    catch { }
        //}

        //public static void SetPropertyValue(PropertyInfo property, object DataItem, object Value)
        //{
        //    if (property.PropertyType.Equals(typeof(string)))
        //        property.SetValue(DataItem, Utils.GetString(Value));
        //    else if (property.PropertyType.Equals(typeof(int)))
        //        property.SetValue(DataItem, Utils.GetInt(Value));
        //    else if (property.PropertyType.Equals(typeof(int?)))
        //        property.SetValue(DataItem, Utils.GetNullInt(Value));
        //    else if (property.PropertyType.Equals(typeof(long)))
        //        property.SetValue(DataItem, Utils.GetLong(Value));
        //    else if (property.PropertyType.Equals(typeof(long?)))
        //        property.SetValue(DataItem, Utils.GetNullLong(Value));
        //    else if (property.PropertyType.Equals(typeof(double)))
        //        property.SetValue(DataItem, Utils.GetDouble(Value));
        //    else if (property.PropertyType.Equals(typeof(double?)))
        //        property.SetValue(DataItem, Utils.GetNullDouble(Value));
        //    else if (property.PropertyType.Equals(typeof(decimal)))
        //        property.SetValue(DataItem, Utils.GetDecimal(Value));
        //    else if (property.PropertyType.Equals(typeof(decimal?)))
        //        property.SetValue(DataItem, Utils.GetNullDecimal(Value));
        //    else if (property.PropertyType.Equals(typeof(float)))
        //        property.SetValue(DataItem, Utils.GetFloat(Value));
        //    else if (property.PropertyType.Equals(typeof(float?)))
        //        property.SetValue(DataItem, Utils.GetNullFloat(Value));
        //    else if (property.PropertyType.Equals(typeof(bool)))
        //        property.SetValue(DataItem, Utils.GetBool(Value));
        //    else if (property.PropertyType.Equals(typeof(bool?)))
        //        property.SetValue(DataItem, Utils.GetNullBool(Value));
        //    else if (property.PropertyType.Equals(typeof(DateTime)))
        //        property.SetValue(DataItem, Utils.GetDate(Value, DateTime.Now));
        //    else if (property.PropertyType.Equals(typeof(DateTime?)))
        //        property.SetValue(DataItem, Utils.GetDate(Value));
        //}

        public static long GetUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return (long)Math.Floor(diff.TotalSeconds);
        }

        public static bool StringsMatchApproximately(string string1, string string2)
        {
            // If any is null or whitespace
            if (string.IsNullOrWhiteSpace(string1) || string.IsNullOrWhiteSpace(string2))
                return false;

            // In case they are equal
            if (string1.Trim().ToLower().Equals(string2.Trim().ToLower()))
                return true;

            // Get rid of commas and double spaces, then split on space
            string[] arrName1 = string1.ToLower().Trim().Replace(",", " ").Replace("-", " ").Replace("  ", " ").Split(' ');
            string[] arrName2 = string2.ToLower().Trim().Replace(",", " ").Replace("-", " ").Replace("  ", " ").Split(' ');

            // If their length does not match
            if (arrName1.Length != arrName2.Length)
                return false;

            // Search for all parts of name1 in name2
            foreach (var n1 in arrName1)
                if (arrName2.Contains(n1) == false)
                    return false;

            // Search for all parts of name2 in name1
            foreach (var n2 in arrName2)
                if (arrName1.Contains(n2) == false)
                    return false;

            return true;
        }

        public static DateTime GetDateFromUnixTimestamp(long unixTime, bool throwException = true, bool convertToLocal = true)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            switch (unixTime.ToString().Length)
            {
                case 9:
                case 10:
                    return convertToLocal ? epoch.AddSeconds(unixTime).ToLocalTime() : epoch.AddSeconds(unixTime);
                case 13:
                    return convertToLocal ? epoch.AddMilliseconds(unixTime).ToLocalTime() : epoch.AddMilliseconds(unixTime);
            }

            if (throwException)
                throw new Exception("Error unix timestamp format for:" + unixTime.ToString());

            return epoch;
        }

        public static int MonthDifference(this DateTime lValue, DateTime rValue)
        {
            return Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
        }

        public static DateTime GetGTBStandardTime()
        {
            return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time"));
        }

        public static string WebConfigAppValue(string key)
        {
            string value = GetString(ConfigurationManager.AppSettings[key]);
            if (string.IsNullOrWhiteSpace(value) == false)
                return value;

            throw new Exception(string.Format("Missing key {0} from App Settings", key));
        }

        public static string GetWebConfigAppValue(string key, string defVal)
        {
            string val = defVal;
            try
            {
                string appSetting = GetString(ConfigurationManager.AppSettings[key]);
                if (!string.IsNullOrEmpty(appSetting))
                    val = appSetting;
            }
            catch
            { }

            return val;
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input) == false)
                return input.First().ToString().ToUpper() + input.Substring(1);
            return string.Empty;
        }

        public static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public static object GetPropertyValue(object src, string PropertyName, out PropertyInfo Prop)
        {
            PropertyInfo[] Properties = src.GetType().GetProperties();
            if (Properties.Any(x => x.Name.Equals(PropertyName)))
            {
                Prop = Properties.First(x => x.Name.Equals(PropertyName));
                return Prop.GetValue(src, null);
            }

            Prop = null;
            return null;
        }

        //public static IEnumerable<Type> GetTypesWithAttributeDatabaseColumn(Assembly assembly)
        //{
        //    foreach (Type type in assembly.GetTypes())
        //    {
        //        if (type.GetCustomAttributes(typeof(DatabaseTable), true).Length > 0)
        //        {
        //            yield return type;
        //        }
        //    }
        //}

        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds = 0)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }

        public static string GetStringWithTrailingSpaces(string Str, int TotalCharacters)
        {
            string result = Str;
            for (int i = Str.Length; i < TotalCharacters; i++)
                result += " ";

            return result;
        }

        //public static void CopyImageFromUrlToClipboard(string url)
        //{
        //    try
        //    {
        //        var img = GetImageFromUrl(url);
        //        /**
        //        // Write the image to a temporary location (todo: purge it later)
        //        var tmpFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(url));
        //        img.Save(tmpFilePath);

        //        // Group image(s)
        //        var imgCollection = new System.Collections.Specialized.StringCollection();
        //        imgCollection.Add(tmpFilePath);

        //        // Changing the drop affect to 'move' from the temp location
        //        byte[] moveEffect = new byte[] { 2, 0, 0, 0 };
        //        MemoryStream dropEffect = new MemoryStream();
        //        dropEffect.Write(moveEffect, 0, moveEffect.Length);

        //        // Set up our clipboard data
        //        DataObject data = new DataObject();
        //        data.SetFileDropList(imgCollection);
        //        data.SetData("Preferred DropEffect", dropEffect);

        //        // Push it all to the clipboard
        //        Clipboard.Clear();
        //        Clipboard.SetDataObject(data, true);
        //        */
        //        Clipboard.Clear();
        //        Clipboard.SetImage(img);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("If the following error message does not make sense, please contact system administrator" + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //public static void OpenFileFromUrl(string url)
        //{
        //    try
        //    {
        //        var img = GetImageFromUrl(url);
        //        var tmpFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(url));
        //        img.Save(tmpFilePath);
        //        System.Diagnostics.Process.Start(tmpFilePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("If the following error message does not make sense, please contact system administrator" + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //public static Image GetImageFromUrl(string url)
        //{
        //    HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
        //    using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
        //    {
        //        using (Stream stream = httpWebReponse.GetResponseStream())
        //        {
        //            return Image.FromStream(stream);
        //        }
        //    }
        //}

        //public static string GetLocalIPAddress()
        //{
        //    var host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (var ip in host.AddressList)
        //    {
        //        if (ip.AddressFamily == AddressFamily.InterNetwork)
        //            return ip.ToString();
        //    }
        //    return "0.0.0.0";
        //}

        //public static string Serialize<T>(this T obj)
        //{
        //    var settings = new XmlWriterSettings
        //    {
        //        Indent = true,
        //        OmitXmlDeclaration = false,
        //        Encoding = Encoding.UTF8
        //    };

        //    var namespaces = new XmlSerializerNamespaces();
        //    namespaces.Add(string.Empty, string.Empty);

        //    var serializer = new XmlSerializer(obj.GetType());

        //    using (var stringWriter = new StringWriterUtf8())
        //    {
        //        using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
        //        {
        //            serializer.Serialize(xmlWriter, obj, namespaces);
        //        }
        //        return stringWriter.ToString();
        //    }
        //}

        public class StringWriterUtf8 : StringWriter
        {
            public override Encoding Encoding
            {
                get
                {
                    return Encoding.UTF8;
                }
            }
        }

        //public static string Serialize<T>(this T value, System.Text.Encoding Encoding)
        //{
        //    string Result = string.Empty;

        //    if (value != null)
        //    {

        //        try
        //        {
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                using (var xw = XmlWriter.Create(ms)) // Remember to stop using XmlTextWriter  
        //                {
        //                    var serializer = new XmlSerializer(typeof(T));
        //                    serializer.Serialize(xw, value);
        //                    ms.Seek(0, SeekOrigin.Begin);
        //                    using (StreamReader sr = new StreamReader(ms, Encoding))
        //                    {
        //                        Result = sr.ReadToEnd();
        //                    }
        //                    xw.Flush();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("An error occurred", ex);
        //        }
        //    }

        //    return Result;
        //}

        public static DateTime UnixTimeStampToDateTime(long unixTimeStampInMSecs)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStampInMSecs).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime? convertToUniversalTime(DateTime? dtTime)
        {
            return dtTime.HasValue ? (DateTime?)dtTime.Value.ToUniversalTime() : null;
        }

        public static bool IsConnectedToInternet()
        {
            bool success = false;

            try
            {
                success = InternetGetConnectedState(out int description, 0);
            }
            catch
            {
                success = false;
            }

            return success;
        }
    }

    public class LinuxTime
    {

        static System.DateTime LinuxDateTimeEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        public static DateTime JavaTimeStampToDateTime(double javaTimeStamp)
        {
            // Java timestamp is millisecods past epoch

            return LinuxDateTimeEpoch.AddSeconds(Math.Round((javaTimeStamp - (javaTimeStamp % 1000)) / 1000)).ToLocalTime();
        }

        public static long Now
        {
            get
            {
                return DateTimeToJavaTimeStamp(DateTime.Now);
            }
        }
        public static DateTime JavaTimeStampToDateTimeUTC(double javaTimeStamp)
        {
            // Java timestamp is millisecods past epoch

            return LinuxDateTimeEpoch.AddSeconds(Math.Round((javaTimeStamp - (javaTimeStamp % 1000)) / 1000));
        }

        public static long DateTimeToJavaTimeStamp(DateTime date)
        {
            // Java timestamp is millisecods past epoch

            return (long)(date.ToUniversalTime() - LinuxDateTimeEpoch).TotalMilliseconds;
        }

        public static long DateTimeToJavaTimeStampUTC(DateTime date)
        {
            // Java timestamp is millisecods past epoch

            return (long)(date - LinuxDateTimeEpoch).TotalMilliseconds;
        }

        public static DateTime DateDiffStampToDateTime(DateTime refDate, double javaTimeStamp)
        {
            // Java timestamp is millisecods past epoch

            return refDate.AddSeconds(Math.Round(javaTimeStamp / 1000)).ToLocalTime();
        }

        public static long DateTimeDiffToTimeStamp(DateTime refDate, DateTime date)
        {
            // Java timestamp is millisecods past epoch

            return (long)(date.ToUniversalTime() - refDate).TotalMilliseconds;
        }

    }
}