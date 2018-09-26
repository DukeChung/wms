
$.fn.serializeObject = function () {
    debugger;
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined)
        {
            if (!o[this.name].push)
            {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else
        {
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