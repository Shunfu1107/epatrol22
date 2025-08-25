/// <reference path="../jquery/dist/jquery.js" />
/// <reference path="mel.core.js" />
/// <reference path="mel.plugins.js" />
/// <reference path="../bootbox/dist/bootbox.js" />



$(window).on("load", function () {
    MEL.startup(function () {
        pms.accessLevel();
    });
});


pms.accessLevel = function () {
    var
        ID_ACCESS_VIEW = 'access-view-grid'
        , ID_BTN_ADD_ACCESS = 'btn-add-access'
        , ID_ACCESS_VIEW_TABLE = 'access-table'
        , ID_PERMISSION_VIEW_TABLE = 'permission-table'
        , ID_ACCESS_VIEW_REGISTER = 'access-view-register'
        , ID_ACCESS_VIEW_EDIT = 'access-view-edit'
        , ID_MEL_LAYER = 'mel-layer'
        , ID_BTN_REGISTER_ACCESS = 'btn-register-access'
        , ID_BTN_REGISTER_CANCEL_ACCESS = 'btn-register-cancel'
        , ID_BTN_MANAGE_CLOSE = 'btn-manage-close'
        , ID_CMB_MODULE_SELECTION = 'cmb-module-selection'
        , ID_FORM_REGISTER = 'form-register-access'
        , ID_MODAL_REGISTER = 'modal-register-access'
        , ID_FORM_MANAGE_ACCESS = 'form-manage-access'
        , ID_FORM_ACCESS_EDIT = 'form-edit-accesslevel'
        , ID_MODAL_ACCESS_EDIT = 'modal-edit-accesslevel'
        , ID_MODULE_VIEW_MANAGE = 'access-view-manage'
        , ID_TXT_ACCESS_EDIT_NAME = 'txt-edit-access-name'
        , ID_MANAGE_ACCESS_TXT_ACCESS_NAME = 'txt-manage-access-name'
        , ID_MANAGE_ACCESS_BTN_EDIT_ACCESS = 'btn-manage-edit-access-name'
        , ID_MANAGE_ACCESS_BTN_SAVE_ACCESS = 'btn-manage-save-access-name'
        , ID_MANAGE_ACCESS_BTN_CANCEL_ACCESS = 'btn-manage-cancel-access-name'

        , CSS_CLASS_MEL_SECTION = 'mel-section'
        , CSS_CLASS_MEL_ACTIVE_SECTION = 'mel-active-section'
        , CSS_CLASS_LEFT_EDGE_SHADOW = 'left-edge-shadow'
        , CSS_CLASS_MSG_REMOVE_CONFIRM = 'mel-dialog-confirm-msg'
        , CSS_CLASS_CELL_ERROR = 'cell-unknown-ref'

        , CSS_CLASS_SECTION_TRIGGER = 'mel-section-trigger'

        , URL_ACCESSLEVEL_UPDATE = '/AccessLevel/UpdateAccessLevel'

        , global_records_per_page = 100

        , contentView = null
        , contentRegister = null
        , contentManage = null
        , currentView = null

        //----------------------- OLD
        , ID_MODULE_VIEW_ADD_PERMISSION = 'module-view-manage-permission'
        , ID_BTN_ADD_PERMISSION = 'btn-add-permission'
        , ID_BTN_ADD_PERMISSION_CLOSE = 'btn-add-permission-close'

        , ID_DIALOG_CONFIRM = 'mel-dialog-confirm'
        , moduleManageAddPermission = null
        ;


    function init() {
        contentView = _accessView();
        contentRegister = _accessRegister();
        contentManage = _accessManage();
        moduleManageAddPermission = _addPermission();

        wireupEvent();

        contentView.focus();

        $(MEL.toSelector(ID_MEL_LAYER)).css('display', '');
    }
    function wireupEvent() {

    }

    function updateAccessLevel(accesspvid, sFormId, callback) {
        var form = $(MEL.toSelector(sFormId));
        if (!form.valid()) {
            $('.input-validation-error').first().focus();
            return;
        }
      
        var dto = MEL.toDTO(form);
        dto['Manage.AccessLevelPvid'] = accesspvid;

        MEL.load({
            url: URL_ACCESSLEVEL_UPDATE
            , dto: dto
            , callback: function (result) {
                //window.alert('Successfully save change of the Module.');
                //_inst.blur();
                //setMode(false);
                if (typeof callback == 'function') callback.call(this);
            }
        });
    }

    function activeSection(id) {

        if (!id) {
            contentView.focus();
            return;
        }

        if ($(MEL.toSelector(id)).is(':visible')) {

            activecontrol();
            return;
        }

        $(MEL.toClass(CSS_CLASS_MEL_SECTION)).css('display', 'none').stop(true).removeClass(CSS_CLASS_LEFT_EDGE_SHADOW);
        $(MEL.toSelector(id)).css({
            opacity: .1
            , marginLeft: '+=400px'
            , display: ''
        })
            //.addClass(CSS_CLASS_LEFT_EDGE_SHADOW)
            .animate({
                marginLeft: 0
                , opacity: 1
            }
                , 300, function () {
                    //$(this).removeClass(CSS_CLASS_LEFT_EDGE_SHADOW);
                    activecontrol();
                });

        function activecontrol() {
            var txtbox = $('#mel-layer input[type=text]:not(:disabled)');
            if (txtbox.length) {
                txtbox.first().focus();
            }
        }
    }
    function isActiveSection(oView) {
        return oView == currentView;
    }

    function _accessView() {
        var
            URL_ACCESS_VIEW = '/AccessLevel/GetAccessLevels'
            , URL_ACCESS_REMOVE = '/AccessLevel/RemoveAccessLevel'
            , COL_ACCESS_INDEX_SN = 0
            , COL_ACCESS_INDEX_NAME = 1
            , COL_ACCESS_INDEX_CONTROLS = 2

            , CSS_CLASS_GRID_BTN_REMOVE = 'grid-btn-remove'
            , CSS_CLASS_GRID_BTN_EDIT = 'grid-btn-edit'
            , CSS_CLASS_GRID_BTN_MANAGE = 'grid-btn-manage'

            , records_per_page = 25
            , accesstable = null
            , dialogConfirmRemove = null
            ;

        function init() {
            initAccessGrid();

            wireupEvent();
        }
        function initAccessGrid() {
            accesstable = $(MEL.toSelector(ID_ACCESS_VIEW_TABLE)).dataTable({
                'bServerSide': true
                , 'sAjaxSource': URL_ACCESS_VIEW
                , 'iDisplayLength': records_per_page
                , "bProcessing": true
                , 'sAjaxDataProp': 'data'
                , 'sPaginationType': 'full_numbers'
                , 'bPaginate': true
                , 'bSort': false
                , 'bJQueryUI': false
                , 'autoWidth': false
                , 'oLanguage': {
                    sEmptyTable: 'No records found.',
                    "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> ' 
                }
                , 'fnServerData': function (sSource, aoData, fnCallback, oSettings) {
                    oSettings.jqXHR = $.ajax({
                        type: 'post',
                        datatype: 'json',
                        url: sSource,
                        data: aoData,
                        success: function (result) {
                            if (result && typeof result.success != 'undefined' && !result.success) {
                                var msg = String.format('{0}.General error', URL_ACCESS_VIEW);
                                if (result.exception) msg = String.format('{0} - {1}', URL_ACCESS_VIEW, result.exception.message);
                                MEL.log(msg);
                                return;
                            }

                            fnCallback.call(this, result);
                        }
                    });
                }
                , "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [COL_ACCESS_INDEX_SN] }
                    , {
                        'aTargets': [COL_ACCESS_INDEX_CONTROLS],
                        'mRender': function (data, type, full) {
                            return String.format('<a class="{3} mel-disable" data-accs="{0}" data-permission-id="{1}" data-permission-act="mel-enable|mel-disable" title="Edit Access Level Name"><i class="fa fa-edit"></i></a>&nbsp' +
                                '<a class="{5} mel-disable" data-accs="{0}" data-permission-id="{6}" data-permission-act="mel-enable|mel-disable" title="Manage Access Level"><i class="fa fa-cog"></i></a>&nbsp' +
                                '<a class="{4} mel-disable" data-accs="{0}" data-permission-id="{2}" data-permission-act="mel-enable|mel-disable" title="Remove Access Level" ><i class="fa fa-ban"></i></a>&nbsp'
                                , data.pvid
                                , pms.keys.accesslevel.editAccessLevelName
                                , pms.keys.accesslevel.removeAccessLevel
                                , CSS_CLASS_GRID_BTN_EDIT
                                , CSS_CLASS_GRID_BTN_REMOVE
                                , CSS_CLASS_GRID_BTN_MANAGE
                                , pms.keys.accesslevel.manageAccessLevel
                            );
                        }
                    }
                ]
                , 'fnDrawCallback': function (oSettings) {
                    /* Need to redo the counters if filtered or sorted */
                    //if (oSettings.bSorted || oSettings.bFiltered) {
                    //    for (var i = 0, iLen = oSettings.aiDisplay.length ; i < iLen ; i++) {
                    //        $(String.format("td:eq({0})", COL_ACCESS_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                    //    }
                    //}
                    for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                        $(String.format("td:eq({0})", COL_ACCESS_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + oSettings._iDisplayStart + 1);
                    }

                    wireupEventGrid();
                }
                , 'aoColumns': [
                    { 'sWidth': '25px', 'mData': null }
                    , { 'mData': 'name' }
                    , { 'sWidth': '80px', 'mData': null }
                ]
            });
        }
        function refreshGrid() {
            accesstable.api().ajax.reload();
        }

        function removeAccessLevel(al) {
            MEL.load({
                url: URL_ACCESS_REMOVE
                , dto: al
                , callback: function (data) {

                    bootbox.alert({
                        centerVertical: true,
                        size: "small",
                        message: String.format('The module ({0}) has been removed successfully.', al.name),
                        callback: function () {
                            refreshGrid();
                        }
                    });
                }
            });
        }

        function wireupEvent() {
            var btnadd = $(MEL.toSelector(ID_BTN_ADD_ACCESS));

            btnadd.off();

            btnadd.click(function (e) {
                e.preventDefault();
                contentRegister.focus();
            });
        }
        function wireupEventGrid() {
            var btnedit = accesstable.find((MEL.toClass(CSS_CLASS_GRID_BTN_EDIT)));
            var btnremove = accesstable.find((MEL.toClass(CSS_CLASS_GRID_BTN_REMOVE)));
            var btnmanage = accesstable.find(MEL.toClass(CSS_CLASS_GRID_BTN_MANAGE));

            btnedit.off();
            btnremove.off();
            btnmanage.off();

            btnedit.click(function () {
                var id = $(this).data('accs');
                var list = accesstable.fnGetData();
                var accs = MEL.getByValue(list, 'pvid', id);
                //console.log(accs);
                var tempform = $(MEL.toSelector(ID_FORM_ACCESS_EDIT));
                tempform.find(MEL.toSelector(ID_TXT_ACCESS_EDIT_NAME)).val(accs.name);

                $(MEL.toSelector(ID_FORM_ACCESS_EDIT)).on('submit', function (e) {

                    e.preventDefault();

                    updateAccessLevel(accs.pvid, ID_FORM_ACCESS_EDIT, function () {

                        $(MEL.toSelector(ID_MODAL_ACCESS_EDIT)).modal('hide');

                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: "Access level has been updated successfully.",
                            callback: function () {
                                refreshGrid();
                            }
                        });

                    });

                });

                $(MEL.toSelector(ID_MODAL_ACCESS_EDIT)).modal('show');
                $(MEL.toSelector(ID_ACCESS_VIEW_EDIT)).css('display', 'block');

            });
            btnremove.click(function () {
                var id = $(this).data('accs');
                var list = accesstable.fnGetData();
                var accs = MEL.getByValue(list, 'pvid', id);

                bootbox.confirm({
                    centerVertical: true,
                    size: "small",
                    message: "Do you wish to remove the access level (" + accs.name + ") ?",
                    buttons: {
                        cancel: {
                            label: 'Cancel'
                        },
                        confirm: {
                            label: 'Remove'
                        }
                    },
                    callback: function (result) {
                        if (result) {
                            removeAccessLevel(accs);
                            return true;
                        }
                        else {
                            return true;
                        }
                    }
                });
            });
            btnmanage.click(function () {
                var id = $(this).data('accs');
                var list = accesstable.fnGetData();
                var accs = MEL.getByValue(list, 'pvid', id);
                contentManage.focus({ accessLevel: accs });
            });


            MEL.validate();
        }

        var _inst = {
            refresh: function () {
                refreshGrid();
            },
            focus: function (opt) {
                refreshGrid();
                activeSection(ID_ACCESS_VIEW);
                currentView = this;
            },
            blur: function () {

            }
        };

        init();
        return _inst;
    }
    function _accessRegister() {
        var
            URL_REGISTER_POST = '/AccessLevel/Register'
            , isModal = true
            ;


        function init() {
            wireupEvent();
        }
        function wireupEvent() {
            var btnregister = $(MEL.toSelector(ID_BTN_REGISTER_ACCESS));
            var btncancel = $(MEL.toSelector(ID_BTN_REGISTER_CANCEL_ACCESS));

            btnregister.off();
            btncancel.off();

            btnregister.click(function () {
                if (!isActiveSection(_inst) && !isModal) return false;
            });
            btncancel.click(function () {
                _inst.blur();
            });


        }

        function clearForm() {
            var form = $(MEL.toSelector(ID_FORM_REGISTER));
            form.formReset();
        }

        $(MEL.toSelector(ID_FORM_REGISTER)).on('submit', function (e) {
            var form = $(MEL.toSelector(ID_FORM_REGISTER));
            if (!form.valid()) {
                $('.input-validation-error').first().focus();
                return;
            }

            var dto = MEL.toDTO(form);

            MEL.load({
                url: URL_REGISTER_POST
                , dto: dto
                , callback: function (result) {
                    $(MEL.toSelector(ID_MODAL_REGISTER)).modal('hide');

                    bootbox.alert({
                        centerVertical: true,
                        size: "small",
                        message: "Access level has been created successfully.",
                        callback: function () { _inst.blur(); }
                    });
                }
            });
        });

        var _inst = {
            refresh: function () {

            },
            focus: function (opt) {
                //activeSection(ID_ACCESS_VIEW_REGISTER);
                //currentView = this;

                if (isModal) {

                    $(MEL.toSelector(ID_FORM_REGISTER)).on('submit', function (e) {

                        e.preventDefault();

                        if (!isActiveSection(_inst) && !isModal) return false;


                    });

                    $(MEL.toSelector(ID_MODAL_REGISTER)).modal('show');
                    $(MEL.toSelector(ID_ACCESS_VIEW_REGISTER)).css('display', 'block');

                }
                else {
                    activeSection(ID_ACCESS_VIEW_REGISTER);
                    currentView = this;
                }
            },
            blur: function () {
                clearForm();
                activeSection();

            }
        };

        init();
        return _inst;
    }
    function _accessManage() {

        var

            COL_ACCESSDETAILS_INDEX_SN = 0
            , COL_ACCESSDETAILS_INDEX_NAME = 3
            , COL_ACCESSDETAILS_INDEX_ROOT = 2
            , COL_ACCESSDETAILS_INDEX_DESC = 4
            , COL_ACCESSDETAILS_INDEX_MODULE = 1
            , COL_ACCESSDETAILS_INDEX_CONTROLS = 5

            , CSS_CLASS_GRID_BTN_REMOVE = 'grid-button-remove'

            , URL_ACCESS_DETAILS_VIEW = '/AccessLevel/GetAccessDetails'
            , URL_ACCESS_MANAGE_POST = '/AccessLevel/ManageAccess'

            , currentAccess = null
            , availablePermissions = []

            , permissiontable = null
            , dialogConfirmRemove = null
            ;


        function init() {
            //initAccessDetailGrid();
            wireupEvent();
            MEL.loadPermissions(function (list) {
                availablePermissions = list;
                initAccessDetailGrid();
            });

        }
        function initView() {
            var txtmodulename = $(MEL.toSelector(ID_MANAGE_ACCESS_TXT_ACCESS_NAME));

            txtmodulename.val(currentAccess.name);
        }
        function initAccessDetailGrid() {
            permissiontable = $(MEL.toSelector(ID_PERMISSION_VIEW_TABLE)).dataTable({
                'bServerSide': true
                , 'sAjaxSource': URL_ACCESS_DETAILS_VIEW
                , 'iDisplayLength': global_records_per_page
                , 'sAjaxDataProp': 'data'
                , "bProcessing": true
                , 'sPaginationType': 'full_numbers'
                , 'deferLoading': 0
                , 'bPaginate': true
                , 'bSort': false
                , 'bJQueryUI': false
                , 'autoWidth': false
                , 'oLanguage': {
                    sEmptyTable: 'No records found.',
                    "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> ' 
                }
                , 'fnServerData': function (sSource, aoData, fnCallback, oSettings) {

                    //if (!currentAccess) return;

                    aoData.push({
                        name: 'AccessLevelPvid',
                        value: (!currentAccess ? 0 : currentAccess.pvid)
                    })

                    oSettings.jqXHR = $.ajax({
                        type: 'post',
                        datatype: 'json',
                        url: sSource,
                        data: aoData,
                        success: fnCallback
                    });
                }
                , "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [COL_ACCESSDETAILS_INDEX_SN], 'sWidth': '25px', 'mData': null },
                    {
                        "bSortable": false,
                        "aTargets": [COL_ACCESSDETAILS_INDEX_ROOT], 'sWidth': '50px',
                        'mRender': function (data, type, full) {
                            var perms = availablePermissions[full.permissionKey];
                            if (!perms) return full.permissionKey;
                            if (!perms.root) return perms.obj.title;
                            return perms.root.obj.title ? perms.root.obj.title : '-';
                        }
                    },
                    { "aTargets": [COL_ACCESSDETAILS_INDEX_MODULE], 'sWidth': '120px', 'mData': 'module.name' },
                    {
                        'aTargets': [COL_ACCESSDETAILS_INDEX_NAME],
                        'mRender': function (data, type, full) {
                            var perms = availablePermissions[full.permissionKey];
                            if (!perms) return full.permissionKey;
                            return perms.obj.title ? perms.obj.title : '-';
                        }
                    },
                    {
                        'aTargets': [COL_ACCESSDETAILS_INDEX_DESC],
                        'mRender': function (data, type, full) {
                            var perms = availablePermissions[full.permissionKey];
                            if (!perms) return full.permissionKey;
                            return perms.obj.desc ? perms.obj.desc : '-';
                        }
                    }
                ]
                , 'fnDrawCallback': function (oSettings) {
                    /* Need to redo the counters if filtered or sorted */
                    //if (oSettings.bSorted || oSettings.bFiltered) {
                    for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                        $(String.format("td:eq({0})", COL_ACCESSDETAILS_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + oSettings._iDisplayStart + 1);
                    }
                    //}

                    //if (oSettings.bSorted || oSettings.bFiltered) {
                    //    for (var i = 0, iLen = oSettings.aiDisplay.length ; i < iLen ; i++) {
                    //        var nTr = oSettings.aoData[oSettings.aiDisplay[i]].nTr;

                    //        // Update the index column and do so without redrawing the table
                    //        this.fnUpdate(i + 1, nTr, 0, false, false);
                    //    }
                    //}

                    wireupEventGrid();
                }
                , 'createdRow': function (row, data, index) {
                    if (index == COL_ACCESSDETAILS_INDEX_CONTROLS) return;
                    var perms = availablePermissions[data.permissionKey];
                    if (!perms) {
                        $('td', row).addClass(CSS_CLASS_CELL_ERROR);//.text(data.PermissionKey);
                    }
                }
                /*
                , 'aoColumns': [
                    { 'sWidth': '25px', 'mData': null }
                    , { 'mData': null } // Title
                    , { 'mData': null } // Desc
                    //, { 'sWidth': '50px', 'mData': null }
                ]*/
            });
        }
        function initAccessDetailGrid_tree() {

            $(MEL.toSelector(ID_PERMISSION_VIEW_TABLE)).fancytree({
                extensions: ["table"],
                checkbox: true,
                table: {
                    indentation: 20,      // indent 20px per node level
                    nodeColumnIdx: 2,     // render the node title into the 2nd column
                    checkboxColumnIdx: 0  // render the checkboxes into the 1st column
                },
                source: availablePermissions,
                //source: {
                //    url: "ajax-tree-products.json"
                //},
                //lazyLoad: function (event, data) {
                //    data.result = { url: "ajax-sub2.json" }
                //},
                renderColumns: function (event, data) {
                    var node = data.node,
                        $tdList = $(node.tr).find(">td");
                    // (index #0 is rendered by fancytree by adding the checkbox)
                    // SAMPLE-1:$tdList.eq(1).text(node.getIndexHier()).addClass("alignRight");
                    $tdList.eq(1).text(node.data.obj.title);
                    // (index #2 is rendered by fancytree)
                    $tdList.eq(2).text(node.data.obj.desc);
                    //$tdList.eq(3).text(node.key);
                }
            });
        }

        function refreshGrid() {
            permissiontable.api().ajax.reload();
        }
        function setMode(editmode) {
            if (editmode) {
                $(MEL.toSelector(ID_MANAGE_ACCESS_TXT_ACCESS_NAME)).removeAttr('disabled').select();
                $(MEL.toSelector(ID_MANAGE_ACCESS_BTN_EDIT_ACCESS)).css('display', 'none');
                $(MEL.toSelector(ID_MANAGE_ACCESS_BTN_SAVE_ACCESS)).css('display', '');
                $(MEL.toSelector(ID_MANAGE_ACCESS_BTN_CANCEL_ACCESS)).css('display', '');
            }
            else {
                $(MEL.toSelector(ID_MANAGE_ACCESS_TXT_ACCESS_NAME)).attr('disabled', 'disabled');
                $(MEL.toSelector(ID_MANAGE_ACCESS_BTN_EDIT_ACCESS)).css('display', '');
                $(MEL.toSelector(ID_MANAGE_ACCESS_BTN_SAVE_ACCESS)).css('display', 'none');
                $(MEL.toSelector(ID_MANAGE_ACCESS_BTN_CANCEL_ACCESS)).css('display', 'none');
            }
        }
        function doSave() {
            //var form = $(MEL.toSelector(ID_FORM_MANAGE_ACCESS));
            //if (!form.valid()) {
            //    $('.input-validation-error').first().focus();
            //    return;
            //}

            //var dto = MEL.toDTO(form);
            //dto['Manage.AccessLevelPvid'] = currentAccess.Pvid;

            //MEL.load({
            //    url: URL_ACCESS_MANAGE_POST
            //    , dto: dto
            //    , callback: function (result) {
            //        //window.alert('Successfully save change of the Module.');
            //        //_inst.blur();
            //        setMode(false);
            //    }
            //});

            updateAccessLevel(currentAccess.pvid, ID_FORM_MANAGE_ACCESS, function () {
                setMode(false);
            });

        }

        function wireupEvent() {
            var btnAddPermission = $(MEL.toSelector(ID_BTN_ADD_PERMISSION));
            var btnEditModuleName = $(MEL.toSelector(ID_MANAGE_ACCESS_BTN_EDIT_ACCESS));
            var btnCancelModuleName = $(MEL.toSelector(ID_MANAGE_ACCESS_BTN_CANCEL_ACCESS));
            var btnSaveName = $(MEL.toSelector(ID_MANAGE_ACCESS_BTN_SAVE_ACCESS));
            var btnclose = $(MEL.toSelector(ID_BTN_MANAGE_CLOSE));


            btnAddPermission.off();
            btnEditModuleName.off();
            btnCancelModuleName.off();
            btnSaveName.off();
            btnclose.off();

            btnAddPermission.click(function () {
                if (currentView != _inst) return;
                var list = permissiontable.fnGetData();
                moduleManageAddPermission.focus({
                    access: currentAccess,
                    permissions: list,
                    availablePermissions: availablePermissions
                });
            });

            btnEditModuleName.click(function () {
                setMode(true);
            });
            btnCancelModuleName.click(function () {
                //console.log(currentAccess);
                $(MEL.toSelector(ID_MANAGE_ACCESS_TXT_ACCESS_NAME)).val(currentAccess.name);
                setMode(false);
            });
            btnSaveName.click(function () {
                if (currentView != _inst) return;
                doSave();
            });
            btnclose.click(function () {
                _inst.blur();
            });

        }
        function wireupEventGrid() {

        }

        var _inst = {
            refresh: function () {
                refreshGrid();
            },
            focus: function (opt) {
                currentAccess = opt.accessLevel;
                activeSection(ID_MODULE_VIEW_MANAGE);
                currentView = this;
                initView();

                refreshGrid();
                setMode(false);
            },
            blur: function () {
                activeSection();
            }
        };

        init();
        return _inst;
    }
    function _addPermission() {
        var
            COL_PERMISSION_INDEX_SN = 1
            , COL_PERMISSION_CHECKBOX = 0
            , COL_PERMISSION_INDEX_NAME = 2
            , COL_PERMISSION_INDEX_DESC = 3
            , ID_TABLE_PERMISSION_ADDING = 'permission-new-table'

            , URL_MODULE_LIST = '/AccessLevel/GetAllActiveModule'
            , URL_MANAGE_PERMISSION = '/AccessLevel/ManagePermission'

            , CSS_CLASS_CHECKBOX = 'chk-permission-tick'

            , ajax_request_module_org = []
            , tempModuleOrgList = []

            , _opt = null
            , permissiontable = null
            ;

        function init() {

            initGrid();
            initModuleSelection(function () {
                wireupEvent();
            });
        }
        function initModuleSelection(fn) {

            $.ajax({
                url: URL_MODULE_LIST,
                type: 'post',
                dataType: 'json',
                success: function (result) {
                    if (!result.success) {
                        var errMsg = '';
                        if (result.exception) errMsg = result.exception.message;
                        window.alert('Error on Opening Pages : ' + (errMsg.length == 0 ? 'General Error' : errMsg));
                        return;
                    }

                    var data = result.data;
                    var cmb = $(MEL.toSelector(ID_CMB_MODULE_SELECTION));
                    for (var i = 0; i < data.length; i++) {
                        var mod = data[i];
                        cmb.append(String.format('<option value="{0}">{1}</option>)', mod.pvid, mod.name));
                    }

                    if (typeof fn == 'function') fn.call(_inst);
                },
                error: function (xhr, status, ex) {
                    if (!ex && ex.message) window.alert(ex.message);
                    $('body').html(xhr.responseText);
                }
            });
        }
        function initView() {
        }
        function initGrid() {

            permissiontable = $(MEL.toSelector(ID_TABLE_PERMISSION_ADDING)).dataTable({
                //ajax:_opt.availablePermissions,
                'iDisplayLength': global_records_per_page
                , 'sPaginationType': 'full_numbers'
                , 'bPaginate': true
                , 'bSort': true
                , "bProcessing": true
                , 'deferLoading': 0
                , 'bJQueryUI': false
                , 'autoWidth': false
                , 'oLanguage': {
                    sEmptyTable: 'No records found.'
                    ,"processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> ' 
                }

                , "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [COL_PERMISSION_INDEX_SN] }

                    /*DATA COLUMN COL_PERMISSION_CHECKBOX*/
                    , {
                        'aTargets': [COL_PERMISSION_CHECKBOX], 'mRender': function (data, type, full) {
                            var hasownpermission = hasOwnPermission(data.key);
                            return String.format('<input type="checkbox" class="{0}" data-ref-key="{1}" {2}/>'
                                , CSS_CLASS_CHECKBOX
                                , data.key
                                , hasownpermission ? 'checked' : ''
                            );
                        }
                    }
                ]

                , 'fnDrawCallback': function (oSettings) {
                    /* Need to redo the counters if filtered or sorted */
                    //if (oSettings.bSorted || oSettings.bFiltered) {
                    //    for (var i = 0, iLen = oSettings.aiDisplay.length ; i < iLen ; i++) {
                    //        $(String.format("td:eq({0})", COL_PERMISSION_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                    //    }
                    //}
                    for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                        $(String.format("td:eq({0})", COL_PERMISSION_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + oSettings._iDisplayStart + 1);
                    }

                    wireupEventGrid();
                }
                , 'aoColumns': [
                    { 'sWidth': '25px', 'mData': null }
                    , { 'sWidth': '25px', 'mData': null }
                    , { 'mData': 'obj.title' }
                    , { 'mData': 'obj.desc' }
                ]
            });

            refreshGrid();

        }
        function refreshGrid() {
            var api = permissiontable.api();

            api.clear();
            if (_opt && _opt.availablePermissions) {
                var rootlist = [];
                for (var x = 0; x < tempModuleOrgList.length; x++) {
                    var org = tempModuleOrgList[x];
                    var content = _opt.availablePermissions[org.permissionKey];
                    if (content != null) {
                        if (!rootlist[content.key]) {
                            api.row.add(content);
                            rootlist[content.key] = content;
                        }
                        var details = content.allContents;
                        for (var i = 0; i < details.length; i++) {
                            api.row.add(details[i]);
                        }
                    }
                    
                }
            }

            api.draw();
        }

        function wireupEvent() {
            var btnclose = $(MEL.toSelector(ID_BTN_ADD_PERMISSION_CLOSE));
            var cmbModule = $(MEL.toSelector(ID_CMB_MODULE_SELECTION));

            btnclose.off();
            cmbModule.off();

            btnclose.click(function () {
                _inst.blur();
            });
            cmbModule.change(function () {
                MEL.load({
                    url: '/AccessLevel/GetModuleOrganize',
                    dto: { ModulePvid: this.value },
                    requestAjaxList: ajax_request_module_org,
                    callback: function (data) {
                        tempModuleOrgList = data;
                        refreshGrid();
                    }
                });
            });

            cmbModule.change();
        }
        function wireupEventGrid() {
            var chkpermission = $(MEL.toClass(CSS_CLASS_CHECKBOX));

            chkpermission.off();

            chkpermission.change(function () {
                var self = $(this);
                var ctn = permissiontable.fnGetData(self.closest('tr'));
                var checked = self.is(':checked');
                modifyPermission({
                    Included: checked,
                    AccessLevelPvid: _opt.access.pvid,
                    ModulePvid: $(MEL.toSelector(ID_CMB_MODULE_SELECTION)).val(),
                    PermissionKey: ctn.key
                });
            });

        }

        function hasOwnPermission(key) {
            return MEL.getByValue(_opt.permissions, 'permissionKey', key);
        }
        function modifyPermission(args) {

            $.ajax({
                url: URL_MANAGE_PERMISSION,
                type: 'post',
                dataType: 'json',
                data: args,
                success: function (result) {
                    if (!result.success) {
                        var errMsg = '';
                        if (result.exception) errMsg = result.exception.message;
                        window.alert('Error on Opening Pages : ' + (errMsg.length == 0 ? 'General Error' : errMsg));
                        return;
                    }
                },
                error: function (xhr, status, ex) {
                    if (!ex && ex.message) window.alert(ex.message);
                    $('body').html(xhr.responseText);
                }
            });
        }

        var _inst = {
            refresh: function () {
                refreshGrid();
            },
            focus: function (opt) {
                _opt = opt;

                activeSection(ID_MODULE_VIEW_ADD_PERMISSION);
                currentView = this;
                initView();
                refreshGrid();
            },
            blur: function () {
                contentManage.focus({
                    module: _opt.module,
                    accessLevel: _opt.access
                });
            }
        };

        init();
        return _inst;
    }

    var inst = {

    };

    init();
    return inst;
}