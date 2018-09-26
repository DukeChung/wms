using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.ThirdParty;
using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.DTO.ThirdParty.OutboundReturn;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IThirdPartyAppService : IApplicationService
    {
        /// <summary>
        /// 插入/更新SKU
        /// </summary>
        /// <param name="skuDto"></param>
        /// <returns></returns>
        CommonResponse InsertOrUpdateSku(ThirdPartySkuDto skuDto);

        /// <summary>
        /// 插入/更新包装
        /// </summary>
        /// <param name="skuPack"></param>
        /// <returns></returns>
        CommonResponse InsertOrUpdateSkuPack(ThirdPartySkuPackDto skuPack);

        /// <summary>
        /// 插入/更新SKU分类
        /// </summary>
        /// <param name="skuClassDto"></param>
        /// <returns></returns>
        CommonResponse InsertOrUpdateSkuClass(ThirdPartySkuClassDto skuClassDto);

        /// <summary>
        /// 插入采购单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        CommonResponse InsertPurchase(ThirdPartyPurchaseDto purchaseDto);

        /// <summary>
        /// 插入订单
        /// </summary>
        /// <param name="outboundDto"></param>
        /// <returns></returns>
        CommonResponse InsertOutbound(ThirdPartyOutboundDto outboundDto);

        /// <summary>
        /// 商城入库接口
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="currentDisplayName"></param>
        /// <param name="sysId"></param>
        /// <param name="purchaseDetailDtoList">本次收货明细</param> 
        /// <returns></returns>
        CommonResponse InsertInStock(int currentUserId, string currentDisplayName, Guid sysId, List<PurchaseDetailDto> purchaseDetailDtoList = null);

        /// <summary>
        /// 商城出库接口
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        CommonResponse InsertOutStock(Guid sysId, string currentUserName, int currentUserId);

        /// <summary>
        /// 关闭出库单
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentUserName"></param>
        /// <returns></returns>
        CommonResponse VoidOutbound(string orderCode, string wareHouseId, int currentUserId, string currentUserName);

        CommonResponse InsertWareHouse(ThirdPartyWareHouseDto thirdPartyWareHouseDto);

        /// <summary>
        /// 组装单
        /// </summary>
        /// <param name="assemblyDto"></param>
        /// <returns></returns>
        CommonResponse InsertAssembly(ThirdPartyAssemblyDto assemblyDto);

        /// <summary>
        /// 回写ECC加工单
        /// </summary>
        /// <param name="assemblyWriteBack"></param>
        /// <returns></returns>
        CommonResponse WriteBackECCAssembly(assembly assembly, int currentUserId, string currentDisplayName);

        /// <summary>
        /// 回写ECC加工单领导
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentDisplayName"></param>
        /// <returns></returns>
        CommonResponse WriteBackECCModifyAssemblyStatus(assembly assembly, int currentUserId, string currentDisplayName);

        /// <summary>
        /// 关闭采购单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        CommonResponse ClosePurchase(ThirdPartyPurchaseOperateDto purchaseDto);

        /// <summary>
        /// 创建移仓单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CommonResponse CreateTransferInventory(ThirdPartTransferInventoryDto request);

        /// <summary>
        /// 移仓单出库完成调用ECC接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CommonResponse WriteBackTransferInventoryByOutbound(transferinventory transferInv);

        /// <summary>
        /// 移仓单入库调用ECC接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CommonResponse WriteBackTransferInventoryByInbound(transferinventory transferInv, List<PurchaseDetailViewDto> PurchaseDetailViewDto);

        /// <summary>
        /// ECC增加损益单
        /// </summary>
        /// <param name="adjust"></param>
        /// <returns></returns>
        CommonResponse InsertAdjustment(adjustment adjust);

        /// <summary>
        /// 通过名称查询地区列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Dictionary<string, string> GetRegionListByName(string name);

        /// <summary>
        /// 获取详细地址
        /// </summary>
        /// <param name="regionSysId"></param>
        /// <returns></returns>
        string GetRegionIntactBySysId(Guid regionSysId);

        /// <summary>
        /// 库存转移
        /// </summary>
        /// <param name="stockTransferDto"></param>
        /// <returns></returns>
        CommonResponse InsertStockTransfer(ThirdPartyStockTransferDto stockTransferDto);

        List<ThirdPartyStockTransferDto> GetInitChannelInventoryData(InitInventoryFromChannelRequest request);

        /// <summary>
        /// 退货入库 通知ecc 创建入库单，并返回入库单号
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="currentDisplayName"></param>
        /// <param name="ExternOrderId"></param>
        /// <param name="purchase"></param>
        /// <returns></returns>
        CreatePurchaseOrderNumberResponse CreatePurchaseOrderNumber(ECCReturnOrder trdAPIRequest, int currentUserId, string currentDisplayName,  Guid purchaseSysId);

        /// <summary>
        /// 质检完成通知ECC
        /// </summary>
        /// <param name="finishQualityControlDto"></param>
        /// <param name="qualityControl"></param>
        /// <returns></returns>
        CommonResponse FinishQualityControl(ThirdPartyFinishQualityControlDto finishQualityControlDto, qualitycontrol qualityControl);

        /// <summary>
        /// 向TMS推送箱号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        CommonResponse PreBullPackSendToTMS(ThirdPreBullPackDto dto);

        /// <summary>
        ///  TMS调用：装车顺序
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CommonResponse InsertLoadingSequence(ThirdPartyLoadingSequenceDto request);
        CommonResponse InsertTMSOrder(ThirdPartyLoadingSequenceDto request);

        /// <summary>
        /// 调用TMS：出库单状态变更通知TMS
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        CommonResponse UpdateOutboundTypeToTMS(ThirdPartyUpdateOutboundTypeDto dto);

        /// <summary>
        /// 给TMS推送出库单总箱数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        CommonResponse PushBoxCount(ThirdPartyPushBoxCountDto dto); 

        CommonResponse InsertSkuBorrow(ThirdPartySkuBorrowAddDto dto);

        CommonResponse PushLendInfoToECC(ThirdPartySkuBorrowLendDto dto);

        CommonResponse PushReturnInfoToECC(ThirdPartySkuBorrowReturnDto dto);

        void PushLockOrderToECCSync(List<LockOrderInput> request, int userId, string userName);

        CommonResponse InsertReturnPurchase(ThirdPartyReturnPurchaseDto request);

        CommonResponse UpdateOutboundByReturnPurchase(ThirdPartyUpdateOutboundDto thirdPartyUpdateOutboundDto);
    }
}