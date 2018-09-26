//防止jquery和别的类库冲突
$.noConflict()
var $j = jQuery;

//var https = "http://localhost:9058/";
//var https = "http://10.66.21.19:81/";
var https = "http://10.66.8.47:8089/";
//var https = "http://10.66.141.100:8089/";
//var https = "http://api-wms.gznb.com/";

var com = {};

//公共ajax方法
com.CommonAjax = function (url, datas, callback) {
    com.Loading();
    $.ajax({
        type: 'post',
        url: https + url,
        data: datas,
        success: function (data) {
            com.hideLoading();
            callback(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            com.hideLoading();
            if ($("#msg") != null && $("#msg") != undefined) {
                com.FailMsg("#msg", JSON.parse(XMLHttpRequest.response).ErrorMessage);
            } else {
                alert(JSON.parse(XMLHttpRequest.response).ErrorMessage);
            }
            $.hideIndicator();
        }
    });
}

com.CommonNoLoadingAjax = function (url, datas, callback) {
    $.ajax({
        type: 'post',
        url: https + url,
        data: datas,
        success: function (data) {
            callback(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if ($("#msg") != null && $("#msg") != undefined) {
                com.FailMsg("#msg", JSON.parse(XMLHttpRequest.response).ErrorMessage);
            } else {
                alert(JSON.parse(XMLHttpRequest.response).ErrorMessage);
            }
            $.hideIndicator();
        }
    });
}

//公共Ajax同步方法
com.CommonSyncAjax = function (url, datas, callback) {
    com.Loading();
    $.ajax({
        type: 'post',
        url: https + url,
        async: false,
        data: datas,
        success: function (data) {
            com.hideLoading();
            callback(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            com.hideLoading();
            if ($("#msg") != null && $("#msg") != undefined) {
                com.FailMsg("#msg", JSON.parse(XMLHttpRequest.response).ErrorMessage);
            } else {
                alert(JSON.parse(XMLHttpRequest.response).ErrorMessage);
            }
            $.hideIndicator();
        }
    });
}

//分页查询Ajax方法
com.CommonLoadDataAjax = function (url, datas, callback) {
    com.hideLoading();
    com.InitPageDiv();
    $.ajax({
        type: 'post',
        url: https + url,
        data: datas,
        success: function (data) {
            callback(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if ($("#msg") != null && $("#msg") != undefined) {
                com.FailMsg("#msg", JSON.parse(XMLHttpRequest.response).ErrorMessage);
            } else {
                alert(JSON.parse(XMLHttpRequest.response).ErrorMessage);
            }
            $.hideIndicator();
        }
    });
}

com.CommonGetAjax = function (url, callback) {
    com.Loading();
    $.ajax({
        type: 'get',
        url: https + url,
        success: function (data) {
            com.hideLoading();
            callback(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            com.hideLoading();
            if ($("#msg") != null && $("#msg") != undefined) {
                com.FailMsg("#msg", JSON.parse(XMLHttpRequest.response).ErrorMessage);
            } else {
                alert(JSON.parse(XMLHttpRequest.response).ErrorMessage);
            }
            $.hideIndicator();
        }
    });
}

com.CommonNoLoadingGetAjax = function (url, callback) {
    $.ajax({
        type: 'get',
        url: https + url,
        success: function (data) {
            callback(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if ($("#msg") != null && $("#msg") != undefined) {
                com.FailMsg("#msg", JSON.parse(XMLHttpRequest.response).ErrorMessage);
            } else {
                alert(JSON.parse(XMLHttpRequest.response).ErrorMessage);
            }
            $.hideIndicator();
        }
    });
}

//获取参数
com.GetQueryString = function (name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}

/*
获取本地化持久缓存
key：名称
*/
function GetlocalStorage(key) {
    var value = localStorage[key];
    if (value != undefined && value != null) {
        return value;
    } else {
        return "";
    }
}
/*
写入本地化持久缓存
key：名称
value：值
*/
function SetlocalStorage(key, value) {
    localStorage.setItem(key, value);
}

/*
移除本地化持久缓存
key：名称
*/
function RemovelocalStorage(key) {
    localStorage.removeItem(key);
}


//验证是否为数字
com.CheckPositiveNumber = function (text) {
    var strP = /^\d+(\.\d+)?$/;
    if (!strP.test(text)) {
        return false;
    } else {
        return true;
    }
}

//是否为数字
com.CheckNumber = function (text) {
    if (isNaN(text)) {
        return false;
    } else {
        return true;
    }
}

//成功提示语
com.SuccessMsg = function (obj, msg) {
    $(obj).removeClass("msg-error").addClass("msg-success").html(msg);
}

//失败提示语
com.FailMsg = function (obj, msg) {
    //  window.softinput.PlayError();
    $(obj).removeClass("msg-success").addClass("msg-error").html(msg);
}

//未检索到数据提示
com.NoData = function (obj) {
    $("#noData").remove();
    $(obj).parent().after('<div id="noData" style="text-align:center;width:100%;padding-bottom:15px;">未检索到数据</div>');
    com.NoScroll();
}

//加载提示
com.Loading = function () {
    com.hideLoading();
    if ($(".table").length > 0) {
        $("#noData").remove();
        $(".table").after('<div id="loading"><div class="infinite-scroll-preloader"><div class="preloader"></div></div></div>');
    }
}

//清除加载
com.hideLoading = function () {
    $("#loading").remove();
}

com.InitPageDiv = function () {
    $('.infinite-scroll-preloader').show();
    if ($("#noData") != null && $("#noData") != undefined) {
        $("#noData").remove();
    }
}

var loading = false;
// 最多可加载的条目
var iTotalDisplayRecords = 0;
// 每次加载添加多少条目
var iDisplayLength = 20;
//每次从第几行开始
var iDisplayStart = 0;

//初始化无限滚动参数
com.InitLoadData = function () {
    loading = false;
    iTotalDisplayRecords = 0;
    iDisplayLength = 20;
    iDisplayStart = 0;
}

//无限滚动加载数据
com.ScrollLoadData = function (callback) {
    com.InitLoadData();
    $(document).unbind('infinite', '.infinite-scroll-bottom');
    $(document).on('infinite', '.infinite-scroll-bottom', function () {
        // 如果正在加载，则退出
        if (loading) return;
        // 更新最后加载的序号
        iDisplayStart += iDisplayLength;
        // 添加新条目
        callback();
        if (iDisplayStart >= iTotalDisplayRecords) {
            try {
                // 删除加载提示符
                $('.infinite-scroll-preloader').hide();
                // 加载完毕，则注销无限加载事件，以防不必要的加载
                $.detachInfiniteScroll($('.infinite-scroll'));
            } catch (ex) { }
            return;
        }
        //容器发生改变,如果是js滚动，需要刷新滚动
        $.refreshScroller();
    });
}

com.NoScroll = function () {
    if (iDisplayLength >= iTotalDisplayRecords) {
        // 删除加载提示符
        $('.infinite-scroll-preloader').hide();
        // 加载完毕，则注销无限加载事件，以防不必要的加载
        try {
            $.detachInfiniteScroll($('.infinite-scroll'));
        } catch (ex) { };
    }
}

var sku = {};
sku.SkuDetailList;

//预加载选择框
sku.OnPicker = function (upc, obj, focusObj, callback, onCloseCallBack) {
    var valueArray = new Array();
    var textArray = new Array();
    $("#selectkey").val("");
    if (sku.SkuDetailList != null && sku.SkuDetailList.length > 0) {
        for (var i in sku.SkuDetailList) {
            var detail = sku.SkuDetailList[i];
            if (detail.UPC == upc) {
                var value = "S|" + detail.SkuSysId;
                valueArray.push(value);
                textArray.push("商品:" + detail.SkuName + ";数量:1");
            }

            if (detail.UPC02 == upc) {
                var value = "P|" + detail.FieldValue02 + "|" + detail.SkuSysId;
                valueArray.push(value);
                textArray.push("包装:" + detail.SkuName + ";数量:" + detail.FieldValue02);
            }
            if (detail.UPC03 == upc) {
                var value = "P|" + detail.FieldValue03 + "|" + detail.SkuSysId;
                valueArray.push(value);
                textArray.push("包装:" + detail.SkuName + ";数量:" + detail.FieldValue03);
            }
            if (detail.UPC04 == upc) {
                var value = "P|" + detail.FieldValue04 + "|" + detail.SkuSysId;
                valueArray.push(value);
                textArray.push("包装:" + detail.SkuName + ";数量:" + detail.FieldValue04);
            }
            if (detail.UPC05 == upc) {
                var value = "P|" + detail.FieldValue05 + "|" + detail.SkuSysId;
                valueArray.push(value);
                textArray.push("包装:" + detail.SkuName + ";数量:" + detail.FieldValue05);
            }

        }
    }

    $("#values").val(valueArray.join('*'));
    $("#texts").val(textArray.join('*'));

    if (valueArray != null && valueArray.length > 0) {
        if (valueArray.length == 1) {
            $("#selectkey").val(valueArray[0]);
            var v = valueArray[0].split('|');
            if (v[0] == "P") {
                if ($("#inputQty").val() == "" || parseInt($("#inputQty").val()) <= 1) {
                    $(obj).val(v[1]);
                }
            }

            if (onCloseCallBack != undefined && onCloseCallBack != null) {
                onCloseCallBack();
            }
        } else {
            $("#selectkey").picker({
                toolbarTemplate: '<header class="bar bar-nav">\
                   <button class="button button-link pull-right close-picker">确定</button>\
                   <h1 class="title">请选择</h1>\
                   </header>',
                cols: [
                  {
                      textAlign: 'center',
                      values: valueArray,
                      texts: textArray
                  }
                ],
                onOpen: function (picker) {
                    picker.cols[0].replaceValues($("#values").val().split('*'), $("#texts").val().split('*'));
                    picker.updateValue();
                },
                onClose: function () {
                    if (focusObj != undefined && focusObj != null) {
                        $(focusObj).focus();
                    }

                    if (onCloseCallBack != undefined && onCloseCallBack != null) {
                        onCloseCallBack();
                    }
                }
            });
            $("#selectkey").picker("open");

            var inputValue = $("#selectkey").val().split('|');
            if (inputValue[0] == "P") {
                if ($("#inputQty").val() == "" || parseInt($("#inputQty").val()) <= 1) {
                    $(obj).val(inputValue[1]);
                }
            }

            $("#selectkey").on("change", function () {
                var v = $("#selectkey").val().split("|");
                if ($("#inputQty").val() == "" || parseInt($("#inputQty").val()) <= 1) {
                    if (v[0] == "P") {
                        $(obj).val(v[1]);
                    } else {
                        $(obj).val("");
                    }
                }

                if (callback != null && callback != undefined) {
                    callback();
                }
            });

        }
    } else {
        return false;
    }
    return true;
}

sku.OnClose = function () {
    if ($("#selectkey").length != 0) {
        if ($("#selectkey").val().length > 0) {
            $("#selectkey").picker("close");
        }
    }
}

//获取选择的SkuSysId
sku.GetSkuSysId = function () {
    var skusysid = "";
    if ($("#selectkey").val() != "") {
        var v = $("#selectkey").val().split("|");
        if (v[0] == "P") {
            skusysid = v[2];
        } else {
            skusysid = v[1];
        }
    }

    return skusysid;
}

//存储明细
sku.SetSkuDetailList = function (data) {
    sku.SkuDetailList = null;
    sku.SkuDetailList = data;
}

//拣货
var picking = {};
picking.DetailList;
picking.SetDetailList = function (data) {
    picking.DetailList = null;
    picking.DetailList = data;
}