function PasswordValidation() {

    var comments = document.getElementsByName('RegisterViewModel.Password');
    var numComments = comments.length;
    for (var i = 0; i < numComments; i++) {
        var text = "To make your password more secure<br>";
        text += "<span class='glyphicon glyphicon-remove' style='color:red'></span> Use at least one uppercase <br>";
        text += "<span class='glyphicon glyphicon-remove' style='color:red'></span> Use at least one lowercase <br>";
        text += "<span class='glyphicon glyphicon-remove' style='color:red'></span> Use at least one number <br>";
        text += "<span class='glyphicon glyphicon-remove' style='color:red'></span> Use special characters (e.g. !, #, $, % and etc)<br>";
        text += "<span class='glyphicon glyphicon-remove' style='color:red'></span> Password length at least 8 characters <br>";

        comments[i].setAttribute('data-content', text);
        comments[i].setAttribute('data-toggle', 'popover');
        comments[i].setAttribute('data-placement', 'right');
        comments[i].addEventListener('focus', myFunction, false);
        comments[i].addEventListener('keyup', textChange, false);

    }

    function myFunction() {
        $('[data-toggle="popover"]').popover({ html: true, placement: "bottom" });

    }

    function textChange() {
        var text = "";
        var a = 0, b = 0, c = 0, d = 0, e = 0;
        var textVal = document.getElementsByName("RegisterViewModel.Password")[0].value;
        text = "To make your password more secure<br>";
        if (textVal.match(/[a-z]/)) {
            text += "<span class='glyphicon glyphicon-ok' style='color:green'></span> Use at least one lowercase <br>";
            if (b == 0) {
                a = a + 1;
                b = 1;
            }
        }
        else {
            text += "<span class='glyphicon glyphicon-remove' style='color:red'></span> Use at least one lowercase <br>";
        }

        if (textVal.match(/[A-Z]/)) {
            text += "<span class='glyphicon glyphicon-ok' style='color:green'></span> Use at least one uppercase <br>";

            if (c == 0) {
                a = a + 1;
                c = 1;
            }
        }
        else {
            text += "<span class='glyphicon glyphicon-remove' style='color:red'></span> Use at least one uppercase <br>";
        }

        if (/\d/.test(textVal)) {
            text += "<span class='glyphicon glyphicon-ok' style='color:green'></span> Use at least one number <br>";
            if (d == 0) {
                a = a + 1;
                d = 1;
            }
        }
        else {
            text += "<span class='glyphicon glyphicon-remove' style='color:red'></span> Use at least one number <br>";
        }

        var format = /[ !@@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/;
        if (format.test(textVal)) {
            text += "<span class='glyphicon glyphicon-ok' style='color:green'></span> Use special characters (e.g. !, #, $, % and etc)<br>";
            if (e == 0) {
                a = a + 1;
                e = 1;
            }
        }
        else {
            text += "<span class='glyphicon glyphicon-remove' style='color:red'></span> Use special characters (e.g. !, #, $, % and etc)<br>";
        }

        if (textVal.length >= 8) {
            text += "<span class='glyphicon glyphicon-ok' style='color:green'></span> Password length at least 8 characters <br>";
        }
        else {
            text += "<span class='glyphicon glyphicon-remove' style='color:red'></span> Password length at least 8 characters <br>";
        }

        if (a >= 3 && textVal.length >= 8) {
            text = "<span class='glyphicon glyphicon-ok' style='color:green'></span> Password is secure<br>";
        }




        document.getElementsByName('RegisterViewModel.Password')[0].setAttribute('data-content', text);
        $('[data-toggle="popover"]').popover('show');
    }
}
function hideHint() {
    $('[data-toggle="popover"]').popover('hide');
}