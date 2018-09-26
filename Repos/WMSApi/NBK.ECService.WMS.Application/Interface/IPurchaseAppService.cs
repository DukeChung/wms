using System;
using System.Collections.Generic;
using Abp.Application.Services;
using NBK.ECService.WMS.DTO;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IPurchaseAppService : IApplicationService
    {
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool InsertPurchase(PurchaseDto purchaseDto);

        /// <summary>
        /// 获取一条数据
        /// </summary>
        /// <param name="purchaseSysId"></param>
        /// <returns></returns>
         PurchaseViewDto GetPurchaseViewDtoBySysId(Guid purchaseSysId, Guid warehouseSysId);

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        Pages<PurchaseListDto> GetPurchaseDtoListByPageInfo(PurchaseQuery purchaseQuery);

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="purchaseDtos"></param>
        /// <returns></returns>
        bool BatchInsertPurchase(List<PurchaseDto> purchaseDtos, Guid warehouseSysId);

        /// <summary>
        /// 删除采购订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool CancelPurchase(string orderId, Guid warehouseSysId);
        /// <summary>
        /// 获取采购单 sku UPC IsNULL
        /// </summary>
        /// <param name="purchaseSysId"></param>
        /// <returns></returns>
        List<PurchaseDetailSkuDto> GetPurchaseDetailSkuByUpcIsNull(Guid purchaseSysId, Guid warehouseSysId);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="purchaseDetailSkuDto"></param>
        /// <returns></returns>
        void SavePurchaseDetailSkuStyle(PurchaseDetailSkuDto purchaseDetailSkuDto);

        /// <summary>
        /// 生成采购单并收货
        /// </summary>
        /// <param name="purchaseBatchDto"></param>
        /// <returns></returns>
        bool SaveBatchPurchaseAndReceipt(PurchaseBatchDto purchaseBatchDto);

        /// <summary>
        /// 指定入库批号
        /// </summary>
        /// <param name="sysId"></param>
        void AppointBatchNumber(List<Guid> sysId, string batchNumber, Guid warehouseSysId);

        /// <summary>
        /// 作废采购订单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        bool ObsoletePurchase(PurchaseOperateDto purchaseDto);

        /// <summary>
        /// 关闭采购订单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        bool ClosePurchase(PurchaseOperateDto purchaseDto);

        /// <summary>
        /// 入库单生成质检单
        /// </summary>
        /// <param name="purchaseQcDto"></param>
        /// <returns></returns>
        bool GenerateQcOrderByPurchase(PurchaseQcDto purchaseQcDto);

        /// <summary>
        /// 退货入库，自动收货上架
        /// </summary>
        /// <param name="purchaseDto"></param>
        void AutoShelves(PurchaseOperateDto purchaseDto);

        /// <summary>
        /// 修改业务类型（指定上下行）
        /// </summary>
        /// <param name="sysId"></param>
        bool UpdatePurchaseBusinessTypeBySysId(List<Guid> sysId, string businessType, Guid warehouseSysId);

        void InsertPurchaseAndDetailsByReturnOutbound(PurchaseForReturnDto purchase);
    }
}