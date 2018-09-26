using System;
using System.Collections.Generic;
using Abp.Application.Services;
using NBK.ECService.WMS.DTO;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IPreBulkPackAppService : IApplicationService
    {
        /// <summary>
        /// 增加散货预包装
        /// </summary>
        /// <returns></returns>
        bool AddPreBulkPack(BatchPreBulkPackDto batchPreBulkPackDto);

        Pages<PreBulkPackDto> GetPreBulkPackByPage(PreBulkPackQuery request);

        PreBulkPackDto GetPreBulkPackBySysId(Guid sysId, Guid warehouseSysId);

        void UpdatePreBulkPack(PreBulkPackDto request);

        void DeletePrebulkPackSkus(List<Guid> request, Guid warehouseSysId);

        void DeletePrebulkPack(List<Guid> request, Guid warehouseSysId);


        /// <summary>
        /// 散货装导入
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        bool ImportPreBulkPack(PreBulkPackDto dto);

        /// <summary>
        /// 根据出库单ID获取散货封箱单号
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        List<string> GetPrebulkPackStorageCase(Guid outboundSysId, Guid warehouseSysId);
    }
}
