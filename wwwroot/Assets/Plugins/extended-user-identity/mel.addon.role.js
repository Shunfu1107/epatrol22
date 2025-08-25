/// <reference path="../jquery/dist/jquery.js" />
/// <reference path="mel.core.js" />
/// <reference path="mel.plugins.js" />
/// <reference path="../bootbox/dist/bootbox.js" />


$(window).on("load",function () {
    MEL.startup(function () {
        pms.roles();
    });
});


pms.roles = function () {
    var
            ID_ROLE_VIEW = 'roles-view-grid'
          , ID_ROLE_EDIT = 'role-view-edit'
          , ID_BTN_ADD_ROLE = 'btn-add-role'
          , ID_ROLE_VIEW_TABLE = 'role-table'
          , ID_MODULE_ACCESSRIGHT_VIEW_TABLE = 'module-accesslevel-table'
          , ID_MODULE_OWNERSHIP_TABLE = 'module-ownership-table'
          , ID_ACCESS_VIEW_REGISTER = 'role-view-register'
          , ID_ACCESSRIGHT_NEW = 'role-add-access-right'
          , ID_ROLE_OWNERSHIP_NEW = 'role-add-ownership'
          , ID_ACCESSRIGHT_TITLE = 'access-right-title'
          , ID_MEL_LAYER = 'mel-layer'
          , ID_BTN_REGISTER_ROLE = 'btn-register-role'
          , ID_BTN_REGISTER_ACCESSRIGHT = 'btn-register-accessright'
          , ID_BTN_REGISTER_CANCEL_ROLE = 'btn-register-cancel'
          , ID_BTN_ACCESSRIGHT_CANCEL_ROLE = 'btn-accessright-cancel'
          , ID_BTN_ROLE_ACCESSRIGHT_BACK = 'btn-role-accessright-back'
        , ID_FORM_REGISTER = 'form-register-role'
        , ID_MODAL_REGISTER = 'modal-register-role'
        , ID_FORM_EDIT = 'form-edit-role'
        , ID_MODAL_EDIT = 'modal-edit-role'
        , ID_FORM_ACCESSRIGHT_REGISTER = 'form-add-access-rights'
        , ID_MODAL_ACCESSRIGHT_REGISTER = 'modal-add-access-rights'
        , ID_FORM_OWNERSHIP_REGISTER = 'form-add-ownership'
        , ID_MODAL_OWNERSHIP_REGISTER = 'modal-add-ownership'
          , ID_FORM_MANAGE_ROLE = 'form-manage-role'
          , ID_MODULE_VIEW_MANAGE = 'role-view-manage'
          , ID_MANAGE_ROLE_TXT_ROLE_NAME = 'txt-manage-role-name'
          , ID_MANAGE_ROLE_TXT_ROLE_STARTDATE = 'Manage_StartActiveDate'
          , ID_MANAGE_ROLE_TXT_ROLE_ENDDATE = 'Manage_EndActiveDate'
          , ID_MANAGE_ROLE_TXT_ROLE_ISACTIVE = 'txt-manage-role-isactive'
          , ID_MANAGE_ROLE_BTN_EDIT_ROLE = 'btn-manage-edit-role-name'
          , ID_MANAGE_ROLE_BTN_SAVE_ROLE = 'btn-manage-save-role-name'
          , ID_MANAGE_ROLE_BTN_CANCEL_ROLE = 'btn-manage-cancel-role-name'
          , ID_MANAGE_ROLE_BTN_ADD_ACCESSRIGHT = 'btn-add-accessright'
          , ID_BTN_GRID_OWNERSHIP_ADD = 'btn-add-ownership'
          , ID_ROLE_EDIT_ISACTIVE = 'txt-edit-role-isactive'
          , ID_ROLE_EDIT_NAME = 'txt-role-edit-name'
          , ID_ROLE_EDIT_STARTDATE = 'txt-role-edit-startdate'
          , ID_ROLE_EDIT_ENDDATE = 'txt-role-edit-enddate'

          , CSS_CLASS_MEL_SECTION = 'mel-section'
          , CSS_CLASS_MEL_ACTIVE_SECTION = 'mel-active-section'
          , CSS_CLASS_LEFT_EDGE_SHADOW = 'left-edge-shadow'
          , CSS_CLASS_MSG_REMOVE_CONFIRM = 'mel-dialog-confirm-msg'
          , CSS_CLASS_CELL_ERROR = 'cell-unknown-ref'
          , CSS_CLASS_GRID_BTN_OWNERSHIP_REMOVE = 'btn-ownership-remove'

          , CSS_CLASS_SECTION_TRIGGER = 'mel-section-trigger'

          , URL_ACCESS_MANAGE_POST = '/Role/UpdateRole'

          , global_records_per_page = 25

          , contentView = null
          , contentRegister = null
          , contentManage = null
          , currentView = null

          //----------------------- OLD
        , ID_MODULE_VIEW_ADD_PERMISSION = 'module-view-manage-permission'
        , ID_BTN_ADD_PERMISSION = 'btn-add-permission'
        , ID_BTN_ADD_PERMISSION_CLOSE = 'btn-add-permission-close'

        , ID_DIALOG_CONFIRM = 'mel-dialog-confirm'
    ;


    function init() {
        contentView = _roleView();
        contentRegister = _roleRegister();
        contentManage = _roleManage();

        wireupEvent();

        contentView.focus();

        $(MEL.toSelector(ID_MEL_LAYER)).css('display', '');
    }
    function wireupEvent() {

    }

    function activeSection(id) {

        //console.log(id);
        if (!id) {
            contentView.focus();
            return;
        }

        //$(MEL.toClass(CSS_CLASS_MEL_SECTION)).css('display', 'none').stop(true).removeClass(CSS_CLASS_LEFT_EDGE_SHADOW);

        //$(MEL.toSelector(id)).css({
        //    opacity: .1
        //    , marginLeft: '+=400px'
        //    , display: ''
        //})
        //    //.addClass(CSS_CLASS_LEFT_EDGE_SHADOW)
        //    .animate({
        //        marginLeft: 0
        //        , opacity: 1
        //    }
        //  , 300, function () {
        //      //$(this).removeClass(CSS_CLASS_LEFT_EDGE_SHADOW);
        //      var txtbox = $('#mel-layer input[type=text]:not(:disabled)');
        //      if (txtbox.length) {
        //          txtbox.first().focus();
        //      }
        //  });


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

    function updateRole(rolePvid,bIsActive,sFormId,callback) {
        var form = $(MEL.toSelector(sFormId));
        if (!form.valid()) {
            $('.input-validation-error').first().focus();
            return;
        }

        var dto = MEL.toDTO(form);
        dto['Manage.RolePvid'] = rolePvid;
        dto['Manage.IsActive'] = bIsActive;

        MEL.load({
            url: URL_ACCESS_MANAGE_POST
            , dto: dto
            , callback: function (result) {
                if (typeof callback == 'function') callback.call(this);
            }
        });
    }

    function _roleView() {
        var
              URL_ROLE_VIEW = '/Role/GetRoles'
            , URL_ROLE_REMOVE = '/Role/RemoveRole'
            , COL_ROLE_INDEX_SN = 0
            , COL_ROLE_INDEX_NAME = 1
            , COL_ROLE_INDEX_STARTDATE = 2
            , COL_ROLE_INDEX_ENDDATE = 3
            , COL_ROLE_INDEX_STATUS = 4
            , COL_ROLE_INDEX_CONTROLS = 5

            , CSS_CLASS_GRID_BTN_REMOVE = 'grid-btn-remove'
            , CSS_CLASS_GRID_BTN_EDIT = 'grid-btn-edit'
            , CSS_CLASS_GRID_BTN_MANAGE = 'grid-btn-manage'


            , records_per_page = 25
            , contenttable = null
            , dialogConfirmRemove = null
        ;

        function init() {
            initContentGrid();

            wireupEvent();
        }
        function initContentGrid() {
            contenttable = $(MEL.toSelector(ID_ROLE_VIEW_TABLE)).dataTable({
                'bServerSide': true
                , 'sAjaxSource': URL_ROLE_VIEW
                , 'iDisplayLength': records_per_page
                , 'sAjaxDataProp': 'data'
                , 'sPaginationType': 'full_numbers'
                , 'bPaginate': true
                , 'bSort': false
                , 'bJQueryUI': false
                , 'autoWidth': false
                , "bProcessing": true
                , 'oLanguage': {
                    sEmptyTable: 'No records found.'
                    , "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
                }
                , 'fnServerData': function (sSource, aoData, fnCallback, oSettings) {
                    oSettings.jqXHR = $.ajax({
                        type: 'post',
                        datatype: 'json',
                        url: sSource,
                        data: aoData,
                        success: function (result) {
                            if (result && typeof result.success != 'undefined' && !result.success) {
                                var msg = String.format('{0}.General error', URL_ROLE_VIEW);
                                if (result.exception) msg = String.format('{0} - {1}', URL_ROLE_VIEW, result.exception.message);
                                MEL.log(msg);
                                return;
                            }

                            fnCallback.call(this, result);
                        }
                    });
                }
                , "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [COL_ROLE_INDEX_SN] }
                , {
                    'aTargets': [COL_ROLE_INDEX_STARTDATE],
                    'bSearchable': false,
                    'sType': 'date',
                    'mRender': function (data, type, full) {
                        //console.log("datee");
                        //console.log(data);
                        var startdate = JSON.dateStringToDate(data.startActiveDate)
                        return startdate.format('dd/MM/yyyy');
                    }
                }
                , {
                    'aTargets': [COL_ROLE_INDEX_ENDDATE],
                    'bSearchable': false,
                    'sType':'date',
                    'mRender': function (data, type, full) {
                        var enddate = JSON.dateStringToDate(data.endActiveDate)
                        if ((enddate.getFullYear() - (new Date).getFullYear()) >= 100) return '-';
                        return enddate.format('dd/MM/yyyy');
                    }
                }
                , {
                    'aTargets': [COL_ROLE_INDEX_STATUS],
                    'mRender': function (data, type, full) {
                        return full.isActive == true ? 'Active' : 'Inactive';
                    }
                }
                , {
                    'aTargets': [COL_ROLE_INDEX_CONTROLS],
                    'mRender': function (data, type, full) {
                        return String.format('<a class="{5} mel-disable" data-rol="{0}" data-permission-id="{6}" data-permission-act="mel-enable|mel-disable" title="Edit Role"><i class="fa fa-edit"></i></a>&nbsp' +
                                            '<a class="{3} mel-disable" data-rol="{0}" data-permission-id="{1}" data-permission-act="mel-enable|mel-disable" title="Manage Role"><i class="fa fa-cog"></i></a>&nbsp' +
                                                '<a class="{4} mel-disable" data-rol="{0}" data-permission-id="{2}" data-permission-act="mel-enable|mel-disable" title="Remove Role" ><i class="fa fa-ban"></i></a>&nbsp'
                                                , data.pvid
                                                , pms.keys.role.manageRole
                                                , pms.keys.role.removeRole
                                                , CSS_CLASS_GRID_BTN_MANAGE
                                                , CSS_CLASS_GRID_BTN_REMOVE
                                                , CSS_CLASS_GRID_BTN_EDIT
                                                , pms.keys.role.editRole
                                                );
                    }
                }
                ]
                , 'fnDrawCallback': function (oSettings) {
                    /* Need to redo the counters if filtered or sorted */
                    //if (oSettings.bSorted || oSettings.bFiltered) {
                    //    for (var i = 0, iLen = oSettings.aiDisplay.length ; i < iLen ; i++) {
                    //        $(String.format("td:eq({0})", COL_ROLE_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                    //    }
                    //}
                    for (var i = 0, iLen = oSettings.aiDisplay.length ; i < iLen ; i++) {
                        $(String.format("td:eq({0})", COL_ROLE_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + oSettings._iDisplayStart + 1);
                    }

                    wireupEventGrid();
                }
                , 'aoColumns': [
                    { 'sWidth': '25px', 'mData': null }
                    , { 'mData': 'name' }
                    , { 'sWidth': '90px','mData': null }
                    , { 'sWidth': '90px', 'mData': null }
                    , { 'sWidth': '80px', 'mData': null }
                    , { 'sWidth': '70px', 'mData': null }
                ]
            });
        }
        function refreshGrid() {
            contenttable.api().ajax.reload();
        }

        function removeRole(role) {
            MEL.load({
                url: URL_ROLE_REMOVE
                , dto: role
                , callback: function (data) {

                    bootbox.alert({
                        centerVertical: true,
                        size: "small",
                        message: String.format('The role ({0}) has been removed successfully.', role.name),
                        callback: function () {
                            refreshGrid();
                        }
                    });

                }
            });
        }

        function wireupEvent() {
            var btnadd = $(MEL.toSelector(ID_BTN_ADD_ROLE));

            btnadd.off();

            btnadd.click(function (e) {
                e.preventDefault();
                contentRegister.focus();
            });
        }
        function wireupEventGrid() {
            var btnedit = contenttable.find((MEL.toClass(CSS_CLASS_GRID_BTN_EDIT)));
            var btnremove = contenttable.find((MEL.toClass(CSS_CLASS_GRID_BTN_REMOVE)));
            var btnmanage = contenttable.find(MEL.toClass(CSS_CLASS_GRID_BTN_MANAGE));

            btnedit.off();
            btnremove.off();
            btnmanage.off();

            btnedit.click(function () {
                var id = $(this).data('rol');
                var list = contenttable.fnGetData();
                var currentRole = MEL.getByValue(list, 'pvid', id);
                
                var editform = $(MEL.toSelector(ID_FORM_EDIT));
                var txtmodulename = $(MEL.toSelector(ID_ROLE_EDIT_NAME));
                var txtstartdate = editform.find(MEL.toSelector(ID_MANAGE_ROLE_TXT_ROLE_STARTDATE));
                var txtenddate = editform.find(MEL.toSelector(ID_MANAGE_ROLE_TXT_ROLE_ENDDATE));
                var chkstatus = $(MEL.toSelector(ID_ROLE_EDIT_ISACTIVE));

                txtmodulename.val(currentRole.name);
                txtstartdate.val(JSON.dateStringToDate(currentRole.startActiveDate).format('yyyy-MM-dd'));
                txtenddate.val(JSON.dateStringToDate(currentRole.endActiveDate).format('yyyy-MM-dd'));

                chkstatus.prop('checked', currentRole.isActive);

                $(MEL.toSelector(ID_FORM_EDIT)).on('submit', function (e) {

                    e.preventDefault();

                    updateRole(
                        currentRole.pvid,
                        $(MEL.toSelector(ID_ROLE_EDIT_ISACTIVE)).is(':checked'),
                        ID_FORM_EDIT,
                        function () {

                            $(MEL.toSelector(ID_MODAL_EDIT)).modal('hide');

                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: "Role has been updated successfully.",
                                callback: function () {
                                    contentView.focus();
                                }
                            });

                        });
                });

                $(MEL.toSelector(ID_MODAL_EDIT)).modal('show');
                $(MEL.toSelector(ID_ROLE_EDIT)).css('display', 'block');

       
            });
            btnremove.click(function () {
                var id = $(this).data('rol');
                var list = contenttable.fnGetData();
                var role = MEL.getByValue(list, 'pvid', id);

                bootbox.confirm({
                    centerVertical: true,
                    size: "small",
                    message: "Do you wish to remove the role (" + role.name + ") ?",
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
                            removeRole(role);
                            return true;
                        }
                        else {
                            return true;
                        }
                    }
                });

            });
            btnmanage.click(function () {
                var id = $(this).data('rol');
                var list = contenttable.fnGetData();
                var role = MEL.getByValue(list, 'pvid', id);
                contentManage.focus({ role: role });
            });
            

            MEL.validate();
        }

        var _inst = {
            refresh: function () {
                refreshGrid();
            },
            focus: function (opt) {
                //console.log(opt);
                refreshGrid();
                activeSection(ID_ROLE_VIEW);
                currentView = this;
            },
            blur: function () {

            }
        };

        init();
        return _inst;
    }
    function _roleRegister() {
        var
            verifysubmit = 0
            ,URL_REGISTER_POST = '/Role/Register'

            , ID_CHECKBOX_ISACTIVE = 'txt-register-role-isactive'
            , isModal = true
        ;

        function init() {
            wireupEvent();
        }
        function wireupEvent() {
            var btnregister = $(MEL.toSelector(ID_BTN_REGISTER_ROLE));
            var btncancel = $(MEL.toSelector(ID_BTN_REGISTER_CANCEL_ROLE));

            btnregister.off();
            btncancel.off();

            //btnregister.click(function () {
            //    if (!isActiveSection(_inst) && !isModal) return false;
            //    doregister();
            //});
            btncancel.click(function () {
                _inst.blur();
            });
        }

        function clearForm() {
            var form = $(MEL.toSelector(ID_FORM_REGISTER));
            form.formReset();
        }
        function doregister(callback) {
            var form = $(MEL.toSelector(ID_FORM_REGISTER));
            if (!form.valid()) {
                $('.input-validation-error').first().focus();
                return;
            }
            var dto = MEL.toDTO(form);
            dto['Register.IsActive'] = $(MEL.toSelector(ID_CHECKBOX_ISACTIVE)).is(':checked');

            MEL.load({
                url: URL_REGISTER_POST
                , dto: dto
                , callback: function (result) {

                    if (typeof callback == 'function') callback.call(this);

                }
            });
        }

        $(MEL.toSelector(ID_FORM_REGISTER)).on('submit', function (e) {

            e.preventDefault();

            if (!isActiveSection(_inst) && !isModal) return false;
            doregister(function () {


                $(MEL.toSelector(ID_MODAL_REGISTER)).modal('hide');

                bootbox.alert({
                    centerVertical: true,
                    size: "small",
                    message: "Role has been added successfully.",
                    callback: function () { _inst.blur(); }
                });


            });

        });

        var _inst = {
            refresh: function () {

            },
            focus: function (opt) {
                //activeSection(ID_ACCESS_VIEW_REGISTER);
                //currentView = this;

                if (isModal) {                    

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
    function _roleManage() {

        var
              COL_ROLEACCESS_INDEX_SN = 0
            , COL_ROLEACCESS_INDEX_MODULE = 1
            , COL_ROLEACCESS_INDEX_ACCESSLEVEL = 2
            , COL_ROLEACCESS_INDEX_GRANTEDBY = 3
            , COL_ROLEACCESS_INDEX_GRANTEDDATE = 4
            , COL_ROLEACCESS_INDEX_CONTROLS = 5

            , COL_OWN_INDEX_SN = 0
            , COL_OWN_INDEX_LOCATION = 1
            , COL_OWN_INDEX_GRANTEDBY = 2
            , COL_OWN_INDEX_GRANTEDAT = 3
            , COL_OWN_INDEX_CONTROLS = 4

            , CSS_CLASS_GRID_BTN_REMOVE = 'grid-button-remove'
            , CSS_CLASS_DETAIL_ROLE_MANAGE = 'role-manage-detail'
            , CSS_CLASS_GRID_BTN_REMOVEACCESS = 'grid-button-remove-access'

            , URL_ROLEACCESS_VIEW = '/Role/GetRoleAccessDetails'
            , URL_ROLEACCESS_REMOVE = '/Role/RemoveAccessRight'
            , URL_OWNERSHIP_ADD = '/Role/AddOwnershipAccess'
            , URL_OWNERSHIP_VIEW = '/Role/GetOwnerships'
            , URL_OWNERSHIP_REMOVE = '/Role/RemoveOwnership'

            , currentRole = null
            , availablePermissions = []
            , contentNewAccessRight = null

            , contenttable = null
            , ownershiptable = null
            , dialogConfirmRemove = null
        ;

        function init() {
            contentNewAccessRight = _newAccessRightView();

            initContentGrid();
            initOwnershipGrid();

            wireupEvent();
        }
        function initView() {
            var txtmodulename = $(MEL.toSelector(ID_MANAGE_ROLE_TXT_ROLE_NAME));
            var txtstartdate = $(MEL.toSelector(ID_MANAGE_ROLE_TXT_ROLE_STARTDATE));
            var txtenddate = $(MEL.toSelector(ID_MANAGE_ROLE_TXT_ROLE_ENDDATE));
            var chkstatus = $(MEL.toSelector(ID_MANAGE_ROLE_TXT_ROLE_ISACTIVE));

            txtmodulename.val(currentRole.name);
            txtstartdate.val(JSON.dateStringToDate(currentRole.startActiveDate).format('yyyy-MM-dd'));
            txtenddate.val(JSON.dateStringToDate(currentRole.endActiveDate).format('yyyy-MM-dd'));

            if (currentRole.isActive) chkstatus.closest('.icheckbox_minimal').addClass('checked');
            else chkstatus.closest('.icheckbox_minimal').removeClass('checked');
            chkstatus[0].checked = currentRole.isActive;
        }
        function initContentGrid() {
            
            contenttable = $(MEL.toSelector(ID_MODULE_ACCESSRIGHT_VIEW_TABLE)).dataTable({
                'bServerSide': true
                , 'sAjaxSource': URL_ROLEACCESS_VIEW
                , 'iDisplayLength': global_records_per_page
                , 'sAjaxDataProp': 'data'
                , 'sPaginationType': 'full_numbers'
                , 'deferLoading': 0
                , 'bPaginate': true
                , 'bSort': false
                , 'bJQueryUI': false
                , 'autoWidth': false
                , "bProcessing": true
                , 'oLanguage': {
                    sEmptyTable: 'No records found.'
                    , "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
                }
                , 'fnServerData': function (sSource, aoData, fnCallback, oSettings) {
                    //console.log(".");
                    //console.log(currentRole);
                    aoData.push({
                        name: 'rolePvid',
                        value: (!currentRole ? 0 : currentRole.pvid)
                        
                    })

                    oSettings.jqXHR = $.ajax({
                        type: 'post',
                        datatype: 'json',
                        url: sSource,
                        data: aoData,
                        success: function (result) {
                            if (result && typeof result.success != 'undefined' && !result.success) {
                                var msg = String.format('{0}.General error', URL_ROLEACCESS_VIEW);
                                if (result.exception) msg = String.format('{0} - {1}', URL_ROLEACCESS_VIEW, result.exception.message);
                                MEL.log(msg);
                                return;
                            }

                            fnCallback.call(this, result);
                        }
                    });
                }
                , "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [COL_ROLEACCESS_INDEX_SN], 'sWidth': '25px', 'mData': null }
                    ,
                    {
                        'aTargets': [COL_ROLEACCESS_INDEX_CONTROLS],
                        'mRender': function (data, type, full) {
                            return String.format('<a class="{1} mel-disable" data-rolacc="{0}" data-permission-id="{2}" data-permission-act="mel-enable|mel-disable" title="Remove Access Right" ><i class="fa fa-ban"></i></a>&nbsp',
                                full.pvid,
                                CSS_CLASS_GRID_BTN_REMOVEACCESS,
                                pms.keys.role.removeRoleAccess);
                        }
                    }
                    ,
                    {
                        'aTargets': [COL_ROLEACCESS_INDEX_GRANTEDDATE],
                        'mRender': function (data, type, full) {
                            
                            var grantedDate = JSON.dateStringToDate(data);
                            return grantedDate.format('dd/MM/yyyy');
                        }
                    }
                ]
                , 'fnDrawCallback': function (oSettings) {
                    /* Need to redo the counters if filtered or sorted */
                    //if (oSettings.bSorted || oSettings.bFiltered) {
                    //    for (var i = 0, iLen = oSettings.aiDisplay.length ; i < iLen ; i++) {
                    //        $(String.format("td:eq({0})", COL_ROLEACCESS_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                    //    }
                    //}
                    for (var i = 0, iLen = oSettings.aiDisplay.length ; i < iLen ; i++) {
                        $(String.format("td:eq({0})", COL_ROLEACCESS_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + oSettings._iDisplayStart + 1);
                    }

                    wireupEventGrid();
                }
                /*
                , 'createdRow': function (row, data, index) {
                    var perms = availablePermissions[data.PermissionKey];
                    if (!perms) {
                        $('td', row).addClass(CSS_CLASS_CELL_ERROR).text(data.PermissionKey);
                    }
                }
                */
                , 'aoColumns': [
                      { 'sWidth': '25px', 'mData': null }
                    , { 'mData': "module.name" } // Title
                    , { 'mData': "accessLevel.name" } // Desc
                    , { 'sWidth': '100px', 'mData': "grantedBy.userName" }
                    , { 'sWidth': '120px', 'mData': "grantedDate" }
                    , { 'sWidth': '50px', 'mData': null }
                ]
            });
        }
        function initOwnershipGrid() {
            ownershiptable = $(MEL.toSelector(ID_MODULE_OWNERSHIP_TABLE)).dataTable({
                'bServerSide': true
                , 'sAjaxSource': URL_OWNERSHIP_VIEW
                , 'iDisplayLength': global_records_per_page
                , 'sAjaxDataProp': 'data'
                , 'sPaginationType': 'full_numbers'
                , 'deferLoading': 0
                , 'bPaginate': true
                , 'bSort': false
                , 'bJQueryUI': false
                , 'autoWidth': false
                , "bProcessing": true
                , 'oLanguage': {
                    sEmptyTable: 'No records found.'
                    , "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
                }
                , 'fnServerData': function (sSource, aoData, fnCallback, oSettings) {

                    aoData.push({
                        name: 'rolePvid',
                        value: (!currentRole ? 0 : currentRole.pvid)
                    })

                    oSettings.jqXHR = $.ajax({
                        type: 'post',
                        datatype: 'json',
                        url: sSource,
                        data: aoData,
                        success: function (result) {
                            if (result && typeof result.success != 'undefined' && !result.success) {
                                var msg = String.format('{0}.General error', URL_ROLEACCESS_VIEW);
                                if (result.exception) msg = String.format('{0} - {1}', URL_ROLEACCESS_VIEW, result.exception.message);
                                MEL.log(msg);
                                return;
                            }

                            fnCallback.call(this, result);
                        }
                    });
                }
                , "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [COL_OWN_INDEX_SN], 'sWidth': '25px', 'mData': null }
                    ,
                    {
                        'aTargets': [COL_OWN_INDEX_LOCATION],
                        'mRender': function (data, type, full) {
                            return String.format('{0}', data.location.name);
                        }
                    },
                    {
                        'aTargets': [COL_OWN_INDEX_CONTROLS],
                        'mRender': function (data, type, full) {
                            return String.format(
                                '<a class="{1} mel-disable" data-own="{0}" data-permission-id="{2}" data-permission-act="mel-enable|mel-disable" title="Remove Ownership" ><i class="fa fa-ban"></i></a>&nbsp',
                                full.pvid,
                                CSS_CLASS_GRID_BTN_OWNERSHIP_REMOVE,
                                pms.keys.role.removeOwnershipAccess);
                        }
                    }
                    ,
                    {
                        'aTargets': [COL_OWN_INDEX_GRANTEDAT],
                        'mRender': function (data, type, full) {
                            var grantedDate = JSON.dateStringToDate(data);
                            return grantedDate.format('dd/MM/yyyy');
                        }
                    }
                ]
                , 'fnDrawCallback': function (oSettings) {
                    /* Need to redo the counters if filtered or sorted */
                    //if (oSettings.bSorted || oSettings.bFiltered) {
                    //    for (var i = 0, iLen = oSettings.aiDisplay.length ; i < iLen ; i++) {
                    //        $(String.format("td:eq({0})", COL_ROLEACCESS_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                    //    }
                    //}
                    for (var i = 0, iLen = oSettings.aiDisplay.length ; i < iLen ; i++) {
                        $(String.format("td:eq({0})", COL_ROLEACCESS_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + oSettings._iDisplayStart + 1);
                    }

                    wireupEventOwnershipGrid();
                }
                , 'aoColumns': [
                      { 'sWidth': '25px', 'mData': null }
                    , { 'mData': null } // Title
                    , { 'sWidth': '100px', 'mData': "grantedBy.userName" }
                    , { 'sWidth': '120px', 'mData': "grantedDate" }
                    , { 'sWidth': '50px', 'mData': null }
                ]
            });
        }

        function refreshGrid() {
            contenttable.api().ajax.reload();
            ownershiptable.api().ajax.reload();
        }
        function setMode(editmode) {
            if (editmode) {
                $(MEL.toClass(CSS_CLASS_DETAIL_ROLE_MANAGE)).css('display', '');
                $(MEL.toSelector(ID_MANAGE_ROLE_TXT_ROLE_NAME)).removeAttr('disabled').select();
                $(MEL.toSelector(ID_MANAGE_ROLE_BTN_EDIT_ROLE)).css('display', 'none');
                $(MEL.toSelector(ID_MANAGE_ROLE_BTN_SAVE_ROLE)).css('display', '');
                $(MEL.toSelector(ID_MANAGE_ROLE_BTN_CANCEL_ROLE)).css('display', '');
            }
            else {
                $(MEL.toClass(CSS_CLASS_DETAIL_ROLE_MANAGE)).css('display', 'none');
                $(MEL.toSelector(ID_MANAGE_ROLE_TXT_ROLE_NAME)).attr('disabled', 'disabled');
                $(MEL.toSelector(ID_MANAGE_ROLE_BTN_EDIT_ROLE)).css('display', '');
                $(MEL.toSelector(ID_MANAGE_ROLE_BTN_SAVE_ROLE)).css('display', 'none');
                $(MEL.toSelector(ID_MANAGE_ROLE_BTN_CANCEL_ROLE)).css('display', 'none');
            }
        }

        function doSave() {
            //var form = $(MEL.toSelector(ID_FORM_MANAGE_ROLE));
            //if (!form.valid()) {
            //    $('.input-validation-error').first().focus();
            //    return;
            //}

            //var dto = MEL.toDTO(form);
            //dto['Manage.RolePvid'] = currentRole.Pvid;
            //dto['Manage.IsActive'] = $(MEL.toSelector(ID_MANAGE_ROLE_TXT_ROLE_ISACTIVE)).is(':checked');

            //MEL.load({
            //    url: URL_ACCESS_MANAGE_POST
            //    , dto: dto
            //    , callback: function (result) {
            //        //window.alert('Successfully save change of the Module.');
            //        //_inst.blur();
            //        setMode(false);
            //    }
            //});

            updateRole(currentRole.pvid, $(MEL.toSelector(ID_MANAGE_ROLE_TXT_ROLE_ISACTIVE)).is(':checked'), ID_FORM_MANAGE_ROLE, function () {
                setMode(false);
            });
        }
        function removeRoleAccess(access) {
            MEL.load({
                url: URL_ROLEACCESS_REMOVE
                , dto: access
                , callback: function (data) {

                    bootbox.alert({
                        centerVertical: true,
                        size: "small",
                        message: "The access rights has been removed successfully.",
                        callback: function () {
                            refreshGrid();
                        }
                    });
                }
            });
        }

        function wireupEvent() {
            var btnAddAccessRight = $(MEL.toSelector(ID_MANAGE_ROLE_BTN_ADD_ACCESSRIGHT));
            var btnEditModuleName = $(MEL.toSelector(ID_MANAGE_ROLE_BTN_EDIT_ROLE));
            var btnCancelModuleName = $(MEL.toSelector(ID_MANAGE_ROLE_BTN_CANCEL_ROLE));
            var btnSaveName = $(MEL.toSelector(ID_MANAGE_ROLE_BTN_SAVE_ROLE));
            var btnRoleBack = $(MEL.toSelector(ID_BTN_ROLE_ACCESSRIGHT_BACK));
            var btnAddOwnership = $(MEL.toSelector(ID_BTN_GRID_OWNERSHIP_ADD));

            btnAddAccessRight.off();
            btnEditModuleName.off();
            btnCancelModuleName.off();
            btnSaveName.off();
            btnRoleBack.off();
            btnAddOwnership.off();

            btnAddAccessRight.click(function () {
                if (currentView != _inst) return;
                contentNewAccessRight.focus({
                    role: currentRole,
                });
            });

            btnEditModuleName.click(function () {
                setMode(true);
            });
            btnCancelModuleName.click(function () {
                $(MEL.toSelector(ID_MANAGE_ROLE_TXT_ROLE_NAME)).val(currentRole.name);
                initView();
                setMode(false);
            });
            btnSaveName.click(function () {
                if (currentView != _inst) return;
                doSave();
            });
            btnRoleBack.click(function () {
                _inst.blur();
            });
            btnAddOwnership.click(function () {

                $(MEL.toSelector(ID_FORM_OWNERSHIP_REGISTER)).on('submit', function (e) {

                    e.preventDefault();

                    var form = $(MEL.toSelector(ID_FORM_OWNERSHIP_REGISTER));
                    if (!form.valid()) {
                        $('.input-validation-error').first().focus();
                        return;
                    }

                    var dto = MEL.toDTO(form);
                    dto['AddOwnership.RolePvid'] = currentRole.pvid;

                    $(MEL.toSelector(ID_MODAL_OWNERSHIP_REGISTER)).modal('hide');

                    MEL.load({
                        url: URL_OWNERSHIP_ADD
                        , dto: dto
                        , callback: function (result) {

                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: "Ownership has been added successfully.",
                                callback: function () { refreshGrid(); }
                            });
                        }
                    });


                });

                $(MEL.toSelector(ID_MODAL_OWNERSHIP_REGISTER)).modal('show');
                $(MEL.toSelector(ID_ROLE_OWNERSHIP_NEW)).css('display', 'block');

            });
        }
        function wireupEventGrid() {
            var btnRemoveAccess = $(MEL.toClass(CSS_CLASS_GRID_BTN_REMOVEACCESS));

            btnRemoveAccess.off();

            btnRemoveAccess.click(function () {
                var id = $(this).data('rolacc');
                var list = contenttable.fnGetData();
                var roleAccess = MEL.getByValue(list, 'pvid', id);

                bootbox.confirm({
                    centerVertical: true,
                    size: "small",
                    message: "Do you wish to remove the access right (" + roleAccess.module.name + " - " + roleAccess.accessLevel.name +") ?",
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
                            removeRoleAccess(roleAccess);
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
        function wireupEventOwnershipGrid() {
            var btnremove = $(MEL.toClass(CSS_CLASS_GRID_BTN_OWNERSHIP_REMOVE));

            btnremove.off();

            btnremove.click(function () {
                var id = $(this).data('own');
                var list = ownershiptable.fnGetData();
                var ownership = MEL.getByValue(list, 'pvid', id);

                var cloneown = {
                    Pvid: ownership.pvid,
                    IdentityRolePvid: ownership.identityRolePvid,
                    GrantedByIdentityUserPvid: ownership.grantedByIdentityUserPvid,
                    GrantedDate: ownership.grantedDate
                };

                bootbox.confirm({
                    centerVertical: true,
                    size: "small",
                    message: "Do you wish to remove the ownership (" + ownership.location.name + ") ?",
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

                            MEL.load({
                                url: URL_OWNERSHIP_REMOVE,
                                dto: cloneown,
                                callback: function () {
                                    bootbox.alert({
                                        centerVertical: true,
                                        size: "small",
                                        message: "Ownership has been removed successfully.",
                                        callback: function () {
                                            refreshGrid();
                                        }
                                    });
                                }
                            });

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
                //console.log("..");
                //console.log(opt);
                currentRole = opt.role;
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

    function _newAccessRightView() {
        var
            URL_REGISTER_POST = '/Role/AddAccessRight'

            , ID_CHECKBOX_ISACTIVE = 'txt-register-role-isactive'

            , isModal = true
            , currentRole = null
        ;

        function init() {
            wireupEvent();
        }
        function initView() {
            var title = $(MEL.toSelector(ID_ACCESSRIGHT_TITLE));
            title.text(currentRole.name);
        }
        function wireupEvent() {
            var btnregister = $(MEL.toSelector(ID_BTN_REGISTER_ACCESSRIGHT));
            var btncancel = $(MEL.toSelector(ID_BTN_ACCESSRIGHT_CANCEL_ROLE));

            btnregister.off();
            btncancel.off();

            //btnregister.click(function () {
            //    if (!isActiveSection(_inst) && !isModal) return false;
            //    doregister();
            //});
            btncancel.click(function () {
                _inst.blur();
            });
        }

        function clearForm() {
            var form = $(MEL.toSelector(ID_FORM_ACCESSRIGHT_REGISTER));
            form.formReset();
        }
        function doregisterAcc(callback) {
            //console.log(callback);
            var form = $(MEL.toSelector(ID_FORM_ACCESSRIGHT_REGISTER));
            if (!form.valid()) {
                $('.input-validation-error').first().focus();
                return;
            }

            var dto = MEL.toDTO(form);
            dto['AddAccessRight.RolePvid'] = currentRole.pvid;
            //dto['Register.IsActive'] = $(MEL.toSelector(ID_CHECKBOX_ISACTIVE)).is(':checked');

            MEL.load({
                url: URL_REGISTER_POST
                , dto: dto
                , callback: function (result) {
                    //console.log(result);
                    //window.alert('Successfully register new Role.');
                    //_inst.blur();
                    if (typeof callback == 'function') callback.call(this);
                }
            });
        }


        var _inst = {
            refresh: function () {

            },
            focus: function (opt) {
                currentRole = opt.role;
                //activeSection(ID_ACCESSRIGHT_NEW);
                //currentView = this;
                initView();

                if (isModal) {

                    $(MEL.toSelector(ID_FORM_ACCESSRIGHT_REGISTER)).off('submit').on('submit', function (e) {

                        e.preventDefault();

                        if (!isActiveSection(_inst) && !isModal) return false;

                        doregisterAcc(function () {

                            $(MEL.toSelector(ID_MODAL_ACCESSRIGHT_REGISTER)).modal('hide');

                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: "Access right has been added successfully.",
                                callback: function () { _inst.blur(); }
                            });


                        });

                    });

                    $(MEL.toSelector(ID_MODAL_ACCESSRIGHT_REGISTER)).modal('show');
                    $(MEL.toSelector(ID_ACCESSRIGHT_NEW)).css('display', 'block');

                }
                else {
                    activeSection(ID_ACCESSRIGHT_NEW);
                    currentView = this;
                }
            },
            blur: function () {
                clearForm();
                contentManage.focus({
                    role:currentRole
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