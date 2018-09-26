using System;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class SysCodeDetailDto:BaseDto
    {
        public Guid? SysId { get; set; }
        public Guid? SysCodeSysId { get; set; }
        public string SeqNo { get; set; }
        public string Code { get; set; }
        public string Descr { get; set; }
        public long CreateBy { get; set; }
        public string CreateUserName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
        public bool IsActive { get; set; }

        public string IsActiveText
        {
            get
            {
                if (IsActive)
                {
                    return PublicConst.IsActiveTrue;
                }
                else
                {
                    return PublicConst.IsActiveFalse;
                }
            }
        }

        public string UpdateText
        {
            get
            {
                if (UpdateDate.HasValue)
                {
                    return UpdateDate.Value.ToString("yyyy-MM-dd hh:ss:mm") + "[" + UpdateBy + "]";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}