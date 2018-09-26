//自定义版本内容


var com = {};

com.RefreshData = function (keyId, newNum) {
    $({ numberValue: 0 }).animate({ numberValue: newNum }, {
        duration: 1000,
        easing: 'linear',
        step: function () {
            $(keyId).html(Math.floor(this.numberValue));
        },
        done: function () {
            if (newNum != null && $(keyId).html() != newNum.toString()) {
                $(keyId).html(newNum);
            }
        }
    });
}

com.GetColor = function (i) {
    var color = '';
    switch (i) {
        case 0:
            color = '#DC0909';
            break;
        case 1:
            color = '#ED213D';
            break;
        case 2:
            color = '#FD334F';
            break;
        case 3:
            color = '#FF556D';
            break;
        case 4:
            color = '#FF7A75';
            break;
        case 5:
            color = '#FF9E91';
            break;
        case 6:
            color = '#FFA857';
            break;
        case 7:
            color = '#FFCB61';
            break;
        case 8:
            color = '#FFE143';
            break;
        case 9:
            color = '#F8ED39';
            break;
    }
    return color;
}