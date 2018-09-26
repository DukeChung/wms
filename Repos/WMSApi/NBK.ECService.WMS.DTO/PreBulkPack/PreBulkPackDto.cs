using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PreBulkPackDto : BaseQuery
    {
        public Guid SysId { get; set; }

        // public Guid WarehouseSysId { get; set; }

        public string PreBulkPackOrder { get; set; }

        public int Status { get; set; }

        public string StatusName
        {
            get
            {
                return ((PreBulkPackStatus)Status).ToDescription();
            }
        }

        /// <summary>
        /// 箱号
        /// </summary>
        public string StorageCase { get; set; }

        public string CreateUserName { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateDateDisplay
        {
            get
            {
                return CreateDate.ToString(PublicConst.DateFormat);
            }
        }

        public List<PreBulkPackDetailDto> PreBulkPackDetailList { get; set; } = new List<PreBulkPackDetailDto>();

        public Guid? OutboundSysId { get; set; }
        public string OutboundOrder { get; set; }
        public string ImportSysIds { get; set; }
    }

    public class PreBulkPackDetailDto
    {
        public Guid SysId { get; set; }

        public Guid PreBulkPackSysId { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public int Qty { get; set; }

        public string Loc { get; set; }

        public string Lot { get; set; }

        public string UomCode { get; set; }

        private string _packCode;
        public string PackCode
        {
            get
            {
                return _packCode;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _packCode = value.Replace("\t", "").Replace("\n", "").Replace("\r", "");
                }
                else { _packCode = ""; }
            }
        }

        /// <summary>
        /// 导入计划数量
        /// </summary>
        public int? PreQty { get; set; }
        /// <summary>
        /// 导入商品外部Id
        /// </summary>
        public string OtherId { get; set; }

        public string SkuDescr { get; set; }
    }
}
