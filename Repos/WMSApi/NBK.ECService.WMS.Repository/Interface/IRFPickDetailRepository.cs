using System;
using System.Collections.Generic;
using NBK.ECService.WMS.DTO;
using System.Linq;
using NBK.ECService.WMS.Model.Models;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IRFPickDetailRepository : ICrudRepository
    {
        /// <summary>
        /// 获取待拣货的出库单
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        Pages<RFWaitingPickListDto> GetWaitingPickOutboundList(RFPickQuery pickQuery);

        /// <summary>
        /// 获取待容器拣货的出库单
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        Pages<RFContainerPickingListDto> GetWaitingContainerPickingListByPaging(RFPickQuery pickQuery);

        /// <summary>
        /// 获取某个出库单的待拣货商品
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        List<RFWaitingPickSkuListDto> GetWaitingPickSkuList(RFPickQuery pickQuery);

        /// <summary>
        /// 获取出库单容器拣货明细
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        RFContainerPickingDto GetContainerPickingDetailList(RFPickQuery pickQuery);

        #region 加工单拣货
        /// <summary>
        /// 获取待拣货的加工单
        /// </summary>
        /// <param name="assemblyPickQuery"></param>
        /// <returns></returns>
        Pages<RFWaitingAssemblyPickListDto> GetWaitingAssemblyList(RFAssemblyPickQuery assemblyPickQuery);

        /// <summary>
        /// 获取加工单中的待拣货商品
        /// </summary>
        /// <param name="assemblyPickQuery"></param>
        /// <returns></returns>
        List<RFWaitingAssemblyPickSkuListDto> GetWaitingAssemblyPickSkuList(RFAssemblyPickQuery assemblyPickQuery);
        #endregion

        /// <summary>
        /// 加工拣货时根据规则获取库存记录
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="skuSysId"></param>
        /// <param name="wareHouseSysId"></param>
        /// <param name="channel"></param>
        /// <param name="assemblyrule"></param>
        /// <returns></returns>
        IQueryable<FRInvLotLocLpnListDto> GetInvlotloclpnList(string loc, Guid skuSysId, Guid wareHouseSysId, string channel, assemblyrule assemblyrule);
    }
}
