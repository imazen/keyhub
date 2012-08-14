/*
    HOW TO USE:
    $("INPUT[type=checkbox].Label").prettyradiocheckbox();
     $("INPUT[type=checkbox].Label").prettyradiocheckbox( {debug:true} );
   

*/


(function ($) {

    jQuery.fn.prettyradiocheckbox = function (options) {
        var settings = jQuery.extend({ debug: false, group: false }, options || {});

        var addEvents = function (object) {
            var checked = object.checked;
            var disabled = object.disabled;
            var $object = $(object);
            if (object.stateInterval) clearInterval(object.stateInterval);
            object.stateInterval = setInterval(function () {
                if (object.disabled != disabled) $object.trigger((disabled = !!object.disabled) ? 'disable' : 'enable');
                if (object.checked != checked) $object.trigger((checked = !!object.checked) ? 'check' : 'uncheck');
            }, 10);
            return $object;
        };


        return this.each(function () {
            var ch = this;

            /* Reference to DOM Element*/
            var $ch = addEvents(ch);

            /* Adds custom events and returns, jQuery enclosed object */
            /* Removing wrapper if already applied  */
            if (ch.wrapper) ch.wrapper.remove();

            /* Creating wrapper for checkbox and assigning "hover" event */

            ch.wrapper = $('<span class="prettyradiocheckbox ' + $ch.attr('type') + '" id="cb_' + $ch.attr('id') + '"></span>');
            ch.wrapper.hover(function (e) {
                ch.wrapper.addClass('hover');
                CB(e);
            }, function (e) {
                ch.wrapper.removeClass('hover');
                CB(e);
            });

            /* Wrapping checkbox */
            var style = !settings.debug ? { position: 'absolute', zIndex: -1, left: '-10000px' } : { position: 'absolute', left: '0px' };
            $ch.css(style).after(ch.wrapper);


            var label = false;
            if ($ch.attr('id')) {
                label = $('label[for=' + $ch.attr('id') + ']');
                if (!label.length) label = false;
            } if (!label) {
                /* Trying to utilize "closest()" from jQuery 1.3+ */
                label = $ch.closest ? $ch.closest('label') : $ch.parents('label:eq(0)');
                if (!label.length) label = false;
            }/* Labe found, applying event hanlers */

            if (label) {
                label.hover(function (e) {
                    ch.wrapper.trigger('mouseover', [e]);
                }, function (e) { ch.wrapper.trigger('mouseout', [e]); }); label.click(function (e) { $ch.trigger('click', [e]); CB(e); return false; });
            }

    

            ch.wrapper.append($ch);

            ch.wrapper.click(function (e) {
                $ch.trigger('click', [e]);
                CB(e);
                return false;
            });

            $ch.click(function (e) {
                CB(e);
            });


            $ch.bind('disable', function () {
                ch.wrapper.addClass('disabled');
            }).bind('enable', function () {
                ch.wrapper.removeClass('disabled');
            });

            $ch.bind('check', function () {
                ch.wrapper.addClass('checked');
            }).bind('uncheck', function () {
                ch.wrapper.removeClass('checked');
            }).bind('focus', function () {
                ch.wrapper.addClass('focus');
            }).bind('blur', function () {
                ch.wrapper.removeClass('focus');
            });


            /* Firefox antiselection hack */
            if (window.getSelection) ch.wrapper.css('MozUserSelect', 'none');

            /* Applying checkbox state */
            if (ch.checked) ch.wrapper.addClass('checked');
            if (ch.disabled) ch.wrapper.addClass('disabled');

        });

    }

    var CB = function (e) {
        if (!e) var e = window.event;
        e.cancelBubble = true;
        if (e.stopPropagation) e.stopPropagation();
    };

})(jQuery);