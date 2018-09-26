using Abp.EntityFramework;
using NBK.ECService.WMSLog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Model
{
    public partial class NBK_WMS_CheckStore : AbpDbContext
    {
        public NBK_WMS_CheckStore() : base("nbk_wms_checkContext") { }

        public void ChangeDB(int type)
        {
            var connectionString = string.Empty;
            switch (type)
            {
                case 1:
                    connectionString = PublicConst.ConnectionString1;
                    break;
                case 2:
                    connectionString = PublicConst.ConnectionString2;
                    break;
                case 3:
                    connectionString = PublicConst.ConnectionString3;
                    break;
            }
            if (this.Database.Connection.ConnectionString != connectionString)
            {
                this.Database.Connection.ConnectionString = connectionString;
            }

        }
    }
}
