using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeDetailViewDto
    {
        public Guid SysId { get; set; }

        public Guid StockTakeSysId { get; set; }

        public Guid SkuSysId { get; set; }

        public string Loc { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string SkuUPC { get; set; }

        public string UOMCode { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        /// <summary>
        /// 初盘数量
        /// </summary>
        public int StockTakeQty { get; set; }

        public decimal DisplayStockTakeQty { get; set; }

        /// <summary>
        /// 复盘数量
        /// </summary>
        public int? ReplayQty { get; set; }

        public decimal? DisplayReplayQty { get; set; }

        ///// <summary>
        ///// 盘点数量(盘点差异显示)
        ///// </summary>
        //public int? InventoryQty
        //{
        //    get
        //    {
        //        if(Status == (int)StockTakeDetailStatus.StockTake)
        //        {
        //            return StockTakeQty;
        //        }
        //        if(Status == (int)StockTakeDetailStatus.Replay)
        //        {
        //            return ReplayQty;
        //        }
        //        return 0;
        //    }
        //}

        public string Remark { get; set; }

        public int? Status { get; set; }

        public string StatusText { get { return Status.HasValue ? ((StockTakeDetailStatus)Status.Value).ToDescription() : string.Empty; } }

        public DateTime CreateDate { get; set; }
    }
}
