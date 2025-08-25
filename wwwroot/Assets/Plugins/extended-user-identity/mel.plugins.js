Array.prototype.contains = function (item) { return this.indexOf(item) != -1; };
String.prototype.contains = function (it) { return this.indexOf(it) != -1; };
String.prototype.trim = function () { return this.replace(/^\s\s*/, '').replace(/\s\s*$/, '') };
String.prototype.padLeft = function (totalWidth, paddingChar) {
    if (!paddingChar || this.length >= totalWidth) {
        return this;
    }

    var s = this;
    var max = (totalWidth - this.length) / paddingChar.length;
    for (var i = 0; i < max; i++) {
        s = paddingChar + s;
    }

    return s;
};
String.prototype.padRight = function (totalWidth, paddingChar) {
    if (!paddingChar || this.length >= totalWidth) {
        return this;
    }

    var s = this;
    var max = (totalWidth - this.length) / paddingChar.length;
    for (var i = 0; i < max; i++) {
        s += paddingChar;
    }
    return s;
};

if (this.JSON && !this.JSON.parseWithDate) {
   /* var reISO = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/;*/
    var reMsAjax = /^\/Date\((d|-|.*)\)[\/|\\]$/;
    var reISO = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})$/;

    JSON.parseWithDate = function (json) {
        /// <summary>  
        /// parses a JSON string and turns ISO or MSAJAX date strings  
        /// into native JS date objects  
        /// </summary>      
        /// <param name="json" type="var">json with dates to parse</param>          
        /// </param>  
        /// <returns type="value, array or object" />  
        try {
            var res = JSON.parse(json,
            function (key, value) {
                if (typeof value === 'string') {
                    var a = reISO.exec(value);
                    if (a)
                        return new Date(Date.UTC(+a[1], +a[2] - 1,
                                                 +a[3], +a[4], +a[5], +a[6]));
                    a = reMsAjax.exec(value);
                    if (a) {
                        var b = a[1].split(/[-+,.]/);
                        return new Date(b[0] ? +b[0] : 0 - +b[1]);
                    }
                }
                return value;
            });
            return res;
        } catch (e) {
            // orignal error thrown has no error message so rethrow with message  
            throw new Error("JSON content could not be parsed");
            return null;
        }
    };
    JSON.dateStringToDate = function (dtString) {
        /// <summary>  
        /// Converts a JSON ISO or MSAJAX string into a date object  
        /// </summary>      
        /// <param name="" type="var">Date String</param>  
        /// <returns type="date or null if invalid" />   
        var a = reISO.exec(dtString);
        if (a)
            return new Date(Date.UTC(+a[1], +a[2] - 1, +a[3],
                                     +a[4], +a[5], +a[6]));
        a = reMsAjax.exec(dtString);
        if (a) {
            var b = a[1].split(/[-,.]/);
            return new Date(+b[0]);
        }
        return null;
    };
    JSON.stringifyWcf = function (json) {
        /// <summary>  
        /// Wcf specific stringify that encodes dates in the  
        /// a WCF compatible format ("/Date(9991231231)/")  
        /// Note: this format works ONLY with WCF.   
        ///       ASMX can use ISO dates as of .NET 3.5 SP1  
        /// </summary>  
        /// <param name="key" type="var">property name</param>  
        /// <param name="value" type="var">value of the property</param>           
        return JSON.stringify(json, function (key, value) {
            if (typeof value == "string") {
                var a = reISO.exec(value);
                if (a) {
                    var val = '/Date(' +
                              new Date(Date.UTC(+a[1], +a[2] - 1,
                                       +a[3], +a[4],
                                       +a[5], +a[6])).getTime() + ')/';
                    this[key] = val;
                    return val;
                }
            }
            return value;
        })
    };

}


/*
* Natural Sort algorithm for Javascript - Version 0.6 - Released under MIT license
* Author: Jim Palmer (based on chunking idea from Dave Koelle)
* Contributors: Mike Grier (mgrier.com), Clint Priest, Kyle Adams, guillermo
* http://www.overset.com/2008/09/01/javascript-natural-sort-algorithm/
* http://code.google.com/p/js-naturalsort/source/browse/trunk/unit-tests.html
*/
function naturalSort(a, b) {
    var re = /(^-?[0-9]+(\.?[0-9]*)[df]?e?[0-9]?$|^0x[0-9a-f]+$|[0-9]+)/gi,
		sre = /(^[ ]*|[ ]*$)/g,
		dre = /(^([\w ]+,?[\w ]+)?[\w ]+,?[\w ]+\d+:\d+(:\d+)?[\w ]?|^\d{1,4}[\/\-]\d{1,4}[\/\-]\d{1,4}|^\w+, \w+ \d+, \d{4})/,
		hre = /^0x[0-9a-f]+$/i,
		ore = /^0/,
    // convert all to strings and trim()
		x = a.toString().replace(sre, '') || '',
		y = b.toString().replace(sre, '') || '',
    // chunk/tokenize
		xN = x.replace(re, '\0$1\0').replace(/\0$/, '').replace(/^\0/, '').split('\0'),
		yN = y.replace(re, '\0$1\0').replace(/\0$/, '').replace(/^\0/, '').split('\0'),
    // numeric, hex or date detection
		xD = parseInt(x.match(hre)) || (xN.length != 1 && x.match(dre) && Date.parse(x)),
		yD = parseInt(y.match(hre)) || xD && y.match(dre) && Date.parse(y) || null;
    // first try and sort Hex codes or Dates
    if (yD)
        if (xD < yD) return -1;
        else if (xD > yD) return 1;
    // natural sorting through split numeric strings and default strings
    for (var cLoc = 0, numS = Math.max(xN.length, yN.length); cLoc < numS; cLoc++) {
        // find floats not starting with '0', string or 0 if not defined (Clint Priest)
        oFxNcL = !(xN[cLoc] || '').match(ore) && parseFloat(xN[cLoc]) || xN[cLoc] || 0;
        oFyNcL = !(yN[cLoc] || '').match(ore) && parseFloat(yN[cLoc]) || yN[cLoc] || 0;
        // handle numeric vs string comparison - number < string - (Kyle Adams)
        if (isNaN(oFxNcL) !== isNaN(oFyNcL)) return (isNaN(oFxNcL)) ? 1 : -1;
        // rely on string comparison if different types - i.e. '02' < 2 != '02' < '2'
        else if (typeof oFxNcL !== typeof oFyNcL) {
            oFxNcL += '';
            oFyNcL += '';
        }
        if (oFxNcL < oFyNcL) return -1;
        if (oFxNcL > oFyNcL) return 1;
    }
    return 0;
}


// forceNumeric() plug-in implementation
jQuery.fn.forceNumeric = function () {

    return this.each(function () {
        $(this).keydown(function (e) {
            var key = e.which || e.keyCode;

            if (!e.shiftKey && !e.altKey && !e.ctrlKey &&
            // numbers   
                         key >= 48 && key <= 57 ||
            // Numeric keypad
                         key >= 96 && key <= 105 ||
            // comma, period and minus, . on keypad
                        key == 190 || key == 188 || key == 109 || key == 110 ||
            // Backspace and Tab and Enter
                        key == 8 || key == 9 || key == 13 ||
            // Home and End
                        key == 35 || key == 36 ||
            // left and right arrows
                        key == 37 || key == 39 ||
            // Del and Ins
                        key == 46 || key == 45)
                return true;

            return false;
        });
    });
};

jQuery.fn.forceDecimal = function () {

    function validate(elm,e) {
        //if (($(this).attr("class")).indexOf("price") >= 0) {

        var price = String.fromCharCode(e.which);

        //if the letter is not digit then display error and don't type anything
        if (/^[0-9]+\.?[0-9]*$/.test(e.which ? ($(elm).val() + price) : $(elm).val()) == false) {
            $(elm).css("background-color", "rgb(245, 203, 203)");
            //$(elm).css("border", "1px solid #FF0000");
            //$(elm).attr("placeholder", "Enter only numbers");
            return false;
        }
        else {
            $(elm).css("background-color", "#fff");
            //$(elm).removeAttr("placeholder");
        }
        //}
    }

    return this.each(function () {
        $(this).keypress(function (e) {
            return validate(this,e);
        })
        .blur(function (e) {
            if (!$(this).val()) return;
            validate(this,e);
        });
    });
};


/*!
	Autosize 1.18.12
	license: MIT
	http://www.jacklmoore.com/autosize
*/
(function (e) { var t, o = { className: "autosizejs", id: "autosizejs", append: "\n", callback: !1, resizeDelay: 10, placeholder: !0 }, i = '<textarea tabindex="-1" style="position:absolute; top:-999px; left:0; right:auto; bottom:auto; border:0; padding: 0; -moz-box-sizing:content-box; -webkit-box-sizing:content-box; box-sizing:content-box; word-wrap:break-word; height:0 !important; min-height:0 !important; overflow:hidden; transition:none; -webkit-transition:none; -moz-transition:none;"/>', n = ["fontFamily", "fontSize", "fontWeight", "fontStyle", "letterSpacing", "textTransform", "wordSpacing", "textIndent", "whiteSpace"], s = e(i).data("autosize", !0)[0]; s.style.lineHeight = "99px", "99px" === e(s).css("lineHeight") && n.push("lineHeight"), s.style.lineHeight = "", e.fn.autosize = function (i) { return this.length ? (i = e.extend({}, o, i || {}), s.parentNode !== document.body && e(document.body).append(s), this.each(function () { function o() { var t, o = window.getComputedStyle ? window.getComputedStyle(u, null) : !1; o ? (t = u.getBoundingClientRect().width, (0 === t || "number" != typeof t) && (t = parseInt(o.width, 10)), e.each(["paddingLeft", "paddingRight", "borderLeftWidth", "borderRightWidth"], function (e, i) { t -= parseInt(o[i], 10) })) : t = p.width(), s.style.width = Math.max(t, 0) + "px" } function a() { var a = {}; if (t = u, s.className = i.className, s.id = i.id, d = parseInt(p.css("maxHeight"), 10), e.each(n, function (e, t) { a[t] = p.css(t) }), e(s).css(a).attr("wrap", p.attr("wrap")), o(), window.chrome) { var r = u.style.width; u.style.width = "0px", u.offsetWidth, u.style.width = r } } function r() { var e, n; t !== u ? a() : o(), s.value = !u.value && i.placeholder ? (p.attr("placeholder") || "") + i.append : u.value + i.append, s.style.overflowY = u.style.overflowY, n = parseInt(u.style.height, 10), s.scrollTop = 0, s.scrollTop = 9e4, e = s.scrollTop, d && e > d ? (u.style.overflowY = "scroll", e = d) : (u.style.overflowY = "hidden", c > e && (e = c)), e += w, n !== e && (u.style.height = e + "px", f && i.callback.call(u, u), p.trigger("autosize.resized")) } function l() { clearTimeout(h), h = setTimeout(function () { var e = p.width(); e !== g && (g = e, r()) }, parseInt(i.resizeDelay, 10)) } var d, c, h, u = this, p = e(u), w = 0, f = e.isFunction(i.callback), z = { height: u.style.height, overflow: u.style.overflow, overflowY: u.style.overflowY, wordWrap: u.style.wordWrap, resize: u.style.resize }, g = p.width(), y = p.css("resize"); p.data("autosize") || (p.data("autosize", !0), ("border-box" === p.css("box-sizing") || "border-box" === p.css("-moz-box-sizing") || "border-box" === p.css("-webkit-box-sizing")) && (w = p.outerHeight() - p.height()), c = Math.max(parseInt(p.css("minHeight"), 10) - w || 0, p.height()), p.css({ overflow: "hidden", overflowY: "hidden", wordWrap: "break-word" }), "vertical" === y ? p.css("resize", "none") : "both" === y && p.css("resize", "horizontal"), "onpropertychange" in u ? "oninput" in u ? p.on("input.autosize keyup.autosize", r) : p.on("propertychange.autosize", function () { "value" === event.propertyName && r() }) : p.on("input.autosize", r), i.resizeDelay !== !1 && e(window).on("resize.autosize", l), p.on("autosize.resize", r), p.on("autosize.resizeIncludeStyle", function () { t = null, r() }), p.on("autosize.destroy", function () { t = null, clearTimeout(h), e(window).off("resize", l), p.off("autosize").off(".autosize").css(z).removeData("autosize") }), r()) })) : this } })(jQuery || $);


(function ($) {

    $.fn.extend({

        addClassTimeout: function (className, duration, callback) {
            var elements = this;
            setTimeout(function () {
                elements.removeClass(className);
                if (callback) {
                    callback.call(this);
                }
            }, duration);

            return this.each(function () {
                $(this).addClass(className);
            });
        },

        threeDots:function(option){
            return this;
        }
    });



    //re-set all client validation given a jQuery selected form or child
    $.fn.resetValidation = function () {

        var $form = this.closest('form');

        //reset jQuery Validate's internals
        $form.validate().resetForm();

        //reset unobtrusive validation summary, if it exists
        $form.find("[data-valmsg-summary=true]")
            .removeClass("validation-summary-errors")
            .addClass("validation-summary-valid")
            .find("ul").empty();

        //reset unobtrusive field level, if it exists
        $form.find("[data-valmsg-replace]")
            .removeClass("field-validation-error")
            .addClass("field-validation-valid")
            .empty();

        return $form;
    };

    //reset a form given a jQuery selected form or a child
    //by default validation is also reset
    $.fn.formReset = function (resetValidation) {
        var $form = this.closest('form');

        $form[0].reset();

        if (resetValidation == undefined || resetValidation) {
            $form.resetValidation();
        }

        return $form;
    }

})(jQuery);


/**
 GIT : https://github.com/theproductguy/ThreeDots/
 website : http://tpgblog.com/threedots/
           http://tpgblog.com/2009/12/21/threedots-the-jquery-ellipsis-plugin/
*/
(function ($) {
    $.fn.ThreeDots = function (options) {
        var return_value = this;

        // check for new & valid options
        if ((typeof options == 'object') || (options == undefined)) {
            $.fn.ThreeDots.the_selected = this;

            var return_value = $.fn.ThreeDots.update(options);

        }

        return return_value;
    };
    $.fn.ThreeDots.update = function (options) {
        // initialize local variables
        var curr_this, last_word = null;
        var lineh, paddingt, paddingb, innerh, temp_height;
        var curr_text_span, lws; /* last word structure */
        var last_text, three_dots_value, last_del;

        // check for new & valid options
        if ((typeof options == 'object') || (options == undefined)) {

            // then update the settings
            // CURRENTLY, settings are not CONSTRUCTIVE, but merged with the DEFAULTS every time
            $.fn.ThreeDots.c_settings = $.extend({}, $.fn.ThreeDots.settings, options);
            var max_rows = $.fn.ThreeDots.c_settings.max_rows;
            if (max_rows < 1) {
                return $.fn.ThreeDots.the_selected;
            }

            // make sure at least 1 valid delimiter
            var valid_delimiter_exists = false;
            jQuery.each($.fn.ThreeDots.c_settings.valid_delimiters, function (i, curr_del) {
                if (((new String(curr_del)).length == 1)) {
                    valid_delimiter_exists = true;
                }
            });
            if (valid_delimiter_exists == false) {
                return $.fn.ThreeDots.the_selected;
            }

            // process all provided objects
            $.fn.ThreeDots.the_selected.each(function () {

                // element-specific code here
                curr_this = $(this);

                // obtain the text span
                if ($(curr_this).children('.' + $.fn.ThreeDots.c_settings.text_span_class).length == 0) {
                    // if span doesnt exist, then go to next
                    return true;
                }
                curr_text_span = $(curr_this).children('.' + $.fn.ThreeDots.c_settings.text_span_class).get(0);

                // pre-calc fixed components of num_rows
                var nr_fixed = num_rows(curr_this, true);

                // remember where it all began so that we can see if we ended up exactly where we started
                var init_text_span = $(curr_text_span).text();

                // preprocessor
                the_bisector(curr_this, curr_text_span, nr_fixed);
                var init_post_b = $(curr_text_span).text();

                // if the object has been initialized, then user must be calling UPDATE
                // THEREFORE refresh the text area before re-operating
                if ((three_dots_value = $(curr_this).attr('threedots')) != undefined) {
                    $(curr_text_span).text(three_dots_value);
                    $(curr_this).children('.' + $.fn.ThreeDots.c_settings.e_span_class).remove();
                }

                last_text = $(curr_text_span).text();
                if (last_text.length <= 0) {
                    last_text = '';
                }
                $(curr_this).attr('threedots', init_text_span);

                if (num_rows(curr_this, nr_fixed) > max_rows) {
                    // append the ellipsis span & remember the original text
                    curr_ellipsis = $(curr_this).append('<span style="white-space:nowrap" class="'
														+ $.fn.ThreeDots.c_settings.e_span_class + '">'
														+ $.fn.ThreeDots.c_settings.ellipsis_string
														+ '</span>');

                    // remove 1 word at a time UNTIL max_rows
                    while (num_rows(curr_this, nr_fixed) > max_rows) {

                        lws = the_last_word($(curr_text_span).text()); // HERE
                        $(curr_text_span).text(lws.updated_string);
                        last_word = lws.word;
                        last_del = lws.del;

                        if (last_del == null) {
                            break;
                        }
                    } // while (num_rows(curr_this, nr_fixed) > max_rows)

                    // check for super long words
                    if (last_word != null) {
                        var is_dangling = dangling_ellipsis(curr_this, nr_fixed);

                        if ((num_rows(curr_this, nr_fixed) <= max_rows - 1)
							|| (is_dangling)
							|| (!$.fn.ThreeDots.c_settings.whole_word)) {

                            last_text = $(curr_text_span).text();
                            if (lws.del != null) {
                                $(curr_text_span).text(last_text + last_del);
                            }

                            if (num_rows(curr_this, nr_fixed) > max_rows) {
                                // undo what i just did and stop
                                $(curr_text_span).text(last_text);
                            } else {
                                // keep going
                                $(curr_text_span).text($(curr_text_span).text() + last_word);

                                // break up the last word IFF (1) word is longer than a line, OR (2) whole_word == false
                                if ((num_rows(curr_this, nr_fixed) > max_rows + 1)
									|| (!$.fn.ThreeDots.c_settings.whole_word)
									|| (init_post_b == last_word)
									|| is_dangling) {
                                    // remove 1 char at a time until it all fits
                                    while ((num_rows(curr_this, nr_fixed) > max_rows)) {
                                        if ($(curr_text_span).text().length > 0) {
                                            $(curr_text_span).text(
												$(curr_text_span).text().substr(0, $(curr_text_span).text().length - 1)
											);
                                        } else {
                                            /* 
                                            there is no hope for you; you are crazy;
                                            either pick a shorter ellipsis_string OR
                                            use a wider object --- geeze!
                                            */
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // if nothing has changed, remove the ellipsis
                if (init_text_span == $($(curr_this).children('.' + $.fn.ThreeDots.c_settings.text_span_class).get(0)).text()) {
                    $(curr_this).children('.' + $.fn.ThreeDots.c_settings.e_span_class).remove();
                } else {
                    // only add any title text if the ellipsis is visible
                    if (($(curr_this).children('.' + $.fn.ThreeDots.c_settings.e_span_class)).length > 0) {
                        if ($.fn.ThreeDots.c_settings.alt_text_t) {
                            $(curr_this).children('.' + $.fn.ThreeDots.c_settings.text_span_class).attr('title', init_text_span);
                        }

                        if ($.fn.ThreeDots.c_settings.alt_text_e) {
                            $(curr_this).children('.' + $.fn.ThreeDots.c_settings.e_span_class).attr('title', init_text_span);
                        }

                    }
                }
            }); // $.fn.ThreeDots.the_selected.each(function() 
        }

        return $.fn.ThreeDots.the_selected;
    };
    $.fn.ThreeDots.settings = {
        valid_delimiters: [' ', ',', '.'], 	// what defines the bounds of a word to you?
        ellipsis_string: '...',
        max_rows: 2,
        text_span_class: 'ellipsis_text',
        e_span_class: 'threedots_ellipsis',
        whole_word: true,
        allow_dangle: false,
        alt_text_e: false, 				// if true, mouse over of ellipsis displays the full text
        alt_text_t: false  					// if true & if ellipsis displayed, mouse over of text displays the full text
    };
    function dangling_ellipsis(obj, nr_fixed) {
        if ($.fn.ThreeDots.c_settings.allow_dangle == true) {
            return false; // why do when no doing need be done?
        }

        // initialize variables
        var ellipsis_obj = $(obj).children('.' + $.fn.ThreeDots.c_settings.e_span_class).get(0);
        var remember_display = $(ellipsis_obj).css('display');
        var num_rows_before = num_rows(obj, nr_fixed);

        // temporarily hide ellipsis
        $(ellipsis_obj).css('display', 'none');
        var num_rows_after = num_rows(obj, nr_fixed);

        // restore ellipsis
        $(ellipsis_obj).css('display', remember_display);

        if (num_rows_before > num_rows_after) {
            return true; 	// ASSUMPTION: 	removing the ellipsis changed the height
            // 				THEREFORE the ellipsis was on a row all by its lonesome
        } else {
            return false; // nothing dangling here
        }
    }
    function num_rows(obj, cstate) {
        var the_type = typeof cstate;

        if ((the_type == 'object')
			|| (the_type == undefined)) {

            // do the math & return
            return $(obj).height() / cstate.lh;

        } else if (the_type == 'boolean') {
            var lineheight = lineheight_px($(obj));

            return {
                lh: lineheight
            };
        }
    }
    function the_last_word(str) {
        var temp_word_index;
        var v_del = $.fn.ThreeDots.c_settings.valid_delimiters;

        // trim the string
        str = jQuery.trim(str);

        // initialize variables
        var lastest_word_idx = -1;
        var lastest_word = null;
        var lastest_del = null;

        // for all given delimiters, determine which delimiter results in the smallest word cut
        jQuery.each(v_del, function (i, curr_del) {
            if (((new String(curr_del)).length != 1)
				|| (curr_del == null)) {  // implemented to handle IE NULL condition; if only typeof could say CHAR :(
                return false; // INVALID delimiter; must be 1 character in length
            }

            var tmp_word_index = str.lastIndexOf(curr_del);
            if (tmp_word_index != -1) {
                if (tmp_word_index > lastest_word_idx) {
                    lastest_word_idx = tmp_word_index;
                    lastest_word = str.substring(lastest_word_idx + 1);
                    lastest_del = curr_del;
                }
            }
        });

        // return data structure of word reduced string and the last word
        if (lastest_word_idx > 0) {
            return {
                updated_string: jQuery.trim(str.substring(0, lastest_word_idx/*-1*/)),
                word: lastest_word,
                del: lastest_del
            };
        } else { // the lastest word
            return {
                updated_string: '',
                word: jQuery.trim(str),
                del: null
            };
        }
    }
    function lineheight_px(obj) {
        // shhhh... show
        $(obj).append("<div id='temp_ellipsis_div' style='position:absolute; visibility:hidden'>H</div>");
        // measure
        var temp_height = $('#temp_ellipsis_div').height();
        // cut
        $('#temp_ellipsis_div').remove();

        return temp_height;
    }
    function the_bisector(obj, curr_text_span, nr_fixed) {
        var init_text = $(curr_text_span).text();
        var curr_text = init_text;
        var max_rows = $.fn.ThreeDots.c_settings.max_rows;
        var front_half, back_half, front_of_back_half, middle, back_middle;
        var start_index;

        if (num_rows(obj, nr_fixed) <= max_rows) {
            // do nothing
            return;
        } else {
            // zero in on the solution
            start_index = 0;
            curr_length = curr_text.length;

            curr_middle = Math.floor((curr_length - start_index) / 2);
            front_half = init_text.substring(start_index, start_index + curr_middle);
            back_half = init_text.substring(start_index + curr_middle);

            while (curr_middle != 0) {
                $(curr_text_span).text(front_half);

                if (num_rows(obj, nr_fixed) <= (max_rows)) {
                    // text = text + front half of back-half
                    back_middle = Math.floor(back_half.length / 2);
                    front_of_back_half = back_half.substring(0, back_middle);

                    start_index = front_half.length;
                    curr_text = front_half + front_of_back_half;
                    curr_length = curr_text.length;

                    $(curr_text_span).text(curr_text);
                } else {
                    // text = front half (which it already is)
                    curr_text = front_half;
                    curr_length = curr_text.length;
                }

                curr_middle = Math.floor((curr_length - start_index) / 2);
                front_half = init_text.substring(0, start_index + curr_middle);
                back_half = init_text.substring(start_index + curr_middle);
            }
        }
    }

})(jQuery);

/*!
   2:   * jQuery toDictionary() plugin
   3:   *
   4:   * Version 1.2 (11 Apr 2011)
   5:   *
   6:   * Copyright (c) 2011 Robert Koritnik
   7:   * Licensed under the terms of the MIT license
   8:   * http://www.opensource.org/licenses/mit-license.php
   9:   */
 
(function ($) {
 
    // #region String.prototype.format
    // add String prototype format function if it doesn't yet exist
    if ($.isFunction(String.prototype.format) === false)
    {
        String.prototype.format = function () {
            var s = this;
            var i = arguments.length;
            while (i--)
            {
                s = s.replace(new RegExp("\\{" + i + "\\}", "gim"), arguments[i]);
            }
            return s;
        };
    }
    // #endregion
 
    // #region Date.prototype.toISOString
    // add Date prototype toISOString function if it doesn't yet exist
    if ($.isFunction(Date.prototype.toISOString) === false)
    {
        Date.prototype.toISOString = function () {
            var pad = function (n, places) {
                n = n.toString();
                for (var i = n.length; i < places; i++)
                {
                    n = "0" + n;
                }
                return n;
            };
            var d = this;
            return "{0}-{1}-{2}T{3}:{4}:{5}.{6}Z".format(
                d.getUTCFullYear(),
                pad(d.getUTCMonth() + 1, 2),
                pad(d.getUTCDate(), 2),
                pad(d.getUTCHours(), 2),
                pad(d.getUTCMinutes(), 2),
                pad(d.getUTCSeconds(), 2),
                pad(d.getUTCMilliseconds(), 3)
            );
        };
    }
    // #endregion
 
    var _flatten = function (input, output, prefix, includeNulls) {
        if ($.isPlainObject(input))
        {
            for (var p in input)
            {
                if (includeNulls === true || typeof (input[p]) !== "undefined" && input[p] !== null)
                {
                    _flatten(input[p], output, prefix.length > 0 ? prefix + "." + p : p, includeNulls);
                }
            }
        }
        else
        {
            if ($.isArray(input))
            {
                $.each(input, function (index, value) {
                    _flatten(value, output, "{0}[{1}]".format(prefix, index));
                });
                return;
            }
            if (!$.isFunction(input))
            {
                if (input instanceof Date)
                {
                    output.push({ name: prefix, value: input.toISOString() });
                }
                else
                {
                    var val = typeof (input);
                    switch (val)
                    {
                        case "boolean":
                        case "number":
                            val = input;
                            break;
                        case "object":
                            // this property is null, because non-null objects are evaluated in first if branch
                            if (includeNulls !== true)
                            {
                                return;
                            }
                        default:
                            val = input || "";
                    }
                    output.push({ name: prefix, value: val });
                }
            }
        }
    };
 
    $.extend({
        toDictionary: function (data, prefix, includeNulls) {
            /// <summary>Flattens an arbitrary JSON object to a dictionary that Asp.net MVC default model binder understands.</summary>
            /// <param name="data" type="Object">Can either be a JSON object or a function that returns one.</data>
            /// <param name="prefix" type="String" Optional="true">Provide this parameter when you want the output names to be prefixed by something (ie. when flattening simple values).</param>
            /// <param name="includeNulls" type="Boolean" Optional="true">Set this to 'true' when you want null valued properties to be included in result (default is 'false').</param>
 
            // get data first if provided parameter is a function
            data = $.isFunction(data) ? data.call() : data;
 
            // is second argument "prefix" or "includeNulls"
            if (arguments.length === 2 && typeof (prefix) === "boolean")
            {
                includeNulls = prefix;
                prefix = "";
            }
 
            // set "includeNulls" default
            includeNulls = typeof (includeNulls) === "boolean" ? includeNulls : false;
 
            var result = [];
            _flatten(data, result, prefix || "", includeNulls);
 
            return result;
        }
    });
})(jQuery);


//http://jsperf.com/jquery-textfill/14
// original name '$.fn.fontSizePerfect'
$.fn.fillText1 = function (maxFontSize) {
    ///<summary>by eecolella (derived by mekwall)</summary>

    maxFontSize = parseInt(maxFontSize || Number.POSITIVE_INFINITY, 10);

    for (var i = 0, selLength = this.length; i < selLength; i++) {
        var $el = this.eq(i).children(),
                el = $el[0], maxWidth = $el.parent()[0].offsetWidth,
                newSize = +(parseInt(parseInt(getComputedStyle(el).fontSize)) * (maxWidth / el.offsetWidth)).toFixed(),
                size = (maxFontSize > 0 && newSize > maxFontSize) ? maxFontSize : newSize;

        el.style.fontSize = size + 'px';
        while (el.offsetWidth > maxWidth) {
            size--;
            el.style.fontSize = size + 'px';
        }
    }
    return this;
};


//http://jsperf.com/jquery-textfill/14
// original name '$.fn.fastFontSizeFit'
$.fn.fillText2 = function (maxFontSize) {
    ///<summary>by Cesar Mascarenhas (derived by mekwall)</summary>
    var objlen = this.length;
    var objs = new Array();
    for (var i = 0; i < objlen; i++) {
        obj = this.eq(i).children();
        objs[i] = {
            obj: obj,
            parentwidth: obj.parent()[0].offsetWidth,
            width: obj[0].offsetWidth,
            font: parseInt(getComputedStyle(obj[0]).fontSize)
        };
    }
    for (var i = 0; i < objlen; i++) {
        newObjTextSize = objs[i].parentwidth / objs[i].width * objs[i].font;
        newObjTextSize = newObjTextSize * objs[i].parentwidth / (objs[i].parentwidth + newObjTextSize / (objs[i].font / 10));
        if (newObjTextSize >= maxFontSize) {
            newObjTextSize = maxFontSize;
        }
        objs[i].obj[0].style.fontSize = newObjTextSize + "px";
    }
};



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// End of mini plugins /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


/**
* This jQuery plugin displays pagination links inside the selected elements.
*
* @author Gabriel Birke (birke *at* d-scribe *dot* de)
* @version 1.2
* @param {int} maxentries Number of entries to paginate
* @param {Object} opts Several options (see README for documentation)
* @return {Object} jQuery Object
*/
jQuery.fn.paginationMEL = function (maxentries, opts) {
    ///<summary>
    ///<para>This jQuery plugin displays pagination links inside the selected elements.</para>
    ///<para>author Gabriel Birke (birke *at* d-scribe *dot* de)</para>
    ///<para>version 1.2</para>
    ///</summary>
    ///<param name="maxentries" type="number">Number of entries to paginate</param>
    

    opts = jQuery.extend({
        items_per_page: 10,
        num_display_entries: 10,
        current_page: 0,
        num_edge_entries: 0,
        link_to: "#",
        prev_text: "Prev",
        next_text: "Next",
        ellipse_text: "...",
        prev_show_always: true,
        next_show_always: true,
        callback: function () { return false; }
    }, opts || {});

    return this.each(function () {
        /**
        * Calculate the maximum number of pages
        */
        function numPages() {
            return Math.ceil(maxentries / opts.items_per_page);
        }

        /**
        * Calculate start and end point of pagination links depending on 
        * current_page and num_display_entries.
        * @return {Array}
        */
        function getInterval() {
            var ne_half = Math.ceil(opts.num_display_entries / 2);
            var np = numPages();
            var upper_limit = np - opts.num_display_entries;
            var start = current_page > ne_half ? Math.max(Math.min(current_page - ne_half, upper_limit), 0) : 0;
            var end = current_page > ne_half ? Math.min(current_page + ne_half, np) : Math.min(opts.num_display_entries, np);
            return [start, end];
        }

        /**
        * This is the event handling function for the pagination links. 
        * @param {int} page_id The new page number
        */
        function pageSelected(page_id, evt) {
            current_page = page_id;
            drawLinks();
            var continuePropagation = opts.callback(page_id, panel);
            if (!continuePropagation) {
                if (evt.stopPropagation) {
                    evt.stopPropagation();
                }
                else {
                    evt.cancelBubble = true;
                }
            }
            return continuePropagation;
        }

        /**
        * This function inserts the pagination links into the container element
        */
        function drawLinks() {
            panel.empty();
            var interval = getInterval();
            var np = numPages();
            // This helper function returns a handler function that calls pageSelected with the right page_id
            var getClickHandler = function (page_id) {
                return function (evt) { return pageSelected(page_id, evt); }
            }
            // Helper function for generating a single link (or a span tag if it's the current page)
            var appendItem = function (page_id, appendopts) {
                page_id = page_id < 0 ? 0 : (page_id < np ? page_id : np - 1); // Normalize page id to sane value
                appendopts = jQuery.extend({ text: page_id + 1, classes: "" }, appendopts || {});
                if (page_id == current_page) {
                    var lnk = jQuery("<span class='current'>" + (appendopts.text) + "</span>");
                }
                else {
                    var lnk = jQuery("<a>" + (appendopts.text) + "</a>")
						.bind("click", getClickHandler(page_id))
						.attr('href', opts.link_to.replace(/__id__/, page_id));


                }
                if (appendopts.classes) { lnk.addClass(appendopts.classes); }
                panel.append(lnk);
            }
            // Generate "Previous"-Link
            if (opts.prev_text && (current_page > 0 || opts.prev_show_always)) {
                appendItem(current_page - 1, { text: opts.prev_text, classes: "prev" });
            }
            // Generate starting points
            if (interval[0] > 0 && opts.num_edge_entries > 0) {
                var end = Math.min(opts.num_edge_entries, interval[0]);
                for (var i = 0; i < end; i++) {
                    appendItem(i);
                }
                if (opts.num_edge_entries < interval[0] && opts.ellipse_text) {
                    jQuery("<span>" + opts.ellipse_text + "</span>").appendTo(panel);
                }
            }
            // Generate interval links
            for (var i = interval[0]; i < interval[1]; i++) {
                appendItem(i);
            }
            // Generate ending points
            if (interval[1] < np && opts.num_edge_entries > 0) {
                if (np - opts.num_edge_entries > interval[1] && opts.ellipse_text) {
                    jQuery("<span>" + opts.ellipse_text + "</span>").appendTo(panel);
                }
                var begin = Math.max(np - opts.num_edge_entries, interval[1]);
                for (var i = begin; i < np; i++) {
                    appendItem(i);
                }

            }
            // Generate "Next"-Link
            if (opts.next_text && (current_page < np - 1 || opts.next_show_always)) {
                appendItem(current_page + 1, { text: opts.next_text, classes: "next" });
            }
        }

        // Extract current_page from options
        var current_page = opts.current_page;
        // Create a sane value for maxentries and items_per_page
        maxentries = (!maxentries || maxentries < 0) ? 1 : maxentries;
        opts.items_per_page = (!opts.items_per_page || opts.items_per_page < 0) ? 1 : opts.items_per_page;
        // Store DOM element for easy access from all inner functions
        var panel = jQuery(this);
        // Attach control functions to the DOM element 
        this.selectPage = function (page_id) { pageSelected(page_id); }
        this.prevPage = function () {
            if (current_page > 0) {
                pageSelected(current_page - 1);
                return true;
            }
            else {
                return false;
            }
        }
        this.nextPage = function () {
            if (current_page < numPages() - 1) {
                pageSelected(current_page + 1);
                return true;
            }
            else {
                return false;
            }
        }
        // When all initialisation is done, draw the links
        drawLinks();


        /*
            disabled by Ifan
            reason:
            disable trigger that invoke callback function when client initialize the pagination
            // call callback function
            opts.callback(current_page, this);
        */
        // call callback function
        //opts.callback(current_page, this);
    });
}



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// End of Pagination ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




/**
* Copyright (c)2005-2009 Matt Kruse (javascripttoolbox.com)
* 
* Dual licensed under the MIT and GPL licenses. 
* This basically means you can use this code however you want for
* free, but don't claim to have written it yourself!
* Donations always accepted: http://www.JavascriptToolbox.com/donate/
* 
* Please do not link to the .js files on javascripttoolbox.com from
* your site. Copy the files locally to your server instead.
* 
*/
/*
Date functions

//short documentation

Field	Syntax	    Parse	                            Format
Year	yyyy	    4 digits	                        4 digits
Year	yy	        2 digits	                        2 digits
Year	y	        2 or 4 digits	                    4 digits
Month	MMM	        Name or Abbreviation	            Name
Month	NNN	        Abbreviation	                    Abbreviation
Month	MM	        2 digits (01-12)	                2 digits (01-12)
Month	M	        1 or 2 digits (1-12)	            1-2 digits (1-12)
Day of Month	    dd	2 digits (01-31)	            2 digits (01-31)
Day of Month	    d	1 or 2 digits (1-31)	        1-2 digits (1-31)
Day of Week	        EE	Full name (Sunday-Saturday)	    Full name
Day of Week	        E	Abbreviation (Sun-Sat)	        Abbreviation
Hour	hh	        2 digits (01-12)	                2 digits (01-12)
Hour	h	        1 or 2 digits (1-12)	            1-2 digits (1-12)
Hour	HH	        2 digits (00-23)	                2 digits (00-23)
Hour	H	        1 or 2 digits (0-23)	            1-2 digits (0-23)
Hour	KK	        2 digits (00-11)	                2 digits (00-11)
Hour	K	        1 or 2 digits (0-11)	            1-2 digits (0-11)
Hour	kk	        2 digits (01-24)	                2 digits (01-24)
Hour	k	        1 or 2 digits (1-24)	            1-2 digits (1-24)
Minute	mm	        2 digits (00-59)	                2 digits (00-59)
Minute	m	        1 or 2 digits (0-59)	            1-2 digits (0-59)
Second	ss	        2 digits (00-59)	                2 digits (00-59)
Second	s	        1 or 2 digits (0-59)	            1-2 digits (0-59)
AM/PM	a	        AM/am/PM/pm	                        AM/PM

These functions are used to parse, format, and manipulate Date objects.
See documentation and examples at http://www.JavascriptToolbox.com/lib/date/

*/
Date.$VERSION = 1.02;

// Utility function to append a 0 to single-digit numbers
Date.LZ = function (x) { return (x < 0 || x > 9 ? "" : "0") + x };
// Full month names. Change this for local month names
Date.monthNames = new Array('January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December');
// Month abbreviations. Change this for local month names
Date.monthAbbreviations = new Array('Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec');
// Full day names. Change this for local month names
Date.dayNames = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');
// Day abbreviations. Change this for local month names
Date.dayAbbreviations = new Array('Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat');
// Used for parsing ambiguous dates like 1/2/2000 - default to preferring 'American' format meaning Jan 2.
// Set to false to prefer 'European' format meaning Feb 1
Date.preferAmericanFormat = true;

// If the getFullYear() method is not defined, create it
if (!Date.prototype.getFullYear) {
    Date.prototype.getFullYear = function () { var yy = this.getYear(); return (yy < 1900 ? yy + 1900 : yy); };
}

// Parse a string and convert it to a Date object.
// If no format is passed, try a list of common formats.
// If string cannot be parsed, return null.
// Avoids regular expressions to be more portable.
Date.parseString = function (val, format) {
    // If no format is specified, try a few common formats
    if (typeof (format) == "undefined" || format == null || format == "") {
        var generalFormats = new Array('y-M-d', 'MMM d, y', 'MMM d,y', 'y-MMM-d', 'd-MMM-y', 'MMM d', 'MMM-d', 'd-MMM');
        var monthFirst = new Array('M/d/y', 'M-d-y', 'M.d.y', 'M/d', 'M-d');
        var dateFirst = new Array('d/M/y', 'd-M-y', 'd.M.y', 'd/M', 'd-M');
        var checkList = new Array(generalFormats, Date.preferAmericanFormat ? monthFirst : dateFirst, Date.preferAmericanFormat ? dateFirst : monthFirst);
        for (var i = 0; i < checkList.length; i++) {
            var l = checkList[i];
            for (var j = 0; j < l.length; j++) {
                var d = Date.parseString(val, l[j]);
                if (d != null) {
                    return d;
                }
            }
        }
        return null;
    };

    this.isInteger = function (val) {
        for (var i = 0; i < val.length; i++) {
            if ("1234567890".indexOf(val.charAt(i)) == -1) {
                return false;
            }
        }
        return true;
    };
    this.getInt = function (str, i, minlength, maxlength) {
        for (var x = maxlength; x >= minlength; x--) {
            var token = str.substring(i, i + x);
            if (token.length < minlength) {
                return null;
            }
            if (this.isInteger(token)) {
                return token;
            }
        }
        return null;
    };
    val = val + "";
    format = format + "";
    var i_val = 0;
    var i_format = 0;
    var c = "";
    var token = "";
    var token2 = "";
    var x, y;
    var year = new Date().getFullYear();
    var month = 1;
    var date = 1;
    var hh = 0;
    var mm = 0;
    var ss = 0;
    var ampm = "";
    while (i_format < format.length) {
        // Get next token from format string
        c = format.charAt(i_format);
        token = "";
        while ((format.charAt(i_format) == c) && (i_format < format.length)) {
            token += format.charAt(i_format++);
        }
        // Extract contents of value based on format token
        if (token == "yyyy" || token == "yy" || token == "y") {
            if (token == "yyyy") {
                x = 4; y = 4;
            }
            if (token == "yy") {
                x = 2; y = 2;
            }
            if (token == "y") {
                x = 2; y = 4;
            }
            year = this.getInt(val, i_val, x, y);
            if (year == null) {
                return null;
            }
            i_val += year.length;
            if (year.length == 2) {
                if (year > 70) {
                    year = 1900 + (year - 0);
                }
                else {
                    year = 2000 + (year - 0);
                }
            }
        }
        else if (token == "MMM" || token == "NNN") {
            month = 0;
            var names = (token == "MMM" ? (Date.monthNames.concat(Date.monthAbbreviations)) : Date.monthAbbreviations);
            for (var i = 0; i < names.length; i++) {
                var month_name = names[i];
                if (val.substring(i_val, i_val + month_name.length).toLowerCase() == month_name.toLowerCase()) {
                    month = (i % 12) + 1;
                    i_val += month_name.length;
                    break;
                }
            }
            if ((month < 1) || (month > 12)) {
                return null;
            }
        }
        else if (token == "EE" || token == "E") {
            var names = (token == "EE" ? Date.dayNames : Date.dayAbbreviations);
            for (var i = 0; i < names.length; i++) {
                var day_name = names[i];
                if (val.substring(i_val, i_val + day_name.length).toLowerCase() == day_name.toLowerCase()) {
                    i_val += day_name.length;
                    break;
                }
            }
        }
        else if (token == "MM" || token == "M") {
            month = this.getInt(val, i_val, token.length, 2);
            if (month == null || (month < 1) || (month > 12)) {
                return null;
            }
            i_val += month.length;
        }
        else if (token == "dd" || token == "d") {
            date = this.getInt(val, i_val, token.length, 2);
            if (date == null || (date < 1) || (date > 31)) {
                return null;
            }
            i_val += date.length;
        }
        else if (token == "hh" || token == "h") {
            hh = this.getInt(val, i_val, token.length, 2);
            if (hh == null || (hh < 1) || (hh > 12)) {
                return null;
            }
            i_val += hh.length;
        }
        else if (token == "HH" || token == "H") {
            hh = this.getInt(val, i_val, token.length, 2);
            if (hh == null || (hh < 0) || (hh > 23)) {
                return null;
            }
            i_val += hh.length;
        }
        else if (token == "KK" || token == "K") {
            hh = this.getInt(val, i_val, token.length, 2);
            if (hh == null || (hh < 0) || (hh > 11)) {
                return null;
            }
            i_val += hh.length;
            hh++;
        }
        else if (token == "kk" || token == "k") {
            hh = this.getInt(val, i_val, token.length, 2);
            if (hh == null || (hh < 1) || (hh > 24)) {
                return null;
            }
            i_val += hh.length;
            hh--;
        }
        else if (token == "mm" || token == "m") {
            mm = this.getInt(val, i_val, token.length, 2);
            if (mm == null || (mm < 0) || (mm > 59)) {
                return null;
            }
            i_val += mm.length;
        }
        else if (token == "ss" || token == "s") {
            ss = this.getInt(val, i_val, token.length, 2);
            if (ss == null || (ss < 0) || (ss > 59)) {
                return null;
            }
            i_val += ss.length;
        }
        else if (token == "a") {
            if (val.substring(i_val, i_val + 2).toLowerCase() == "am") {
                ampm = "AM";
            }
            else if (val.substring(i_val, i_val + 2).toLowerCase() == "pm") {
                ampm = "PM";
            }
            else {
                return null;
            }
            i_val += 2;
        }
        else {
            if (val.substring(i_val, i_val + token.length) != token) {
                return null;
            }
            else {
                i_val += token.length;
            }
        }
    }
    // If there are any trailing characters left in the value, it doesn't match
    if (i_val != val.length) {
        return null;
    }
    // Is date valid for month?
    if (month == 2) {
        // Check for leap year
        if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0)) { // leap year
            if (date > 29) {
                return null;
            }
        }
        else {
            if (date > 28) {
                return null;
            }
        }
    }
    if ((month == 4) || (month == 6) || (month == 9) || (month == 11)) {
        if (date > 30) {
            return null;
        }
    }
    // Correct hours value
    if (hh < 12 && ampm == "PM") {
        hh = hh - 0 + 12;
    }
    else if (hh > 11 && ampm == "AM") {
        hh -= 12;
    }
    return new Date(year, month - 1, date, hh, mm, ss);
};

// Check if a date string is valid
Date.isValid = function (val, format) {
    return (Date.parseString(val, format) != null);
};

// Check if a date object is before another date object
Date.prototype.isBefore = function (date2) {
    if (date2 == null) {
        return false;
    }
    return (this.getTime() < date2.getTime());
};

// Check if a date object is after another date object
Date.prototype.isAfter = function (date2) {
    if (date2 == null) {
        return false;
    }
    return (this.getTime() > date2.getTime());
};

// Check if two date objects have equal dates and times
Date.prototype.equals = function (date2) {
    if (date2 == null) {
        return false;
    }
    return (this.getTime() == date2.getTime());
};

// Check if two date objects have equal dates, disregarding times
Date.prototype.equalsIgnoreTime = function (date2) {
    if (date2 == null) {
        return false;
    }
    var d1 = new Date(this.getTime()).clearTime();
    var d2 = new Date(date2.getTime()).clearTime();
    return (d1.getTime() == d2.getTime());
};

// Format a date into a string using a given format string
Date.prototype.format = function (format) {
    format = format + "";
    var result = "";
    var i_format = 0;
    var c = "";
    var token = "";

    /*EDITED BY IFAN*/
    //var y = this.getYear() + "";
    var y = this.getFullYear() + "";

    var M = this.getMonth() + 1;
    var d = this.getDate();
    var E = this.getDay();
    var H = this.getHours();
    var m = this.getMinutes();
    var s = this.getSeconds();
    var yyyy, yy, MMM, MM, dd, hh, h, mm, ss, ampm, HH, H, KK, K, kk, k;
    // Convert real date parts into formatted versions
    var value = new Object();
    if (y.length < 4) {
        y = "" + (+y + 1900);
    }
    value["y"] = "" + y;
    value["yyyy"] = y;
    value["yy"] = y.substring(2, 4);
    value["M"] = M;
    value["MM"] = Date.LZ(M);
    value["MMM"] = Date.monthNames[M - 1];
    value["NNN"] = Date.monthAbbreviations[M - 1];
    //value["MMM"] = Date.monthAbbreviations[M - 1];
    //value["MMMM"] = Date.monthNames[M - 1];
    value["d"] = d;
    value["dd"] = Date.LZ(d);
    value["E"] = Date.dayAbbreviations[E];
    value["EE"] = Date.dayNames[E];
    value["H"] = H;
    value["HH"] = Date.LZ(H);
    if (H == 0) {
        value["h"] = 12;
    }
    else if (H > 12) {
        value["h"] = H - 12;
    }
    else {
        value["h"] = H;
    }
    value["hh"] = Date.LZ(value["h"]);
    value["K"] = value["h"] - 1;
    value["k"] = value["H"] + 1;
    value["KK"] = Date.LZ(value["K"]);
    value["kk"] = Date.LZ(value["k"]);
    if (H > 11) {
        value["a"] = "PM";
    }
    else {
        value["a"] = "AM";
    }
    value["m"] = m;
    value["mm"] = Date.LZ(m);
    value["s"] = s;
    value["ss"] = Date.LZ(s);
    while (i_format < format.length) {
        c = format.charAt(i_format);
        token = "";
        while ((format.charAt(i_format) == c) && (i_format < format.length)) {
            token += format.charAt(i_format++);
        }
        if (typeof (value[token]) != "undefined") {
            result = result + value[token];
        }
        else {
            result = result + token;
        }
    }
    return result;
};

// Get the full name of the day for a date
Date.prototype.getDayName = function () {
    return Date.dayNames[this.getDay()];
};

// Get the abbreviation of the day for a date
Date.prototype.getDayAbbreviation = function () {
    return Date.dayAbbreviations[this.getDay()];
};

// Get the full name of the month for a date
Date.prototype.getMonthName = function () {
    return Date.monthNames[this.getMonth()];
};

// Get the abbreviation of the month for a date
Date.prototype.getMonthAbbreviation = function () {
    return Date.monthAbbreviations[this.getMonth()];
};

// Clear all time information in a date object
Date.prototype.clearTime = function () {
    this.setHours(0);
    this.setMinutes(0);
    this.setSeconds(0);
    this.setMilliseconds(0);
    return this;
};

// Add an amount of time to a date. Negative numbers can be passed to subtract time.
Date.prototype.add = function (interval, number) {
    if (typeof (interval) == "undefined" || interval == null || typeof (number) == "undefined" || number == null) {
        return this;
    }
    number = +number;
    if (interval == 'y') { // year
        this.setFullYear(this.getFullYear() + number);
    }
    else if (interval == 'M') { // Month
        this.setMonth(this.getMonth() + number);
    }
    else if (interval == 'd') { // Day
        this.setDate(this.getDate() + number);
    }
    else if (interval == 'w') { // Weekday
        var step = (number > 0) ? 1 : -1;
        while (number != 0) {
            this.add('d', step);
            while (this.getDay() == 0 || this.getDay() == 6) {
                this.add('d', step);
            }
            number -= step;
        }
    }
    else if (interval == 'h') { // Hour
        this.setHours(this.getHours() + number);
    }
    else if (interval == 'm') { // Minute
        this.setMinutes(this.getMinutes() + number);
    }
    else if (interval == 's') { // Second
        this.setSeconds(this.getSeconds() + number);
    }
    return this;
};


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// End of DateTime /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


/**
* Combodate - 1.0.7
* Dropdown date and time picker.
* Converts text input into dropdowns to pick day, month, year, hour, minute and second.
* Uses momentjs as datetime library http://momentjs.com.
* For i18n include corresponding file from https://github.com/timrwood/moment/tree/master/lang 
*
* Confusion at noon and midnight - see http://en.wikipedia.org/wiki/12-hour_clock#Confusion_at_noon_and_midnight
* In combodate: 
* 12:00 pm --> 12:00 (24-h format, midday)
* 12:00 am --> 00:00 (24-h format, midnight, start of day)
* 
* Differs from momentjs parse rules:
* 00:00 pm, 12:00 pm --> 12:00 (24-h format, day not change)
* 00:00 am, 12:00 am --> 00:00 (24-h format, day not change)
* 
* 
* Author: Vitaliy Potapov
* Project page: http://github.com/vitalets/combodate
* Copyright (c) 2012 Vitaliy Potapov. Released under MIT License.
**/
(function ($) {

    var Combodate = function (element, options) {
        this.$element = $(element);
        if (!this.$element.is('input')) {
            $.error('Combodate should be applied to INPUT element');
            return;
        }
        this.options = $.extend({}, $.fn.combodate.defaults, options, this.$element.data());
        this.init();
    };

    Combodate.prototype = {
        constructor: Combodate,
        init: function () {
            this.map = {
                //key   regexp    moment.method
                day: ['D', 'date'],
                month: ['M', 'month'],
                year: ['Y', 'year'],
                hour: ['[Hh]', 'hours'],
                minute: ['m', 'minutes'],
                second: ['s', 'seconds'],
                ampm: ['[Aa]', '']
            };

            this.$widget = $('<span class="combodate"></span>').html(this.getTemplate());

            this.initCombos();

            //update original input on change 
            this.$widget.on('change', 'select', $.proxy(function (e) {
                this.$element.val(this.getValue()).change();
                // update days count if month or year changes
                if (this.options.smartDays) {
                    if ($(e.target).is('.month') || $(e.target).is('.year')) {
                        this.fillCombo('day');
                    }
                }
            }, this));

            this.$widget.find('select').css('width', 'auto');

            // hide original input and insert widget                                       
            this.$element.hide().after(this.$widget);

            // set initial value
            this.setValue(this.$element.val() || this.options.value);
        },

        /*
         Replace tokens in template with <select> elements 
        */
        getTemplate: function () {
            var tpl = this.options.template;
            var customClass = this.options.customClass;

            //first pass
            $.each(this.map, function (k, v) {
                v = v[0];
                var r = new RegExp(v + '+'),
                    token = v.length > 1 ? v.substring(1, 2) : v;

                tpl = tpl.replace(r, '{' + token + '}');
            });

            //replace spaces with &nbsp;
            tpl = tpl.replace(/ /g, '&nbsp;');

            //second pass
            $.each(this.map, function (k, v) {
                v = v[0];
                var token = v.length > 1 ? v.substring(1, 2) : v;

                tpl = tpl.replace('{' + token + '}', '<select class="' + k + ' ' + customClass + '"></select>');
            });

            return tpl;
        },

        /*
         Initialize combos that presents in template 
        */
        initCombos: function () {
            for (var k in this.map) {
                var $c = this.$widget.find('.' + k);
                // set properties like this.$day, this.$month etc.
                this['$' + k] = $c.length ? $c : null;
                // fill with items
                this.fillCombo(k);
            }
        },

        /*
         Fill combo with items 
        */
        fillCombo: function (k) {
            var $combo = this['$' + k];
            if (!$combo) {
                return;
            }

            // define method name to fill items, e.g `fillDays`
            var f = 'fill' + k.charAt(0).toUpperCase() + k.slice(1);
            var items = this[f]();
            var value = $combo.val();

            $combo.empty();
            for (var i = 0; i < items.length; i++) {
                $combo.append('<option value="' + items[i][0] + '">' + items[i][1] + '</option>');
            }

            $combo.val(value);
        },

        /*
         Initialize items of combos. Handles `firstItem` option 
        */
        fillCommon: function (key) {
            var values = [],
                relTime;

            if (this.options.firstItem === 'name') {
                //need both to support moment ver < 2 and  >= 2
                relTime = moment.relativeTime || moment.langData()._relativeTime;
                var header = typeof relTime[key] === 'function' ? relTime[key](1, true, key, false) : relTime[key];
                //take last entry (see momentjs lang files structure) 
                header = header.split(' ').reverse()[0];
                values.push(['', header]);
            } else if (this.options.firstItem === 'empty') {
                values.push(['', '']);
            }
            return values;
        },


        /*
        fill day
        */
        fillDay: function () {
            var items = this.fillCommon('d'), name, i,
                twoDigit = this.options.template.indexOf('DD') !== -1,
                daysCount = 31;

            // detect days count (depends on month and year)
            // originally https://github.com/vitalets/combodate/pull/7
            if (this.options.smartDays && this.$month && this.$year) {
                var month = parseInt(this.$month.val(), 10);
                var year = parseInt(this.$year.val(), 10);

                if (!isNaN(month) && !isNaN(year)) {
                    daysCount = moment([year, month]).daysInMonth();
                }
            }

            for (i = 1; i <= daysCount; i++) {
                name = twoDigit ? this.leadZero(i) : i;
                items.push([i, name]);
            }
            return items;
        },

        /*
        fill month
        */
        fillMonth: function () {
            var items = this.fillCommon('M'), name, i,
                longNames = this.options.template.indexOf('MMMM') !== -1,
                shortNames = this.options.template.indexOf('MMM') !== -1,
                twoDigit = this.options.template.indexOf('MM') !== -1;

            for (i = 0; i <= 11; i++) {
                if (longNames) {
                    //see https://github.com/timrwood/momentjs.com/pull/36
                    name = moment().date(1).month(i).format('MMMM');
                } else if (shortNames) {
                    name = moment().date(1).month(i).format('MMM');
                } else if (twoDigit) {
                    name = this.leadZero(i + 1);
                } else {
                    name = i + 1;
                }
                items.push([i, name]);
            }
            return items;
        },

        /*
        fill year
        */
        fillYear: function () {
            var items = [], name, i,
                longNames = this.options.template.indexOf('YYYY') !== -1;

            for (i = this.options.maxYear; i >= this.options.minYear; i--) {
                name = longNames ? i : (i + '').substring(2);
                items[this.options.yearDescending ? 'push' : 'unshift']([i, name]);
            }

            items = this.fillCommon('y').concat(items);

            return items;
        },

        /*
        fill hour
        */
        fillHour: function () {
            var items = this.fillCommon('h'), name, i,
                h12 = this.options.template.indexOf('h') !== -1,
                h24 = this.options.template.indexOf('H') !== -1,
                twoDigit = this.options.template.toLowerCase().indexOf('hh') !== -1,
                min = h12 ? 1 : 0,
                max = h12 ? 12 : 23;

            for (i = min; i <= max; i++) {
                name = twoDigit ? this.leadZero(i) : i;
                items.push([i, name]);
            }
            return items;
        },

        /*
        fill minute
        */
        fillMinute: function () {
            var items = this.fillCommon('m'), name, i,
                twoDigit = this.options.template.indexOf('mm') !== -1;

            for (i = 0; i <= 59; i += this.options.minuteStep) {
                name = twoDigit ? this.leadZero(i) : i;
                items.push([i, name]);
            }
            return items;
        },

        /*
        fill second
        */
        fillSecond: function () {
            var items = this.fillCommon('s'), name, i,
                twoDigit = this.options.template.indexOf('ss') !== -1;

            for (i = 0; i <= 59; i += this.options.secondStep) {
                name = twoDigit ? this.leadZero(i) : i;
                items.push([i, name]);
            }
            return items;
        },

        /*
        fill ampm
        */
        fillAmpm: function () {
            var ampmL = this.options.template.indexOf('a') !== -1,
                ampmU = this.options.template.indexOf('A') !== -1,
                items = [
                    ['am', ampmL ? 'am' : 'AM'],
                    ['pm', ampmL ? 'pm' : 'PM']
                ];
            return items;
        },

        /*
         Returns current date value from combos. 
         If format not specified - `options.format` used.
         If format = `null` - Moment object returned.
        */
        getValue: function (format) {
            var dt, values = {},
                that = this,
                notSelected = false;

            //getting selected values    
            $.each(this.map, function (k, v) {
                if (k === 'ampm') {
                    return;
                }
                var def = k === 'day' ? 1 : 0;

                values[k] = that['$' + k] ? parseInt(that['$' + k].val(), 10) : def;

                if (isNaN(values[k])) {
                    notSelected = true;
                    return false;
                }
            });

            //if at least one visible combo not selected - return empty string
            if (notSelected) {
                return '';
            }

            //convert hours 12h --> 24h 
            if (this.$ampm) {
                //12:00 pm --> 12:00 (24-h format, midday), 12:00 am --> 00:00 (24-h format, midnight, start of day)
                if (values.hour === 12) {
                    values.hour = this.$ampm.val() === 'am' ? 0 : 12;
                } else {
                    values.hour = this.$ampm.val() === 'am' ? values.hour : values.hour + 12;
                }
            }

            dt = moment([values.year, values.month, values.day, values.hour, values.minute, values.second]);

            //highlight invalid date
            this.highlight(dt);

            format = format === undefined ? this.options.format : format;
            if (format === null) {
                return dt.isValid() ? dt : null;
            } else {
                return dt.isValid() ? dt.format(format) : '';
            }
        },

        setValue: function (value) {
            if (!value) {
                return;
            }

            // parse in strict mode (third param `true`)
            var dt = typeof value === 'string' ? moment(value, this.options.format, true) : moment(value),
                that = this,
                values = {};

            //function to find nearest value in select options
            function getNearest($select, value) {
                var delta = {};
                $select.children('option').each(function (i, opt) {
                    var optValue = $(opt).attr('value'),
                    distance;

                    if (optValue === '') return;
                    distance = Math.abs(optValue - value);
                    if (typeof delta.distance === 'undefined' || distance < delta.distance) {
                        delta = { value: optValue, distance: distance };
                    }
                });
                return delta.value;
            }

            if (dt.isValid()) {
                //read values from date object
                $.each(this.map, function (k, v) {
                    if (k === 'ampm') {
                        return;
                    }
                    values[k] = dt[v[1]]();
                });

                if (this.$ampm) {
                    //12:00 pm --> 12:00 (24-h format, midday), 12:00 am --> 00:00 (24-h format, midnight, start of day)
                    if (values.hour >= 12) {
                        values.ampm = 'pm';
                        if (values.hour > 12) {
                            values.hour -= 12;
                        }
                    } else {
                        values.ampm = 'am';
                        if (values.hour === 0) {
                            values.hour = 12;
                        }
                    }
                }

                $.each(values, function (k, v) {
                    //call val() for each existing combo, e.g. this.$hour.val()
                    if (that['$' + k]) {

                        if (k === 'minute' && that.options.minuteStep > 1 && that.options.roundTime) {
                            v = getNearest(that['$' + k], v);
                        }

                        if (k === 'second' && that.options.secondStep > 1 && that.options.roundTime) {
                            v = getNearest(that['$' + k], v);
                        }

                        that['$' + k].val(v);
                    }
                });

                // update days count
                if (this.options.smartDays) {
                    this.fillCombo('day');
                }

                this.$element.val(dt.format(this.options.format)).change();
            }
        },

        /*
         highlight combos if date is invalid
        */
        highlight: function (dt) {
            if (!dt.isValid()) {
                if (this.options.errorClass) {
                    this.$widget.addClass(this.options.errorClass);
                } else {
                    //store original border color
                    if (!this.borderColor) {
                        this.borderColor = this.$widget.find('select').css('border-color');
                    }
                    this.$widget.find('select').css('border-color', 'red');
                }
            } else {
                if (this.options.errorClass) {
                    this.$widget.removeClass(this.options.errorClass);
                } else {
                    this.$widget.find('select').css('border-color', this.borderColor);
                }
            }
        },

        leadZero: function (v) {
            return v <= 9 ? '0' + v : v;
        },

        destroy: function () {
            this.$widget.remove();
            this.$element.removeData('combodate').show();
        }

        //todo: clear method        
    };

    $.fn.combodate = function (option) {
        var d, args = Array.apply(null, arguments);
        args.shift();

        //getValue returns date as string / object (not jQuery object)
        if (option === 'getValue' && this.length && (d = this.eq(0).data('combodate'))) {
            return d.getValue.apply(d, args);
        }

        return this.each(function () {
            var $this = $(this),
            data = $this.data('combodate'),
            options = typeof option == 'object' && option;
            if (!data) {
                $this.data('combodate', (data = new Combodate(this, options)));
            }
            if (typeof option == 'string' && typeof data[option] == 'function') {
                data[option].apply(data, args);
            }
        });
    };

    $.fn.combodate.defaults = {
        //in this format value stored in original input
        format: 'DD-MM-YYYY HH:mm',
        //in this format items in dropdowns are displayed
        template: 'D / MMM / YYYY   H : mm',
        //initial value, can be `new Date()`    
        value: null,
        minYear: 1970,
        maxYear: 2015,
        yearDescending: true,
        minuteStep: 5,
        secondStep: 1,
        firstItem: 'empty', //'name', 'empty', 'none'
        errorClass: null,
        customClass: '',
        roundTime: true, // whether to round minutes and seconds if step > 1
        smartDays: false // whether days in combo depend on selected month: 31, 30, 28
    };

}(window.jQuery));



/*!
* jQuery blockUI plugin
* Version 2.56.0-2013.01.31
* @requires jQuery v1.7 or later
*
* Examples at: http://malsup.com/jquery/block/
* Copyright (c) 2007-2013 M. Alsup
* Dual licensed under the MIT and GPL licenses:
* http://www.opensource.org/licenses/mit-license.php
* http://www.gnu.org/licenses/gpl.html
*
* Thanks to Amir-Hossein Sobhi for some excellent contributions!
*/

 ;(function () {
    "use strict";

    function setup($) {
        $.fn._fadeIn = $.fn.fadeIn;

        var noOp = $.noop || function () { };

        // this bit is to ensure we don't call setExpression when we shouldn't (with extra muscle to handle
        // retarded userAgent strings on Vista)
        var msie = /MSIE/.test(navigator.userAgent);
        var ie6 = /MSIE 6.0/.test(navigator.userAgent) && !/MSIE 8.0/.test(navigator.userAgent);
        var mode = document.documentMode || 0;
        // var setExpr = msie && (($.browser.version < 8 && !mode) || mode < 8);
        var setExpr = $.isFunction(document.createElement('div').style.setExpression);

        // global $ methods for blocking/unblocking the entire page
        $.blockUI = function (opts) { install(window, opts); };
        $.unblockUI = function (opts) { remove(window, opts); };

        // convenience method for quick growl-like notifications  (http://www.google.com/search?q=growl)
        $.growlUI = function (title, message, timeout, onClose) {
            var $m = $('<div class="growlUI"></div>');
            if (title) $m.append('<h1>' + title + '</h1>');
            if (message) $m.append('<h2>' + message + '</h2>');
            if (timeout === undefined) timeout = 3000;
            $.blockUI({
                message: $m, fadeIn: 700, fadeOut: 1000, centerY: false,
                timeout: timeout, showOverlay: false,
                onUnblock: onClose,
                css: $.blockUI.defaults.growlCSS
            });
        };

        // plugin method for blocking element content
        $.fn.block = function (opts) {
            var fullOpts = $.extend({}, $.blockUI.defaults, opts || {});
            this.each(function () {
                var $el = $(this);
                if (fullOpts.ignoreIfBlocked && $el.data('blockUI.isBlocked'))
                    return;
                $el.unblock({ fadeOut: 0 });
            });

            return this.each(function () {
                if ($.css(this, 'position') == 'static') {
                    this.style.position = 'relative';
                    $(this).data('blockUI.static', true);
                }
                this.style.zoom = 1; // force 'hasLayout' in ie
                install(this, opts);
            });
        };

        // plugin method for unblocking element content
        $.fn.unblock = function (opts) {
            return this.each(function () {
                remove(this, opts);
            });
        };

        $.blockUI.version = 2.56; // 2nd generation blocking at no extra cost!

        // override these in your code to change the default behavior and style
        $.blockUI.defaults = {
            // message displayed when blocking (use null for no message)
            message: '<h1>Please wait...</h1>',

            title: null, 	// title string; only used when theme == true
            draggable: true, // only used when theme == true (requires jquery-ui.js to be loaded)

            theme: false, // set to true to use with jQuery UI themes

            // styles for the message when blocking; if you wish to disable
            // these and use an external stylesheet then do this in your code:
            // $.blockUI.defaults.css = {};
            css: {
                padding: 0,
                margin: 0,
                width: '30%',
                top: '40%',
                left: '35%',
                textAlign: 'center',
                color: '#000',
                border: '3px solid #aaa',
                backgroundColor: '#fff',
                cursor: 'wait'
            },

            // minimal style set used when themes are used
            themedCSS: {
                width: '30%',
                top: '40%',
                left: '35%'
            },

            // styles for the overlay
            overlayCSS: {
                backgroundColor: '#000',
                opacity: 0.6,
                cursor: 'wait'
            },

            // style to replace wait cursor before unblocking to correct issue
            // of lingering wait cursor
            cursorReset: 'default',

            // styles applied when using $.growlUI
            growlCSS: {
                width: '350px',
                top: '10px',
                left: '',
                right: '10px',
                border: 'none',
                padding: '5px',
                opacity: 0.6,
                cursor: 'default',
                color: '#fff',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                'border-radius': '10px'
            },

            // IE issues: 'about:blank' fails on HTTPS and javascript:false is s-l-o-w
            // (hat tip to Jorge H. N. de Vasconcelos)
            /*jshint scripturl:true */
            iframeSrc: /^https/i.test(window.location.href || '') ? 'javascript:false' : 'about:blank',

            // force usage of iframe in non-IE browsers (handy for blocking applets)
            forceIframe: false,

            // z-index for the blocking overlay
            baseZ: 1000,

            // set these to true to have the message automatically centered
            centerX: true, // <-- only effects element blocking (page block controlled via css above)
            centerY: true,

            // allow body element to be stetched in ie6; this makes blocking look better
            // on "short" pages.  disable if you wish to prevent changes to the body height
            allowBodyStretch: true,

            // enable if you want key and mouse events to be disabled for content that is blocked
            bindEvents: true,

            // be default blockUI will supress tab navigation from leaving blocking content
            // (if bindEvents is true)
            constrainTabKey: true,

            // fadeIn time in millis; set to 0 to disable fadeIn on block
            fadeIn: 200,

            // fadeOut time in millis; set to 0 to disable fadeOut on unblock
            fadeOut: 400,

            // time in millis to wait before auto-unblocking; set to 0 to disable auto-unblock
            timeout: 0,

            // disable if you don't want to show the overlay
            showOverlay: true,

            // if true, focus will be placed in the first available input field when
            // page blocking
            focusInput: true,

            // suppresses the use of overlay styles on FF/Linux (due to performance issues with opacity)
            // no longer needed in 2012
            // applyPlatformOpacityRules: true,

            // callback method invoked when fadeIn has completed and blocking message is visible
            onBlock: null,

            // callback method invoked when unblocking has completed; the callback is
            // passed the element that has been unblocked (which is the window object for page
            // blocks) and the options that were passed to the unblock call:
            //	onUnblock(element, options)
            onUnblock: null,

            // callback method invoked when the overlay area is clicked.
            // setting this will turn the cursor to a pointer, otherwise cursor defined in overlayCss will be used.
            onOverlayClick: null,

            // don't ask; if you really must know: http://groups.google.com/group/jquery-en/browse_thread/thread/36640a8730503595/2f6a79a77a78e493#2f6a79a77a78e493
            quirksmodeOffsetHack: 4,

            // class name of the message block
            blockMsgClass: 'blockMsg',

            // if it is already blocked, then ignore it (don't unblock and reblock)
            ignoreIfBlocked: false
        };

        // private data and functions follow...

        var pageBlock = null;
        var pageBlockEls = [];

        function install(el, opts) {
            var css, themedCSS;
            var full = (el == window);
            var msg = (opts && opts.message !== undefined ? opts.message : undefined);
            opts = $.extend({}, $.blockUI.defaults, opts || {});

            if (opts.ignoreIfBlocked && $(el).data('blockUI.isBlocked'))
                return;

            opts.overlayCSS = $.extend({}, $.blockUI.defaults.overlayCSS, opts.overlayCSS || {});
            css = $.extend({}, $.blockUI.defaults.css, opts.css || {});
            if (opts.onOverlayClick)
                opts.overlayCSS.cursor = 'pointer';

            themedCSS = $.extend({}, $.blockUI.defaults.themedCSS, opts.themedCSS || {});
            msg = msg === undefined ? opts.message : msg;

            // remove the current block (if there is one)
            if (full && pageBlock)
                remove(window, { fadeOut: 0 });

            // if an existing element is being used as the blocking content then we capture
            // its current place in the DOM (and current display style) so we can restore
            // it when we unblock
            if (msg && typeof msg != 'string' && (msg.parentNode || msg.jquery)) {
                var node = msg.jquery ? msg[0] : msg;
                var data = {};
                $(el).data('blockUI.history', data);
                data.el = node;
                data.parent = node.parentNode;
                data.display = node.style.display;
                data.position = node.style.position;
                if (data.parent)
                    data.parent.removeChild(node);
            }

            $(el).data('blockUI.onUnblock', opts.onUnblock);
            var z = opts.baseZ;

            // blockUI uses 3 layers for blocking, for simplicity they are all used on every platform;
            // layer1 is the iframe layer which is used to supress bleed through of underlying content
            // layer2 is the overlay layer which has opacity and a wait cursor (by default)
            // layer3 is the message content that is displayed while blocking
            var lyr1, lyr2, lyr3, s;
            if (msie || opts.forceIframe)
                lyr1 = $('<iframe class="blockUI" style="z-index:' + (z++) + ';display:none;border:none;margin:0;padding:0;position:absolute;width:100%;height:100%;top:0;left:0" src="' + opts.iframeSrc + '"></iframe>');
            else
                lyr1 = $('<div class="blockUI" style="display:none"></div>');

            if (opts.theme)
                lyr2 = $('<div class="blockUI blockOverlay ui-widget-overlay" style="z-index:' + (z++) + ';display:none"></div>');
            else
                lyr2 = $('<div class="blockUI blockOverlay" style="z-index:' + (z++) + ';display:none;border:none;margin:0;padding:0;width:100%;height:100%;top:0;left:0"></div>');

            if (opts.theme && full) {
                s = '<div class="blockUI ' + opts.blockMsgClass + ' blockPage ui-dialog ui-widget ui-corner-all" style="z-index:' + (z + 10) + ';display:none;position:fixed">';
                if (opts.title) {
                    s += '<div class="ui-widget-header ui-dialog-titlebar ui-corner-all blockTitle">' + (opts.title || '&nbsp;') + '</div>';
                }
                s += '<div class="ui-widget-content ui-dialog-content"></div>';
                s += '</div>';
            }
            else if (opts.theme) {
                s = '<div class="blockUI ' + opts.blockMsgClass + ' blockElement ui-dialog ui-widget ui-corner-all" style="z-index:' + (z + 10) + ';display:none;position:absolute">';
                if (opts.title) {
                    s += '<div class="ui-widget-header ui-dialog-titlebar ui-corner-all blockTitle">' + (opts.title || '&nbsp;') + '</div>';
                }
                s += '<div class="ui-widget-content ui-dialog-content"></div>';
                s += '</div>';
            }
            else if (full) {
                s = '<div class="blockUI ' + opts.blockMsgClass + ' blockPage" style="z-index:' + (z + 10) + ';display:none;position:fixed"></div>';
            }
            else {
                s = '<div class="blockUI ' + opts.blockMsgClass + ' blockElement" style="z-index:' + (z + 10) + ';display:none;position:absolute"></div>';
            }
            lyr3 = $(s);

            // if we have a message, style it
            if (msg) {
                if (opts.theme) {
                    lyr3.css(themedCSS);
                    lyr3.addClass('ui-widget-content');
                }
                else
                    lyr3.css(css);
            }

            // style the overlay
            if (!opts.theme /*&& (!opts.applyPlatformOpacityRules)*/)
                lyr2.css(opts.overlayCSS);
            lyr2.css('position', full ? 'fixed' : 'absolute');

            // make iframe layer transparent in IE
            if (msie || opts.forceIframe)
                lyr1.css('opacity', 0.0);

            //$([lyr1[0],lyr2[0],lyr3[0]]).appendTo(full ? 'body' : el);
            var layers = [lyr1, lyr2, lyr3], $par = full ? $('body') : $(el);
            $.each(layers, function () {
                this.appendTo($par);
            });

            if (opts.theme && opts.draggable && $.fn.draggable) {
                lyr3.draggable({
                    handle: '.ui-dialog-titlebar',
                    cancel: 'li'
                });
            }

            // ie7 must use absolute positioning in quirks mode and to account for activex issues (when scrolling)
            var expr = setExpr && (!$.support.boxModel || $('object,embed', full ? null : el).length > 0);
            if (ie6 || expr) {
                // give body 100% height
                if (full && opts.allowBodyStretch && $.support.boxModel)
                    $('html,body').css('height', '100%');

                // fix ie6 issue when blocked element has a border width
                if ((ie6 || !$.support.boxModel) && !full) {
                    var t = sz(el, 'borderTopWidth'), l = sz(el, 'borderLeftWidth');
                    var fixT = t ? '(0 - ' + t + ')' : 0;
                    var fixL = l ? '(0 - ' + l + ')' : 0;
                }

                // simulate fixed position
                $.each(layers, function (i, o) {
                    var s = o[0].style;
                    s.position = 'absolute';
                    if (i < 2) {
                        if (full)
                            s.setExpression('height', 'Math.max(document.body.scrollHeight, document.body.offsetHeight) - (jQuery.support.boxModel?0:' + opts.quirksmodeOffsetHack + ') + "px"');
                        else
                            s.setExpression('height', 'this.parentNode.offsetHeight + "px"');
                        if (full)
                            s.setExpression('width', 'jQuery.support.boxModel && document.documentElement.clientWidth || document.body.clientWidth + "px"');
                        else
                            s.setExpression('width', 'this.parentNode.offsetWidth + "px"');
                        if (fixL) s.setExpression('left', fixL);
                        if (fixT) s.setExpression('top', fixT);
                    }
                    else if (opts.centerY) {
                        if (full) s.setExpression('top', '(document.documentElement.clientHeight || document.body.clientHeight) / 2 - (this.offsetHeight / 2) + (blah = document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop) + "px"');
                        s.marginTop = 0;
                    }
                    else if (!opts.centerY && full) {
                        var top = (opts.css && opts.css.top) ? parseInt(opts.css.top, 10) : 0;
                        var expression = '((document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop) + ' + top + ') + "px"';
                        s.setExpression('top', expression);
                    }
                });
            }

            // show the message
            if (msg) {
                if (opts.theme)
                    lyr3.find('.ui-widget-content').append(msg);
                else
                    lyr3.append(msg);
                if (msg.jquery || msg.nodeType)
                    $(msg).show();
            }

            if ((msie || opts.forceIframe) && opts.showOverlay)
                lyr1.show(); // opacity is zero
            if (opts.fadeIn) {
                var cb = opts.onBlock ? opts.onBlock : noOp;
                var cb1 = (opts.showOverlay && !msg) ? cb : noOp;
                var cb2 = msg ? cb : noOp;
                if (opts.showOverlay)
                    lyr2._fadeIn(opts.fadeIn, cb1);
                if (msg)
                    lyr3._fadeIn(opts.fadeIn, cb2);
            }
            else {
                if (opts.showOverlay)
                    lyr2.show();
                if (msg)
                    lyr3.show();
                if (opts.onBlock)
                    opts.onBlock();
            }

            // bind key and mouse events
            bind(1, el, opts);

            if (full) {
                pageBlock = lyr3[0];
                pageBlockEls = $(':input:enabled:visible', pageBlock);
                if (opts.focusInput)
                    setTimeout(focus, 20);
            }
            else
                center(lyr3[0], opts.centerX, opts.centerY);

            if (opts.timeout) {
                // auto-unblock
                var to = setTimeout(function () {
                    if (full)
                        $.unblockUI(opts);
                    else
                        $(el).unblock(opts);
                }, opts.timeout);
                $(el).data('blockUI.timeout', to);
            }
        }

        // remove the block
        function remove(el, opts) {
            var full = (el == window);
            var $el = $(el);
            var data = $el.data('blockUI.history');
            var to = $el.data('blockUI.timeout');
            if (to) {
                clearTimeout(to);
                $el.removeData('blockUI.timeout');
            }
            opts = $.extend({}, $.blockUI.defaults, opts || {});
            bind(0, el, opts); // unbind events

            if (opts.onUnblock === null) {
                opts.onUnblock = $el.data('blockUI.onUnblock');
                $el.removeData('blockUI.onUnblock');
            }

            var els;
            if (full) // crazy selector to handle odd field errors in ie6/7
                els = $('body').children().filter('.blockUI').add('body > .blockUI');
            else
                els = $el.find('>.blockUI');

            // fix cursor issue
            if (opts.cursorReset) {
                if (els.length > 1)
                    els[1].style.cursor = opts.cursorReset;
                if (els.length > 2)
                    els[2].style.cursor = opts.cursorReset;
            }

            if (full)
                pageBlock = pageBlockEls = null;

            if (opts.fadeOut) {
                els.fadeOut(opts.fadeOut);
                setTimeout(function () { reset(els, data, opts, el); }, opts.fadeOut);
            }
            else
                reset(els, data, opts, el);
        }

        // move blocking element back into the DOM where it started
        function reset(els, data, opts, el) {
            var $el = $(el);
            els.each(function (i, o) {
                // remove via DOM calls so we don't lose event handlers
                if (this.parentNode)
                    this.parentNode.removeChild(this);
            });

            if (data && data.el) {
                data.el.style.display = data.display;
                data.el.style.position = data.position;
                if (data.parent)
                    data.parent.appendChild(data.el);
                $el.removeData('blockUI.history');
            }

            if ($el.data('blockUI.static')) {
                $el.css('position', 'static'); // #22
            }

            if (typeof opts.onUnblock == 'function')
                opts.onUnblock(el, opts);

            // fix issue in Safari 6 where block artifacts remain until reflow
            var body = $(document.body), w = body.width(), cssW = body[0].style.width;
            body.width(w - 1).width(w);
            body[0].style.width = cssW;
        }

        // bind/unbind the handler
        function bind(b, el, opts) {
            var full = el == window, $el = $(el);

            // don't bother unbinding if there is nothing to unbind
            if (!b && (full && !pageBlock || !full && !$el.data('blockUI.isBlocked')))
                return;

            $el.data('blockUI.isBlocked', b);

            // don't bind events when overlay is not in use or if bindEvents is false
            if (!opts.bindEvents || (b && !opts.showOverlay))
                return;

            // bind anchors and inputs for mouse and key events
            var events = 'mousedown mouseup keydown keypress keyup touchstart touchend touchmove';
            if (b)
                $(document).bind(events, opts, handler);
            else
                $(document).unbind(events, handler);

            // former impl...
            //		var $e = $('a,:input');
            //		b ? $e.bind(events, opts, handler) : $e.unbind(events, handler);
        }

        // event handler to suppress keyboard/mouse events when blocking
        function handler(e) {
            // allow tab navigation (conditionally)
            if (e.keyCode && e.keyCode == 9) {
                if (pageBlock && e.data.constrainTabKey) {
                    var els = pageBlockEls;
                    var fwd = !e.shiftKey && e.target === els[els.length - 1];
                    var back = e.shiftKey && e.target === els[0];
                    if (fwd || back) {
                        setTimeout(function () { focus(back); }, 10);
                        return false;
                    }
                }
            }
            var opts = e.data;
            var target = $(e.target);
            if (target.hasClass('blockOverlay') && opts.onOverlayClick)
                opts.onOverlayClick();

            // allow events within the message content
            if (target.parents('div.' + opts.blockMsgClass).length > 0)
                return true;

            // allow events for content that is not being blocked
            return target.parents().children().filter('div.blockUI').length === 0;
        }

        function focus(back) {
            if (!pageBlockEls)
                return;
            var e = pageBlockEls[back === true ? pageBlockEls.length - 1 : 0];
            if (e)
                e.focus();
        }

        function center(el, x, y) {
            var p = el.parentNode, s = el.style;
            var l = ((p.offsetWidth - el.offsetWidth) / 2) - sz(p, 'borderLeftWidth');
            var t = ((p.offsetHeight - el.offsetHeight) / 2) - sz(p, 'borderTopWidth');
            if (x) s.left = l > 0 ? (l + 'px') : '0';
            if (y) s.top = t > 0 ? (t + 'px') : '0';
        }

        function sz(el, p) {
            return parseInt($.css(el, p), 10) || 0;
        }

    }


    /*global define:true */
    if (typeof define === 'function' && define.amd && define.amd.jQuery) {
        define(['jquery'], setup);
    } else {
        setup(jQuery);
    }

})();
