using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FortuneLab
{
    public static class StringExtention
    {
        public static string[] GetSubString(this string thisValue, string regexPattern)
        {
            string[] result = null;
            MatchCollection collection = Regex.Matches(thisValue, regexPattern, RegexOptions.IgnoreCase);
            if (collection.Count > 0)
            {
                result = new string[collection.Count];
                for (int i = 0; i < collection.Count; i++)
                {

                    result[i] = collection[i].Value;
                }
            }
            return result;
        }

        public static string ReplaceString(this string thisValue, string regexPattern, string replacement)
        {
            if (string.IsNullOrWhiteSpace(thisValue))
                return thisValue;

            return Regex.Replace(thisValue, regexPattern, replacement, RegexOptions.IgnoreCase);
        }

        public static string GetLimitString(this string thisValue, int frontLength = 512, int endLength = 512)
        {
            if (string.IsNullOrWhiteSpace(thisValue))
                return thisValue;

            StringBuilder sbString = new StringBuilder(frontLength + endLength + 50);
            if (thisValue.Length > (frontLength + endLength))
            {
                sbString.Append(thisValue.Substring(0, frontLength));
                sbString.AppendFormat("--trunked data(size:{0})--", thisValue.Length - frontLength - endLength);
                sbString.Append(thisValue.Substring(thisValue.Length - endLength));
            }
            return sbString.ToString();
        }

        public static Regex CommonIdsValidationRegex = new Regex(@"^((\d?)|(([-+]?\d+\.?\d*)|([-+]?\d*\.?\d+))|(([-+]?\d+\.?\d*\,\ ?)*([-+]?\d+\.?\d*))|(([-+]?\d*\.?\d+\,\ ?)*([-+]?\d*\.?\d+))|(([-+]?\d+\.?\d*\,\ ?)*([-+]?\d*\.?\d+))|(([-+]?\d*\.?\d+\,\ ?)*([-+]?\d+\.?\d*)))$");
        public static string CommaIdsValidation(this string obj)
        {
            if (!CommonIdsValidationRegex.IsMatch(obj))
            {
                throw new Exception("Wrong format of Comma Sparated string");
            }
            return obj;
        }
    }
}
