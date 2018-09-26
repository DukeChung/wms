
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            if (this.value === 'on' && $("#" + this.name).attr("type") === 'checkbox') {
                o[this.name] = true || '';
            }
            else {
                o[this.name] = this.value || '';
            }

        }
    });
    return o;
};

function commonCheckDateCompare(startDate, endDate) {
    if ($.trim(startDate).length > 0
          && $.trim(endDate).length > 0) {
        var start = new Date(startDate.replace("-", "/").replace("-", "/"));
        var end = new Date(endDate.replace("-", "/").replace("-", "/"));
        if (end < start) {
            return false;
        }
    }
    return true;
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

function msgErro(msg) {
    swal({
        title: "操作错误",
        text: msg,
        type: "warning",
        allowOutsideClick: false
    });
    $(".pace-done").focus();
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
            + ' <p style="color: white">请稍候...</p></div></div></div></div>';
        $(html).modal('show');
    }
    else {
        $("#saveLoading").modal('hide');
        $("#saveLoading").remove();
        $(".modal-backdrop").remove();
    }
}