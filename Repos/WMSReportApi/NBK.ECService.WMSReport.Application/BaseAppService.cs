using NBK.ECService.WMSReport.Application.Interface;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Application
{
    public class BaseAppService : IBaseAppService
    {
        /// <summary>
        /// 获取经纬度
        /// </summary>
        /// <param name="city">城市名称 可空</param>
        /// <param name="address">详细地址 不可空</param>
        /// <returns></returns>
        public CoordinateDto GetCoordinate(string city, string address)
        {
            #region 获取经纬度
            string apiUrl = PublicConst.GeocoderURL;

            var allAddress = city + address;
            if (allAddress.Length > 42)
            {   //由于百度地图支持地址长度为42个字，所以截取地址内容到42个字
                allAddress = allAddress.Substring(0, 42);
            }
            allAddress = allAddress.Replace('#', '号');

            apiUrl += "?address=" + allAddress + "&output=json" + "&ak=" + PublicConst.GeocoderAK;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpRequest.Timeout = 2000;
            httpRequest.Method = "GET";
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
            string result = sr.ReadToEnd();
            result = result.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            int status = (int)httpResponse.StatusCode;
            sr.Close();

            return result.JsonToDto<CoordinateDto>();
            #endregion
        }
    }
}
