using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO.ThirdParty.ZTO;
using Newtonsoft.Json;
using System.Collections.Specialized;
using NBK.ECService.WMS.DTO.ThirdParty;

namespace NBK.ECService.WMS.Application
{
    public class ZTOAppService : IZTOAppService
    {
        /// <summary>
        /// 生成订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public dynamic OrderSubmit(CreateZTOOrderRequest request)
        {
            string dateStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //请求主信息
            string contentBase = StringHelper.Strtobase(JsonConvert.SerializeObject(request)); 
            //请求验证信息 
            string verify = StringHelper.GetMd5(PublicConst.ZTOUserName + dateStr + contentBase + PublicConst.ZTOPassword, "UTF-8");

            NameValueCollection postValues = new NameValueCollection();
            postValues.Add("style", "json");
            postValues.Add("func", PublicConst.ZTOOrderSubmit);
            postValues.Add("partner", PublicConst.ZTOUserName);
            postValues.Add("datetime", dateStr);
            postValues.Add("content", contentBase);
            postValues.Add("verify", verify);
            string returnMases = HttpHelper.CreatePostHttpResponse(PublicConst.ZTOApi, postValues);

            var response = JsonConvert.DeserializeObject<ZTOOrderResponse>(returnMases);

            if (response == null)
            {
                return null;
            } 
            if (response.result == true)
            {
                return JsonConvert.DeserializeObject<CreateZTOOrderResponse>(returnMases);
            }
            else
            {
                return JsonConvert.DeserializeObject<CreateZTOOrderResponseFail>(returnMases);
            }
        }


        /// <summary>
        /// 2017-09-18
        /// 中通新接口
        /// 获取集包地 和 大头笔
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GenerateZTOOrderMarkeResponse OrderMarke(GenerateZTOOrderMarkeRequest request)
        {
            GenerateZTOOrderMarkeResponse response = new GenerateZTOOrderMarkeResponse();
            //// -- 合作商编码（自己的编码） ---
            //string company_id = "fcf3291d08c047649cde0dd0c0bade96";
            //// -- 需要调用的接口 ---
            //string msg_type = "BAGADDRMARK_GETMARK";
            //// -- 业务参数 这个查看具体的参数 （http://zop.zto.com/tool）---
            //string data = JsonConvert.SerializeObject(request);
            //// -- 分配的合作商编码对应的KEY（自己的KEY） ---
            //string key = "0d6bbbb933c7"; 

            string data = JsonConvert.SerializeObject(request);
            var nvc = new NameValueCollection();
            nvc.Add("company_id", PublicConst.ZTOMarkeCompanyID);
            nvc.Add("msg_type", PublicConst.ZTOMarke);
            nvc.Add("data", data);
            nvc.Add("data_digest", StringHelper.EncryptMD5Base64(data + PublicConst.ZTOMarkeKey, "UTF-8"));


            string returnMases = HttpHelper.CreatePostHttpResponse(PublicConst.ZTOMarkeAPI, nvc);
            response = JsonConvert.DeserializeObject<GenerateZTOOrderMarkeResponse>(returnMases);
            return response; 
        }


        /// <summary>
        /// 获取电子运单号接口(根据客户的请求返回可用的单号信息)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GenerateZTOOrderSubmitResponse OrderSubmit(GenerateZTOOrderSubmitRequest request)
        {
            GenerateZTOOrderSubmitResponse response = new GenerateZTOOrderSubmitResponse(); 
            string data = JsonConvert.SerializeObject(request); 
            var nvc = new NameValueCollection();
            nvc.Add("company_id", PublicConst.ZTOSubmitCompanyID);
            nvc.Add("msg_type", PublicConst.ZTOSubmit);
            nvc.Add("data", data);
            nvc.Add("data_digest", StringHelper.EncryptMD5Base64(data + PublicConst.ZTOSubmitKey, "UTF-8"));


            string returnMases = HttpHelper.CreatePostHttpResponse(PublicConst.ZTOSubmitAPI, nvc);
            response = JsonConvert.DeserializeObject<GenerateZTOOrderSubmitResponse>(returnMases);
            return response;
        } 

    }
}
