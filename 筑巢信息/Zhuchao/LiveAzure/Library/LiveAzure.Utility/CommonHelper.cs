using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Globalization;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;

namespace LiveAzure.Utility
{
    public static class CommonHelper
    {

        #region 验证方法

        /// <summary>
        /// 验证字符串"a,b,c"中是否都是数字(true:成功 false:失败)
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        public static bool CheckStringIsNumber(string fString)
        {
            string[] sArray = fString.Split(',');
            for (int i = 0; i < sArray.Length; i++)
            {
                if (!CheckInt(sArray[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 验证整数(true:成功 false:失败)
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        public static bool CheckInt(string fString)
        {
            return Regex.IsMatch(fString, @"^([+-]?)\d+$");
        }

        /// <summary>
        /// 验证正整数(true:成功 false:失败)
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        public static bool CheckPositiveInt(string fString)
        {
            return Regex.IsMatch(fString, @"^([+]?)\d+$");
        }

        /// <summary>
        /// 验证负整数(true:成功 false:失败)
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        public static bool CheckNetagiveInt(string fString)
        {
            return Regex.IsMatch(fString, @"^-\d+$");
        }

        /// <summary>
        /// 验证日期短日期，形如(2003-12-05)(true:成功 false:失败)
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        public static bool CheckShortDate(string fString)
        {
            return Regex.IsMatch(fString, @"^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2})$");
        }

        /// <summary>
        /// 验证密码复杂度，要求字母数字，至少6位
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        public static bool CheckPassword(string fString)
        {
            return ((fString.Length >= 6) && Regex.IsMatch(fString, @"(([0-9]+[a-zA-Z]+[0-9]*)|([a-zA-Z]+[0-9]+[a-zA-Z]*))"));
        }

        /// <summary>
        /// 验证IPv4地址(true:成功 false:失败)
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        public static bool CheckIpAddress(string fString)
        {
            return Regex.IsMatch(fString, @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
        }

        /// <summary>
        /// 验证指定范围内的字节数(true:成功 false:失败)
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        /// <param name="MinSize">最小字节数</param>
        /// <param name="MaxSize">最大字节数</param>
        public static bool CheckSize(string fString, int MinSize, int MaxSize)
        {
            if (fString.Length < MinSize || fString.Length > MaxSize)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 验证数字在指定范围内(true:成功 false:失败)
        /// </summary>
        /// <param name="nNumber">处理的数字</param>
        /// <param name="MinSize">最小值</param>
        /// <param name="MaxSize">最大值</param>
        public static bool CheckSize(int nNumber, int MinSize, int MaxSize)
        {
            if (nNumber < MinSize || nNumber > MaxSize)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 判断文件格式
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        /// <param name="FileType">验证的文件格式，例如“.txt”</param>
        public static bool CheckFileType(string fString, string fFileType)
        {
            string extension = Path.GetExtension(fString);
            if (extension.ToUpper() == fFileType.ToUpper())
                return true;
            else
                return false;
        }

        /// <summary>
        /// 验证是否英文与数字的组合,验证帐号和密码用(true:成功 false:失败)
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        public static bool CheckAccount(string fString)
        {
            return Regex.IsMatch(fString, @"^([A-Za-z0-9])+$");
        }

        #endregion

        #region 其他方法

        /// <summary>
        /// 转换成整数（针对可能有科学计算表达式的内容）
        /// </summary>
        /// <param name="sTotalCount">科学计数表达式</param>
        /// <returns>整数</returns>
        public static int GetIntFromScientific(string sTotalCount)
        {
            int nTotalCount = 0;
            try
            {
                string[] sTotalCountArray = sTotalCount.Split('+');
                if (sTotalCountArray.Length == 2)
                {
                    decimal d1 = decimal.Parse(sTotalCountArray[0].Remove(sTotalCountArray[0].Length - 1, 1));
                    int nPer = int.Parse(sTotalCountArray[1]);
                    int nPerFinal = 1;
                    for (int i = 0; i < nPer; i++)
                    {
                        nPerFinal = nPerFinal * 10;
                    }
                    d1 = d1 * nPerFinal;
                    nTotalCount = Convert.ToInt32(d1);
                }
                else
                {
                    nTotalCount = int.Parse(sTotalCountArray[0]);
                }
            }
            catch
            {
                Random oRnd = new Random();
                nTotalCount = oRnd.Next(123412341, 1234123411);
            }
            return nTotalCount;
        }

        /// <summary>
        /// 防止sql注入(只允许字母和数字 A-Za-z0-9)
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        public static string QueryStringProtect(string fString)
        {
            if (!String.IsNullOrEmpty(fString))
                return Regex.Replace(fString, @"\W", "", RegexOptions.IgnoreCase);
            else
                return "";
        }

        /// <summary>
        /// 防止sql注入(允许中文的时候用，或者必须拼接sql的时候使用)
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        public static string QueryStringProtect2(string fString)
        {
            string sSqlKeyStr = "and,exec,insert,select,delete,update,count,chr,mid,master,truncate,char,declare";
            string[] sSqlKeyArray = sSqlKeyStr.Split(',');
            if (!String.IsNullOrEmpty( fString))
            {
                fString = Regex.Replace(                    fString,                    @"'|\\|""|;| |-|,|\*|\%",                    "",                    RegexOptions.IgnoreCase);
                foreach (string sKey in sSqlKeyArray)
                    fString = Regex.Replace(fString, " " + sKey + " ", "", RegexOptions.IgnoreCase);
                return fString;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 产生随机数字符串
        /// <param name="nLength">字符串长度</param>
        /// </summary>
        public static string RandomString(int nLength)
        {
            int number;
            char code;
            string checkCode = String.Empty;
            System.Random random = new Random();
            for (int i = 0; i < nLength; i++)
            {
                number = random.Next();
                if (number % 2 == 0)
                    code = (char)('0' + (char)(number % 10));
                else
                    code = (char)('A' + (char)(number % 26));
                checkCode += code.ToString();
            }
            return checkCode;
        }

        /// <summary>
        /// 产生唯一随机数
        /// </summary>
        public static string RandomNumber()
        {
            Random R = new Random();
            int MaxLimit = 10000;//上限
            int MinLimit = 1;//下限
            int a = R.Next(MinLimit, MaxLimit);
            int b = R.Next(MinLimit, MaxLimit);
            DateTimeOffset nowtime = DateTimeOffset.Now;
            return "" + nowtime.Year + nowtime.Month + nowtime.Day + nowtime.Hour + nowtime.Minute + nowtime.Second + nowtime.Millisecond + a + b;
        }

        /// <summary>
        /// 产生唯一随机数
        /// </summary>
        /// <param name="nLength">随机数长度</param>
        public static string RandomNumber(int nLength)
        {
            Random R = new Random();
            string sMin = "1";
            string sMax = "9";
            for (int i = 0; i < nLength - 1; i++)
            {
                sMin = sMin + "0";
                sMax = sMax + "9";
            }
            int nMinLimit = int.Parse(sMin);  //下限
            int nMaxLimit = int.Parse(sMax);  //上限
            int nResult = R.Next(nMinLimit, nMaxLimit);
            return nResult.ToString();
        }

        /// <summary>
        /// 返回"1970-1-1 00:00:00"到现在的秒间隔,时间戳验证用
        /// </summary>
        public static string GetTimeSpan(DateTimeOffset dDate)
        {
            DateTimeOffset date1 = DateTimeOffset.Parse("1970-1-1 00:00:00");
            System.TimeSpan diff = dDate.Subtract(date1);
            //天，时，分，秒
            long TimeSpan = diff.Days * 24 * 3600 + diff.Hours * 3600 + diff.Minutes * 60 + diff.Seconds;
            return TimeSpan.ToString();
        }

        /// <summary>
        /// 时间戳验证
        /// </summary>
        /// <param name="nTimeSpan">待验证的时间戳</param>
        /// <param name="SecondSpan">验证的秒数</param>
        public static bool CheckTimeSpan(long nTimeSpan, int SecondSpan)
        {
            long TimeSpan_Now = Int64.Parse(GetTimeSpan(DateTimeOffset.Now));
            if (TimeSpan_Now - nTimeSpan > 0 && TimeSpan_Now - nTimeSpan < SecondSpan)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 返回XML特殊格式CDATA字符串
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns>
        public static string GetXMLCDATA(string sText)
        {
            return "<![CDATA[" + sText + "]]>";
        }

        /// <summary>
        /// 将xml文本输出中的不规范字符去除
        /// </summary>
        /// <param name="sText">原始文本</param>
        /// <returns></returns>
        public static string FilterXMLText(string sText)
        {
            Regex InvalidXMLCharacter = new Regex(@"[\u0000-\u0008\u000B\u000C\u000E-\u001F\uD800-\uDFFF\uFFFE\uFFFF]", RegexOptions.Compiled);
            return InvalidXMLCharacter.Replace(sText, " ");
        }
        
        /// <summary>
        /// 将输入内容中的html内容替换，避免被asp错认为特定符号
        /// </summary>
        public static string HtmlFormat(string fString)
        {
            fString = fString.Replace("<", "&lt;");
            fString = fString.Replace(">", "&gt;");
            fString = fString.Replace("'", "''");
            fString = fString.Replace(" ", "&nbsp;");
            //fString = fString.Replace("\r\n", "<br />");
            //fString = fString.Replace("\n", "<br />");
            return fString;
        }

        /// <summary>
        /// 移除html标记
        /// </summary>
        public static string HtmlRemoveFormat(string fString)
        {
            //删除脚本
            fString = Regex.Replace(fString, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            fString = Regex.Replace(fString, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"-->", "", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"<!--.*", "", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            fString = Regex.Replace(fString, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            fString.Replace("<", "");
            fString.Replace(">", "");
            fString.Replace("\r\n", "");
            // fString = HttpContext.Current.Server.HtmlEncode(fString).Trim();
            return fString;
        }

        /// <summary>
        /// 显示在表单的文本域中
        /// </summary>
        public static string RHtmlFormat(string fString)
        {
            fString = fString.Replace("&lt;", "<");
            fString = fString.Replace("&gt;", ">");
            return fString;
        }

        /// <summary>
        /// 格式化显示在页面中
        /// </summary>
        public static string FormatStr(string fString)
        {
            fString = fString.Replace("\n", " <br />");
            fString = fString.Replace("  ", "　");
            return fString;
        }

        /// <summary>
        /// 实现JAVASCRIPT escape方法
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string JSescape(string s)
        {
            StringBuilder sb = new StringBuilder();
            byte[] ba = System.Text.Encoding.Unicode.GetBytes(s);
            for (int i = 0; i < ba.Length; i += 2)
            {
                sb.Append("%u");
                sb.Append(ba[i + 1].ToString("X2"));
                sb.Append(ba[i].ToString("X2"));
            }
            return sb.ToString();
        }
        
        /// <summary>
        /// 截取html字符
        /// </summary>
        /// <param name="sText">处理的字符串</param>
        /// <param name="nlen">保留字节数</param>
        /// <param name="nType">类型 0:不显示... 1:显示...</param>
        /// <returns></returns>
        public static string StringMaxlongHtml(string sText, int nlen, int nType)
        {
            string Pattern = null;
            MatchCollection m = null;
            StringBuilder result = new StringBuilder();
            int n = 0;
            char temp;
            bool isCode = false; //是不是HTML代码
            bool isHTML = false; //是不是HTML特殊字符,如&nbsp;
            char[] pchar = sText.ToCharArray();
            for (int i = 0; i < pchar.Length; i++)
            {
                temp = pchar[i];
                if (temp == '<')
                {
                    isCode = true;
                }
                else if (temp == '&')
                {
                    isHTML = true;
                }
                else if (temp == '>' && isCode)
                {
                    n = n - 1;
                    isCode = false;
                }
                else if (temp == ';' && isHTML)
                {
                    isHTML = false;
                }

                if (!isCode && !isHTML)
                {
                    n = n + 1;
                    //UNICODE码字符占两个字节
                    if (System.Text.Encoding.Default.GetBytes(temp + "").Length > 1)
                    {
                        n = n + 1;
                    }
                }

                result.Append(temp);
                if (n >= nlen)
                {
                    break;
                }
            }
            if (nType == 1)
            {
                result.Append("...");
            }

            //取出截取字符串中的HTML标记
            string temp_result = Regex.Replace(result.ToString(), "(>)[^<>]*(<?)", "$1$2");
            //去掉不需要结素标记的HTML标记
            temp_result = Regex.Replace(temp_result, @"</?(img|area|base|basefont|body|br|col|colgroup|dd|dt|frame|head|hr|html|img|input|isindex|li|link|meta|option|p|sText|tbody|td|tfoot|th|thead|tr)[^<>]*/?>", "");
            //去掉成对的HTML标记
            temp_result = Regex.Replace(temp_result, @"<([a-zA-Z]+)[^<>]*>(.*?)</\1>", "$2");
            //用正则表达式取出标记
            Pattern = ("<([a-zA-Z]+)[^<>]*>");
            m = Regex.Matches(temp_result, Pattern);
            ArrayList endHTML = new ArrayList();

            foreach (Match mt in m)
            {
                endHTML.Add(mt.Result("$1"));
            }
            //补全不成对的HTML标记
            for (int i = endHTML.Count - 1; i >= 0; i--)
            {
                result.Append("</");
                result.Append(endHTML[i]);
                result.Append(">");
            }
            return result.ToString();
        }

        /// <summary>
        /// 截取字符
        /// </summary>
        /// <param name="sText">处理的字符串</param>
        /// <param name="nlen">保留字节数</param>
        /// <param name="nType">类型 0:不显示... 1:显示...</param>
        public static string StringMaxlong(string sText, int nlen, int nType)
        {
            int p_num = 0;
            int i;
            string New_sText = "";

            if (sText == "")
            {
                New_sText = "";
            }
            else
            {
                int Len_Num = sText.Length;
                for (i = 0; i <= Len_Num - 1; i++)
                {
                    //sText.Substring(i,1);
                    if (i > Len_Num) break;
                    char c = Convert.ToChar(sText.Substring(i, 1));
                    if (((int)c > 255) || ((int)c < 0))
                        p_num = p_num + 2;
                    else
                        p_num = p_num + 1;
                    if (p_num >= nlen)
                    {
                        New_sText = sText.Substring(0, i + 1);
                        if (nType == 1)
                            New_sText += "...";
                        break;
                    }
                    else
                    {
                        New_sText = sText;
                    }
                }
            }
            return New_sText;
        }

        /// <summary>
        /// 全角字符转换为半角字符
        /// </summary>
        public static string ConvertStr(string str)
        {
            if (str != null)
            {
                char[] ns = str.ToCharArray();
                for (int i = 0; i < ns.Length; i++)
                {
                    if (ns[i] > 65280 && ns[i] < 65375)
                        ns[i] = Convert.ToChar(Convert.ToInt32(ns[i]) - 65248);
                }
                return new string(ns).Trim();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// html编辑器过滤格式
        /// </summary>
        /// <param name="html">处理的字符串</param>
        /// <param name="filter">过滤开关</param>
        public static string HtmlDecodeFilter(string html, string filter)
        {
            switch (filter.ToUpper())
            {
                case "SCRIPT":
                    html = Regex.Replace(html, @"</?script[^>]*>", "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, @"(javascript|jscript|vbscript|vbs):", "$1：", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, @"on(mouse|exit|error|click|dblclick|key)", "<I>on$1</I>", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, @"&#", "<I>&#</I>", RegexOptions.IgnoreCase);
                    break;
                case "XML":
                    html = Regex.Replace(html, @"<\?xml[^>]*>", "", RegexOptions.IgnoreCase);
                    break;
                case "NAMESPACE":
                    html = Regex.Replace(html, @"<\/?[a-z]+:[^>]*>", "", RegexOptions.IgnoreCase);
                    break;
                case "MARQUEE":
                    html = Regex.Replace(html, @"</?marquee[^>]*>", "", RegexOptions.IgnoreCase);
                    break;
                case "OBJECT":
                    html = Regex.Replace(html, @"</?object[^>]*>", "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, @"</?param[^>]*>", "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, @"</?embed[^>]*>", "", RegexOptions.IgnoreCase);
                    break;
                case "FORM":
                    html = Regex.Replace(html, @"</?form[^>]*>", "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, @"</?input[^>]*>", "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, @"</?select[^>]*>", "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, @"</?option[^>]*>", "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, @"</?textarea[^>]*>", "", RegexOptions.IgnoreCase);
                    break;
            }
            return html;
        }

        #endregion

        #region 加密和解密

        //默认密钥向量
        private static byte[] DesVector = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="sSource">待加密的字符串</param>
        /// <param name="sSaltKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返空串</returns>
        public static string EncryptDES(string sSource, string sSaltKey)
        {
            try
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(sSource);
                DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();
                descsp.Key = Encoding.UTF8.GetBytes(sSaltKey.Substring(0, 8));
                descsp.IV = DesVector;
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, descsp.CreateEncryptor(), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                // return Convert.ToBase64String(mStream.ToArray());
                StringBuilder sResult = new StringBuilder();
                foreach (byte btHex in mStream.ToArray())
                    sResult.AppendFormat("{0:X2}", btHex);    // Format as hex
                return sResult.ToString();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="sSource">待解密的字符串</param>
        /// <param name="sSaltKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返空串</returns>
        public static string DecryptDES(string sSource, string sSaltKey)
        {
            try
            {
                byte[] inputByteArray = new byte[sSource.Length / 2];
                for (int x = 0; x < sSource.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(sSource.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                // byte[] inputByteArray = Convert.FromBase64String(sSource);
                DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();
                descsp.Key = Encoding.UTF8.GetBytes(sSaltKey.Trim());
                descsp.IV = DesVector;
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, descsp.CreateDecryptor(), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                // return Encoding.UTF8.GetString(mStream.ToArray());
                return System.Text.Encoding.Default.GetString(mStream.ToArray());
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 语言文化

        /// <summary>
        /// 语言代码转换
        /// </summary>
        /// <param name="nCulture">LCID，如2052</param>
        /// <returns>字符串，如zh-CN</returns>
        public static string GetCulture(int nCulture)
        {
            CultureInfo culture = new CultureInfo(nCulture);
            return culture.Name;
        }

        /// <summary>
        /// 语言代码转换
        /// </summary>
        /// <param name="sCultureName">字符串，如zh-CN</param>
        /// <returns>LCID，如2052</returns>
        public static int GetCulture(string sCultureName)
        {
            CultureInfo oCulture = new CultureInfo(sCultureName);
            return oCulture.LCID;
        }

        #endregion

    }
}
