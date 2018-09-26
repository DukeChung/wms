namespace NBK.ECService.WMSReport.DTO.Base
{
    public class BaseQuery : BaseDto
    {
        /// <summary>
        /// 请求次数
        /// </summary>
        public int sEcho { get; set; }

        public int iColumns { get; set; }

        public string sColumns { get; set; }

        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }

        public string mDataProp_1 { get; set; }

        public string mDataProp_2 { get; set; }

        public int iTotalDisplayRecords { get; set; }
    }
}