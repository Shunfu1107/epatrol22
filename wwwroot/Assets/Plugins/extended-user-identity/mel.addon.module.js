/// <reference path="../jquery/dist/jquery.js" />
/// <reference path="mel.core.js" />
/// <reference path="mel.plugins.js" />
/// <reference path="../bootbox/dist/bootbox.js" />


$(window).on("load", function () {
    MEL.startup(function () {
        pms.modules();
    });
});


pms.modules = function () {
    var
        ID_MODULE_VIEW = 'modules-view-grid'
        , ID_MODULE_VIEW_REGISTER = 'modules-view-register'
        , ID_MODULE_VIEW_MANAGE = 'modules-view-manage'
        , ID_MODULE_VIEW_TABLE = 'modules-table'
        , ID_MODULE_VIEW_EDIT = 'modules-view-edit'
        , ID_MODULE_VIEW_ADD_PERMISSION = 'module-view-manage-permission'
        , ID_PERMISSION_VIEW_TABLE = 'permission-table'
        , ID_MEL_LAYER = 'mel-layer'
        , ID_BTN_ADD_MODULE = 'btn-add-module'
        , ID_BTN_MANAGE_CLOSE = 'btn-manage-close'
        , ID_FORM_REGISTER = 'form-register-module'
        , ID_MODAL_REGISTER = 'modal-register-module'
        , ID_FORM_MANAGE_MODULE = 'form-manage-module'
        , ID_FORM_EDIT_USER = 'form-edit-user'
        , ID_FORM_EDIT_MODULE = 'form-edit-module'
        , ID_MODAL_EDIT_MODULE = 'modal-edit-module'
        , ID_BTN_REGISTER_MODULE = 'btn-register-module'
        , ID_BTN_REGISTER_CANCEL_MODULE = 'btn-register-cancel'
        , ID_BTN_EDIT_MODULE = 'btn-edit-module'
        , ID_BTN_EDIT_CANCEL = 'btn-edit-cancel'
        , ID_BTN_ADD_PERMISSION = 'btn-add-permission'
        , ID_BTN_ADD_PERMISSION_CLOSE = 'btn-add-permission-close'
        , ID_BTN_MODULE_EDIT_UPDATE = 'btn-module-edit-update'
        , ID_BTN_MODULE_EDIT_CANCEL = 'btn-module-edit-cancel'
        , ID_MANAGE_MODULE_TXT_MODULE_NAME = 'txt-manage-module-name'
        , ID_MANAGE_MODULE_BTN_EDIT_MODULE = 'btn-manage-edit-module-name'
        , ID_MANAGE_MODULE_BTN_SAVE_MODULE = 'btn-manage-save-module-name'
        , ID_MANAGE_MODULE_BTN_CANCEL_MODULE = 'btn-manage-cancel-module-name'
        , ID_MODULE_EDIT_NAME = 'txt-edit-module-name'

        , CSS_CLASS_MEL_SECTION = 'mel-section'
        , CSS_CLASS_MEL_ACTIVE_SECTION = 'mel-active-section'
        , CSS_CLASS_LEFT_EDGE_SHADOW = 'left-edge-shadow'
        , CSS_CLASS_CELL_ERROR = 'cell-unknown-ref'
        , CSS_CLASS_SECTION_TRIGGER = 'mel-section-trigger'

        , URL_MODULE_MANAGE_POST = '/Module/EditModule'

        , global_records_per_page = 25

        , moduleView = null
        , moduleRegister = null
        , moduleManage = null
        , moduleManageAddPermission = null
        , currentView = null
        ;


    function init() {
        moduleView = _moduleView();
        moduleRegister = _moduleRegister();
        moduleManage = _moduleManage();
        moduleManageAddPermission = _addPermission();

        wireupEvent();

        moduleView.focus();

        $(MEL.toSelector(ID_MEL_LAYER)).css('display', '');
    }
    function wireupEvent() {

    }

    function activeSection(id) {

        if (!id) {
            moduleView.focus();
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

    function updateModule(currentModulePvid, sFormId, callback) {
        var form = $(MEL.toSelector(sFormId));
        if (!form.valid()) {
            $('.input-validation-error').first().focus();
            return;
        }

        var dto = MEL.toDTO(form);
        dto['Manage.ModulePvid'] = currentModulePvid;

        MEL.load({
            url: URL_MODULE_MANAGE_POST
            , dto: dto
            , callback: function (result) {
                //window.alert('Successfully save change of the Module.');
                //_inst.blur();
                //setMode(false);
                if (typeof callback == 'function') callback.call(this);
            }
        });
    }

    function _moduleView() {
        var
            URL_MODULE_VIEW = '/Module/GetModules'
            , URL_REMOVE_MODULE = '/Module/RemoveModule'
            , COL_MODULE_INDEX_SN = 0
            , COL_MODULE_INDEX_NAME = 1
            , COL_MODULE_INDEX_CONTROLS = 2

            , CSS_CLASS_GRID_BTN_REMOVE = 'grid-btn-remove'
            , CSS_CLASS_GRID_BTN_EDIT = 'grid-btn-edit'
            , CSS_CLASS_GRID_BTN_MANAGE = 'grid-btn-manage'

            , records_per_page = 25
            , moduletable = null
            , dialogConfirmRemove = null
            ;

        function init() {
            initModuleGrid();

            wireupEvent();
        }
        function initModuleGrid() {
            moduletable = $(MEL.toSelector(ID_MODULE_VIEW_TABLE)).dataTable({
                'bServerSide': true
                , 'sAjaxSource': URL_MODULE_VIEW
                , 'iDisplayLength': records_per_page
                , 'sAjaxDataProp': 'data'
                , "bProcessing": true
                , 'sPaginationType': 'full_numbers'
                , 'bPaginate': true
                , 'bSort': false
                , 'bJQueryUI': false
                , 'autoWidth': false
                , 'oLanguage': {
                    sEmptyTable: 'No records found.'
                    , "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
                }
                , "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [COL_MODULE_INDEX_SN] }
                    , { "bSortable": false, "aTargets": [COL_MODULE_INDEX_NAME] }
                    , {
                        "bSortable": false,
                        'aTargets': [COL_MODULE_INDEX_CONTROLS],
                        'mRender': function (data, type, full) {
                            return String.format('<a class="{5} mel-disable" data-mods="{0}" data-permission-id="{6}" data-permission-act="mel-enable|mel-disable" title="Edit Module" ><i class="fa fa-edit"></i></a>&nbsp;' +
                                '<a class="{4} mel-disable" data-mods="{0}" data-permission-id="{2}" data-permission-act="mel-enable|mel-disable" title="Manage Module" ><i class="fa fa-cog"></i></a>&nbsp;' +
                                '<a class="{3} mel-disable" data-mods="{0}" data-permission-id="{1}" data-permission-act="mel-enable|mel-disable" title="Remove Module"><i class="fa fa-ban"></i></a>&nbsp'
                                , data.pvid
                                , pms.keys.module.removeModule
                                , pms.keys.module.manageModule
                                , CSS_CLASS_GRID_BTN_REMOVE
                                , CSS_CLASS_GRID_BTN_MANAGE
                                , CSS_CLASS_GRID_BTN_EDIT
                                , pms.keys.module.editModule
                            );
                        }
                    }
                ]
                , 'fnDrawCallback': function (oSettings) {
                    /* Need to redo the counters if filtered or sorted */
                    //if (oSettings.bSorted || oSettings.bFiltered) {
                    //    for (var i = 0, iLen = oSettings.aiDisplay.length ; i < iLen ; i++) {
                    //        $(String.format("td:eq({0})", COL_MODULE_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                    //    }
                    //}

                    for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                        $(String.format("td:eq({0})", COL_MODULE_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + oSettings._iDisplayStart + 1);
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
            moduletable.api().ajax.reload();
        }

        function removeModule(module) {
            MEL.load({
                url: URL_REMOVE_MODULE
                , dto: module
                , callback: function (data) {

                    bootbox.alert({
                        centerVertical: true,
                        size: "small",
                        message: String.format('The module ({0}) has been removed successfully.', module.name),
                        callback: function () {
                            refreshGrid();
                        }
                    });
                }
            });
        }

        function wireupEvent() {
            var btnadd = $(MEL.toSelector(ID_BTN_ADD_MODULE));

            btnadd.off();

            btnadd.click(function (e) {
                e.preventDefault();
                moduleRegister.focus();
            });
        }
        function wireupEventGrid() {
            var btnedit = moduletable.find((MEL.toClass(CSS_CLASS_GRID_BTN_EDIT)));
            var btnremove = moduletable.find((MEL.toClass(CSS_CLASS_GRID_BTN_REMOVE)));
            var btnmanage = moduletable.find(MEL.toClass(CSS_CLASS_GRID_BTN_MANAGE));

            btnedit.off();
            btnremove.off();
            btnmanage.off();

            btnedit.click(function () {
                var id = $(this).data('mods');
                var list = moduletable.fnGetData();
                var mods = MEL.getByValue(list, 'pvid', id);

                $(MEL.toSelector(ID_MODULE_EDIT_NAME)).val(mods.name);

                $(MEL.toSelector(ID_FORM_EDIT_MODULE)).on('submit', function (e) {

                    e.preventDefault();

                    var form = $(MEL.toSelector(ID_FORM_EDIT_MODULE));
                    if (!form.valid()) {
                        $('.input-validation-error').first().focus();
                        return;
                    }

                    var dto = MEL.toDTO(form);
                    dto['Manage.ModulePvid'] = mods.pvid;

                    MEL.load({
                        url: URL_MODULE_MANAGE_POST
                        , dto: dto
                        , callback: function (result) {
                            $(MEL.toSelector(ID_MODAL_EDIT_MODULE)).modal('hide');

                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: "Module has been updated successfully.",
                                callback: function () {
                                    moduleView.focus();
                                }
                            });
                        }
                    });

                });

                $(MEL.toSelector(ID_MODAL_EDIT_MODULE)).modal('show');
                $(MEL.toSelector(ID_MODULE_VIEW_EDIT)).css('display', 'block');


            });
            btnmanage.click(function () {
                var id = $(this).data('mods');
                var list = moduletable.fnGetData();
                var mods = MEL.getByValue(list, 'pvid', id);
                moduleManage.focus({ module: mods });
            });
            btnremove.click(function () {
                var id = $(this).data('mods');
                var list = moduletable.fnGetData();
                var mods = MEL.getByValue(list, 'pvid', id);

                bootbox.confirm({
                    centerVertical: true,
                    size: "small",
                    message: "Do you wish to remove the module (" + mods.name + ") ?",
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
                            removeModule(mods);
                            return true;
                        }
                        else {
                            return true;
                        }
                    }
                });

            });

            MEL.validate();
        }

        var _inst = {
            refresh: function () {
                refreshGrid();
            },
            focus: function (opt) {
                refreshGrid();
                activeSection(ID_MODULE_VIEW);
                currentView = this;
            },
            blur: function () {

            }
        };

        init();
        return _inst;
    }
    function _moduleRegister() {
        var
            URL_REGISTER_POST = '/Module/Register'
            , isModal = true
            ;

        function init() {
            wireupEvent();
        }
        function wireupEvent() {
            var btnregister = $(MEL.toSelector(ID_BTN_REGISTER_MODULE));
            var btncancel = $(MEL.toSelector(ID_BTN_REGISTER_CANCEL_MODULE));

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

            e.preventDefault();

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
                        message: "Module has been created successfully.",
                        callback: function () { _inst.blur(); }
                    });
                }
            });

        });

        var _inst = {
            refresh: function () {

            },
            focus: function (opt) {

                if (isModal) {

                    $(MEL.toSelector(ID_MODAL_REGISTER)).modal('show');
                    $(MEL.toSelector(ID_MODULE_VIEW_REGISTER)).css('display', 'block');


                }
                else {
                    activeSection(ID_MODULE_VIEW_REGISTER);
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
    function _moduleManage() {

        var

            COL_PERMISSION_INDEX_SN = 0
            , COL_PERMISSION_INDEX_NAME = 1
            , COL_PERMISSION_INDEX_DESC = 2
            , COL_PERMISSION_INDEX_CONTROLS = 3

            , CSS_CLASS_GRID_BTN_REMOVE = 'grid-button-remove'

            , URL_MODULE_ORG_VIEW = '/Module/GetModuleOrganize'

            , currentModule = null
            , availablePermissions = []

            , permissiontable = null
            , dialogConfirmRemove = null
            ;


        function init() {
            initPermissionGrid();
            wireupEvent();
            MEL.loadPermissions(function (list) {
                availablePermissions = list;
                //console.log(list);
            });

        }
        function initView() {
            var txtmodulename = $(MEL.toSelector(ID_MANAGE_MODULE_TXT_MODULE_NAME));
            txtmodulename.val(currentModule.name);
        }
        function initPermissionGrid() {
            permissiontable = $(MEL.toSelector(ID_PERMISSION_VIEW_TABLE)).dataTable({
                'bServerSide': true
                , 'sAjaxSource': URL_MODULE_ORG_VIEW
                , 'iDisplayLength': global_records_per_page
                , 'sAjaxDataProp': 'data'
                , 'sPaginationType': 'full_numbers'
                , 'deferLoading': 0
                , "bProcessing": true
                , 'bPaginate': true
                , 'bSort': false
                , 'bJQueryUI': false
                , 'autoWidth': false
                , 'oLanguage': {
                    sEmptyTable: 'No records found.'
                    , "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
                }
                , 'fnServerData': function (sSource, aoData, fnCallback, oSettings) {

                    //if (!currentModule) return;

                    aoData.push({
                        name: 'modulePvid',
                        value: (!currentModule ? 0 : currentModule.pvid)
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
                    { "bSortable": false, "aTargets": [COL_PERMISSION_INDEX_SN], 'sWidth': '25px', 'mData': null },
                    {
                        "bSortable": false,
                        'aTargets': [COL_PERMISSION_INDEX_NAME],
                        'mRender': function (data, type, full) {
                            var perms = availablePermissions[full.permissionKey];
                            return perms ? perms.obj.title ? perms.obj.title : '-' : full.permissionKey;
                        }
                    },
                    {
                        "bSortable": false,
                        'aTargets': [COL_PERMISSION_INDEX_DESC],
                        'mRender': function (data, type, full) {
                            var perms = availablePermissions[full.permissionKey];
                            return perms ? perms.obj.desc ? perms.obj.desc : '-' : full.permissionKey;
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
                , 'createdRow': function (row, data, index) {
                    var perms = availablePermissions[data.permissionKey];
                    if (!perms) {
                        $('td', row).addClass(CSS_CLASS_CELL_ERROR).text(data.permissionKey);
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

        function refreshGrid() {
            permissiontable.api().ajax.reload();
        }
        function setMode(editmode) {
            if (editmode) {
                $(MEL.toSelector(ID_MANAGE_MODULE_TXT_MODULE_NAME)).removeAttr('disabled').select();
                $(MEL.toSelector(ID_MANAGE_MODULE_BTN_EDIT_MODULE)).css('display', 'none');
                $(MEL.toSelector(ID_MANAGE_MODULE_BTN_CANCEL_MODULE)).css('display', '');
                $(MEL.toSelector(ID_MANAGE_MODULE_BTN_SAVE_MODULE)).css('display', '');
            }
            else {
                $(MEL.toSelector(ID_MANAGE_MODULE_TXT_MODULE_NAME)).attr('disabled', 'disabled');
                $(MEL.toSelector(ID_MANAGE_MODULE_BTN_EDIT_MODULE)).css('display', '');
                $(MEL.toSelector(ID_MANAGE_MODULE_BTN_CANCEL_MODULE)).css('display', 'none');
                $(MEL.toSelector(ID_MANAGE_MODULE_BTN_SAVE_MODULE)).css('display', 'none');
            }
        }
        function doSave() {
            //var form = $(MEL.toSelector(ID_FORM_MANAGE_MODULE));
            //if (!form.valid()) {
            //    $('.input-validation-error').first().focus();
            //    return;
            //}

            //var dto = MEL.toDTO(form);
            //dto['Manage.ModulePvid'] = currentModule.Pvid;

            //MEL.load({
            //    url: URL_MODULE_MANAGE_POST
            //    , dto: dto
            //    , callback: function (result) {
            //        //window.alert('Successfully save change of the Module.');
            //        //_inst.blur();
            //        setMode(false);
            //    }
            //});

            updateModule(currentModule.pvid, ID_FORM_MANAGE_MODULE, function () {
                setMode(false);
            });
        }

        function wireupEvent() {
            var btnAddPermission = $(MEL.toSelector(ID_BTN_ADD_PERMISSION));
            var btnEditModuleName = $(MEL.toSelector(ID_MANAGE_MODULE_BTN_EDIT_MODULE));
            var btnCancelModuleName = $(MEL.toSelector(ID_MANAGE_MODULE_BTN_CANCEL_MODULE));
            var btnSaveName = $(MEL.toSelector(ID_MANAGE_MODULE_BTN_SAVE_MODULE));
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
                    module: currentModule,
                    permissions: list,
                    availablePermissions: availablePermissions
                });
            });

            btnEditModuleName.click(function () {
                setMode(true);
            });
            btnCancelModuleName.click(function () {
                $(MEL.toSelector(ID_MANAGE_MODULE_TXT_MODULE_NAME)).val(currentModule.name);
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
                currentModule = opt.module;
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

            , URL_MANAGE_PERMISSION = '/Module/ManagePermission'

            , CSS_CLASS_CHECKBOX = 'chk-permission-tick'

            , _opt = null
            , permissiontable = null
            ;

        function init() {
            initGrid();
            wireupEvent();
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
                , 'deferLoading': 0
                , "bProcessing": true
                , 'bJQueryUI': false
                , 'autoWidth': false
                , 'oLanguage': {
                    sEmptyTable: 'No records found.'
                    , "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
                }

                , "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [COL_PERMISSION_INDEX_SN] }

                    /*DATA COLUMN COL_PERMISSION_CHECKBOX*/
                    , {
                        "bSortable": false,
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

                //    /*DATA COLUMN CHECKBOX*/
                //    , {
                //        'aTargets': [COL_PERMISSION_CHECKBOX], 'mRender': function (data, type, full) {
                //            return String.format('<input type="checkbox" class="chk-perission-select"/>');
                //        }
                //    }
                //]
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
                for (var i = 0; i < _opt.availablePermissions.length; i++) {
                    var customctn = _opt.availablePermissions[i];
                    api.row.add(customctn);
                }
            }

            api.draw();
        }

        function wireupEvent() {
            var btnclose = $(MEL.toSelector(ID_BTN_ADD_PERMISSION_CLOSE));

            btnclose.off();

            btnclose.click(function () {
                _inst.blur();
            });

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
                    ModulePvid: _opt.module.pvid,
                    PermissionKey: ctn.key,
                    tag: self
                });
            });

        }

        function hasOwnPermission(key) {
            return MEL.getByValue(_opt.module.permissions, 'permissionKey', key);
        }
        function modifyPermission(args) {

            var tag = args.tag;
            args.tag = null;

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

                        tag.prop('checked', !args.Included);
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
                moduleManage.focus({ module: _opt.module });
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