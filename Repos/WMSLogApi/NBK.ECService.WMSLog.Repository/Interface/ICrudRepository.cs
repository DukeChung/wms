using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Repository.Interface
{
    public interface ICrudRepository : ISimpleRepository<Guid>
    {
        void ChangeDB(int type);
    }
}
