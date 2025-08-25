/// <reference path="mel.core.js" />
/// <reference path="../bootbox/dist/bootbox.js" />

$(window).on("load",function () {
    MEL.startup(function () {
        pms.profiles();
    });
});



pms.profiles = function () {
    var
        URL = ''
        , ID_TXT_USER_NAME = 'txt-edit-name'
        , ID_TXT_USER_EMAIL = 'txt-edit-email'
        , ID_TXT_USER_USERNAME = 'txt-edit-username'
        , ID_TXT_OLD_PWD = 'txt-edit-password-old'
        , ID_TXT_NEW_PWD = 'txt-edit-password-new'
        , ID_TXT_CONFIRM_PWD = 'txt-edit-password-confirm'

        , ID_BTN_CHANGEPASSWORD = 'btn-change-password'
        , ID_FORM_USER = 'form-edit-user'

        , URL_CHANGE_PWD = '/Profile/ChangePassword'

        , currentUser = null
    ;

    function init() {
        wireupEvent();
        fillForm();
    }
    function fillForm() {
        var user = MEL.core.config.User;
        var profile = MEL.core.config.Profiles;

        var txtname = $(MEL.toSelector(ID_TXT_USER_NAME));
        var txtemail = $(MEL.toSelector(ID_TXT_USER_EMAIL));
        var txtusername = $(MEL.toSelector(ID_TXT_USER_USERNAME));

        txtname.val(user.Name);
        txtemail.val(user.Email);
        txtusername.val(user.UserName);

        currentUser = user;
    }
    function clearPassword() {
        var txtoldpwd = $(MEL.toSelector(ID_TXT_OLD_PWD));
        var txtnewpwd = $(MEL.toSelector(ID_TXT_NEW_PWD));
        var txtconfirmpwd = $(MEL.toSelector(ID_TXT_CONFIRM_PWD));

        txtoldpwd.val('');
        txtnewpwd.val('');
        txtconfirmpwd.val('');
    }
    function wireupEvent() {
        var btnchangepwd = $(MEL.toSelector(ID_BTN_CHANGEPASSWORD));

        btnchangepwd.off();

        btnchangepwd.click(function () {
            var form = $(MEL.toSelector(ID_FORM_USER));
            if (!form.valid()) {
                $('.input-validation-error').first().focus();
                return;
            }

            var dto = MEL.toDTO(form);
            dto['UserID'] = currentUser.UserID;

            MEL.load({
                url: URL_CHANGE_PWD,
                dto:dto,
                callback: function (data) {
                    if (data.stat == false) {
                        

                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: "Admin password length must be at least 8 characters.",
                            callback: function () { /* your callback code */ }
                        });
                    }
                    else if (data.check == true) {

                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: "Password same with previous password. Change new password for secure.",
                            callback: function () { /* your callback code */ }
                        });
                    }
                    else {

                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: "Password has been Updated successfully.",
                            callback: function () {
                                clearPassword();
                                window.location.reload();
                            }
                        });

                    }
                }
            });
        });
        

        MEL.validate();
    }

    var inst = {
        refresh: function () {
        }
    };

    init();
    return inst;
}