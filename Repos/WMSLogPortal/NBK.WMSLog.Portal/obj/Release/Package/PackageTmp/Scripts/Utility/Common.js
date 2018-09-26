 
function gvCheckAll(obj) {
    $("table tbody input[type='checkbox']").each(function() {
        this.checked = obj.checked;
    });
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
  
    $(check).each(function ()
    {
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

function msgErro(msg)
{
    swal({
        title: "操作错误",
        text: msg,
        type: "warning",
        allowOutsideClick: false
    });
    $(".pace-done").focus();
}

function msgSuccess(msg,action) {
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
    $("#"+id).modal('hide');
    $("#" + id).empty();
}

function goBack()
{
    window.history.back(-1);
}

///show  close
function saveLoading(type)
{
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

//箱贴
print.PrintBoxLable = function(vanningDetail, actionType, printName) {
    if (actionType == "") {
        actionType = "PrintNoFinish";
    }

    var LODOP = getLodop();
    LODOP.SET_LICENSES("", print.Licenses1, print.Licenses2, "");
    LODOP.SET_PRINTER_INDEX(printName);
    LODOP.SET_PRINT_STYLEA(0, "FontSize", 10);
    LODOP.ADD_PRINT_URL(10, 10, 0, "100%", "http://" + window.location.host + "/Print/PrintBoxLable?sysId=" + vanningDetail.SysId + "&actionType=" + actionType);

    LODOP.ADD_PRINT_BARCODE(175, 55, 200, 25, "128B", vanningDetail.ExternOrderId);
    LODOP.ADD_PRINT_BARCODE(335, 12, 270, 45, "128B", vanningDetail.ContainerNumber);
    LODOP.ADD_PRINT_BARCODE(490, 180, 200, 25, "128B", vanningDetail.ExternOrderId);
    LODOP.ADD_PRINT_BARCODE(75, 260, 210, 120, "QRCode", "tel:" + vanningDetail.ConsigneePhone);
    LODOP.SET_PRINT_STYLEA(0, "HOrient", 3);
    LODOP.SET_PRINT_STYLEA(0, "VOrient", 3);
    //LODOP.PREVIEW();
    LODOP.PRINT();
}

//箱贴ToB
print.PrintBoxLableToB = function(vanningDetail, actionType, printName) {
    if (actionType == "") {
        actionType = "PrintNoFinish";
    }

    var LODOP = getLodop();
    LODOP.SET_LICENSES("", print.Licenses1, print.Licenses2, "");
    LODOP.SET_PRINTER_INDEX(printName);
    LODOP.SET_PRINT_STYLEA(0, "FontSize", 10);
    LODOP.ADD_PRINT_URL(10, 10, 0, "100%", "http://" + window.location.host + "/Print/PrintBoxLableToB?sysId=" + vanningDetail.SysId + "&actionType=" + actionType);
    var row = vanningDetail.VannginSkuCount;
    var height = print.GetPrintBarCodeTop(row);

    LODOP.ADD_PRINT_BARCODE(490 + height, 180, 200, 25, "128B", vanningDetail.ExternOrderId);
    LODOP.ADD_PRINT_BARCODE(540 + height, 20, 250, 65, "128B", vanningDetail.ContainerNumber);

    //LODOP.PREVIEW();
    LODOP.PRINT();
}

print.GetPrintBarCodeTop = function(row){
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
print.PrintVanningPackingDetail = function(sysId, printName, printUserName) {
    var LODOP = getLodop();
    LODOP.SET_LICENSES("", print.Licenses1, print.Licenses2, "");
    LODOP.SET_PRINTER_INDEX(printName);
    LODOP.SET_PRINT_STYLEA(0, "FontSize", 10);
    LODOP.ADD_PRINT_URL(10, 10, 0, "100%", encodeURI("http://" + window.location.host + "/Print/PrintVanningPackingDetail?sysId=" + sysId + "&printUserName=" + printUserName));

    LODOP.SET_PRINT_STYLEA(0, "HOrient", 3);
    LODOP.SET_PRINT_STYLEA(0, "VOrient", 3);
    //LODOP.PREVIEW();
    LODOP.PRINT();
} 