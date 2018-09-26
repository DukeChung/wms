using System;
using System.Collections.Generic;
using NBK.ECService.WMS.DTO;
using System.Linq;
using NBK.ECService.WMS.Model.Models;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IReceiptRepository : ICrudRepository
    {
        /// <summary>
        /// 根据ReceiptSysId 获取 入库明细信息
        /// </summary>
        /// <param name="sysId">主表SysId</param>
        /// <returns></returns>
        List<ReceiptDetailOperationDto> GetReceiptDetailOperationDtoBySysId(Guid sysId);

        /// <summary>
        /// 获取收货单列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        Pages<ReceiptListDto> GetReceiptListByPaging(ReceiptQuery receiptQuery);

        /// <summary>
        /// 根据Id获取收货单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        ReceiptViewDto GetReceiptViewById(Guid sysId);

        /// <summary>
        /// 获取收货清单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        List<ReceiptDetailViewDto> GetReceiptDetailViewList(Guid receiptSysId);

        List<ReceiptDetailCheckLotDto> GetToLotByReceiptDetail(List<receiptdetail> receiptDetails, Guid wareHouseSysId);

        /// <summary>
        /// TODO于17.9.28号发现此方法暂无使用所以注释掉
        /// </summary>
        /// <param name="receipt"></param>
        //void CreateReceipt(receipt receipt);

        /// <summary>
        /// 获取领料记录
        /// </summary>
        /// <param name="pickingQuery"></param>
        /// <returns></returns>
        Pages<PickingMaterialListDto> GetPickingMaterialList(PickingMaterialQuery pickingQuery);

        void CancelReceiptsnByPurchase(Guid purchaseSysId);

        void BatchInsertReceiptSN(List<receiptsn> snlist);

        #region 批次采集
        /// <summary>
        /// 批次采集根据商品获取收货明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        List<ReceiptDetailViewDto> GetReceiptDetailCollectionLotViewList(ReceiptCollectionLotQuery request);
        #endregion
    }
}