/*
 */
/**
 * lazy load for html element.
 * $('.lazy-container').lazyload({
 *      lazySelector: '[data-src]',
 *      lazyAttribute: 'data-src',
 *      loadDivFn: function (el, src) {
 *         // DO SOMETHING ..
 *      }
 * });
 *
 * @author yangchanghe
 */
;
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery'], factory);
    } else if (typeof exports === 'object') {
        module.exports = factory;
    } else {
        factory(jQuery);
    }
}(function ($, Dom, Events) {
    window.console = window.console || { log: function () { } };
    var WIN = window,
        NODE_TYPE_DOC = 9,
        SCROLL = 'scroll',
        TOUCH_MOVE = "touchmove",
        RESIZE = 'resize',
        DURATION = 100,

        DEFAULTS = {
            container: undefined,           //懒加载容器
            lazySelector: '[data-src]',     //懒加载元素
            lazyAttribute: 'data-src',      //懒加载元素属性，
            duration: DURATION,             // 响应时间默认100毫秒.
            threshold: 0,                   // 水平和垂直阈值。
            thresholdX: 0,                  // 水平值
            thresholdY: 0,                  // 垂直值
            placeholder: 'loading.gif',     // 等待效果
            errorPlaceholder: undefined,    // 错误提示

            /**
             * load function, load + TagName + Fn
             */
            loadImgFn: function (el, src) {
                var me = this,
                    options = me._options,
                    img;

                el.src = options.placeholder;
                img = new Image();
                img.onload = function () {
                    setTimeout(function () {
                        el.src = src;
                    }, 800);
                };
                img.onerror = function () {
                    console.log('load error:', src);
                    if (me.errorPlaceholder) {
                        el.src = me.errorPlaceholder;
                    }
                };
                img.src = src;
            },
            loadDivFn: function (el, src) {
                var iframe = document.createElement('iframe');
                iframe.style.width = '100%';
                iframe.style.height = '100%';
                iframe.src = src;
                el.appendChild(iframe);
            },
            loadFn: function (el, src) {
                // console.log('no load function configure for ' + el.tagName + '?');
            }
        };

    /**
     * LazyLoad constructor and factory.
     *
     * @param options lazyload configuration.
     * @returns {LazyLoad}
     * @constructor
     */
    function LazyLoad(options) {
        var me = this, container;

        if (!(me instanceof LazyLoad)) {
            return new LazyLoad(opts);
        }

        options = me._options = $.extend({}, DEFAULTS, options);
        container = options.container;

        container = container || document;
        if (container == container.window) { // window
            container = container.document;
        } else if ('body' === container.nodeName.toLowerCase()) {
            container = container.ownerDocument;
        }

        me.container = container;
        me._containerIsNotDocument = NODE_TYPE_DOC !== me.container.nodeType;

        me._init();
        me._loadFn();

        $.ready(function () {
            me._loadFn();
        });

        me.resume();
    }

    /*-
     * lazyload instance method.
     */
    // TODO replace jQuery to native js.
    $.extend(LazyLoad.prototype, {
        /**
         * initial load function.
         *
         * @private
         */
        _init: function () {
            var me = this,
                options = me._options,
                duration = options.duration || DURATION;

            // lazy call load function.
            me._loadFn = util.buffer(function () {
                me.loadItems();
            }, duration, me);
        },

        /**
         * force lazyload to recheck constraints and load lazyload
         */
        refresh: function () {
            this._loadFn();
        },

        /**
         * load elements in container if necessary.
         */
        loadItems: function () {
            var me = this,
                container = me.container,
                options = me._options,
                selector = options.lazySelector;

            // container is not document and container is display none
            if (me._containerIsNotDocument && !container.offsetWidth) {
                return;
            }

            // calculate the window and container region.
            me._windowRegion = getBoundingRect();
            if (me._containerIsNotDocument) {
                me._containerRegion = getBoundingRect(container);
            }

            // TODO replace jQuery to native js.
            $(selector, container).each(function (i, el) {
                me.loadItem(el);
            });
        },

        /**
         * load given element if necessary.
         *
         * @param el dom element.
         */
        loadItem: function (el) {
            var me = this,
                options = me._options,
                attr = options.lazyAttribute,
                threshold = options.threshold,
                thresholdX = options.thresholdX || threshold,
                thresholdY = options.thresholdY || threshold,
                loadFn, src, tag, fn;

            // element in window region and container region TODO here.
            if (elementInViewport(el, me._windowRegion, me._containerRegion, thresholdX, thresholdY)) {
                fn = el.tagName.toLowerCase();
                src = el.getAttribute(attr);
                el.removeAttribute(attr);

                if (!src) {
                    console.log('no src configure?', el, fn, src);
                    return;
                }

                fn = 'load' + fn.substring(0, 1).toUpperCase() + fn.substring(1) + 'Fn';
                loadFn = options[fn] || options.loadFn;
                if (!loadFn) {
                    console.log('no "load' + fn + 'Fn" or "loadFn" configuration found.', el, fn, src);
                    return;
                }

                console.log('loading', el, src);
                loadFn.call(me, el, src);
            }
        },

        /**
         * destroy lazyload.
         */
        destroy: function () {
            var me = this;
            me._destroyed = 1;
            me.pause();
        },

        /**
         * pause lazyload
         */
        pause: function () {
            var me = this,
                c = me.container,
                loadFn = me._loadFn;

            // return if destroyed.
            if (me._destroyed) {
                return;
            }

            loadFn.stop();

            $(WIN).off(SCROLL, loadFn);
            $(WIN).off(TOUCH_MOVE, loadFn);
            $(WIN).off(RESIZE, loadFn);

            if (me._containerIsNotDocument) {
                $(c).off(SCROLL, loadFn);
                $(c).off(TOUCH_MOVE, loadFn);
            }
        },

        /**
         * resume lazyload
         */
        resume: function () {
            var me = this,
                c = me.container,
                loadFn = me._loadFn;

            if (me._destroyed) {
                return;
            }

            $(WIN).on(SCROLL, loadFn);
            $(WIN).on(TOUCH_MOVE, loadFn);
            $(WIN).on(RESIZE, loadFn);

            if (me._containerIsNotDocument) {
                $(c).on(SCROLL, loadFn);
                $(c).on(TOUCH_MOVE, loadFn);
            }
        }
    });

    /* *****************************************
     *              HELPER METHOD
     * *************************************** */

    /**
     * get given element region. if no element specified, document will be used.
     *
     * @param elem
     * @returns {{x: *, y: *, w: *, h: *}}
     */
    function getBoundingRect(elem) {
        var x, y, w, h;

        if (undefined !== elem) {
            var offset = $(elem).offset();
            x = offset.left;
            y = offset.top;
            w = $(elem).outerWidth();
            h = $(elem).outerHeight();
        } else {
            var $doc = $(document),
                $win = $(window);
            x = $doc.scrollLeft();
            y = $doc.scrollTop();
            w = $win.innerWidth();
            h = $win.innerHeight();
        }

        return { x: x, y: y, w: w, h: h };
    }

    /**
     * whether the two regions intersect.
     *
     * { x: *, y: *, w: *, h: * }
     * @param r1 first region.
     * @param r2 second region.
     * @returns {boolean}
     */
    function isCrossRect(r1, r2) {
        var r = {};
        r.top = Math.max(r1.y, r2.y);
        r.bottom = Math.min(r1.y + r1.h, r2.y + r2.h);
        r.left = Math.max(r1.x, r2.x);
        r.right = Math.min(r1.x + r1.w, r2.x + r2.w);

        return r.bottom >= r.top && r.right >= r.left;
    }

    /**
     * whether part of element can be seen by user.
     * note: it will not handle display none.
     * @ignore
     */
    function elementInViewport(el, windowRegion, containerRegion, thresholdX, thresholdY) {
        if (!el.offsetWidth) {
            return false;
        }

        var elemOffset = $(el).offset(),
            inContainer = true,
            inWin,
            top = elemOffset.top,
            left = elemOffset.left,
            elemRegion = {
                x: left - (thresholdX || 0),
                y: top - (thresholdY || 0),
                w: $(el).width() + (thresholdX || 0),// cacheWidth(elem),
                h: $(el).height() + (thresholdY || 0)// cacheHeight(elem)
            };

        // element in window region.
        inWin = isCrossRect(windowRegion, elemRegion);

        // element in window region and container region.
        if (inWin && containerRegion) {
            // maybe the container has decode scroll bar, so do this.
            inContainer = isCrossRect(containerRegion, elemRegion);
        }

        return inContainer && inWin;
    }

    /**
     * @type {{proxy: util.proxy, buffer: util.buffer, later: util.later, each: util.each, makeArray: util.makeArray}}
     */
    var util = {
        proxy: function (fn, context, args) {
            var me = this;
            return function () {
                context = context || this;
                return fn.apply(context, me.makeArray(args || arguments));
            };
        },
        /**
         * buffers decode call between decode fixed time
         *
         * @param task
         * @param duration
         * @param context
         * @returns {*}
         */
        buffer: function (task, duration, context) {
            var me = this, timer;

            duration = duration || 150;

            if (-1 === duration) {
                return function () {
                    task.apply(context || this, arguments)
                };
            }

            function run() {
                run.stop(); // clear not executed task
                timer = me.later(task, duration, false, context || this, arguments)
            }

            run.stop = function () {
                timer && (timer.cancel(), timer = 0);
            };

            return run;
        },

        /**
         * schedule invoke
         *
         * @param fn
         * @param when
         * @param interval
         * @param context
         * @param args
         * @returns {{id: number, interval: *, cancel: cancel}}
         */
        later: function (fn, when, interval, context, args) {
            when = when || 0;

            var me = this,
                task = fn,
                b = me.makeArray(args),
                id;

            if ('string' === typeof fn) {
                task = context[fn];
            }

            if (!task) {
                $.error('method undefined')
            }

            fn = function () {
                task.apply(context, b)
            };

            id = interval ? setInterval(fn, when) : setTimeout(fn, when);

            return {
                id: id,
                interval: interval,
                cancel: function () {
                    this.interval ? clearInterval(id) : clearTimeout(id)
                }
            }
        },
        makeArray: function (o) {
            if (null == o) {
                return [];
            }

            if (Object.prototype.toString.call(o) == '[object Array]') {
                return o;
            }

            var lenType = typeof o.length,
                oType = typeof o;

            if ('number' !== lenType ||            // not has 'length'
                'string' === oType ||              // string
                'string' === typeof o.nodeName ||  // select element
                (null != o && o == o.window) ||    // window object
                // https://github.com/ariya/phantomjs/issues/11478
                ('function' === oType && !('item' in o && 'number' === lenType))) {
                return [o];
            }

            var ret = [];
            for (var i = 0, l = o.length; i < l; i++) {
                ret[i] = o[i];
            }
            return ret;
        }
    };



    /**
     * jQuery plugin integration.
     */
    $.fn.extend({
        'lazyload': function (options) {
            this.each(function (i, el) {
                var loader = $.data(el, 'lz-loader');
                if (!loader) {
                    loader = new LazyLoad($.extend({
                        container: this
                    }, options));

                    $.data(el, 'lz-loader', loader);
                }
                return loader;
            });
            return this;
        }
    });
}));