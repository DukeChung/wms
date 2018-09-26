using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IRFPickDetailAppService
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

        /// <summary>
        /// 检查商品是否存在于出库单明细中
        /// </summary>
        /// <returns></returns>
        RFCommResult CheckOutboundDetailSku(RFPickDetailDto rFPickDetailDto);

        /// <summary>
        /// 扫描拣货
        /// </summary>
        /// <param name="pickDetailDto"></param>
        /// <returns></returns>
        RFCommResult ScanPickDetail(RFPickDetailDto pickDetailDto);

        /// <summary>
        /// 判断扫描的单号是否待容器拣货
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        RFCommResult CheckContainerPickingOutboundOrder(RFOutboundQuery outboundQuery);

        /// <summary>
        /// 判断容器是否可用
        /// </summary>
        /// <param name="storageCase"></param>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        RFCommResult CheckContainerIsAvailable(string storageCase, string outboundOrder, Guid warehouseSysId);

        /// <summary>
        /// 容器拣货扫描
        /// </summary>
        /// <param name="pickingDetailDto"></param>
        /// <returns></returns>
        RFCommResult GenerateContainerPickingDetail(RFGenerateContainerPickingDetailDto pickingDetailDto);

        /// <summary>
        /// RF拣货记录缓存
        /// </summary>
        /// <param name="setRedisDto"></param>
        /// <returns></returns>
        RFCommResult RFSetPickingRedis(RFPickFinishDto setRedisDto);

        /// <summary>
        /// RF拣货完成
        /// </summary>
        /// <param name="pickFinishDto"></param>
        /// <returns></returns>
        RFCommResult RFPickFinish(RFPickFinishDto pickFinishDto);

        /// <summary>
        /// 拣货完成结果
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        RFPickResultDto GetPickResult(RFPickQuery pickQuery);

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

        /// <summary>
        /// 检查商品是否存在于加工单明细中
        /// </summary>
        /// <returns></returns>
        RFCommResult CheckAssemblyDetailSku(RFAssemblyPickDetailDto rfAssemblyPickDetailDto);

        /// <summary>
        /// 加工单扫描拣货
        /// </summary>
        /// <param name="pickDetailDto"></param>
        /// <returns></returns>
        RFCommResult AssemblyScanPickDetail(RFAssemblyPickDetailDto rfAssemblyPickDetailDto);
        #endregion
    }
}
