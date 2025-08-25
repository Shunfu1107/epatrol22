/// <reference path="../jquery/dist/jquery.js" />
/// <reference path="../jquery-validation/dist/jquery.validate.js" />
/// <reference path="../jquery-validation-unobtrusive/jquery.validate.unobtrusive.js" />
/// <reference path="../bootbox/dist/bootbox.js" />

if (typeof MEL == 'undefined') MEL = {};


String.prototype.replaceAll = function (find, replace) {
    return this.replace(new RegExp(find, 'g'), replace);
}
String.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }

    return s;
}
Object.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var arg = arguments[i + 1];
        for (prop in arg) {
            var reg = new RegExp("\\[" + prop + "\\]", "gm");
            s = s.replace(reg, arg[prop]);
        }
    }
    return s;
}

MEL.tableToExcel = function (table, name, filename) {
    var uri = 'data:application/vnd.ms-excel;base64,'
        , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--><meta http-equiv="content-type" content="text/plain; charset=UTF-8"/></head><body><table>{table}</table></body></html>'
        , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
        , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }

    //if (!table.nodeType) table = document.getElementById(table)
    //var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML }
    var ctx = { worksheet: name || 'Worksheet', table: table }

    var tagtrigger = $('#mel-table-to-excel-dlink');
    if (!tagtrigger.length) {

        tagtrigger = $('<a id="mel-table-to-excel-dlink"  style="display:none;"></a>');
        $('body').append(tagtrigger);
    }

    document.getElementById("mel-table-to-excel-dlink").href = uri + base64(format(template, ctx));
    document.getElementById("mel-table-to-excel-dlink").download = filename;
    document.getElementById("mel-table-to-excel-dlink").click();

}
MEL.getByValue = function (list, propertyName, value, isCaseSensitive) {
    ///<summary>Get object by its value of the property</summary>
    ///<param name="list" type="Array">List of object that has a propertyName property</param>
    ///<param name="propertyName" type="string">Name of property of the object</param>
    ///<param name="value" type="string">Value of the property to be compared</param>
    ///<param name="isCaseSensitive" type="bool">Used to do comparison, default is false</param>

    if (!list || !list.length) return null;
    if (typeof isCaseSensitive !== 'boolean') isCaseSensitive = false;
    var props = propertyName.split('.');
    for (var i = 0; i < list.length; i++) {
        var temp1 = list[i];
        var obj = temp1;
        for (var y = 0; y < props.length ; y++) {
            obj = obj[props[y]];
            if (typeof obj[props[y + 1]] === 'undefined') {
                /*
                if( isCaseSensitive && typeof obj !== 'undefined' && obj == value){
                    return temp1;
                }
                else if(typeof obj === 'string' && obj.toLowerCase() == value.toLowerCase()){
                    return temp1
                }
                else if( !isCaseSensitive && typeof obj === 'number' && obj == parseInt(value)){
                    return temp1
                }*/

                var typestring = typeof obj;
                switch (typestring) {
                    case 'number':
                        if (obj == parseFloat(value)) return temp1;
                        break;
                    case 'string':
                        if (isCaseSensitive && obj == value) return temp1;
                        else if (obj.toLowerCase() == value.toString().toLowerCase()) return temp1;
                        break;
                    default:
                        if (obj == value) return temp1;
                        break;
                }
            }
        }
    }
    return null;
};//getByValue
MEL.toDigitGrouping = function (input) {
    var nStr = input.value + '';
    nStr = nStr.replace(/\,/g, "");
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    input.value = x1 + x2;
}
MEL.toStringMoney = function (money) {
    var nStr = money + '';
    nStr = nStr.replace(/\,/g, "");
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return (x1 + x2);
}
MEL.toClass = function (selector) {
    return String.format('.{0}', selector);
}
MEL.toSelector = function (tagId) {
    ///<summary>Convert html id to jquery selector id</summary>
    return tagId && tagId.contains('#') ? tagId : String.format('#{0}', tagId);
}
MEL.log = function (text) {
    if (typeof console === 'undefined') return;
    /*console.log(text);*/
};//MEL.log
MEL.addAntiForgeryToken = function (dto, formselector) {
    dto.__RequestVerificationToken = $(formselector).find('input[name=__RequestVerificationToken]').val();
    return data;
}
MEL.toDTO = function (sFormselector) {
    var dto = {};
    var arr = $(sFormselector).serializeArray();
    for (var i = 0; i < arr.length; i++) {
        dto[arr[i].name] = arr[i].value;
    }

    return dto;
}
MEL.load = function (opt) {
    if (!MEL.core.loaded) {
        MEL.startup(function () {
            this.load(opt);
        });
        return;
    }

    MEL.core.load(opt);
}
MEL.loadPermissions = function (fn, preinitdata) {
    var list = []
        , rootList = []
        , danglingList = []
        , missingList = []
        ;

    if (typeof fn !== 'function') return;


    function addContentToRoot(content) {
        /*console.log(content);*/
        if (!content.obj.associatedKey) return;
        var root = content;
        while (root) {
            if (!root.obj.associatedKey) {
                root.allContents.push(content);
                break;
            }
            root = root.parent;
        }

        content.root = root;
    }
    function parsePermission() {
        /*console.log(list);*/
        //iterate all the roots
        for (var i = 0; i < list.length; i++) {
            var cpa = list[i];//ContentPermissionAttribute

            if (!cpa.associatedKey) {

                rootList[cpa.key] = {
                    parent: null,
                    key: cpa.key,
                    level: 1,

                    //data: cpa,
                    //children:[]

                    obj: cpa,
                    contents: [],
                    allContents: []
                };
                rootList.push(rootList[cpa.key]);
            }

            else if (rootList[cpa.associatedKey]) {

                var temproot1 = rootList[cpa.associatedKey];
                rootList[cpa.key] = {
                    parent: temproot1,
                    key: cpa.key,
                    level: (temproot1.level + 1),

                    //data: cpa,
                    //children:[]

                    obj: cpa,
                    contents: []
                };
                //rootList.push(rootList[cpa.Key]);
                addContentToRoot(rootList[cpa.key]);
                temproot1.contents.push(rootList[cpa.key]);
            }
            else {
                danglingList.push(cpa);
            }

           
        }

        //iterate throught the dangling
        for (var i = 0; i < danglingList.length; i++) {
            var cpa = danglingList[i];
            if (rootList[cpa.associatedKey]) {
                var temproot1 = rootList[cpa.associatedKey];
                rootList[cpa.key] = {
                    parent: temproot1,
                    key: cpa.key,
                    level: (temproot1.level + 1),

                    //data: cpa,
                    //children:[]

                    obj: cpa,
                    contents: []
                };
                //rootList.push(rootList[cpa.Key]);
                addContentToRoot(rootList[cpa.key]);
                temproot1.contents.push(rootList[cpa.key]);
            }
            else {
                missingList.push(cpa);
            }
        }

        //put all missing content root in one content
        if (missingList['missing']) missingList['missing'] = { contents: [] };
        for (var i = 0; i < missingList.length; i++) {
            missingList['missing'].contents.push(missingList[i]);
        }

    }

    if (preinitdata) {
        list = preinitdata;
        /*console.log(list);*/
        parsePermission();
        fn.call(this, rootList);
        return;
    }

    MEL.load({
        url: '/Account/GetPermissions',
        callback: function (data) {
            list = data;
            parsePermission();
            fn.call(this, rootList);

            /*console.log(list);*/
        }
    });
    
};
MEL.getTimeDifference = function (earlierDate, laterDate) {
    var nTotalDiff = laterDate.getTime() - earlierDate.getTime();
    var oDiff = new Object();

    oDiff.days = Math.floor(nTotalDiff / 1000 / 60 / 60 / 24);
    nTotalDiff -= oDiff.days * 1000 * 60 * 60 * 24;

    oDiff.hours = Math.floor(nTotalDiff / 1000 / 60 / 60);
    nTotalDiff -= oDiff.hours * 1000 * 60 * 60;

    oDiff.minutes = Math.floor(nTotalDiff / 1000 / 60);
    nTotalDiff -= oDiff.minutes * 1000 * 60;

    oDiff.seconds = Math.floor(nTotalDiff / 1000);

    return oDiff;
};

MEL.getActiveLocation = function () {
    if (typeof MEL == 'undefined' || typeof MEL.core == 'undefined') throw 'Calling MEL library before initializing.';
    return MEL.core.getActiveLocation();
};
MEL.getLocations = function () {
    if (typeof MEL == 'undefined' || typeof MEL.core == 'undefined') throw 'Calling MEL library before initializing';
    return MEL.core.getLocations();
};
MEL.startup = function (fn) {

    
    if (typeof MEL.core == 'undefined') MEL.core = MEL._core();

    if (fn) {
        MEL.core.fnStartup.push({
            called: false,
            fn: fn
        });
    }

    MEL.core.initialize();
};
MEL.option = function (opt) {
    if (typeof MEL.core == 'undefined') {
        MEL.core = MEL._core();
    }
    MEL.core.option(opt);
};
MEL.validate = function () {
    MEL.startup(function () {
        this.validate();
    });

};

MEL.loading = function (text, opt) {
    /* opt config list :
        - target : elementSelector to which the alert box belongs to (default is body)
        - size:{width,height}
        - dialogCss: css object
    */

    var tagContainer = [];
    var tagId = [];// $(MEL.toSelector(this.Id));
    var classIdentifier = 'mel-progress-loading-notify';
    var _opt = opt;
    var REPO_ID = '~loadNX';

    if (typeof text === 'undefined' && typeof opt === 'undefined') hide();
    else show();

    function clear() {
        var old_target = MEL.repo(REPO_ID);
        if (old_target) {
            if ($(old_target).is($('body'))) $.unblockUI();
            else $(old_target).unblock();
        }
    }
    function show() {
        $('.' + classIdentifier).empty().remove();
        if (typeof _opt === 'undefined') _opt = {};

        validateOpt();

        clear();
        if (!tagId.length) render();

        if ($(_opt.target).is($('body'))) {
            $.blockUI({
                message: tagId
                , overlayCSS: { backgroundColor: 'rgba(0,0,0,.8)', opacity: .8 }
                , css: {
                    //border: '1px solid #bbb'
                    backgroundColor: 'rgba(0,0,0,.6)',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    color: '#DDD',
                    'font-size':'1.5em'
                }
                //,css: {
                //    width: _opt.size.width+'px'
                //}
            });
        }
        else {
            //$.blockUI({
            $(_opt.target).block({
                message: tagId
                , overlayCSS: { backgroundColor: 'rgba(0,0,0,.8)', opacity: .8 }
                , css: {
                    //border: '1px solid #bbb',
                    width: _opt.size.width + 'px',
                    height: _opt.size.height + 'px',
                    backgroundColor: 'rgba(0,0,0,.6)',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                }
                //,css: {
                //    width: _opt.size.width+'px'
                //}
            });
        }

        MEL.repo(REPO_ID, _opt.target);
        $('.loading-text').find('span').html(text);
        tagId.css('display', '');

    }
    function hide() {
        //$.unblockUI();
        clear();

        if (!tagId.length) return;
        tagId.css('display', 'none');
    }
    function validateOpt() {

        if (typeof _opt.target === 'undefined' || !$(_opt.target).length) _opt.target = 'body';

        if (typeof _opt.size === 'undefined' || typeof _opt.size.width === 'undefined') {
            _opt.size = {};
            _opt.size.width = 300;
        }

        if (typeof _opt.size === 'undefined' || typeof _opt.size.height === 'undefined')
            _opt.size.height = 40;

        if (_opt.size.width <= 200) _opt.size.width = 200;
        if (_opt.size.height <= 150) _opt.size.height = 150;

    };//validateOpt
    function render() {
        //MEL.track((new Date).format('hh:mm:ss')+' alert.render called..');
        tagContainer = $(_opt.target);
        tagContainer.find(' > .' + classIdentifier).empty();

        if (tagContainer.is($('body'))) tagId = $(document.createElement('div')).addClass(classIdentifier).css('display', 'none').appendTo(tagContainer);
        else tagId = $(document.createElement('div')).addClass(classIdentifier).css('display', 'none');//.appendTo(tagContainer);

        var html = '<div class="loading-image"></div>' +
                   '<div class="loading-text">' +
                        '<span></span>' +
                   '</div>'
        ;

        tagId.append(html);
        tagId.css({
            height: _opt.size.height
        });

        tagId.find('.loading-text').css({
            height: _opt.size.height
        })
            .find('span').css({
                lineHeight: _opt.size.height + 'px'
            });

    };//render

};//MEL.loading
MEL.repo = function (id, object) {

    if (typeof object === 'undefined') return deserialize();
    serialize(object);

    function serialize(object) {
        var stringify = object;
        if (typeof object !== 'string') stringify = JSON.stringify(object);

        try {
            localStorage.setItem(id, stringify);
        } catch (e) {
            MEL.log('unable to save data with id \'' + id + '\' to local storage.');
            MEL.log(e);
        }
    }
    function deserialize() {
        var stringify = localStorage.getItem(id);
        if (typeof stringify === 'undefined' || !stringify) return null;

        try {
            var obj = JSON.parse(stringify);
            return obj;
        } catch (e) {
            return stringify;
        }
    }
},//MEL.repo

MEL._core  = function () {
    var
        SELECTOR_BODY = 'body',

        URL_CONFIG = '/Account/GetConfig',
        URL_ACTIVE_LOCATION_POST = '/Account/ActiveLocation',

        MSG_INVALID_URL = 'Invalid URL or empty URL',
        MSG_FORMAT_PERMISSION_ACT_INVALID = 'Invalid \'{0}\' property format of \'{1}\' ',

        FILTER_CSS_PERMISSION_ID = '[data-permission-id]',
        FILTER_CSS_PERMISSION_ACT = '[data-permission-act]',

        DATA_CSS_PERMISSION_ID = 'permission-id',
        DATA_CSS_PERMISSION_ACT = 'permission-act',

        ID_COMBOBOX_PROPERTY_SECTION ='cmb-property-section'

        CSS_CLASS_PROPERTY_OPT = 'property-opt',

        tempLoadAjaxDatas = [],
        
        __config = null,
        __opt = {},
        errDialog = null,
        availablePermissions = [],
        isloading=false
    ;


    function releaseAjaxCall(list) {
        for (var i = 0; i < list.length; i++) {
            list[i].abort();
        }
    }
    function loadData(args) {
        /*console.log(args.url);*/
        /*
            args.url
            args.dto
            args.callback
            args.scope
            args.requestAjaxList
            args.fnError
        */

        if (!args.url) MEL.log(MSG_INVALID_URL);
        
        //LOG
        //MEL.log('post ' + args.url);

        //MEL.log(args.data);

        releaseAjaxCall(!args.requestAjaxList ? tempLoadAjaxDatas : args.requestAjaxList);
        var request = $.ajax({
            url: args.url,
            type: 'post',
            dataType: 'json',
            data: args.dto,
            success: function (result) {
                /*console.log(result);*/
                if (!result.success) {
                   
                    var errMsg = '';
                    if (result.exception) errMsg = result.exception.message;

                    bootbox.alert({
                        centerVertical: true,
                        size: "small",
                        message: (errMsg.length == 0 ? 'General Error' : errMsg)
                    });

              
                    return;
                }
                
                if (typeof args.callback == 'function') {
                    /*console.log(result.data);*/
                    args.callback.call(args.scope ? args.scope : _instance, result.data);
                }

            },
            error: function (xhr, statusText, ex) {
                if (!ex && ex.message) window.alert(ex.message);
                $('body').html(xhr.responseText);
                if (typeof args.fnError == 'function') args.fnError.call(args.scope ? args.scope : _instance, xhr, statusText, ex);
            }
        });

        if (!args.requestAjaxList) tempLoadAjaxDatas.push(request);
        else args.requestAjaxList.push(request);
    }
    function loadConfig(callback) {
        loadData({
            url: URL_CONFIG,
            callback: function (data) {
                //console.log("response data in loadConfig mel.core.js")
                //console.log(data)
                __config = data;
                initConfig(function () {
                    displayUserInfo();
                    displayPropertyInfo();

                    MEL.loadPermissions(function (list) {
                        availablePermissions = list;
                    }, __config.permission);

                });
                $('.main-sidebar').show();
                //$('body').removeClass("sidebar-collapse");
                if (typeof callback == 'function') callback.call(_instance, data);
            }
        });
    }
    function exestartupCallback() {
        for (var i = 0; i < _instance.fnStartup.length; i++) {
            var fnopt = _instance.fnStartup[i];
            var fn = fnopt.fn;
            try {
                if (typeof fn == 'function' && !fnopt.called) {
                    fn.call(_instance);
                    fnopt.called = true;
                }
            } catch (e) {
                MEL.log(e);
            }
        }
    }
    function coreInit() {
        if (isloading) return;
        isloading = true;
        
        loadConfig(function () {
            _instance.config = __config;
            
            exestartupCallback();
            _instance.permission.validate();
            _instance.loaded = true;
            isloading = false;
        });
    }
    function displayUserInfo() {
        var user = __config.user;
        //console.log("user details in function that sets user logout variable")
        //console.log(user)
        if (!user)
        {
            var logoutformTag = document.getElementById('logoutForm');
            $('#autologout').val('yes');
            if (logoutformTag) logoutformTag.submit();
            return;
        }

        $('.user-menu > a > span').text(user? user.UserName : '');
        $('.user-header > p').text(user ? user.Name : '');
        $('.user-panel > .info > p').text(String.format('Hello, {0}',user ? user.UserName : ''));
    }
    function displayPropertyInfo() {
        var container = $(MEL.toClass(CSS_CLASS_PROPERTY_OPT));

        if (!__opt.showProperty) {
            container.css({ display: 'none' });
            return;
        }


        var cmbtag = container.find('select');
        cmbtag.empty();
        for (var i = 0; i < __config.Locations.length; i++) {
            var property = __config.Locations[i];
            var temp = $(String.format('<option value="{0}">{1} - {2}</option>', property.Pvid, property.Code,property.Name));
            cmbtag.append(temp);
        }

        cmbtag.off();
        cmbtag.change(function () {
            var tempid = $(this).val();
            var location = MEL.getByValue(__config.Locations, 'Pvid', tempid);
            if (!location) {
                MEL.log('Unable to get selected location.');
                return false;
            }

            activeLocation(location, function () {
                if (typeof __opt.fnPropertyChanged == 'function') __opt.fnPropertyChanged.call(_instance, location);
            });
            //if (typeof __opt.fnPropertyChanged == 'function') __opt.fnPropertyChanged.call(_instance, location);
        });

        container.css({ display: '' });

        if (__config.ActiveLocation) {
            cmbtag.val(__config.ActiveLocation.Pvid);
            cmbtag.change();
        }
    }
    function initDefaultOpt() {
        if (typeof __opt.showProperty == 'undefined') __opt.showProperty = false;
        if (typeof __opt.fnPropertyChanged == 'undefined') __opt.fnPropertyChanged = null;
    }
    function initConfig(fn) {

        //if (!__config.ActiveLocation && __config.Locations && __config.Locations.length) {
        //    activeLocation(__config.Locations[0],fn);
        //}

        if (typeof fn == 'function') fn.call(_instance);
    }
    function activeLocation(location, fn) {
        MEL.loading('Please wait...');
        $.ajax({
            url: URL_ACTIVE_LOCATION_POST,
            type: 'post',
            dataType: 'json',
            data: location,
            success: function (result) {
                MEL.loading();
                __config = result.data;
                if (typeof fn == 'function') fn.call(_instance);
            },
            error: function (xhr, status, ex) {
                MEL.loading();
                MEL.log(ex);
            }
        });
    }

    function permission() {

        function getCssProperty(plaincss) {
            var props = plaincss.replaceAll('{').replaceAll('}');
            var plains = props.split(';');
            var dictionary = [];
            for (var i = 0; i < plains.length; i++) {
                var plain = plains[i];
                if (!plain) continue;
                var keyval = plains[i].split(':');
                dictionary.push(keyval[0]);//add key
                dictionary[keyval[0]] = keyval[1];
            }
            return dictionary;
        }
        function addCssProperty(tag, plaincss) {
            var props = getCssProperty(plaincss);
            for (var i = 0; i < props.length; i++) {
                var property = props[i];
                tag.css(property, props[property]);
            }
        }
        function removeCssProperty(tag, plaincss) {
            var props = getCssProperty(plaincss);
            for (var i = 0; i < props.length; i++) {
                var property = props[i];
                tag.css(property, '');
            }
        }
        function isCssClass(text) {
            return !text.contains('{') && !text.contains('}') && !text.contains(':');
        }
        function enable(ctrl) {

       
            if (ctrl.data(DATA_CSS_PERMISSION_ACT)) {

                var data = ctrl.data(DATA_CSS_PERMISSION_ACT);

                var sections = data.split('|');
                if (sections.length != 2) MEL.log(String.format(MSG_FORMAT_PERMISSION_ACT_INVALID, FILTER_CSS_PERMISSION_ACT, ctrl.data(DATA_CSS_PERMISSION_ID)));
                var enablesection = sections[0];
                var disablesection = sections[1];
                if (isCssClass(enablesection)) {
                    ctrl.removeClass(disablesection);
                    ctrl.addClass(enablesection);
                }
                else {
                    removeCssProperty(ctrl, disablesection);
                    addCssProperty(ctrl, enablesection);
                }
            }
        }
        function disable(ctrl) {

            if (ctrl.data(DATA_CSS_PERMISSION_ACT)) {

                var data = ctrl.data(DATA_CSS_PERMISSION_ACT);
                var sections = data.split('|');
                if (sections.length != 2) MEL.log(String.format(MSG_FORMAT_PERMISSION_ACT_INVALID, FILTER_CSS_PERMISSION_ACT, ctrl.data(DATA_CSS_PERMISSION_ID)));
                var disablesection = sections[1];
                var enablesection = sections[0];
                if (isCssClass(disablesection)) {
                    ctrl.removeClass(enablesection);
                    ctrl.addClass(disablesection);
                }
                else {
                    removeCssProperty(ctrl, enablesection);
                    addCssProperty(ctrl, disablesection);
                }
            }
            
        }
        function hasOwner(ctrlTag) {
            var id = null;

            if (typeof ctrlTag == 'string') id = ctrlTag;
            else id = ctrlTag.data(DATA_CSS_PERMISSION_ID);


            if (!__config.userProfiles) return false;

            var haspermission = false;
            var tempPermissionOwnership = [];
            var profile = __config.userProfiles;
            var roles = profile.roleProfiles;
            var activeLocation = __config.ActiveLocation;
            var roleprofiles = profile.roleProfiles;

            //DEFAULT ROLE
            for (var i = 0; i < roleprofiles.length; i++)
            {
                var role = roleprofiles[i].role;
                /*console.log(role);*/
                if (!role.isActive) continue;

                //var endDate = JSON.dateStringToDate(role.endActiveDate);
                //var startDate = JSON.dateStringToDate(role.startActiveDate);
                var endDate = new Date(role.endActiveDate);
                var startDate = new Date(role.startActiveDate);
                var today = (new Date);
                endDate.setHours(0, 0, 0, 0);
                startDate.setHours(0, 0, 0, 0);
                today.setHours(0, 0, 0, 0);

                if (endDate.isBefore(today) ||
                    startDate.isAfter(today)) continue;

                var roleAccess = role.accessList;

                for (var x = 0; x < roleAccess.length; x++)
                {
                    var roleaccess = roleAccess[x];
                    var access = roleaccess.accessLevel;


                    for (var y = 0; y < access.accessDetails.length; y++)
                    {
                        var permission = access.accessDetails[y];

                        /*CONSIDER AGAIN*/
                        if (roleaccess.identityModulePvid != permission.identityModulePvid) continue;

                        var cpobj = availablePermissions[permission.permissionKey];
                        var contentPermission = cpobj ? cpobj.obj : null;
                        if (contentPermission && contentPermission.staticAuthorized) {
                            haspermission = (permission.permissionKey == id);

                        }
                        if (haspermission) break;
                    }
                    if (haspermission) break;
                }
                if (haspermission) break;
            }

            //OVERRIDE ROLE BY EXLUSIVE ROLE
            for (var i = 0; i < profile.accessLevelExclusiveList.length; i++)
            {
                var permission = profile.accessLevelExclusiveList[i];

             

                var cpobj = availablePermissions[permission.permissionKey];

                var contentPermission = cpobj ? cpobj.obj : null;


                if (permission.permissionKey == id) {
                    if (contentPermission && contentPermission.staticAuthorized) {
                        haspermission = permission.accessible ? true : false;
                    }
                    else {
                        if (permission.permissionKey == id) {
                            haspermission = permission.accessible ? true : false;

                            break;
                        }
                    }
                }
                
            }


            return haspermission;
        }
       
        function validate() {

            //console.log(availablePermissions);
            ///Even is hidden, but users still can access to the page by navigating using URL
            ///No point to hide it if not yet reset password, but below code still kept for future use

            ////HIDE ALL MENU IF NOT YET RESET PASSWORD 
            //$.ajax({
            //    type: "POST",
            //    dataType: "text",
            //    url: "/Account/CheckIfFirstLoginPasswordReset",
            //    success: function (data) {

            //        var controls = $(SELECTOR_BODY).find(FILTER_CSS_PERMISSION_ID);


            //        for (var i = 0; i < controls.length; i++) {
            //            var tag = $(controls[i]);

            //            if (data === "DONE") {

            //            if (hasOwner(tag)) {
            //                enable(tag);
            //            }
            //            else {
            //                disable(tag);
            //                }
            //            }
            //        }

                    
            //    }
            //});
            var s = availablePermissions;
            var controls = $(SELECTOR_BODY).find(FILTER_CSS_PERMISSION_ID);
            //console.log(controls);

            for (var i = 0; i < controls.length; i++) {
                var tag = $(controls[i]);

                if (hasOwner(tag)) {


                    enable(tag);
                }
                else {


                    disable(tag);
                }
            }
           
        }
        var inst = {
            validate:function(){
                validate();
            },
            isAuthorized: function (permissionid) {
                return hasOwner(permissionid);
            }
        };

        return inst;
    }

    var _instance = {
        config:__config,
        fnStartup: [],
        loaded:false,

        

        load:function(args){
            loadData(args);
        },

        getActiveLocation: function () {
            return __config.ActiveLocation;
        },
        getLocations: function () {
            return __config.Locations;
        },
        setLocation:function(locationId){
        },
        option: function (opt) {

            if (!opt) return __opt;
            else {
                __opt = opt;
                displayPropertyInfo();
            }
        },

        initialize: function () {
            if (this.loaded) {
                exestartupCallback();
                return;
            }

            coreInit();
        },
        validate: function () {
            _instance.permission.validate();
        },
        permission: permission()
    }

    initDefaultOpt();
    return _instance;
}



if ($.validator && $.validator.unobtrusive) {
    $.validator.unobtrusive.adapters.addBool('validnumber');
    $.validator.addMethod('validnumber', function (value, element) {
        var temp = $(element);
       

        if (value) {
            if (!$.isNumeric(value)) return false;

            var val = parseFloat(value);
            var maxValue = parseFloat(temp.data('val-validnumber-maxvalue'));
            var minValue = parseFloat(temp.data('val-validnumber-minvalue'));
            var allowZero = temp.data('val-validnumber-allowzero');

            return val < maxValue && val > minValue && (allowZero || val != 0);
        }

        return false;
    });
    
}