using System.Collections.Generic;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.Utility;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using NBK.ECService.WMSReport.DTO.Report;
using NBK.ECService.WMSReport.Utility.Enum;

namespace WMS.Global.Portal.Services
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
        public static void SetCellStyle(HSSFWorkbook book, IRow row, int cellNumber)
        {
            ICellStyle style = book.CreateCellStyle();
            style.Alignment = HorizontalAlignment.CENTER;
            style.BorderLeft = BorderStyle.MEDIUM;
            style.BorderRight = BorderStyle.MEDIUM;
            style.BorderTop = BorderStyle.MEDIUM;
            style.BorderBottom = BorderStyle.MEDIUM;
            IFont font = book.CreateFont();
            font.Boldweight = (short)FontBoldWeight.BOLD;
            style.SetFont(font);
            for (int i = 0; i < cellNumber; i++)
            {
                row.GetCell(i).CellStyle = style;
            }
        }

        public static HSSFWorkbook InvSkuReportExport(List<InvSkuGlobalDto> list)
        {
            //创建Excel文件的对象
            HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("所属仓库");
            row1.CreateCell(1).SetCellValue("商品外部ID");
            row1.CreateCell(2).SetCellValue("商品名称");
            row1.CreateCell(3).SetCellValue("商品条码");
            row1.CreateCell(4).SetCellValue("库存数量");


            //设置顶部标题样式
            SetCellStyle(book, row1, 5);

            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                rowtemp.CreateCell(1).SetCellValue(list[i].SkuOtherId);
                rowtemp.CreateCell(2).SetCellValue(list[i].SkuName);
                rowtemp.CreateCell(3).SetCellValue(list[i].UPC);
                rowtemp.CreateCell(4).SetCellValue((double)list[i].DisplayQty);
            }

            return book;
        }

        public static HSSFWorkbook ReceiptDetailReportExport(List<ReceiptDetailGlobalDto> list)
        {
            //创建Excel文件的对象
            HSSFWorkbook book = new HSSFWorkbook();
            int sheetRow = PublicConst.EachSheetDataRowsCount;
            int pageCount = (list.Count - 1) / sheetRow + 1;
            for (int page = 1; page <= pageCount; page++)
            {
                ISheet sheet1 = book.CreateSheet("Sheet" + page);

                //给sheet1添加第一行的头部标题
                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("所属仓库");
                row1.CreateCell(1).SetCellValue("收货单号");
                row1.CreateCell(2).SetCellValue("入库单号");
                row1.CreateCell(3).SetCellValue("出库单号");
                row1.CreateCell(4).SetCellValue("收货单状态");
                row1.CreateCell(5).SetCellValue("供应商名称");
                row1.CreateCell(6).SetCellValue("商品编号");
                row1.CreateCell(7).SetCellValue("商品名称");
                row1.CreateCell(8).SetCellValue("UPC码");
                row1.CreateCell(9).SetCellValue("商品描述");
                row1.CreateCell(10).SetCellValue("收货数量");
                row1.CreateCell(11).SetCellValue("上架数量");
                row1.CreateCell(12).SetCellValue("收货时间");

                //设置顶部标题样式
                SetCellStyle(book, row1, 13);

                int rowCount = 0;
                for (int i = (page - 1) * sheetRow; i < page * sheetRow; i++)
                {
                    if (i >= list.Count)
                    {
                        break;
                    }
                    IRow rowtemp = sheet1.CreateRow(rowCount + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].ReceiptOrder);
                    rowtemp.CreateCell(2).SetCellValue(list[i].ExternalOrder);
                    rowtemp.CreateCell(3).SetCellValue(list[i].OutboundOrder);
                    rowtemp.CreateCell(4).SetCellValue(list[i].StatusDisplay);
                    rowtemp.CreateCell(5).SetCellValue(list[i].VendorName);
                    rowtemp.CreateCell(6).SetCellValue(list[i].SkuCode);
                    rowtemp.CreateCell(7).SetCellValue(list[i].SkuName);
                    rowtemp.CreateCell(8).SetCellValue(list[i].UPC);
                    rowtemp.CreateCell(9).SetCellValue(list[i].SkuDescr);
                    rowtemp.CreateCell(10).SetCellValue((double)list[i].DisplayReceivedQty);
                    rowtemp.CreateCell(11).SetCellValue((double)list[i].DisplayShelvesQty);
                    rowtemp.CreateCell(12).SetCellValue(list[i].ReceivedDate.Value.ToString(PublicConst.DateTimeFormat));
                    rowCount++;
                }
            }
            return book;
        }

        public static HSSFWorkbook OutboundDetailReportExport(List<OutboundDetailGlobalDto> list, HSSFWorkbook book, int rspCount)
        {
            ////创建Excel文件的对象
            //HSSFWorkbook book = new HSSFWorkbook();  

            //添加一个sheet
            ISheet sheet1 = book.CreateSheet("Sheet" + rspCount);

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
            row1.CreateCell(12).SetCellValue("服务站编码");
            row1.CreateCell(13).SetCellValue("服务站");
            row1.CreateCell(14).SetCellValue("收货人");
            row1.CreateCell(15).SetCellValue("收货人电话");
            row1.CreateCell(16).SetCellValue("收货人地址");

            row1.CreateCell(17).SetCellValue("TMS运单号");
            row1.CreateCell(18).SetCellValue("TMS装车顺序");
            row1.CreateCell(19).SetCellValue("TMS装车时间");

            //设置顶部标题样式
            SetCellStyle(book, row1, 19);

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

        public static HSSFWorkbook FinanceInvoicingReportExport(List<FinanceInvoicingGlobalDto> list)
        {
            //创建Excel文件的对象
            HSSFWorkbook book = new HSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("仓库名称");
            row1.CreateCell(1).SetCellValue("商品外部ID");
            row1.CreateCell(2).SetCellValue("商品名称");
            row1.CreateCell(3).SetCellValue("UPC");
            row1.CreateCell(4).SetCellValue("期初数量");
            row1.CreateCell(5).SetCellValue("本期收货数量");
            row1.CreateCell(6).SetCellValue("本期出库数量");
            row1.CreateCell(7).SetCellValue("本期损益数量");
            row1.CreateCell(8).SetCellValue("期末数量");

            //设置顶部标题样式
            SetCellStyle(book, row1, 9);

            for (int i = 0; i < list.Count; i++)
            {
                IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                rowtemp.CreateCell(1).SetCellValue(list[i].SkuOtherId);
                rowtemp.CreateCell(2).SetCellValue(list[i].SkuName);
                rowtemp.CreateCell(3).SetCellValue(list[i].UPC);
                rowtemp.CreateCell(4).SetCellValue((double)list[i].DisplayInitialQty);
                rowtemp.CreateCell(5).SetCellValue((double)list[i].DisplayCurrentPeriodReceiptQty);
                rowtemp.CreateCell(6).SetCellValue((double)list[i].DisplayCurrentPeriodOutboundQty);
                rowtemp.CreateCell(7).SetCellValue((double)list[i].DisplayAdjustmentQty);
                rowtemp.CreateCell(8).SetCellValue((double)list[i].EndingInventoryQty);
            }

            return book;
        }

        /// <summary>
        /// 入库明细导出
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook PurchaseDetailReportExport(List<PurchaseDetailGlobalDto> list)
        {
            HSSFWorkbook book = new HSSFWorkbook();
            int sheetRow = PublicConst.EachSheetDataRowsCount;
            int pageCount = (list.Count - 1) / sheetRow + 1;
            for (int page = 1; page <= pageCount; page++)
            {
                ISheet sheet1 = book.CreateSheet("Sheet" + page);

                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("所属仓库");
                row1.CreateCell(1).SetCellValue("入库单号");
                row1.CreateCell(2).SetCellValue("商品名称");
                row1.CreateCell(3).SetCellValue("UPC");
                row1.CreateCell(4).SetCellValue("采购数量");
                row1.CreateCell(5).SetCellValue("实际入库数量");
                row1.CreateCell(6).SetCellValue("最后入库日期");
                row1.CreateCell(7).SetCellValue("供应商");

                //设置顶部标题样式
                SetCellStyle(book, row1, 8);

                int rowCount = 0;
                for (int i = (page - 1) * sheetRow; i < page * sheetRow; i++)
                {
                    if (i >= list.Count)
                    {
                        break;
                    }
                    IRow rowtemp = sheet1.CreateRow(rowCount + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].PurchaseOrder);
                    rowtemp.CreateCell(2).SetCellValue(list[i].SkuName);
                    rowtemp.CreateCell(3).SetCellValue(list[i].UPC);
                    rowtemp.CreateCell(4).SetCellValue((double)list[i].Qty);
                    rowtemp.CreateCell(5).SetCellValue((double)list[i].ReceivedQty);
                    rowtemp.CreateCell(6).SetCellValue(list[i].LastReceiptDateDisplay);
                    rowtemp.CreateCell(7).SetCellValue(list[i].VendorName);
                    rowCount++;
                }
            }
            return book;
        }

        /// <summary>
        /// 冻结商品导出
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook FrozenSkuReportExport(List<FrozenSkuGlobalDto> list)
        {
            HSSFWorkbook book = new HSSFWorkbook();
            int sheetRow = PublicConst.EachSheetDataRowsCount;
            int pageCount = (list.Count - 1) / sheetRow + 1;
            for (int page = 1; page <= pageCount; page++)
            {
                ISheet sheet1 = book.CreateSheet("Sheet" + page);

                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("所属仓库");
                row1.CreateCell(1).SetCellValue("商品名称");
                row1.CreateCell(2).SetCellValue("UPC");
                row1.CreateCell(3).SetCellValue("货位");
                row1.CreateCell(4).SetCellValue("批次");
                row1.CreateCell(5).SetCellValue("渠道");
                row1.CreateCell(6).SetCellValue("库存数量");

                //设置顶部标题样式
                SetCellStyle(book, row1, 7);

                int rowCount = 0;
                for (int i = (page - 1) * sheetRow; i < page * sheetRow; i++)
                {
                    if (i >= list.Count)
                    {
                        break;
                    }
                    IRow rowtemp = sheet1.CreateRow(rowCount + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].SkuName);
                    rowtemp.CreateCell(2).SetCellValue(list[i].UPC);
                    rowtemp.CreateCell(3).SetCellValue(list[i].Loc);
                    rowtemp.CreateCell(4).SetCellValue(list[i].Lot);
                    rowtemp.CreateCell(5).SetCellValue(list[i].LotAttr01);
                    rowtemp.CreateCell(6).SetCellValue((double)list[i].DisplayQty);
                    rowCount++;
                }
            }
            return book;
        }

        public static HSSFWorkbook InboundReportExport(List<InboundGlobalDto> list)
        {
            //创建Excel文件的对象
            HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("入库单号");
            row1.CreateCell(1).SetCellValue("入库单类型");
            row1.CreateCell(2).SetCellValue("入库单状态");
            row1.CreateCell(3).SetCellValue("最后入库时间");
            row1.CreateCell(4).SetCellValue("实际入库数量");
            row1.CreateCell(5).SetCellValue("拒收数量");

            //设置顶部标题样式
            SetCellStyle(book, row1, 6);

            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].PurchaseOrder);
                rowtemp.CreateCell(1).SetCellValue(list[i].PurchaseTypeText);
                rowtemp.CreateCell(2).SetCellValue(list[i].StatusText);
                rowtemp.CreateCell(3).SetCellValue(list[i].LastReceiptDateText);
                rowtemp.CreateCell(4).SetCellValue(Convert.ToDouble(list[i].DisplayReceivedQty));
                rowtemp.CreateCell(5).SetCellValue(Convert.ToDouble(list[i].DisplayRejectedQty));
            }

            return book;
        }

        /// <summary>
        /// 货位库存导出
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook InvLocBySkuReportExport(List<InvSkuLocGlobalDto> list)
        {
            //创建Excel文件的对象
            HSSFWorkbook book = new HSSFWorkbook();
            int sheetRow = PublicConst.EachSheetDataRowsCount;
            int pageCount = (list.Count - 1) / sheetRow + 1;
            for (int page = 1; page <= pageCount; page++)
            {
                ISheet sheet1 = book.CreateSheet("Sheet" + page);

                //给sheet1添加第一行的头部标题
                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("所属仓库");
                row1.CreateCell(1).SetCellValue("商品编号");
                row1.CreateCell(2).SetCellValue("商品名称");
                row1.CreateCell(3).SetCellValue("商品UPC");
                row1.CreateCell(4).SetCellValue("商品描述");
                row1.CreateCell(5).SetCellValue("外部Id");
                row1.CreateCell(6).SetCellValue("货位");
                row1.CreateCell(7).SetCellValue("库存数量");
                row1.CreateCell(8).SetCellValue("分配数量");
                row1.CreateCell(9).SetCellValue("拣货数量");

                //设置顶部标题样式
                SetCellStyle(book, row1, 10);

                int rowCount = 0;
                for (int i = (page - 1) * sheetRow; i < page * sheetRow; i++)
                {
                    if (i >= list.Count)
                    {
                        break;
                    }

                    IRow rowtemp = sheet1.CreateRow(rowCount + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].SkuCode);
                    rowtemp.CreateCell(2).SetCellValue(list[i].SkuName);
                    rowtemp.CreateCell(3).SetCellValue(list[i].UPC);
                    rowtemp.CreateCell(4).SetCellValue(list[i].SkuDescr);
                    rowtemp.CreateCell(5).SetCellValue(list[i].OtherId);
                    rowtemp.CreateCell(6).SetCellValue(list[i].Loc);
                    rowtemp.CreateCell(7).SetCellValue((double)list[i].DisplayQty);
                    rowtemp.CreateCell(8).SetCellValue((double)list[i].DisplayAllocatedQty);
                    rowtemp.CreateCell(9).SetCellValue((double)list[i].DisplayPickedQty);
                    rowCount++;
                }
            }
            return book;
        }

        /// <summary>
        /// 导出收发货明细
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook ReceivedAndSendSkuReportExport(List<ReceivedAndSendSkuGlobalDto> list)
        {
            HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("仓库名称");
            row1.CreateCell(1).SetCellValue("当天收货SKU数量");
            row1.CreateCell(2).SetCellValue("收货总件数");
            row1.CreateCell(3).SetCellValue("当天发货SKU数量");
            row1.CreateCell(4).SetCellValue("发货总件数");
            row1.CreateCell(5).SetCellValue("入库单数量");
            row1.CreateCell(6).SetCellValue("出库单数量");
            row1.CreateCell(7).SetCellValue("日期");

            //设置顶部标题样式
            SetCellStyle(book, row1, 8);

            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                rowtemp.CreateCell(1).SetCellValue((double)list[i].ReceivedQty);
                rowtemp.CreateCell(2).SetCellValue((double)list[i].ReceivedPieceQty);
                rowtemp.CreateCell(3).SetCellValue((double)list[i].DeliveryQty);
                rowtemp.CreateCell(4).SetCellValue((double)list[i].DeliveryPieceQty);
                rowtemp.CreateCell(5).SetCellValue((double)list[i].PurchaseCount);
                rowtemp.CreateCell(6).SetCellValue((double)list[i].OutboundCount);
                rowtemp.CreateCell(7).SetCellValue(list[i].CreateDate);
            }
            return book;
        }

        /// <summary>
        /// 出库处理时间统计表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook OutboundHandleDateStatisticsExport(List<OutboundHandleDateStatisticsGlobalDto> list)
        {
            HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("所属仓库");
            row1.CreateCell(1).SetCellValue("出库单号");
            row1.CreateCell(2).SetCellValue("单据类型");
            row1.CreateCell(3).SetCellValue("创建时间");
            row1.CreateCell(4).SetCellValue("拣货时间");
            row1.CreateCell(5).SetCellValue("装箱时间");
            row1.CreateCell(6).SetCellValue("出库时间");
            row1.CreateCell(7).SetCellValue("单据状态");

            //设置顶部标题样式
            SetCellStyle(book, row1, 8);

            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                rowtemp.CreateCell(1).SetCellValue(list[i].OutboundOrder);
                rowtemp.CreateCell(2).SetCellValue(list[i].OutboundTypeName);
                rowtemp.CreateCell(3).SetCellValue(list[i].CreateDateDisplay);
                rowtemp.CreateCell(4).SetCellValue(list[i].PickDateDisplay);
                rowtemp.CreateCell(5).SetCellValue(list[i].VanningDateDisplay);
                rowtemp.CreateCell(6).SetCellValue(list[i].ActualShipDateDisplay);
                rowtemp.CreateCell(7).SetCellValue(list[i].StatusName);
            }
            return book;
        }

        /// <summary>
        /// 收货、上架时间统计表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook ReceiptAndDeliveryDateExport(List<ReceiptAndDeliveryDateGlobalDto> list)
        {
            HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            ISheet sheet1 = book.CreateSheet("Sheet1");

            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("入库单号");
            row1.CreateCell(1).SetCellValue("单据类型");
            row1.CreateCell(2).SetCellValue("审核时间");
            row1.CreateCell(3).SetCellValue("收货单号");
            row1.CreateCell(4).SetCellValue("收货单生成时间");
            row1.CreateCell(5).SetCellValue("实际收货数量");
            row1.CreateCell(6).SetCellValue("收货完成时间");
            row1.CreateCell(7).SetCellValue("实际上架数量");
            row1.CreateCell(8).SetCellValue("上架完成时间");

            //设置顶部标题样式
            SetCellStyle(book, row1, 9);

            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].PurchaseOrder);
                rowtemp.CreateCell(1).SetCellValue(list[i].TypeName);
                rowtemp.CreateCell(2).SetCellValue(list[i].AuditingDateDisplay);
                rowtemp.CreateCell(3).SetCellValue(list[i].ReceiptOrder);
                rowtemp.CreateCell(4).SetCellValue(list[i].CreateDateDisplay);
                rowtemp.CreateCell(5).SetCellValue((double)list[i].TotalReceivedQty);
                rowtemp.CreateCell(6).SetCellValue(list[i].ReceiptDateDisplay);
                rowtemp.CreateCell(7).SetCellValue((double)list[i].TotalShelvesQty);
                rowtemp.CreateCell(8).SetCellValue(list[i].ShelvesDateDisplay);
            }
            return book;
        }

        /// <summary>
        /// 异常报告报表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook OutboundExceptionReportExport(List<OutboundExceptionGlobalDto> list)
        {
            HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            ISheet sheet1 = book.CreateSheet("Sheet1");

            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("年月");
            row1.CreateCell(1).SetCellValue("州/市");
            row1.CreateCell(2).SetCellValue("区/县");
            row1.CreateCell(3).SetCellValue("镇/乡");

            row1.CreateCell(4).SetCellValue("服务站名称");
            row1.CreateCell(5).SetCellValue("服务站编码");
            row1.CreateCell(6).SetCellValue("异常类别");
            row1.CreateCell(7).SetCellValue("商品条码");
            row1.CreateCell(8).SetCellValue("商品名称");

            row1.CreateCell(9).SetCellValue("异常数量");
            row1.CreateCell(10).SetCellValue("单位");
            row1.CreateCell(11).SetCellValue("异常描述");

            row1.CreateCell(12).SetCellValue("跟进结果");
            row1.CreateCell(13).SetCellValue("责任部门");
            row1.CreateCell(14).SetCellValue("责任归属与处理");

            row1.CreateCell(15).SetCellValue("备注");
            row1.CreateCell(16).SetCellValue("是否理赔");

            //设置顶部标题样式
            SetCellStyle(book, row1, 17);

            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].ActualShipDateDispaly);
                rowtemp.CreateCell(1).SetCellValue(list[i].ConsigneeCity);
                rowtemp.CreateCell(2).SetCellValue(list[i].ConsigneeArea);
                rowtemp.CreateCell(3).SetCellValue(list[i].ConsigneeTown);

                rowtemp.CreateCell(4).SetCellValue(list[i].ServiceStationName);
                rowtemp.CreateCell(5).SetCellValue(list[i].ServiceStationCode);
                rowtemp.CreateCell(6).SetCellValue(list[i].ExceptionReason);
                rowtemp.CreateCell(7).SetCellValue(list[i].UPC);
                rowtemp.CreateCell(8).SetCellValue(list[i].SkuName);

                rowtemp.CreateCell(9).SetCellValue((double)list[i].ExceptionQty);
                rowtemp.CreateCell(10).SetCellValue(list[i].UOMCode);
                rowtemp.CreateCell(11).SetCellValue(list[i].ExceptionDesc);

                rowtemp.CreateCell(12).SetCellValue(list[i].Result);
                rowtemp.CreateCell(13).SetCellValue(list[i].Department);
                rowtemp.CreateCell(14).SetCellValue(list[i].Responsibility);

                rowtemp.CreateCell(15).SetCellValue(list[i].Remark);
                rowtemp.CreateCell(16).SetCellValue(list[i].IsSettlementName);

            }
            return book;
        }

        /// <summary>
        /// 出库箱数统计报表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook OutboundBoxReportExport(List<OutboundBoxGlobalDto> list, HSSFWorkbook book, int rspCount)
        {
            ISheet sheet1 = book.CreateSheet("Sheet" + rspCount);

            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("仓库名称");
            row1.CreateCell(1).SetCellValue("出库单号");
            row1.CreateCell(2).SetCellValue("状态");
            row1.CreateCell(3).SetCellValue("创建时间");
            row1.CreateCell(4).SetCellValue("发货时间");
            row1.CreateCell(5).SetCellValue("整箱数量");
            row1.CreateCell(6).SetCellValue("散箱数量");
            row1.CreateCell(7).SetCellValue("出库总箱数");

            //设置顶部标题样式
            SetCellStyle(book, row1, 8);

            for (int i = 0; i < list.Count; i++)
            {
                IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                rowtemp.CreateCell(1).SetCellValue(list[i].OutboundOrder);
                rowtemp.CreateCell(2).SetCellValue(list[i].StatusText);
                rowtemp.CreateCell(3).SetCellValue(list[i].CreateDateText);
                rowtemp.CreateCell(4).SetCellValue(list[i].ActualShipDateText);
                rowtemp.CreateCell(5).SetCellValue(list[i].WholeCaseQty);
                rowtemp.CreateCell(6).SetCellValue(list[i].ScatteredCaseQty);
                rowtemp.CreateCell(7).SetCellValue(list[i].TotalCaseQty);
            }
            return book;
        }

        /// <summary>
        /// 整散箱装箱明细报表导出
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook OutboundPackReportExport(List<OutboundPackGlobalDto> list)
        {
            HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            ISheet sheet1 = book.CreateSheet("Sheet1");

            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("序号");
            row1.CreateCell(1).SetCellValue("仓库名称");
            row1.CreateCell(2).SetCellValue("订单类型");
            row1.CreateCell(3).SetCellValue("出库单号");
            row1.CreateCell(4).SetCellValue("服务站名称");
            row1.CreateCell(5).SetCellValue("箱型");
            row1.CreateCell(6).SetCellValue("件数");
            row1.CreateCell(7).SetCellValue("出库数量");
            row1.CreateCell(8).SetCellValue("出库日期");
            row1.CreateCell(9).SetCellValue("操作人员");
            row1.CreateCell(10).SetCellValue("拼箱号");

            //设置顶部标题样式
            SetCellStyle(book, row1, 11);

            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(i + 1);
                rowtemp.CreateCell(1).SetCellValue(list[i].WarehouseName);
                rowtemp.CreateCell(2).SetCellValue(list[i].Channel);
                rowtemp.CreateCell(3).SetCellValue(list[i].OutboundOrder);
                rowtemp.CreateCell(4).SetCellValue(list[i].ServiceStationName);
                rowtemp.CreateCell(5).SetCellValue(list[i].TransferTypeDisplay);
                rowtemp.CreateCell(6).SetCellValue((double)list[i].packCount);
                rowtemp.CreateCell(7).SetCellValue((double)list[i].OutboundQty);
                rowtemp.CreateCell(8).SetCellValue(list[i].ActualShipDate.ToString(PublicConst.DateTimeFormat));
                rowtemp.CreateCell(9).SetCellValue(list[i].CreateUserName);
                rowtemp.CreateCell(10).SetCellValue(list[i].TransferOrder);
            }
            return book;
        }

        /// <summary>
        /// 整散箱商品明细报表导出
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook OutboundPackSkuReportExport(List<OutboundPackSkuGlobalDto> list)
        {
            HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            ISheet sheet1 = book.CreateSheet("Sheet1");

            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("仓库名称");
            row1.CreateCell(1).SetCellValue("订单类型");
            row1.CreateCell(2).SetCellValue("出库单号");
            row1.CreateCell(3).SetCellValue("服务站名称");
            row1.CreateCell(4).SetCellValue("出库日期");
            row1.CreateCell(5).SetCellValue("操作人员");
            row1.CreateCell(6).SetCellValue("箱号");
            row1.CreateCell(7).SetCellValue("箱型");
            row1.CreateCell(8).SetCellValue("商品名称");
            row1.CreateCell(9).SetCellValue("UPC");
            row1.CreateCell(10).SetCellValue("数量");

            //设置顶部标题样式
            SetCellStyle(book, row1, 11);

            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                rowtemp.CreateCell(1).SetCellValue(list[i].Channel);
                rowtemp.CreateCell(2).SetCellValue(list[i].OutboundOrder);
                rowtemp.CreateCell(3).SetCellValue(list[i].ServiceStationName);
                rowtemp.CreateCell(4).SetCellValue(list[i].ActualShipDate.ToString(PublicConst.DateTimeFormat));
                rowtemp.CreateCell(5).SetCellValue(list[i].CreateUserName);
                rowtemp.CreateCell(6).SetCellValue(list[i].TransferOrder);
                rowtemp.CreateCell(7).SetCellValue(((OutboundTransferOrderType)list[i].TransferType).ToDescription());
                rowtemp.CreateCell(8).SetCellValue(list[i].SkuName);
                rowtemp.CreateCell(9).SetCellValue(list[i].UPC);
                rowtemp.CreateCell(10).SetCellValue((double)list[i].Qty);

            }
            return book;
        }

        /// <summary>
        /// 商品包装查询报表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook SkuPackReportExport(List<SkuPackGlobalListDto> list)
        {
            HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            ISheet sheet1 = book.CreateSheet("Sheet1");

            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("商品编号");
            row1.CreateCell(1).SetCellValue("商品名称");
            row1.CreateCell(2).SetCellValue("商品UPC");

            row1.CreateCell(3).SetCellValue("包装UPC02");
            row1.CreateCell(4).SetCellValue("单位02");
            row1.CreateCell(5).SetCellValue("单位数量02");

            row1.CreateCell(6).SetCellValue("包装UPC03");
            row1.CreateCell(7).SetCellValue("单位03");
            row1.CreateCell(8).SetCellValue("单位数量03");

            row1.CreateCell(9).SetCellValue("包装UPC04");
            row1.CreateCell(10).SetCellValue("单位04");
            row1.CreateCell(11).SetCellValue("单位数量04");

            row1.CreateCell(12).SetCellValue("包装UPC05");
            row1.CreateCell(13).SetCellValue("单位05");
            row1.CreateCell(14).SetCellValue("单位数量05");

            //设置顶部标题样式
            SetCellStyle(book, row1, 15);

            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].SkuCode);
                rowtemp.CreateCell(1).SetCellValue(list[i].SkuName);
                rowtemp.CreateCell(2).SetCellValue(list[i].UPC);

                rowtemp.CreateCell(3).SetCellValue(list[i].UPC02);
                rowtemp.CreateCell(4).SetCellValue(list[i].UOMCode02);
                rowtemp.CreateCell(5).SetCellValue(list[i].FieldValue02.HasValue ? list[i].FieldValue02.ToString() : "");

                rowtemp.CreateCell(6).SetCellValue(list[i].UPC03);
                rowtemp.CreateCell(7).SetCellValue(list[i].UOMCode03);
                rowtemp.CreateCell(8).SetCellValue(list[i].FieldValue03.HasValue ? list[i].FieldValue03.ToString() : "");

                rowtemp.CreateCell(9).SetCellValue(list[i].UPC04);
                rowtemp.CreateCell(10).SetCellValue(list[i].UOMCode04);
                rowtemp.CreateCell(11).SetCellValue(list[i].FieldValue04.HasValue ? list[i].FieldValue04.ToString() : "");

                rowtemp.CreateCell(12).SetCellValue(list[i].UPC05);
                rowtemp.CreateCell(13).SetCellValue(list[i].UOMCode05);
                rowtemp.CreateCell(14).SetCellValue(list[i].FieldValue05.HasValue ? list[i].FieldValue05.ToString() : "");
            }
            return book;
        }


        /// <summary>
        /// B2C结算报表导出
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook BalanceReportExport(List<BalanceGlobalDto> list)
        {
            //创建Excel文件的对象
            HSSFWorkbook book = new HSSFWorkbook();
            int sheetRow = PublicConst.EachSheetDataRowsCount;
            int pageCount = (list.Count - 1) / sheetRow + 1;
            for (int page = 1; page <= pageCount; page++)
            {
                ISheet sheet1 = book.CreateSheet("Sheet" + page);

                //给sheet1添加第一行的头部标题
                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("快递公司");
                row1.CreateCell(1).SetCellValue("出库日期");
                row1.CreateCell(2).SetCellValue("出库单号");
                row1.CreateCell(3).SetCellValue("订单备注");
                row1.CreateCell(4).SetCellValue("发货仓库");
                row1.CreateCell(5).SetCellValue("收货省份");
                row1.CreateCell(6).SetCellValue("收货州/市");
                row1.CreateCell(7).SetCellValue("收货县/区");
                row1.CreateCell(8).SetCellValue("收货地址");
                row1.CreateCell(9).SetCellValue("物流运单号");
                row1.CreateCell(10).SetCellValue("重量(KG)");

                //设置顶部标题样式
                SetCellStyle(book, row1, 11);

                int rowCount = 0;
                for (int i = (page - 1) * sheetRow; i < page * sheetRow; i++)
                {
                    if (i >= list.Count)
                    {
                        break;
                    }
                    IRow rowtemp = sheet1.CreateRow(rowCount + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].CarrierName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].OutboundDateText);
                    rowtemp.CreateCell(2).SetCellValue(list[i].OutboundOrder);
                    rowtemp.CreateCell(3).SetCellValue(list[i].Remark);
                    rowtemp.CreateCell(4).SetCellValue(list[i].WarehouseName);
                    rowtemp.CreateCell(5).SetCellValue(list[i].ConsigneeProvince);
                    rowtemp.CreateCell(6).SetCellValue(list[i].ConsigneeCity);
                    rowtemp.CreateCell(7).SetCellValue(list[i].ConsigneeArea);
                    rowtemp.CreateCell(8).SetCellValue(list[i].ConsigneeAddress);
                    rowtemp.CreateCell(9).SetCellValue(list[i].CarrierNumber);
                    rowtemp.CreateCell(10).SetCellValue((double)list[i].Weight);
                    rowCount++;
                }
            }
            return book;
        }


        /// <summary>
        /// 出库捡货工时报表导出
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook PickingTimeSpanReportExport(List<PickingTimeSpanGlobalDto> list)
        {
            //创建Excel文件的对象
            HSSFWorkbook book = new HSSFWorkbook();
            int sheetRow = PublicConst.EachSheetDataRowsCount;
            int pageCount = (list.Count - 1) / sheetRow + 1;
            for (int page = 1; page <= pageCount; page++)
            {
                ISheet sheet1 = book.CreateSheet("Sheet" + page);

                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("所属仓库");
                row1.CreateCell(1).SetCellValue("出库单号");
                row1.CreateCell(2).SetCellValue("商品种类");
                row1.CreateCell(3).SetCellValue("总数量");
                row1.CreateCell(4).SetCellValue("操作人");
                row1.CreateCell(5).SetCellValue("开始时间");
                row1.CreateCell(6).SetCellValue("结束时间");
                row1.CreateCell(7).SetCellValue("用时");

                //设置顶部标题样式
                SetCellStyle(book, row1, 8);

                int rowCount = 0;
                for (int i = (page - 1) * sheetRow; i < page * sheetRow; i++)
                {
                    if (i >= list.Count)
                    {
                        break;
                    }
                    IRow rowtemp = sheet1.CreateRow(rowCount + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].OutboundOrder);
                    rowtemp.CreateCell(2).SetCellValue((double)list[i].SkuTypeNumber);
                    rowtemp.CreateCell(3).SetCellValue((double)list[i].SkuTotalCount);
                    rowtemp.CreateCell(4).SetCellValue(list[i].Operator);
                    rowtemp.CreateCell(5).SetCellValue(list[i].StartTimeDisplay);
                    rowtemp.CreateCell(6).SetCellValue(list[i].EndTimeDisplay);
                    rowtemp.CreateCell(7).SetCellValue(list[i].StayTime);
                    rowCount++;
                }
            }
            return book;
        }


        /// <summary>
        /// 出库复核工时报表导出
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook OutboundTransferOrderReportExport(List<OutboundTransferOrderGlobalDto> list)
        {
            //创建Excel文件的对象
            HSSFWorkbook book = new HSSFWorkbook();
            int sheetRow = PublicConst.EachSheetDataRowsCount;
            int pageCount = (list.Count - 1) / sheetRow + 1;
            for (int page = 1; page <= pageCount; page++)
            {
                ISheet sheet1 = book.CreateSheet("Sheet" + page);

                //给sheet1添加第一行的头部标题
                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("所属仓库");
                row1.CreateCell(1).SetCellValue("出库单号");
                row1.CreateCell(2).SetCellValue("商品种类");
                row1.CreateCell(3).SetCellValue("总数量");
                row1.CreateCell(4).SetCellValue("操作人");
                row1.CreateCell(5).SetCellValue("开始时间");
                row1.CreateCell(6).SetCellValue("结束时间");
                row1.CreateCell(7).SetCellValue("用时");

                //设置顶部标题样式
                SetCellStyle(book, row1, 8);

                int rowCount = 0;
                for (int i = (page - 1) * sheetRow; i < page * sheetRow; i++)
                {
                    if (i >= list.Count)
                    {
                        break;
                    }
                    IRow rowtemp = sheet1.CreateRow(rowCount + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].OutboundOrder);
                    rowtemp.CreateCell(2).SetCellValue((double)list[i].SkuTypeNumber);
                    rowtemp.CreateCell(3).SetCellValue((double)list[i].SkuTotalCount);
                    rowtemp.CreateCell(4).SetCellValue(list[i].Operator);
                    rowtemp.CreateCell(5).SetCellValue(list[i].StartTimeDisplay);
                    rowtemp.CreateCell(6).SetCellValue(list[i].EndTimeDisplay);
                    rowtemp.CreateCell(7).SetCellValue(list[i].StayTime);
                    rowCount++;
                }
            }
            return book;
        }



        /// <summary>
        /// 出库单商品汇总报表导出
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook OutboundSkuReportExport(List<OutboundSkuGlobalDto> list)
        {
            //创建Excel文件的对象
            HSSFWorkbook book = new HSSFWorkbook();
            int sheetRow = PublicConst.EachSheetDataRowsCount;
            int pageCount = (list.Count - 1) / sheetRow + 1;
            for (int page = 1; page <= pageCount; page++)
            {
                ISheet sheet1 = book.CreateSheet("Sheet" + page);

                //给sheet1添加第一行的头部标题
                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("站点编号");
                row1.CreateCell(1).SetCellValue("市");
                row1.CreateCell(2).SetCellValue("区县");
                row1.CreateCell(3).SetCellValue("服务综合体名称");
                row1.CreateCell(4).SetCellValue("出库单号");
                row1.CreateCell(5).SetCellValue("出库时间");
                row1.CreateCell(6).SetCellValue("发货仓库");
                row1.CreateCell(7).SetCellValue("发货种类");
                row1.CreateCell(8).SetCellValue("发货数量");
                row1.CreateCell(9).SetCellValue("退货数量");

                //设置顶部标题样式
                SetCellStyle(book, row1, 10);

                int rowCount = 0;
                for (int i = (page - 1) * sheetRow; i < page * sheetRow; i++)
                {
                    if (i >= list.Count)
                    {
                        break;
                    }
                    IRow rowtemp = sheet1.CreateRow(rowCount + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].ServiceStationCode);
                    rowtemp.CreateCell(1).SetCellValue(list[i].ConsigneeCity);
                    rowtemp.CreateCell(2).SetCellValue(list[i].ConsigneeArea);
                    rowtemp.CreateCell(3).SetCellValue(list[i].ServiceStationName);
                    rowtemp.CreateCell(4).SetCellValue(list[i].OutboundOrder);
                    rowtemp.CreateCell(5).SetCellValue(list[i].ActualShipDateDisplay);
                    rowtemp.CreateCell(6).SetCellValue(list[i].WarehouseName);
                    rowtemp.CreateCell(7).SetCellValue(list[i].SkuType);
                    rowtemp.CreateCell(8).SetCellValue(list[i].SkuCount);
                    rowtemp.CreateCell(9).SetCellValue(list[i].ReturnQty);

                    rowCount++;
                }
            }
            return book;
        }


        /// <summary>
        /// 农资出库商品报表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HSSFWorkbook FertilizerOutboundSkuReportExport(List<FertilizerOutboundSkuGlobalDto> list)
        {
            HSSFWorkbook book = new HSSFWorkbook();
            ISheet sheet1 = book.CreateSheet("Sheet1");
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("所属仓库");
            row1.CreateCell(1).SetCellValue("服务综合体编码");
            row1.CreateCell(2).SetCellValue("服务综合体名称");
            row1.CreateCell(3).SetCellValue("出库单号");
            row1.CreateCell(4).SetCellValue("出库时间");
            row1.CreateCell(5).SetCellValue("商品名称");
            row1.CreateCell(6).SetCellValue("UPC");
            row1.CreateCell(7).SetCellValue("发货数量");
            row1.CreateCell(8).SetCellValue("退货数量");

            SetCellStyle(book, row1, 9);

            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].WarehouseName);
                rowtemp.CreateCell(1).SetCellValue(list[i].ServiceStationCode);
                rowtemp.CreateCell(2).SetCellValue(list[i].ServiceStationName);
                rowtemp.CreateCell(3).SetCellValue(list[i].OutboundOrder);
                rowtemp.CreateCell(4).SetCellValue(list[i].ActualShipDateDisplay);
                rowtemp.CreateCell(5).SetCellValue(list[i].SkuName);
                rowtemp.CreateCell(6).SetCellValue(list[i].UPC);
                rowtemp.CreateCell(7).SetCellValue(list[i].ShippedQty);
                rowtemp.CreateCell(8).SetCellValue(list[i].ReturnQty);
            }
            return book;
        }


        public static HSSFWorkbook ChannelInventoryExport(List<ChannelInventoryGlobalDto> list)
        {
            HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            ISheet sheet1 = book.CreateSheet("Sheet1");

            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("仓库名称");
            row1.CreateCell(1).SetCellValue("商品名称");
            row1.CreateCell(2).SetCellValue("商品UPC");

            row1.CreateCell(3).SetCellValue("外部编号");
            row1.CreateCell(4).SetCellValue("渠道");
            row1.CreateCell(5).SetCellValue("库存数量");

            row1.CreateCell(6).SetCellValue("冻结数量");
            row1.CreateCell(7).SetCellValue("分配数量");
            row1.CreateCell(8).SetCellValue("拣货数量");

            //设置顶部标题样式
            SetCellStyle(book, row1, 9);

            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].WareHouseName);
                rowtemp.CreateCell(1).SetCellValue(list[i].SkuName);
                rowtemp.CreateCell(2).SetCellValue(list[i].UPC);

                rowtemp.CreateCell(3).SetCellValue(list[i].OtherId);
                rowtemp.CreateCell(4).SetCellValue(list[i].Channel);
                rowtemp.CreateCell(5).SetCellValue((double)list[i].Qty);

                rowtemp.CreateCell(6).SetCellValue((double)list[i].FrozenQty);
                rowtemp.CreateCell(7).SetCellValue((double)list[i].AllocatedQty);
                rowtemp.CreateCell(8).SetCellValue((double)list[i].PickedQty);
            }
            return book;
        }
    }
}