using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Volte.Utils
{
    public class Util {

        const string ZFILE_NAME     = "CoreUtil";
        //private static Regex _regex = new Regex (@"\{(?<Variable>[\@\w\.\s]+)\}");
        public static readonly char[] WhiteSpaceChars = new char[]
        {
            (char)0x00 , (char)0x01 , (char)0x02 , (char)0x03 , (char)0x04 , (char)0x05 ,
                (char)0x06 , (char)0x07 , (char)0x08 , (char)0x09 , (char)0x0a , (char)0x0b , (char)0x0c , (char)0x0d , (char)0x0e   , (char)0x0f   ,
                (char)0x10 , (char)0x11 , (char)0x12 , (char)0x13 , (char)0x14 , (char)0x15 , (char)0x16 , (char)0x17 , (char)0x18   , (char)0x19   ,
                (char)0x1a , (char)0x1b , (char)0x1c , (char)0x1d , (char)0x1e , (char)0x1f , (char)0x7f , (char)0x85 , (char)0x2028 , (char)0x2029 ,
                (char)0xa0
        };

        public static List<string> Parameters(string text)
        {
            var matchVale = new List<string>();
            string Reg = @"(?<=\${)[^\${}]*(?=})";
            string key = string.Empty;
            foreach (Match m in Regex.Matches(text, Reg))
            {
                matchVale.Add(m.Value);
            }
            return matchVale;
        }

        public static string ReplaceWith(string original, string pattern, string replacement)
        {
            int count;
            int position0;
            int position1;

            count               = position0 = position1 = 0;
            string upperString  = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc             = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            char [] chars       = new char[original.Length + Math.Max(0, inc)];

            while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1) {
                for (int i = position0 ; i < position1 ; ++i) {
                    chars[count++] = original[i];
                }

                for (int i = 0 ; i < replacement.Length ; ++i) {
                    chars[count++] = replacement[i];
                }

                position0 = position1 + pattern.Length;
            }

            if (position0 == 0) {
                return original;
            }

            for (int i = position0 ; i < original.Length ; ++i) {
                chars[count++] = original[i];
            }

            return new string(chars, 0, count);
        }
        public static bool IsNonPrintable(string s)
        {
            if(string.IsNullOrEmpty(s)){
                return false;
            }

            if (s.Trim(WhiteSpaceChars).Length != s.Length) {
                return true;
            }

            return false;
        }

        public static bool HasNonPrintable(string s)
        {
            if (string.IsNullOrEmpty(s)) {
                return false;
            } else {
                for (int i = 0; i < WhiteSpaceChars.Length; i++) {
                    if (s.IndexOf(WhiteSpaceChars[i])>=0){
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool HasChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        }

        public static string DataTypeChar(string datatype)
        {
            string typechar = "c";

            switch (datatype.Trim().ToLower()) {
                case "boolean":
                    typechar = "l";
                    break;

                case "datetime":
                    typechar = "d";
                    break;

                case "float":
                case "decimal":
                    typechar = "n";
                    break;

                case "int":
                    typechar = "i";
                    break;

                case "image":
                    typechar = "b";
                    break;

                case "ntext":
                case "picture":
                case "nvarchar":
                    typechar = "c";
                    break;

                case "caption":
                case "label":
                    typechar = "x";
                    break;
            }

            return typechar;
        }

        public static void CreateDir(string sPathName)
        {
            try {
                if (!Directory.Exists(sPathName)) {
                    Directory.CreateDirectory(sPathName);
                }
            } catch (Exception ex) {
            }
        }

        public static void WriteContents(string sFilePath, string sContents, bool bAppend = true)
        {
            FileStream file = null;
            StreamWriter sw = null;

            try {

                string _DirName = System.IO.Path.GetDirectoryName(sFilePath);

                if (_DirName != "") {
                    if (!Directory.Exists(_DirName)) {
                        Directory.CreateDirectory(_DirName);
                    }
                }

                if (bAppend == false) {
                    if (File.Exists(sFilePath)) {
                        File.Delete(sFilePath);
                    }
                }

                // Specify file, instructions, and privelegdes
                if (bAppend) {
                    if (File.Exists(sFilePath)) {
                        file = new FileStream(sFilePath, FileMode.Append , FileAccess.Write);
                    } else {
                        file = new FileStream(sFilePath, FileMode.Create, FileAccess.Write);
                    }
                } else {
                    file = new FileStream(sFilePath, FileMode.Create, FileAccess.Write);
                }

                UTF8Encoding _UTF8Encoding = new UTF8Encoding(false, true);

                sw = new StreamWriter(file, _UTF8Encoding);

                // Write a string to the file
                sw.Write(sContents);
            } catch (Exception ex) {
                throw new Exception("WriteFileContents() failed with error: " + ex.Message);
            } finally {
                // Close StreamWriter
                if (sw != null) {
                    sw.Close();
                }

                // Close file
                if (file != null) {
                    file.Close();
                }
            }
        }

        public static bool IsBoolean(string oValue)
        {
            if (string.IsNullOrEmpty(oValue)) {
                oValue = "";
            }

            oValue = oValue.ToUpper();

            if (oValue.Equals("CHECKED")) {
                return true;
            }

            if (oValue.Equals("Y")) {
                return true;
            }

            if (oValue.Equals("N")) {
                return true;
            }

            if (oValue.Equals("YES")) {
                return true;
            }

            if (oValue.Equals("NO")) {
                return true;
            }

            if (oValue.Equals("TRUE")) {
                return true;
            }

            if (oValue.Equals("FALSE")) {
                return true;
            }

            return false;
        }

        public static Stream FileDecrypt(string fileName)
        {
            if (System.IO.File.Exists(fileName)) {
                if (System.IO.Path.GetExtension(fileName) == ".tpl") {

                    StreamReader objReader = new StreamReader(fileName);

                    byte[] bt = Convert.FromBase64String(objReader.ReadToEnd());
                    objReader.Close();
                    return new System.IO.MemoryStream(bt);
                } else {
                    FileStream tfs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    byte[] data = Util.ReadFully(tfs);
                    tfs.Close();

                    return new System.IO.MemoryStream(data);

                }
            }

            return null;
        }

        public static string FileEncrypt(string fileName)
        {
            string _TempFile = Util.ReplaceWith(fileName, "xls", "tpl");

            if (System.IO.File.Exists(fileName)) {
                if (System.IO.Path.GetExtension(fileName) == ".xls") {
                    FileStream tfs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    byte[] data = Util.ReadFully(tfs);
                    tfs.Close();

                    string base64string = Convert.ToBase64String(data);

                    if (System.IO.File.Exists(_TempFile)) {
                        File.Delete(_TempFile);
                    }

                    StreamWriter sw = new StreamWriter(_TempFile, false);
                    sw.WriteLine(base64string);
                    sw.Flush();
                    sw.Close();
                }
            }

            return _TempFile;
        }

        public static string Base64UrlEncodeByte(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }
        public static string Base64UrlEncode(string input)
        {
            byte[] by  = Encoding.Unicode.GetBytes(input.ToCharArray());
            var output = Convert.ToBase64String(by);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }
        public static byte[] Base64UrlDecodeByte(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding

            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break;  // One pad char
                default: throw new FormatException("Illegal base64url string!");
            }
            return Convert.FromBase64String(output); // Standard base64 decoder
        }

        public static string Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break;  // One pad char
                default: throw new FormatException("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return Encoding.Unicode.GetString(converted);
        }

        public static string Base64Encoder(string cString)
        {
            byte[] by  = Encoding.Unicode.GetBytes(cString.ToCharArray());
            string str = Convert.ToBase64String(by);
            return str;
        }

        public static string Base64Encoder(byte[] by)
        {
            string str = Convert.ToBase64String(by);
            return str;
        }

        public static string Base64Decoder(string cString)
        {
            byte[] barray = Convert.FromBase64String(cString);
            return Encoding.Unicode.GetString(barray);
        }

        public static string Base64Decoder(byte[] barray)
        {
            return Encoding.Unicode.GetString(barray);
        }

        public static byte[] ReadFully(Stream stream)
        {
            // 初始化一个32k的缓存
            byte[] buffer = new byte[32768];
            using(MemoryStream ms = new MemoryStream()) {
                //返回结果后会自动回收调用该对象的Dispose方法释放内存
                // 不停的读取
                while (true) {
                    int read = stream.Read(buffer, 0, buffer.Length);

                    // 直到读取完最后的3M数据就可以返回结果了
                    if (read <= 0) {
                        return ms.ToArray();
                    }

                    ms.Write(buffer, 0, read);
                }
            }
        }

        public static string DateTimeVariable(string cValue)
        {
            return DateTimeVariable(cValue, DateTime.Now);
        }

        public static string TypeChar(string datatype)
        {
            string typechar = "c";

            switch (datatype.Trim().ToLower()) {
                case "boolean":
                    typechar = "l";
                    break;

                case "datetime":
                    typechar = "d";
                    break;

                case "float":
                case "decimal":
                    typechar = "n";
                    break;

                case "int":
                    typechar = "i";
                    break;

                case "image":
                    typechar = "b";
                    break;

                case "ntext":
                case "picture":
                case "nvarchar":
                    typechar = "c";
                    break;

                case "caption":
                case "label":
                    typechar = "x";
                    break;
            }

            return typechar;
        }

        public static string DateTimeVariable(string cValue , DateTime _DateTime)
        {
            if (string.IsNullOrEmpty(cValue)) {
                return "";
            }
            string sValue = "";

            string[] arry = cValue.Replace("]","[").Split('[');

            foreach (string s in arry){
                DateTime _t   = _DateTime;
                if (!string.IsNullOrEmpty(s)){
                    string _Value = s;

                    for (int i = 1; i < 24; i++) {
                        if (_Value.IndexOf("{M+" + i + "}")>=0){
                            _t= _DateTime.AddMonths(+i);
                            _Value = ReplaceWith(_Value , "{M+" + i + "}" , "");
                        }
                        if (_Value.IndexOf("{M-" + i + "}")>=0){
                            _t= _DateTime.AddMonths(-i);
                            _Value = ReplaceWith(_Value , "{M-" + i + "}" , "");
                        }
                    }

                    for (int i = 1; i < 60; i++) {
                        if (_Value.IndexOf("{D+" + i + "}")>=0){
                            _t= _DateTime.AddDays(+i);
                            _Value = ReplaceWith(_Value , "{D+" + i + "}" , "");
                        }
                        if (_Value.IndexOf("{D-" + i + "}")>=0){
                            _t= _DateTime.AddDays(-i);
                            _Value = ReplaceWith(_Value , "{D-" + i + "}" , "");
                        }
                    }
                    for (int i = 1; i < 5; i++) {
                        if (_Value.IndexOf("{Y+" + i + "}")>=0){
                            _t= _DateTime.AddYears(+i);
                            _Value = ReplaceWith(_Value , "{Y+" + i + "}" , "");
                        }
                        if (_Value.IndexOf("{Y-" + i + "}")>=0){
                            _t= _DateTime.AddYears(-i);
                            _Value = ReplaceWith(_Value , "{Y-" + i + "}" , "");
                        }
                    }

                    _Value = ReplaceWith(_Value , "{MM}"   , _t.ToString("MM"));
                    _Value = ReplaceWith(_Value , "{YY}"   , _t.ToString("yy"));
                    _Value = ReplaceWith(_Value , "{YYYY}" , _t.ToString("yyyy"));
                    _Value = ReplaceWith(_Value , "{YY}"   , _t.ToString("yy"));
                    _Value = ReplaceWith(_Value , "{DD}"   , _t.ToString("dd"));
                    _Value = ReplaceWith(_Value , "{~}"    , IdGenerator.NewBase36("-"));
                    sValue = sValue+_Value;

                }
            }

            if (sValue==""){
                sValue=cValue;
            }

            sValue = ReplaceWith(sValue , "{~}"    , IdGenerator.NewBase36("-"));
            sValue = ReplaceWith(sValue , "{MM}"   , _DateTime.ToString("MM"));
            sValue = ReplaceWith(sValue , "{YY}"   , _DateTime.ToString("yy"));
            sValue = ReplaceWith(sValue , "{YYYY}" , _DateTime.ToString("yyyy"));
            sValue = ReplaceWith(sValue , "{YY}"   , _DateTime.ToString("yy"));
            sValue = ReplaceWith(sValue , "{DD}"   , _DateTime.ToString("dd"));

            DateTime startWeek = _DateTime.AddDays(1 - Convert.ToInt32(_DateTime.DayOfWeek.ToString("d")));
            DateTime endWeek   = startWeek.AddDays(6);

            return sValue;
        }

        public static StringBuilder ReadFile(string cFileName)
        {
            StringBuilder _s = new StringBuilder();

            if (File.Exists(cFileName)) {
                using(StreamReader sr = new StreamReader(cFileName)) {
                    _s.Append(sr.ReadToEnd());
                }
            }

            return _s;
        }

        public static string HTMLEnCode(string text)
        {

            StringBuilder _s = new StringBuilder();

            foreach (char ch in text) {
                switch (ch) {
                    case '\'':
                        _s.Append("&apos;");
                        break;

                    case '<':
                        _s.Append("&lt;");
                        break;

                    case '>':
                        _s.Append("&gt;");
                        break;

                    case '"':
                        _s.Append("&quot;");
                        break;

                    case '&':
                        _s.Append("&amp;");
                        break;

                    default:
                        _s.Append(ch);
                        break;
                }
            }

            return _s.ToString();
        }

        public static void EscapeString(StringBuilder ss, string text)
        {
            foreach (char ch in text) {
                switch (ch) {
                    case '"':
                        ss.Append("\\\"");
                        break;

                    case '\\':
                        ss.Append(@"\\");
                        break;

                    case '\b':
                        ss.Append(@"\b");
                        break;

                    case '\f':
                        ss.Append(@"\f");
                        break;

                    case '\n':
                        ss.Append(@"\n");
                        break;

                    case '\r':
                        ss.Append(@"\r");
                        break;

                    case '\t':
                        ss.Append(@"\t");
                        break;

                    default:
                        if (char.IsLetterOrDigit(ch)) {
                            ss.Append(ch);
                            break;
                        } else if (char.IsPunctuation(ch)) {
                            ss.Append(ch);
                            break;
                        } else if (char.IsSeparator(ch)) {
                            ss.Append(ch);
                        break;
                        } else if (char.IsWhiteSpace(ch)) {
                            ss.Append(ch);
                        break;
                        } else if (char.IsSymbol(ch)) {
                            ss.Append(ch);
                        break;
                        } else {
                            ss.Append("\\u");
                            ss.Append(((int) ch).ToString("X4", NumberFormatInfo.InvariantInfo));
                        break;
                        }

                }
            }
        }
        private static char SEPARATOR = '_';

        public static string ToUnderlineName(string s) {
            if (string.IsNullOrEmpty(s)) {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            bool upperCase = false;
            for (int i = 0; i < s.Length; i++) {
                char c = s[i];

                bool nextUpperCase = true;

                if (i < (s.Length - 1)) {
                    nextUpperCase = char.IsUpper(s[i + 1]);
                }

                if ((i >= 0) && char.IsUpper(c)) {
                    if (!upperCase || !nextUpperCase) {
                        if (i > 0) sb.Append(SEPARATOR);
                    }
                    upperCase = true;
                } else {
                    upperCase = false;
                }

                sb.Append(char.ToLower(c));
            }

            return sb.ToString();
        }

        public static string ToCamelCase(string s,int Capitalize = 0) {
            if (string.IsNullOrEmpty(s)) {
                return "";
            }

            s = s.ToLower();

            StringBuilder sb = new StringBuilder(s.Length);
            bool upperCase = false;
            for (int i = 0; i < s.Length; i++) {
                char c = s[i];

                if (c == SEPARATOR) {
                    upperCase = true;
                } else if (Capitalize == i) {
                    sb.Append(char.ToUpper(c));
                } else if (upperCase) {
                    sb.Append(char.ToUpper(c));
                    upperCase = false;
                } else {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string AntiSQLInjection(string str)
        {
            if (string.IsNullOrEmpty(str)) {
                return "";
            }

            str = str.Replace("'"    , "");
            //str = str.Replace(Chr(0) , '');
            str = str.Replace("\\"   , "");
            str = str.Replace(";"    , "");
            return str;
        }

        public static string UrlEncode(object c)
        {
            if (string.IsNullOrEmpty(c.ToString())) {
                return "";
            }

            return System.Web.HttpUtility.UrlEncode(c.ToString());
        }

        public static string UrlDecode(object c)
        {
            if (string.IsNullOrEmpty(c.ToString())) {
                return "";
            }

            return System.Web.HttpUtility.UrlDecode(c.ToString());
        }

        public static string Request(HttpContext oContext, string cName)
        {
            string _Value = "";

            try {

                if (oContext.Request[cName] != null) {
                    _Value = (string) oContext.Request[cName];
                }

            } catch (Exception ex) {

            }

            return _Value;
        }

        public static string UserAgent()
        {
            string sValue= "";
            try {
                sValue= HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
                if (string.IsNullOrEmpty(sValue)){
                    sValue="";
                }
            } catch {
            }

            return sValue;
        }

        public static string RemoteAddr()
        {
            string result = "0.0.0.0";

            try {
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (null == result || result == String.Empty) {
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (null == result || result == String.Empty) {
                    result = HttpContext.Current.Request.UserHostAddress;
                }

                return result;
            } catch {
            }

            return result;
        }

        public static void Cookies(HttpContext context , string cName , string cValue)
        {

            try {
                HttpCookie cookies = new HttpCookie("iGOS");
                cookies["LogonId"] = cValue;
                HttpContext.Current.Response.Cookies.Add(cookies);

            } catch (Exception ex) {
            }
        }

        public static string Cookies(HttpContext context , string cName)
        {
            string _Value = "";

            try {
                HttpCookie cookies = HttpContext.Current.Request.Cookies["iGOS"];

                if (cookies != null) {
                    _Value = cookies["LogonId"];
                }

            } catch (Exception ex) {
            }

            return _Value;
        }

        public static string Request(string cName)
        {
            string _Value = "";

            try {

                if (HttpContext.Current.Request[cName] != null) {
                    _Value = (string) HttpContext.Current.Request[cName];
                }

            } catch (Exception ex) {
            }

            return _Value;
        }

        public static Image ScaleImage(Image imgPhoto , int maxWidth , int maxHeight)
        {
            if (maxWidth <= 0) {
                maxWidth = 400;
            }

            if (maxHeight <= 0) {
                maxHeight = 400;
            }

            float _OriginalWidth  = imgPhoto.Width;
            float _OriginalHeight = imgPhoto.Height;
            float destHeight        = 0;
            float destWidth         = 0;

            int sourceX = 0;
            int sourceY = 0;
            int destX   = 0;
            int destY   = 0;

            if (_OriginalWidth <= maxWidth && _OriginalHeight <= maxHeight) {
                destWidth  = _OriginalWidth;
                destHeight = _OriginalHeight;
            } else {
                if (_OriginalWidth > maxWidth) {
                    destWidth  = maxWidth;
                } else {
                    destWidth  = _OriginalWidth;
                }

                destHeight = (destWidth * _OriginalHeight) / _OriginalWidth;
            }

            Bitmap bmPhoto = new Bitmap((int)destWidth, (int)destHeight, PixelFormat.Format32bppPArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                    new Rectangle(destX, destY, (int)destWidth, (int)destHeight),
                    new Rectangle(sourceX, sourceY, (int)_OriginalWidth, (int)_OriginalHeight),
                    GraphicsUnit.Pixel);

            grPhoto.Dispose();

            return bmPhoto;
        }

        public static DateTime DateTime_MinValue
        {
            get {
                return DateTime.MinValue;
            }
        }

        public static System.Drawing.Size NewSize(int maxWidth, int maxHeight, int imageOriginalWidth, int imageOriginalHeight)
        {
            double w = 0.0;
            double h = 0.0;
            double sw = Convert.ToDouble(imageOriginalWidth);
            double sh = Convert.ToDouble(imageOriginalHeight);
            double mw = Convert.ToDouble(maxWidth);
            double mh = Convert.ToDouble(maxHeight);

            if (sw < mw && sh < mh) {
                w = sw;
                h = sh;
            } else if ((sw / sh) > (mw / mh)) {
                w = maxWidth;
                h = (w * sh) / sw;
            } else {
                h = maxHeight;
                w = (h * sw) / sh;
            }

            return new System.Drawing.Size(Convert.ToInt32(w), Convert.ToInt32(h));
        }

        public static Image ThumbNailImage(Image originalImage, int thumMaxWidth, int thumMaxHeight)
        {
            Size thumRealSize = System.Drawing.Size.Empty;
            Image newImage = originalImage;
            Graphics graphics = null;

            try {
                thumRealSize = NewSize(thumMaxWidth, thumMaxHeight, originalImage.Width, originalImage.Height);
                //newImage = new System.Drawing.Bitmap(thumRealSize.Width, thumRealSize.Height);
                newImage = new System.Drawing.Bitmap(thumMaxWidth, thumMaxHeight);
                graphics = Graphics.FromImage(newImage);
                //graphics.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, thumRealSize.Width, thumRealSize.Height), new Rectangle(0, 0, originalImage.Width, originalImage.Height), GraphicsUnit.Pixel);
                graphics.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, thumMaxWidth, thumMaxHeight), new Rectangle(0, 0, originalImage.Width, originalImage.Height), GraphicsUnit.Pixel);
            } catch { }
            finally {
                if (graphics != null) {
                    graphics.Dispose();
                    graphics = null;
                }
            }

            return newImage;
        }

        public string ParsePeriod(string sPeriod)
        {

            int _position = sPeriod.IndexOf(":");
            string sType  = "";
            string sRtv   = "";

            if (_position > 0) {
                sType         = sPeriod.Substring(0, _position);
                string sValue = sPeriod.Substring(_position+1,sPeriod.Length-_position-1);
                string[] aRang = Regex.Split(sValue , "," , RegexOptions.IgnoreCase);

                string sFrom = "";
                string sTo   = "";
                if (aRang.Length==1){

                    sFrom = aRang[0];
                    sTo   = aRang[0];
                }
                if (aRang.Length==2){

                    sFrom = aRang[0];
                    sTo   = aRang[1];
                }

                if (sType=="day"){

                    if (sFrom!=""){
                        int nFrom = int.Parse(sFrom);
                        sRtv=DateTime.Now.AddDays(nFrom).ToString("yyyy-MM-dd");
                    }

                    if (sTo!=""){
                        int nTo = int.Parse(sTo);
                        sRtv=sRtv +","+ DateTime.Now.AddDays(nTo).ToString("yyyy-MM-dd");
                    }
                }else if (sType=="week"){

                }else if (sType=="month"){

                    if (sFrom!=""){
                        int nFrom = int.Parse(sFrom);
                        sRtv=DateTime.Now.AddMonths(nFrom).ToString("yyyy-MM-01");
                    }

                    if (sTo!=""){
                        int nTo = int.Parse(sTo);
                        sRtv=sRtv +","+ DateTime.Now.AddMonths(nTo).ToString("yyyy-MM-dd");
                    }

                }else if (sType=="quarter"){

                }else if (sType=="year"){
                    if (sFrom!=""){
                        int nFrom = int.Parse(sFrom);
                        sRtv=DateTime.Now.AddYears(nFrom).ToString("yyyy-MM-dd");
                    }

                    if (sTo!=""){
                        int nTo = int.Parse(sTo);
                        sRtv=sRtv +","+ DateTime.Now.AddYears(nTo).AddDays(-1).ToString("yyyy-MM-dd");
                    }

                }else if (sType=="usr"){

                }

            }

            return sRtv;

        }

        public static long DateTimeToMilliSecond(DateTime dateTime)
        {
            DateTime windowsEpoch = new DateTime(1601, 1, 1, 0, 0, 0, 0);
            DateTime javaEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long epochDiff = (javaEpoch.ToFileTimeUtc() - windowsEpoch.ToFileTimeUtc())/TimeSpan.TicksPerMillisecond;
            return (dateTime.ToFileTime() / TimeSpan.TicksPerMillisecond) - epochDiff;
        }

        public static bool IsNumeric(object str)
        {
            decimal d;
            return decimal.TryParse(str.ToString(), out d);
        }
        public static bool IsDateTime(object str)
        {
            DateTime d;
            return DateTime.TryParse(str.ToString(), out d);
        }

        public static bool StringToBoolean(object oValue)
        {
            if (string.Compare(oValue.ToString(), "CHECKED", true) == 0) {
                return true;
            }

            if (string.Compare(oValue.ToString(), "ON", true) == 0) {
                return true;
            }

            if (string.Compare(oValue.ToString(), "Y", true) == 0) {
                return true;
            }

            if (string.Compare(oValue.ToString(), "YES", true) == 0) {
                return true;
            }

            if (string.Compare(oValue.ToString(), "1", true) == 0) {
                return true;
            }

            if (string.Compare(oValue.ToString(), "TRUE", true) == 0) {
                return true;
            }

            return false;
        }

        public static bool ToBoolean(object cValue)
        {
            bool d;
            return bool.TryParse(Convert.ToString(cValue), out d) ? d : false;
        }

        public static decimal ToDecimal(object cValue)
        {
            decimal d;
            return decimal.TryParse(Convert.ToString(cValue), out d) ? d : 0M;
        }

        public static int ToInt(object oValue)
        {
            return Util.ToInt32(oValue);
        }

        public static int ToInt32(object oValue)
        {
            if (oValue==null){
                return 0;
            }
            int d;
            return int.TryParse(oValue.ToString(), out d) ? d : 0;
        }

        public static long ToLong(object oValue)
        {
            long d;
            return long.TryParse(oValue.ToString(), out d) ? d : 0;
        }

        public static DateTime ToDateTime(object oValue)
        {

            if (oValue is DateTime) {
                return (DateTime) oValue;
            } else if (oValue == null || oValue.ToString() == "") {
                return Util.DateTime_MinValue;
            } else if (Util.IsNumeric(oValue) && oValue.ToString().Length == 8) {
                return DateTime.ParseExact(oValue.ToString(), "yyyyMMdd", null);
            } else if (Util.IsNumeric(oValue) && oValue.ToString().Length >= 13) {
                long nDateTime=Util.ToLong(oValue);
                if (nDateTime>0){
                    try {
                        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                        TimeSpan toNow = new TimeSpan(nDateTime*10000);
                        return dtStart.Add(toNow);
                    } catch (Exception ex) {
                    }
                }
                return Util.DateTime_MinValue;
            } else {
                string s=oValue.ToString();
                if (s.IndexOf("T")>0 && s.IndexOf("Z")>0){
                    return GMT2Local(s);
                }else{
                    DateTime d;
                    return DateTime.TryParse(oValue.ToString(), out d) ? d : Util.DateTime_MinValue;
                }
            }
        }

        public static DateTime GMT2Local(string gmt)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                string pattern = "";
                if (gmt.IndexOf("+0") != -1)
                {
                    gmt = gmt.Replace("GMT", "");
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss zzz";
                }
                if (gmt.ToUpper().IndexOf("GMT") != -1)
                {
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
                }
                if (pattern != "")
                {
                    dt = DateTime.ParseExact(gmt, pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
                    dt = dt.ToLocalTime();
                }
                else
                {
                    dt = Convert.ToDateTime(gmt);
                }
            }
            catch
            {
            }
            return dt;
        }

        public static bool InDateRange(DateTime dt , DateTime? dt1 , DateTime? dt2)
        {
            if (dt1.HasValue && dt2.HasValue){
                return dt.CompareTo(dt1) >= 0 && dt.CompareTo(dt2) <= 0;
            }else if (dt1.HasValue && !dt2.HasValue){
                return dt.CompareTo(dt1) >= 0;
            }else if (!dt1.HasValue && dt2.HasValue){
                return dt.CompareTo(dt2) >= 0;
            }else{
                return true;
            }
        }

        public static DateTime? ToDateTime2(object oValue)
        {

            if (oValue is DateTime) {
                return (DateTime) oValue;
            } else if (oValue.ToString() == "") {
                return null;
            } else if (Util.IsNumeric(oValue) && oValue.ToString().Length == 8) {
                return DateTime.ParseExact(oValue.ToString(), "yyyyMMdd", null);
            } else if (Util.IsNumeric(oValue) && oValue.ToString().Length >= 13) {
                long nDateTime=Util.ToLong(oValue);
                if (nDateTime>0){
                    try {
                        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                        TimeSpan toNow = new TimeSpan(nDateTime*10000);
                        return dtStart.Add(toNow);
                    } catch (Exception ex) {
                    }
                }
                return null;
            } else {
                return Convert.ToDateTime(oValue);
            }
        }

    }
}
