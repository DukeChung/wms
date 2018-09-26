/**
 * jquery plugin -- jquery.bankInput.js
 * Description: jQuery 银行卡号格式化插件
 * Version: 1.0
 * update: 2017-10-22
 */
(function ($) {
    $.fn.bankInput = function (options) {
        var defaults = {
            min: 16,                            // 最少输入字数 
            max: 25,                            // 最多输入字数 
            deimiter: ' ',                      // 账号分隔符 
            onlyNumber: true,                   // 只能输入数字 
            copy: false,                        // 允许复制 
            border: '',                         //边框
            color: '',                          //字体颜色  
            fontFamly: 'Times New Roman'        //字体
        };
        var opts = $.extend({}, defaults, options);
        var obj = $(this);
        obj.css({ imeMode: 'Disabled', border: opts.border, color: opts.color, fontFamly: opts.fontFamly }).attr('maxlength', opts.max);
        if (obj.val() != '') obj.val(obj.val().replace(/\s/g, '').replace(/(\d{4})(?=\d)/g, "$1" + opts.deimiter));
        obj.bind('keyup', function (event) {
            if (opts.onlyNumber) {
                if (!(event.keyCode >= 48 && event.keyCode <= 57)) {
                    this.value = this.value.replace(/\D/g, '');
                }
            }
            this.value = this.value.replace(/\s/g, '').replace(/(\d{4})(?=\d)/g, "$1" + opts.deimiter);
        }).bind('dragenter', function () {
            return false;
        }).bind('onpaste', function () {
            return !clipboardData.getData('text').match(/\D/);
        }).bind('blur', function () {
            this.value = this.value.replace(/\s/g, '').replace(/(\d{4})(?=\d)/g, "$1" + opts.deimiter);
            if (this.value.length < opts.min) {
                obj.focus();
            }
        })
    }
    // 列表显示格式化 
    $.fn.bankList = function (options) {
        var defaults = {
            deimiter: ' ' // 分隔符 
        };
        var opts = $.extend({}, defaults, options);
        return this.each(function () {
            $(this).text($(this).text().replace(/\s/g, '').replace(/(\d{4})(?=\d)/g, "$1" + opts.deimiter));
        })
    }
})(jQuery);