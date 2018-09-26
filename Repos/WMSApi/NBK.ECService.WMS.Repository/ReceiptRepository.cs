using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using MySql.Data.MySqlClient;
using System.Text;

namespace NBK.ECService.WMS.Repository
{
    public class ReceiptRepository : CrudRepository, IReceiptRepository
    {
        /// <param name="dbContextProvider"></param>
        public ReceiptRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// 根据RceiptSysId 获取 明细和 货品的批次属性
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public List<ReceiptDetailOperationDto> GetReceiptDetailOperationDtoBySysId(Guid sysId)
        {
            var query = from rd in Context.receiptdetails
                        join s in Context.skus on rd.SkuSysId equals s.SysId
                        where rd.ReceiptSysId == sysId
                        select new ReceiptDetailOperationDto
                        {
                            SysId = rd.SysId,
                            SkuSysId = rd.SkuSysId,

                        };
            return query.ToList();
        }

        /// <summary>
        /// 获取收货单列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public Pages<ReceiptListDto> GetReceiptListByPaging(ReceiptQuery receiptQuery)
        {
            var receiptDetail = from rd in Context.receiptdetails
                                group rd by new { rd.ReceiptSysId } into g
                                select new
                                {
                                    ReceiptSysId = g.Key.ReceiptSysId,
                                    TotalReceivedQty = g.Sum(x => x.ReceivedQty),
                                    TotalShelvesQty = g.Sum(x => x.ShelvesQty)
                                };

            var query = from r in Context.receipts
                        join p in Context.purchases on r.ExternalOrder equals p.PurchaseOrder
                        join v in Context.vendors on p.VendorSysId equals v.SysId
                        join trd in receiptDetail on r.SysId equals trd.ReceiptSysId into temprd
                        from rd in temprd.DefaultIfEmpty()

                        join trd1 in Context.receiptdetails on r.SysId equals trd1.ReceiptSysId into temprd1
                        from rd1 in temprd1.DefaultIfEmpty()

                        join s in Context.skus on rd1.SkuSysId equals s.SysId into sTemp
                        from sku in sTemp.DefaultIfEmpty()
                        where r.Status != 0
                        select new { r, v.VendorName, rd, sku };

            query = query.Where(p => p.r.WarehouseSysId == receiptQuery.WarehouseSysId);

            if (!receiptQuery.ReceiptOrderSearch.IsNull())
            {
                receiptQuery.ReceiptOrderSearch = receiptQuery.ReceiptOrderSearch.Trim();
                query = query.Where(p => p.r.ReceiptOrder == receiptQuery.ReceiptOrderSearch);
            }
            if (!receiptQuery.VendorNameSearch.IsNull())
            {
                receiptQuery.VendorNameSearch = receiptQuery.VendorNameSearch.Trim();
                query = query.Where(p => p.VendorName.Contains(receiptQuery.VendorNameSearch));
            }
            if (receiptQuery.StatusSearch.HasValue)
            {
                query = query.Where(p => p.r.Status == receiptQuery.StatusSearch.Value);
            }
            if (!receiptQuery.ExternalOrderSearch.IsNull())
            {
                receiptQuery.ExternalOrderSearch = receiptQuery.ExternalOrderSearch.Trim();
                query = query.Where(p => p.r.ExternalOrder == receiptQuery.ExternalOrderSearch);
            }
            if (receiptQuery.ReceiptDateFromSearch.HasValue && receiptQuery.ReceiptDateToSearch.HasValue)
            {
                query = query.Where(p => receiptQuery.ReceiptDateFromSearch <= p.r.ReceiptDate && p.r.ReceiptDate <= receiptQuery.ReceiptDateToSearch);
            }
            if (receiptQuery.CreateDateFromSearch.HasValue && receiptQuery.CreateDateToSearch.HasValue)
            {
                query = query.Where(p => receiptQuery.CreateDateFromSearch <= p.r.CreateDate && p.r.CreateDate <= receiptQuery.CreateDateToSearch);
            }
            if (receiptQuery.IsMaterial.HasValue)
            {
                query = query.Where(p => p.sku.IsMaterial == receiptQuery.IsMaterial.Value);
            }

            var receipts = query.Select(p => new ReceiptListDto()
            {
                SysId = p.r.SysId,
                ReceiptOrder = p.r.ReceiptOrder,
                //DisplayExternalOrder = p.r.DisplayExternalOrder,
                VendorName = p.VendorName,
                ReceiptType = p.r.ReceiptType,
                Status = p.r.Status,
                ExpectedReceiptDate = p.r.ExpectedReceiptDate,
                ExternalOrder = p.r.ExternalOrder,
                ReceiptDate = p.r.ReceiptDate,
                CreateDate = p.r.CreateDate,
                TotalReceivedQty = p.rd.TotalReceivedQty,
                TotalShelvesQty = p.rd.TotalShelvesQty
            }).Distinct();
            receiptQuery.iTotalDisplayRecords = receipts.Count();
            receipts = receipts.OrderByDescending(p => p.CreateDate).Skip(receiptQuery.iDisplayStart).Take(receiptQuery.iDisplayLength);
            return ConvertPages(receipts, receiptQuery);
        }

        /// <summary>
        /// 根据Id获取收货单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ReceiptViewDto GetReceiptViewById(Guid sysId)
        {
            var query = from r in Context.receipts
                        join v in Context.vendors on r.VendorSysId equals v.SysId
                        where r.SysId == sysId
                        select new { r, VendorName = v.VendorName };
            var receipt = query.Select(p => new ReceiptViewDto()
            {
                SysId = p.r.SysId,
                ReceiptOrder = p.r.ReceiptOrder,
                //DisplayExternalOrder = p.r.DisplayExternalOrder,
                ExternalOrder = p.r.ExternalOrder,
                ReceiptType = p.r.ReceiptType,
                WarehouseSysId = p.r.WarehouseSysId,
                ExpectedReceiptDate = p.r.ExpectedReceiptDate,
                ReceipDate = p.r.ReceiptDate,
                Status = p.r.Status,
                Descr = p.r.Descr,
                ReturnDescr = p.r.ReturnDescr,
                CreateBy = p.r.CreateBy,
                CreateDate = p.r.CreateDate,
                UpdateBy = p.r.UpdateBy,
                UpdateDate = p.r.UpdateDate,
                IsActive = p.r.IsActive,
                VendorId = p.r.VendorSysId,
                VendorName = p.VendorName,
                ClosedDate = p.r.ClosedDate,
                ArrivalDate = p.r.ArrivalDate,
                TotalExpectedQty = p.r.TotalExpectedQty,
                TotalReceivedQty = p.r.TotalReceivedQty,
                TotalRejectedQty = p.r.TotalRejectedQty
            }).Distinct().FirstOrDefault();
            return receipt;
        }

        /// <summary>
        /// 获取收货清单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        public List<ReceiptDetailViewDto> GetReceiptDetailViewList(Guid receiptSysId)
        {
            var query = from rd in Context.receiptdetails
                        join s in Context.skus on rd.SkuSysId equals s.SysId
                        join u in Context.uoms on rd.UOMSysId equals u.SysId into t1
                        where rd.ReceiptSysId == receiptSysId
                        from ut1 in t1.DefaultIfEmpty()
                        join p in Context.packs on s.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        join uu in Context.uoms on p1.FieldUom02 equals uu.SysId into t3
                        from uu1 in t3.DefaultIfEmpty()
                        join r in Context.receipts on rd.ReceiptSysId equals r.SysId into t4
                        from r1 in t4.DefaultIfEmpty()
                        join pu in Context.purchases on r1.ExternalOrder equals pu.PurchaseOrder into t5
                        from pu1 in t5.DefaultIfEmpty()
                        join pd in Context.purchasedetails on new { PurchaseSysId = pu1.SysId, rd.SkuSysId } equals new { pd.PurchaseSysId, pd.SkuSysId } into t6
                        from pd1 in t6.DefaultIfEmpty()
                        select new { rd, s, UOMDescr = ut1.UOMCode, p1, uu1, pd1 };
            var receiptDetails = query.Select(p => new ReceiptDetailViewDto()
            {
                SysId = p.rd.SysId,
                ReceiptSysId = p.rd.ReceiptSysId,
                SkuSysId = p.rd.SkuSysId,
                ExpectedQty = p.rd.ExpectedQty,
                ReceivedQty = p.rd.ReceivedQty,
                RejectedQty = p.rd.RejectedQty,
                Remark = p.rd.Remark,
                UOMSysId = p.rd.UOMSysId,
                PackSysId = p.rd.PackSysId,
                SkuCode = p.s.SkuCode,
                SkuName = p.s.SkuName,
                SkuUPC = p.s.UPC,
                OtherId = p.s.OtherId,
                SkuDescr = p.s.SkuDescr,
                UOMDescr = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0 ? p.uu1.UOMCode : p.UOMDescr,
                //UOMDescr = p.uu1.UOMCode ?? p.UOMDescr,
                //ShelvesStatus = p.rd.ShelvesStatus,
                ShelvesQty = p.rd.ShelvesQty,
                PackFactor = p.pd1.PackFactor,
                UPC01 = p.p1.UPC01,
                UPC02 = p.p1.UPC02,
                UPC03 = p.p1.UPC03,
                UPC04 = p.p1.UPC04,
                UPC05 = p.p1.UPC05,
                FieldValue01 = p.p1.FieldValue01,
                FieldValue02 = p.p1.FieldValue02,
                FieldValue03 = p.p1.FieldValue03,
                FieldValue04 = p.p1.FieldValue04,
                FieldValue05 = p.p1.FieldValue05,
                DisplayExpectedQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * (p.rd.ExpectedQty.HasValue ? p.rd.ExpectedQty.Value : 0) * 1.00m) / p.p1.FieldValue01.Value), 3) : (p.rd.ExpectedQty.HasValue ? p.rd.ExpectedQty.Value : 0),
                DisplayReceivedQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * (p.rd.ReceivedQty.HasValue ? p.rd.ReceivedQty.Value : 0) * 1.00m) / p.p1.FieldValue01.Value), 3) : (p.rd.ReceivedQty.HasValue ? p.rd.ReceivedQty.Value : 0),
                DisplayRejectedQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * (p.rd.RejectedQty.HasValue ? p.rd.RejectedQty.Value : 0) * 1.00m) / p.p1.FieldValue01.Value), 3) : (p.rd.RejectedQty.HasValue ? p.rd.RejectedQty.Value : 0),
                DisplayShelvesQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * p.rd.ShelvesQty * 1.00m) / p.p1.FieldValue01.Value), 3) : p.rd.ShelvesQty

            }).Distinct().OrderBy(p => p.SkuName).ToList();
            return receiptDetails;
        }

        public List<ReceiptDetailCheckLotDto> GetToLotByReceiptDetail(List<receiptdetail> receiptDetails, Guid wareHouseSysId)
        {
            var query = from receiptDetail in receiptDetails
                        join rd in Context.receiptdetails on new
                        {
                            SkuSysId = receiptDetail.SkuSysId,
                            LotAttr01 = receiptDetail.LotAttr01 ?? string.Empty,
                            LotAttr02 = receiptDetail.LotAttr02 ?? string.Empty,
                            LotAttr03 = receiptDetail.LotAttr03,
                            LotAttr04 = receiptDetail.LotAttr04,
                            LotAttr05 = receiptDetail.LotAttr05,
                            LotAttr06 = receiptDetail.LotAttr06,
                            LotAttr07 = receiptDetail.LotAttr07,
                            LotAttr08 = receiptDetail.LotAttr08,
                            LotAttr09 = receiptDetail.LotAttr09,
                            ProduceDate = receiptDetail.ProduceDate,
                            ExternalLot = receiptDetail.ExternalLot,
                            ExpiryDate = receiptDetail.ExpiryDate
                        } equals new
                        {
                            SkuSysId = rd.SkuSysId,
                            LotAttr01 = rd.LotAttr01 ?? string.Empty,
                            LotAttr02 = rd.LotAttr02 ?? string.Empty,
                            LotAttr03 = rd.LotAttr03,
                            LotAttr04 = rd.LotAttr04,
                            LotAttr05 = rd.LotAttr05,
                            LotAttr06 = rd.LotAttr06,
                            LotAttr07 = rd.LotAttr07,
                            LotAttr08 = rd.LotAttr08,
                            LotAttr09 = rd.LotAttr09,
                            ProduceDate = rd.ProduceDate,
                            ExternalLot = rd.ExternalLot,
                            ExpiryDate = rd.ExpiryDate
                        } into t
                        from ti in t.DefaultIfEmpty()
                        join r in Context.receipts on ti.ReceiptSysId equals r.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        where ti1.ReceiptType != (int)ReceiptType.FIFO && ti1.WarehouseSysId == wareHouseSysId
                        select new ReceiptDetailCheckLotDto()
                        {
                            SysId = receiptDetail.SysId,
                            CheckLotSysId = ti.SysId,
                            ToLot = ti.ToLot,
                            ToLoc = ti.ToLoc,
                            ToLpn = ti.ToLpn,
                        };

            return query.ToList();
        }

        #region TODO于17.9.28号发现此方法暂无使用所以注释掉

        //public void CreateReceipt(receipt receipt)
        //{
        //    string insertSql = string.Empty;

        //    var paraList = new List<MySqlParameter>();

        //    if (receipt.ExpectedReceiptDate.HasValue)
        //    {
        //        insertSql = $@"
        //            INSERT INTO receipt(SysId, ReceiptOrder, ExternalOrder, ReceiptType, WarehouseSysId, 
        //              ExpectedReceiptDate,  Status, Descr, ReturnDescr, 
        //              CreateBy, CreateDate, UpdateBy, UpdateDate, IsActive, 
        //              VendorSysId, ArrivalDate, TotalExpectedQty, TotalReceivedQty, 
        //              TotalRejectedQty, CreateUserName, UpdateUserName)
        //            SELECT @SysId ,@ReceiptOrder,@ExternalOrder,@ReceiptType,@WarehouseSysId, 
        //              @ExpectedReceiptDate,  @Status  ,@Descr,@ReturnDescr, 
        //              @CreateBy,@CreateDate,@UpdateBy, @UpdateDate,@IsActive, 
        //              @VendorSysId, @ArrivalDate, @TotalExpectedQty,@TotalReceivedQty, 
        //              @TotalRejectedQty,@CreateUserName, @UpdateUserName 
        //            FROM dual WHERE NOT EXISTS(SELECT * FROM receipt WHERE ReceiptOrder =@ReceiptOrder)";

        //        paraList.Add(new MySqlParameter($"@ExpectedReceiptDate", receipt.ExpectedReceiptDate.ToString()));
        //    }
        //    else
        //    {
        //        insertSql = $@"
        //            INSERT INTO receipt(SysId, ReceiptOrder, ExternalOrder, ReceiptType, WarehouseSysId, 
        //                Status, Descr, ReturnDescr, 
        //              CreateBy, CreateDate, UpdateBy, UpdateDate, IsActive, 
        //              VendorSysId, ArrivalDate, TotalExpectedQty, TotalReceivedQty, 
        //              TotalRejectedQty, CreateUserName, UpdateUserName)
        //            SELECT  @SysId,@ReceiptOrder,@ExternalOrder,@ReceiptType,@WarehouseSysId, 
        //               @Status , @Descr ,@ReturnDescr , 
        //              @CreateBy,@CreateDate,@UpdateBy,@UpdateDate,@IsActive, 
        //              @VendorSysId,@ArrivalDate,@TotalExpectedQty,@TotalReceivedQty, 
        //              @TotalRejectedQty,@CreateUserName, @UpdateUserName 
        //            FROM dual WHERE NOT EXISTS(SELECT * FROM receipt WHERE ReceiptOrder =@ReceiptOrder)";
        //    }


        //    paraList.Add(new MySqlParameter($"@SysId", receipt.SysId));
        //    paraList.Add(new MySqlParameter($"@ReceiptOrder", receipt.ReceiptOrder));
        //    paraList.Add(new MySqlParameter($"@ExternalOrder", receipt.ExternalOrder));
        //    paraList.Add(new MySqlParameter($"@ReceiptType", receipt.ReceiptType));
        //    paraList.Add(new MySqlParameter($"@WarehouseSysId", receipt.WarehouseSysId));
        //    paraList.Add(new MySqlParameter($"@Status", receipt.Status));
        //    paraList.Add(new MySqlParameter($"@Descr", receipt.Descr));
        //    paraList.Add(new MySqlParameter($"@ReturnDescr", receipt.ReturnDescr));
        //    paraList.Add(new MySqlParameter($"@CreateBy", receipt.CreateBy));
        //    paraList.Add(new MySqlParameter($"@CreateDate", receipt.CreateDate));
        //    paraList.Add(new MySqlParameter($"@UpdateBy", receipt.UpdateBy));
        //    paraList.Add(new MySqlParameter($"@UpdateDate", receipt.UpdateDate));
        //    paraList.Add(new MySqlParameter($"@IsActive", receipt.IsActive));
        //    paraList.Add(new MySqlParameter($"@VendorSysId", receipt.VendorSysId));
        //    paraList.Add(new MySqlParameter($"@ArrivalDate", receipt.ArrivalDate));
        //    paraList.Add(new MySqlParameter($"@TotalExpectedQty", receipt.TotalExpectedQty));
        //    paraList.Add(new MySqlParameter($"@TotalReceivedQty", receipt.TotalReceivedQty));
        //    paraList.Add(new MySqlParameter($"@TotalRejectedQty", receipt.TotalRejectedQty));
        //    paraList.Add(new MySqlParameter($"@CreateUserName", receipt.CreateUserName));
        //    paraList.Add(new MySqlParameter($"@UpdateUserName", receipt.UpdateUserName));
        //    paraList.Add(new MySqlParameter($"@ReceiptOrder", receipt.ReceiptOrder));

        //    var insertRows = Context.Database.ExecuteSqlCommand(insertSql, paraList.ToArray());
        //    if (insertRows == 0)
        //    {
        //        throw new Exception("已创建相同收货单，请检查!");
        //    }
        //}
        #endregion 

        /// <summary>
        /// 获取领料记录
        /// </summary>
        /// <param name="pickingQuery"></param>
        /// <returns></returns>
        public Pages<PickingMaterialListDto> GetPickingMaterialList(PickingMaterialQuery pickingQuery)
        {
            var query = from pr in Context.pickingrecords
                        join s in Context.skus on pr.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        select new { pr, s, p1 };

            query = query.Where(x => x.pr.ReceiptSysId == pickingQuery.ReceiptSysId);
            if (!string.IsNullOrEmpty(pickingQuery.PickingUserName))
            {
                pickingQuery.PickingUserName = pickingQuery.PickingUserName.Trim();
                query = query.Where(x => x.pr.PickingUserName == pickingQuery.PickingUserName);
            }

            if (pickingQuery.PickingNumber.HasValue)
            {
                query = query.Where(x => x.pr.PickingNumber == pickingQuery.PickingNumber);
            }

            var pickings = query.Select(p => new PickingMaterialListDto
            {
                SysId = p.pr.SysId,
                ReceiptSysId = p.pr.ReceiptSysId,
                ReceiptOrder = p.pr.ReceiptOrder,
                SkuSysId = p.pr.SkuSysId,
                UPC = p.s.UPC,
                SkuName = p.s.SkuName,
                PickingNumber = p.pr.PickingNumber,
                PickingUserName = p.pr.PickingUserName,
                PickingDate = p.pr.PickingDate,
                DisplayQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * (p.pr.Qty) * 1.00m) / p.p1.FieldValue01.Value), 3) : (p.pr.Qty),
            }).Distinct();

            pickingQuery.iTotalDisplayRecords = query.Count();
            pickings = pickings.OrderByDescending(p => p.PickingUserName).ThenByDescending(x => x.PickingNumber).Skip(pickingQuery.iDisplayStart).Take(pickingQuery.iDisplayLength);
            return ConvertPages(pickings, pickingQuery);
        }

        /// <summary>
        /// 取消收货，删除  Receiptsn 相关数据
        /// </summary>
        /// <param name="purchaseSysId"></param>
        public void CancelReceiptsnByPurchase(Guid purchaseSysId)
        {
            string sql = $@"DELETE FROM receiptsn  WHERE purchaseSysId = @purchaseSysId ";

            base.Context.Database.ExecuteSqlCommand(sql, new MySqlParameter($"@purchaseSysId", purchaseSysId));
        }

        /// <summary>
        /// 批量插入SN
        /// </summary>
        /// <param name="snlist"></param>
        public void BatchInsertReceiptSN(List<receiptsn> snlist)
        {
            if (snlist != null && snlist.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"INSERT INTO receiptsn(SysId, ReceiptSysId, SkuSysId, SN,  purchaseSysId, 
                                    CreateDate, CreateUserName, CreateBy, WarehouseSysId, 
                                    status, UpdateBy, UpdateDate, UpdateUserName) VALUES ");


                var paraList = new List<MySqlParameter>();
                int i = 0;
                snlist.ForEach(info =>
                {
                    sb.Append($@" ( @SysId{i},@ReceiptSysId{i},@SkuSysId{i},@SN{i},@PurchaseSysId{i}, 
                                  NOW(),@CreateUserName{i},@CreateBy{i},@WarehouseSysId{i},@Status{i},
                                  @UpdateBy{i},NOW(),@UpdateUserName{i} ),");

                    paraList.Add(new MySqlParameter($"@SysId{i}", info.SysId));
                    paraList.Add(new MySqlParameter($"@ReceiptSysId{i}", info.ReceiptSysId));
                    paraList.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                    paraList.Add(new MySqlParameter($"@SN{i}", info.SN));
                    paraList.Add(new MySqlParameter($"@PurchaseSysId{i}", info.PurchaseSysId));
                    paraList.Add(new MySqlParameter($"@CreateUserName{i}", info.CreateUserName));
                    paraList.Add(new MySqlParameter($"@CreateBy{i}", info.CreateBy));
                    paraList.Add(new MySqlParameter($"@WarehouseSysId{i}", info.WarehouseSysId));
                    paraList.Add(new MySqlParameter($"@Status{i}", info.Status));
                    paraList.Add(new MySqlParameter($"@UpdateBy{i}", info.UpdateBy));
                    paraList.Add(new MySqlParameter($"@UpdateUserName{i}", info.UpdateUserName));

                    //sb.Append($@" '{p.SysId}', '{p.ReceiptSysId}', '{p.SkuSysId}', '{p.SN}', '{p.PurchaseSysId}',
                    //NOW(), '{p.CreateUserName}', { p.CreateBy}, 
                    //'{p.WarehouseSysId}', { p.Status}, { p.UpdateBy}, NOW(), '{p.UpdateUserName}'),");

                    i++;
                });

                string sql = sb.ToString().Trim(',') + ";";

                base.Context.Database.ExecuteSqlCommand(sql, paraList.ToArray());
            }
        }

        #region 批次采集
        /// <summary>
        /// 批次采集根据商品获取收货明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        public List<ReceiptDetailViewDto> GetReceiptDetailCollectionLotViewList(ReceiptCollectionLotQuery request)
        {
            var query = from rd in Context.receiptdetails
                        join s in Context.skus on rd.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        where rd.ReceiptSysId == request.ReceiptSysId
                        select new { rd, s, p1 };

            if (request.SkuSysId.HasValue)
            {
                query = query.Where(x => x.rd.SkuSysId == request.SkuSysId);
            }
            if (!string.IsNullOrEmpty(request.UPC))
            {
                request.UPC = request.UPC.Trim();
                query = query.Where(x => x.s.UPC == request.UPC);
            }

            var receiptDetails = query.Select(p => new ReceiptDetailViewDto()
            {
                SysId = p.rd.SysId,
                ReceiptSysId = p.rd.ReceiptSysId,
                SkuSysId = p.rd.SkuSysId,
                ExpectedQty = p.rd.ExpectedQty,
                ReceivedQty = p.rd.ReceivedQty,
                SkuCode = p.s.SkuCode,
                SkuName = p.s.SkuName,
                SkuUPC = p.s.UPC,
                SkuDescr = p.s.SkuDescr,
                ToLot = p.rd.ToLot,
                IsDefaultLot = p.rd.IsDefaultLot,
                LotAttr01 = p.rd.LotAttr01,
                LotAttr02 = p.rd.LotAttr02,
                LotAttr03 = p.rd.LotAttr03,
                LotAttr04 = p.rd.LotAttr04,
                LotAttr05 = p.rd.LotAttr05,
                LotAttr06 = p.rd.LotAttr06,
                LotAttr07 = p.rd.LotAttr07,
                LotAttr08 = p.rd.LotAttr08,
                LotAttr09 = p.rd.LotAttr09,
                ExternalLot = p.rd.ExternalLot,
                ProduceDate = p.rd.ProduceDate,
                ExpiryDate = p.rd.ExpiryDate,
                DisplayExpectedQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * (p.rd.ExpectedQty.HasValue ? p.rd.ExpectedQty.Value : 0) * 1.00m) / p.p1.FieldValue01.Value), 3) : (p.rd.ExpectedQty.HasValue ? p.rd.ExpectedQty.Value : 0),
                DisplayReceivedQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * (p.rd.ReceivedQty.HasValue ? p.rd.ReceivedQty.Value : 0) * 1.00m) / p.p1.FieldValue01.Value), 3) : (p.rd.ReceivedQty.HasValue ? p.rd.ReceivedQty.Value : 0),
            }).Distinct().ToList();

            return receiptDetails;
        }
        #endregion
    }
}