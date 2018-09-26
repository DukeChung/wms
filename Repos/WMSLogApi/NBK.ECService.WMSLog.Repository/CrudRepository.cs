using Abp.EntityFramework;
using Abp.EntityFramework.SimpleRepositories;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Model;
using NBK.ECService.WMSLog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Repository
{
    public class CrudRepository : EfSimpleRepositoryBase<NBK_WMS_CheckStore, Guid>, ICrudRepository
    {
        public CrudRepository(IDbContextProvider<NBK_WMS_CheckStore> dbContextProvider) : base(dbContextProvider) { }

        public void ChangeDB(int type)
        {
            this.Context.ChangeDB(type);
        }

    }
}
