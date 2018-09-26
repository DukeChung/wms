using Schubert.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMSBussinessApi.Dto.DataSync;

namespace WMSBussinessApi.Service
{
    public interface IDataSyncSvc : IDependency
    {
        string Test();

        void SyncCreateSku(SkuDto sku);

        void SyncUpdateSku(SkuDto sku);

        void SyncCreatePack(PackDto pack);

        void SyncUpdatePack(PackDto pack);

        void SyncDeletePack(List<Guid> sysIdList);

        void SyncCreateSyscode(SyscodeDto syscode);

        void SyncCreateSyscodeDetail(SyscodeDetailDto syscodedetail);
    }
}
