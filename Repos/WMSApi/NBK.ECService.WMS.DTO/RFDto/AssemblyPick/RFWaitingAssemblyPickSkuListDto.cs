using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFWaitingAssemblyPickSkuListDto
    {
        /// <summary>
        /// 加工单主键
        /// </summary>
        public Guid? AssemblySysId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        /// <summary>
        /// 待拣货商品数量
        /// </summary>
        public int WaitPickQty { get; set; }

        public decimal DisplayWaitPickQty { get; set; }

        public string UPC01 { get; set; }

        public string UPC02 { get; set; }

        public string UPC03 { get; set; }

        public string UPC04 { get; set; }

        public string UPC05 { get; set; }

        public int? FieldValue01 { get; set; }

        public int? FieldValue02 { get; set; }

        public int? FieldValue03 { get; set; }

        public int? FieldValue04 { get; set; }

        public int? FieldValue05 { get; set; }
    }
}
