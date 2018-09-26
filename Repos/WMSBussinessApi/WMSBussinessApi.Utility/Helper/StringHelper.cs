using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WMSBussinessApi.Utility
{
    public static class StringHelper
    {
        /// <summary>
        /// 判断时间类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns><c>true</c> 
        public static bool IsDate(this string str)
        {
            try
            {
                DateTime d = DateTime.Parse(str);
                DateTime s = Convert.ToDateTime(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 转时间格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns>DateTime.</returns>
        public static DateTime ToDateTime(this object str)
        {
            if (IsDate(str.ToString()) == true)
            {
                return Convert.ToDateTime(str.ToString());
            }
            return default(DateTime);
        }

        /// <summary>
        /// 判断guid格式
        /// </summary>
        /// <returns></returns>
        public static bool IsGuid(this string str)
        {
            try
            {
                Guid p = Guid.Parse(str);
                Guid g = new Guid(str);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 转guid格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid ToGuid(this object str)
        {
            if (IsGuid(str.ToString()) == true)
            {
                return new Guid(str.ToString());
            }
            return default(Guid);
        }

        /// <summary>
        /// 验证是不是为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNull(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 验证是不是整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string str)
        {
            if (!StringHelper.IsNull(str))
            {
                return Regex.IsMatch(str, @"^-?[0-9]*$", RegexOptions.IgnoreCase);
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 验证内容是否为浮点型
        /// </summary>
        /// <param name="str"></param>
        /// <returns>true</returns>
        public static bool IsFloat(this string str)
        {
            if (!StringHelper.IsNull(str))
            {
                string[] arrStr = str.Split(new char[] { '.' });

                if (arrStr.Length > 2)
                {
                    return false;
                }
                else if (arrStr.Length == 2)
                {
                    if (arrStr[0] == "" || arrStr[1] == "")
                    {
                        return false;
                    }
                    str = arrStr[0] + arrStr[1];
                }
                return IsNumeric(str);
            }
            return false;
        }
        /// <summary>
        /// 将指定的 object 的值转换为decimal数字
        /// </summary>
        /// <param name="str">任意字符串</param>
        /// <returns>System.Decimal.</returns>
        public static decimal ToDecimal(this object str)
        {
            if (StringHelper.IsFloat(str.ToString()) == true)
            {
                return Convert.ToDecimal(str);
            }
            return default(decimal);
        }

        /// <summary>
        /// ids 逗号拼接转换成 Guid List 在删除时候用
        /// </summary>
        /// <returns></returns>
        public static List<Guid> ToGuidList(this string sysIds)
        {
            if (!string.IsNullOrEmpty(sysIds))
            {
                var idList = sysIds.Split(',');
                return (from id in idList where !string.IsNullOrEmpty(id) select Guid.Parse(id)).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///Guid List 转换成逗号拼接  ids  
        /// </summary>
        /// <returns></returns>
        public static string GuidListToIds(this List<Guid> guidList)
        {
            var str = string.Empty;
            if (guidList.Any())
            {
                guidList.ForEach(item =>
                {
                    str += "'" + item + "',";
                });
            }
            return str.Substring(0, str.Length - 1);
        }

        #region 分隔符

        /// <summary>
        /// 分隔符返回list
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strSplit"></param>
        /// <returns></returns>
        private static List<string> GetSeparatorByList(string str, char separator)
        {
            return str.Split(separator).ToList();
        }
        #endregion

        #region 返回集合
        /// <summary>
        /// 返回集合
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] GetSeparatorByArray(string str, char separator)
        {
            return str.Split(separator);
        }
        #endregion


        /// <summary>
        /// 友盟计算MD5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5Hash(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));
            StringBuilder strReturn = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {
                strReturn.Append(Convert.ToString(result[i], 16).PadLeft(2, '0'));
            }

            return strReturn.ToString().PadLeft(32, '0');
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 转换成Sql语句In参数
        /// </summary>
        /// <returns></returns>
        public static string GetConvertSqlIn<T>(List<T> strParams)
        {
            try
            {
                var sysIds = string.Empty;
                foreach (var info in strParams)
                {
                    sysIds += "'" + info + "',";
                }
                sysIds = sysIds.Substring(0, sysIds.Length - 1);
                return sysIds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #region 加解密算法

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static String EncryptionRSA(String plaintext)
        {
            using (RSACryptoServiceProvider RSACryptography = new RSACryptoServiceProvider())
            {
                Byte[] PlaintextData = Encoding.UTF8.GetBytes(plaintext);
                int MaxBlockSize = RSACryptography.KeySize / 8 - 11;    //加密块最大长度限制

                if (PlaintextData.Length <= MaxBlockSize)
                    return Convert.ToBase64String(RSACryptography.Encrypt(PlaintextData, false));

                using (MemoryStream PlaiStream = new MemoryStream(PlaintextData))
                using (MemoryStream CrypStream = new MemoryStream())
                {
                    Byte[] Buffer = new Byte[MaxBlockSize];
                    int BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);

                    while (BlockSize > 0)
                    {
                        Byte[] ToEncrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);

                        Byte[] Cryptograph = RSACryptography.Encrypt(ToEncrypt, false);
                        CrypStream.Write(Cryptograph, 0, Cryptograph.Length);

                        BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                    }

                    return Convert.ToBase64String(CrypStream.ToArray(), Base64FormattingOptions.None);
                }
            }
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        public static String DecryptRSA(String ciphertext)
        {
            using (RSACryptoServiceProvider RSACryptography = new RSACryptoServiceProvider())
            {
                Byte[] CiphertextData = Convert.FromBase64String(ciphertext);
                int MaxBlockSize = RSACryptography.KeySize / 8;    //解密块最大长度限制

                if (CiphertextData.Length <= MaxBlockSize)
                    return Encoding.UTF8.GetString(RSACryptography.Decrypt(CiphertextData, false));

                using (MemoryStream CrypStream = new MemoryStream(CiphertextData))
                using (MemoryStream PlaiStream = new MemoryStream())
                {
                    Byte[] Buffer = new Byte[MaxBlockSize];
                    int BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);

                    while (BlockSize > 0)
                    {
                        Byte[] ToDecrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToDecrypt, 0, BlockSize);

                        Byte[] Plaintext = RSACryptography.Decrypt(ToDecrypt, false);
                        PlaiStream.Write(Plaintext, 0, Plaintext.Length);

                        BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);
                    }

                    return Encoding.UTF8.GetString(PlaiStream.ToArray());
                }
            }
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        public static string EncryptDES(string encryptString, string key = "wms")
        {
            var input = Encoding.UTF8.GetBytes(encryptString);
            var ouptputData = ProcessDES(input, key, true);
            var outputStr = Convert.ToBase64String(ouptputData);

            //base64编码中有不能作为文件名的'/'符号，这里把它替换一下，增强适用范围
            return outputStr.Replace('/', '@');
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="decryptString"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        public static string DecryptDES(string decryptString, string key = "wms")
        {
            decryptString = decryptString.Replace('@', '/');

            var input = Convert.FromBase64String(decryptString);
            var data = ProcessDES(input, key, false);
            return Encoding.UTF8.GetString(data);
        }


        private static byte[] ProcessDES(byte[] data, string key, bool isEncrypt)
        {
            using (var dCSP = new DESCryptoServiceProvider())
            {
                var keyData = Md5(key);
                var rgbKey = new ArraySegment<byte>(keyData, 0, 8).ToArray();
                var rgbIV = new ArraySegment<byte>(keyData, 8, 8).ToArray();
                var dCSPKey = isEncrypt ? dCSP.CreateEncryptor(rgbKey, rgbIV) : dCSP.CreateDecryptor(rgbKey, rgbIV);

                using (var memory = new MemoryStream())
                using (var cStream = new CryptoStream(memory, dCSPKey, CryptoStreamMode.Write))
                {
                    cStream.Write(data, 0, data.Length);
                    cStream.FlushFinalBlock();
                    return memory.ToArray();
                }
            }
        }

        public static byte[] Md5(string str)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            }
        }

        #endregion 
    }
}
