$(document).ready(function () {

    addDeleteRowclick();
    //Initialize phone field
    /*  initPhoneField();*/
    fieldValidation();
    initPhoneFieldwithPara("#PhoneNumber", "#AddEditVolunteer_PersonalDetails_ContactNumber", 1);
    initPhoneFieldwithPara("#EmergencyPhoneNumber", "#AddEditVolunteer_EmergencyContactDetails_ContactNumber", 1);

    //$('#AddEditVolunteer_PersonalDetails_DateOfJoin').datepicker({
    //    startView: 2,
    //    format: 'dd/mm/yyyy'
    //});
    //$('#AddEditVolunteer_PersonalDetails_DOB').datepicker({
    //    startView: 2,
    //    format: 'dd/mm/yyyy'
    //});

    var s = new Date();
    var e = new Date();
    s.setFullYear(s.getFullYear() - 100);
    e.setFullYear(e.getFullYear() - 17);

    $('#AddEditVolunteer_PersonalDetails_DOB').datepicker({
        minDate: s,
        maxDate: e,
        useCurrent: false,
        changeYear: true,
        yearRange: "-100:+0",
        format: 'dd/mm/yyyy',
    });




    $('#gender input[type=radio]').each(function () {
        if (this.checked) {
            $('#AddEditVolunteer_PersonalDetails_Gender').val(this.value);
        }
    });

    $('#gender').change(function () {
        $('#gender input[type=radio]').each(function () {
            if (this.checked) {
                $('#AddEditVolunteer_PersonalDetails_Gender').val(this.value);
            }
        });
    });


    function fieldValidation() {

        $("#AddEditVolunteer_PersonalDetails_PostalCode").rules("add", {
            number: true,
            maxlength: 6,
            minlength: 6
        });
        $('#AddEditVolunteer_PersonalDetails_Email').change(() => {
            var mailformat = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;

            if ($('#AddEditVolunteer_PersonalDetails_Email').val().match(mailformat)) {
                $('.email-msg').css('display', 'none')
                return true;
            }
            if ($('#AddEditVolunteer_PersonalDetails_Email').val().toUpperCase() == "NA") {
                $('.email-msg').css('display', 'none')
                return true;
            }
            else {
                $('.email-msg').css('display', 'block')
                return false;
            }
        });
        //Validation at each page
        $('[data-toggle="tab"]').on('show.bs.tab', function (e) {

            var form = $(MEL.toSelector('registrations-form'));

            if (!form.valid()) {
                console.log(form);
                //if ($('.ic-selector').val() == 'none') {
                //    $('.validation-msg').css('display', 'inline')
                //} 

                if ($('#AddEditVolunteer_PersonalDetails_NRIC').val() == '') {
                    $('.validation-msg').css('display', 'inline');
                }

                $('.input-validation-error').first().focus();
                e.preventDefault();
                return;
            }



        })
    }

    $('.next-btn').click(() => {
        //if ($('.validation-msg').css('display', 'inline')) {
        //    console.log('5');
        $('html, body').animate({
            scrollTop: 0
        }, 500);
        //    $('.next-page').css('cursor', 'none')
        //    $('.next-btn').prop('disabled', true);
        //}

        //$('.validation-msg').css('display', 'none')
        //$('.next-page').css('cursor', 'pointer')
        //$('.next-btn').prop('disabled', false);
    })

    $('.prev-btn').click(() => {
        $('.validation-msg').css('display', 'none')
        $('.next-page').css('cursor', 'pointer')
        $('.next-btn').prop('disabled', false);
    })





    $('#nric').change(function () {
        $('.start-box').val('')
        $('.middle-box').val('')
        $('.end-box').val('')
        $('.passport-box').val('')
        $('#AddEditVolunteer_PersonalDetails_NRIC').val('');

        if ($('.ic-selector').val() == 'Nric') {
            $('.passport-input').css("display", "none")
            $('.ic-input').css("display", "block")
            $('.passport-msg').css('display', 'none')
            $('.validation-msg').css('display', 'none')
           // $('.nric-msg').css('display', 'block')
            $('.invalid-nric').css('display', 'none')
            $('.valid-nric').css('display', 'none')

        } else if ($('.ic-selector').val() == 'Passport') {
            $('.ic-input').css("display", "none")
            $('.passport-input').css("display", "block")
            $('.nric-msg').css('display', 'none')
            $('.passport-msg').css('display', 'block')
            $('.validation-msg').css('display', 'none')
            $('.invalid-nric').css('display', 'none')
            $('.valid-nric').css('display', 'none')
            //$('.passport-box').change(() => {

            //    if ($('.passport-box').val().length > 3) {
            //        $('.passport-msg').css('display', 'none')

            //    }
            //    else {
            //        $('#AddEditVolunteer_PersonalDetails_NRIC').val($('.passport-box').val());
            //        $('.validation-msg').css('display', 'none')
            //        $('.next-page').css('cursor', 'pointer')
            //        $('.next-btn').prop('disabled', false);
            //    }
            //})

        } else if ($('.ic-selector').val() == 'none') {
            $('.passport-input').css("display", "none")
            $('.ic-input').css("display", "none")
            $('.nric-msg').css('display', 'none')
            $('.passport-msg').css('display', 'none')
            $('.next-page').css('cursor', 'none')
            $('.next-btn').prop('disabled', true)
            $('.invalid-nric').css('display', 'none')
            $('.valid-nric').css('display', 'none')
        }


    })

    $('.passport-box').change(() => {
        if ($('.passport-box').val().length >= 6) {
            $('.passport-msg').css('display', 'none')
            $('#AddEditVolunteer_PersonalDetails_NRIC').val($('.passport-box').val());
            $('.validation-msg').css('display', 'none')
            $('.next-page').css('cursor', 'pointer')
            $('.next-btn').prop('disabled', false);
        }
        else if ($('.passport-box').val() == '') {
            $('.passport-msg').css('display', 'block')
            $('.next-page').css('cursor', 'none')
            $('.next-btn').prop('disabled', true);
        }
        else {
            $('.passport-msg').css('display', 'block')
            $('.next-page').css('cursor', 'none')
            $('.next-btn').prop('disabled', true);

        }


    })




    function validateNRIC(str) {
        if (str.length != 9)
            return false;

        str = str.toUpperCase();

        var i,
            icArray = [];
        for (i = 0; i < 9; i++) {
            icArray[i] = str.charAt(i);
        }

        icArray[1] = parseInt(icArray[1], 10) * 2;
        icArray[2] = parseInt(icArray[2], 10) * 7;
        icArray[3] = parseInt(icArray[3], 10) * 6;
        icArray[4] = parseInt(icArray[4], 10) * 5;
        icArray[5] = parseInt(icArray[5], 10) * 4;
        icArray[6] = parseInt(icArray[6], 10) * 3;
        icArray[7] = parseInt(icArray[7], 10) * 2;

        var weight = 0;
        for (i = 1; i < 8; i++) {
            weight += icArray[i];
        }

        var offset = (icArray[0] == "T" || icArray[0] == "G") ? 4 : 0;
        var temp = (offset + weight) % 11;

        var st = ["J", "Z", "I", "H", "G", "F", "E", "D", "C", "B", "A"];
        var fg = ["X", "W", "U", "T", "R", "Q", "P", "N", "M", "L", "K"];

        var theAlpha;
        if (icArray[0] == "S" || icArray[0] == "T") { theAlpha = st[temp]; }
        else if (icArray[0] == "F" || icArray[0] == "G") { theAlpha = fg[temp]; }

        return (icArray[8] === theAlpha);
    }

    $('.ic-input').change(function () {
        var nric = $('.start-box').val() + $('.middle-box').val() + $('.end-box').val()

        if (nric == '') {
            $('.nric-msg').css('display', 'block')
            $('.next-page').css('cursor', 'none')
            $('.next-btn').prop('disabled', true);
        } else {

            if (validateNRIC(nric)) {

                $('#AddEditVolunteer_PersonalDetails_NRIC').val(nric)
                console.log('true')
                $('.valid-nric').css('display', 'inline')
                $('.invalid-nric').css('display', 'none')
                $('.nric-msg').css('display', 'none')
                $('.validation-msg').css('display', 'none')
                $('.next-page').css('cursor', 'pointer')
                $('.next-btn').prop('disabled', false);
                //$('#btnSubmit').prop('disabled', false)

            } else {
                console.log('wrong');
                $('.invalid-nric').css('display', 'inline')
                $('.valid-nric').css('display', 'none')
                $('.nric-msg').css('display', 'inline')
                $('.validation-msg').css('display', 'none')
                $('.next-page').css('cursor', 'none')
                $('.next-btn').prop('disabled', true);
                //$('#btnSubmit').prop('disabled',true)

            }
        }
    })
    $('.next-page').click(() => {
        console.log('asdasdasd')

        if ($('.ic-selector').val() == 'none') {
            $('.validation-msg').css('display', 'inline')
            alert("Please Select Identiy Type")
            return false;
        }
        if ($('.ic-selector').val() == 'Nric') {
            if ($('.start-box').val() == '') {
                alert("Please fill in the IC number.");
                $('.validation-msg').css('display', 'inline')
                return false;
            }
            if ($('.middle-box').val() == '') {
                alert("Please fill in the IC number.");
                return false;
            }
            if ($('.end-box').val() == '') {
                alert("Please fill in the IC number.");
                return false;
            }
        }
        if ($('.ic-selector').val() == 'Passport') {
            if ($('.passport-box').val() == '') {
                alert("Please fill in the Passport number")
                return false;
            }
        }
        $('html, body').animate({
            scrollTop: 0
        }, 500);

    })

    //$('.passport-box').change(function () {
    //    //let data = []
    //    //data.push($('.passport-box').val())

    //    $('#AddEditVolunteer_PersonalDetails_NRIC').val($('.passport-box').val());
    //    $('.validation-msg').css('display', 'none')
    //    $('.next-page').css('cursor', 'pointer')
    //    $('.next-btn').prop('disabled', false);

    //})


    // Combine data in VOLUNTEERING AREA
    $('#areaInterest').change(function () {
        var data = [];
        $('#areaInterest input[type=checkbox]').each(function () {
            if (this.checked) {
                data.push($(this).val());
            }
        });
        $('#AddEditVolunteer_VoluntaryDetails_VolunteeringArea').val(data.join());
    });

    // Combine data in Availability and pass to dto
    $('.availability').change(function () {
        var monday = [];
        var tuesday = [];
        var wednesday = [];
        var thursday = [];
        var friday = [];
        var saturday = [];
        var sunday = [];
        $('.availability tbody input[type=checkbox]').each(function () {
            if (this.checked) {
                if (this.className === 'a') {
                    monday.push($(this).val());
                }
                if (this.className === 'b') {
                    tuesday.push($(this).val());
                }
                if (this.className === 'c') {
                    wednesday.push($(this).val());
                }
                if (this.className === 'd') {
                    thursday.push($(this).val());
                }
                if (this.className === 'e') {
                    friday.push($(this).val());
                }
                if (this.className === 'f') {
                    saturday.push($(this).val());
                }
                if (this.className === 'g') {
                    sunday.push($(this).val());
                }

            }
        });
        $('#AddEditVolunteer_VoluntaryDetails_MonAvailability').val(monday.join());
        $('#AddEditVolunteer_VoluntaryDetails_TueAvailability').val(tuesday.join());
        $('#AddEditVolunteer_VoluntaryDetails_WedAvailability').val(wednesday.join());
        $('#AddEditVolunteer_VoluntaryDetails_ThuAvailability').val(thursday.join());
        $('#AddEditVolunteer_VoluntaryDetails_FriAvailability').val(friday.join());
        $('#AddEditVolunteer_VoluntaryDetails_SatAvailability').val(saturday.join());
        $('#AddEditVolunteer_VoluntaryDetails_SunAvailability').val(sunday.join());
    });

    $('#add').click(function () {
        var td = '<td><a style="cursor:pointer;margin-right:5px;color:red;" class="fa fa-minus"></td>';
        var td1 = '<td><input type="text" id="Organization" class="form-control" /></td>';
        var td2 = '<td><input type="text" id="Description" class="form-control" /></td>';
        var td3 = '<td><input type="text" id="Period" class="form-control" /></td>';
        $("#pastrecord").append('<tr>' + td + td1 + td2 + td3 + '</tr>');
        addDeleteRowclick();

    });
    // Combine data in Past Voluntary Table
    $('#pastrecord').change(function () {
        var data = new Array();
        $('#pastrecord tr').each(function () {
            var Organization = $(this).find('#Organization').val();
            var Description = $(this).find('#Description').val();
            var Period = $(this).find('#Period').val();
            if (Organization !== "") {
                var alldata = {
                    "Organization": Organization,
                    "Description": Description,
                    "Period": Period
                }
                data.push(alldata);
            }
        });

        if (data.length !== 0) {
            $('#AddEditVolunteer_VoluntaryDetails_PastRecord').val(JSON.stringify(data));
        } else {
            $('#AddEditVolunteer_VoluntaryDetails_PastRecord').val("");
        }
    });
    function addDeleteRowclick() {
        /*var index, table = document.getElementById('pastvoluntarytbl');*/
        var index, table = document.getElementById('pastvoluntarytbl');
        console.log(table.rows.length);
        for (var i = 1; i < table.rows.length - 1; i++) {
            var currentRow = table.rows[i].cells[0];
            //console.log(currentRow)
            currentRow.onclick = function () {
                index = this.parentElement.rowIndex;
                //console.log(index)
                table.deleteRow(index);
                var data = new Array();
                $('#pastrecord tr').each(function () {
                    var Organization = $(this).find('#Organization').val();
                    var Description = $(this).find('#Description').val();
                    var Period = $(this).find('#Period').val();
                    if (Organization !== "") {
                        var alldata = {
                            "Organization": Organization,
                            "Description": Description,
                            "Period": Period
                        }
                        data.push(alldata);
                    }
                });

                if (data.length !== 0) {
                    $('#AddEditVolunteer_VoluntaryDetails_PastVoluntaryRecord').val(JSON.stringify(data));
                } else {
                    $('#AddEditVolunteer_VoluntaryDetails_PastVoluntaryRecord').val("");
                }
            }
            //table.rows[i].cells[1].onclick = function () {
            //    var c = confirm("do you want to delete this row");
            //    if (c === true) {
            //        index = this.parentElement.rowIndex;
            //        table.deleteRow(index);
            //    }

            //    //console.log(index);
            //};

        }
    }

    //for dropdown others selection 
    $('#ddlNationality').change(function () {
        var selectedCountry = $(this).children("option:selected").val();
        if (selectedCountry == "Others") {
            document.getElementById("otherNationality").style.display = "block";
        }
        else {
            document.getElementById("otherNationality").style.display = "none";
        }

    });

    $('#ddlRace').change(function () {
        var selectedRace = $(this).children("option:selected").val();
        if (selectedRace == "Other") {
            document.getElementById("otherRace").style.display = "block";
        }
        else {
            document.getElementById("otherRace").style.display = "none";
        }

    });
    $('#ddlReligion').change(function () {
        var selectedRace = $(this).children("option:selected").val();
        if (selectedRace == "Other") {
            document.getElementById("otherReligion").style.display = "block";
        }
        else {
            document.getElementById("otherReligion").style.display = "none";
        }

    });

    $("#registrations-form").submit(function (event) {
        event.preventDefault(); // Prevent the default form submission

        // Make an AJAX request to submit the form data
        $.ajax({
            url: $(this).attr("action"),
            type: $(this).attr("method"),
            data: $(this).serialize(),
            success: function (data) {
                if (data.success == true) {
                    OnSuccess(data);
                }
                else {
                    OnFailure(data);
                }
            }
        });
    });
    function OnSuccess(result) {
        //console.log(result.Data.message)
        if (result.success) {
            let timerInterval
            Swal.fire({
                icon: 'success',
                title: result.data.message,
                timer: 1300,
                timerProgressBar: true,
                didOpen: () => {
                    Swal.showLoading()
                    const b = Swal.getHtmlContainer().querySelector('b')
                    timerInterval = setInterval(() => {
                        //b.textContent = Swal.getTimerLeft()
                    }, 100)
                },
                willClose: () => {
                    clearInterval(timerInterval)
                }
            }).then((result) => {
                if (result.dismiss === Swal.DismissReason.timer) {
                    window.location.replace("http://45.32.105.34:5587/VolunteerManagement/RegistrationDone");
                }
            })

        } else {

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: result.data.message

            })
            return;
        }

    }
    function OnFailure(result) {
        console.log("failure");
        console.log(result);
    }
})



function initPhoneFieldwithPara(fieldID, HiddenID, x) {
    //initialize contact no mobile
    //var a = $(HiddenID).val();
    var input = document.querySelector(fieldID);
    var phone = window.intlTelInput(input, {
        onlyCountries: ["sg", "my", "id", "cn"],
        utilsScript: "/js/utils.js",
        initialCountry: "sg",
        formatOnDisplay: true,
        separateDialCode: true,
        autoPlaceholder: "aggressive",
        geoIpLookup: function (callback) {
            $.get('https://ipinfo.io', function () { }, "jsonp").always(function (resp) {
                var countryCode = (resp && resp.country) ? resp.country : "";
                callback(countryCode);
            });
        }
    });

    //Set value to input

    if (x == 2) {
        //$(fieldID).val("");
        //var a = $(HiddenID).val();console.log(a)
        phone.setNumber($(HiddenID).val());//for edit purpose:initialize back phone number
    } else {
        phone.setNumber($(fieldID).val());
    }

    $(fieldID).change(function () {//set value into hidden input
        const fullNumber = $(fieldID).intlTelInput("getNumber")
        //console.log(fullNumber[0].value)
        //console.log(11)
        if (fullNumber[0].value != "") {
            $(HiddenID).val(fullNumber[0].parentElement.innerText + fullNumber[0].value);

        } else {
            $(HiddenID).val("");

        }
    });

    var z = phone.getSelectedCountryData().dialCode;
    $(fieldID).rules("add", {
        number: true

    });
    if (z == "65") {
        //$(fieldID).attr('maxlength', 8);
        $(fieldID).rules("add", {
            /*number: true,*/
            maxlength: 8,
            minlength: 8
        });
    } else if (z == "60") {
        $(fieldID).rules("add", {
            /*number: true,*/
            maxlength: 10,
            minlength: 9

        });
    }
    else if (z == "86") {
        //$('#PhoneNumber').attr('maxlength', 12);
        $(fieldID).rules("add", {
            /*number: true,*/
            maxlength: 11,
            minlength: 11

        });
    }
    else if (z == "62") {
        //$('#PhoneNumber').attr('maxlength', 12);
        $(fieldID).rules("add", {
            /*number: true,*/
            maxlength: 9,
            minlength: 9

        });
    } else {
        //$(fieldID).attr('maxlength', 12);
        $(fieldID).rules("add", {
            /*number: true,*/
            maxlength: 12

        });
    }
    input.addEventListener("countrychange", function () {

        //console.log("country chaneg trigger");
        var z = phone.getSelectedCountryData().dialCode;

        //$(fieldID).val("");
        //phone.setNumber($(HiddenID).val());
        if (z == "65") {
            $(fieldID).rules("remove")
            $(fieldID).rules("add", {
                /*number: true,*/
                maxlength: 8,
                minlength: 8

            });
        }
        else if (z == "60") {
            $(fieldID).rules("remove")
            $(fieldID).rules("add", {
                /*number: true,*/
                maxlength: 10,
                minlength: 9

            });
        }
        else if (z == "86") {
            $(fieldID).rules("remove")
            //$('#PhoneNumber').attr('maxlength', 12);
            $(fieldID).rules("add", {
                /*number: true,*/
                maxlength: 11,
                minlength: 11

            });
        }
        else if (z == "62") {
            $(fieldID).rules("remove")
            //$('#PhoneNumber').attr('maxlength', 12);
            $(fieldID).rules("add", {
                /*number: true,*/
                maxlength: 9,
                minlength: 9

            });
        }

    });
    $(fieldID).rules("add", {
        required: true
    });

}






//function ValidateEmail($('#AddEditVolunteer_PersonalDetails_Email').val()) {
//    var mailformat = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
//    if (inputText.value.match(mailformat)) {
//        alert("Valid email address!");
//        document.form1.text1.focus();
//        return true;
//    }
//    else {
//        alert("You have entered an invalid email address!");
//        document.form1.text1.focus();
//        return false;
//    }
//}


// validate email with abstractapi

//$('#AddEditVolunteer_PersonalDetails_Email').change(() => {
//    $.getJSON("https://emailvalidation.abstractapi.com/v1/?api_key=dc1b2c2cfa314680ae5fd132a30426f2&email=" + $('#AddEditVolunteer_PersonalDetails_Email').val() , function (data) {
//        //console.log(data.deliverability);

//        if (data.deliverability == 'DELIVERABLE') {
//            console.log(data)
//            console.log('ok')
//            return true
//        } else {
//            console.log(data)
//            $('#AddEditVolunteer_PersonalDetails_Email').val('') 
//            alert('invalid email ')
//        }
//    })
//})


$(function () {

    $('#ddlSkill').multiselect({
        nonSelectedText: '---Select Qualification & Skill 选择资质与技能---',
        dropRight: true

    });

    $('#ddlDriving').multiselect({
        nonSelectedText: '---Select Driving License 选择驾照---',
        dropRight: true
    });

    $('#ddlLanguage').multiselect({
        nonSelectedText: '---Select Language of Preference 选择优先语言---',
        dropRight: true
    });

    $('#ddlKnow').multiselect({
        nonSelectedText: '---May select multiple 可选择多个---',
        dropRight: true
    });
});

function ddlSkill_change() {
    var selected = [];
    for (var option of document.getElementById('ddlSkill').options) {
        if (option.selected) {
            selected.push(option.value);
        }
    }
    if (selected.contains("Others")) {
        var txt = document.getElementById("txtSkill");
        txt.style = "display:block;margin-top:10px";
    }
    else {
        var txt = document.getElementById("txtSkill");
        txt.style = "display:none";
    }

    $('#hidden_skill').val(selected);
}

function ddlEducation_change() {
    var selected = [];
    for (var option of document.getElementById('ddlEducation').options) {
        if (option.selected) {
            selected.push(option.value);
        }
    }
    if (selected.contains("Others")) {
        var txt = document.getElementById("txtEducation");
        txt.style = "display:block;margin-top:10px";
    }
    else {
        var txt = document.getElementById("txtEducation");
        txt.style = "display:none";
    }
}

function ddlEmployment_change() {

    var selected = [];
    for (var option of document.getElementById('ddlEmployment').options) {
        if (option.selected) {
            selected.push(option.value);
        }
    }

    if (selected.contains("Others")) {
        var txt = document.getElementById("txtEmployment");
        txt.style = "display:block;margin-top:10px";
    }
    else {
        var txt = document.getElementById("txtEmployment");
        txt.style = "display:none";
    }
}

function ddlKnow_change() {
    var selected = [];
    for (var option of document.getElementById('ddlKnow').options) {
        if (option.selected) {
            selected.push(option.value);
        }
    }
    if (selected.contains("Others")) {
        var txt = document.getElementById("txtKnownBy");
        txt.style = "display:block;margin-top:10px";
    }
    else {
        var txt = document.getElementById("txtKnownBy");
        txt.style = "display:none";
    }

    $('#hidden_know').val(selected);
}

function ddlLanguage_change() {
    var selected = [];
    for (var option of document.getElementById('ddlLanguage').options) {
        if (option.selected) {
            selected.push(option.value);
        }
    }
    if (selected.contains("Others")) {
        var txt = document.getElementById("txtLanguage");
        txt.style = "display:block;margin-top:10px";
    }
    else {
        var txt = document.getElementById("txtLanguage");
        txt.style = "display:none";
    }

    $('#hidden_language').val(selected);
}

function ddlMedical_change() {
    var ddl = document.getElementById("ddlMedical");
    var txt = document.getElementById("txtMedical");
    if (ddl.value == "Yes") {

        txt.style = "display:block;margin-top:10px";
    }
    else {
        txt.style = "display:none;";
    }
}

function rdClick(name) {
    var Val;
    if (name == "q1") {
        Val = $("input[name='AddEditVolunteer.PersonalDeclare.question1']:checked").val();
        var txt = document.getElementById("txtPersonal_q1");
        if (Val == "Yes") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q2") {
        Val = $("input[name='AddEditVolunteer.PersonalDeclare.question2']:checked").val();
        var txt = document.getElementById("txtPersonal_q2");
        if (Val == "Yes") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q3") {
        Val = $("input[name='AddEditVolunteer.PersonalDeclare.question3']:checked").val();
        var txt = document.getElementById("txtPersonal_q3");
        if (Val == "Yes") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "Nationality") {
        Val = $("input[name='AddEditVolunteer.PersonalDetails.Nationality']:checked").val();
        var txt = document.getElementById("otherNationality");
        if (Val == "Others") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "Race") {
        Val = $("input[name='AddEditVolunteer.PersonalDetails.Race']:checked").val();
        var txt = document.getElementById("otherRace");
        if (Val == "Others") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "med") {
        Val = $("input[name='AddEditVolunteer.MedicalDeclare.question1']:checked").val();
        var txt = document.getElementById("txtMedical");
        if (Val == "Yes") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }

}

function chkAck() {
    var chk = $("input[name='AddEditVolunteer.Acknowledge.general']:checked").val();
    var chk2 = $("input[name='AddEditVolunteer.Acknowledge.question1']:checked").val();
    var chk3 = $("input[name='AddEditVolunteer.Acknowledge.question2']:checked").val();
    var chk4 = $("input[name='AddEditVolunteer.Acknowledge.question3']:checked").val();


    var btn = document.getElementById("btnSubmit");
    if (chk != null && chk2 != null && chk3 != null && chk4 != null) {
        btn.disabled = false;
    }
    else {
        btn.disabled = true;
    }

}

$.extend($.validator, {
    messages: {
        required: "This field is required.",
        remote: "Please fix this field.",
        email: "Please enter a valid email address or enter 'NA' if no email address.",
        url: "Please enter a valid URL.",
        date: "Please enter a valid date.",
        dateISO: "Please enter a valid date (ISO).",
        number: "Please enter a valid number.",
        digits: "Please enter only digits.",
        creditcard: "Please enter a valid credit card number.",
        equalTo: "Please enter the same value again.",
        accept: "Please enter a value with a valid extension.",
        maxlength: $.validator.format("Please enter no more than {0} digits."),
        minlength: $.validator.format("Please enter at least {0} digits"),
        number: "Please enter a valid number.",
        rangelength: $.validator.format("Please enter a value between {0} and {1} characters long."),
        range: $.validator.format("Please enter a value between {0} and {1}."),
        max: $.validator.format("Please enter a value less than or equal to {0}."),
        min: $.validator.format("Please enter a value greater than or equal to {0}.")
    },
})

//function onSuccess(result) {
//    //console.log(result.Data.message)
//    if (result.success) {
//        let timerInterval
//        Swal.fire({
//            icon: 'success',
//            title: result.data.message,
//            timer: 1300,
//            timerProgressBar: true,
//            didOpen: () => {
//                Swal.showLoading()
//                const b = Swal.getHtmlContainer().querySelector('b')
//                timerInterval = setInterval(() => {
//                    //b.textContent = Swal.getTimerLeft()
//                }, 100)
//            },
//            willClose: () => {
//                clearInterval(timerInterval)
//            }
//        }).then((result) => {
//            if (result.dismiss === Swal.DismissReason.timer) {
//                window.location.replace("../VolunteerManagement/RegistrationDone")
//            }
//        })

//    } else {

//        Swal.fire({
//            icon: 'error',
//            title: 'Error',
//            text: result.data.message

//        }).then((result) => {
//            if (result.isConfirmed) {
//                // User clicked "OK", navigate to another page
//                window.location.replace("../VolunteerManagement/RegistrationExist")
//            }

//        })
//        return;
//    }

//}
//function onFailure(result) {
//    console.log("failure");
//    console.log(result);
//}



