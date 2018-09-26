using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IWorkMangerRepository
    {
        Pages<WorkListDto> GetWorkByPage(WorkQueryDto request);
    }
}
