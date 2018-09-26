using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMSBussinessApi.Dto.DataSync;
using WMSBussinessApi.Dto.Log;
using WMSBussinessApi.Repository;
using WMSBussinessApi.Service;
using WMSBussinessApi.Utility;
using WMSBussinessApi.Utility.Enum;
using WMSBussinessApi.Utility.Enum.Log;
using WMSBussinessApi.Utility.Helper;
using WMSBussinessApi.Utility.MQ;

namespace WMSBussinessApi.ServiceImp
{
    public class DataSyncSvc : IDataSyncSvc
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private IDataSyncRep _dataSyncRep;
        public DataSyncSvc(IDataSyncRep dataSyncRep)
        {
            _dataSyncRep = dataSyncRep;
        }

        public string Test()
        {
            //_dataSyncRep.SetConnection("server=10.66.2.33;port=3321;password=setpay@123;Allow User Variables=True;user id=root;persistsecurityinfo=True;database=prd_wms_lincang;character set=utf8");
            return JsonConvert.SerializeObject(new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() });
        }

        public void SyncCreateSku(SkuDto sku)
        {
            sku.SysId = Guid.NewGuid();

            var warehouseList = _dataSyncRep.GetAllWarehouseInfo();
            if (warehouseList.Count > 0)
            {
                warehouseList.ForEach(p =>
                {
                    try
                    {
                        _dataSyncRep.SetConnection(p.DecryptConnectionString);
                        _dataSyncRep.SyncCreateSku(sku);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"SyncCreateSku Error:{ex.Message}");

                        InterfaceLogDto interfaceLogDto = new InterfaceLogDto(sku)
                        {
                            interface_type = InterfaceType.Invoked.ToDescription(),
                            interface_name = PublicConst.SyncCreateSku,
                            response_json = JsonConvert.SerializeObject(new { ErrorMessage = $"{ex.Message}" }),
                            flag = false,
                            descr = "DataSync/SyncCreateSku",
                            end_time = DateTime.Now
                        };
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    }
                });
            }
        }

        public void SyncUpdateSku(SkuDto sku)
        {
            var warehouseList = _dataSyncRep.GetAllWarehouseInfo();
            if (warehouseList.Count > 0)
            {
                warehouseList.ForEach(p =>
                {
                    try
                    {
                        _dataSyncRep.SetConnection(p.DecryptConnectionString);
                        _dataSyncRep.SyncUpdateSku(sku);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"SyncUpdateSku Error:{ex.Message}");

                        InterfaceLogDto interfaceLogDto = new InterfaceLogDto(sku)
                        {
                            interface_type = InterfaceType.Invoked.ToDescription(),
                            interface_name = PublicConst.SyncUpdateSku,
                            response_json = JsonConvert.SerializeObject(new { ErrorMessage = $"{ex.Message}" }),
                            flag = false,
                            descr = "DataSync/SyncUpdateSku",
                            end_time = DateTime.Now
                        };
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    }
                });
            }
        }

        public void SyncCreatePack(PackDto pack)
        {
            pack.SysId = Guid.NewGuid();

            var warehouseList = _dataSyncRep.GetAllWarehouseInfo();
            if (warehouseList.Count > 0)
            {
                warehouseList.ForEach(p =>
                {
                    try
                    {
                        _dataSyncRep.SetConnection(p.DecryptConnectionString);
                        _dataSyncRep.SyncCreatePack(pack);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"SyncCreatePack Error:{ex.Message}");

                        InterfaceLogDto interfaceLogDto = new InterfaceLogDto(pack)
                        {
                            interface_type = InterfaceType.Invoked.ToDescription(),
                            interface_name = PublicConst.SyncCreatePack,
                            response_json = JsonConvert.SerializeObject(new { ErrorMessage = $"{ex.Message}" }),
                            flag = false,
                            descr = "DataSync/SyncCreatePack",
                            end_time = DateTime.Now
                        };
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    }
                });
            }
        }

        public void SyncUpdatePack(PackDto pack)
        {
            var warehouseList = _dataSyncRep.GetAllWarehouseInfo();
            if (warehouseList.Count > 0)
            {
                warehouseList.ForEach(p =>
                {
                    try
                    {
                        _dataSyncRep.SetConnection(p.DecryptConnectionString);
                        _dataSyncRep.SyncUpdatePack(pack);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"SyncUpdatePack Error:{ex.Message}");

                        InterfaceLogDto interfaceLogDto = new InterfaceLogDto(pack)
                        {
                            interface_type = InterfaceType.Invoked.ToDescription(),
                            interface_name = PublicConst.SyncUpdatePack,
                            response_json = JsonConvert.SerializeObject(new { ErrorMessage = $"{ex.Message}" }),
                            flag = false,
                            descr = "DataSync/SyncUpdatePack",
                            end_time = DateTime.Now
                        };
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    }
                });
            }
        }

        public void SyncDeletePack(List<Guid> sysIdList)
        {
            var warehouseList = _dataSyncRep.GetAllWarehouseInfo();
            if (warehouseList.Count > 0)
            {
                warehouseList.ForEach(p =>
                {
                    try
                    {
                        _dataSyncRep.SetConnection(p.DecryptConnectionString);
                        _dataSyncRep.SyncDeletePack(sysIdList);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"SyncDeletePack Error:{ex.Message}");

                        InterfaceLogDto interfaceLogDto = new InterfaceLogDto(sysIdList)
                        {
                            interface_type = InterfaceType.Invoked.ToDescription(),
                            interface_name = PublicConst.SyncDeletePack,
                            response_json = JsonConvert.SerializeObject(new { ErrorMessage = $"{ex.Message}" }),
                            flag = false,
                            descr = "DataSync/SyncDeletePack",
                            end_time = DateTime.Now
                        };
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    }
                });
            }
        }


        public void SyncCreateSyscode(SyscodeDto syscode)
        {
            syscode.SysId = Guid.NewGuid();

            var warehouseList = _dataSyncRep.GetAllWarehouseInfo();
            if (warehouseList.Count > 0)
            {
                warehouseList.ForEach(p =>
                {
                    try
                    {
                        _dataSyncRep.SetConnection(p.DecryptConnectionString);
                        _dataSyncRep.SyncCreateSyscode(syscode);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"SyncCreateSyscode Error:{ex.Message}");

                        InterfaceLogDto interfaceLogDto = new InterfaceLogDto(syscode)
                        {
                            interface_type = InterfaceType.Invoked.ToDescription(),
                            interface_name = PublicConst.SyncCreateSyscode,
                            response_json = JsonConvert.SerializeObject(new { ErrorMessage = $"{ex.Message}" }),
                            flag = false,
                            descr = "DataSync/SyncCreateSyscode",
                            end_time = DateTime.Now
                        };
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    }
                });
            }
        }

        public void SyncCreateSyscodeDetail(SyscodeDetailDto syscodedetail)
        {
            syscodedetail.SysId = Guid.NewGuid();

            var warehouseList = _dataSyncRep.GetAllWarehouseInfo();
            if (warehouseList.Count > 0)
            {
                warehouseList.ForEach(p =>
                {
                    try
                    {
                        _dataSyncRep.SetConnection(p.DecryptConnectionString);
                        _dataSyncRep.SyncCreateSyscodeDetail(syscodedetail);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"SyncCreateSyscodeDetail Error:{ex.Message}");

                        InterfaceLogDto interfaceLogDto = new InterfaceLogDto(syscodedetail)
                        {
                            interface_type = InterfaceType.Invoked.ToDescription(),
                            interface_name = PublicConst.SyncCreateSyscodeDetail,
                            response_json = JsonConvert.SerializeObject(new { ErrorMessage = $"{ex.Message}" }),
                            flag = false,
                            descr = "DataSync/SyncCreateSyscodeDetail",
                            end_time = DateTime.Now
                        };
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    }
                });
            }
        }
    }
}
