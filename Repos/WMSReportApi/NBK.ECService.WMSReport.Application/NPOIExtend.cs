using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Other;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Application
{
    public class NPOIExtend
    {
        /// <summary>
        /// 设置单元格样式
        /// 注：此方法不支持设置某一行的其中几个单元格样式
        /// </summary>
        /// <param name="book">需要设置格式测workbook</param>
        /// <param name="row">需要设置格式的行</param>
        /// <param name="cellNumber">需要设置的列总数</param>
        public static void SetCellStyle(XSSFWorkbook book, IRow row, int cellNumber)
        {
            ICellStyle style = book.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.BorderLeft = BorderStyle.Medium;
            style.BorderRight = BorderStyle.Medium;
            style.BorderTop = BorderStyle.Medium;
            style.BorderBottom = BorderStyle.Medium;
            IFont font = book.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            style.SetFont(font);
            for (int i = 0; i < cellNumber; i++)
            {
                row.GetCell(i).CellStyle = style;
            }
        }

        public static XSSFWorkbook OutboundDetailReportExport(List<OutboundDetailReportDto> list)
        {
            //创建Excel文件的对象
            XSSFWorkbook book = new XSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("出库单号");
            row1.CreateCell(1).SetCellValue("状态");
            row1.CreateCell(2).SetCellValue("类型");
            row1.CreateCell(3).SetCellValue("业务类型");
            row1.CreateCell(4).SetCellValue("商品编号");
            row1.CreateCell(5).SetCellValue("商品名称");
            row1.CreateCell(6).SetCellValue("UPC");
            row1.CreateCell(7).SetCellValue("商品描述");
            row1.CreateCell(8).SetCellValue("订单数量");
            row1.CreateCell(9).SetCellValue("发货数量");
            row1.CreateCell(10).SetCellValue("下单时间");
            row1.CreateCell(11).SetCellValue("发货时间");
            row1.CreateCell(12).SetCellValue("服务综合体编码");
            row1.CreateCell(13).SetCellValue("服务综合体名称");
            row1.CreateCell(14).SetCellValue("收货人");
            row1.CreateCell(15).SetCellValue("收货人电话");
            row1.CreateCell(16).SetCellValue("收货人地址");

            row1.CreateCell(17).SetCellValue("TMS运单号");
            row1.CreateCell(18).SetCellValue("TMS装车顺序");
            row1.CreateCell(19).SetCellValue("TMS装车时间");

            //设置顶部标题样式
            SetCellStyle(book, row1, 20);

            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].OutboundOrder);
                rowtemp.CreateCell(1).SetCellValue(list[i].StatusDisplay);
                rowtemp.CreateCell(2).SetCellValue(list[i].OutboundTypeDisplay);
                rowtemp.CreateCell(3).SetCellValue(list[i].OutboundChildType);
                rowtemp.CreateCell(4).SetCellValue(list[i].SkuCode);
                rowtemp.CreateCell(5).SetCellValue(list[i].SkuName);
                rowtemp.CreateCell(6).SetCellValue(list[i].UPC);
                rowtemp.CreateCell(7).SetCellValue(list[i].SkuDescr);
                rowtemp.CreateCell(8).SetCellValue((double)list[i].Qty);
                rowtemp.CreateCell(9).SetCellValue((double)list[i].ShippedQty);
                rowtemp.CreateCell(10).SetCellValue(list[i].OutboundDateDisplay);
                rowtemp.CreateCell(11).SetCellValue(list[i].ActualShipDateDisplay);
                rowtemp.CreateCell(12).SetCellValue(list[i].ServiceStationCode);
                rowtemp.CreateCell(13).SetCellValue(list[i].ServiceStationName);
                rowtemp.CreateCell(14).SetCellValue(list[i].ConsigneeName);
                rowtemp.CreateCell(15).SetCellValue(list[i].ConsigneePhone);
                rowtemp.CreateCell(16).SetCellValue(list[i].AddressDisplay);

                rowtemp.CreateCell(17).SetCellValue(list[i].TMSOrder);
                rowtemp.CreateCell(18).SetCellValue(list[i].SortNumberDisplay);
                rowtemp.CreateCell(19).SetCellValue(list[i].DepartureDateDisplay);
            }

            return book;
        }

        public static XSSFWorkbook FrozenSkuReportByFile(List<FrozenSkuReportDto> list)
        {
            //创建Excel文件的对象
            XSSFWorkbook book = new XSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("商品名称");
            row1.CreateCell(1).SetCellValue("UPC");
            row1.CreateCell(2).SetCellValue("货位");
            row1.CreateCell(3).SetCellValue("批次");
            row1.CreateCell(4).SetCellValue("渠道");
            row1.CreateCell(5).SetCellValue("库存数量");

            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].SkuName);
                rowtemp.CreateCell(1).SetCellValue(list[i].UPC);
                rowtemp.CreateCell(2).SetCellValue(list[i].Loc);
                rowtemp.CreateCell(3).SetCellValue(list[i].Lot);
                rowtemp.CreateCell(4).SetCellValue(list[i].LotAttr01);
                rowtemp.CreateCell(5).SetCellValue((double)list[i].DisplayQty);
            }

            return book;
        }

        /// <summary>
        /// 出库汇总报表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static XSSFWorkbook OutboundSummaryExport(List<OutboundListDto> list)
        {
            //创建Excel文件的对象
            XSSFWorkbook book = new XSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("出库单编号");
            row1.CreateCell(1).SetCellValue("订单状态");
            row1.CreateCell(2).SetCellValue("单据类型");
            row1.CreateCell(3).SetCellValue("业务类型");
            row1.CreateCell(4).SetCellValue("外部单据号");
            row1.CreateCell(5).SetCellValue("创建时间");
            row1.CreateCell(6).SetCellValue("分配时间");
            row1.CreateCell(7).SetCellValue("发货时间");
            row1.CreateCell(8).SetCellValue("订单商品总数");
            row1.CreateCell(9).SetCellValue("发货商品总数");
            row1.CreateCell(10).SetCellValue("收货人姓名");
            row1.CreateCell(11).SetCellValue("收货人地址");
            row1.CreateCell(12).SetCellValue("服务综合体编码");
            row1.CreateCell(13).SetCellValue("服务综合体名称");
            row1.CreateCell(14).SetCellValue("订单备注");
            row1.CreateCell(15).SetCellValue("平台订单号");
            row1.CreateCell(16).SetCellValue("TMS运单号");
            row1.CreateCell(17).SetCellValue("装车顺序号");
            row1.CreateCell(18).SetCellValue("装车时间");
            row1.CreateCell(19).SetCellValue("作业人");
            row1.CreateCell(20).SetCellValue("渠道");
            row1.CreateCell(21).SetCellValue("是否开票");

            //设置顶部标题样式
            SetCellStyle(book, row1, 22);

            for (int i = 0; i < list.Count; i++)
            {
                IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].OutboundOrder);
                rowtemp.CreateCell(1).SetCellValue(list[i].StatusName);
                rowtemp.CreateCell(2).SetCellValue(list[i].OutboundTypeName);
                rowtemp.CreateCell(3).SetCellValue(list[i].OutboundChildType);
                rowtemp.CreateCell(4).SetCellValue(list[i].ExternOrderId);
                rowtemp.CreateCell(5).SetCellValue(list[i].CreateDateDisplay);
                rowtemp.CreateCell(6).SetCellValue(list[i].PickdetailCreateDateDisplay);
                rowtemp.CreateCell(7).SetCellValue(list[i].ActualShipDateDisplay);
                rowtemp.CreateCell(8).SetCellValue((double)list[i].DisplayTotalQty);
                rowtemp.CreateCell(9).SetCellValue((double)list[i].DisplayTotalShippedQty);
                rowtemp.CreateCell(10).SetCellValue(list[i].ConsigneeName);
                rowtemp.CreateCell(11).SetCellValue(list[i].FullAddress);
                rowtemp.CreateCell(12).SetCellValue(list[i].ServiceStationCode);
                rowtemp.CreateCell(13).SetCellValue(list[i].ServiceStationName);
                rowtemp.CreateCell(14).SetCellValue(list[i].Remark);
                rowtemp.CreateCell(15).SetCellValue(list[i].PlatformOrder);
                rowtemp.CreateCell(16).SetCellValue(list[i].TMSOrder);
                rowtemp.CreateCell(17).SetCellValue(list[i].SortNumber.ToString());
                rowtemp.CreateCell(18).SetCellValue(list[i].DepartureDateDisplay);
                rowtemp.CreateCell(19).SetCellValue(list[i].AppointUserNames);
                rowtemp.CreateCell(20).SetCellValue(list[i].Channel);
                rowtemp.CreateCell(21).SetCellValue(list[i].IsInvoiceName);

            }

            return book;
        }

        public static XSSFWorkbook ReturnOrderGlobalReportExport(List<ReturnOrderGlobalDto> list)
        {
            //创建Excel文件的对象
            XSSFWorkbook book = new XSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("入库单号");
            row1.CreateCell(1).SetCellValue("退货入库仓库");
            row1.CreateCell(2).SetCellValue("商品种数");
            row1.CreateCell(3).SetCellValue("商品个数");
            row1.CreateCell(4).SetCellValue("创建人");
            row1.CreateCell(5).SetCellValue("创建时间");
            row1.CreateCell(6).SetCellValue("源出库单号");
            row1.CreateCell(7).SetCellValue("源出库类型");
            row1.CreateCell(8).SetCellValue("源出库仓库");

            //设置顶部标题样式
            SetCellStyle(book, row1, 9);

            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].PurchaseOrder);
                rowtemp.CreateCell(1).SetCellValue(list[i].PurchaseWarehouse);
                rowtemp.CreateCell(2).SetCellValue((double)list[i].skuGroupCount);
                rowtemp.CreateCell(3).SetCellValue((double)list[i].skuTotalCount);
                rowtemp.CreateCell(4).SetCellValue(list[i].CreateUserName);
                rowtemp.CreateCell(5).SetCellValue(list[i].CreateDateDisplay);
                rowtemp.CreateCell(6).SetCellValue(list[i].OutboundOrder);
                rowtemp.CreateCell(7).SetCellValue(list[i].OutboundTypeDisplay);
                rowtemp.CreateCell(8).SetCellValue(list[i].OutboundWarehouse);
            }

            return book;
        }

        public static XSSFWorkbook OutboundDetailGlobalExport(List<OutboundDetailGlobalDto> list)
        {
            //创建Excel文件的对象
            XSSFWorkbook book = new XSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("所属仓库"); 
            row1.CreateCell(1).SetCellValue("出库单号");
            row1.CreateCell(2).SetCellValue("状态");
            row1.CreateCell(3).SetCellValue("类型");
            row1.CreateCell(4).SetCellValue("业务类型");
            row1.CreateCell(5).SetCellValue("商品编号");
            row1.CreateCell(6).SetCellValue("商品名称");
            row1.CreateCell(7).SetCellValue("UPC");
            row1.CreateCell(8).SetCellValue("商品描述");
            row1.CreateCell(9).SetCellValue("订单数量");
            row1.CreateCell(10).SetCellValue("发货数量");
            row1.CreateCell(11).SetCellValue("下单时间");
            row1.CreateCell(12).SetCellValue("发货时间");
            row1.CreateCell(13).SetCellValue("服务综合体编码");
            row1.CreateCell(14).SetCellValue("服务综合体名称");
            row1.CreateCell(15).SetCellValue("收货人");
            row1.CreateCell(16).SetCellValue("收货人电话");
            row1.CreateCell(17).SetCellValue("收货人地址"); 
            row1.CreateCell(18).SetCellValue("TMS运单号");
            row1.CreateCell(19).SetCellValue("TMS装车顺序");
            row1.CreateCell(20).SetCellValue("TMS装车时间");

            //设置顶部标题样式
            SetCellStyle(book, row1, 21);

            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName); 
                rowtemp.CreateCell(1).SetCellValue(list[i].OutboundOrder);
                rowtemp.CreateCell(2).SetCellValue(list[i].StatusDisplay);
                rowtemp.CreateCell(3).SetCellValue(list[i].OutboundTypeDisplay);
                rowtemp.CreateCell(4).SetCellValue(list[i].OutboundChildType);
                rowtemp.CreateCell(5).SetCellValue(list[i].SkuCode);
                rowtemp.CreateCell(6).SetCellValue(list[i].SkuName);
                rowtemp.CreateCell(7).SetCellValue(list[i].UPC);
                rowtemp.CreateCell(8).SetCellValue(list[i].SkuDescr);
                rowtemp.CreateCell(9).SetCellValue((double)list[i].Qty);
                rowtemp.CreateCell(10).SetCellValue((double)list[i].ShippedQty);
                rowtemp.CreateCell(11).SetCellValue(list[i].OutboundDateDisplay);
                rowtemp.CreateCell(12).SetCellValue(list[i].ActualShipDateDisplay);
                rowtemp.CreateCell(13).SetCellValue(list[i].ServiceStationCode);
                rowtemp.CreateCell(14).SetCellValue(list[i].ServiceStationName);
                rowtemp.CreateCell(15).SetCellValue(list[i].ConsigneeName);
                rowtemp.CreateCell(16).SetCellValue(list[i].ConsigneePhone);
                rowtemp.CreateCell(17).SetCellValue(list[i].AddressDisplay); 
                rowtemp.CreateCell(18).SetCellValue(list[i].TMSOrder);
                rowtemp.CreateCell(19).SetCellValue(list[i].SortNumberDisplay);
                rowtemp.CreateCell(20).SetCellValue(list[i].DepartureDateDisplay);
            }

            return book;
        }

        /// <summary>
        /// 出库汇总报表全局仓
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static XSSFWorkbook OutboundSummaryGlobalExport(List<OutboundListGlobalDto> list)
        {
            //创建Excel文件的对象
            XSSFWorkbook book = new XSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("所属仓库");
            row1.CreateCell(1).SetCellValue("出库单编号");
            row1.CreateCell(2).SetCellValue("订单状态");
            row1.CreateCell(3).SetCellValue("单据类型");
            row1.CreateCell(4).SetCellValue("业务类型");
            row1.CreateCell(5).SetCellValue("外部单据号");
            row1.CreateCell(6).SetCellValue("创建时间");
            row1.CreateCell(7).SetCellValue("分配时间");
            row1.CreateCell(8).SetCellValue("发货时间");
            row1.CreateCell(9).SetCellValue("订单商品总数");
            row1.CreateCell(10).SetCellValue("发货商品总数");
            row1.CreateCell(11).SetCellValue("收货人姓名");
            row1.CreateCell(12).SetCellValue("收货人地址");
            row1.CreateCell(13).SetCellValue("服务综合体编码");
            row1.CreateCell(14).SetCellValue("服务综合体名称");
            row1.CreateCell(15).SetCellValue("订单备注");
            row1.CreateCell(16).SetCellValue("平台订单号");
            row1.CreateCell(17).SetCellValue("TMS运单号");
            row1.CreateCell(18).SetCellValue("装车顺序号");
            row1.CreateCell(19).SetCellValue("装车时间");
            row1.CreateCell(20).SetCellValue("作业人");
            row1.CreateCell(21).SetCellValue("渠道");
            row1.CreateCell(22).SetCellValue("是否开票");

            //设置顶部标题样式
            SetCellStyle(book, row1, 23);

            for (int i = 0; i < list.Count; i++)
            {
                IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                rowtemp.CreateCell(1).SetCellValue(list[i].OutboundOrder);
                rowtemp.CreateCell(2).SetCellValue(list[i].StatusName);
                rowtemp.CreateCell(3).SetCellValue(list[i].OutboundTypeName);
                rowtemp.CreateCell(4).SetCellValue(list[i].OutboundChildType);
                rowtemp.CreateCell(5).SetCellValue(list[i].ExternOrderId);
                rowtemp.CreateCell(6).SetCellValue(list[i].CreateDateDisplay);
                rowtemp.CreateCell(7).SetCellValue(list[i].PickdetailCreateDateDisplay);
                rowtemp.CreateCell(8).SetCellValue(list[i].ActualShipDateDisplay);
                rowtemp.CreateCell(9).SetCellValue((double)list[i].DisplayTotalQty);
                rowtemp.CreateCell(10).SetCellValue((double)list[i].DisplayTotalShippedQty);
                rowtemp.CreateCell(11).SetCellValue(list[i].ConsigneeName);
                rowtemp.CreateCell(12).SetCellValue(list[i].FullAddress);
                rowtemp.CreateCell(13).SetCellValue(list[i].ServiceStationCode);
                rowtemp.CreateCell(14).SetCellValue(list[i].ServiceStationName);
                rowtemp.CreateCell(15).SetCellValue(list[i].Remark);
                rowtemp.CreateCell(16).SetCellValue(list[i].PlatformOrder);
                rowtemp.CreateCell(17).SetCellValue(list[i].TMSOrder);
                rowtemp.CreateCell(18).SetCellValue(list[i].SortNumber.ToString());
                rowtemp.CreateCell(19).SetCellValue(list[i].DepartureDateDisplay);
                rowtemp.CreateCell(20).SetCellValue(list[i].AppointUserNames);
                rowtemp.CreateCell(21).SetCellValue(list[i].Channel);
                rowtemp.CreateCell(22).SetCellValue(list[i].IsInvoiceName);

            }

            return book;
        }
    }

    public class NpoiMemoryStream : MemoryStream
    {
        public NpoiMemoryStream()
        {
            AllowClose = true;
        }

        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
}
