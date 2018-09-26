using System;
using WMSBussinessApi.Service;
using WMSBussinessApi.Repository;
using WMSBussinessApi.Dto.XXX;
using System.Collections.Generic;
using System.Linq;
using WMSBussinessApi.Utility.MQ;

namespace WMSBussinessApi.ServiceImp
{
    public class XXXSvc : IXXXSvc
    {
        private IXXXRep _xxxRep;
        public XXXSvc(IXXXRep xxxRep)
        {
            _xxxRep = xxxRep;
        }

        public Report1OutDto GetReport1(Report1InDto request)
        {
            List<Report1Item> allItems = new List<Report1Item>();
            allItems.Add(new Report1Item() { Date = "2016-05-01", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-05-02", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-05-03", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-05-04", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-05-05", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-05-06", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-05-07", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-05-08", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-05-09", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-05-10", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });

            allItems.Add(new Report1Item() { Date = "2016-06-01", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-06-02", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-06-03", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-06-04", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-06-05", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-06-06", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-06-07", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-06-08", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-06-09", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-06-10", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });

            allItems.Add(new Report1Item() { Date = "2016-07-01", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-07-02", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-07-03", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-07-04", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-07-05", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-07-06", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-07-07", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-07-08", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-07-09", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-07-10", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });

            allItems.Add(new Report1Item() { Date = "2016-08-01", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-08-02", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-08-03", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-08-04", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-08-05", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-08-06", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-08-07", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-08-08", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-08-09", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });
            allItems.Add(new Report1Item() { Date = "2016-08-10", XingMing = "王小虎", Province = "上海", City = "普陀区", Address = "上海市普陀区金沙江路 1518 弄", Zip = 200333 });

            Report1OutDto result = new Report1OutDto() { ReportData = new List<Report1Item>() };

            var reportData = allItems.Skip((request.CurrentPage - 1) * request.PageSize).Take(request.PageSize);
            result.ReportData = reportData.ToList();

            result.ResultTotal = allItems.Count;
            return result;
        }

        public Report2OutDto GetReport2(Report2InDto request)
        {
            List<Report2Item> allItems = new List<Report2Item>();
            allItems.Add(new Report2Item() { Date = "2016-05-01", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-05-02", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-05-03", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-05-04", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-05-05", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-05-06", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-05-07", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-05-08", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-05-09", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-05-10", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });

            allItems.Add(new Report2Item() { Date = "2016-06-01", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-06-02", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-06-03", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-06-04", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-06-05", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-06-06", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-06-07", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-06-08", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-06-09", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-06-10", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });

            allItems.Add(new Report2Item() { Date = "2016-07-01", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-07-02", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-07-03", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-07-04", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-07-05", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-07-06", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-07-07", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-07-08", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-07-09", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-07-10", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });

            allItems.Add(new Report2Item() { Date = "2016-08-01", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-08-02", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-08-03", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-08-04", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-08-05", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-08-06", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-08-07", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-08-08", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-08-09", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });
            allItems.Add(new Report2Item() { Date = "2016-08-10", XingMing = "JelaxWang", Province = "陕西", City = "高新区", Address = "陕西省西安市高新区研祥城市广场", Zip = 200333 });

            Report2OutDto result = new Report2OutDto() { ReportData = new List<Report2Item>() };

            var reportData = allItems.Skip((request.PageInfo.CurrentPage - 1) * request.PageInfo.PageSize).Take(request.PageInfo.PageSize);
            result.ReportData = reportData.ToList();

            result.ResultTotal = allItems.Count;
            return result;
        }

        public int MyOp(Model.XXX.XC xc)
        {
            return _xxxRep.MyOp(xc);
        }
        public int DbCeshi()
        {
            return _xxxRep.DbCeshi();
        }

        public int FSXXCeshi(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                MQProducer producer = new MQProducer("10.66.150.102", 15672, "ecc_admin", "setpay@123");
                producer.SendMsgToDExchange(msg, "jelax.dexchange");
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
