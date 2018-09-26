using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum RabbitMQType
    {
        /// <summary>
        /// 入库消息
        /// </summary>
        [Description("RabbitMQ.WMS.WriteBackReceipt")]
        WriteBackReceipt = 100,

        /// <summary>
        /// 出库消息
        /// </summary>
        [Description("RabbitMQ.WMS.WriteBackOutbound")]
        WriteBackOutbound = 200, 

        /// <summary>
        /// 库存消息
        /// </summary>
        [Description("RabbitMQ.WMS.Inventory")]
        Inventory = 300,

        /// <summary>
        /// 移仓出库
        /// </summary>
        [Description("RabbitMQ.WMS.TransferInventoryOutbound")]
        TransferInventoryOutbound = 310,
        
        /// <summary>
        /// 移仓入库
        /// </summary>
        [Description("RabbitMQ.WMS.TransferInventoryInbound")]
        TransferInventoryInbound = 311,

        /// <summary>
        /// ECC同步创建出库单
        /// </summary>
        [Description("RabbitMQ.WMS.InsertOutbound")]
        InsertOutbound = 315,

        /// <summary>
        /// ECC同步创建出库单(预包装处理)
        /// </summary>
        [Description("RabbitMQ.WMS.InsertOutbound_Prepack")]
        InsertOutbound_Prepack = 318,

        /// <summary>
        /// 快速发货
        /// </summary>
        [Description("RabbitMQ.WMS.OutboundQuickDelivery")]
        OutboundQuickDelivery = 330,

        /// <summary>
        /// 访问请求日志
        /// </summary>
        [Description("RabbitMQ.WMS.AccessLog")]
        AccessLog = 400,

        /// <summary>
        /// 业务日志
        /// </summary>
        [Description("RabbitMQ.WMS.BusinessLog")]
        BusinessLog = 500,

        /// <summary>
        /// 接口日志
        /// </summary>
        [Description("RabbitMQ.WMS.InterfaceLog")]
        InterfaceLog = 600,

        /// <summary>
        /// 交易日志
        /// </summary>
        [Description("RabbitMQ.WMS.InvTrans")]
        InvTrans = 700,

        /// <summary>
        /// 收货回写ECC
        /// </summary>
        [Description("RabbitMQ.WMS.ECCCallback.ReceiptERP_InStockOrder")]
        ReceiptERP_InStockOrder = 810,

        /// <summary>
        /// 发货回写ECC
        /// </summary>
        [Description("RabbitMQ.WMS.ECCCallback.OutStockERP_OutStock")]
        OutStockERP_OutStock = 820,

        /// <summary>
        /// 出库单自动分配
        /// </summary>
        [Description("RabbitMQ.WMS.Outbound_AutoAllocation")]
        Outbound_AutoAllocation = 830,

        /// <summary>
        /// 工单MQ
        /// </summary>
        [Description("RabbitMQ.WMS.Work_Insert_Update")]
        Work_Insert_Update = 900
    }
}