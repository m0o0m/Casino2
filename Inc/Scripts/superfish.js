
(function($) {
    $.fn.superfish = function(op) {

        var sf = $.fn.superfish,
			c = sf.c,
			$arrow = $(['<span class="', c.arrowClass, '"> &#187;</span>'].join('')),
			over = function() {
			    var $$ = $(this), menu = getMenu($$);
			    clearTimeout(menu.sfTimer);
			    $$.showSuperfishUl().siblings().hideSuperfishUl();
			},
			out = function() {
			    var $$ = $(this), menu = getMenu($$), o = sf.op;
			    clearTimeout(menu.sfTimer);
			    menu.sfTimer = setTimeout(function() {
			        o.retainPath = ($.inArray($$[0], o.$path) > -1);
			        $$.hideSuperfishUl();
			        if (o.$path.length && $$.parents(['li.', o.hoverClass].join('')).length < 1) { over.call(o.$path); }
			    }, o.delay);
			},
			getMenu = function($menu) {
			    var menu = $menu.parents(['ul.', c.menuClass, ':first'].join(''))[0];
			    sf.op = sf.o[menu.serial];
			    return menu;
			},
			addArrow = function($a) { $a.addClass(c.anchorClass).append($arrow.clone()); };

        return this.each(function() {
            var s = this.serial = sf.o.length;
            var o = $.extend({}, sf.defaults, op);
            o.$path = $('li.' + o.pathClass, this).slice(0, o.pathLevels).each(function() {
                $(this).addClass([o.hoverClass, c.bcClass].join(' '))
					.filter('li:has(ul)').removeClass(o.pathClass);
            });
            sf.o[s] = sf.op = o;

            $('li:has(ul)', this)[($.fn.hoverIntent && !o.disableHI) ? 'hoverIntent' : 'hover'](over, out).each(function() {
                if (o.autoArrows) addArrow($('>a:first-child', this));
            })
			.not('.' + c.bcClass)
				.hideSuperfishUl();

            var $a = $('a', this);
            $a.each(function(i) {
                var $li = $a.eq(i).parents('li');
                $a.eq(i).focus(function() { over.call($li); }).blur(function() { out.call($li); });
            });
            o.onInit.call(this);

        }).each(function() {
            var menuClasses = [c.menuClass];
            if (sf.op.dropShadows && !($.browser.msie && $.browser.version < 7)) menuClasses.push(c.shadowClass);
            $(this).addClass(menuClasses.join(' '));
        });
    };

    var sf = $.fn.superfish;
    sf.o = [];
    sf.op = {};
    sf.IE7fix = function() {
        var o = sf.op;
        if ($.browser.msie && $.browser.version > 6 && o.dropShadows && o.animation.opacity != undefined)
            this.toggleClass(sf.c.shadowClass + '-off');
    };
    sf.c = {
        bcClass: 'sf-breadcrumb',
        menuClass: 'sf-js-enabled',
        anchorClass: 'sf-with-ul',
       // arrowClass: 'sf-sub-indicator',
        shadowClass: 'sf-shadow'
    };
    sf.defaults = {
        hoverClass: 'sfHover',
        pathClass: 'overideThisToUse',
        pathLevels: 1,
        delay: 800,
        animation: { opacity: 'show' },
        speed: 'normal',
       // autoArrows: true,
        dropShadows: true,
        disableHI: false, 	// true disables hoverIntent detection
        onInit: function() { }, // callback functions
        onBeforeShow: function() { },
        onShow: function() { },
        onHide: function() { }
    };
    $.fn.extend({
        hideSuperfishUl: function() {
            var o = sf.op,
				not = (o.retainPath === true) ? o.$path : '';
            o.retainPath = false;
            var $ul = $(['li.', o.hoverClass].join(''), this).add(this).not(not).removeClass(o.hoverClass)
					.find('>ul').hide().css('visibility', 'hidden');
            o.onHide.call($ul);
            return this;
        },
        showSuperfishUl: function() {
            var o = sf.op,
				sh = sf.c.shadowClass + '-on',
				$ul = this.addClass(o.hoverClass)
					.find('>ul:hidden').css('visibility', 'visible');
            sf.IE7fix.call($ul);
            o.onBeforeShow.call($ul);
            $ul.animate(o.animation, o.speed, function() { sf.IE7fix.call($ul); o.onShow.call($ul); });
            return this;
        }
    });

})(jQuery);

$(document).ready(function() {
    if ($('#slider').length == 1) {
        $('#slider').cycle({
            fx: 'fade',
            speed: 2000,
            timeout: 4000
        });
    }

    var ie6 = $.browser.msie && /MSIE 6.0/.test(navigator.userAgent);

    if (!ie6) {

        $('ul.sf-menu').superfish({
            delay: 1000,                            // one second delay on mouseout 
            animation: { opacity: 'show', height: 'show' },  // fade-in and slide-down animation 
            speed: 'normal'                         // faster animation speed 
        });

    }
    else {
        /*DD_belatedPNG.fix('#topbar, img, a, .commentbubble, .infobubble, .view-all, #category-picture1, #category-picture2, #category-picture3, #footer');*/
         DD_belatedPNG.fix('topbar,img,a,.commentbubble,.infobubble,.view-all,category-picture1,category-picture2,category-picture3,category-picture4,category-picture5,category-picture6,footer,.left-article,wrapper,slider,featureditem1,featureditem2,featureditem3,categorywrap,categorywrap-blog,content,sidebar,.name,.email,textarea,.button,.infobutton,blognav,blognav,.textInput,.ZipLookupLoading,.buttonnormal,.buttonhover,blog-home');

    }
});
//]]>