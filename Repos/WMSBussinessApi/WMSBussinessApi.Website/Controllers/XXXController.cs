using Microsoft.AspNetCore.Mvc;
using WMSBussinessApi.Service;
using Schubert.Framework.Web.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using WMSBussinessApi.Dto.XXX;
using WMSBussinessApi.Utility.Extend;

namespace WMSBussinessApi.Website.Controllers
{
    [Route("api/[controller]")]
    public class XXXController : SchubertApiController
    {
        private IXXXSvc _xxxSvc;
        public XXXController(IXXXSvc xxxSvc)
        {
            _xxxSvc = xxxSvc;
        }

        [HttpPost("Add")]
        public int MyOp(Dto.XXX.XCInDto request)
        {
            //将request 转换为 xc
            Model.XXX.XC xc = request.JTransformTo<Model.XXX.XC>();
            return _xxxSvc.MyOp(xc);
        }

        [HttpGet("dbceshi")]
        public int DBCeshi()
        {
            return _xxxSvc.DbCeshi();
        }

        //jelax: 为了统一,规定get url都走querystring 模式.所以querystring 中的key 直接写在action的参数中,并且设置默认值 
        //所以url 格式为: localhost:5000/api/xxx/report1?tiaoJian1=wang&tiaoJian2=jelax
        [HttpGet("report1")]
        public Dto.XXX.Report1OutDto GetReport1(string tiaojian1 = "", string tiaojian2 = "", int pageSize = 0, int currentPage = 1)
        {
            return _xxxSvc.GetReport1(new Dto.XXX.Report1InDto() { TiaoJian1 = tiaojian1, TiaoJian2 = tiaojian2, PageSize = pageSize, CurrentPage = currentPage });
        }

        [HttpPost("report2")]
        public Dto.XXX.Report2OutDto GetReport2(Dto.XXX.Report2InDto request)
        {
            if (!this.ModelState.IsValid)
            {
                // string msg = this.ModelState["TiaoJian1"].Errors[0].ErrorMessage;
                var allPropErrorMsgList = this.ModelState.Select(p => new { Prop = p.Key, Msg = string.Join("", p.Value.Errors.Select(x => x.ErrorMessage)) });  // List<(Prop,Msg)>
                string msg = string.Join(System.Environment.NewLine, allPropErrorMsgList.Select(s => s.Prop + s.Msg));
                throw new Exception(msg);
            }
            return _xxxSvc.GetReport2(request);
        }

        [HttpGet("fsxxceshi")]
        public int FSXXCeshi(string msg = "")
        {
            return _xxxSvc.FSXXCeshi(msg);
        }
    }
}
