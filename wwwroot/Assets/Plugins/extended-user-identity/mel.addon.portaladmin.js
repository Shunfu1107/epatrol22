/// <reference path="mel.core.js" />
/// <reference path="../bootbox/dist/bootbox.js" />

$(window).on("load", function () {
    MEL.startup(function () {
        pms.users();
    });
});



pms.users = function () {
    var
        ID_USER_VIEW = 'users-view-grid'
        , ID_USER_VIEW_REGISTER = 'users-view-register'
        , ID_USER_VIEW_ACCESSRIGHTS = 'users-view-accessrights'
        , ID_USER_VIEW_EDIT = 'users-view-edit'
        , ID_USER_VIEW_ROLE = 'user-add-role'
        , ID_USER_VIEW_REST = 'user-add-rest'
        , ID_USER_ADD_EXCLUSIVE = 'user-add-exclusive'
        , ID_USER_EDIT_EXCLUSIVE = 'user-edit-exclusive'
        , ID_TABLE = 'users-table'
        , ID_TABLE_ACCESSRIGHT = 'users-accessrights-table'
        , ID_TABLE_RESTAURANT = 'users-restaurant-table'
        , ID_TABLE_EXCLUSIVE_ACCESS = 'users-exclusiveaccess-table'
        , ID_BUTTON_ADD_USER = 'btn-add-user'
        , ID_MEL_LAYER = 'mel-layer'
        , ID_FORM_REGISTER = 'form-register'
        , ID_FORM_EDIT_USER = 'form-edit-user'
        , ID_FORM_ADD_ROLE = 'form-add-role'
        , ID_FORM_ADD_REST = 'form-add-rest'
        , ID_MODAL_ADD_ROLE = 'modal-add-role'
        , ID_MODAL_ADD_REST = 'modal-add-rest'
        , ID_FORM_ADD_EXCLUSIVE = 'form-add-exclusive'
        , ID_MODAL_ADD_EXCLUSIVE = 'modal-add-exclusive'
        , ID_FORM_EDIT_EXCLUSIVE = 'form-edit-exclusive'
        , ID_MODAL_EDIT_EXCLUSIVE = 'modal-edit-exclusive'
        , ID_BTN_REGISTER_USER = 'btn-register-user'
        , ID_BTN_REGISTER_CANCEL = 'btn-register-cancel'
        , ID_BTN_EDIT_USER = 'btn-edit-user'
        , ID_BTN_EDIT_CANCEL = 'btn-edit-cancel'
        , ID_BTN_USERROLE_ADD = 'btn-add-role'
        , ID_BTN_REST_ADD = 'btn-add-restaurant'
        , ID_BTN_BACK_ROLE = 'btn-back-role'
        , ID_BTN_ADD_EXCLUSIVE = 'btn-add-exclusive-rights'
        , ID_BTN_FIND_PERMISSION_EXCLUSIVE = 'btn-access-find'
        , ID_BTN_FIND_EDIT_PERMISSION_EXCLUSIVE = 'btn-edit-access-find'
        , ID_CMB_MODULE_LIST = 'cmb-module-selection'
        , ID_CMB_EDIT_EXCLUSIVE_LOCATION = 'EditExclusiveAccess_LocationPvid'
        , ID_CMB_EDIT_EXCLUSIVE_ACCESSIBLE = 'EditExclusiveAccess_Accessible'
        , ID_DIALOG_CONFIRM = 'mel-dialog-confirm'
        , ID_LB_USER_NAME = 'lb-user-name'
        , ID_TXT_EXCLUSIVE_SELECTED = 'txt-access-selected'
        , ID_TXT_EDIT_EXCLUSIVE_SELECTED = 'txt-edit-access-selected'

        , CSS_CLASS_MEL_SECTION = 'mel-section'
        , CSS_CLASS_GRID_BTN_REMOVE = 'grid-btn-remove'
        , CSS_CLASS_GRID_ROLE_REMOVE = 'grid-btn-role-remove'
        , CSS_CLASS_GRID_REST_REMOVE = 'grid-btn-rest-remove'
        , CSS_BTN_GRID_EXCLUSIVE_EDIT = 'grid-btn-exclusive-edit'
        , CSS_BTN_GRID_EXCLUSIVE_REMOVE = 'grid-btn-exclusive-remove'
        , CSS_CLASS_GRID_BTN_EDIT = 'grid-btn-edit'
        , CSS_CLASS_GRID_BTN_MANAGE = 'grid-btn-manage'
        , CSS_CLASS_MEL_ACTIVE_SECTION = 'mel-active-section'
        , CSS_CLASS_LEFT_EDGE_SHADOW = 'left-edge-shadow'
        , CSS_CLASS_MSG_REMOVE_CONFIRM = 'mel-dialog-confirm-msg'

        , CSS_CLASS_SECTION_TRIGGER = 'mel-section-trigger'

        , global_records_per_page = 25

        , currentUserSelected = null
        , userView = null
        , userRegister = null
        , userEdit = null
        , userManageAccessRights = null
        , currentView = null
        ;

    function init() {
        userView = _userView();
        userRegister = _userRegister();
        userEdit = _userEdit();
        userManageAccessRights = _userManageAccessRights();

        wireupEvent();

        //activeSection(ID_USER_VIEW);
        userView.focus();

        $(MEL.toSelector(ID_MEL_LAYER)).css('display', '');
    }

    function wireupEvent() {
        /*
        var trigger = $(MEL.toClass(CSS_CLASS_SECTION_TRIGGER));

        trigger.off();

        trigger.click(function () {
            var self = $(this);
            activeSection(self.data('trigger'));
        });
        */
    }
    function validate() {
        wireupEvent();
        MEL.core.validate();
    }
    function activeSection(id) {

        if (!id) {
            userView.focus();
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
                    var txtbox = $('#mel-layer input[type=text]:not(:disabled)');
                    if (txtbox.length) {
                        txtbox.first().focus();
                    }
                });
    }
    function isActiveSection(oView) {
        return oView == currentView;
    }

    function _userView() {
        var
            URL_USERS_VIEW = '/PortalAdmin/GetUsers'
            , URL_DELETE_USER = '/PortalAdmin/RemoveUser'

            , COL_INDEX_SN = 0
            , COL_INDEX_NAME = 1
            , COL_INDEX_EMAIL = 2
            //, COL_INDEX_NRIC = 2
            //, COL_INDEX_MOBILE = 3
            , COL_INDEX_USERNAME = 3
            , COL_INDEX_CONTROLS = 5
            , COL_INDEX_RECEIVE = 4

            , records_per_page = 10
            , datatable = null
            , dialogConfirmRemove = null
            ;

        function initGrid() {
            datatable = $(MEL.toSelector(ID_TABLE)).dataTable({
                'bServerSide': true
                , 'sAjaxSource': URL_USERS_VIEW
                , 'iDisplayLength': global_records_per_page
                , 'sAjaxDataProp': 'data'
                , 'sPaginationType': 'full_numbers'
                , 'bPaginate': true
                , "bProcessing": true
                , 'oLanguage': {
                    sEmptyTable: 'No records found.'
                    , "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
                }
                , 'bSort': false
                , 'bJQueryUI': false
                , 'autoWidth': false
                , 'fnServerData': function (sSource, aoData, fnCallback, oSettings) {
                    //console.log(sSource);
                    oSettings.jqXHR = $.ajax({
                        //type: 'post',
                        datatype: 'json',
                        url: sSource,
                        data: aoData,
                        success: function (result) {
                            if (result && typeof result.success != 'undefined' && !result.success) {
                                var msg = String.format('{0}.General error', URL_USERS_VIEW);
                                if (result.exception) msg = String.format('{0} - {1}', URL_USERS_VIEW, result.exception.message);
                                MEL.log(msg);
                                window.alert(msg);
                                return;
                            }
                            fnCallback.call(this, result);
                            //console.log(result);
                        }
                    });
                }
                , "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [COL_INDEX_SN] }
                    , {
                        'aTargets': [COL_INDEX_CONTROLS], 'mRender': function (data, type, full) {
                            return String.format('<a class="{4}" data-usr="{0}" data-permission-id="{2}" data-permission-act="mel-enable|mel-disable" title="Edit User" ><i class="fa fa-edit"></i></a>&nbsp;' +
                                '<a class="{5}" data-usr="{0}" data-permission-id="{6}" data-permission-act="mel-enable|mel-disable" title="Manage Access Rights" ><i class="fa fa-cog"></i></a>&nbsp;' +
                                '<a class="{3}" data-usr="{0}" data-permission-id="{1}" data-permission-act="mel-enable|mel-disable" title="Remove User"><i class="fa fa-ban"></i></a>'
                                , data.userID
                                , pms.keys.user.removeUser
                                , pms.keys.user.editUser
                                , CSS_CLASS_GRID_BTN_REMOVE
                                , CSS_CLASS_GRID_BTN_EDIT
                                , CSS_CLASS_GRID_BTN_MANAGE
                                , pms.keys.user.manageAccessRights
                            );
                        }
                    }
                ]
                , 'fnDrawCallback': function (oSettings) {
                    /* Need to redo the counters if filtered or sorted */
                    //if (oSettings.bSorted || oSettings.bFiltered) {
                    //    for (var i = 0, iLen = oSettings.aiDisplay.length ; i < iLen ; i++) {
                    //        $(String.format("td:eq({0})", COL_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                    //    }
                    //}
                    for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                        $(String.format("td:eq({0})", COL_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + oSettings._iDisplayStart + 1);
                    }

                    wireupEventGrid();
                }
                , 'aoColumns': [
                    { 'sWidth': '25px', 'mData': null }
                    , { 'mData': 'name' }
                    , { 'mData': 'email' }
                    , { 'mData': 'userName' }
                    , { 'mData': 'receivedEmail'} 
                    , { 'sWidth': '80px', 'mData': null }
                ]
            });
        }
        function init() {
            initGrid();
            wireupEvent();
        }
        function refreshGrid() {
            datatable.api().ajax.reload();
        }

        function doremove(usr) {

            MEL.load({
                url: URL_DELETE_USER
                , dto: usr
                , callback: function (data) {
                    //window.alert(String.format('The user \'{0}\' has been deleted successfully.', usr.UserName));
                    //refreshGrid();

                    bootbox.alert({
                        centerVertical: true,
                        size: "small",
                        message: String.format('The user ({0}) has been removed successfully.', usr.userName),
                        callback: function () {
                            refreshGrid();
                        }
                    });


                }
            });
        }

        function wireupEvent() {
            var btnadduser = $(MEL.toSelector(ID_BUTTON_ADD_USER));

            btnadduser.off();

            btnadduser.click(function () {
                userRegister.focus();
            });
        }
        function wireupEventGrid() {
            var btnremove = $(MEL.toClass(CSS_CLASS_GRID_BTN_REMOVE));
            var btnedit = $(MEL.toClass(CSS_CLASS_GRID_BTN_EDIT));
            var btnmanage = $(MEL.toClass(CSS_CLASS_GRID_BTN_MANAGE));

            btnremove.off();
            btnedit.off();
            btnmanage.off();

            btnremove.click(function () {
                if (!isActiveSection(_inst)) return;
                if (dialogConfirmRemove) return;

                var list = datatable.fnGetData();
                var id = $(this).data('usr');

       

                var userremove = MEL.getByValue(list, 'id', id);

                dialogConfirmRemove = true;

                bootbox.confirm({
                    centerVertical: true,
                    size: "small",
                    message: "Do you wish to remove the user (" + userremove.userName + ") ?",
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
                            doremove(userremove);
                            dialogConfirmRemove = false;
                            return true;
                        }
                        else {
                            dialogConfirmRemove = false;
                            return true;
                        }
                    }
                });


            });

            btnedit.click(function () {
                if (!isActiveSection(_inst)) return;
                var id = $(this).data('usr');
                var list = datatable.fnGetData();
                var userediting = MEL.getByValue(list, 'id', id);
                currentUserSelected = userediting;
                userEdit.focus({ user: userediting });
            });

            btnmanage.click(function () {
                if (!isActiveSection(_inst)) return;
                var id = $(this).data('usr');
                var list = datatable.fnGetData();
                var usermanage = MEL.getByValue(list, 'id', id);
                currentUserSelected = usermanage;
                userManageAccessRights.focus({ user: usermanage });
            });


            validate();
        }

        var _inst = {
            refresh: function () {
                refreshGrid();
            },
            focus: function () {
                refreshGrid();
                activeSection(ID_USER_VIEW);
                currentView = this;
                currentUserSelected = null;
            },
            blur: function () {

            }
        }

        init();
        return _inst;
    }
    function _userRegister() {
        var
            URL_REGISTER_POST = '/PortalAdmin/AddUser'
            ;

        function init() {
            wireupEvent();
        }
        function wireupEvent() {
            var btnregister = $(MEL.toSelector(ID_BTN_REGISTER_USER));
            var btncancel = $(MEL.toSelector(ID_BTN_REGISTER_CANCEL));

            btnregister.off();
            btncancel.off();

            btnregister.click(function () {
                if (!isActiveSection(_inst)) return false;
                doregister();
            });
            btncancel.click(function () {
                _inst.blur();
            });
        }
        function clearForm() {
            var form = $(MEL.toSelector(ID_FORM_REGISTER));
            form.formReset();
        }

        function doregister() {
            var form = $(MEL.toSelector(ID_FORM_REGISTER));
            if (!form.valid()) {
                $('.input-validation-error').first().focus();
                return;
            }

            var dto = MEL.toDTO(form);
            dto['RegisterViewModel.ReceivedEmail'] = $('#chk-rg-usr').is(':checked');

            //console.log(dto);

            MEL.load({
                url: URL_REGISTER_POST
                , dto: dto
                , callback: function (info) {
                    //window.alert('New user has been registered successfully.');
                    if (info.available == false) {

                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: info.exception
                        });

                    }
                    else {

                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: "New user has been registered successfully.",
                            callback: function () { _inst.blur(); }
                        });

                    }
                }
            });
        }

        var _inst = {
            refresh: function () {
            },
            focus: function (opt) {
                activeSection(ID_USER_VIEW_REGISTER);
                currentView = this;
            },
            blur: function () {
                clearForm();
                activeSection();
            }
        };

        init();
        return _inst;
    }
    function _userEdit() {

        var
            URL_MANAGE_USER = '/PortalAdmin/EditUser'
            , ID_TXT_NAME = 'txt-edit-name'
            , ID_TXT_EMAIL = 'txt-edit-email'
            , ID_TXT_USERNAME = 'txt-edit-username'
            , ID_TXT_PASSWORD = 'txt-edit-password'

            , currentUser = null
            ;


        function init() {
            wireupEvent();
        }
        function wireupEvent() {
            var btnsubmit = $(MEL.toSelector(ID_BTN_EDIT_USER));
            var btncancel = $(MEL.toSelector(ID_BTN_EDIT_CANCEL));

            btnsubmit.off();
            btncancel.off();


            btnsubmit.click(function () {
                if (!isActiveSection(_inst)) return;
                doEdit();
            });
            btncancel.click(function () {
                _inst.blur();
            });

        }
        function clearForm() {
            var form = $(MEL.toSelector(ID_FORM_EDIT_USER));
            form.formReset();
        }
        function fillForm(user) {

            if (!user) {
                MEL.log('Empty user data to edit.');
                return;
            }

            currentUser = user;
            $(MEL.toSelector(ID_TXT_NAME)).val(user.name);
            $(MEL.toSelector(ID_TXT_EMAIL)).val(user.email);
            $(MEL.toSelector(ID_TXT_USERNAME)).val(user.userName);

            //console.log(currentUser);

            var chkstatus2 = $('#chk-ed-act');
            chkstatus2.prop('checked', currentUser.active);

            var receive = currentUser.receivedEmail;
            var chk = false;

            if (receive == "Yes") {
                chk = true;
            }
            var chkstatus3 = $('#chk-ed-usr');
            chkstatus3.prop('checked', chk);
        }

        function doEdit() {
            var form = $(MEL.toSelector(ID_FORM_EDIT_USER));
            
            if (!form.valid()) {
                $('.input-validation-error').first().focus();
                return;
            }

            var dto = MEL.toDTO(form);
            dto['ManageUserViewModel.UserID'] = currentUser.userID;
            dto['ManageUserViewModel.Active'] = $('#chk-ed-act').is(':checked');

            if (!dto['ManageUserViewModel.Password']) {
                dto['ManageUserViewModel.Password'] = currentUser.password;
                dto['ManageUserViewModel.ConfirmPassword'] = currentUser.password;
            }

            dto['ManageUserViewModel.ReceivedEmail'] = $('#chk-ed-usr').is(':checked');

            MEL.load({
                url: URL_MANAGE_USER
                , dto: dto
                , callback: function (result) {
                    if (result.stat == false) {

                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: "Admin password length must be at least 12 characters.",
                            callback: function () { _inst.blur(); }
                        });


                    }
                    else if (result.check == true) {
                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: "Password same with previous password. Change new password for secure.",
                            callback: function () { _inst.blur(); }

                        });


                    }
                    else {

                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: "The user information has been updated successfully.",
                            callback: function () { _inst.blur(); }

                        });

                    }


                }
            });
        }

        var _inst = {
            refresh: function () {
            },
            focus: function (opt) {
                activeSection(ID_USER_VIEW_EDIT);
                currentView = this;
                fillForm(opt.user);
            },
            blur: function () {
                clearForm();
                activeSection();
            }
        }

        init();
        return _inst;
    }
    function _userManageAccessRights() {
        var
            URL_USER_ACCESSRIGHT = '/PortalAdmin/GetUserRoles'
            , URL_ADD_ROLE = '/PortalAdmin/AddRole'
            , URL_REMOVE_ROLE = '/PortalAdmin/RemoveRole'
            , URL_EXCLUSIVE_VIEW = '/PortalAdmin/GetExclusiveAccess'
            , URL_EXCLUSIVE_ADD = '/PortalAdmin/AddAccessExclusive'
            , URL_EXCLUSIVE_EDIT = '/PortalAdmin/EditAccessExclusive'
            , URL_EXCLUSIVE_REMOVE = '/PortalAdmin/RemoveAccessExclusive'
            , URL_USER_RESTAURANT = '/PortalAdmin/GetRestaurant'
            , URL_ADD_REST = '/PortalAdmin/AddRestaurant'
            , URL_REMOVE_REST = '/PortalAdmin/RemoveRestaurant'

            , COL_ACCESS_INDEX_SN = 0
            , COL_ACCESS_INDEX_ROLENAME = 1
            , COL_ACCESS_INDEX_STARTDATE = 2
            , COL_ACCESS_INDEX_ENDDATE = 3
            , COL_ACCESS_INDEX_ISACTIVE = 4
            , COL_ACCESS_INDEX_CONTROLS = 5

            , COL_EXCLUSIVE_INDEX_SN = 0
            , COL_EXCLUSIVE_INDEX_ACTION = 1
            , COL_EXCLUSIVE_INDEX_STATUS = 2
            , COL_EXCLUSIVE_INDEX_CONTROL = 3

            , currentUser = null
            , contenttable = null
            , exclusivetable = null
            , availablePermissions = null
            , restauranttable = null


            , permissionDialog = null
            , _opt = null
            ;

        function init() {
            initAccessRightsGrid();
            MEL.loadPermissions(function (data) {
                availablePermissions = data;
                initExcusliveRightsGrid();
                wireupEvent();
            });
            initRestaurantGrid();
            permissionDialog = _addPermission();
        }
        function initAccessRightsGrid() {

            contenttable = $(MEL.toSelector(ID_TABLE_ACCESSRIGHT)).dataTable({
                'bServerSide': true
                , 'sAjaxSource': URL_USER_ACCESSRIGHT
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
                    sSearch: 'Search'
                    , sInfo: '_START_ to _END_'
                    , sEmptyTable: 'No records found.'
                    , "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
                }
                , 'fnServerData': function (sSource, aoData, fnCallback, oSettings) {

                    aoData.push({
                        name: 'UserPvid',
                        value: (!currentUser ? 0 : currentUser.userID)
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
                    { "bSortable": false, "aTargets": [COL_ACCESS_INDEX_SN], 'sWidth': '25px', 'mData': null }
                    ,
                    {
                        'aTargets': [COL_ACCESS_INDEX_CONTROLS],
                        'mRender': function (data, type, full) {
                            return String.format('<a class="{1} mel-disable" data-rolpro="{0}" data-permission-id="{2}" data-permission-act="mel-enable|mel-disable" title="Remove User"><i class="fa fa-ban"></i></a>',
                                data.pvid,
                                CSS_CLASS_GRID_ROLE_REMOVE,
                                pms.keys.user.removeRole);
                        }
                    }
                    ,
                    {
                        'aTargets': [COL_ACCESS_INDEX_STARTDATE],
                        'mRender': function (data, type, full) {
                            var grantedDate = new Date(data);
                            //console.log(grantedDate);
                            return grantedDate.format('dd/MM/yyyy');
                        }
                    },
                    {
                        'aTargets': [COL_ACCESS_INDEX_ENDDATE],
                        'mRender': function (data, type, full) {
                            //var grantedDate = JSON.dateStringToDate(data);
                            //return grantedDate.format('dd/MM/yyyy');

                            var enddate = new Date(data)
                            if ((enddate.getFullYear() - (new Date).getFullYear()) >= 100) return '-';
                            return enddate.format('dd/MM/yyyy');
                        }
                    },
                    {
                        'aTargets': [COL_ACCESS_INDEX_ISACTIVE],
                        'mRender': function (data, type, full) {
                            return data ? 'Active' : 'Inactive';
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
                    for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                        $(String.format("td:eq({0})", COL_ACCESS_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + oSettings._iDisplayStart + 1);
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
                    , { 'mData': "name" } // Title
                    , { 'sWidth': '120px', 'mData': "startActiveDate" }
                    , { 'sWidth': '120px', 'mData': "endActiveDate" }
                    , { 'sWidth': '100px', 'mData': "isActive" }
                    , { 'sWidth': '50px', 'mData': null }
                ]
            });
        }
        function initExcusliveRightsGrid() {
            //console.log(COL_EXCLUSIVE_INDEX_CONTROL);
            exclusivetable = $(MEL.toSelector(ID_TABLE_EXCLUSIVE_ACCESS)).dataTable({
                'bServerSide': true
                , 'sAjaxSource': URL_EXCLUSIVE_VIEW
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
                    sSearch: 'Search'
                    , sInfo: '_START_ to _END_'
                    , sEmptyTable: 'No records found.'
                    , "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
                }
                , 'fnServerData': function (sSource, aoData, fnCallback, oSettings) {

                    aoData.push({
                        name: 'UserPvid',
                        value: (!currentUser ? 0 : currentUser.userID)
                    })

                    oSettings.jqXHR = $.ajax({
                        type: 'post',
                        datatype: 'json',
                        url: sSource,
                        data: aoData,
                        success: function (result) {
                            if (result && typeof result.success != 'undefined' && !result.success) {
                                var msg = String.format('{0}.General error', URL_EXCLUSIVE_VIEW);
                                if (result.exception) msg = String.format('{0} - {1}', URL_EXCLUSIVE_VIEW, result.exception.message);
                                MEL.log(msg);
                                return;
                            }

                            fnCallback.call(this, result);
                        }
                    });
                }
                , "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [COL_EXCLUSIVE_INDEX_SN], 'sWidth': '25px', 'mData': null }
                    ,
                    {                     
                        'aTargets': [COL_EXCLUSIVE_INDEX_CONTROL],
                        'mRender': function (data, type, full) {

                            return String.format(
                                '<a class="{1} mel-disable" data-excacc="{0}" data-permission-id="{3}" data-permission-act="mel-enable|mel-disable" title="Edit User"><i class="fa fa-edit"></i></a>&nbsp;' +
                                '<a class="{2} mel-disable" data-excacc="{0}" data-permission-id="{4}" data-permission-act="mel-enable|mel-disable" title="Remove User"><i class="fa fa-ban"></i></a>',
                                data.pvid,
                                CSS_BTN_GRID_EXCLUSIVE_EDIT,
                                CSS_BTN_GRID_EXCLUSIVE_REMOVE,
                                pms.keys.user.editExclusiveRight,
                                pms.keys.user.removeExclusiveRight);
                        }
                    }
                    ,
                    {
                        'aTargets': [COL_EXCLUSIVE_INDEX_ACTION],
                        'mRender': function (data, type, full) {
                            //console.log(availablePermissions);
                            var perms = availablePermissions[full.permissionKey];
                            if (!perms || !perms.obj) return full.permissionKey;
                            var title = perms.obj.title ? perms.obj.title : '';
                            var desc = perms.obj.desc ? perms.obj.desc : '';
                            return (title + ' - ' + desc);

                        }
                    },
                    {
                        'aTargets': [COL_EXCLUSIVE_INDEX_STATUS],
                        'mRender': function (data, type, full) {
                            return data.accessible ? 'Granted' : 'Denied';
                        }
                    }
                ]
                , 'fnDrawCallback': function (oSettings) {
                    for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                        $(String.format("td:eq({0})", COL_ACCESS_INDEX_SN), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + oSettings._iDisplayStart + 1);
                    }
                    wireupEventExclusiveGrid();
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
                    , { 'mData': null }
                    , { 'sWidth': '120px', 'mData': null }
                    , { 'sWidth': '50px', 'mData': null }
                ]
            });
        }
        function initRestaurantGrid() {

            restauranttable = $(MEL.toSelector(ID_TABLE_RESTAURANT)).dataTable({
                'bServerSide': true
                , 'sAjaxSource': URL_USER_RESTAURANT
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
                    sSearch: 'Search'
                    , sInfo: '_START_ to _END_'
                    , sEmptyTable: 'No records found.'
                    , "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
                }
                , 'fnServerData': function (sSource, aoData, fnCallback, oSettings) {
                    
                    aoData.push({
                        name: 'UserPvid',
                        value: (!currentUser ? 0 : currentUser.userID)
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
                    { "bSortable": false, "aTargets": [0], 'sWidth': '25px', 'mData': null }
                    ,
                    {
                        'aTargets': [4],
                        'mRender': function (data, type, full) {
                            return String.format('<a class="{1} mel-disable" data-rolpro="{0}" data-permission-id="{2}" data-permission-act="mel-enable|mel-disable" title="Remove Restaurant"><i class="fa fa-ban"></i></a>',
                                data.pvid,
                                CSS_CLASS_GRID_REST_REMOVE,
                                pms.keys.user.removeRestaurant);
                        }
                    }
                    ,
                    {
                        'aTargets': [2],
                        'mRender': function (data, type, full) {
                            var grantedDate = new Date(data);
                            //console.log(grantedDate);
                            return grantedDate.format('dd/MM/yyyy');
                        }
                    },
                    
                ]
                , 'fnDrawCallback': function (oSettings) {
                 
                    for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                        $(String.format("td:eq({0})", 0), oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + oSettings._iDisplayStart + 1);
                    }

                    wireupEventRestaurantGrid();
                }
               
                , 'aoColumns': [
                    { 'sWidth': '25px', 'mData': null }
                    , { 'mData': "restaurant_name" } // Title
                    , { 'sWidth': '120px', 'mData': "startActiveDate" }
                    , { 'sWidth': '100px', 'mData': "status" }
                    , { 'sWidth': '50px', 'mData': null }
                ]
            });
        }

        function initView() {
            var lbname = $(MEL.toSelector(ID_LB_USER_NAME));
            lbname.text(currentUser.userName)
        }
        function wireupEvent() {
            var btnback = $(MEL.toSelector(ID_BTN_BACK_ROLE));
            var btnaddrole = $(MEL.toSelector(ID_BTN_USERROLE_ADD));
            var btnaddExclusive = $(MEL.toSelector(ID_BTN_ADD_EXCLUSIVE));
            var btnFindPermission = $(MEL.toSelector(ID_BTN_FIND_PERMISSION_EXCLUSIVE));
            var btnFindPermissionEdit = $(MEL.toSelector(ID_BTN_FIND_EDIT_PERMISSION_EXCLUSIVE));
            var btnaddrest = $(MEL.toSelector(ID_BTN_REST_ADD));

            btnback.off();
            btnaddrole.off();
            btnaddExclusive.off();
            btnFindPermission.off();
            btnFindPermissionEdit.off();
            btnaddrest.off();

            btnback.click(function () {
                _inst.blur();
            });
            btnaddExclusive.click(function () {
                $(MEL.toSelector(ID_TXT_EXCLUSIVE_SELECTED))
                    .data('permission', null)
                    .prop('title', '')
                    .val('');
                //$(MEL.toSelector(ID_FORM_ADD_EXCLUSIVE)).formReset();

                $(MEL.toSelector(ID_MODAL_ADD_EXCLUSIVE)).modal('show');
                $(MEL.toSelector(ID_USER_ADD_EXCLUSIVE)).css('display', 'block');


            });

            $(MEL.toSelector(ID_FORM_ADD_EXCLUSIVE)).on('submit', function (e) {

                e.preventDefault();
                var form = $(MEL.toSelector(ID_FORM_ADD_EXCLUSIVE));
                if (!form.valid()) {
                    $('.input-validation-error').first().focus();
                    return;
                }

                var permissionkey = $(MEL.toSelector(ID_TXT_EXCLUSIVE_SELECTED)).data('permission') ? $(MEL.toSelector(ID_TXT_EXCLUSIVE_SELECTED)).data('permission').obj.key : '';
                var dto = MEL.toDTO(form);
                dto['AddExclusiveAccess.UserPvid'] = currentUser.userID;
                dto['AddExclusiveAccess.PermissionKey'] = permissionkey;

                var list = exclusivetable.fnGetData();

                var exclusive = MEL.getByValue(list, 'permissionKey', dto['AddExclusiveAccess.PermissionKey']);

                if (exclusive) {

                    $(MEL.toSelector(ID_MODAL_ADD_EXCLUSIVE)).modal('hide');

                    bootbox.alert({
                        centerVertical: true,
                        size: "small",
                        message: "Exclusive permission already exist for the current User.",
                        callback: function () { refreshGrid(); }
                    });

                    return;
                }
                else {
                    MEL.load({
                        url: URL_EXCLUSIVE_ADD
                        , dto: dto
                        , callback: function (result) {

                            $(MEL.toSelector(ID_MODAL_ADD_EXCLUSIVE)).modal('hide');

                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: "Exclusive access has been added successfully.",
                                callback: function () { refreshGrid(); }
                            });
                        }
                    });
                }


            });
            btnaddrole.click(function () {
                if (!currentUser) return;

                $(MEL.toSelector(ID_FORM_ADD_ROLE)).formReset();

                $(MEL.toSelector(ID_MODAL_ADD_ROLE)).modal('show');
                $(MEL.toSelector(ID_USER_VIEW_ROLE)).css('display', 'block');


            });

            btnaddrest.click(function () {
                if (!currentUser) return;

                $(MEL.toSelector(ID_FORM_ADD_REST)).formReset();

                $(MEL.toSelector(ID_MODAL_ADD_REST)).modal('show');
                $(MEL.toSelector(ID_USER_VIEW_REST)).css('display', 'block');


            });
            $(MEL.toSelector(ID_FORM_ADD_REST)).on('submit', function (e) {
                e.preventDefault();
                var form = $(MEL.toSelector(ID_FORM_ADD_REST));
                var dto = MEL.toDTO(form);
                dto['AddUserRestaurant.userId'] = currentUser.userID;

                var list = restauranttable.fnGetData();
                var exist = MEL.getByValue(list, 'restaurant_id', dto['AddUserRestaurant.restaurantId']);

                if (exist) {

                    $(MEL.toSelector(ID_MODAL_ADD_REST)).modal('hide');

                    bootbox.alert({
                        centerVertical: true,
                        size: "small",
                        message: "Restaurant already exist for the current User.",
                        callback: function () { refreshGrid(); }
                    });

                    return;
                }

                MEL.load({
                    url: URL_ADD_REST
                    , dto: dto
                    , callback: function (result) {

                        $(MEL.toSelector(ID_MODAL_ADD_REST)).modal('hide');

                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: "Restaurant has been added successfully.",
                            callback: function () { refreshGrid(); }
                        });
                    }
                });

            });
            $(MEL.toSelector(ID_FORM_ADD_ROLE)).on('submit', function (e) {

                e.preventDefault();

                var form = $(MEL.toSelector(ID_FORM_ADD_ROLE));
                if (!form.valid()) {
                    $('.input-validation-error').first().focus();
                    return;
                }

                var dto = MEL.toDTO(form);
                dto['AddUserRole.UserPvid'] = currentUser.userID;

                var list = contenttable.fnGetData();
                var role = MEL.getByValue(list, 'pvid', dto['AddUserRole.RolePvid']);
                if (role) {

                    $(MEL.toSelector(ID_MODAL_ADD_ROLE)).modal('hide');

                    bootbox.alert({
                        centerVertical: true,
                        size: "small",
                        message: "Role already exist for the current User.",
                        callback: function () { refreshGrid(); }
                    });

                    return;
                }
                var volunteertext = "";
                var role = "Volunteer"
                if ($('#AddUserRole_RolePvid option:selected').text().toLowerCase().indexOf("met ") >= 0 || $('#AddUserRole_RolePvid option:selected').text().toLowerCase().indexOf("cbp ") >= 0) {
                    if ($('#AddUserRole_RolePvid option:selected').text().toLowerCase().indexOf("met ") >= 0) {volunteertext = "MET";}
                    if ($('#AddUserRole_RolePvid option:selected').text().toLowerCase().indexOf("cbp ") >= 0) {
                        volunteertext = "CBP";

                        var selectedOption = $("#AddUserRole_RolePvid option:selected").text();
                        if (selectedOption == "CBP Admin") {
                            var role = "Admin";
                        }
                        if (selectedOption == "CBP Leader") {
                            var role = "Leader";
                        }
                        if (selectedOption == "CBP Client") {
                            var role = "Client";
                        }
                    }
                    bootbox.confirm({
                        title: "Volunteer Verification",
                        message: "Is this a verified " + volunteertext +" "+ role +" ?",
                        buttons: {
                            cancel: {
                                label: 'No'
                            },
                            confirm: {
                                label: 'Yes'
                            }
                        },
                        callback: function (result) {
                            if (result == true) {
                                MEL.load({
                                    url: URL_ADD_ROLE
                                    , dto: dto
                                    , callback: function (result) {

                                        $(MEL.toSelector(ID_MODAL_ADD_ROLE)).modal('hide');

                                        bootbox.alert({
                                            centerVertical: true,
                                            size: "small",
                                            message: "User role has been added successfully.",
                                            callback: function () { refreshGrid(); }
                                        });
                                    }
                                });
                            }
                            else {
                                return;
                            }
                        }
                    });
                } else {
                    MEL.load({
                        url: URL_ADD_ROLE
                        , dto: dto
                        , callback: function (result) {

                            $(MEL.toSelector(ID_MODAL_ADD_ROLE)).modal('hide');

                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: "User role has been added successfully.",
                                callback: function () { refreshGrid(); }
                            });
                        }
                    });
                }

            });

            btnFindPermission.click(function () {

                var selectedPermission = $(MEL.toSelector(ID_TXT_EXCLUSIVE_SELECTED)).data('permission');
                permissionDialog.focus({
                    selectedPermission: selectedPermission,
                    availablePermissions: availablePermissions,
                    callback: function (permissionContent) {
                        if (!permissionContent) return;
                        $(MEL.toSelector(ID_TXT_EXCLUSIVE_SELECTED))
                            .data('permission', permissionContent)
                            .val(permissionContent.obj.title)
                            .prop('title', permissionContent.obj.title);
                    }
                });
            });
            btnFindPermissionEdit.click(function () {
                var selectedPermission = $(MEL.toSelector(ID_TXT_EXCLUSIVE_SELECTED)).data('permission');
                permissionDialog.focus({
                    selectedPermission: selectedPermission,
                    availablePermissions: availablePermissions,
                    callback: function (permissionContent) {
                        if (!permissionContent) return;
                        $(MEL.toSelector(ID_TXT_EDIT_EXCLUSIVE_SELECTED))
                            .data('permission', permissionContent)
                            .val(permissionContent.obj.title)
                            .prop('title', permissionContent.obj.title);
                    }
                });
            });
        }
        function wireupEventGrid() {
            btnremoveRole = $(MEL.toClass(CSS_CLASS_GRID_ROLE_REMOVE));

            btnremoveRole.off();

            btnremoveRole.click(function () {

                var id = $(this).data('rolpro');
                var list = contenttable.fnGetData();
                var role = MEL.getByValue(list, 'pvid', id);

                bootbox.confirm({
                    centerVertical: true,
                    size: "small",
                    message: "Do you wish to remove role (" + role.name + ") from the user (" + currentUser.userName + ") ?",
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
                                url: URL_REMOVE_ROLE,
                                dto: {
                                    UserPvid: currentUser.userID,
                                    RolePvid: role.pvid
                                }
                                ,
                                callback: function () {

                                    bootbox.alert({
                                        centerVertical: true,
                                        size: "small",
                                        message: "Role has been removed successfully.",
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
        function wireupEventExclusiveGrid() {
            var btnedit = $(MEL.toClass(CSS_BTN_GRID_EXCLUSIVE_EDIT));
            var btnremove = $(MEL.toClass(CSS_BTN_GRID_EXCLUSIVE_REMOVE));

            btnedit.off();
            btnremove.off();

            btnedit.click(function () {
                var id = $(this).data('excacc');
                var list = exclusivetable.fnGetData();
                var exclusive = MEL.getByValue(list, 'pvid', id);
                var contentPermission = availablePermissions[exclusive.permissionKey];
                var tempform = $(MEL.toSelector(ID_FORM_EDIT_EXCLUSIVE));

                $(MEL.toSelector(ID_TXT_EDIT_EXCLUSIVE_SELECTED))
                    .data('permission', contentPermission)
                    .prop('title', contentPermission.obj.title)
                    .val(contentPermission.obj.title);

                $(MEL.toSelector(ID_CMB_EDIT_EXCLUSIVE_ACCESSIBLE)).val(exclusive.Accessible ? 'True' : 'False');

                $(MEL.toSelector(ID_FORM_EDIT_EXCLUSIVE)).on('submit', function (e) {

                    e.preventDefault();

                    var form = $(MEL.toSelector(ID_FORM_EDIT_EXCLUSIVE));
                    if (!form.valid()) {
                        $('.input-validation-error').first().focus();
                        return;
                    }

                    var permissionkey = $(MEL.toSelector(ID_TXT_EDIT_EXCLUSIVE_SELECTED)).data('permission') ? $(MEL.toSelector(ID_TXT_EDIT_EXCLUSIVE_SELECTED)).data('permission').obj.key : '';
                    var dto = MEL.toDTO(form);

                    dto['EditExclusiveAccess.UserPvid'] = currentUser.userID;
                    dto['EditExclusiveAccess.ExclusivePvid'] = id;
                    dto['EditExclusiveAccess.PermissionKey'] = permissionkey;

                    MEL.load({
                        url: URL_EXCLUSIVE_EDIT
                        , dto: dto
                        , callback: function (result) {

                            $(MEL.toSelector(ID_MODAL_EDIT_EXCLUSIVE)).modal('hide');

                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: "Exclusive access has been updated successfully.",
                                callback: function () {
                                    refreshGrid();
                                }
                            });


                        }
                    });

                });

                $(MEL.toSelector(ID_MODAL_EDIT_EXCLUSIVE)).modal('show');
                $(MEL.toSelector(ID_USER_EDIT_EXCLUSIVE)).css('display', 'block');


            });
            btnremove.click(function () {
                var id = $(this).data('excacc');
                var list = exclusivetable.fnGetData();
                var exclusive = MEL.getByValue(list, 'pvid', id);

                //console.log(exclusive);

                dialogConfirmRemove = true;

                bootbox.confirm({
                    centerVertical: true,
                    size: "small",
                    message: "Do you wish to remove the access right ?",
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
                            removeExclusive(exclusive);
                            dialogConfirmRemove = false;
                            return true;
                        }
                        else {
                            dialogConfirmRemove = false;
                            return true;
                        }
                    }
                });

            });

            MEL.validate();
        }
        function wireupEventRestaurantGrid() {
            btnremoveRole = $(MEL.toClass(CSS_CLASS_GRID_REST_REMOVE));

            btnremoveRole.off();

            btnremoveRole.click(function () {

                var id = $(this).data('rolpro');
                var list = restauranttable.fnGetData();
                var exist = MEL.getByValue(list, 'pvid', id);

                bootbox.confirm({
                    centerVertical: true,
                    size: "small",
                    message: "Do you wish to remove restaurant (" + exist.restaurant_name + ") from the user (" + currentUser.userName + ") ?",
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
                                url: URL_REMOVE_REST,
                                dto: {
                                    UserPvid: currentUser.userID,
                                    RestPvid: exist.pvid
                                }
                                ,
                                callback: function () {

                                    bootbox.alert({
                                        centerVertical: true,
                                        size: "small",
                                        message: "Restaurant has been removed successfully.",
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
        function refreshGrid() {
            contenttable.api().ajax.reload();
            exclusivetable.api().ajax.reload();
            restauranttable.api().ajax.reload();
        }

        function removeExclusive(exclusive) {
            MEL.load({
                url: URL_EXCLUSIVE_REMOVE
                , dto: exclusive
                , callback: function (data) {

                    //console.log(data);
                    bootbox.alert({
                        centerVertical: true,
                        size: "small",
                        message: "The access right has been removed successfully.",
                        callback: function () {
                            refreshGrid();
                        }
                    });

                }
            });
        }

        var _inst = {
            refresh: function () {
                refreshGrid();
            },
            focus: function (opt) {
                _opt = opt;
                currentUser = _opt.user;
                refreshGrid();
                activeSection(ID_USER_VIEW_ACCESSRIGHTS);
                currentView = this;
                initView();
            },
            blur: function () {
                activeSection();
            }
        }

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
            , ID_BTN_ADD_PERMISSION_CLOSE = 'btn-add-permission-close'
            , ID_DIALOG_FORM = 'module-view-manage-permission'
            , ID_DIALOG_MODAL = 'modal-view-manage-permission'
            , ID_CMB_MODULE_SELECTION = 'cmb-module-selection'

            , URL_MODULE_LIST = '/AccessLevel/GetAllActiveModule'
            , URL_MANAGE_PERMISSION = '/AccessLevel/ManagePermission'

            , CSS_CLASS_CHECKBOX = 'chk-permission-tick'

            , ajax_request_module_org = []
            , tempModuleOrgList = []
            , modalForm = null

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
        function initGrid() {

            permissiontable = $(MEL.toSelector(ID_TABLE_PERMISSION_ADDING)).dataTable({
                //ajax:_opt.availablePermissions,
                'iDisplayLength': global_records_per_page
                , 'sPaginationType': 'full_numbers'
                , 'bPaginate': true
                , 'pageLength': 15
                , 'lengthMenu': [[10, 15, 20], [10, 15, 20]]
                , 'bSort': true
                , 'deferLoading': 0
                , 'bJQueryUI': false
                , 'autoWidth': false
                , "bProcessing": true          
                , 'oLanguage': {
                    sSearch: 'Search'
                    , sInfo: '_START_ to _END_'
                    , "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
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
                if (checked) {
                    if (typeof _opt.callback == 'function') _opt.callback.call(_inst, ctn);
                    modalForm.modal('hide');
                }
            });

        }

        function hasOwnPermission(key) {
            return _opt.selectedPermission && _opt.selectedPermission.key == key;
        }

        var _inst = {
            refresh: function () {
                refreshGrid();
            },
            focus: function (opt) {
                _opt = opt;

                modalForm = $(MEL.toSelector(ID_DIALOG_MODAL)).modal('show');
                $(MEL.toSelector(ID_DIALOG_FORM)).css('display', 'block');
                refreshGrid();
            },
            blur: function () {
                modalForm.modal('hide');
            }
        };

        init();
        return _inst;
    }

    var inst = {
        refresh: function () {
            userView.refresh();
        }
    };

    init();
    return inst;
}