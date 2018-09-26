using Abp.Domain.Uow;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace NBK.ECService.WMS.Application
{
    public class WorkMangerAppService : IWorkMangerAppService
    {
        private ICrudRepository _crudRepository = null;
        private IWorkMangerRepository _workMangerRepositoryRepository = null;
        private IBaseAppService _baseAppService = null;
        private IWMSSqlRepository _WMSSqlRepository = null;

        public WorkMangerAppService(ICrudRepository crudRepository, IWorkMangerRepository workMangerRepositoryRepository, IBaseAppService baseAppService, IWMSSqlRepository wmsSqlRepository)
        {
            this._crudRepository = crudRepository;
            this._workMangerRepositoryRepository = workMangerRepositoryRepository;
            this._baseAppService = baseAppService;
            this._WMSSqlRepository = wmsSqlRepository;
        }

        /// <summary>
        /// 分页获取工单list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<WorkListDto> GetWorkByPage(WorkQueryDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            return _workMangerRepositoryRepository.GetWorkByPage(request);
        }


        /// <summary>
        /// 根据ID获取工单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public WorkDetailDto GetWorkBySysId(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var reuslt = new WorkDetailDto();
            try
            {
                var info = _crudRepository.FirstOrDefault<work>(x => x.SysId == sysId);
                if (info == null)
                {
                    throw new Exception("工单不存在");
                }
                reuslt = info.JTransformTo<WorkDetailDto>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reuslt;
        }

        /// <summary>
        /// 更新工单
        /// </summary>
        /// <param name="request"></param>
        public void UpdateWorkInfo(WorkUpdateDto request)
        {
            try
            {
                _crudRepository.ChangeDB(request.WarehouseSysId);
                var info = _crudRepository.FirstOrDefault<work>(x => x.SysId == request.SysId);
                if (info == null)
                {
                    throw new Exception("工单不存在");
                }
                if (info.Status != (int)WorkStatus.Hang && info.Status != (int)WorkStatus.Working)
                {
                    throw new Exception("只有挂起或进行中的工单可编辑");
                }
                info.AppointUserId = request.AppointUserId;
                info.AppointUserName = request.AppointUserName;
                info.UpdateBy = request.CurrentUserId;
                info.UpdateDate = DateTime.Now;
                info.UpdateUserName = request.CurrentDisplayName;
                _crudRepository.Update<work>(info);

                #region 更新成功之后，更新与之对应的订单
                UpdateFromTableUser(info);
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("更新失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 作废工单
        /// </summary>
        /// <param name="request"></param>
        public void CancelWork(CancelWorkDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var workList = _crudRepository.GetQuery<work>(p => request.SysIds.Contains(p.SysId)).ToList();
            if (workList.Any(p => p.Status != (int)WorkStatus.Hang && p.Status != (int)WorkStatus.Working))
            {
                throw new Exception(string.Format("以下工单必须为挂起或者进行中状态：\r\n{0}", string.Join("\r\n", workList.Where(p => p.Status != (int)WorkStatus.Hang && p.Status != (int)WorkStatus.Working).Select(p => p.WorkOrder))));
            }
            workList.ForEach(p =>
            {
                _crudRepository.Update<work>(p.SysId, x => x.Status = (int)WorkStatus.Void);
            });
        }

        /// <summary>
        /// 根据来源单据号修改工单状态
        /// </summary>
        /// <param name="request"></param>
        public CommonResponse UpdateWorkByDocOrder(MQWorkDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            CommonResponse rsp = new CommonResponse { IsSuccess = true };
            try
            {
                if (request != null && request.CancelWorkDto != null)
                {
                    _WMSSqlRepository.UpdateWorkByDocOrder(request);
                }
                else
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "传入的参数为空";
                }
            }
            catch (Exception ex)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
            }
            return rsp;
        }

        /// <summary>
        /// 增加工单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public CommonResponse MQAddWork(MQWorkDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            CommonResponse rsp = new CommonResponse { IsSuccess = true };
            try
            {
                if (request != null && request.WorkDetailDtoList != null)
                {
                    var workUserList = _crudRepository.GetQuery<workuser>(x => x.WarehouseSysId == request.WarehouseSysId && x.WorkType == request.WorkType && x.IsActive == true).OrderBy(x => x.CreateDate).ToList();
                    if (workUserList == null || workUserList.Count == 0)
                    {
                        rsp.IsSuccess = false;
                        rsp.ErrorMessage = "未找到对应操作类型的可用操作人信息";
                        return rsp;
                    }
                    workuser workUser = null;
                    workuser newWorkUser = null;

                    #region 指派单据作业人
                    foreach (var info in request.WorkDetailDtoList)
                    {
                        workUser = workUserList.FirstOrDefault(x => x.WorkType == info.WorkType && x.IsAssigned == true);
                        if (workUser == null)
                        {
                            workUser = workUserList.FirstOrDefault();
                            workUser.IsAssigned = true;
                            info.AppointUserId = workUser.SysId;
                            info.AppointUserName = workUser.WorkUserName;
                        }
                        else
                        {
                            workUser.IsAssigned = false;

                            newWorkUser = workUserList.FirstOrDefault(x => x.WorkType == info.WorkType && x.CreateDate > workUser.CreateDate);
                            if (newWorkUser == null)
                            {
                                newWorkUser = workUserList.FirstOrDefault(x => x.WorkType == info.WorkType);
                            }
                            newWorkUser.IsAssigned = true;

                            info.AppointUserId = newWorkUser.SysId;
                            info.AppointUserName = newWorkUser.WorkUserName;
                        }
                    }
                    #endregion

                    var workList = GetWorkList(request.WorkDetailDtoList);

                    #region 事务
                    TransactionOptions transactionOption = new TransactionOptions();
                    transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                    {
                        if (workUser != null)
                        {
                            _crudRepository.Update(workUser);
                        }
                        if (newWorkUser != null)
                        {
                            _crudRepository.Update(newWorkUser);
                        }
                        _crudRepository.BatchInsert(workList);

                        if (request.WorkType == (int)UserWorkType.Receipt || request.WorkType == (int)UserWorkType.Shelve)
                        {
                            _WMSSqlRepository.UpdateReceiptWorkName(request.WorkDetailDtoList);
                        }
                        else if (request.WorkType == (int)UserWorkType.Picking)
                        {
                            _WMSSqlRepository.UpdateOutboundWorkName(request.WorkDetailDtoList);
                        }

                        _crudRepository.SaveChange();
                        scope.Complete();
                    }
                    #endregion
                }
                else
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "传入的参数为空";
                }
            }
            catch (Exception ex)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
            }
            return rsp;
        }

        [UnitOfWork(isTransactional: false)]
        public List<work> GetWorkList(List<WorkDetailDto> dtoList)
        {
            var workList = new List<work>();

            if (dtoList != null && dtoList.Count > 0)
            {
                var workOrders = _baseAppService.GetNumber(PublicConst.GenNextNumberWork, dtoList.Count);
                var i = 0;
                dtoList.ForEach(p =>
                {
                    var model = new work();
                    model.SysId = Guid.NewGuid();
                    model.WorkOrder = workOrders[i];
                    model.Status = p.Status;
                    model.WorkType = p.WorkType;
                    model.Priority = p.Priority;
                    model.AppointUserId = p.AppointUserId;
                    model.AppointUserName = p.AppointUserName;
                    model.StartTime = p.StartTime;
                    model.WorkTime = p.WorkTime;
                    model.Descr = p.Descr;
                    model.Source = p.Source;
                    model.DocSysId = p.DocSysId;
                    model.DocOrder = p.DocOrder;
                    model.DocDetailSysId = p.DocDetailSysId;
                    model.SkuSysId = p.SkuSysId;
                    model.Lot = p.Lot;
                    model.Lpn = p.Lpn;
                    model.FromLoc = p.FromLoc;
                    model.ToLoc = p.ToLoc;
                    model.FromQty = p.FromQty;
                    model.ToQty = p.ToQty;
                    model.WarehouseSysId = p.WarehouseSysId;
                    model.CreateBy = model.UpdateBy = p.CurrentUserId;
                    model.CreateDate = model.UpdateDate = DateTime.Now;
                    model.CreateUserName = model.UpdateUserName = p.CurrentDisplayName;
                    i++;
                    workList.Add(model);
                });
            }
            return workList;
        }

        /// <summary>
        /// 新增工单
        /// </summary>
        public void AddWork(List<WorkDetailDto> dtoList)
        {
            var workList = GetWorkList(dtoList);
            _crudRepository.BatchInsert(workList);
        }

        /// <summary>
        /// 更新拣货单表时，更新来源表指定人
        /// </summary>
        private void UpdateFromTableUser(work info)
        {
            if (info.WorkType == (int)UserWorkType.Receipt)
            {
                var model = _crudRepository.GetQuery<receipt>(x => x.SysId == info.DocSysId).FirstOrDefault();
                if (model != null)
                {
                    model.AppointUserNames = info.AppointUserName;
                    _crudRepository.Update<receipt>(model);
                }
            }
            if (info.WorkType == (int)UserWorkType.Shelve)
            {
                var model = _crudRepository.GetQuery<receipt>(x => x.SysId == info.DocSysId).FirstOrDefault();
                if (model != null)
                {
                    model.AppointUserNames = info.AppointUserName;
                    _crudRepository.Update<receipt>(model);
                }
            }
            if (info.WorkType == (int)UserWorkType.Picking)
            {
                var model = _crudRepository.GetQuery<outbound>(x => x.SysId == info.DocSysId).FirstOrDefault();
                if (model != null)
                {
                    model.AppointUserNames = info.AppointUserName;
                    _crudRepository.Update<outbound>(model);
                }
            }
        }
    }
}
