using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.EntityFramework;
using MySql.Data.MySqlClient;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.DTO;
using System;
using MySql.Data.MySqlClient;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.DTO.InvLotLocLpn;
using System.Data.Entity.Infrastructure;
using System.Data;
using NBK.ECService.WMS.DTO.MQ;

namespace NBK.ECService.WMS.Repository
{
    public class WMSSqlRepository : CrudRepository, IWMSSqlRepository
    {
        public WMSSqlRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// 更新库存(上架)
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryQtyByShelves(List<UpdateInventoryDto> updateInventoryList)
        {
            if (updateInventoryList != null && updateInventoryList.Count > 0)
            {
                try
                {
                    var updateId = updateInventoryList != null ? updateInventoryList.FirstOrDefault().CurrentUserId : 0;
                    var updateName = updateInventoryList != null ? updateInventoryList.FirstOrDefault().CurrentDisplayName : "";

                    StringBuilder strSql = new StringBuilder();

                    #region invLot
                    var invLotList = updateInventoryList.FindAll(x => x.InvLotSysId != new Guid());
                    if (invLotList != null && invLotList.Count > 0)
                    {
                        foreach (var item in invLotList)
                        {
                            strSql.AppendFormat(" update invLot set Qty = Qty + {0}, UpdateBy = @UpdateBy, UpdateDate = now(), UpdateUserName = @UpdateUserName where sysid = '{1}'; ", item.Qty, item.InvLotSysId);
                        }
                    }
                    #endregion

                    #region invSkuLoc
                    var invSkuLocList = updateInventoryList.FindAll(x => x.InvSkuLocSysId != new Guid());
                    if (invSkuLocList != null && invSkuLocList.Count > 0)
                    {
                        foreach (var item in invSkuLocList)
                        {
                            strSql.AppendFormat(" update invSkuLoc set Qty = Qty + {0}, UpdateBy = @UpdateBy, UpdateDate = now(), UpdateUserName = @UpdateUserName where sysid = '{1}'; ", item.Qty, item.InvSkuLocSysId);
                        }
                    }
                    #endregion

                    #region invLotLocLpn
                    var invLotLocLpnList = updateInventoryList.FindAll(x => x.InvLotLocLpnSysId != new Guid());
                    if (invLotLocLpnList != null && invLotLocLpnList.Count > 0)
                    {
                        foreach (var item in invLotLocLpnList)
                        {
                            strSql.AppendFormat(" update invLotLocLpn set Qty = Qty + {0}, UpdateBy = @UpdateBy, UpdateDate = now(), UpdateUserName = @UpdateUserName where sysid = '{1}'; ", item.Qty, item.InvLotLocLpnSysId);
                        }
                    }
                    #endregion

                    var row = invLotList.Count() + invSkuLocList.Count() + invLotLocLpnList.Count();
                    if (!string.IsNullOrEmpty(strSql.ToString()))
                    {
                        var result = base.Context.Database.ExecuteSqlCommand(strSql.ToString()
                            , new MySqlParameter("@UpdateBy", updateId)
                            , new MySqlParameter("@UpdateUserName", updateName));
                        if (row != result)
                        {
                            throw new Exception("库存数量异常,无法上架");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 更新分配库存数量
        /// </summary>
        /// <param name="updateInventoryDto"></param>
        public CommonResponse UpdateInventoryAllocatedQty(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {


                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET invlotloclpn.AllocatedQty = invlotloclpn.AllocatedQty + {0} ,updateBy = @updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' and invlotloclpn.qty >0 and (invlotloclpn.Qty - invlotloclpn.AllocatedQty - invlotloclpn.PickedQty - invlotloclpn.FrozenQty) >= {1};", info.InvLotLocLpnSysId, info.Qty);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存不足,无法进行分配");

                }
                var strSqlInvLot = new StringBuilder();



                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET invlot.AllocatedQty = invlot.AllocatedQty + {0} ,updateBy = @updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}'  and invlot.qty >0 AND (invlot.Qty - invlot.PickedQty - invlot.AllocatedQty - invlot.FrozenQty) >= {1};", info.InvLotSysId, info.Qty);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("库存不足,无法进行分配");
                }


                var strSqlSkuLoc = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET invSkuLoc.AllocatedQty = invSkuLoc.AllocatedQty + {0} ,updateBy = @updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}'  and invSkuLoc.qty >0  AND (invSkuLoc.Qty - invSkuLoc.PickedQty - invSkuLoc.AllocatedQty - invSkuLoc.FrozenQty) >= {1};", info.InvSkuLocSysId, info.Qty);

                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("库存不足,无法进行分配");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 更新分配库存到拣货库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryPickedByAllocatedQty(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {
                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {

                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET  invlotloclpn.AllocatedQty = invlotloclpn.AllocatedQty - {0} , invlotloclpn.PickedQty = invlotloclpn.PickedQty + {0} ,updateBy = @updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}'and invlotloclpn.AllocatedQty >0 AND invlotloclpn.AllocatedQty>= {1};", info.InvLotLocLpnSysId, info.Qty);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存不足,无法进行分配");

                }
                var strSqlInvLot = new StringBuilder();

                foreach (var info in updateInventoryList)
                {


                    strSqlInvLot.AppendFormat(" UPDATE invlot SET  invlot.AllocatedQty = invlot.AllocatedQty - {0}, invlot.PickedQty = invlot.PickedQty +{0} ,updateBy = @updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}'  and  invlot.AllocatedQty > 0 AND invlot.AllocatedQty >= {1};", info.InvLotSysId, info.Qty);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("库存不足,无法进行分配");
                }


                var strSqlSkuLoc = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET invSkuLoc.AllocatedQty = invSkuLoc.AllocatedQty - {0}, invSkuLoc.PickedQty = invSkuLoc.PickedQty + {0} ,updateBy = @updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}'  AND invSkuLoc.AllocatedQty > 0 AND invSkuLoc.AllocatedQty >= {1};", info.InvSkuLocSysId, info.Qty);

                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("库存不足,无法进行分配");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 更新拣货库存到财务库存发生扣减
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryQtyByPickedQty(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {

                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {

                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET   invlotloclpn.PickedQty = invlotloclpn.PickedQty - {0}, invlotloclpn.Qty = invlotloclpn.Qty - {0}  ,updateBy = @updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' and invlotloclpn.Qty > 0 AND invlotloclpn.Qty >= {1} and invlotloclpn.PickedQty > 0 AND invlotloclpn.PickedQty >= {1};", info.InvLotLocLpnSysId, info.Qty);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存不足,无法进行分配");

                }
                var strSqlInvLot = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET  invlot.PickedQty = invlot.PickedQty - {0} , invlot.Qty = invlot.Qty - {0} ,updateBy = @updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}'  and   invlot.Qty > 0 AND invlot.Qty >= {1} and  invlot.PickedQty > 0 AND invlot.PickedQty >=  {1} ;", info.InvLotSysId, info.Qty);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("库存不足,无法进行分配");
                }


                var strSqlSkuLoc = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET invSkuLoc.PickedQty = invSkuLoc.PickedQty - {0}, invSkuLoc.Qty = invSkuLoc.Qty - {0},updateBy = @updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}'  and  invSkuLoc.Qty > 0 AND invSkuLoc.Qty >= {1} and  invSkuLoc.PickedQty > 0 AND invSkuLoc.PickedQty >= {1} ;", info.InvSkuLocSysId, info.Qty);

                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("库存不足,无法进行分配");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 更新拣货库存到财务库存发生扣减
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryQuickDelivery(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {
                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET invlotloclpn.Qty = invlotloclpn.Qty - {0} ,updateBy = @updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' and invlotloclpn.Qty > 0 AND (invlotloclpn.Qty - invlotloclpn.PickedQty - invlotloclpn.AllocatedQty - invlotloclpn.FrozenQty) >= {1};", info.InvLotLocLpnSysId, info.Qty);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存不足,无法进行分配");

                }
                var strSqlInvLot = new StringBuilder();

                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET invlot.Qty = invlot.Qty - {0} ,updateBy = @updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}' and invlot.Qty > 0 AND (invlot.Qty - invlot.PickedQty - invlot.AllocatedQty - invlot.FrozenQty) >= {1};", info.InvLotSysId, info.Qty);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("库存不足,无法进行分配");
                }

                var strSqlSkuLoc = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET invSkuLoc.Qty = invSkuLoc.Qty - {0} ,updateBy = @updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}' and invSkuLoc.Qty > 0 AND (invSkuLoc.Qty - invSkuLoc.PickedQty - invSkuLoc.AllocatedQty - invSkuLoc.FrozenQty) >= {1};", info.InvSkuLocSysId, info.Qty);

                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("库存不足,无法进行分配");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 更新分配库存数量(取消分配)
        /// </summary>
        /// <param name="updateInventoryDto"></param>
        public CommonResponse UpdateInventoryCancelAllocatedQty(List<UpdateInventoryDto> updateInventoryList)
        {
            try
            {
                var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
                var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;

                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET invlotloclpn.AllocatedQty = invlotloclpn.AllocatedQty - {0}  ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' and (invlotloclpn.AllocatedQty - {1}) >= 0 ;", info.InvLotLocLpnSysId, info.Qty);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存分配数量异常,无法取消分配");

                }
                var strSqlInvLot = new StringBuilder();

                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET invlot.AllocatedQty = invlot.AllocatedQty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}' and (invlot.AllocatedQty - {1}) >= 0;", info.InvLotSysId, info.Qty);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("库存分配数量异常,无法取消分配");
                }


                var strSqlSkuLoc = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET invSkuLoc.AllocatedQty = invSkuLoc.AllocatedQty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}'  and (invSkuLoc.AllocatedQty - {1}) >= 0  ;", info.InvSkuLocSysId, info.Qty);

                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("库存分配数量异常,无法取消分配");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 更新拣货库存数量(加工单拣货)
        /// </summary>
        /// <param name="updateInventoryDto"></param>
        public CommonResponse UpdateInventoryAssemblyPickedQty(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {
                var strSqlInvLocLotLpn = new StringBuilder();

                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET invlotloclpn.PickedQty = invlotloclpn.PickedQty + {0}  ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' and (invlotloclpn.Qty - invlotloclpn.AllocatedQty - invlotloclpn.PickedQty - invlotloclpn.FrozenQty) >= {1} ;", info.InvLotLocLpnSysId, info.Qty);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存不足,无法进行拣货");

                }
                var strSqlInvLot = new StringBuilder();

                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET invlot.PickedQty = invlot.PickedQty +  {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}'  and(invlot.Qty - invlot.AllocatedQty - invlot.PickedQty - invlot.FrozenQty) >= {1} ;", info.InvLotSysId, info.Qty);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("库存不足,无法进行拣货");
                }


                var strSqlSkuLoc = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET invSkuLoc.PickedQty = invSkuLoc.PickedQty + {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}'  and(invSkuLoc.Qty - invSkuLoc.AllocatedQty - invSkuLoc.PickedQty - invSkuLoc.FrozenQty) >= {1} ;", info.InvSkuLocSysId, info.Qty);

                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("库存不足,无法进行拣货");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 更新出库明细
        /// </summary>
        /// <param name="updateOutboundDto"></param>
        /// <returns></returns>
        public CommonResponse UpdateOutboundDetailQuickDelivery(List<UpdateOutboundDto> updateOutboundDto)
        {
            try
            {
                var updateId = updateOutboundDto != null ? updateOutboundDto.FirstOrDefault().CurrentUserId : 0;
                var updateName = updateOutboundDto != null ? updateOutboundDto.FirstOrDefault().CurrentDisplayName : "";

                const string updateSql =
              " update OutboundDetail set Status={2}, ShippedQty={0},PickedQty={0},AllocatedQty={0},UpdateDate=now(),UpdateBy=@UpdateBy,UpdateUserName=@UpdateUserName WHERE SysId='{1}'; ";
                var strSql = new StringBuilder();
                foreach (var info in updateOutboundDto)
                {
                    strSql.AppendFormat(updateSql, info.Qty, info.SysId, info.Status);
                }
                base.Context.Database.ExecuteSqlCommand(strSql.ToString()
                    , new MySqlParameter("@UpdateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
            }
            catch (Exception ex)
            {
                throw new Exception("快速发货失败,更新出库明细出错");
            }

            return new CommonResponse();
        }



        /// <summary>
        /// 快速发货更新出库明细
        /// </summary>
        /// <param name="updateOutboundDto"></param>
        /// <returns></returns>
        public CommonResponse QuickDeliveryInsertPickDetail(List<pickdetail> pickdetails, string pickDetailOrder)
        {
            try
            {
                const string insertPickDetailSql = "INSERT INTO pickdetail(`SysId`,`WareHouseSysId`,`OutboundSysId`,`OutboundDetailSysId`,`PickDetailOrder`,`PickDate`,`Status`,`SkuSysId`,`UOMSysId`,`PackSysId`,`Loc`,`Lot`,`Lpn`,`Qty`,`CreateBy`,`CreateDate`,`UpdateBy`,`UpdateDate`,`UpdateUserName`,`CreateUserName`)VALUES";
                const string insertPickDetailValue =
                 "(uuid(),'{0}','{1}' ,'{2}','{3}',now(),{4},'{5}','{6}','{7}','{8}','{9}','{10}',{11},{12},now(),{13},now(),'{14}','{15}'),";
                var strSql = new StringBuilder();
                foreach (var info in pickdetails)
                {
                    strSql.AppendFormat(insertPickDetailValue, info.WareHouseSysId, info.OutboundSysId, info.OutboundDetailSysId, pickDetailOrder, info.Status, info.SkuSysId, info.UOMSysId, info.PackSysId, info.Loc, info.Lot, info.Lpn, info.Qty, info.CreateBy, info.UpdateBy, info.CreateUserName, info.UpdateUserName);
                }
                var strSqlValue = strSql.ToString();
                strSqlValue = strSqlValue.Substring(0, strSqlValue.Length - 1) + ";";
                base.Context.Database.ExecuteSqlCommand(insertPickDetailSql + strSqlValue);
            }
            catch (Exception ex)
            {
                throw new Exception("快速发货失败,写入拣货明细出错:" + ex.Message);
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 快速发货交易
        /// </summary>
        /// <param name="updateOutboundDto"></param>
        /// <returns></returns>
        public CommonResponse QuickDeliveryInsertInvTrans(List<invtran> invtrans)
        {
            try
            {
                const string insertInvTransSql = "INSERT INTO invtrans ( SysId, WareHouseSysId, DocOrder, DocSysId, DocDetailSysId, SkuSysId, SkuCode, TransType, SourceTransType, Qty, Loc, Lot, Lpn, ToLoc, ToLot, ToLpn, Status, LotAttr01, LotAttr02, LotAttr04, LotAttr03, LotAttr05, LotAttr06, LotAttr07, LotAttr08, LotAttr09, ExternalLot, ProduceDate, ExpiryDate, ReceivedDate, PackSysId, PackCode, UOMSysId, UOMCode, CreateBy, CreateDate, UpdateBy, UpdateDate, CreateUserName, UpdateUserName)VALUES";
                const string insertInvTransValue = "(uuid(),'{0}','{1}' ,'{2}','{3}','{4}','{5}','{6}','{7}',{8},'{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}',null,null,'{26}','{27}','{28}','{29}','{30}',{31},now(),{32},now(),'{33}','{34}'),";

                var strSql = new StringBuilder();
                foreach (var info in invtrans)
                {
                    strSql.AppendFormat(insertInvTransValue, info.WareHouseSysId, info.DocOrder, info.DocSysId, info.DocDetailSysId, info.SkuSysId,
                                                             info.SkuCode, info.TransType, info.SourceTransType, info.Qty, info.Loc,
                                                             info.Lot, info.Lpn, info.ToLoc, info.ToLot, info.ToLpn,
                                                             info.Status, info.LotAttr01, info.LotAttr02, info.LotAttr04, info.LotAttr03,
                                                             info.LotAttr05, info.LotAttr06, info.LotAttr07, info.LotAttr08, info.LotAttr09,
                                                             info.ExternalLot, info.ReceivedDate.Value.ToString(PublicConst.DateTimeFormat), info.PackSysId, info.PackCode, info.UOMSysId,
                                                             info.UOMCode, info.CreateBy, info.UpdateBy, info.CreateUserName, info.UpdateUserName);
                }
                var strSqlValue = strSql.ToString();
                strSqlValue = strSqlValue.Substring(0, strSqlValue.Length - 1) + ";";
                base.Context.Database.ExecuteSqlCommand(insertInvTransSql + strSqlValue);
            }
            catch (Exception ex)
            {
                throw new Exception("快速发货失败,写入交易出错:" + ex.Message);
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 三方写入出库明细接口
        /// </summary>
        /// <returns></returns>
        public CommonResponse ThirdPartyInsertOutboundDetail(List<outbounddetail> outbounddetails)
        {
            try
            {
                const string insertOutboundDetailSql = " INSERT INTO outbounddetail(SysId, OutboundSysId, SkuSysId, Status, CreateBy, CreateDate, UpdateBy, UpdateDate, UOMSysId, PackSysId, Loc, Lot , Lpn, LotAttr01, LotAttr02, LotAttr04, LotAttr03, LotAttr05, LotAttr06, LotAttr07, LotAttr08, LotAttr09, ExternalLot, ProduceDate, ExpiryDate, Qty, ShippedQty, PickedQty, AllocatedQty, Price, CreateUserName, UpdateUserName, PackFactor,IsGift,GiftQty) VALUES ";

                var strSql = new StringBuilder();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                var i = 1;

                foreach (var info in outbounddetails)
                {
                    strSql.Append($"(uuid(),@OutboundSysId{i},@SkuSysId{i},@Status{i},@CreateBy{i},now(),@UpdateBy{i},now(),@UOMSysId{i},@PackSysId{i},null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,@Qty{i},null,null,null,@Price{i},null,null,@PackFactor{i},@IsGift{i},@GiftQty{i}),");

                    parameters.Add(new MySqlParameter($"@OutboundSysId{i}", info.OutboundSysId));
                    parameters.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                    parameters.Add(new MySqlParameter($"@Status{i}", info.Status));
                    parameters.Add(new MySqlParameter($"@CreateBy{i}", info.CreateBy));
                    parameters.Add(new MySqlParameter($"@UpdateBy{i}", info.UpdateBy));
                    parameters.Add(new MySqlParameter($"@UOMSysId{i}", info.UOMSysId));
                    parameters.Add(new MySqlParameter($"@PackSysId{i}", info.PackSysId));
                    parameters.Add(new MySqlParameter($"@Qty{i}", info.Qty));
                    parameters.Add(new MySqlParameter($"@Price{i}", info.Price));
                    parameters.Add(new MySqlParameter($"@PackFactor{i}", info.PackFactor));
                    parameters.Add(new MySqlParameter($"@IsGift{i}", info.IsGift));
                    parameters.Add(new MySqlParameter($"@GiftQty{i}", info.GiftQty));
                    i++;
                }
                var strSqlValue = strSql.ToString();
                strSqlValue = strSqlValue.Substring(0, strSqlValue.Length - 1) + ";";
                base.Context.Database.ExecuteSqlCommand(insertOutboundDetailSql + strSqlValue, parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception("接口写入订单明细失败");
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 更新拣货库存数量(撤销领料)
        /// </summary>
        /// <param name="updateInventoryDto"></param>
        public CommonResponse UpdateInventoryCancelPickedQty(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {
                var strSqlInvLocLotLpn = new StringBuilder();


                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET invlotloclpn.PickedQty = invlotloclpn.PickedQty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}'  and (invlotloclpn.PickedQty - {1}) >= 0 ;", info.InvLotLocLpnSysId, info.Qty);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存拣货数量异常,无法撤销领料");

                }
                var strSqlInvLot = new StringBuilder();

                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET invlot.PickedQty = invlot.PickedQty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}'   and (invlot.PickedQty - {1}) >= 0;", info.InvLotSysId, info.Qty);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("库存拣货数量异常,无法撤销领料");
                }


                var strSqlSkuLoc = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET invSkuLoc.PickedQty = invSkuLoc.PickedQty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}' and (invSkuLoc.PickedQty - {1}) >= 0 ;", info.InvSkuLocSysId, info.Qty);

                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("库存拣货数量异常,无法撤销领料");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }

        #region 预包装明细更新
        /// <summary>
        /// 预包装明细更新
        /// </summary>
        /// <param name="updatePrePackDetailDtoList"></param>
        /// <returns></returns>
        public CommonResponse UpdatePrePackDetail(List<UpdatePrePackDetailDto> updatePrePackDetailDtoList)
        {
            try
            {
                var updateId = updatePrePackDetailDtoList != null ? updatePrePackDetailDtoList.FirstOrDefault().CurrentUserId : 0;
                var updateName = updatePrePackDetailDtoList != null ? updatePrePackDetailDtoList.FirstOrDefault().CurrentDisplayName : "";

                const string updateSql = " update PrePackDetail set Qty = ifnull(Qty,0) + {0},UpdateBy=@UpdateBy,UpdateUserName=@UpdateUserName,UpdateDate=now() WHERE SysId='{1}' and ifnull(PreQty,0) - ifnull(Qty,0) >= {0}; ";
                var strSql = new StringBuilder();
                foreach (var info in updatePrePackDetailDtoList)
                {
                    strSql.AppendFormat(updateSql, info.Qty, info.SysId);
                }
                var result = base.Context.Database.ExecuteSqlCommand(strSql.ToString()
                    , new MySqlParameter("@UpdateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (result != updatePrePackDetailDtoList.Count)
                {
                    throw new Exception("预包装数量不能大于计划数量");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("预包装失败," + ex.Message);
            }

            return new CommonResponse();
        }
        #endregion

        #region 库存转移

        /// <summary>
        /// 库存转移,源库存减少
        /// </summary>
        /// <param name="updateInventoryDto"></param>
        public CommonResponse UpdateInventoryQtyByFromStockTransfer(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {
                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET invlotloclpn.Qty = invlotloclpn.Qty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}'  and (invlotloclpn.Qty - invlotloclpn.AllocatedQty - invlotloclpn.PickedQty) >= {1} ;", info.InvLotLocLpnSysId, info.Qty);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("invlotloclpn库存不足,无法进行转移");

                }

                var strSqlInvLot = new StringBuilder();

                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET invlot.Qty = invlot.Qty - {0}  ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}' and (invlot.Qty - invlot.AllocatedQty - invlot.PickedQty) >= {1} ;", info.InvLotSysId, info.Qty);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("invlot库存不足,无法进行转移");
                }

                var strSqlSkuLoc = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET invSkuLoc.Qty = invSkuLoc.Qty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}' and (invSkuLoc.Qty - invSkuLoc.AllocatedQty - invSkuLoc.PickedQty - invSkuLoc.FrozenQty) >= {1}  ;", info.InvSkuLocSysId, info.Qty);

                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("invskuloc库存不足,无法进行转移");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 库存转移,目标库存增加
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryQtyByToStockTransfer(List<UpdateInventoryDto> updateInventoryList)
        {
            if (updateInventoryList != null && updateInventoryList.Count > 0)
            {
                try
                {
                    var updateId = updateInventoryList != null ? updateInventoryList.FirstOrDefault().CurrentUserId : 0;
                    var updateName = updateInventoryList != null ? updateInventoryList.FirstOrDefault().CurrentDisplayName : "";

                    StringBuilder strSql = new StringBuilder();

                    #region invLot
                    var invLotList = updateInventoryList.FindAll(x => x.InvLotSysId != new Guid() && x.InvLotSysId != null);
                    if (invLotList != null && invLotList.Count > 0)
                    {
                        foreach (var item in invLotList)
                        {
                            strSql.AppendFormat(" update invLot set Qty = Qty + {0}, UpdateBy = @UpdateBy, UpdateDate = now(), UpdateUserName = @UpdateUserName where sysid = '{1}'; ", item.Qty, item.InvLotSysId);
                        }
                    }
                    #endregion

                    #region invLotLocLpn
                    var invLotLocLpnList = updateInventoryList.FindAll(x => x.InvLotLocLpnSysId != new Guid() && x.InvLotLocLpnSysId != null);
                    if (invLotLocLpnList != null && invLotLocLpnList.Count > 0)
                    {
                        foreach (var item in invLotLocLpnList)
                        {
                            strSql.AppendFormat(" update invLotLocLpn set Qty = Qty + {0}, UpdateBy = @UpdateBy, UpdateDate = now(), UpdateUserName = @UpdateUserName where sysid = '{1}'; ", item.Qty, item.InvLotLocLpnSysId);
                        }
                    }
                    #endregion

                    #region invskuloc
                    var invskulocList = updateInventoryList.FindAll(x => x.InvSkuLocSysId != new Guid() && x.InvSkuLocSysId != null);
                    if (invskulocList != null && invskulocList.Count > 0)
                    {
                        foreach (var item in invskulocList)
                        {
                            strSql.AppendFormat(" update invskuloc set Qty = Qty + {0}, UpdateBy = @UpdateBy, UpdateDate = now(), UpdateUserName = @UpdateUserName where sysid = '{1}'; ", item.Qty, item.InvSkuLocSysId);
                        }
                    }
                    #endregion

                    var row = invLotList.Count() + invLotLocLpnList.Count() + invskulocList.Count();
                    if (!string.IsNullOrEmpty(strSql.ToString()))
                    {
                        var result = base.Context.Database.ExecuteSqlCommand(strSql.ToString()
                            , new MySqlParameter("@updateBy", updateId)
                            , new MySqlParameter("@UpdateUserName", updateName));
                        if (row != result)
                        {
                            throw new Exception("库存数量异常,无法转移");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return new CommonResponse();
        }

        /// <summary>
        /// 库存转移,目标库存增加 且 不存在货位记录
        /// </summary>
        /// <param name="invlotloclpn"></param>
        /// <param name="invlot"></param>
        /// <param name="invskuloc"></param>
        public void AddInventoryQtyByToStockTransfer(invlotloclpn invlotloclpn, invlot invlot, invskuloc invskuloc)
        {
            if (invlotloclpn.SysId != Guid.Empty)
            {
                List<MySqlParameter> paraList = new List<MySqlParameter>();
                StringBuilder strSqlInvLocLotLpn = new StringBuilder();
                strSqlInvLocLotLpn.Append(@"INSERT INTO invlotloclpn(SysId, WareHouseSysId, SkuSysId, Loc, Lot, 
                            Lpn, Qty, AllocatedQty, PickedQty, Status, 
                            CreateBy, CreateDate, UpdateBy, UpdateDate, CreateUserName, UpdateUserName)");
                strSqlInvLocLotLpn.Append($@" SELECT @SysId, @WareHouseSysId, @SkuSysId, @Loc, @Lot, 
                            @Lpn, @Qty, @AllocatedQty, @PickedQty, @Status, 
                            @CreateBy, NOW(), @UpdateBy, NOW(), @CreateUserName, @UpdateUserName");

                strSqlInvLocLotLpn.Append($@" FROM dual WHERE NOT EXISTS(SELECT 1 FROM invlotloclpn i WHERE i.SkuSysId = @SkuSysId AND i.WareHouseSysId = @WareHouseSysId AND i.Loc = @Loc AND i.Lot = @Lot AND i.Lpn = @Lpn); ");

                paraList.Add(new MySqlParameter("@SysId", invlotloclpn.SysId));
                paraList.Add(new MySqlParameter("@WareHouseSysId", invlotloclpn.WareHouseSysId));
                paraList.Add(new MySqlParameter("@SkuSysId", invlotloclpn.SkuSysId));
                paraList.Add(new MySqlParameter("@Loc", invlotloclpn.Loc));
                paraList.Add(new MySqlParameter("@Lot", invlotloclpn.Lot));
                paraList.Add(new MySqlParameter("@Lpn", invlotloclpn.Lpn));
                paraList.Add(new MySqlParameter("@Qty", invlotloclpn.Qty));
                paraList.Add(new MySqlParameter("@AllocatedQty", invlotloclpn.AllocatedQty));
                paraList.Add(new MySqlParameter("@PickedQty", invlotloclpn.PickedQty));
                paraList.Add(new MySqlParameter("@Status", invlotloclpn.Status));
                paraList.Add(new MySqlParameter("@CreateBy", invlotloclpn.CreateBy));
                paraList.Add(new MySqlParameter("@UpdateBy", invlotloclpn.UpdateBy));
                paraList.Add(new MySqlParameter("@CreateUserName", invlotloclpn.CreateUserName));
                paraList.Add(new MySqlParameter("@UpdateUserName", invlotloclpn.UpdateUserName));

                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString(), paraList.ToArray());
                if (invLotlocLpnResult == 0)
                {
                    string sqlInvLocLotLpn = $@"update invLotLocLpn i set Qty = Qty + {invlotloclpn.Qty}, UpdateBy = @UpdateBy, UpdateDate = now(), UpdateUserName = @UpdateUserName
                            where i.SkuSysId = @SkuSysId AND i.WareHouseSysId = @WareHouseSysId AND i.Loc = @Loc AND i.Lot = @Lot AND i.Lpn = @Lpn;";

                    base.Context.Database.ExecuteSqlCommand(sqlInvLocLotLpn, paraList.ToArray());
                }
            }

            if (invlot.SysId != Guid.Empty)
            {
                List<MySqlParameter> paraList = new List<MySqlParameter>();

                StringBuilder strSqlInvlot = new StringBuilder();
                strSqlInvlot.Append(@"INSERT INTO invlot(SysId, WareHouseSysId, Lot, SkuSysId, CaseQty, InnerPackQty, Qty, AllocatedQty, PickedQty, 
                            HoldQty, Status, Price, CreateBy, CreateDate, UpdateBy, UpdateDate, LotAttr01, LotAttr02, 
                            LotAttr04, LotAttr03, LotAttr05, LotAttr06, LotAttr07, LotAttr08, 
                            LotAttr09, ProduceDate, ExpiryDate, ExternalLot, ReceiptDate, CreateUserName, UpdateUserName)");

                string formatStringSql = $@" SELECT @SysId, @WareHouseSysId, @Lot, @SkuSysId, @CaseQty,@InnerPackQty ,@Qty ,@AllocatedQty, @PickedQty, 
                            @HoldQty, @Status, @Price, @CreateBy, NOW(), @UpdateBy, NOW(), @LotAttr01, @LotAttr02, 
                            @LotAttr04, @LotAttr03, @LotAttr05, @LotAttr06, @LotAttr07, @LotAttr08, 
                            @LotAttr09, ReplaceProduceDate,ReplaceExpiryDate, @ExternalLot, ReplaceReceiptDate,@CreateUserName,@CreateUserName";

                formatStringSql = formatStringSql.Replace("ReplaceProduceDate", invlot.ProduceDate.HasValue ? $"'{invlot.ProduceDate.Value.ToString("yyyy-MM-dd HH:mm:ss")}'" : "null");
                formatStringSql = formatStringSql.Replace("ReplaceExpiryDate", invlot.ExpiryDate.HasValue ? $"'{invlot.ExpiryDate.Value.ToString("yyyy-MM-dd HH:mm:ss")}'" : "null");
                formatStringSql = formatStringSql.Replace("ReplaceReceiptDate", invlot.ReceiptDate.HasValue ? $"'{invlot.ReceiptDate.Value.ToString("yyyy-MM-dd HH:mm:ss")}'" : "null");

                strSqlInvlot.Append(formatStringSql);


                strSqlInvlot.Append($@" FROM dual WHERE NOT EXISTS(SELECT 1 FROM invlot i WHERE i.SkuSysId = '{invlot.SkuSysId}' AND i.WareHouseSysId = '{invlot.WareHouseSysId}' AND i.Lot = '{invlot.Lot}'); ");

                paraList.Add(new MySqlParameter("@SysId", invlot.SysId));
                paraList.Add(new MySqlParameter("@WareHouseSysId", invlot.WareHouseSysId));
                paraList.Add(new MySqlParameter("@Lot", invlot.Lot));
                paraList.Add(new MySqlParameter("@SkuSysId", invlot.SkuSysId));
                paraList.Add(new MySqlParameter("@CaseQty", invlot.CaseQty));
                paraList.Add(new MySqlParameter("@InnerPackQty", invlot.InnerPackQty));
                paraList.Add(new MySqlParameter("@Qty", invlot.Qty));
                paraList.Add(new MySqlParameter("@AllocatedQty", invlot.AllocatedQty));
                paraList.Add(new MySqlParameter("@PickedQty", invlot.PickedQty));
                paraList.Add(new MySqlParameter("@HoldQty", invlot.HoldQty));
                paraList.Add(new MySqlParameter("@Status", invlot.Status));
                paraList.Add(new MySqlParameter("@Price", invlot.Price));
                paraList.Add(new MySqlParameter("@CreateBy", invlot.CreateBy));
                paraList.Add(new MySqlParameter("@UpdateBy", invlot.UpdateBy));
                paraList.Add(new MySqlParameter("@CreateUserName", invlot.CreateUserName));

                paraList.Add(new MySqlParameter("@LotAttr01", invlot.LotAttr01));
                paraList.Add(new MySqlParameter("@LotAttr02", invlot.LotAttr02));
                paraList.Add(new MySqlParameter("@LotAttr03", invlot.LotAttr03));
                paraList.Add(new MySqlParameter("@LotAttr04", invlot.LotAttr04));
                paraList.Add(new MySqlParameter("@LotAttr05", invlot.LotAttr05));
                paraList.Add(new MySqlParameter("@LotAttr06", invlot.LotAttr06));
                paraList.Add(new MySqlParameter("@LotAttr07", invlot.LotAttr07));
                paraList.Add(new MySqlParameter("@LotAttr08", invlot.LotAttr08));
                paraList.Add(new MySqlParameter("@LotAttr09", invlot.LotAttr09));
                paraList.Add(new MySqlParameter("@ExternalLot", invlot.ExternalLot));

                var invlotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvlot.ToString(), paraList.ToArray());
                if (invlotResult == 0)
                {
                    string sqlInvlot = $@"update invlot i set Qty = Qty + @Qty, UpdateBy = @UpdateBy, UpdateDate = now(), UpdateUserName = @CreateUserName
                            where i.SkuSysId = @SkuSysId AND i.WareHouseSysId = @WareHouseSysId AND i.Lot = @Lot ;";

                    base.Context.Database.ExecuteSqlCommand(sqlInvlot, paraList.ToArray());
                }
            }

            if (invskuloc.SysId != Guid.Empty)
            {
                List<MySqlParameter> paraList = new List<MySqlParameter>();

                StringBuilder strSqlInvSkuLoc = new StringBuilder();
                strSqlInvSkuLoc.Append(@"INSERT INTO invskuloc(SysId, WareHouseSysId, SkuSysId, Loc, Qty, 
                            AllocatedQty, PickedQty, CreateBy, CreateDate, UpdateBy, UpdateDate, CreateUserName, UpdateUserName)");

                strSqlInvSkuLoc.Append($@" SELECT @SysId, @WareHouseSysId, @SkuSysId, @Loc, @Qty, 
                            @AllocatedQty, @PickedQty, @CreateBy, NOW(), @UpdateBy, NOW(), @CreateUserName, @UpdateUserName ");

                strSqlInvSkuLoc.Append($@" FROM dual WHERE NOT EXISTS(SELECT 1 FROM invskuloc i WHERE i.SkuSysId = @SkuSysId AND i.WareHouseSysId = @WareHouseSysId AND i.Loc = @Loc ); ");

                paraList.Add(new MySqlParameter("@SysId", invskuloc.SysId));
                paraList.Add(new MySqlParameter("@WareHouseSysId", invskuloc.WareHouseSysId));
                paraList.Add(new MySqlParameter("@SkuSysId", invskuloc.SkuSysId));
                paraList.Add(new MySqlParameter("@Loc", invskuloc.Loc));
                paraList.Add(new MySqlParameter("@Qty", invskuloc.Qty));
                paraList.Add(new MySqlParameter("@AllocatedQty", invskuloc.AllocatedQty));
                paraList.Add(new MySqlParameter("@PickedQty", invskuloc.PickedQty));
                paraList.Add(new MySqlParameter("@CreateBy", invskuloc.CreateBy));
                paraList.Add(new MySqlParameter("@UpdateBy", invskuloc.UpdateBy));
                paraList.Add(new MySqlParameter("@CreateUserName", invskuloc.CreateUserName));
                paraList.Add(new MySqlParameter("@UpdateUserName", invskuloc.UpdateUserName));

                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlInvSkuLoc.ToString(), paraList.ToArray());
                if (invSkuLocResult == 0)
                {
                    string sqlSkuInvLoc = $@"update invskuloc i set Qty = Qty + @Qty, UpdateBy = @UpdateBy, UpdateDate = now(), UpdateUserName = @UpdateUserName 
                            where i.SkuSysId = @SkuSysId AND i.WareHouseSysId = @WareHouseSysId AND i.Loc = @Loc ;";

                    base.Context.Database.ExecuteSqlCommand(sqlSkuInvLoc, paraList.ToArray());
                }

            }

        }

        /// <summary>
        /// 库存减少(渠道变更)
        /// </summary>
        /// <param name="updateInventoryDto"></param>
        public CommonResponse UpdateInventoryQtyByStockTransfer(List<UpdateInventoryDto> updateInventoryList)
        {
            if (updateInventoryList != null && updateInventoryList.Count > 0)
            {
                var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
                var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
                try
                {
                    var strSqlInvLocLotLpn = new StringBuilder();
                    foreach (var info in updateInventoryList)
                    {
                        strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET invlotloclpn.Qty = invlotloclpn.Qty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                        strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}'  and  (invlotloclpn.Qty - invlotloclpn.AllocatedQty - invlotloclpn.PickedQty - invlotloclpn.FrozenQty) >= {1} ;", info.InvLotLocLpnSysId, info.Qty);

                    }
                    var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                        , new MySqlParameter("@updateBy", updateId)
                        , new MySqlParameter("@UpdateUserName", updateName));
                    if (updateInventoryList.Count() != invLotlocLpnResult)
                    {
                        throw new Exception("invlotloclpn库存不足,无法进行转移");

                    }

                    var strSqlinvLot = new StringBuilder();
                    foreach (var info in updateInventoryList)
                    {
                        strSqlinvLot.AppendFormat(" UPDATE invlot SET invlot.Qty = invlot.Qty - {0}  ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                        strSqlinvLot.AppendFormat(" where invlot.sysId='{0}' and (invlot.Qty - invlot.AllocatedQty - invlot.PickedQty - invlot.FrozenQty)  >= {1}  ;", info.InvLotSysId, info.Qty);

                    }
                    var invInvlotResult = base.Context.Database.ExecuteSqlCommand(strSqlinvLot.ToString()
                        , new MySqlParameter("@updateBy", updateId)
                        , new MySqlParameter("@UpdateUserName", updateName));
                    if (updateInventoryList.Count() != invInvlotResult)
                    {
                        throw new Exception("invskuloc库存不足,无法进行转移");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 更新库存(库存转移)
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryAddQtyByStockTransfer(List<UpdateInventoryDto> updateInventoryList)
        {
            if (updateInventoryList != null && updateInventoryList.Count > 0)
            {
                var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
                var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;

                try
                {
                    StringBuilder strSql = new StringBuilder();

                    #region invLot
                    var invLotList = updateInventoryList.FindAll(x => x.InvLotSysId != new Guid() && x.InvLotSysId != null);
                    if (invLotList != null && invLotList.Count > 0)
                    {
                        foreach (var item in invLotList)
                        {
                            strSql.AppendFormat(" update invLot set Qty = Qty + {0}, UpdateBy = @UpdateBy, UpdateDate = now(), UpdateUserName = @UpdateUserName where sysid = '{1}'; ", item.Qty, item.InvLotSysId);
                        }
                    }
                    #endregion

                    #region invLotLocLpn
                    var invLotLocLpnList = updateInventoryList.FindAll(x => x.InvLotLocLpnSysId != new Guid() && x.InvLotLocLpnSysId != null);
                    if (invLotLocLpnList != null && invLotLocLpnList.Count > 0)
                    {
                        foreach (var item in invLotLocLpnList)
                        {
                            strSql.AppendFormat(" update invLotLocLpn set Qty = Qty + {0}, UpdateBy = @UpdateBy, UpdateDate = now(), UpdateUserName = @UpdateUserName where sysid = '{1}'; ", item.Qty, item.InvLotLocLpnSysId);
                        }
                    }
                    #endregion

                    var row = invLotList.Count() + invLotLocLpnList.Count();
                    if (!string.IsNullOrEmpty(strSql.ToString()))
                    {
                        var result = base.Context.Database.ExecuteSqlCommand(strSql.ToString()
                            , new MySqlParameter("@UpdateBy", updateId)
                            , new MySqlParameter("@UpdateUserName", updateName));
                        if (row != result)
                        {
                            throw new Exception("库存数量异常,无法转移");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return new CommonResponse();
        }
        #endregion

        #region 分配发货
        /// <summary>
        /// 更新分配库存到财务库存发生扣减
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryAllocationDelivery(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {

                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET  invlotloclpn.Qty = invlotloclpn.Qty - {0}, invlotloclpn.AllocatedQty = invlotloclpn.AllocatedQty - {0},updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' and invlotloclpn.Qty  > 0 AND (invlotloclpn.Qty - invlotloclpn.PickedQty - invlotloclpn.AllocatedQty - invlotloclpn.FrozenQty + {1}) >= {1};", info.InvLotLocLpnSysId, info.Qty);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存不足,无法进行分配发货");

                }

                var strSqlInvLot = new StringBuilder();

                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET invlot.Qty = invlot.Qty - {0}, invlot.AllocatedQty = invlot.AllocatedQty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}' and (invlot.Qty - invlot.PickedQty - invlot.AllocatedQty - invlot.FrozenQty + {1} ) >= {1} ;", info.InvLotSysId, info.Qty);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("库存不足,无法进行分配发货");
                }

                var strSqlSkuLoc = new StringBuilder();

                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET invSkuLoc.Qty = invSkuLoc.Qty - {0}, invSkuLoc.AllocatedQty = invSkuLoc.AllocatedQty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}' and invSkuLoc.Qty > 0 AND (invSkuLoc.Qty - invSkuLoc.PickedQty - invSkuLoc.AllocatedQty - invSkuLoc.FrozenQty + {1}) >= {1}   ;", info.InvSkuLocSysId, info.Qty);
                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("invskuloc库存不足,无法进行转移");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 更新出库明细(分配发货)
        /// </summary>
        /// <param name="updateOutboundDto"></param>
        /// <returns></returns>
        public CommonResponse UpdateOutboundDetailAllocationDelivery(List<UpdateOutboundDto> updateOutboundDto)
        {
            try
            {
                if (updateOutboundDto != null && updateOutboundDto.Count > 0)
                {
                    var updateId = updateOutboundDto.FirstOrDefault().CurrentUserId;
                    var updateName = updateOutboundDto.FirstOrDefault().CurrentDisplayName;

                    const string updateSql = " update OutboundDetail set Status={2}, ShippedQty={0},PickedQty={0},UpdateDate=now(),UpdateBy=@UpdateBy,UpdateUserName=@UpdateUserName WHERE SysId='{1}'; ";
                    var strSql = new StringBuilder();
                    foreach (var info in updateOutboundDto)
                    {
                        strSql.AppendFormat(updateSql, info.Qty, info.SysId, info.Status);
                    }
                    base.Context.Database.ExecuteSqlCommand(strSql.ToString()
                        , new MySqlParameter("@UpdateBy", updateId)
                        , new MySqlParameter("@UpdateUserName", updateName));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("分配发货失败,更新出库明细出错");
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 分配发货交易
        /// </summary>
        /// <param name="updateOutboundDto"></param>
        /// <returns></returns>
        public CommonResponse AllocationDeliveryInsertInvTrans(List<invtran> invtrans)
        {
            try
            {
                const string insertInvTransSql = "INSERT INTO invtrans ( SysId, WareHouseSysId, DocOrder, DocSysId, DocDetailSysId, SkuSysId, SkuCode, TransType, SourceTransType, Qty, Loc, Lot, Lpn, ToLoc, ToLot, ToLpn, Status, LotAttr01, LotAttr02, LotAttr04, LotAttr03, LotAttr05, LotAttr06, LotAttr07, LotAttr08, LotAttr09, ExternalLot, ProduceDate, ExpiryDate, ReceivedDate, PackSysId, PackCode, UOMSysId, UOMCode, CreateBy, CreateDate, UpdateBy, UpdateDate, CreateUserName, UpdateUserName)VALUES";

                var strSql = new StringBuilder();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                var i = 1;

                foreach (var info in invtrans)
                {
                    strSql.Append($@"(uuid(), @WareHouseSysId{i}, @DocOrder{i}, @DocSysId{i}, @DocDetailSysId{i}, @SkuSysId{i},
                                                             @SkuCode{i}, @TransType{i}, @SourceTransType{i}, @Qty{i}, @Loc{i},
                                                             @Lot{i}, @Lpn{i}, @ToLoc{i}, @ToLot{i}, @ToLpn{i},
                                                             @Status{i}, @LotAttr01{i}, @LotAttr02{i}, @LotAttr04{i}, @LotAttr03{i},
                                                             @LotAttr05{i}, @LotAttr06{i}, @LotAttr07{i}, @LotAttr08{i}, @LotAttr09{i},
                                                             @ExternalLot{i}, null,null,@ReceivedDate{i}, @PackSysId{i}, @PackCode{i}, @UOMSysId{i},
                                                             @UOMCode{i}, @CreateBy{i}, now(),@UpdateBy{i}, now(),@CreateUserName{i}, @UpdateUserName{i}),");

                    parameters.Add(new MySqlParameter($"@WareHouseSysId{i}", info.WareHouseSysId));
                    parameters.Add(new MySqlParameter($"@DocOrder{i}", info.DocOrder));
                    parameters.Add(new MySqlParameter($"@DocSysId{i}", info.DocSysId));
                    parameters.Add(new MySqlParameter($"@DocDetailSysId{i}", info.DocDetailSysId));
                    parameters.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                    parameters.Add(new MySqlParameter($"@SkuCode{i}", info.SkuCode));
                    parameters.Add(new MySqlParameter($"@TransType{i}", info.TransType));
                    parameters.Add(new MySqlParameter($"@SourceTransType{i}", info.SourceTransType));
                    parameters.Add(new MySqlParameter($"@Qty{i}", info.Qty));
                    parameters.Add(new MySqlParameter($"@Loc{i}", info.Loc));
                    parameters.Add(new MySqlParameter($"@Lot{i}", info.Lot));
                    parameters.Add(new MySqlParameter($"@Lpn{i}", info.Lpn));
                    parameters.Add(new MySqlParameter($"@ToLoc{i}", info.ToLoc));
                    parameters.Add(new MySqlParameter($"@ToLot{i}", info.ToLot));
                    parameters.Add(new MySqlParameter($"@ToLpn{i}", info.ToLpn));
                    parameters.Add(new MySqlParameter($"@Status{i}", info.Status));
                    parameters.Add(new MySqlParameter($"@LotAttr01{i}", info.LotAttr01));
                    parameters.Add(new MySqlParameter($"@LotAttr02{i}", info.LotAttr02));
                    parameters.Add(new MySqlParameter($"@LotAttr04{i}", info.LotAttr04));
                    parameters.Add(new MySqlParameter($"@LotAttr03{i}", info.LotAttr03));
                    parameters.Add(new MySqlParameter($"@LotAttr05{i}", info.LotAttr05));
                    parameters.Add(new MySqlParameter($"@LotAttr06{i}", info.LotAttr06));
                    parameters.Add(new MySqlParameter($"@LotAttr07{i}", info.LotAttr07));
                    parameters.Add(new MySqlParameter($"@LotAttr08{i}", info.LotAttr08));
                    parameters.Add(new MySqlParameter($"@LotAttr09{i}", info.LotAttr09));
                    parameters.Add(new MySqlParameter($"@ExternalLot{i}", info.ExternalLot));
                    parameters.Add(new MySqlParameter($"@ReceivedDate{i}", info.ReceivedDate.Value.ToString(PublicConst.DateTimeFormat)));
                    parameters.Add(new MySqlParameter($"@PackSysId{i}", info.PackSysId));
                    parameters.Add(new MySqlParameter($"@PackCode{i}", info.PackCode));
                    parameters.Add(new MySqlParameter($"@UOMSysId{i}", info.UOMSysId));
                    parameters.Add(new MySqlParameter($"@UOMCode{i}", info.UOMCode));
                    parameters.Add(new MySqlParameter($"@CreateBy{i}", info.CreateBy));
                    parameters.Add(new MySqlParameter($"@UpdateBy{i}", info.UpdateBy));
                    parameters.Add(new MySqlParameter($"@CreateUserName{i}", info.CreateUserName));
                    parameters.Add(new MySqlParameter($"@UpdateUserName{i}", info.UpdateUserName));

                    i++;
                }
                var strSqlValue = strSql.ToString();
                strSqlValue = strSqlValue.Substring(0, strSqlValue.Length - 1) + ";";
                base.Context.Database.ExecuteSqlCommand(insertInvTransSql + strSqlValue, parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception("分配发货失败,写入交易出错:" + ex.Message);
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 分配发货 修改发货明细和拣货明细
        /// </summary>
        /// <returns></returns>
        public CommonResponse UpdateOdAndPdByAllocationDelivery(UpdateOutboundDto updateOutboundDto, List<UpdateOutboundDetailDto> updateOutboundDetailDtoList)
        {
            try
            {
                int i = 1;
                var strSql = new StringBuilder();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@PickDetailStatus", (int)PickDetailStatus.Finish));
                parameters.Add(new MySqlParameter("@UpdateBy", updateOutboundDto.CurrentUserId));
                parameters.Add(new MySqlParameter("@UpdateUserName", updateOutboundDto.CurrentDisplayName));
                parameters.Add(new MySqlParameter("@SysId", updateOutboundDto.SysId));
                parameters.Add(new MySqlParameter("@PdStatus", (int)PickDetailStatus.New));
                parameters.Add(new MySqlParameter("@OutboundDetailStatus", (int)OutboundDetailStatus.Delivery));

                strSql.Append($"update pickdetail set Status = @PickDetailStatus{ (updateOutboundDto.PartialShipmentFlag ? ", Qty = PickedQty" : string.Empty) }, UpdateBy = @UpdateBy, UpdateDate = now(), UpdateUserName = @UpdateUserName where outboundsysid = @SysId and Status = @PdStatus;");
                //strSql.Append("update OutboundDetail set Status = @OutboundDetailStatus, ShippedQty = Qty,PickedQty = Qty, UpdateDate = now(), UpdateBy = @UpdateBy,UpdateUserName = @UpdateUserName WHERE outboundsysid=@SysId;");
                foreach (var item in updateOutboundDetailDtoList)
                {
                    string paraQty = $"@Qty{i}";
                    string paraSysId = $"@SysId{i}";
                    i++;
                    strSql.Append($"update OutboundDetail set Status = @OutboundDetailStatus, ShippedQty = {paraQty},PickedQty = {paraQty}, UpdateDate = now(), UpdateBy = @UpdateBy,UpdateUserName = @UpdateUserName WHERE sysid={paraSysId};");
                    parameters.Add(new MySqlParameter(paraQty, item.Qty));
                    parameters.Add(new MySqlParameter(paraSysId, item.SysId));
                }

                var result = base.Context.Database.ExecuteSqlCommand(strSql.ToString(), parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception("分配发货失败，修改出库明细或拣货明细出错:" + ex.Message);
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 部分发货，扣减剩余占用库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryByRemainingAllocatedQty(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {
                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET invlotloclpn.AllocatedQty = invlotloclpn.AllocatedQty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' and invlotloclpn.AllocatedQty - {1} >= 0;", info.InvLotLocLpnSysId, info.Qty);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));

                var strSqlInvLot = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET invlot.AllocatedQty = invlot.AllocatedQty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}' and invlot.AllocatedQty - {1} >= 0;", info.InvLotSysId, info.Qty);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));

                var strSqlSkuLoc = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET invSkuLoc.AllocatedQty = invSkuLoc.AllocatedQty - {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}' and invSkuLoc.AllocatedQty - {1} >= 0  ;", info.InvSkuLocSysId, info.Qty);
                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
            }
            catch (Exception ex)
            {
                throw new Exception("分配发货失败，还原剩余占用库存出错：" + ex.Message);
            }
            return new CommonResponse();
        }
        #endregion

        #region 取消发货

        /// <summary>
        /// 取消出库（快速发货），或者退货入库，批量增加财务库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryForCancelOutbound(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {

                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    if (info.InvLotLocLpnSysId != new Guid())
                    {
                        strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET   invlotloclpn.Qty = invlotloclpn.Qty + {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                        strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' ;", info.InvLotLocLpnSysId);

                    }

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                     , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("未找到有效库存记录");

                }

                var strSqlInvLot = new StringBuilder();

                foreach (var info in updateInventoryList)
                {
                    if (info.InvLotSysId != new Guid())
                    {
                        strSqlInvLot.AppendFormat(" UPDATE invlot SET invlot.Qty = invlot.Qty + {0}  ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                        strSqlInvLot.AppendFormat(" where invlot.sysId='{0}'; ", info.InvLotSysId);
                    }

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                     , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("未找到有效库存记录");
                }

                var strSqlSkuLoc = new StringBuilder();

                foreach (var info in updateInventoryList)
                {
                    if (info.InvSkuLocSysId != new Guid())
                    {
                        strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET invSkuLoc.Qty = invSkuLoc.Qty + {0}   ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                        strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}' ;", info.InvSkuLocSysId);
                    }
                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("未找到有效库存记录");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 取消出库（正常出库）
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryForCancelOutboundShipment(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {
                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET invlotloclpn.Qty = invlotloclpn.Qty + {0} ,invlotloclpn.PickedQty = invlotloclpn.PickedQty + {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' ; ", info.InvLotLocLpnSysId);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                     , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存异常,无法进取消出库");

                }

                var strSqlInvLot = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET  invlot.Qty = invlot.Qty + {0} ,invlot.PickedQty = invlot.PickedQty + {0},updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}'; ", info.InvLotSysId);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                     , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("库存异常,无法进取消出库");
                }

                var strSqlSkuLoc = new StringBuilder();

                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET   invSkuLoc.Qty = invSkuLoc.Qty + {0} ,invSkuLoc.PickedQty = invSkuLoc.PickedQty + {0}  ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}' ;", info.InvSkuLocSysId);
                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                     , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("库存异常,无法进取消出库");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 取消出库（分配出库）
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryForCancelOutboundAllocationDelivery(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {

                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET  invlotloclpn.Qty = invlotloclpn.Qty + {0},invlotloclpn.AllocatedQty = invlotloclpn.AllocatedQty + {0},updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' ; ", info.InvLotLocLpnSysId);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存异常,无法进取消出库");

                }

                var strSqlInvLot = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET invlot.Qty = invlot.Qty + {0} ,invlot.AllocatedQty = invlot.AllocatedQty + {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}'; ", info.InvLotSysId);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("库存异常,无法进取消出库");
                }

                var strSqlSkuLoc = new StringBuilder();

                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET   invSkuLoc.Qty = invSkuLoc.Qty + {0} ,invSkuLoc.AllocatedQty = invSkuLoc.AllocatedQty + {0}   ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}' ;", info.InvSkuLocSysId);
                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("库存异常,无法进取消出库");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }

        /// <summary>
        /// 取消出库 （更新出库明细信息，快速出库）
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="updateStatus"></param>
        /// <param name="userid"></param>
        /// <param name="userName"></param>
        public void UpdateOutboundDetailForOutboundCancel_QuickDelivery(Guid outboundSysId, OutboundDetailStatus updateStatus, int userid, string userName)
        {
            string sqlString = @"
                UPDATE outbounddetail 
                SET Status = @updateStatus,
                    AllocatedQty = 0,
                    PickedQty = 0,
                    ShippedQty = 0,
                    UpdateBy = @userid,
                    UpdateDate = NOW(),
                    UpdateUserName = @userName
                WHERE OutboundSysId = @outboundSysId ;
            ";

            base.Context.Database.ExecuteSqlCommand(sqlString,
                new MySqlParameter("@outboundSysId", outboundSysId),
                new MySqlParameter("@updateStatus", (int)updateStatus),
                new MySqlParameter("@userid", userid),
                new MySqlParameter("@userName", userName));
        }

        /// <summary>
        /// 取消出库 （更新出库明细信息,分配发货）
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="updateStatus"></param>
        /// <param name="userid"></param>
        /// <param name="userName"></param>
        /// <param name="allocatedQty"></param>
        public void UpdateOutboundDetailForOutboundCancel_AllocationDelivery(Guid outboundSysId, OutboundDetailStatus updateStatus, int userid, string userName)
        {
            string sqlString = @"
                UPDATE outbounddetail 
                SET Status = @updateStatus,
                    AllocatedQty = Qty,
                    ShippedQty = 0,
                    UpdateBy = @userid,
                    UpdateDate = NOW(),
                    UpdateUserName = @userName
                WHERE OutboundSysId = @outboundSysId ;
            ";

            base.Context.Database.ExecuteSqlCommand(sqlString,
                new MySqlParameter("@outboundSysId", outboundSysId),
                new MySqlParameter("@updateStatus", (int)updateStatus),
                new MySqlParameter("@userid", userid),
                new MySqlParameter("@userName", userName));
        }

        /// <summary>
        /// 取消出库 （更新出库明细信息,正常发货）
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="updateStatus"></param>
        /// <param name="userid"></param>
        /// <param name="userName"></param>
        /// <param name="pickedQty"></param>
        public void UpdateOutboundDetailForOutboundCancel_Shipment(Guid outboundSysId, OutboundDetailStatus updateStatus, int userid, string userName)
        {
            string sqlString = @"
                UPDATE outbounddetail 
                SET Status = @updateStatus,
                    PickedQty = Qty,
                    ShippedQty = 0,
                    UpdateBy = @userid,
                    UpdateDate = NOW(),
                    UpdateUserName = @userName
                WHERE OutboundSysId = @outboundSysId ;
            ";

            base.Context.Database.ExecuteSqlCommand(sqlString,
                new MySqlParameter("@outboundSysId", outboundSysId),
                new MySqlParameter("@updateStatus", (int)updateStatus),
                new MySqlParameter("@userid", userid),
                new MySqlParameter("@userName", userName));
        }

        /// <summary>
        /// 取消发货，更新pickdetail(快速发货)
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="updateStatus"></param>
        /// <param name="userid"></param>
        /// <param name="userName"></param>
        public void UpdatePickdetailForOutboundCancel_QuickDelivery(Guid outboundSysId, PickDetailStatus updateStatus, int userid, string userName)
        {
            string sqlString = @"
                UPDATE pickdetail 
                SET Status = @updateStatus,
                    UpdateBy = @userid,
                    UpdateDate = NOW(),
                    UpdateUserName = @userName
                WHERE OutboundSysId = @outboundSysId ;
            ";

            base.Context.Database.ExecuteSqlCommand(sqlString,
                new MySqlParameter("@outboundSysId", outboundSysId),
                new MySqlParameter("@updateStatus", (int)updateStatus),
                new MySqlParameter("@userid", userid),
                new MySqlParameter("@userName", userName));
        }

        /// <summary>
        /// 取消发货，更新pickdetail(快速发货)
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="updateStatus"></param>
        /// <param name="userid"></param>
        /// <param name="userName"></param>
        public void UpdatePickdetailForOutboundCancel_AllocationDelivery(Guid outboundSysId, PickDetailStatus updateStatus, int userid, string userName)
        {
            string sqlString = @"
                UPDATE pickdetail 
                SET Status = @updateStatus,
                    UpdateBy = @userid,
                    UpdateDate = NOW(),
                    UpdateUserName = @userName
                WHERE OutboundSysId = @outboundSysId 
                    AND Status = 50;
            ";

            base.Context.Database.ExecuteSqlCommand(sqlString,
                new MySqlParameter("@outboundSysId", outboundSysId),
                new MySqlParameter("@updateStatus", (int)updateStatus),
                new MySqlParameter("@userid", userid),
                new MySqlParameter("@userName", userName));
        }


        public void UpdateInvtranForOutboundCancel_QuickDelivery(Guid docSysId, string updateStatus, int userid, string userName)
        {
            string sqlString = @"
                UPDATE invtrans 
                SET Status = @updateStatus,
                    UpdateBy = @userid,
                    UpdateDate = NOW(),
                    UpdateUserName = @userName
                WHERE DocSysId = @DocSysId 
                    and Status = 'OK';
            ";

            base.Context.Database.ExecuteSqlCommand(sqlString,
                new MySqlParameter("@DocSysId", docSysId),
                new MySqlParameter("@updateStatus", updateStatus),
                new MySqlParameter("@userid", userid),
                new MySqlParameter("@userName", userName));
        }

        public void UpdateInvtranForOutboundCancel_AllocationDelivery(List<Guid> docSysIds, string updateStatus, int userid, string userName)
        {
            StringBuilder sbSysIds = new StringBuilder();

            docSysIds.ForEach(p =>
            {
                sbSysIds.AppendFormat("'{0}',", p.ToString());
            });

            string sysIds = sbSysIds.ToString().Trim(',');

            string sqlString = $@"
                UPDATE invtrans 
                SET Status = @updateStatus,
                    UpdateBy = @userid,
                    UpdateDate = NOW(),
                    UpdateUserName = @userName
                WHERE DocSysId IN ({sysIds}) 
                    and Status = 'OK';
            ";

            base.Context.Database.ExecuteSqlCommand(sqlString,
                new MySqlParameter("@updateStatus", updateStatus),
                new MySqlParameter("@userid", userid),
                new MySqlParameter("@userName", userName));
        }

        #endregion 取消发货

        #region  取消装箱
        /// <summary>
        /// 更新拣货库存到分配库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryCancelVanning(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {

                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET  invlotloclpn.PickedQty = invlotloclpn.PickedQty - {0},  invlotloclpn.AllocatedQty = invlotloclpn.AllocatedQty + {0},updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' and invlotloclpn.Qty  > 0 AND invlotloclpn.PickedQty >= {1}  ; ", info.InvLotLocLpnSysId, info.Qty);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存异常,无法取消装箱");

                }

                var strSqlInvLot = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET  invlot.PickedQty = invlot.PickedQty - {0}, invlot.AllocatedQty = invlot.AllocatedQty + {0},updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}'  and  invlot.Qty > 0 AND invlot.PickedQty >= {1} ; ", info.InvLotSysId, info.Qty);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("库存异常,无法取消装箱");
                }

                var strSqlSkuLoc = new StringBuilder();


                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET   invSkuLoc.PickedQty = invSkuLoc.PickedQty - {0}, invSkuLoc.AllocatedQty = invSkuLoc.AllocatedQty + {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}'  and  invSkuLoc.Qty > 0 AND invSkuLoc.PickedQty >= {1};", info.InvSkuLocSysId, info.Qty);
                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("库存异常,无法取消装箱");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }
        #endregion

        /// <summary>
        /// 取消上架(扣减财务库存)
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryCancelShelves(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            try
            {
                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    if (info.InvLotLocLpnSysId != new Guid())
                    {
                        strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET  invlotloclpn.Qty = invlotloclpn.Qty - {0}  ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                        strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' and invlotloclpn.Qty > 0 AND (invlotloclpn.Qty - invlotloclpn.PickedQty - invlotloclpn.AllocatedQty - invlotloclpn.FrozenQty) >= {1} ; ", info.InvLotLocLpnSysId, info.Qty);
                    }
                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Where(p => p.InvLotLocLpnSysId != new Guid()).Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存不足,无法取消上架");
                }

                var strSqlInvLot = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    if (info.InvLotSysId != new Guid())
                    {
                        strSqlInvLot.AppendFormat(" UPDATE invlot SET invlot.Qty = invlot.Qty - {0},updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                        strSqlInvLot.AppendFormat(" where invlot.sysId='{0}'   and invlot.Qty > 0 AND (invlot.Qty - invlot.PickedQty - invlot.AllocatedQty - invlot.FrozenQty) >= {1} ; ", info.InvLotSysId, info.Qty);
                    }
                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Where(p => p.InvLotSysId != new Guid()).Count() != invLotResult)
                {
                    throw new Exception("库存不足,无法取消上架");
                }

                var strSqlSkuLoc = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    if (info.InvSkuLocSysId != new Guid())
                    {
                        strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET  invSkuLoc.Qty = invSkuLoc.Qty - {0}  ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                        strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}'  and   invSkuLoc.Qty > 0 AND (invSkuLoc.Qty - invSkuLoc.PickedQty - invSkuLoc.AllocatedQty - invSkuLoc.FrozenQty) >= {1};", info.InvSkuLocSysId, info.Qty);
                    }
                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Where(p => p.InvSkuLocSysId != new Guid()).Count() != invSkuLocResult)
                {
                    throw new Exception("库存不足,无法取消上架");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new CommonResponse();
        }

        #region 取消收货
        /// <summary>
        /// 更新入库单，入库单明细，收货单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        public CommonResponse UpdateReceiptDetailCancelReceipt(ReceiptCancelDto receiptCancelDto)
        {
            try
            {
                var strSql = new StringBuilder();
                strSql.Append(" update purchasedetail set ReceivedQty = 0,RejectedQty = 0,ReceivedGiftQty = 0, RejectedGiftQty = 0 where PurchaseSysId = @PurchaseSysId;");
                strSql.Append(" update receipt set Status = @ReceiptStatus, TotalReceivedQty = 0, TotalRejectedQty = 0,UpdateBy=@UpdateBy,UpdateUserName=@UpdateUserName,UpdateDate=now(),TS=@TS where ExternalOrder = @ExternalOrder and Status != @ReceiptStatus;");
                strSql.Append(" update receiptdetail set Status = @ReceiptDetailStatus, ReceivedQty = 0, RejectedQty = 0,UpdateBy=@UpdateBy,UpdateUserName=@UpdateUserName,UpdateDate=now(),TS=@TS where receiptsysid in (select sysid from receipt where  ExternalOrder = @ExternalOrder and Status != @ReceiptStatus);");

                var result = base.Context.Database.ExecuteSqlCommand(strSql.ToString()
                    , new MySqlParameter("@PurchaseSysId", receiptCancelDto.PurchaseSysId)
                    , new MySqlParameter("@ReceiptStatus", (int)ReceiptStatus.Cancel)
                    , new MySqlParameter("@UpdateBy", receiptCancelDto.CurrentUserId)
                    , new MySqlParameter("@UpdateUserName", receiptCancelDto.CurrentDisplayName)
                    , new MySqlParameter("@TS", Guid.NewGuid())
                    , new MySqlParameter("@ExternalOrder", receiptCancelDto.PurchaseOrder)
                    , new MySqlParameter("@ReceiptDetailStatus", (int)ReceiptDetailStatus.Cancel));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new CommonResponse();
        }
        #endregion

        #region 库存冻结

        /// <summary>
        /// 冻结库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <param name="updateId"></param>
        /// <param name="updateName"></param>
        public void UpdateInvForFrozenRequest(List<InvLotLocLpnDto> updateInventoryList, int updateId, string updateName)
        {
            try
            {
                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET  invlotloclpn.FrozenQty = invlotloclpn.FrozenQty + {0},updateBy =@updateBy,UpdateDate = now(),UpdateUserName = @UpdateUserName ", info.Qty);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' and invlotloclpn.PickedQty = 0 and invlotloclpn.AllocatedQty = 0 ; ", info.InvLotLocLpnSysId);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("批次货位库存占用,无法冻结库存");

                }

                var strSqlInvLot = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET  invlot.FrozenQty = invlot.FrozenQty + {0} ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    //strSqlInvLot.AppendFormat(" where invlot.sysId='{0}' and invlot.PickedQty = 0 and invlot.AllocatedQty = 0; ", info.InvLotSysId);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}' ; ", info.InvLotSysId);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("批次库存占用,无法冻结库存");
                }

                var strSqlSkuLoc = new StringBuilder();


                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET  invSkuLoc.FrozenQty = invSkuLoc.FrozenQty + {0}  ,updateBy =@updateBy,UpdateDate = now(),UpdateUserName =@UpdateUserName ", info.Qty);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}'  and invSkuLoc.PickedQty = 0 and invSkuLoc.AllocatedQty = 0;", info.InvSkuLocSysId);
                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString()
                    , new MySqlParameter("@updateBy", updateId)
                    , new MySqlParameter("@UpdateUserName", updateName));
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("货位库存占用,无法冻结库存");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        /// <summary>
        /// 退货入库，插入入库主表和批量插入入库单明细
        /// </summary>
        /// <param name="details"></param>
        public void BatchInsertPurchaseAndDetails(purchase purchase, List<purchasedetail> details)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (details != null && details.Count > 0)
            {
                var sqlStringBuilder = new StringBuilder();
                sqlStringBuilder.Append(@"INSERT INTO purchase(SysId, PurchaseOrder, DeliveryDate, 
                                            ExternalOrder, VendorSysId, Descr, PurchaseDate, 
                                            AuditingDate, AuditingBy, AuditingName, CreateBy, CreateDate, 
                                            Status, Type, Source, PoGroup, FromWareHouseSysId,
                                            WarehouseSysId, CreateUserName, UPdateUserName, UpdateBy, UpdateDate,
                                            Channel, BatchNumber, OutboundSysId, OutboundOrder, BusinessType)");

                sqlStringBuilder.Append($@"VALUES (@SysId, @PurchaseOrder, NOW(), @ExternalOrder,
                                            @VendorSysId, @Descr, NOW(), NOW(), @AuditingBy, @AuditingName, 
                                            @CreateBy, NOW(), @Status, @Type, @Source, @PoGroup, @FromWareHouseSysId,
                                             @WarehouseSysId, @CreateUserName, @UpdateUserName, @UpdateBy, NOW(),
                                            @Channel, @BatchNumber, @OutboundSysId, @OutboundOrder, @BusinessType);");

                parameters.Add(new MySqlParameter("@SysId", purchase.SysId));
                parameters.Add(new MySqlParameter("@PurchaseOrder", purchase.PurchaseOrder));
                parameters.Add(new MySqlParameter("@ExternalOrder", purchase.ExternalOrder));
                parameters.Add(new MySqlParameter("@VendorSysId", purchase.VendorSysId));
                parameters.Add(new MySqlParameter("@Descr", purchase.Descr));
                parameters.Add(new MySqlParameter("@AuditingBy", purchase.AuditingBy));
                parameters.Add(new MySqlParameter("@AuditingName", purchase.AuditingName));
                parameters.Add(new MySqlParameter("@CreateBy", purchase.CreateBy));
                parameters.Add(new MySqlParameter("@Status", purchase.Status));
                parameters.Add(new MySqlParameter("@Type", purchase.Type));
                parameters.Add(new MySqlParameter("@Source", purchase.Source));
                parameters.Add(new MySqlParameter("@PoGroup", purchase.PoGroup));
                parameters.Add(new MySqlParameter("@FromWareHouseSysId", purchase.FromWareHouseSysId));
                parameters.Add(new MySqlParameter("@WarehouseSysId", purchase.WarehouseSysId));
                parameters.Add(new MySqlParameter("@CreateUserName", purchase.CreateUserName));
                parameters.Add(new MySqlParameter("@UpdateUserName", purchase.UpdateUserName));
                parameters.Add(new MySqlParameter("@UpdateBy", purchase.UpdateBy));
                parameters.Add(new MySqlParameter("@Channel", purchase.Channel));
                parameters.Add(new MySqlParameter("@BatchNumber", purchase.BatchNumber));
                parameters.Add(new MySqlParameter("@OutboundSysId", purchase.OutboundSysId));
                parameters.Add(new MySqlParameter("@OutboundOrder", purchase.OutboundOrder));
                parameters.Add(new MySqlParameter("@BusinessType", purchase.BusinessType));

                sqlStringBuilder.Append(@"INSERT INTO purchasedetail(SysId, PurchaseSysId, SkuSysId, 
                                                SkuClassSysId, UomCode, UOMSysId, PackSysId, PackCode,
                                                Qty, ReceivedQty, RejectedQty, LastPrice, HistoryPrice, 
                                                PurchasePrice, Remark, OtherSkuId, PackFactor) VALUES ");

                var i = 1;

                foreach (var pd in details)
                {
                    sqlStringBuilder.Append($@"(UUID(), @PurchaseSysId{i}, @SkuSysId{i}, @SkuClassSysId{i},
                                                    @UomCode{i}, @UOMSysId{i}, @PackSysId{i}, @PackCode{i},
                                                    @Qty{i}, @ReceivedQty{i}, @RejectedQty{i}, @LastPrice{i}, 
                                                    @HistoryPrice{i},@PurchasePrice{i}, 
                                                    @Remark{i}, @OtherSkuId{i}, @PackFactor{i}),");

                    parameters.Add(new MySqlParameter($"@PurchaseSysId{i}", pd.PurchaseSysId));
                    parameters.Add(new MySqlParameter($"@SkuSysId{i}", pd.SkuSysId));
                    parameters.Add(new MySqlParameter($"@SkuClassSysId{i}", pd.SkuClassSysId));
                    parameters.Add(new MySqlParameter($"@UomCode{i}", pd.UomCode));
                    parameters.Add(new MySqlParameter($"@UOMSysId{i}", pd.UOMSysId));
                    parameters.Add(new MySqlParameter($"@PackSysId{i}", pd.PackSysId));
                    parameters.Add(new MySqlParameter($"@PackCode{i}", pd.PackCode));
                    parameters.Add(new MySqlParameter($"@Qty{i}", pd.Qty));
                    parameters.Add(new MySqlParameter($"@ReceivedQty{i}", pd.ReceivedQty));
                    parameters.Add(new MySqlParameter($"@RejectedQty{i}", pd.RejectedQty));
                    parameters.Add(new MySqlParameter($"@LastPrice{i}", 0));
                    parameters.Add(new MySqlParameter($"@HistoryPrice{i}", 0));
                    parameters.Add(new MySqlParameter($"@PurchasePrice{i}", pd.PurchasePrice));
                    parameters.Add(new MySqlParameter($"@Remark{i}", pd.Remark));
                    parameters.Add(new MySqlParameter($"@OtherSkuId{i}", pd.OtherSkuId));
                    parameters.Add(new MySqlParameter($"@PackFactor{i}", pd.PackFactor));

                    i++;
                }

                string sqlString = sqlStringBuilder.ToString().TrimEnd(',') + ";";

                base.Context.Database.ExecuteSqlCommand(sqlString, parameters.ToArray());
            }
        }


        public void BatchUpdateLocationStatusByZone(Guid zoneSysId, Guid warehouseSysId, LocationStatus status)
        {
            var locations = this.GetQuery<location>(p => p.ZoneSysId == zoneSysId && p.WarehouseSysId == warehouseSysId);

            string sqlString = @"UPDATE location l 
                                        SET l.Status = @Status 
                                        WHERE l.ZoneSysId = @ZoneSysId 
                                        AND l.WarehouseSysId = @WarehouseSysId 
                                        AND l.Status <> @Status;";

            int result = base.Context.Database.ExecuteSqlCommand(sqlString,
                new MySqlParameter("@Status", (int)status),
                new MySqlParameter("@ZoneSysId", zoneSysId),
                new MySqlParameter("@WarehouseSysId", warehouseSysId));
            if (result != locations.Count())
            {
                throw new Exception("储区下已有货位状态发生变化，请检查!");
            }
        }


        /// <summary>
        /// 获取收货明细相同批次
        /// </summary>
        /// <param name="receiptDetails"></param>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public List<ReceiptDetailCheckLotDto> GetToLotByReceiptDetail(List<receiptdetail> receiptDetails, Guid purchaseSysId, Guid wareHouseSysId)
        {
            try
            {
                var strSql = new StringBuilder();
                strSql.Append(@"select rod.SysId AS SysId, rd.SysId AS CheckLotSysId, rd.ReceiptSysId AS ReceiptSysId,
                                rd.ToLot AS ToLot, rd.ToLoc AS ToLoc, 
                                rd.ToLpn AS ToLpn from ( ");
                var subSqls = new List<string>();
                List<MySqlParameter> paraList = new List<MySqlParameter>();
                int i = 1;
                foreach (var info in receiptDetails)
                {
                    var subSql = new StringBuilder();
                    subSql.Append($"select @SysId{i} as SysId, @SkuSysId{i} as SkuSysId");
                    subSql.Append(info.LotAttr01 == null ? ", null as LotAttr01" : $", @LotAttr01{i} as LotAttr01");
                    subSql.Append(info.LotAttr02 == null ? ", null as LotAttr02" : $", @LotAttr02{i} as LotAttr02");
                    subSql.Append(info.LotAttr03 == null ? ", null as LotAttr03" : $", @LotAttr03{i} as LotAttr03");
                    subSql.Append(info.LotAttr04 == null ? ", null as LotAttr04" : $", @LotAttr04{i} as LotAttr04");
                    subSql.Append(info.LotAttr05 == null ? ", null as LotAttr05" : $", @LotAttr05{i} as LotAttr05");
                    subSql.Append(info.LotAttr06 == null ? ", null as LotAttr06" : $", @LotAttr06{i} as LotAttr06");
                    subSql.Append(info.LotAttr07 == null ? ", null as LotAttr07" : $", @LotAttr07{i} as LotAttr07");
                    subSql.Append(info.LotAttr08 == null ? ", null as LotAttr08" : $", @LotAttr08{i} as LotAttr08");
                    subSql.Append(info.LotAttr09 == null ? ", null as LotAttr09" : $", @LotAttr09{i} as LotAttr09");
                    subSql.Append(!info.ProduceDate.HasValue ? ", null as ProduceDate" : $", '{info.ProduceDate.Value}' as ProduceDate");
                    subSql.Append(info.ExternalLot == null ? ", null as ExternalLot" : $", @ExternalLot{i} as ExternalLot");
                    subSql.Append(!info.ExpiryDate.HasValue ? ", null as ExpiryDate from dual" : $", '{info.ExpiryDate.Value}' as ExpiryDate from dual");
                    subSqls.Add(subSql.ToString());

                    paraList.Add(new MySqlParameter($"@SysId{i}", info.SysId));
                    paraList.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                    paraList.Add(new MySqlParameter($"@LotAttr01{i}", info.LotAttr01));
                    paraList.Add(new MySqlParameter($"@LotAttr02{i}", info.LotAttr02));
                    paraList.Add(new MySqlParameter($"@LotAttr03{i}", info.LotAttr03));
                    paraList.Add(new MySqlParameter($"@LotAttr04{i}", info.LotAttr04));
                    paraList.Add(new MySqlParameter($"@LotAttr05{i}", info.LotAttr05));
                    paraList.Add(new MySqlParameter($"@LotAttr06{i}", info.LotAttr06));
                    paraList.Add(new MySqlParameter($"@LotAttr07{i}", info.LotAttr07));
                    paraList.Add(new MySqlParameter($"@LotAttr08{i}", info.LotAttr08));
                    paraList.Add(new MySqlParameter($"@LotAttr09{i}", info.LotAttr09));
                    paraList.Add(new MySqlParameter($"@ExternalLot{i}", info.ExternalLot));

                    i++;
                }
                strSql.Append(string.Join(" union all ", subSqls));
                strSql.Append(@" ) rod
                            left join 
                            (
                                SELECT r1.* FROM purchase p
                                INNER JOIN receipt r
                                ON p.PurchaseOrder = r.ExternalOrder
                                INNER JOIN receiptdetail r1
                                ON r.SysId = r1.ReceiptSysId
                                WHERE p.SysId = '{0}' AND p.WarehouseSysId = '{1}' AND r.ReceiptType != {2}
                            ) rd
                            ON rod.SkuSysId = rd.SkuSysId
                            AND (ifnull(rod.LotAttr01,'') = ifnull(rd.LotAttr01,''))
                            AND (ifnull(rod.LotAttr02,'') = ifnull(rd.LotAttr02,''))
                            AND (ifnull(rod.LotAttr03,'') = ifnull(rd.LotAttr03,''))
                            AND (ifnull(rod.LotAttr04,'') = ifnull(rd.LotAttr04,''))
                            AND (ifnull(rod.LotAttr05,'') = ifnull(rd.LotAttr05,''))
                            AND (ifnull(rod.LotAttr06,'') = ifnull(rd.LotAttr06,''))
                            AND (ifnull(rod.LotAttr07,'') = ifnull(rd.LotAttr07,''))
                            AND (ifnull(rod.LotAttr08,'') = ifnull(rd.LotAttr08,''))
                            AND (ifnull(rod.LotAttr09,'') = ifnull(rd.LotAttr09,''))
                            AND (ifnull(rod.ProduceDate,'') = ifnull(rd.ProduceDate,''))
                            AND (ifnull(rod.ExternalLot,'') = ifnull(rd.ExternalLot,''))
                            AND (ifnull(rod.ExpiryDate,'') = ifnull(rd.ExpiryDate,''))");
                string sql = string.Format(strSql.ToString(), purchaseSysId, wareHouseSysId, (int)ReceiptType.FIFO);
                return Context.Database.SqlQuery<ReceiptDetailCheckLotDto>(sql, paraList.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量插入收货明细
        /// </summary>
        /// <param name="receiptDetails"></param>
        /// <returns></returns>
        public CommonResponse BatchInsertReceiptDetail(List<receiptdetail> receiptDetails)
        {
            try
            {
                var strSql = new StringBuilder();
                strSql.Append(@" insert into receiptdetail (  SysId, ReceiptSysId, Status, 
                                    SkuSysId, ExpectedQty, ReceivedQty, UOMSysId, PackSysId, 
                                    ReceivedDate, Price, LotAttr01, LotAttr02, LotAttr03, 
                                    LotAttr04, LotAttr05, LotAttr06, LotAttr07, LotAttr08, 
                                    LotAttr09, ExternalLot, ProduceDate, ExpiryDate, ToLoc, 
                                    ToLot,ShelvesQty, ShelvesStatus, CreateBy, CreateUserName, 
                                    CreateDate, UpdateBy, UpdateUserName, UpdateDate, 
                                    IsMustLot, IsDefaultLot ) values ");

                List<MySqlParameter> parameters = new List<MySqlParameter>();
                var i = 0;
                foreach (var info in receiptDetails)
                {
                    strSql.Append($@"( @SysId{i}, @ReceiptSysId{i}, @Status{i}, 
                                            @SkuSysId{i}, @ExpectedQty{i}, @ReceivedQty{i}, @UOMSysId{i}, @PackSysId{i}, 
                                            @ReceivedDate{i}, @Price{i}, @LotAttr01{i}, @LotAttr02{i}, @LotAttr03{i}, 
                                            @LotAttr04{i}, @LotAttr05{i}, @LotAttr06{i}, @LotAttr07{i}, @LotAttr08{i}, 
                                            @LotAttr09{i}, @ExternalLot{i}, @ProduceDate{i}, @ExpiryDate{i}, @ToLoc{i}, 
                                            @ToLot{i}, @ShelvesQty{i}, @ShelvesStatus{i}, @CreateBy{i}, @CreateUserName{i}, 
                                            @CreateDate{i}, @UpdateBy{i}, @UpdateUserName{i}, @UpdateDate{i}, 
                                            @IsMustLot{i}, @IsDefaultLot{i} ),");

                    parameters.Add(new MySqlParameter($"@SysId{i}", info.SysId));
                    parameters.Add(new MySqlParameter($"@ReceiptSysId{i}", info.ReceiptSysId));
                    parameters.Add(new MySqlParameter($"@Status{i}", info.Status));
                    parameters.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                    parameters.Add(new MySqlParameter($"@ExpectedQty{i}", info.ExpectedQty));
                    parameters.Add(new MySqlParameter($"@ReceivedQty{i}", info.ReceivedQty));
                    parameters.Add(new MySqlParameter($"@UOMSysId{i}", info.UOMSysId));
                    parameters.Add(new MySqlParameter($"@PackSysId{i}", info.PackSysId));
                    parameters.Add(new MySqlParameter($"@ReceivedDate{i}", info.ReceivedDate));
                    parameters.Add(new MySqlParameter($"@Price{i}", info.Price));
                    parameters.Add(new MySqlParameter($"@LotAttr01{i}", info.LotAttr01));
                    parameters.Add(new MySqlParameter($"@LotAttr02{i}", info.LotAttr02));
                    parameters.Add(new MySqlParameter($"@LotAttr03{i}", info.LotAttr03));
                    parameters.Add(new MySqlParameter($"@LotAttr04{i}", info.LotAttr04));
                    parameters.Add(new MySqlParameter($"@LotAttr05{i}", info.LotAttr05));
                    parameters.Add(new MySqlParameter($"@LotAttr06{i}", info.LotAttr06));
                    parameters.Add(new MySqlParameter($"@LotAttr07{i}", info.LotAttr07));
                    parameters.Add(new MySqlParameter($"@LotAttr08{i}", info.LotAttr08));
                    parameters.Add(new MySqlParameter($"@LotAttr09{i}", info.LotAttr09));
                    parameters.Add(new MySqlParameter($"@ExternalLot{i}", info.ExternalLot));
                    parameters.Add(new MySqlParameter($"@ProduceDate{i}", info.ProduceDate));
                    parameters.Add(new MySqlParameter($"@ExpiryDate{i}", info.ExpiryDate));
                    parameters.Add(new MySqlParameter($"@ToLoc{i}", info.ToLoc));
                    parameters.Add(new MySqlParameter($"@ToLot{i}", info.ToLot));
                    parameters.Add(new MySqlParameter($"@ShelvesQty{i}", info.ShelvesQty));
                    parameters.Add(new MySqlParameter($"@ShelvesStatus{i}", info.ShelvesStatus));
                    parameters.Add(new MySqlParameter($"@CreateBy{i}", info.CreateBy));
                    parameters.Add(new MySqlParameter($"@CreateUserName{i}", info.CreateUserName));
                    parameters.Add(new MySqlParameter($"@CreateDate{i}", DateTime.Now));
                    parameters.Add(new MySqlParameter($"@UpdateBy{i}", info.UpdateBy));
                    parameters.Add(new MySqlParameter($"@UpdateUserName{i}", info.UpdateUserName));
                    parameters.Add(new MySqlParameter($"@UpdateDate{i}", DateTime.Now));
                    parameters.Add(new MySqlParameter($"@IsMustLot{i}", info.IsMustLot));
                    parameters.Add(new MySqlParameter($"@IsDefaultLot{i}", info.IsDefaultLot));
                    i++;
                }
                var strSql1 = strSql.ToString().TrimEnd(',') + " ;";
                base.Context.Database.ExecuteSqlCommand(strSql1, parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new CommonResponse();
        }


        /// <summary>
        /// 批量插入拣货单
        /// </summary>
        /// <param name="pickdetail"></param>
        /// <returns></returns>
        public CommonResponse BatchInsertPickDetail(List<pickdetail> pickdetail)
        {
            try
            {
                var strSql = new StringBuilder(@" INSERT INTO pickdetail  
                                        (  SysId ,WareHouseSysId ,OutboundSysId ,OutboundDetailSysId ,
                                        PickDetailOrder ,PickDate ,Status ,SkuSysId ,UOMSysId ,PackSysId ,
                                        Loc ,Lot ,Lpn ,Qty ,CreateBy ,CreateDate ,UpdateBy ,
                                        UpdateDate ,CreateUserName ,UpdateUserName ,SourceType) VALUES ");

                var para = new List<MySqlParameter>();
                int i = 0;

                foreach (var info in pickdetail)
                {
                    strSql.Append($@" ( @SysId{i},@WareHouseSysId{i},@OutboundSysId{i},@OutboundDetailSysId{i},
                                        @PickDetailOrder{i},NOW() ,@Status{i},@SkuSysId{i},@UOMSysId{i},@PackSysId{i},
                                        @Loc{i},@Lot{i},@Lpn{i},@Qty{i},@CreateBy{i},NOW() ,
                                        @UpdateBy{i},NOW() ,@CreateUserName{i},@UpdateUserName{i},@SourceType{i} ),");

                    para.Add(new MySqlParameter($"@SysId{i}", info.SysId));
                    para.Add(new MySqlParameter($"@WareHouseSysId{i}", info.WareHouseSysId));
                    para.Add(new MySqlParameter($"@OutboundSysId{i}", info.OutboundSysId));
                    para.Add(new MySqlParameter($"@OutboundDetailSysId{i}", info.OutboundDetailSysId));
                    para.Add(new MySqlParameter($"@PickDetailOrder{i}", info.PickDetailOrder));
                    para.Add(new MySqlParameter($"@Status{i}", info.Status));
                    para.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));

                    para.Add(new MySqlParameter($"@UOMSysId{i}", info.UOMSysId));
                    para.Add(new MySqlParameter($"@PackSysId{i}", info.PackSysId));
                    para.Add(new MySqlParameter($"@Loc{i}", info.Loc));
                    para.Add(new MySqlParameter($"@Lot{i}", info.Lot));
                    para.Add(new MySqlParameter($"@Lpn{i}", info.Lpn));
                    para.Add(new MySqlParameter($"@Qty{i}", info.Qty));

                    para.Add(new MySqlParameter($"@CreateBy{i}", info.CreateBy));
                    para.Add(new MySqlParameter($"@UpdateBy{i}", info.UpdateBy));
                    para.Add(new MySqlParameter($"@CreateUserName{i}", info.CreateUserName));
                    para.Add(new MySqlParameter($"@UpdateUserName{i}", info.UpdateUserName));
                    para.Add(new MySqlParameter($"@SourceType{i}", info.SourceType));

                    i++;
                }
                var strSql1 = strSql.ToString().TrimEnd(',') + " ;";

                base.Context.Database.ExecuteSqlCommand(strSql1, para.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new CommonResponse();
        }


        /// <summary>
        /// 批量更新出库单（创建拣货单用，如果后续需要可以扩展）
        /// </summary>
        /// <param name="outbounddetailList"></param>
        /// <returns></returns>
        public CommonResponse BatchUpdateOutboundDetail(List<outbounddetail> outbounddetailList)
        {
            try
            {
                var strSql = new StringBuilder();

                var para = new List<MySqlParameter>();
                int i = 0;

                foreach (var info in outbounddetailList)
                {
                    strSql.Append($@"UPDATE `outbounddetail`
                                                    SET    
                                                   `Status` = @Status{i},
                                                   `UpdateBy` = @UpdateBy{i},
                                                   `UpdateDate` = now(), 
                                                   `AllocatedQty` = @AllocatedQty{i},
                                                   `UpdateUserName` = @UpdateUserName{i} 
                                                   WHERE  `SysId` =@SysId{i} ;");

                    para.Add(new MySqlParameter($"@Status{i}", info.Status));
                    para.Add(new MySqlParameter($"@UpdateBy{i}", info.UpdateBy));
                    para.Add(new MySqlParameter($"@AllocatedQty{i}", info.AllocatedQty));
                    para.Add(new MySqlParameter($"@UpdateUserName{i}", info.UpdateUserName));
                    para.Add(new MySqlParameter($"@SysId{i}", info.SysId));

                    i++;
                }

                base.Context.Database.ExecuteSqlCommand(strSql.ToString(), para.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new CommonResponse();
        }


        /// <summary>
        /// 收货完成更新入库单明细数量
        /// </summary>
        /// <param name="updatePurchaseDetailList"></param>
        /// <returns></returns>
        public CommonResponse UpdatePurchaseDetailAfterReceipt(List<UpdatePurchaseDetailDto> updatePurchaseDetailList)
        {
            var sql = new StringBuilder();

            var para = new List<MySqlParameter>();
            int i = 0;
            foreach (var item in updatePurchaseDetailList)
            {
                sql.AppendFormat($@"update purchasedetail p set p.ReceivedQty = p.ReceivedQty + @ReceivedQty{i},
                                        p.ReceivedGiftQty = p.ReceivedGiftQty + @ReceivedGiftQty{i}, 
                                        p.RejectedQty = p.RejectedQty + @RejectedQty{i},
                                        p.RejectedGiftQty = p.RejectedGiftQty + @RejectedGiftQty{i},
                                        p.Remark = CONCAT(p.Remark, @Remark{i}),
                                        p.UpdateDate = NOW(),
                                        p.UpdateBy = @UpdateBy{i},
                                        p.UpdateUserName = @UpdateUserName{i}
                                    where p.sysId =@sysId{i} 
                                    and (p.Qty - p.ReceivedQty - p.RejectedQty) >= (@ReceivedQty{i});");

                para.Add(new MySqlParameter($"@ReceivedQty{i}", item.ReceivedQty));
                para.Add(new MySqlParameter($"@RejectedQty{i}", item.RejectedQty));
                para.Add(new MySqlParameter($"@Remark{i}", item.Remark));
                para.Add(new MySqlParameter($"@ReceivedGiftQty{i}", item.ReceivedGiftQty));
                para.Add(new MySqlParameter($"@RejectedGiftQty{i}", item.RejectedGiftQty));
                para.Add(new MySqlParameter($"@sysId{i}", item.SysId));
                para.Add(new MySqlParameter($"@UpdateBy{i}", item.UpdateBy));
                para.Add(new MySqlParameter($"@UpdateUserName{i}", item.UpdateUserName));

                i++;

            }
            var purchaseDetailResult = base.Context.Database.ExecuteSqlCommand(sql.ToString(), para.ToArray());
            if (updatePurchaseDetailList.GroupBy(x => x.SysId).Count() != purchaseDetailResult)
            {
                throw new Exception("本次收货数量不能大于剩余收货数量，请重新加载数据");
            }
            return new CommonResponse();

        }

        /// <summary>
        /// 库存变更修改库存
        /// </summary>
        /// <param name="stockMovement"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentDisplayName"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryStockMovement(stockmovement stockMovement, int currentUserId, string currentDisplayName, bool isFromFrozen, bool isToFrozen)
        {
            #region invskuloc
            string updateInvLocReduce = @"UPDATE invskuloc i 
                                                        SET i.Qty = i.Qty - @Qty ,
                                                        i.FrozenQty = i.FrozenQty - @FrozenQty , 
                                                        i.UpdateBy = @UpdateBy , 
                                                        i.UpdateDate = NOW(), 
                                                        i.UpdateUserName =@UpdateUserName  
                                                        WHERE i.SkuSysId =@SkuSysId 
                                                        AND i.Loc = @Loc 
                                                        AND i.WareHouseSysId =@WareHouseSysId 
                                                        AND i.Qty - i.AllocatedQty - i.PickedQty - @Qty >= 0 ;";

            //string updateInvLocReduceSql = string.Format(updateInvLocReduceTemplate, stockMovement.FromQty, currentUserId, currentDisplayName, stockMovement.SkuSysId, stockMovement.FromLoc, stockMovement.WareHouseSysId, (isFromFrozen ? stockMovement.FromQty : "0"));

            var updateInvLocReduceResult = Context.Database.ExecuteSqlCommand(updateInvLocReduce,
                new MySqlParameter($"@Qty", stockMovement.FromQty),
                new MySqlParameter($"@FrozenQty", (isFromFrozen ? stockMovement.FromQty : "0")),
                new MySqlParameter($"@UpdateBy", currentUserId),
                new MySqlParameter($"@UpdateUserName", currentDisplayName),
                new MySqlParameter($"@SkuSysId", stockMovement.SkuSysId),
                new MySqlParameter($"@Loc", stockMovement.FromLoc),
                new MySqlParameter($"@WareHouseSysId", stockMovement.WareHouseSysId));

            if (updateInvLocReduceResult == 0)
            {
                throw new Exception("库存不足,无法进行变更");
            }

            string insertInvLocAddSql = @"INSERT INTO invskuloc ( SysId, WareHouseSysId, SkuSysId, 
                                                    Loc, Qty, AllocatedQty, PickedQty, 
                                                    CreateBy, CreateDate, UpdateBy, UpdateDate, 
                                                    CreateUserName, UpdateUserName,FrozenQty)
                                                    SELECT @SysId , @WareHouseSysId ,@SkuSysId , 
                                                    @Loc , @Qty, 0, 0,@CreateBy , NOW(), 
                                                    @CreateBy , NOW(), @CreateUserName ,@CreateUserName ,
                                                    @FrozenQty  FROM dual WHERE NOT EXISTS
                                                    (SELECT * FROM invskuloc i WHERE i.SkuSysId =@SkuSysId  
                                                     AND i.Loc =@Loc AND i.WareHouseSysId = @WareHouseSysId );";

            //  string insertInvLocAddSql = string.Format(insertInvLocAddTemplate, Guid.NewGuid(), stockMovement.WareHouseSysId, stockMovement.SkuSysId, stockMovement.ToLoc, stockMovement.ToQty,currentUserId, currentDisplayName, (isToFrozen ? stockMovement.ToQty : "0"));

            var insertInvLocAddResult = Context.Database.ExecuteSqlCommand(insertInvLocAddSql,
                    new MySqlParameter($"@SysId", Guid.NewGuid()),
                    new MySqlParameter($"@WareHouseSysId", stockMovement.WareHouseSysId),
                    new MySqlParameter($"@SkuSysId", stockMovement.SkuSysId),
                    new MySqlParameter($"@Loc", stockMovement.ToLoc),
                    new MySqlParameter($"@Qty", stockMovement.ToQty),
                    new MySqlParameter($"@CreateBy", currentUserId),
                    new MySqlParameter($"@CreateUserName", currentDisplayName),
                    new MySqlParameter($"@FrozenQty", (isToFrozen ? stockMovement.ToQty : "0")));
            if (insertInvLocAddResult == 0)
            {
                string updateInvLocAddSql = @"UPDATE invskuloc i 
                                                            SET i.Qty = i.Qty + @Qty ,
                                                            i.FrozenQty = i.FrozenQty + @FrozenQty , 
                                                            i.UpdateBy = @UpdateBy , 
                                                            i.UpdateDate = NOW(), 
                                                            i.UpdateUserName = @UpdateUserName  
                                                            WHERE i.SkuSysId = @SkuSysId 
                                                            AND i.Loc = @Loc 
                                                            AND i.WareHouseSysId = @WareHouseSysId ;";

                //string updateInvLocAddSql = string.Format(updateInvLocAddTemplate, stockMovement.ToQty, currentUserId, currentDisplayName, stockMovement.SkuSysId, stockMovement.ToLoc, stockMovement.WareHouseSysId, (isToFrozen ? stockMovement.ToQty : "0"));

                Context.Database.ExecuteSqlCommand(updateInvLocAddSql,
                        new MySqlParameter($"@Qty", stockMovement.ToQty),
                        new MySqlParameter($"@FrozenQty", (isToFrozen ? stockMovement.ToQty : "0")),
                        new MySqlParameter($"@UpdateBy", currentUserId),
                        new MySqlParameter($"@UpdateUserName", currentDisplayName),
                        new MySqlParameter($"@SkuSysId", stockMovement.SkuSysId),
                        new MySqlParameter($"@Loc", stockMovement.ToLoc),
                        new MySqlParameter($"@WareHouseSysId", stockMovement.WareHouseSysId));
            }
            #endregion

            #region invlotloclpn
            string updateInvLotLocLpnReduceSql = @"UPDATE invlotloclpn i 
                                                            SET i.Qty = i.Qty - @Qty ,
                                                            i.FrozenQty = i.FrozenQty - @FrozenQty , 
                                                            i.UpdateBy = @UpdateBy , 
                                                            i.UpdateDate = NOW(), 
                                                            i.UpdateUserName = @UpdateUserName 
                                                            WHERE i.SkuSysId = @SkuSysId 
                                                            AND i.Lot = @Lot  
                                                            AND i.Loc = @Loc 
                                                            AND i.WareHouseSysId = @WareHouseSysId  
                                                            AND i.Qty - i.AllocatedQty - i.PickedQty - @Qty >= 0 ;";

            //string updateInvLotLocLpnReduceSql = string.Format(updateInvLotLocLpnReduceTemplate, stockMovement.FromQty, currentUserId, currentDisplayName, stockMovement.SkuSysId, stockMovement.Lot, stockMovement.FromLoc, stockMovement.WareHouseSysId, (isFromFrozen ? stockMovement.FromQty : "0"));

            var updateInvLotLocLpnReduceResult = Context.Database.ExecuteSqlCommand(updateInvLotLocLpnReduceSql,
                                new MySqlParameter($"@Qty", stockMovement.FromQty),
                                new MySqlParameter($"@FrozenQty", (isFromFrozen ? stockMovement.FromQty : "0")),
                                new MySqlParameter($"@UpdateBy", currentUserId),
                                new MySqlParameter($"@UpdateUserName", currentDisplayName),
                                new MySqlParameter($"@SkuSysId", stockMovement.SkuSysId),
                                new MySqlParameter($"@Lot", stockMovement.Lot),
                                new MySqlParameter($"@Loc", stockMovement.FromLoc),
                                new MySqlParameter($"@WareHouseSysId", stockMovement.WareHouseSysId));
            if (updateInvLotLocLpnReduceResult == 0)
            {
                throw new Exception("库存不足,无法进行变更");
            }

            string insertInvLotLocLpnAddSql = @"INSERT INTO invlotloclpn ( SysId, WareHouseSysId, 
                                                        SkuSysId, Loc, Lot, Lpn, Qty, AllocatedQty, 
                                                        PickedQty, Status, CreateBy, CreateDate, 
                                                        UpdateBy, UpdateDate, CreateUserName, 
                                                        UpdateUserName,FrozenQty)
                                            SELECT  @SysId , @WareHouseSysId ,@SkuSysId , @Loc ,
                                                    @Lot ,@Lpn , @Qty , 0, 0, 
                                                    @Status ,@UpdateBy , NOW(), @UpdateBy , NOW(), 
                                                    @UpdateUserName ,@UpdateUserName ,@FrozenQty 
                                                    FROM dual WHERE NOT EXISTS(SELECT * FROM 
                                                    invlotloclpn i WHERE i.SkuSysId = @SkuSysId 
                                                    AND i.Lot = @Lot  AND i.Loc = @Loc 
                                                    AND i.WareHouseSysId = @WareHouseSysId );";

            //string insertInvLotLocLpnAddSql = string.Format(insertInvLotLocLpnAddTemplate, Guid.NewGuid(), stockMovement.WareHouseSysId, stockMovement.SkuSysId, stockMovement.ToLoc, stockMovement.Lot, string.Empty, stockMovement.ToQty, 1, currentUserId, currentDisplayName, (isToFrozen ? stockMovement.ToQty : "0"));

            var insertInvLotLocLpnAddResult = Context.Database.ExecuteSqlCommand(insertInvLotLocLpnAddSql,
                                    new MySqlParameter($"@SysId", Guid.NewGuid()),
                                    new MySqlParameter($"@WareHouseSysId", stockMovement.WareHouseSysId),
                                    new MySqlParameter($"@SkuSysId", stockMovement.SkuSysId),
                                    new MySqlParameter($"@Loc", stockMovement.ToLoc),
                                    new MySqlParameter($"@Lot", stockMovement.Lot),
                                    new MySqlParameter($"@Lpn", string.Empty),
                                    new MySqlParameter($"@Qty", stockMovement.ToQty),
                                    new MySqlParameter($"@Status", 1),
                                    new MySqlParameter($"@UpdateBy", currentUserId),
                                    new MySqlParameter($"@UpdateUserName", currentDisplayName),
                                    new MySqlParameter($"@FrozenQty", (isToFrozen ? stockMovement.ToQty : "0")));

            if (insertInvLotLocLpnAddResult == 0)
            {
                string updateInvLotLocLpnAddSql = @"UPDATE invlotloclpn i 
                                                                    SET i.Qty = i.Qty + @Qty ,
                                                                    i.FrozenQty = i.FrozenQty + @FrozenQty , 
                                                                    i.UpdateBy = @UpdateBy , 
                                                                    i.UpdateDate = NOW(), 
                                                                    i.UpdateUserName = @UpdateUserName 
                                                                    WHERE i.SkuSysId = @SkuSysId   
                                                                    AND i.Lot = @Lot  
                                                                    AND i.Loc = @Loc 
                                                                    AND i.WareHouseSysId = @WareHouseSysId ;";

                //string updateInvLotLocLpnAddSql = string.Format(updateInvLotLocLpnAddTemplate, stockMovement.ToQty, currentUserId, currentDisplayName, stockMovement.SkuSysId, stockMovement.Lot, stockMovement.ToLoc, stockMovement.WareHouseSysId, (isToFrozen ? stockMovement.ToQty : "0"));

                Context.Database.ExecuteSqlCommand(updateInvLotLocLpnAddSql,
                                        new MySqlParameter($"@Qty", stockMovement.ToQty),
                                        new MySqlParameter($"@FrozenQty", (isToFrozen ? stockMovement.ToQty : "0")),
                                        new MySqlParameter($"@UpdateBy", currentUserId),
                                        new MySqlParameter($"@UpdateUserName", currentDisplayName),
                                        new MySqlParameter($"@SkuSysId", stockMovement.SkuSysId),
                                        new MySqlParameter($"@Loc", stockMovement.ToLoc),
                                        new MySqlParameter($"@Lot", stockMovement.Lot),
                                        new MySqlParameter($"@WareHouseSysId", stockMovement.WareHouseSysId));
            }
            #endregion

            #region  invlot
            string updateinvlotReduceSql = @"UPDATE invlot i 
                                                            SET i.FrozenQty = i.FrozenQty - @FrozenQty , 
                                                            i.UpdateBy = @UpdateBy , 
                                                            i.UpdateDate = NOW(), 
                                                            i.UpdateUserName = @UpdateUserName 
                                                            WHERE i.SkuSysId = @SkuSysId 
                                                            AND i.Lot = @Lot 
                                                            AND i.WareHouseSysId = @WareHouseSysId  ;";

            //string updateinvlotReduceSql = string.Format(updateinvlotReduceTemplate, (isFromFrozen ? stockMovement.FromQty : "0"), currentUserId, currentDisplayName, stockMovement.SkuSysId, stockMovement.Lot, stockMovement.WareHouseSysId);

            var updateinvlotReduceResult = Context.Database.ExecuteSqlCommand(updateinvlotReduceSql,
                                        new MySqlParameter($"@FrozenQty", (isFromFrozen ? stockMovement.FromQty : "0")),
                                        new MySqlParameter($"@UpdateBy", currentUserId),
                                        new MySqlParameter($"@UpdateUserName", currentDisplayName),
                                        new MySqlParameter($"@SkuSysId", stockMovement.SkuSysId),
                                        new MySqlParameter($"@Lot", stockMovement.Lot),
                                        new MySqlParameter($"@WareHouseSysId", stockMovement.WareHouseSysId));
            if (updateinvlotReduceResult == 0)
            {
                throw new Exception("库存不足,无法进行变更");
            }

            string updateinvlotLocLpnAddSql = @"UPDATE invlot i 
                                                            SET i.FrozenQty = i.FrozenQty + @FrozenQty , 
                                                            i.UpdateBy = @UpdateBy , 
                                                            i.UpdateDate = NOW(), 
                                                            i.UpdateUserName = @UpdateUserName 
                                                            WHERE i.SkuSysId = @SkuSysId 
                                                            AND i.Lot = @Lot 
                                                            AND i.WareHouseSysId = @WareHouseSysId ;";

            //string updateinvlotLocLpnAddSql = string.Format(updateinvlotAddTemplate, (isToFrozen ? stockMovement.ToQty : "0"), currentUserId, currentDisplayName, stockMovement.SkuSysId, stockMovement.Lot, stockMovement.WareHouseSysId);

            Context.Database.ExecuteSqlCommand(updateinvlotLocLpnAddSql,
                                        new MySqlParameter($"@FrozenQty", (isToFrozen ? stockMovement.ToQty : "0")),
                                        new MySqlParameter($"@UpdateBy", currentUserId),
                                        new MySqlParameter($"@UpdateUserName", currentDisplayName),
                                        new MySqlParameter($"@SkuSysId", stockMovement.SkuSysId),
                                        new MySqlParameter($"@Lot", stockMovement.Lot),
                                        new MySqlParameter($"@WareHouseSysId", stockMovement.WareHouseSysId));

            #endregion

            return new CommonResponse();
        }


        /// <summary>
        /// 自动上架更新收货明细
        /// </summary>
        public void UpdateReceiptDetailAfterAutoShelves(List<UpdateReceiptDetailDto> updateReceiptDetails)
        {
            if (updateReceiptDetails != null && updateReceiptDetails.Count > 0)
            {
                StringBuilder strSql = new StringBuilder();

                var para = new List<MySqlParameter>();
                int i = 0;

                foreach (var info in updateReceiptDetails)
                {
                    strSql.Append($@"update receiptdetail set 
                                                ShelvesQty =@ShelvesQty{i} , 
                                                ShelvesStatus =@ShelvesStatus{i} , 
                                                TS =@NewTS{i},
                                                UpdateBy = @UpdateBy{i},
                                                UpdateDate = @UpdateDate{i},
                                                UpdateUserName = @UpdateUserName{i}
                                                where SysId =@SysId{i} 
                                                and TS =@OldTS{i} ;");


                    para.Add(new MySqlParameter($"@ShelvesQty{i}", info.ShelvesQty));
                    para.Add(new MySqlParameter($"@ShelvesStatus{i}", info.ShelvesStatus));
                    para.Add(new MySqlParameter($"@NewTS{i}", info.NewTS)); 
                    para.Add(new MySqlParameter($"@UpdateBy{i}", info.CurrentUserId));
                    para.Add(new MySqlParameter($"@UpdateDate{i}", DateTime.Now)); 
                    para.Add(new MySqlParameter($"@UpdateUserName{i}", info.CurrentDisplayName)); 
                    para.Add(new MySqlParameter($"@SysId{i}", info.SysId));
                    para.Add(new MySqlParameter($"@OldTS{i}", info.OldTS));

                    i++;
                }
                var resultCount = base.Context.Database.ExecuteSqlCommand(strSql.ToString(), para.ToArray());
                if (updateReceiptDetails.Count != resultCount)
                {
                    throw new DbUpdateConcurrencyException();
                }
            }
        }

        /// <summary>
        /// 批量插入invtrans
        /// </summary>
        /// <param name="invtrans"></param>
        public void BatchInsertInvTrans(List<invtran> invtrans)
        {
            try
            {
                var strSql = new StringBuilder();
                strSql.Append(@"insert into invtrans(SysId, WareHouseSysId, DocOrder, 
                                            DocSysId, DocDetailSysId, SkuSysId, 
                                            SkuCode, TransType, SourceTransType, 
                                            Qty, Loc, Lot, Lpn, ToLoc, ToLot, ToLpn, Status, 
                                            LotAttr01, LotAttr02, LotAttr03, LotAttr04, 
                                            LotAttr05, LotAttr06, LotAttr07, LotAttr08, LotAttr09, 
                                            ExternalLot, PackSysId, PackCode, UOMSysId, 
                                            UOMCode, CreateBy, CreateDate, UpdateBy, UpdateDate,
                                            CreateUserName, UpdateUserName,
                                            ProduceDate, ExpiryDate, ReceivedDate ) values ");

                var para = new List<MySqlParameter>();
                int i = 0;
                foreach (var info in invtrans)
                {
                    strSql.Append($@"(@SysId{i},@WareHouseSysId{i},@DocOrder{i},@DocSysId{i},@DocDetailSysId{i}, 
                                      @SkuSysId{i},@SkuCode{i},@TransType{i}, @SourceTransType{i},@Qty{i},@Loc{i},
                                      @Lot{i}, @Lpn{i},@ToLoc{i},@ToLot{i},@ToLpn{i},@Status{i}, 
                                      @LotAttr01{i},@LotAttr02{i},@LotAttr03{i},@LotAttr04{i},@LotAttr05{i}, 
                                      @LotAttr06{i},@LotAttr07{i},@LotAttr08{i},@LotAttr09{i},@ExternalLot{i},
                                      @PackSysId{i}, @PackCode{i}, @UOMSysId{i},@UOMCode{i},@CreateBy{i}, NOW(), 
                                      @UpdateBy{i}, NOW(),@CreateUserName{i},@UpdateUserName{i},
                                      @ProduceDate{i},@ExpiryDate{i},@ReceivedDate{i} ),");

                    para.Add(new MySqlParameter($"@SysId{i}", info.SysId));
                    para.Add(new MySqlParameter($"@WareHouseSysId{i}", info.WareHouseSysId));
                    para.Add(new MySqlParameter($"@DocOrder{i}", info.DocOrder));
                    para.Add(new MySqlParameter($"@DocSysId{i}", info.DocSysId));
                    para.Add(new MySqlParameter($"@DocDetailSysId{i}", info.DocDetailSysId));
                    para.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                    para.Add(new MySqlParameter($"@SkuCode{i}", info.SkuCode));
                    para.Add(new MySqlParameter($"@TransType{i}", info.TransType));
                    para.Add(new MySqlParameter($"@SourceTransType{i}", info.SourceTransType));

                    para.Add(new MySqlParameter($"@Qty{i}", info.Qty));
                    para.Add(new MySqlParameter($"@Loc{i}", info.Loc));
                    para.Add(new MySqlParameter($"@Lot{i}", info.Lot));
                    para.Add(new MySqlParameter($"@Lpn{i}", info.Lpn));
                    para.Add(new MySqlParameter($"@ToLoc{i}", info.ToLoc));
                    para.Add(new MySqlParameter($"@ToLot{i}", info.ToLot));
                    para.Add(new MySqlParameter($"@ToLpn{i}", info.ToLpn));
                    para.Add(new MySqlParameter($"@Status{i}", info.Status));

                    para.Add(new MySqlParameter($"@LotAttr01{i}", info.LotAttr01));
                    para.Add(new MySqlParameter($"@LotAttr02{i}", info.LotAttr02));
                    para.Add(new MySqlParameter($"@LotAttr03{i}", info.LotAttr03));
                    para.Add(new MySqlParameter($"@LotAttr04{i}", info.LotAttr04));
                    para.Add(new MySqlParameter($"@LotAttr05{i}", info.LotAttr05));
                    para.Add(new MySqlParameter($"@LotAttr06{i}", info.LotAttr06));
                    para.Add(new MySqlParameter($"@LotAttr07{i}", info.LotAttr07));
                    para.Add(new MySqlParameter($"@LotAttr08{i}", info.LotAttr08));
                    para.Add(new MySqlParameter($"@LotAttr09{i}", info.LotAttr09));
                    para.Add(new MySqlParameter($"@ExternalLot{i}", info.ExternalLot));


                    para.Add(new MySqlParameter($"@PackSysId{i}", info.PackSysId));
                    para.Add(new MySqlParameter($"@PackCode{i}", info.PackCode));
                    para.Add(new MySqlParameter($"@UOMSysId{i}", info.UOMSysId));
                    para.Add(new MySqlParameter($"@UOMCode{i}", info.UOMCode));
                    para.Add(new MySqlParameter($"@CreateBy{i}", info.CreateBy));
                    para.Add(new MySqlParameter($"@UpdateBy{i}", info.UpdateBy));

                    para.Add(new MySqlParameter($"@CreateUserName{i}", info.CreateUserName));
                    para.Add(new MySqlParameter($"@UpdateUserName{i}", info.UpdateUserName));

                    para.Add(new MySqlParameter($"@ProduceDate{i}", info.ProduceDate));
                    para.Add(new MySqlParameter($"@ExpiryDate{i}", info.ExpiryDate));
                    para.Add(new MySqlParameter($"@ReceivedDate{i}", info.ReceivedDate));

                    i++;
                }
                var strSql1 = strSql.ToString().TrimEnd(',') + " ;";
                base.Context.Database.ExecuteSqlCommand(strSql1, para.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 商品外借修改库存
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>

        public bool UpdateInventoryBySkuBorrow(SkuBorrowDetailDto dto, Guid WareHouseSysId, int AuditingBy, string AuditingName, int status)
        {
            //int actualRange = dto.Qty - dto.ReturnQty;
            StringBuilder sb = new StringBuilder();
            string operateSql = string.Empty;
            if (status == (int)SkuBorrowStatus.Audit)
            {
                operateSql = string.Format(" - {0}", dto.Qty);
            }
            else if (status == (int)SkuBorrowStatus.ReturnAudit)
            {
                operateSql = string.Format(" + {0}", dto.ReturnQty);
            }

            var para = new List<MySqlParameter>();

            //更新库存
            sb.AppendFormat(@"UPDATE invlot i SET   i.Qty = i.Qty {0},
                                                    i.UpdateUserName =@UpdateUserName,
                                                    i.UpdateBy =@UpdateBy ,
                                                    i.UpdateDate =  NOW() 
                                                    WHERE i.WareHouseSysId=@WareHouseSysId 
                                                    AND i.Lot=@Lot 
                                                    AND i.SkuSysId=@SkuSysId ;", operateSql);

            sb.AppendFormat(@"UPDATE invskuloc i SET i.Qty = i.Qty {0},
                                                    i.UpdateUserName = @UpdateUserName,
                                                    i.UpdateBy = @UpdateBy ,
                                                    i.UpdateDate =  NOW() 
                                                    WHERE i.WareHouseSysId=@WareHouseSysId 
                                                    AND i.Loc=@Loc  
                                                    AND i.SkuSysId=@SkuSysId;", operateSql);

            sb.AppendFormat(@"UPDATE invlotloclpn i SET i.Qty = i.Qty {0},
                                                        i.UpdateUserName = @UpdateUserName,
                                                        i.UpdateBy = @UpdateBy,
                                                        i.UpdateDate =  NOW() 
                                                        WHERE i.WareHouseSysId=@WareHouseSysId
                                                        AND i.Loc = @Loc 
                                                        AND i.Lot = @Lot 
                                                        AND i.Lpn = @Lpn 
                                                        AND i.SkuSysId = @SkuSysId ;", operateSql);

            para.Add(new MySqlParameter("@UpdateUserName", AuditingName));
            para.Add(new MySqlParameter("@UpdateBy", AuditingBy));
            para.Add(new MySqlParameter("@WareHouseSysId", WareHouseSysId));
            para.Add(new MySqlParameter("@Lot", dto.Lot));
            para.Add(new MySqlParameter("@Loc", dto.Loc));
            para.Add(new MySqlParameter("@Lpn", dto.Lpn));
            para.Add(new MySqlParameter("@SkuSysId", dto.SkuSysId));


            //更新外借单明细
            sb.AppendFormat(@"UPDATE skuborrowdetail s SET  s.ReturnQty =@ReturnQty ,
                                                            s.BorrowStartTime =@BorrowStartTime ,
                                                            s.BorrowEndTime =@BorrowEndTime ,
                                                            s.Status =@Status ,
                                                            s.IsDamage = @IsDamage ,
                                                            s.UpdateUserName =@UpdateUserName ,
                                                            s.UpdateBy =@UpdateBy ,
                                                            s.UpdateDate =  NOW(),
                                                            s.DamageReason =@DamageReason  
                                                            WHERE s.SysId =@SysId ;");
            para.Add(new MySqlParameter("@ReturnQty", dto.ReturnQty));
            para.Add(new MySqlParameter("@BorrowStartTime", dto.BorrowStartTime));
            para.Add(new MySqlParameter("@BorrowEndTime", dto.BorrowEndTime));
            para.Add(new MySqlParameter("@Status", status));
            para.Add(new MySqlParameter("@IsDamage", dto.IsDamage));
            para.Add(new MySqlParameter("@DamageReason", dto.DamageReason));
            para.Add(new MySqlParameter("@SysId", dto.SysId));

            var count = Context.Database.ExecuteSqlCommand(sb.ToString(), para.ToArray());
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        #region 自增生成单号
        /// <summary>
        /// 自增生成单号
        /// </summary>
        /// <returns></returns>
        public int AutoNextNumber()
        {
            try
            {
                var strSql = "insert INTO autonextnumber values(); SELECT LAST_INSERT_ID();";
                var nextNumber = base.Context.Database.SqlQuery<int>(strSql).FirstOrDefault();
                return nextNumber;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 批量查询invlot
        /// </summary>
        /// <param name="inventorySkus"></param>
        /// <returns></returns>
        public List<invlot> BatchGetInvLot(List<GetInventoryDto> inventorySkus)
        {
            try
            {
                var strSqls = new List<string>();

                var para = new List<MySqlParameter>();
                int i = 0;

                foreach (var info in inventorySkus)
                {
                    strSqls.Add($@"select * from invlot 
                                        where Lot = @Lot{i} 
                                        and SkuSysId =@SkuSysId{i} 
                                        and WareHouseSysId = @WareHouseSysId{i}");

                    para.Add(new MySqlParameter($"@Lot{i}", info.Lot));
                    para.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                    para.Add(new MySqlParameter($"@WareHouseSysId{i}", info.WareHouseSysId));

                    i++;
                }
                string sql = string.Join(" union all ", strSqls);
                return Context.Database.SqlQuery<invlot>(sql, para.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量查询invskuloc
        /// </summary>
        /// <param name="inventorySkus"></param>
        /// <returns></returns>
        public List<invskuloc> BatchGetInvSkuLoc(List<GetInventoryDto> inventorySkus)
        {
            try
            {
                var strSqls = new List<string>();

                var para = new List<MySqlParameter>();
                int i = 0;

                foreach (var info in inventorySkus)
                {
                    strSqls.Add($@"select * from invskuloc 
                                        where Loc =@Loc{i} 
                                        and SkuSysId = @SkuSysId{i}
                                        and WareHouseSysId =@WareHouseSysId{i}");

                    para.Add(new MySqlParameter($"@Loc{i}", info.Loc));
                    para.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                    para.Add(new MySqlParameter($"@WareHouseSysId{i}", info.WareHouseSysId));

                    i++;
                }
                string sql = string.Join(" union all ", strSqls);
                return Context.Database.SqlQuery<invskuloc>(sql, para.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量查询invlotloclpn
        /// </summary>
        /// <param name="inventorySkus"></param>
        /// <returns></returns>
        public List<invlotloclpn> BatchGetInvLotLocLpn(List<GetInventoryDto> inventorySkus)
        {
            try
            {
                var strSqls = new List<string>();
                var para = new List<MySqlParameter>();
                int i = 0;

                foreach (var info in inventorySkus)
                {
                    strSqls.Add($@"select * from invlotloclpn  
                                        where Lot =@Lot{i} 
                                        and Loc =@Loc{i} 
                                        and Lpn = '' 
                                        and SkuSysId = @SkuSysId{i}
                                        and WareHouseSysId =@WareHouseSysId{i} ");

                    para.Add(new MySqlParameter($"@Lot{i}", info.Lot));
                    para.Add(new MySqlParameter($"@Loc{i}", info.Loc));
                    para.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                    para.Add(new MySqlParameter($"@WareHouseSysId{i}", info.WareHouseSysId));

                    i++;
                }
                string sql = string.Join(" union all ", strSqls);
                return Context.Database.SqlQuery<invlotloclpn>(sql, para.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取消上架更新收货明细
        /// </summary>
        /// <param name="updateReceiptDetails"></param>
        public void UpdateReceiptDetailAfterCancelShelves(List<UpdateReceiptDetailDto> updateReceiptDetails, int currentUserId, string currentDisplayName)
        {
            if (updateReceiptDetails != null && updateReceiptDetails.Count > 0)
            {
                StringBuilder strSql = new StringBuilder();

                var para = new List<MySqlParameter>();
                int i = 0;

                foreach (var info in updateReceiptDetails)
                {
                    strSql.Append($@"update receiptdetail set 
                                                ShelvesQty =@ShelvesQty{i}, 
                                                ShelvesStatus =@ShelvesStatus{i}, 
                                                UpdateBy =@UpdateBy , 
                                                UpdateDate = NOW(), 
                                                UpdateUserName =@UpdateUserName , 
                                                TS = @NewTS{i} 
                                                where SysId =@SysId{i}  
                                                and TS = @OldTS{i} ;");

                    para.Add(new MySqlParameter($"@ShelvesQty{i}", info.ShelvesQty));
                    para.Add(new MySqlParameter($"@ShelvesStatus{i}", info.ShelvesStatus));
                    para.Add(new MySqlParameter($"@NewTS{i}", info.NewTS));
                    para.Add(new MySqlParameter($"@SysId{i}", info.SysId));
                    para.Add(new MySqlParameter($"@OldTS{i}", info.OldTS));

                    i++;
                }
                para.Add(new MySqlParameter("@UpdateBy", currentUserId));
                para.Add(new MySqlParameter("@UpdateUserName", currentDisplayName));

                var resultCount = base.Context.Database.ExecuteSqlCommand(strSql.ToString(), para.ToArray());
                if (updateReceiptDetails.Count != resultCount)
                {
                    throw new DbUpdateConcurrencyException();
                }
            }
        }

        /// <summary>
        /// 取消上架批量更新交易表状态
        /// </summary>
        /// <param name="invTransSysIds"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentDisplayName"></param>
        public void UpdateInvTransStatusAfterCancelShelves(IEnumerable<Guid> invTransSysIds, int currentUserId, string currentDisplayName)
        {
            try
            {
                string sysIds = string.Format("'{0}'", string.Join("','", invTransSysIds));
                string sql = $@"update invtrans set 
                                            Status =@Status , 
                                            UpdateBy =@UpdateBy, 
                                            UpdateDate = NOW(),
                                            UpdateUserName =@UpdateUserName 
                                            where SysId in ({sysIds})";

                base.Context.Database.ExecuteSqlCommand(sql,
                    new MySqlParameter($"@Status", InvTransStatus.Cancel),
                    new MySqlParameter($"@UpdateBy", currentUserId),
                    new MySqlParameter($"@UpdateUserName", currentDisplayName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量插入invlot
        /// </summary>
        /// <param name="invLotList"></param>
        public void BatchInsertInvLot(List<invlot> invLotList)
        {
            try
            {
                if (invLotList.Any())
                {
                    var strSql = new StringBuilder();
                    strSql.Append(@"INSERT INTO invlot(SysId, WareHouseSysId, Lot, SkuSysId,
                                        CaseQty, InnerPackQty, Qty, 
                                        AllocatedQty, PickedQty, HoldQty, Status, Price, 
                                        CreateBy, CreateDate, UpdateBy, UpdateDate, 
                                        LotAttr01, LotAttr02, LotAttr03, LotAttr04,  
                                        LotAttr05, LotAttr06, LotAttr07, LotAttr08, LotAttr09, 
                                        ExternalLot, CreateUserName,UpdateUserName, 
                                        ProduceDate, ExpiryDate, ReceiptDate)
                                        VALUES ");

                    var para = new List<MySqlParameter>();
                    int i = 0;

                    foreach (var info in invLotList)
                    {
                        strSql.Append($@"(@SysId{i}, @WareHouseSysId{i}, @Lot{i},@SkuSysId{i},@CaseQty{i}, 
                                          @InnerPackQty{i},@Qty{i}, @AllocatedQty{i},@PickedQty{i},@HoldQty{i}, 
                                          @Status{i},@Price{i},@CreateBy{i}, NOW(),@UpdateBy{i}, NOW(),
                                          @LotAttr01{i}, @LotAttr02{i},@LotAttr03{i},@LotAttr04{i},@LotAttr05{i}, 
                                          @LotAttr06{i}, @LotAttr07{i},@LotAttr08{i},@LotAttr09{i}, 
                                          @ExternalLot{i},@CreateUserName{i},@UpdateUserName{i},@ProduceDate{i},
                                          @ExpiryDate{i},@ReceiptDate{i} ),");

                        para.Add(new MySqlParameter($"@SysId{i}", info.SysId));
                        para.Add(new MySqlParameter($"@WareHouseSysId{i}", info.WareHouseSysId));
                        para.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                        para.Add(new MySqlParameter($"@Lot{i}", info.Lot));
                        para.Add(new MySqlParameter($"@CaseQty{i}", info.CaseQty));
                        para.Add(new MySqlParameter($"@InnerPackQty{i}", info.InnerPackQty));
                        para.Add(new MySqlParameter($"@Qty{i}", info.Qty));
                        para.Add(new MySqlParameter($"@AllocatedQty{i}", info.AllocatedQty));
                        para.Add(new MySqlParameter($"@PickedQty{i}", info.PickedQty));
                        para.Add(new MySqlParameter($"@HoldQty{i}", info.HoldQty));
                        para.Add(new MySqlParameter($"@Status{i}", info.Status));
                        para.Add(new MySqlParameter($"@Price{i}", info.Price));
                        para.Add(new MySqlParameter($"@CreateBy{i}", info.CreateBy));
                        para.Add(new MySqlParameter($"@UpdateBy{i}", info.UpdateBy));

                        para.Add(new MySqlParameter($"@LotAttr01{i}", info.LotAttr01));
                        para.Add(new MySqlParameter($"@LotAttr02{i}", info.LotAttr02));
                        para.Add(new MySqlParameter($"@LotAttr03{i}", info.LotAttr03));
                        para.Add(new MySqlParameter($"@LotAttr04{i}", info.LotAttr04));
                        para.Add(new MySqlParameter($"@LotAttr05{i}", info.LotAttr05));
                        para.Add(new MySqlParameter($"@LotAttr06{i}", info.LotAttr06));
                        para.Add(new MySqlParameter($"@LotAttr07{i}", info.LotAttr07));
                        para.Add(new MySqlParameter($"@LotAttr08{i}", info.LotAttr08));
                        para.Add(new MySqlParameter($"@LotAttr09{i}", info.LotAttr09));
                        para.Add(new MySqlParameter($"@ExternalLot{i}", info.ExternalLot));

                        para.Add(new MySqlParameter($"@CreateUserName{i}", info.CreateUserName));
                        para.Add(new MySqlParameter($"@UpdateUserName{i}", info.UpdateUserName));

                        para.Add(new MySqlParameter($"@ProduceDate{i}", info.ProduceDate));
                        para.Add(new MySqlParameter($"@ExpiryDate{i}", info.ExpiryDate));
                        para.Add(new MySqlParameter($"@ReceiptDate{i}", info.ReceiptDate));

                        i++;

                    }
                    var strSql1 = strSql.ToString().TrimEnd(',') + " ;";
                    base.Context.Database.ExecuteSqlCommand(strSql1, para.ToArray());
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量插入invlotloclpn
        /// </summary>
        /// <param name="invLotLocLpnList"></param>
        public void BatchInsertInvLotLocLpn(List<invlotloclpn> invLotLocLpnList)
        {
            try
            {
                if (invLotLocLpnList.Any())
                {
                    var strSql = new StringBuilder();
                    strSql.Append(@"INSERT INTO invlotloclpn(SysId, WareHouseSysId, 
                                    SkuSysId, Loc, Lot, Lpn, Qty, AllocatedQty, PickedQty, 
                                    Status, CreateBy, CreateDate, UpdateBy, UpdateDate, 
                                    CreateUserName,UpdateUserName)
                                    VALUES ");

                    var para = new List<MySqlParameter>();
                    int i = 0;

                    foreach (var info in invLotLocLpnList)
                    {
                        strSql.Append($@"(@SysId{i},@WareHouseSysId{i},@SkuSysId{i},
                                          @Loc{i},@Lot{i},@Lpn{i},@Qty{i}, 
                                          @AllocatedQty{i},@PickedQty{i},@Status{i},@CreateBy{i}, NOW(),   
                                          @UpdateBy{i}, NOW(),@CreateUserName{i},@UpdateUserName{i}),");

                        para.Add(new MySqlParameter($"@SysId{i}", info.SysId));
                        para.Add(new MySqlParameter($"@WareHouseSysId{i}", info.WareHouseSysId));
                        para.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                        para.Add(new MySqlParameter($"@Loc{i}", info.Loc));
                        para.Add(new MySqlParameter($"@Lot{i}", info.Lot));
                        para.Add(new MySqlParameter($"@Lpn{i}", info.Lpn));
                        para.Add(new MySqlParameter($"@Qty{i}", info.Qty));
                        para.Add(new MySqlParameter($"@AllocatedQty{i}", info.AllocatedQty));
                        para.Add(new MySqlParameter($"@PickedQty{i}", info.PickedQty));
                        para.Add(new MySqlParameter($"@Status{i}", info.Status));
                        para.Add(new MySqlParameter($"@CreateBy{i}", info.CreateBy));
                        para.Add(new MySqlParameter($"@UpdateBy{i}", info.UpdateBy));
                        para.Add(new MySqlParameter($"@CreateUserName{i}", info.CreateUserName));
                        para.Add(new MySqlParameter($"@UpdateUserName{i}", info.UpdateUserName));

                        i++;
                    }
                    var strSql1 = strSql.ToString().TrimEnd(',') + " ;";

                    base.Context.Database.ExecuteSqlCommand(strSql1, para.ToArray());
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量插入invskuloc
        /// </summary>
        /// <param name="invSkuLocList"></param>
        public void BatchInsertInvSkuLoc(List<invskuloc> invSkuLocList)
        {
            try
            {
                if (invSkuLocList.Any())
                {
                    var strSql = new StringBuilder();
                    strSql.Append(@"INSERT INTO invskuloc(SysId, WareHouseSysId, SkuSysId,
                                    Loc, Qty, AllocatedQty, PickedQty, 
                                    CreateBy, CreateDate, UpdateBy, UpdateDate,
                                    CreateUserName,UpdateUserName)
                                    VALUES ");

                    var para = new List<MySqlParameter>();
                    int i = 0;

                    foreach (var info in invSkuLocList)
                    {
                        strSql.Append($@"(@SysId{i}, @WareHouseSysId{i}, @SkuSysId{i}, @Loc{i} ,@Qty{i} , 
                                          @AllocatedQty{i} ,@PickedQty{i},@CreateBy{i}, 
                                          NOW(),@UpdateBy{i}, NOW(), @CreateUserName{i},@UpdateUserName{i}),");

                        para.Add(new MySqlParameter($"@SysId{i}", info.SysId));
                        para.Add(new MySqlParameter($"@WareHouseSysId{i}", info.WareHouseSysId));
                        para.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                        para.Add(new MySqlParameter($"@Loc{i}", info.Loc));
                        para.Add(new MySqlParameter($"@Qty{i}", info.Qty));
                        para.Add(new MySqlParameter($"@AllocatedQty{i}", info.AllocatedQty));
                        para.Add(new MySqlParameter($"@PickedQty{i}", info.PickedQty));
                        para.Add(new MySqlParameter($"@CreateBy{i}", info.CreateBy));
                        para.Add(new MySqlParameter($"@UpdateBy{i}", info.UpdateBy));
                        para.Add(new MySqlParameter($"@CreateUserName{i}", info.CreateUserName));
                        para.Add(new MySqlParameter($"@UpdateUserName{i}", info.UpdateUserName));

                        i++;
                    }
                    var strSql1 = strSql.ToString().TrimEnd(',') + ";";

                    base.Context.Database.ExecuteSqlCommand(strSql1, para.ToArray());
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量插入领料分拣记录
        /// </summary>
        /// <param name="pickingRecordsList"></param>
        public void BatchInsertPickingRecords(List<pickingrecords> pickingRecordsList)
        {
            try
            {
                if (pickingRecordsList.Any())
                {
                    var strSql = new StringBuilder();
                    strSql.Append(@"INSERT INTO pickingrecords(SysId, PickingSysId,
                                WareHouseSysId, ReceiptSysId, ReceiptOrder,
                                SkuSysId, PickingNumber, Qty, PickingUserName,
                                PickingDate, Remark, CreateBy, CreateDate, 
                                CreateUserName, UpdateBy, UpdateDate, UpdateUserName)
                                VALUES ");

                    var para = new List<MySqlParameter>();
                    int i = 0;
                    foreach (var info in pickingRecordsList)
                    {
                        strSql.Append($@"(@SysId{i} ,@PickingSysId{i},@WareHouseSysId{i}, 
                                        @ReceiptSysId{i} ,@ReceiptOrder{i} , @SkuSysId{i} ,
                                        @PickingNumber{i} ,@Qty{i},@PickingUserName{i} ,
                                        NOW(),@Remark{i} ,@CreateBy{i} ,  NOW(),
                                        @CreateUserName{i} ,@UpdateBy{i} , NOW(),@UpdateUserName{i}),");

                        para.Add(new MySqlParameter($"@SysId{i}", info.SysId));
                        para.Add(new MySqlParameter($"@PickingSysId{i}", info.PickingSysId));
                        para.Add(new MySqlParameter($"@WareHouseSysId{i}", info.WareHouseSysId));
                        para.Add(new MySqlParameter($"@ReceiptSysId{i}", info.ReceiptSysId));
                        para.Add(new MySqlParameter($"@ReceiptOrder{i}", info.ReceiptOrder));
                        para.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                        para.Add(new MySqlParameter($"@PickingNumber{i}", info.PickingNumber));
                        para.Add(new MySqlParameter($"@Qty{i}", info.Qty));
                        para.Add(new MySqlParameter($"@PickingUserName{i}", info.PickingUserName));
                        para.Add(new MySqlParameter($"@Remark{i}", info.Remark));
                        para.Add(new MySqlParameter($"@CreateBy{i}", info.CreateBy));

                        para.Add(new MySqlParameter($"@CreateUserName{i}", info.CreateUserName));
                        para.Add(new MySqlParameter($"@UpdateBy{i}", info.UpdateBy));
                        para.Add(new MySqlParameter($"@UpdateUserName{i}", info.UpdateUserName));
                        i++;
                    }
                    var strSql1 = strSql.ToString().TrimEnd(',') + " ;";

                    base.Context.Database.ExecuteSqlCommand(strSql1, para.ToArray());
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 批量插入receiptdatarecord
        /// </summary>
        /// <param name="invLotLocLpnList"></param>
        public void BatchInsertReceiptDataRecordList(List<receiptdatarecord> receiptDataRecordList)
        {
            try
            {
                if (receiptDataRecordList.Any())
                {
                    var strSql = new StringBuilder();
                    strSql.Append(@"INSERT INTO receiptdatarecord
                                    (
                                      SysId
                                     ,WareHouseSysId
                                     ,ReceiptSysId
                                     ,ReceiptOrder
                                     ,SkuSysId
                                     ,Qty
                                     ,GiftQty
                                     ,RejectedQty
                                     ,AdjustmentQty
                                     ,GiftRejectedQty
                                     ,Remark
                                     ,CreateBy
                                     ,CreateDate
                                     ,CreateUserName
                                     ,UpdateBy
                                     ,UpdateDate
                                     ,UpdateUserName
                                    )
                                    VALUES ");

                    var para = new List<MySqlParameter>();
                    int i = 0;
                    foreach (var info in receiptDataRecordList)
                    {
                        strSql.Append($@"(@SysId{i},@WareHouseSysId{i} ,@ReceiptSysId{i} , 
                                           @ReceiptOrder{i} ,@SkuSysId{i} ,@Qty{i} , 
                                           @GiftQty{i},@RejectedQty{i} ,@AdjustmentQty{i},    
                                           @GiftRejectedQty{i} ,@Remark{i} ,@CreateBy{i} , 
                                           @CreateDate{i} ,@CreateUserName{i} , 
                                           @UpdateBy{i} ,@UpdateDate{i} ,@UpdateUserName{i}),");

                        para.Add(new MySqlParameter($"@SysId{i}", info.SysId));
                        para.Add(new MySqlParameter($"@WareHouseSysId{i}", info.WareHouseSysId));
                        para.Add(new MySqlParameter($"@ReceiptSysId{i}", info.ReceiptSysId));
                        para.Add(new MySqlParameter($"@ReceiptOrder{i}", info.ReceiptOrder));
                        para.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                        para.Add(new MySqlParameter($"@Qty{i}", info.Qty));
                        para.Add(new MySqlParameter($"@GiftQty{i}", info.GiftQty));
                        para.Add(new MySqlParameter($"@RejectedQty{i}", info.RejectedQty));
                        para.Add(new MySqlParameter($"@AdjustmentQty{i}", info.AdjustmentQty));
                        para.Add(new MySqlParameter($"@GiftRejectedQty{i}", info.GiftRejectedQty));
                        para.Add(new MySqlParameter($"@Remark{i}", info.Remark));
                        para.Add(new MySqlParameter($"@CreateBy{i}", info.CreateBy));
                        para.Add(new MySqlParameter($"@CreateDate{i}", info.CreateDate));
                        para.Add(new MySqlParameter($"@CreateUserName{i}", info.CreateUserName));
                        para.Add(new MySqlParameter($"@UpdateBy{i}", info.UpdateBy));
                        para.Add(new MySqlParameter($"@UpdateDate{i}", info.UpdateDate));
                        para.Add(new MySqlParameter($"@UpdateUserName{i}", info.UpdateUserName));

                        i++;
                    }
                    var strSql1 = strSql.ToString().TrimEnd(',') + " ;";
                    base.Context.Database.ExecuteSqlCommand(strSql1, para.ToArray());
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取商品库位库存数量
        /// </summary>
        /// <param name="stockTakeDetails"></param>
        /// <returns></returns>
        public List<InvSkuLocDto> GetInvQtyBySkuAndLoc(List<stocktakedetail> stockTakeDetails, Guid warehouseSysId)
        {
            try
            {
                var strSql = new StringBuilder();
                strSql.Append("select std.SkuSysId, std.Loc, i.Qty from ( ");

                var subSqls = new List<string>();
                var para = new List<MySqlParameter>();
                int i = 0;

                foreach (var info in stockTakeDetails)
                {
                    var subSql = new StringBuilder();
                    subSql.Append($"select @SkuSysId{i} as SkuSysId , @Loc{i} as Loc");
                    subSqls.Add(subSql.ToString());

                    para.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                    para.Add(new MySqlParameter($"@Loc{i}", info.Loc));

                    i++;
                }
                strSql.Append(string.Join(" union all ", subSqls));
                strSql.Append(@" ) std
                            inner join invskuloc i
                            ON std.SkuSysId = i.SkuSysId
                            AND std.Loc = i.Loc
                            WHERE i.WareHouseSysId =@WareHouseSysId ");
                para.Add(new MySqlParameter("@WareHouseSysId", warehouseSysId));

                return Context.Database.SqlQuery<InvSkuLocDto>(strSql.ToString(), para.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改散货装箱的出库单号（绑定）
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="outboundOrder"></param>
        /// <param name="prePackSysId"></param>
        /// <returns></returns>
        public CommonResponse UpdatePreBulkPackOutboundByBind(Guid outboundSysId, string outboundOrder, Guid prePackSysId)
        {
            var sql = @"UPDATE prebulkpack SET OutboundSysId = @OutboundSysId, 
                                OutboundOrder = @OutboundOrder WHERE SysId in (
                                    SELECT PreBulkPackSysId FROM 
                                    prepackrelation WHERE PrePackSysId = @PrePackSysId);";

            var result = base.Context.Database.ExecuteSqlCommand(sql
                , new MySqlParameter("@OutboundSysId", outboundSysId)
                , new MySqlParameter("@OutboundOrder", outboundOrder)
                , new MySqlParameter("@PrePackSysId", prePackSysId));

            return new CommonResponse();
        }

        /// <summary>
        /// 修改散货装箱的出库单号（解绑）
        /// </summary>
        /// <param name="prePackSysId"></param>
        /// <returns></returns>
        public CommonResponse UpdatePreBulkPackOutboundByUnBind(Guid prePackSysId)
        {
            var sql = @"UPDATE prebulkpack SET OutboundSysId = null, 
                                            OutboundOrder = null WHERE SysId in (
                                                SELECT PreBulkPackSysId FROM 
                                                prepackrelation WHERE PrePackSysId = @PrePackSysId);";

            var result = base.Context.Database.ExecuteSqlCommand(sql
                , new MySqlParameter("@PrePackSysId", prePackSysId));

            return new CommonResponse();
        }

        /// <summary>
        /// 损益审核更新库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        public CommonResponse UpdateInventoryAdjustmentAudit(List<UpdateInventoryDto> updateInventoryList)
        {
            var updateId = updateInventoryList.FirstOrDefault().CurrentUserId;
            var updateName = updateInventoryList.FirstOrDefault().CurrentDisplayName;
            var warehouseSysId = updateInventoryList.FirstOrDefault().WarehouseSysId;
            try
            {
                var strSqlInvLocLotLpn = new StringBuilder();
                var invLocLotLpnList = updateInventoryList.Where(p => p.InvLotLocLpnSysId.HasValue).GroupBy(p => p.InvLotLocLpnSysId.Value).Select(p => new UpdateInventoryDto
                {
                    InvLotLocLpnSysId = p.Key,
                    InvLotSysId = null,
                    InvSkuLocSysId = null,
                    Qty = p.Sum(x => x.Qty),
                    CurrentUserId = updateId,
                    CurrentDisplayName = updateName,
                    WarehouseSysId = warehouseSysId
                }).ToList();

                var para = new List<MySqlParameter>();
                int i = 0;
                foreach (var info in invLocLotLpnList)
                {
                    strSqlInvLocLotLpn.AppendFormat($@" UPDATE invlotloclpn SET  
                                        invlotloclpn.Qty = invlotloclpn.Qty + (@invQty{i})  ,
                                        updateBy =@UpdateBy ,
                                        UpdateDate = now(),
                                        UpdateUserName =@UpdateUserName 
                                        where invlotloclpn.sysId=@sysId{i} 
                                        and invlotloclpn.Qty >= 0 AND invlotloclpn.Qty + (@invQty{i}) >= 0 ; ");

                    para.Add(new MySqlParameter($"@sysId{i}", info.InvLotLocLpnSysId));
                    para.Add(new MySqlParameter($"@invQty{i}", info.Qty));
                    i++;
                }
                para.Add(new MySqlParameter("@UpdateBy", updateId));
                para.Add(new MySqlParameter("@UpdateUserName", updateName));

                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString(), para.ToArray());

                if (invLocLotLpnList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("库存不足,无法审核完成");
                }

                var strSqlInvLot = new StringBuilder();
                var invLotList = updateInventoryList.Where(p => p.InvLotSysId.HasValue).GroupBy(p => p.InvLotSysId.Value).Select(p => new UpdateInventoryDto
                {
                    InvLotLocLpnSysId = null,
                    InvLotSysId = p.Key,
                    InvSkuLocSysId = null,
                    Qty = p.Sum(x => x.Qty),
                    CurrentUserId = updateId,
                    CurrentDisplayName = updateName,
                    WarehouseSysId = warehouseSysId
                }).ToList();


                var invPara = new List<MySqlParameter>();
                int j = 0;
                foreach (var info in invLotList)
                {
                    strSqlInvLot.AppendFormat($@" UPDATE invlot SET 
                                invlot.Qty = invlot.Qty + (@invQty{j}),
                                updateBy =@UpdateBy ,
                                UpdateDate = now(),
                                UpdateUserName =@UpdateUserName  
                                where invlot.sysId=@sysId{j} 
                                and invlot.Qty >= 0 AND invlot.Qty + (@invQty{j}) >= 0 ; ");

                    invPara.Add(new MySqlParameter($"@invQty{j}", info.Qty));
                    invPara.Add(new MySqlParameter($"@sysId{j}", info.InvLotSysId));
                    j++;
                }
                invPara.Add(new MySqlParameter("@UpdateBy", updateId));
                invPara.Add(new MySqlParameter("@UpdateUserName", updateName));

                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString(), invPara.ToArray());
                if (invLotList.Count() != invLotResult)
                {
                    throw new Exception("库存不足,无法审核完成");
                }

                var strSqlSkuLoc = new StringBuilder();
                var invLocList = updateInventoryList.Where(p => p.InvSkuLocSysId.HasValue).GroupBy(p => p.InvSkuLocSysId.Value).Select(p => new UpdateInventoryDto
                {
                    InvLotLocLpnSysId = null,
                    InvLotSysId = null,
                    InvSkuLocSysId = p.Key,
                    Qty = p.Sum(x => x.Qty),
                    CurrentUserId = updateId,
                    CurrentDisplayName = updateName,
                    WarehouseSysId = warehouseSysId
                }).ToList();


                var skuPara = new List<MySqlParameter>();
                int k = 0;
                foreach (var info in invLocList)
                {
                    strSqlSkuLoc.AppendFormat($@" UPDATE invSkuLoc SET  
                                        invSkuLoc.Qty = invSkuLoc.Qty + (@invSkuQty{k})  ,
                                        updateBy =@UpdateBy,
                                        UpdateDate = now(),
                                        UpdateUserName =@UpdateUserName 
                                        where invSkuLoc.sysId=@sysId{k}  and   
                                        invSkuLoc.Qty >= 0 AND invSkuLoc.Qty + (@invSkuQty{k}) >= 0;");

                    skuPara.Add(new MySqlParameter($"@invSkuQty{k}", info.Qty));
                    skuPara.Add(new MySqlParameter($"@sysId{k}", info.InvSkuLocSysId));

                    k++;
                }
                skuPara.Add(new MySqlParameter("@UpdateBy", updateId));
                skuPara.Add(new MySqlParameter("@UpdateUserName", updateName));

                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString(), skuPara.ToArray());
                if (invLocList.Count() != invSkuLocResult)
                {
                    throw new Exception("库存不足,无法审核完成");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new CommonResponse();
        }

        #region 工单相关
        /// <summary>
        /// 修改工单状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CommonResponse UpdateWorkByDocOrder(MQWorkDto request)
        {
            try
            {
                var strDocSysId = StringHelper.GuidListToIds(request.CancelWorkDto.DocSysIds);
                var strStatus = string.Format("'{0}','{1}'", (int)WorkStatus.Hang, (int)WorkStatus.Working);
                var strSql = string.Empty;

                var mySqlParameter = new List<MySqlParameter>();

                if (request.CancelWorkDto.Status == (int)WorkStatus.Finish)
                {
                    strSql = string.Format(@" 
                                            update work set 
                                            Status = @Status, 
                                            EndTime = now(), 
                                            UpdateBy =@UpdateBy, 
                                            UpdateDate = now(), 
                                            UpdateUserName =@UpdateUserName  
                                            where DocSysId in ({0}) and status in ({1}) 
                                            and WorkType =@WorkType ;", strDocSysId, strStatus
                                            );
                }
                else
                {
                    strSql = string.Format(@" 
                                            update work set 
                                            Status =@Status , 
                                            UpdateBy =@UpdateBy , 
                                            UpdateDate = now(), 
                                            UpdateUserName =@UpdateUserName  
                                            where DocSysId in ({0}) and status in ({1}) 
                                            and WorkType = @WorkType ;", strDocSysId, strStatus);
                }

                mySqlParameter.Add(new MySqlParameter("@Status", request.CancelWorkDto.Status));
                mySqlParameter.Add(new MySqlParameter("@UpdateBy", request.CurrentUserId));
                mySqlParameter.Add(new MySqlParameter("@UpdateUserName", request.CurrentDisplayName));
                mySqlParameter.Add(new MySqlParameter("@WorkType", request.WorkType));

                var result = base.Context.Database.ExecuteSqlCommand(strSql, mySqlParameter.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new CommonResponse();
        }

        /// <summary>
        /// 修改出库单指派人
        /// </summary>
        /// <returns></returns>
        public CommonResponse UpdateOutboundWorkName(List<WorkDetailDto> workDetailList)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                var mySqlParameter = new List<MySqlParameter>();
                int i = 1;
                foreach (var info in workDetailList)
                {
                    strSql.AppendFormat($@"
                                        update outbound set 
                                        AppointUserNames =@AppointUserName{i}, 
                                        UpdateBy =@UpdateBy{i}, 
                                        UpdateDate = now(), 
                                        UpdateUserName =@CurrentDisplayName{i} 
                                        where sysid =@sysid{i};");
                    mySqlParameter.Add(new MySqlParameter("@AppointUserName" + i, info.AppointUserName));
                    mySqlParameter.Add(new MySqlParameter("@UpdateBy" + i, info.CurrentUserId));
                    mySqlParameter.Add(new MySqlParameter("@CurrentDisplayName" + i, info.CurrentDisplayName));
                    mySqlParameter.Add(new MySqlParameter("@sysid" + i, info.DocSysId));
                    i++;
                }
                var result = base.Context.Database.ExecuteSqlCommand(strSql.ToString(), mySqlParameter.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new CommonResponse();
        }

        /// <summary>
        /// 修改收货单指派人
        /// </summary>
        /// <returns></returns>
        public CommonResponse UpdateReceiptWorkName(List<WorkDetailDto> workDetailList)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                var mySqlParameter = new List<MySqlParameter>();
                int i = 1;
                foreach (var info in workDetailList)
                {
                    strSql.AppendFormat($@" update receipt set AppointUserNames =@AppointUserName{i}, 
                                            UpdateBy =@UpdateBy{i} , 
                                            UpdateDate = now(), 
                                            UpdateUserName = @CurrentDisplayName{i} 
                                            where sysid =@sysid{i} ;");
                    mySqlParameter.Add(new MySqlParameter("@UpdateBy" + i, info.CurrentUserId));
                    mySqlParameter.Add(new MySqlParameter("@AppointUserName" + i, info.AppointUserName));
                    mySqlParameter.Add(new MySqlParameter("@CurrentDisplayName" + i, info.CurrentDisplayName));
                    mySqlParameter.Add(new MySqlParameter("@sysid" + i, info.DocSysId));
                    i++;
                }
                var result = base.Context.Database.ExecuteSqlCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new CommonResponse();
        }
        #endregion

        /// <summary>
        /// RF容器拣货更新PickDetail拣货数量
        /// </summary>
        /// <param name="updatePickDetailList"></param>
        /// <returns></returns>
        public CommonResponse UpdatePickDetailRFContainerPicking(List<UpdatePickDetailDto> updatePickDetailList)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                var mySqlParameter = new List<MySqlParameter>();
                int i = 1;
                foreach (var item in updatePickDetailList)
                {
                    sql.AppendFormat($@"
                                        UPDATE pickdetail p SET p.PickDate = NOW(), 
                                        p.PickedQty = p.PickedQty + @PickedQty{i} , 
                                        p.UpdateBy =@CurrentUserId{i} , 
                                        p.UpdateDate = NOW(),
                                        p.UpdateUserName = @CurrentDisplayName{i}
                                        WHERE p.SysId =@SysId{i} 
                                        AND p.PickedQty + @PickedQty{i}  <= p.Qty;"
                                    );
                    mySqlParameter.Add(new MySqlParameter("@CurrentDisplayName" + i, item.CurrentDisplayName));
                    mySqlParameter.Add(new MySqlParameter("@SysId" + i, item.SysId));
                    mySqlParameter.Add(new MySqlParameter("@PickedQty" + i, item.PickedQty));
                    mySqlParameter.Add(new MySqlParameter("@CurrentUserId" + i, item.CurrentUserId));
                    i++;
                }
                var resultCount = base.Context.Database.ExecuteSqlCommand(sql.ToString(), mySqlParameter.ToArray());
                if (resultCount != updatePickDetailList.Count)
                {
                    throw new Exception("商品在该库位可拣货数量不足，无法拣货");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new CommonResponse();
        }

        /// <summary>
        /// 还原拣货容器
        /// </summary>
        /// <param name="clearContainerDto"></param>
        /// <returns></returns>
        public CommonResponse ClearContainer(ClearContainerDto clearContainerDto)
        {
            try
            {
                if (clearContainerDto.ContainerSysIds.Count == 0)
                {
                    return new CommonResponse();
                }
                StringBuilder sql = new StringBuilder();
                var mySqlParameter = new List<MySqlParameter>();
                int i = 1;
                foreach (var containerSysId in clearContainerDto.ContainerSysIds)
                {
                    sql.Append($@"UPDATE prebulkpack p
                          SET p.Status = {(int)PreBulkPackStatus.New},
                          p.OutboundOrder = NULL,
                          p.OutboundSysId = NULL,
                          p.UpdateBy = @CurrentUserId{i},
                          p.UpdateDate = NOW(),
                          p.UpdateUserName = @CurrentDisplayName{i}
                          WHERE p.SysId =@containerSysId{i}  AND p.WarehouseSysId =@WarehouseSysId{i};

                          DELETE FROM prebulkpackdetail WHERE PreBulkPackSysId =@PreBulkPackSysId{i}; ");

                    mySqlParameter.Add(new MySqlParameter("@CurrentUserId" + i, clearContainerDto.CurrentUserId));
                    mySqlParameter.Add(new MySqlParameter("@containerSysId" + i, containerSysId));
                    mySqlParameter.Add(new MySqlParameter("@WarehouseSysId" + i, clearContainerDto.WarehouseSysId));
                    mySqlParameter.Add(new MySqlParameter("@PreBulkPackSysId" + i, containerSysId));
                    mySqlParameter.Add(new MySqlParameter("@CurrentDisplayName" + i, clearContainerDto.CurrentDisplayName));
                    i++;
                }
                base.Context.Database.ExecuteSqlCommand(sql.ToString(), mySqlParameter.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new CommonResponse();
        }

        /// <summary>
        /// 更新出库明细备注
        /// </summary>
        /// <param name="partShipmentMemoDto"></param>
        /// <returns></returns>
        public CommonResponse UpdateOutboundDetailMemo(PartShipmentMemoDto partShipmentMemoDto)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                var mySqlParameter = new List<MySqlParameter>();
                int i = 1;
                foreach (var item in partShipmentMemoDto.PartShipmentDetailMemoList)
                {
                    sql.Append($@"UPDATE outbounddetail o SET o.Memo = @Memo{i}, o.UpdateBy = @UpdateBy{i}, o.UpdateDate = NOW(), o.UpdateUserName = @UpdateUserName{i} WHERE o.SysId = @SysId{i};");
                    mySqlParameter.Add(new MySqlParameter("@Memo" + i, item.Memo));
                    mySqlParameter.Add(new MySqlParameter("@UpdateBy" + i, partShipmentMemoDto.CurrentUserId));
                    mySqlParameter.Add(new MySqlParameter("@UpdateUserName" + i, partShipmentMemoDto.CurrentDisplayName));
                    mySqlParameter.Add(new MySqlParameter("@SysId" + i, item.SysId));
                    i++;
                }
                base.Context.Database.ExecuteSqlCommand(sql.ToString(), mySqlParameter.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new CommonResponse();
        }

        /// <summary>
        /// 更新拣货单拣货数量
        /// </summary>
        /// <param name="pickingOperationDto"></param>
        /// <returns></returns>
        public CommonResponse UpdatePickDetailPickedQty(PickingOperationDto pickingOperationDto)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                var mySqlParameter = new List<MySqlParameter>();
                int i = 1;
                foreach (var item in pickingOperationDto.PickingOperationDetails)
                {
                    sql.Append($@"UPDATE pickdetail p SET p.PickedQty = @PickedQty{i}, p.PickDate = NOW(), p.UpdateBy = @UpdateBy{i}, p.UpdateDate = NOW(), p.UpdateUserName = @UpdateUserName{i} WHERE p.SysId = @SysId{i};");
                    mySqlParameter.Add(new MySqlParameter("@PickedQty" + i, item.PickedQty));
                    mySqlParameter.Add(new MySqlParameter("@UpdateBy" + i, pickingOperationDto.CurrentUserId));
                    mySqlParameter.Add(new MySqlParameter("@UpdateUserName" + i, pickingOperationDto.CurrentDisplayName));
                    mySqlParameter.Add(new MySqlParameter("@SysId" + i, item.SysId));
                    i++;
                }
                base.Context.Database.ExecuteSqlCommand(sql.ToString(), mySqlParameter.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new CommonResponse();
        }
    }
}