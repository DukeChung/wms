
function gvCheckAll(obj) {
    $("table tbody input[type='checkbox']").each(function () {
        this.checked = obj.checked;
    });
}

function getSelectedData(table) {
    var check = $("table tbody input:checked");
    if (check.length == 0) {
        return null;
    }
    var datas = [];
    $(check).each(function () {
        for (var i = 0; i < table.rows().data().length; i++) {
            if (table.rows().data()[i].SysId == $(this).val()) {
                datas.push(table.rows().data()[i]);
                break;
            }
        }
    });
    return datas;
}

function GetGridOnlySysId() {
    var check = $("table tbody input:checked");
    if (check.length === 0) {
        msgErro("请在列表中勾选一项!");
        return null;
    }
    return $(check).val();
}

function GetGridMultiselectSysId() {
    var check = $("table tbody input:checked");
    if (check.length === 0) {
        msgErro("请在列表中勾选一项!");
        return null;
    }
    var ids = "";

    $(check).each(function () {
        ids += $(this).val() + ",";
    });
    return ids;
}

//根据table的Id获取所有checkbox
function GetGridMultiselectSysIdById(id) {
    var check = $("#" + id + " tbody input:checked");
    if (check.length === 0) {
        msgErro("请在列表中勾选一项!");
        return null;
    }
    var ids = "";

    $(check).each(function () {
        ids += $(this).val() + ",";
    });
    return ids;
}

//根据table下的Clas获取所有checkbox
function GetGridMultiselectSysIdByClass(className) {
    var check = $("table tbody input:checkbox[class=" + className + "]:checked");
    if (check.length === 0) {
        msgErro("请在列表中勾选一项!");
        return null;
    }
    var ids = "";

    $(check).each(function () {
        ids += $(this).val() + ",";
    });
    return ids;
}

function GetGridSysId() {
    var check = $("table tbody input:checked");
    if (check.length === 0) {
        msgErro("请在列表中勾选一项!");
        return null;
    }
    if (check.length > 1) {
        msgErro("在列表中最多只能勾选一项!");
        return null;
    }
    return $(check).val();
}

//消息提示
function msgAlert(msg) {
    swal({
        title: "操作提示",
        text: msg,
        allowOutsideClick: false
    });
    $(".pace-done").focus();
}

//消息提示
function msgAlert(msg, action) {
    swal({
        title: "操作提示",
        text: msg,
        allowOutsideClick: false
    }, function () {
        action
    });
}


function msgErro(msg) {
    swal({
        title: "操作错误",
        text: msg,
        type: "warning",
        allowOutsideClick: false
    });
    $(".pace-done").focus();
}

function msgErroConfirm(msg, action, closeOnConfirm) {
    swal({
        title: "操作错误",
        text: msg,
        type: "warning",
        closeOnConfirm: arguments[2] || false,
        confirmButtonText: '库存不足明细',
        cancelButtonText: 'OK',
        showCancelButton: true,
        allowOutsideClick: false
    }, action);

    $(".pace-done").focus();
}

function msgSuccess(msg, action) {
    swal({
        title: "操作成功!",
        text: msg,
        type: "success",
        allowOutsideClick: false
    }, function () { action; });
    $(".pace-done").focus();
}

function msgConfirm(msg, action, closeOnConfirm) {
    swal({
        title: "操作提示",
        text: msg,
        type: "warning",
        closeOnConfirm: arguments[2] || false,
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        showCancelButton: true,
        allowOutsideClick: false
    }, action);

    $(".pace-done").focus();
}




function modelClose(id) {
    $("#" + id).modal('hide');
    $("#" + id).empty();
}

function goBack() {
    window.history.back(-1);
}

///show  close
function saveLoading(type) {
    if (type === 'show') {
        var html = '<div class="modal " id="saveLoading"  tabindex="-1" role="dialog" aria-hidden="true" data-backdrop="static" style="margin-top:250px ">'
            + '<div class="modal-dialog modal-sm">'
            + ' <div class="spiner-example">'
            + ' <div class="sk-spinner sk-spinner-wave">'
            + '  <div class="sk-rect1" style="background-color: #fb0248;"></div>'
            + '  <div class="sk-rect2"  style="background-color: #ffa400;"></div>'
            + '<div class="sk-rect3"  style="background-color: #fbff00;"></div>'
            + '   <div class="sk-rect4"  style="background-color: #02ffcc;"></div>'
            + '<div class="sk-rect5" ></div>'
            + ' <p style="color: white">请稍后...</p></div></div></div></div>';
        $(html).modal('show');
    }
    else {
        $("#saveLoading").modal('hide');
        $("#saveLoading").remove();
        $(".modal-backdrop").remove();
    }
}

//打印方法封装对象
var print = {};
print.Licenses1 = "B373432C4C51542C45D4E0F4A634612C";
print.Licenses2 = "C94CEE276DB2187AE6B65D56B3FC2848";

////箱贴
//print.PrintBoxLable = function (vanningDetail, actionType, printName) {
//    if (actionType == "") {
//        actionType = "PrintNoFinish";
//    }

//    var LODOP = getLodop();
//    LODOP.SET_LICENSES("", print.Licenses1, print.Licenses2, "");
//    LODOP.SET_PRINTER_INDEX(printName);
//    LODOP.SET_PRINT_STYLEA(0, "FontSize", 10);
//    LODOP.ADD_PRINT_URL(10, 10, 0, "100%", "http://" + window.location.host + "/Print/PrintBoxLable?sysId=" + vanningDetail.SysId + "&actionType=" + actionType);

//    LODOP.ADD_PRINT_BARCODE(175, 55, 200, 25, "128B", vanningDetail.ExternOrderId);
//    LODOP.ADD_PRINT_BARCODE(335, 12, 270, 45, "128B", vanningDetail.ContainerNumber);
//    LODOP.ADD_PRINT_BARCODE(490, 160, 200, 25, "128B", vanningDetail.ExternOrderId);
//    LODOP.ADD_PRINT_BARCODE(75, 260, 210, 120, "QRCode", "tel:" + vanningDetail.ConsigneePhone);
//    LODOP.SET_PRINT_STYLEA(0, "HOrient", 3);
//    LODOP.SET_PRINT_STYLEA(0, "VOrient", 3);
//    //LODOP.PREVIEW();
//    LODOP.PRINT();
//}

print.PrintBoxLable = function (vanningDetail, actionType, printName, warehouseSysId) {
    if (actionType == "") {
        actionType = "PrintNoFinish";
    }

    var LODOP = getLodop();
    LODOP.SET_LICENSES("", print.Licenses1, print.Licenses2, "");
    LODOP.SET_PRINTER_INDEX(printName);
    LODOP.SET_PRINT_STYLEA(0, "FontSize", 10);
    LODOP.ADD_PRINT_URL(10, 10, 0, "100%", "http://" + window.location.host + "/Print/PrintBoxLableZTO?sysId=" + vanningDetail.SysId + "&actionType=" + actionType + "&warehouseSysId=" + warehouseSysId);

    LODOP.ADD_PRINT_BARCODE(300, 50, 270, 50, "128B", vanningDetail.CarrierNumber);
    //LODOP.ADD_PRINT_BARCODE(470, 225, 150, 40, "128B", vanningDetail.CarrierNumber);
    LODOP.ADD_PRINT_BARCODE(470, 195, 180, 40, "128B", vanningDetail.CarrierNumber);
    //LODOP.ADD_PRINT_BARCODE(75, 260, 210, 120, "QRCode", "tel:" + vanningDetail.ConsigneePhone);
    LODOP.SET_PRINT_STYLEA(0, "HOrient", 3);
    LODOP.SET_PRINT_STYLEA(0, "VOrient", 3);
    //LODOP.PREVIEW();
    LODOP.PRINT();
}


//箱贴ToB
print.PrintBoxLableToB = function (vanningDetail, actionType, printName, warehouseSysId) {
    if (actionType == "") {
        actionType = "PrintNoFinish";
    }

    var LODOP = getLodop();
    LODOP.SET_LICENSES("", print.Licenses1, print.Licenses2, "");
    LODOP.SET_PRINTER_INDEX(printName);
    LODOP.SET_PRINT_STYLEA(0, "FontSize", 10);
    LODOP.ADD_PRINT_URL(10, 10, 0, "100%", "http://" + window.location.host + "/Print/PrintBoxLableToB?sysId=" + vanningDetail.SysId + "&actionType=" + actionType + "&warehouseSysId=" + warehouseSysId);
    var row = vanningDetail.VannginSkuCount;
    var height = print.GetPrintBarCodeTop(row);

    LODOP.ADD_PRINT_BARCODE(490 + height, 160, 200, 25, "128B", vanningDetail.ExternOrderId);
    LODOP.ADD_PRINT_BARCODE(540 + height, 20, 250, 65, "128B", vanningDetail.ContainerNumber);

    //LODOP.PREVIEW();
    LODOP.PRINT();
}

print.GetPrintBarCodeTop = function (row) {
    var totalRow = 28 * row + 45;
    var firstPage = 310 + 270;
    var totalHeight = 100 + 310 + 270 + 1;
    var topHeight = 100 + 310;
    var page = 0;

    if (totalRow > firstPage) {
        if ((totalRow - firstPage) % totalHeight == 0) {
            page = parseInt((totalRow - firstPage) / totalHeight);
        }
        else {
            page = parseInt((totalRow - firstPage) / totalHeight) + 1;
        }

        var remain = totalRow - firstPage - (totalHeight * (page - 1));
        if (remain > topHeight) {
            page += 1;
        }
    }
    else {
        if (totalRow > 310) {
            page += 1;
        }
    }
    return totalHeight * page;
}

//装箱单
print.PrintVanningPackingDetail = function (sysId, printName, printUserName, actionType, orderType, warehouseSysId) {
    if (actionType == "") {
        actionType = "PrintNoFinish";
    }

    var LODOP = getLodop();
    LODOP.SET_LICENSES("", print.Licenses1, print.Licenses2, "");
    LODOP.SET_PRINTER_INDEX(printName);
    LODOP.SET_PRINT_STYLEA(0, "FontSize", 10);
    if (orderType == "B2C") {
        LODOP.ADD_PRINT_URL(10, 10, 0, "100%", encodeURI("http://" + window.location.host + "/Print/PrintVanningPackingDetail?sysId=" + sysId + "&printUserName=" + printUserName + "&actionType=" + actionType + "&warehouseSysId=" + warehouseSysId));
    } else {
        LODOP.ADD_PRINT_URL(10, 10, 0, "100%", encodeURI("http://" + window.location.host + "/Print/PrintVanningPackingDetailToB?sysId=" + sysId + "&printUserName=" + printUserName + "&actionType=" + actionType + "&warehouseSysId=" + warehouseSysId));
    }

    LODOP.SET_PRINT_STYLEA(0, "HOrient", 3);
    LODOP.SET_PRINT_STYLEA(0, "VOrient", 3);
    //LODOP.PREVIEW();
    LODOP.PRINT();
}

//打印批次
print.PrintLot = function (dataLenght, printName, lot, lotAttr01) {
    for (var i = 0; i < dataLenght; i++) {
        var LODOP = getLodop();
        LODOP.SET_LICENSES("", "B373432C4C51542C45D4E0F4A634612C", "C94CEE276DB2187AE6B65D56B3FC2848", "");
        LODOP.SET_PRINTER_INDEX(printName);
        LODOP.ADD_PRINT_BARCODE(20, 30, 340, 80, "128B", lot);
        LODOP.SET_PRINT_STYLE("FontSize", 40);
        LODOP.SET_PRINT_STYLEA(0, "ShowBarText", 0);
        LODOP.ADD_PRINT_HTM(100, 30, 360, 100, "<div style='font-size:24px;text-align: center;padding:5px 0;'>" + lot + "</div><div style='font-size:20px;text-align: left;padding:5px 0;'><span>渠道：</span><span>" + lotAttr01 + "</span></div>");
        LODOP.PRINT();
    }
}


var common = {};

//获取文件后缀名
common.GetFileTypeName = function getFileSuffixName(filename) {
    if (filename != '' && filename != null) {
        var index1 = filename.lastIndexOf(".");
        var index2 = filename.length;
        var postf = filename.substring(index1, index2);//后缀名  
        return postf.toLowerCase();
    }
    return "";
}


var ChooseSkuModel = {
    chooseSkuSysId: null,
    existsSkuSysId: null,
    skuDialog: {},
    showModal: function (upc, action) {
        ChooseSkuModel.skuDialog = dialog({
            title: '选择商品',
            width: '900px',
            height: '450px',
            zIndex: 6666,
            left: 100,
            cancel: false,
            //content: '欢迎使用 artDialog 对话框组件！'
            url: "/Common/ChooseSkuFromSameUPCPartial?upc=" + upc,
            onclose: function () {
                //if (this.returnValue) {
                //    $('#value').html(this.returnValue);
                //}
                //console.log('onclose');
                action(this.returnValue);
            }
        });
        ChooseSkuModel.skuDialog.showModal();
    }
}



var ChooseSkuNewModel = {
    chooseSkuSysId: null,
    existsSkuSysId: null,
    skuDialog: {},
    showModal: function (query, action) {
        ChooseSkuModel.skuDialog = dialog({
            title: '选择商品',
            width: '900px',
            height: '450px',
            zIndex: 6666,
            left: 0,
            cancel: false,
            //content: '欢迎使用 artDialog 对话框组件！'
            url: "/Common/ChooseSkuFromSameUPCPartial?upc=" + query.upc + "&skuName=" + query.skuName + "&skuCode=" + query.skuCode,
            onclose: function () {
                action(this.returnValue);
            }
        });
        ChooseSkuModel.skuDialog.showModal();
    }
}