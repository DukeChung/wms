using Schubert.Framework;
using System.Collections.Generic;
using WMSBussinessApi.Dto.XXX;

namespace WMSBussinessApi.Service
{
    public interface IXXXSvc : IDependency
    {
        int MyOp(Model.XXX.XC xc);
        int DbCeshi();
        Report1OutDto GetReport1(Report1InDto request);
        Report2OutDto GetReport2(Report2InDto request);
        int FSXXCeshi(string msg);
    }
}
