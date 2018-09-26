using Schubert.Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


//所有数据库与Mode 的mapping 关系都写在这里

namespace WMSBussinessApi.RepositoryImp
{
    public class EmployeeMapping : DapperMetadataProvider<Model.Employee>
    {
        protected override void ConfigureModel(DapperMetadataBuilder<Model.Employee> builder)
        {
            builder.TableName("employee");
            builder.HasKey(s => new { s.SysId });//主键
        }
    }
    /*
    public class YCMapping : DapperMetadataProvider<Model.XXX.YC>
    {
        protected override void ConfigureModel(DapperMetadataBuilder<Model.XXX.YC> builder)
        {
            builder.TableName("yc");
            builder.HasKey(s => new { s.Id });//主键
        }
    }
    */

}
