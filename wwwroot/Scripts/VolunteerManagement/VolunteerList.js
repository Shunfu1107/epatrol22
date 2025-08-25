$(document).ready(function () {
    var t = $('#Volunteer').DataTable({
        "pagingType": "full_numbers"
    });
    t.on('order.dt search.dt', function () {
        t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
    $("table").removeAttr("style");

    $('#Volunteer_paginate').click(() => {
        if (document.querySelector('#full-nric').style.display == 'block') {
            $('.show-ic').css("display", "none")
            $('.hide-ic').css("display", "inline")
        } else {

            $('.show-ic').css("display", "inline")
            $('.hide-ic').css("display", "none")
        }
    });
    $('#question1check').change(function () {
        var data = [];
        $('#question1check input[type=checkbox]').each(function () {
            if (this.checked) {
                data.push($(this).val());
            }
        });
        $('#IAForm_q1').val(data.join());
    });

    $("#SubmitEditVolunteer").submit(function (event) {
        event.preventDefault(); // Prevent the default form submission
        // Make an AJAX request to submit the form data
        $.ajax({
            url: $(this).attr("action"),
            type: $(this).attr("method"),
            data: $(this).serialize(),
            success: function (data) {
                console.log(data);
                if (data.success == true) {
                    OnSuccess(data);
                }
                else {
                    OnFailure(data);
                }
            }
        });
    });
})

$('.show-ic').click(function () {
    $('.hide-ic').css("display", "inline")
    $('.show-ic').css("display", "none")

    $('.show-ic').val(true)
    $('.hide-ic').val(false)

    var fullNric = document.querySelectorAll('#full-nric')

    fullNric.forEach(ic => {
        ic.style.display = "block"
    })

    var encryptedNric = document.querySelectorAll('#nric')

    encryptedNric.forEach(ic => {
        ic.style.display = "none"
    })
})

$('.hide-ic').click(function () {
    $('.show-ic').css("display", "inline")
    $('.hide-ic').css("display", "none")
    $('.hide-ic').val(true)
    $('.show-ic').val(false)

    var fullNric = document.querySelectorAll('#full-nric')

    fullNric.forEach(ic => {
        ic.style.display = "none"
    })

    var encryptedNric = document.querySelectorAll('#nric')

    encryptedNric.forEach(ic => {
        ic.style.display = "block"
    })
})

var volunteerLength = document.querySelectorAll('#nric').length
var array = document.querySelectorAll('#nric')


array.forEach(volunteer => {
    //volunteer.innerHTML = 'weqweqw'
    if (volunteer.innerHTML) {
        var startIC = volunteer.innerHTML.slice(0, 3)
        var endIC = volunteer.innerHTML.slice(-4)
        volunteer.innerHTML = '***' + endIC


    }
})






function   viewDetails(userid) {
    $("#ViewVolunteerDetailsModal").modal("show");
    //alert(userid);
    $.ajax({
        url: '/VolunteerManagement/GetVolunteerDetailsById?UserId=' + userid,
        type: 'POST',
        success: function (result) {
            console.log(result);
            console.log(result.voluntaryDetail.satAvailability);


            $(" #viewPDtbl tr td").each(function () {


                $(this).empty();

            });
            if (result.dateOfJoin != null) {
                var DOJ = new Date(result.dateOfJoin);
                //var DOJ = new Date(parseInt((result.dateOfJoin).match(/\d+/)[0]));
            }
            if (result.dob != null) {
                var DOB = new Date(result.dob);

                //var DOB = new Date(parseInt((result.dob).match(/\d+/)[0]))
            }
            console.log(DOJ);
            console.log(DOB);
            let hiddenNric = result.nric.slice
            //personal details
            $("#view-PD-name").append(result.fullname);
            $("#view-PD-preferredname").append(result.preferredName);
            $("#view-PD-nric").append(result.nric);
            $("#view-PD-email").append(result.email);
            $("#view-PD-gender").append(result.gender);
            $("#view-PD-ezlink").append(result.ezlinkCard);
            $("#view-PD-status").append(result.status);
            $("#view-PD-type").append(result.type);
            $("#view-PD-role").append(result.role);
            $("#view-PD-DOJ").append($.datepicker.formatDate('dd M yy', DOJ));
            /*$("#view-PD-DOB").append($.datepicker.formatDate('dd M yy', DOB));*/
            $("#view-PD-DOB").append(result.dobView);
            $("#view-PD-MaritalStatus").append(result.maritalStatus);
            $("#view-PD-Nationality").append(result.nationality);
            $("#view-PD-Race").append(result.race);
            $("#view-PD-Religion").append(result.religion);
            $("#view-PD-add1").append(result.address1);
            $("#view-PD-add2").append(result.address2);
            $("#view-PD-postal").append(result.postal);
            $("#view-PD-mobile").append(result.mobile);
            $("#view-PD-DL").append(result.drivingLicense);
            $("#view-PD-ToS").append(result.typeOfService);
            $("#view-PD-Programme").append(result.programme);
            $("#view-PD-Occupation").append(result.occupation);
            $("#view-PD-Institution").append(result.institution);
            $("#view-PD-qualification").append(result.qualification);
            $("#view-PD-skills").append(result.skills);
            $("#view-PD-language").append(result.language);
            $("#view-PD-education").append(result.education);
            $("#view-PD-Employ").append(result.employmentStatus);



            //$("#view-PD-name").append(result.Fullname);

            //initialize emergency contact details
            $(" #viewKintbl tr td").each(function () {

                $(this).empty();

            });
            $("#view-Kin-name").append(result.volunteerKindetail.name);
            $("#view-Kin-relationship").append(result.volunteerKindetail.relationship);
            $("#view-Kin-mobile").append(result.volunteerKindetail.contact);

            //voluntarydetails
            $(" #viewVDtbl tr td").each(function () {

                $(this).empty();

            });
            $("#view-VD-reasonvolunteer").append(result.voluntaryDetail.reasonOfVolunteering);
            $("#view-VD-language").append(result.voluntaryDetail.languageSpoken);
            $("#view-VD-hobby").append(result.voluntaryDetail.hobby);
            $("#view-VD-why").append(result.voluntaryDetail.reasonVolunteer);
            $("#view-VD-getknown").append(result.voluntaryDetail.getKnownBy);

            // for availability table checkbox 

            const monAval = result.voluntaryDetail.monAvailability?.split(",");
            const tueAval = result.voluntaryDetail.tueAvailability?.split(",");
            const wedAval = result.voluntaryDetail.wedAvailability?.split(",");
            const thuAval = result.voluntaryDetail.thuAvailability?.split(",");
            const friAval = result.voluntaryDetail.friAvailability?.split(",");
            const satAval = result.voluntaryDetail.satAvailability?.split(",");
            const sunAval = result.voluntaryDetail.sunAvailability?.split(",");

            for (let x = 0; x < monAval?.length; x++) {
                availVal = monAval[x]
                if (availVal == 1) {
                    $('#monMorn').prop('checked', true);
                }
                if (availVal == 2) {
                    $('#monAft').prop('checked', true);
                }
                if (availVal == 3) {
                    $('#monEve').prop('checked', true);
                }
            }

            for (let x = 0; x < tueAval?.length; x++) {
                availVal = tueAval[x]
                if (availVal == 1) {
                    $('#tueMorn').prop('checked', true);
                }
                if (availVal == 2) {
                    $('#tueAft').prop('checked', true);
                }
                if (availVal == 3) {
                    $('#tueEve').prop('checked', true);
                }
            }

            for (let x = 0; x < wedAval?.length; x++) {
                availVal = wedAval[x]
                if (availVal == 1) {
                    $('#wedMorn').prop('checked', true);
                }
                if (availVal == 2) {
                    $('#wedAft').prop('checked', true);
                }
                if (availVal == 3) {
                    $('#wedEve').prop('checked', true);
                }
            }

            for (let x = 0; x < thuAval?.length; x++) {
                availVal = thuAval[x]
                if (availVal == 1) {
                    $('#thuMorn').prop('checked', true);
                }
                if (availVal == 2) {
                    $('#thuAft').prop('checked', true);
                }
                if (availVal == 3) {
                    $('#thuEve').prop('checked', true);
                }
            }

            for (let x = 0; x < friAval?.length; x++) {
                availVal = friAval[x]
                if (availVal == 1) {
                    $('#friMorn').prop('checked', true);
                }
                if (availVal == 2) {
                    $('#friAft').prop('checked', true);
                }
                if (availVal == 3) {
                    $('#friEve').prop('checked', true);
                }
            }

            for (let x = 0; x < satAval?.length; x++) {
                availVal = satAval[x]
                if (availVal == 1) {
                    $('#satMorn').prop('checked', true);
                }
                if (availVal == 2) {
                    $('#satAft').prop('checked', true);
                }
                if (availVal == 3) {
                    $('#satEve').prop('checked', true);
                }
            }

            for (let x = 0; x < sunAval?.length; x++) {
                availVal = sunAval[x]
                if (availVal == 1) {
                    $('#sunMorn').prop('checked', true);
                }
                if (availVal == 2) {
                    $('#sunAft').prop('checked', true);
                }
                if (availVal == 3) {
                    $('#sunEve').prop('checked', true);
                }
            }


            // for area of interest

            const AOI = result.voluntaryDetail.areaOfInterest?.split(",");
            //AOI.forEach(initializeAOI);
            $('#viewareaInterest input:checked').removeAttr('checked');
            $("#viewareaInterest input[type=checkbox]").each(function () {
                $(this).prop("checked", false);

                //console.log("aa")
            });
            //int x;



            for (let x = 0; x < AOI?.length; x++) {
                aoiVal = AOI[x];
                if (aoiVal == 1) {
                    $('#checkbox1').prop('checked', true);
                } else if (aoiVal == 2) {
                    $('#checkbox2').prop('checked', true);
                }
                else if (aoiVal == 3) {
                    $('#checkbox3').prop('checked', true);
                }
                else if (aoiVal == 4) {
                    $('#checkbox4').prop('checked', true);
                }
                else if (aoiVal == 5) {
                    $('#checkbox5').prop('checked', true);
                }
                else if (aoiVal == 6) {
                    $('#checkbox6').prop('checked', true);
                }

            }


            //$('#availability input:checked').removeAttr('checked');
            //$("#availability input[type=checkbox]").each(function () {
            //    $(this).prop("checked", false);


            //});



            $(" #viewPPtbl tr td").each(function () {

                $(this).empty();

            });

            $("#view-PP-name").append(result.fullname);
            $("#view-PP-NRIC").append(result.nric);

            $(" #viewDDtbl tr td").each(function () {

                $(this).empty();

            });

            $("#view-PD-medical_declare").append(result.medicalDeclaration);

            $(" #viewDDPtbl tr td").each(function () {

                $(this).empty();

            });

            $("#view-PD-personal_q1").append(result.personalDeclaration_q1);
            $("#view-PD-personal_q2").append(result.personalDeclaration_q2);
            $("#view-PD-personal_q3").append(result.personalDeclaration_q3);

            $(" #viewDDAtbl tr td").each(function () {

                $(this).empty();

            });

            $("#view-PD-ack").append(result.ack_general);
            $("#view-PD-ack_q1").append(result.ack_q1);
            $("#view-PD-ack_q2").append(result.ack_q2);
            $("#view-PD-ack_q3").append(result.ack_q3);
        }
    });
   

}
function rdClick(name) {
    var Val;
    if (name == "q3") {
        Val = $("input[name='IAForm.q3']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q3");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q7") {
        Val = $("input[name='IAForm.q7']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q7");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q8") {
        Val = $("input[name='IAForm.q8']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q8");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q9") {
        Val = $("input[name='IAForm.q9']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q9");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q10") {
        Val = $("input[name='IAForm.q10']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q10");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q11") {
        Val = $("input[name='IAForm.q11']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q11");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q13") {
        Val = $("input[name='IAForm.q13']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q13");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q14") {
        Val = $("input[name='IAForm.q14']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q14");
        if (Val == "Yes") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
}



function editDetails(userid) {
    $("#EditVolunteerDetailsModal").modal("show");
    document.getElementById('ValueotherNationality').value = "";
    document.getElementById('ValueotherRace').value = "";
    document.getElementById('ValueotherEducation').value = "";

    $.ajax({
        url: '/VolunteerManagement/GetVolunteerDetailsById?UserId=' + userid,
        type: 'POST',
        success: function (result) {
            console.log(result);

            $(" #EditPDtbl tr td").each(function () {

                $(this).empty();

            });
            if (result.dateOfJoin != null) {
                var DOJ = new Date(parseInt((result.dateOfJoin).match(/\d+/)[0]))
            }
            if (result.dob != null) {
                var DOB = new Date(parseInt((result.dob).match(/\d+/)[0]))

            }
            console.log(userid);
            console.log(result);

            $("#AddEditVolunteer_UserID").val(userid);

            $("#ViewExitForm-name").val(result.name);

            $("#Edit-PD-name").val(result.fullname);
            $("#Edit-PD-preferredname").val(result.preferredName);
            $("#Edit-PD-nric").val(result.nric);
            $("#Edit-PD-email").val(result.email);
            $("#Edit-PD-gender").val(result.gender);

            if (result.gender !== "") {
                var data = result.gender;

                $('#Editgender input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        this.checked = true;
                    }

                });
            }
            $('#Edit-PD-ezlink').val(result.ezlinkCard);

            $('#Edit-PD-DOJ').datepicker({
                startView: 2,
                format: 'dd/mm/yyyy'

            });
     /*       var dte = new Date(result.dateOfJoinString)*/
            $("#Edit-PD-DOJ").val(result.dateOfJoinstring);

            $('#Edit-PD-DOB').datepicker({
                startView: 2,
                format: 'dd/mm/yyyy'

            });
            $("#Edit-PD-DOB").val(result.doBstring);

            /* $("#Edit-PD-DOJ").val($.datepicker.formatDate('dd M yy', DOJ));*/
            /* $("#Edit-PD-DOB").val($.datepicker.formatDate('dd M yy', DOB));*/
            $("#Edit-PD-MaritalStatus").val(result.maritalStatus);

            if (result.maritalStatus !== "") {
                var data = result.maritalStatus;

                $('#EditMaritalStatus input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        this.checked = true;
                    }

                });
            }


            $("#Edit-PD-Nationality").val(result.nationality);

            if (result.nationality !== "") {
                var data = result.nationality;

                /*  $('#EditNationality input[type=radio]').each(function () {*/
                $('#EditNationality input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        this.checked = true;

                        breakOut = true;
                        return false;


                    }


                    if ((data != "Singaporean 新加坡公民" && data != "PR 永久居民" && data != "Long Term Pass Holder 长期居留证")) {
                        document.getElementById("EditOthersNationality").checked = true;
                        document.getElementById("ValueotherNationality").style.visibility = 'visible';

                        $("#ValueotherNationality").val($("#ValueotherNationality").val() + data);
                        /* $('#AddEditVolunteer_PersonalDetails_OtherNationality').val(data.join());*/
                        breakOut = true;
                        return false;
                    }

                });

            }
            $('#EditNationality').change(function () {
                var data = []; // declare array
                var data1 = [];
                $('#EditNationality input[type=radio]').each(function () { //loop every checkbox
                    //if (this.checked) { // if checked
                    //    data.push($(this).val()); //push data to array
                    //}
                    if ($(this).attr("id") == "EditOthersNationality") {
                        //  data.push($(this).val());
                        //data1.push(document.getElementById("other_q1").value);
                        //document.getElementById("VolunteerExit_otherPositiveExp").value = document.getElementById("other_q1").value;
                        //$('#VolunteerExit_otherPositiveExp').val($("other_q1").val());
                        data1.push(document.getElementById("ValueotherNationality").value);
                    }
                    //else {
                    //    data1.push($(this).val());
                    //}

                });
                $('#AddEditVolunteer_PersonalDetails_OtherNationality').val(data1.join()); //join array into hiddent text
                //$('#VolunteerExit_otherImprove').val(data1.join());
            });


            $("#Edit-PD-Race").val(result.race);

            if (result.race !== "") {
                var data = result.race;

                $('#EditRace input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        this.checked = true;
                        breakOut = true;
                        return false;
                    }

                    /*if (($(this).val() != data)) {*/

                    if ((data != "Chinese 华裔" && data != "Malay 巫裔" && data != "Indian 印裔")) {
                        document.getElementById("Race_Others").checked = true;
                        document.getElementById("ValueotherRace").style.visibility = 'visible';
                        $("#ValueotherRace").val($("#ValueotherRace").val() + data);
                        //$('#AddEditVolunteer_PersonalDetails_OtherRace').val(data.join());
                        breakOut = true;
                        return false;

                    }

                });
            }
            $('#EditRace').change(function () {
                var data = []; // declare array
                var data1 = [];
                $('#EditRace input[type=radio]').each(function () { //loop every checkbox
                    //if (this.checked) { // if checked
                    //    data.push($(this).val()); //push data to array
                    //}
                    if ($(this).attr("id") == "Race_Others") {
                        //  data.push($(this).val());
                        //data1.push(document.getElementById("other_q1").value);
                        //document.getElementById("VolunteerExit_otherPositiveExp").value = document.getElementById("other_q1").value;
                        //$('#VolunteerExit_otherPositiveExp').val($("other_q1").val());
                        data1.push(document.getElementById("ValueotherRace").value);
                    }
                    //else {
                    //    data1.push($(this).val());
                    //}

                });
                $('#AddEditVolunteer_PersonalDetails_OtherRace').val(data1.join()); //join array into hiddent text
                //$('#VolunteerExit_otherImprove').val(data1.join());
            });
            $("#Edit-PD-Religion").val(result.religion);
            $("#Edit-PD-add1").val(result.address1);
            $("#Edit-PD-add2").val(result.address2);
            $("#Edit-PD-postal").val(result.postal);
            $("#Edit-PD-mobile").val(result.mobile);
            $("#Edit-PD-DL").val(result.drivingLicense);


            if (result.drivingLicense !== "") {
                var data = result.drivingLicense;

                $('#EditDrivingLicense input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        this.checked = true;
                    }

                });
            }
            $("#Edit-PD-ToS").val(result.typeOfService);
            $("#Edit-PD-Programme").val(result.programme);
            $("#Edit-PD-Occupation").val(result.occupation);
            $("#Edit-PD-Institution").val(result.institution);
            $("#Edit-PD-qualification").val(result.qualification);


            if (result.qualification !== "" && result.qualification !== null) {

                //var getDataSplitted = [];
                //getDataSplitted = result.Qualification.split(",");

                var data = result.qualification.split(",");


                $('#EditQualification input[type=checkbox]').each(function () {
                    if ($.inArray($(this).val(), data) > -1) {
                        /* if (($(this).val() == data)) {*/
                        console.log(data);
                        this.checked = true;

                    }
                });

                if (data != "") {
                    $.each(data, function (index) {

                        var splitdata = data[index];
                        if (splitdata != "Organisation Skill 组织能力" && splitdata != "First Aid/CPR 急救/心肺复苏术" && splitdata != "Nursing 护理" && splitdata != "Counseling 辅导" && splitdata != "Therapy 治疗" && splitdata != "Adminstration 行政" && splitdata != "Teaching 教导") {
                            document.getElementById("OthersQualification").checked = true;
                            document.getElementById("other_Qualification").style.visibility = 'visible';

                            $("#other_Qualification").val(splitdata);

                        }
                    });
                }

            }

            $('#EditQualification').change(function () {
                var data = []; // declare array
                var data1 = [];
                var data3 = [];
                $('#EditQualification input[type=checkbox]').each(function () { //loop every checkbox
                    if (this.checked) { // if checked
                        /* console.log("inEdit");*/
                        data.push($(this).val()); //push data to array
                        console.log(data)
                   
                    }

                });
                if ($(this).attr("id") == "OthersQualification" || data == "Others 其他") {
                 

                    data1.push(document.getElementById("other_Qualification").value);
                    /*data = "";*/

                    //if (data1.length != 0) {
                    $('#other_Qualification').val(data1.join());
                    //    //$("#ValueotherEmployment").val($("#ValueotherEmployment").val() + data1);
                    //    $('#AddEditVolunteer_PersonalDetails_EmploymentStatus').val(data1.join());
                    //}

                    //breakOut = true;
                    //return false;

                }
                console.log(data);
                console.log(data1);

                if (data.length != 0 && data1.length != 0) {
                    data3 = data + data1;


                    $('#AddEditVolunteer_PersonalDetails_Qualification').val(data3.join());

                    breakOut = true;
                    return false;
                }

                if (data.length != 0) {
                    $('#AddEditVolunteer_PersonalDetails_Qualification').val(data.join());
                }
            });

            $("#Edit-PD-skills").val(result.skills);
            $("#Edit-PD-language").val(result.language);

            if (result.language !== "" && result.language !== null) {

                //var getDataSplitted = [];
                //getDataSplitted = result.Qualification.split(",");

                var data = result.language.split(",");


                $('#EditLanguage input[type=checkbox]').each(function () {
                    if ($.inArray($(this).val(), data) > -1) {
                        /* if (($(this).val() == data)) {*/
                        console.log(data);
                        this.checked = true;

                    }
                });

                if (data != "") {
                    $.each(data, function (index) {

                        var splitdata = data[index];
                        if (splitdata != "English" && splitdata != "Mandarin" && splitdata != "Malay" && splitdata != "Tamil") {
                            document.getElementById("OthersLanguage").checked = true;
                            document.getElementById("other_Language").style.visibility = 'visible';

                            $("#other_Language").val(splitdata);

                        }
                    });
                }

            }

            $('#EditLanguage').change(function () {
                var data = []; // declare array
                var data1 = [];
                var data3 = [];
                $('#EditLanguage input[type=checkbox]').each(function () { //loop every checkbox
                    if (this.checked) { // if checked
                        /* console.log("inEdit");*/
                        data.push($(this).val()); //push data to array
                        console.log(data)
                
                    }

                  

                });
                if ($(this).attr("id") == "OthersLanguage") {
              

                    data1.push(document.getElementById("other_Language").value);
                    //$('#AddEditVolunteer_PersonalDetails_OtherLanguage').val(data1.join());
                    /*data = "";*/

                    //if (data1.length != 0) {
                    //    //$('#ValueotherEmployment').val(data1.join());
                    //    //$("#ValueotherEmployment").val($("#ValueotherEmployment").val() + data1);
                    //    $('#AddEditVolunteer_PersonalDetails_EmploymentStatus').val(data1.join());
                    //}

                    //breakOut = true;
                    //return false;

                }
                console.log(data);
                console.log(data1);

                if (data.length != 0 && data1.length != 0) {
                    data3 = data + data1;


                    $('#AddEditVolunteer_PersonalDetails_Language').val(data3.join());

                    breakOut = true;
                    return false;
                }

                if (data.length != 0) {
                    $('#AddEditVolunteer_PersonalDetails_Language').val(data.join());
                }

                //if (data1.length != 0) {
                //    //$('#ValueotherEmployment').val(data1.join());
                //    //$("#ValueotherEmployment").val($("#ValueotherEmployment").val() + data1);
                //   
                //}
                //if (data1.length!=0) {
                //    $('#AddEditVolunteer_PersonalDetails_EmploymentStatus').val(data1.join()); 

                //}
                //join array into hiddent text   //$('#VolunteerExit_otherImprove').val(data1.join());
            });

            $("#Edit-PD-education").val(result.education);

            if (result.education !== "") {
                var data = result.education;

                /*  $('#EditNationality input[type=radio]').each(function () {*/
                $('#EditEducation input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        this.checked = true;

                        breakOut = true;
                        return false;


                    }


                    if ((data != "Primary 小学" && data != "Secondary 中学" && data != "ITE/A Level/Diploma| 工艺教育/A水准/文凭" && data != "Degree 硕士学位" && data != "Master/Doctorate 博士学位")) {
                        document.getElementById("Education_Others").checked = true;
                        document.getElementById("ValueotherEducation").style.visibility = 'visible';


                        $("#ValueotherEducation").val($("#ValueotherEducation").val() + data);
                        breakOut = true;
                        return false;
                    }

                });

            }




            $("#Edit-Kin-name").val(result.volunteerKindetail.name);
            $("#Edit-Kin-relationship").val(result.volunteerKindetail.relationship);
            $("#Edit-Kin-mobile").val(result.volunteerKindetail.contact);

            $("#AddEditVolunteer_VoluntaryDetails_ReasonOfVolunteering").val(result.voluntaryDetail.reasonOfVolunteering);
            $("#Edit-VD-language").val(result.voluntaryDetail.languageSpoken);
            $("#Edit-VD-hobby").val(result.voluntaryDetail.hobby);
            $("#Edit-VD-why").val(result.voluntaryDetail.reasonVolunteer);
            $("#Edit-VD-getknown").val(result.voluntaryDetail.getKnownBy);

            if (result.voluntaryDetail.getKnownBy !== "" && result.voluntaryDetail.getKnownBy !== null) {

                //var getDataSplitted = [];
                //getDataSplitted = result.Qualification.split(",");

                var data = result.voluntaryDetail.getKnownBy.split(",");


                $('#EditGetKnownBy input[type=checkbox]').each(function () {
                    if ($.inArray($(this).val(), data) > -1) {
                        /* if (($(this).val() == data)) {*/
                        console.log(data);
                        this.checked = true;

                    }
                });

                if (data != "") {
                    $.each(data, function (index) {

                        var splitdata = data[index];
                        if (splitdata != "Giving.sg Website |Giving.sg网站" && splitdata != "Blossom Seeds Website| Blossom Seeds 网站" && splitdata != "Blossom Seeds Facebook| Blossom Seeds 脸书" && splitdata != "Family/Friends| 家庭/朋友" && splitdata != "Other Organisations 其他组织" && splitdata != "TV Media/Newspaper| 电视媒体/报纸") {
                            document.getElementById("OthersGetKnownBy").checked = true;
                            document.getElementById("other_GetKnownBy").style.visibility = 'visible';

                            $("#other_GetKnownBy").val(splitdata);

                        }
                    });
                }

            }

            $('#EditGetKnownBy').change(function () {
                var data = []; // declare array
                var data1 = [];
                var data3 = [];
                $('#EditGetKnownBy input[type=checkbox]').each(function () { //loop every checkbox
                    if (this.checked) { // if checked
                        /* console.log("inEdit");*/
                        data.push($(this).val()); //push data to array
                        console.log(data)
                      
                    }

              

                });
                if ($(this).attr("id") == "OthersGetKnownBy") {
                  
                    data1.push(document.getElementById("other_GetKnownBy").value);
                    //$('#other_GetKnownBy').val(data1.join());
                    //breakOut = true;
                    //return false;

                }
                console.log(data);
                console.log(data1);

                //if (data.length != 0 && data1.length != 0) {
                //    data3 = data + data1;


                //    $('#AddEditVolunteer_VoluntaryDetails_GetKnownBy').val(data3.join());

                //    breakOut = true;
                //    return false;
                //}

                if (data.length != 0) {
                    $('#AddEditVolunteer_VoluntaryDetails_GetKnownBy').val(data.join());
                }

                //if (data1.length != 0) {
                //    //$('#other_GetKnownBy').val(data1.join());
                //    //$("#ValueotherEmployment").val($("#ValueotherEmployment").val() + data1);
                //    $('#AddEditVolunteer_VoluntaryDetails_GetKnownBy').val(data1.join());
                //}
                //if (data1.length!=0) {
                //    $('#AddEditVolunteer_PersonalDetails_EmploymentStatus').val(data1.join()); 

                //}
                //join array into hiddent text   //$('#VolunteerExit_otherImprove').val(data1.join());
            });



            const AOInterest = result.voluntaryDetail.areaOfInterest?.split(",");
            //AOI.forEach(initializeAOI);
            $('#EditareaInterest1 input:checked').removeAttr('checked');
            $("#EditareaInterest1 input[type=checkbox]").each(function () {
                $(this).prop("checked", false);

                //console.log("aa")
            });
            //int x;



            for (let x = 0; x < AOInterest?.length; x++) {
                aointerestVal = AOInterest[x];
                if (aointerestVal == 1) {
                    $('#Editcheckbox1').prop('checked', true);
                } else if (aointerestVal == 2) {
                    $('#Editcheckbox2').prop('checked', true);
                }
                else if (aointerestVal == 3) {
                    $('#Editcheckbox3').prop('checked', true);
                }
                else if (aointerestVal == 4) {
                    $('#Editcheckbox4').prop('checked', true);
                }
                else if (aointerestVal == 5) {
                    $('#Editcheckbox5').prop('checked', true);
                }
                else if (aointerestVal == 6) {
                    $('#Editcheckbox6').prop('checked', true);
                }

            }

            $('#EditareaInterest1').change(function () {
                var data = []; // declare array

                $('#EditareaInterest1 input[type=checkbox]').each(function () { //loop every checkbox
                    if (this.checked) { // if checked
                        /* console.log("inEdit");*/
                        data.push($(this).val()); //push data to array
                        console.log(data)
                        //join array into hiddent text
                        //breakOut = true;
                        //return false;
                    }

                    //else {
                    //    data1.push($(this).val());
                    //}

                });



                if (data.length != 0) {
                    $('#AddEditVolunteer_VoluntaryDetails_VolunteeringArea').val(data.join());
                }


            });



            $("#view-PD-medical_declare").val(result.medicalDeclaration);

            if (result.medicalDeclaration !== "") {
                var data = result.medicalDeclaration;

                $('#editMedicalDeclare input[type=radio]').each(function () {


                    //if (($(this).val() == data)) {
                    //    this.checked = true;

                    if (data == "No") {
                        this.checked = true;
                        $('#YesMedical').prop('checked', false);
                    }
                    else {
                        $('#YesMedical').prop('checked', true);
                        if (document.getElementById("YesMedical").checked = true) {
                            document.getElementById("ValueYesMediacal").style.visibility = 'visible';

                            $("#ValueYesMediacal").val(data);
                        }
                    }

                    /* }*/


                });


            }

            $('#editMedicalDeclare').change(function () {
                var data = []; // declare array
                var data1 = [];
                $('#editMedicalDeclare input[type=radio]').each(function () { //loop every checkbox
                    if (this.checked) { // if checked
                        //    data.push($(this).val()); //push data to array
                        //}
                        if ($(this).attr("id") == "YesMedical") {
                            //  data.push($(this).val());
                            //data1.push(document.getElementById("other_q1").value);
                            //document.getElementById("VolunteerExit_otherPositiveExp").value = document.getElementById("other_q1").value;
                            //$('#VolunteerExit_otherPositiveExp').val($("other_q1").val());
                            data1.push(document.getElementById("ValueYesMediacal").value);
                        } else {
                            data1 = "";
                            breakOut = true;
                            return false;
                        }
                    }

                });
                if (data1 == "") {
                    $('#AddEditVolunteer_MedicalDeclare_yes_question1').val(data1.join()); //join array into hiddent text
                }

                //$('#VolunteerExit_otherImprove').val(data1.join());
            });

            $("#view-PD-personal_q1").append(result.personalDeclaration_q1);


            if (result.personalDeclaration_q1 !== "") {
                var data = result.personalDeclaration_q1;

                $('#EditQ1 input[type=radio]').each(function () {
                    if (data == "No") {
                        this.checked = true;
                        $('#YesQ1').prop('checked', false);
                    }
                    else {
                        $('#YesQ1').prop('checked', true);
                        if (document.getElementById("YesQ1").checked = true) {
                            document.getElementById("ValueQ1").style.visibility = 'visible';

                            $("#ValueQ1").val(data);
                        }
                    }


                });

                //if (document.getElementById("YesQ1").checked = true) {
                //    document.getElementById("ValueQ1").style.visibility = 'visible';

                //    $("#ValueQ1").val(data);
                //}
            }
            $("#view-PD-personal_q2").append(result.personalDeclaration_q2);

            if (result.personalDeclaration_q2 !== "") {
                var data = result.personalDeclaration_q2;

                $('#EditQ2 input[type=radio]').each(function () {
                    if (data == "No") {
                        this.checked = true;
                        $('#YesQ2').prop('checked', false);
                    }
                    else {
                        $('#YesQ2').prop('checked', true);
                        if (document.getElementById("YesQ2").checked = true) {
                            document.getElementById("ValueQ2").style.visibility = 'visible';

                            $("#ValueQ2").val(data);
                        }
                    }

                });


            }
            $("#view-PD-personal_q3").append(result.personalDeclaration_q3);
            if (result.personalDeclaration_q3 !== "") {
                var data = result.personalDeclaration_q3;

                $('#EditQ3 input[type=radio]').each(function () {
                    if (data == "No") {
                        this.checked = true;
                        $('#YesQ3').prop('checked', false);
                    }
                    else {
                        $('#YesQ3').prop('checked', true);
                        if (document.getElementById("YesQ3").checked = true) {
                            document.getElementById("ValueQ3").style.visibility = 'visible';

                            $("#ValueQ3").val(data);
                        }
                    }

                });


            }


            const EditmonAval = result.voluntaryDetail.monAvailability?.split(",");
            const EdittueAval = result.voluntaryDetail.tueAvailability?.split(",");
            const EditwedAval = result.voluntaryDetail.wedAvailability?.split(",");
            const EditthuAval = result.voluntaryDetail.thuAvailability?.split(",");
            const EditfriAval = result.voluntaryDetail.friAvailability?.split(",");
            const EditsatAval = result.voluntaryDetail.satAvailability?.split(",");
            const EditsunAval = result.voluntaryDetail.sunAvailability?.split(",");

            $('#Edittime-availability').change(function () {

                var monday = [];
                var tuesday = [];
                var wednesday = [];
                var thursday = [];
                var friday = [];
                var saturday = [];
                var sunday = [];
                $('#Edittime-availability tbody input[type=checkbox]').each(function () {
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


            for (let x = 0; x < EditmonAval?.length; x++) {
                EditavailVal = EditmonAval[x]
                if (EditavailVal == 1) {
                    $('#EditmonMorn').prop('checked', true);
                }
                if (EditavailVal == 2) {
                    $('#EditmonAft').prop('checked', true);
                }
                if (EditavailVal == 3) {
                    $('#EditmonEve').prop('checked', true);
                }
            }

            for (let x = 0; x < EdittueAval?.length; x++) {
                EditavailVal = EdittueAval[x]
                if (EditavailVal == 1) {
                    $('#EdittueMorn').prop('checked', true);
                }
                if (EditavailVal == 2) {
                    $('#EdittueAft').prop('checked', true);
                }
                if (EditavailVal == 3) {
                    $('#EdittueEve').prop('checked', true);
                }
            }

            for (let x = 0; x < EditwedAval?.length; x++) {
                EditavailVal = EditwedAval[x]
                if (EditavailVal == 1) {
                    $('#EditwedMorn').prop('checked', true);
                }
                if (EditavailVal == 2) {
                    $('#EditwedAft').prop('checked', true);
                }
                if (EditavailVal == 3) {
                    $('#EditwedEve').prop('checked', true);
                }
            }

            for (let x = 0; x < EditthuAval?.length; x++) {
                EditavailVal = EditthuAval[x]
                if (EditavailVal == 1) {
                    $('#EditthuMorn').prop('checked', true);
                }
                if (EditavailVal == 2) {
                    $('#EditthuAft').prop('checked', true);
                }
                if (EditavailVal == 3) {
                    $('#EditthuEve').prop('checked', true);
                }
            }

            for (let x = 0; x < EditfriAval?.length; x++) {
                EditavailVal = EditfriAval[x]
                if (EditavailVal == 1) {
                    $('#EditfriMorn').prop('checked', true);
                }
                if (EditavailVal == 2) {
                    $('#EditfriAft').prop('checked', true);
                }
                if (EditavailVal == 3) {
                    $('#EditfriEve').prop('checked', true);
                }
            }

            for (let x = 0; x < EditsatAval?.length; x++) {
                EditavailVal = EditsatAval[x]
                if (EditavailVal == 1) {
                    $('#EditsatMorn').prop('checked', true);
                }
                if (EditavailVal == 2) {
                    $('#EditsatAft').prop('checked', true);
                }
                if (EditavailVal == 3) {
                    $('#EditsatEve').prop('checked', true);
                }
            }

            for (let x = 0; x < EditsunAval?.length; x++) {
                EditavailVal = EditsunAval[x]
                if (EditavailVal == 1) {
                    $('#EditsunMorn').prop('checked', true);
                }
                if (EditavailVal == 2) {
                    $('#EditsunAft').prop('checked', true);
                }
                if (EditavailVal == 3) {
                    $('#EditsunEve').prop('checked', true);
                }
            }



            $("#Edit-PD-Employ").val(result.employmentStatus);

            console.log(result.employmentStatus);

            if (result.employmentStatus !== "" && result.employmentStatus !== null) {
                var data = result.employmentStatus;

                /*  $('#EditNationality input[type=radio]').each(function () {*/
                $('#EditEmployment input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        this.checked = true;

                        breakOut = true;
                        return false;


                    }


                    if ((data != "Full Time" && data != "Part Time" && data != "Self Employed" && data != "Part Time" && data != "Home Maker" && data != "Part Time" && data != "Retire")) {
                        document.getElementById("Employment_Others").checked = true;
                        document.getElementById("ValueotherEmployment").style.visibility = 'visible';

                        $("#ValueotherEmployment").val($("#ValueotherEmployment").val() + data);
                        //$('#AddEditVolunteer_PersonalDetails_OtherEmploymentStatus').val(data.join());
                        breakOut = true;
                        return false;
                    }

                });

            }

            $('#EditEmployment').change(function () {
                var data = []; // declare array
                var data1 = [];
                $('#EditEmployment input[type=radio]').each(function () { //loop every checkbox
                    if (this.checked) { // if checked
                        console.log("inEdit");
                        data.push($(this).val()); //push data to array
                        console.log(data)
                        //join array into hiddent text
                        breakOut = true;
                        return false;
                    }

                    //else {
                    //    data1.push($(this).val());
                    //}

                });
                if ($(this).attr("id") == "Employment_Others" || data == "Others") {
                    //  data.push($(this).val());
                    //data1.push(document.getElementById("other_q1").value);
                    //document.getElementById("VolunteerExit_otherPositiveExp").value = document.getElementById("other_q1").value;
                    //$('#VolunteerExit_otherPositiveExp').val($("other_q1").val());

                    data1.push(document.getElementById("ValueotherEmployment").value);
                    data = "";

                    if (data1.length != 0) {
                        //$('#ValueotherEmployment').val(data1.join());
                        //$("#ValueotherEmployment").val($("#ValueotherEmployment").val() + data1);
                        $('#AddEditVolunteer_PersonalDetails_OtherEmploymentStatus').val(data1.join());
                    }

                    breakOut = true;
                    return false;

                }
                console.log(data1)

                if (data.length != 0) {
                    $('#AddEditVolunteer_PersonalDetails_EmploymentStatus').val(data.join());
                }
                //if (data1.length!=0) {
                //    $('#AddEditVolunteer_PersonalDetails_EmploymentStatus').val(data1.join()); 

                //}
                //join array into hiddent text   //$('#VolunteerExit_otherImprove').val(data1.join());
            });

        }
    });







}
function showOtherNationality() {
    if (document.getElementById("EditOthersNationality").checked == true) {
        document.getElementById("ValueotherNationality").style.visibility = 'visible';
    }
    else {
        document.getElementById("ValueotherNationality").style.visibility = 'hidden';
    }
};


function showOtherRace() {
    if (document.getElementById("Race_Others").checked == true) {
        document.getElementById("ValueotherRace").style.visibility = 'visible';
    }
    else {
        document.getElementById("ValueotherRace").style.visibility = 'hidden';
    }
};

function showOtherEmployment() {
    if (document.getElementById("Employment_Others").checked == true) {
        document.getElementById("ValueotherEmployment").style.visibility = 'visible';
    }
    else {
        document.getElementById("ValueotherEmployment").style.visibility = 'hidden';
    }
};

function showOtherEducation() {
    if (document.getElementById("Education_Others").checked == true) {
        document.getElementById("ValueotherEducation").style.visibility = 'visible';
    }
    else {
        document.getElementById("ValueotherEducation").style.visibility = 'hidden';
    }
};
function ShowOtherQualification() {
    if (document.getElementById("OthersQualification").checked == true) {
        document.getElementById("other_Qualification").style.visibility = 'visible';
    }
    else {
        document.getElementById("other_Qualification").style.visibility = 'hidden';
    }
};

function ShowOtherLanguage() {
    if (document.getElementById("OthersLanguage").checked == true) {
        document.getElementById("other_Language").style.visibility = 'visible';
    }
    else {
        document.getElementById("other_Language").style.visibility = 'hidden';
    }
};

function ShowOtherGetKnownBy() {
    if (document.getElementById("OthersGetKnownBy").checked == true) {
        document.getElementById("other_GetKnownBy").style.visibility = 'visible';
    }
    else {
        document.getElementById("other_GetKnownBy").style.visibility = 'hidden';
    }
};

function showYesMedical() {
    if (document.getElementById("YesMedical").checked == true) {
        document.getElementById("ValueYesMediacal").style.visibility = 'visible';
    }
    else {
        document.getElementById("ValueYesMediacal").style.visibility = 'hidden';
    }
};
function PersonalDeclareQ1() {
    if (document.getElementById("YesQ1").checked == true) {
        document.getElementById("ValueQ1").style.visibility = 'visible';
    }
    else {
        document.getElementById("ValueQ1").style.visibility = 'hidden';
    }
};
function PersonalDeclareQ2() {
    if (document.getElementById("YesQ2").checked == true) {
        document.getElementById("ValueQ2").style.visibility = 'visible';
    }
    else {
        document.getElementById("ValueQ2").style.visibility = 'hidden';
    }
};
function PersonalDeclareQ3() {
    if (document.getElementById("YesQ3").checked == true) {
        document.getElementById("ValueQ3").style.visibility = 'visible';
    }
    else {
        document.getElementById("ValueQ3").style.visibility = 'hidden';
    }
};
//function rdClick(name) {
//    if (name == "Nationality") {
//        Val = $("input[name='AddEditVolunteer.PersonalDetails.Nationality']:checked").val();
//        var txt = document.getElementById("EditOthersNationality");
//        if (Val == "Others") {
//            txt.style = "display:block;margin-top:10px";
//        }
//        else {
//            txt.style = "display:none;";
//        }
//    }
//}


//function delete_Volunteer(userid) {
//    Swal.fire({
//        title: 'Do you want to delete this volunteer?',
//        showCancelButton: true,
//        confirmButtonText: 'Confirm',
//    }).then((result) => {
//        /* Read more about isConfirmed, isDenied below */
//        if (result.isConfirmed) {
//            $.ajax({
//                url: 'localhost:44350/api/Staff/DeleteUserDetailById?UserId=' + userid,
//                type: 'POST',
//                success: function (result) {
//                    let timerInterval
//                    Swal.fire({
//                        icon: 'success',
//                        title: result.data.message,
//                        timer: 1500,
//                        timerProgressBar: true,
//                        didOpen: () => {
//                            Swal.showLoading()
//                            const b = Swal.getHtmlContainer().querySelector('b')
//                            timerInterval = setInterval(() => {
//                                b.textContent = Swal.getTimerLeft()
//                            }, 100)
//                        },
//                        willClose: () => {
//                            clearInterval(timerInterval)
//                        }
//                    }).then((result) => {
//                        if (result.dismiss === Swal.DismissReason.timer) {

//                            window.location.reload();
//                        }
//                    })
//                }
//            });
//        }
//    })
//}

function formatDataDate(date) {
    var d = date;
    if (d.getDate() < 10) { var DATE = '0' + d.getDate(); } else { var DATE = d.getDate(); }
    if (d.getMonth() < 10) { var MONTH = '0' + (d.getMonth() + 1); } else { var MONTH = (d.getMonth() + 1); }
    var hours = d.getHours()
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours < 10 ? '0' + hours : hours;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    var dformat = [DATE,
        MONTH,
        d.getFullYear()].join('/') + ' ' +
        [hours,
            d.getMinutes() < 10 ? '0' + d.getMinutes() : d.getMinutes()].join(':') + ' ' + ampm;
    return dformat;
}

function open_VolunteerJourney(userid) {



    $("#ViewVolunteerJourneyModal").modal("show");

    $.ajax({
        url: '/VolunteerManagement/GetPastEventsByUserId?UserId=' + userid,
        type: 'POST',
        success: function (result) {
            console.log(result);
            $("#past-activites-table").DataTable(
                {
                    autoWidth: false,
                    bLengthChange: true,
                    lengthMenu: [[5, 10, -1], [5, 10, "All"]],
                    bFilter: true,
                    bSort: true,
                    bPaginate: true,
                    data: result,
                    columns: [{ 'data': 'EventName' },
                    { 'data': 'EventModule' },
                        {
                            'data': 'EventDate',
                            'render': function (data, type, row) {
                                return moment(data).format("DD MMM YYYY");
                            }
                        }]
                    
                })




        }
    });
    

    $.ajax({
        url: '/VolunteerManagement/GetFutureEventsByUserId?UserId=' + userid,
        type: 'POST',
        success: function (result) {
            console.log(result);
            $("#future-activites-table").DataTable(
                {
                    autoWidth: false,
                    bLengthChange: true,
                    lengthMenu: [[5, 10, -1], [5, 10, "All"]],
                    bFilter: true,
                    bSort: true,
                    bPaginate: true,
                    data: result,
                    columns: [{ 'data': 'EventName' },
                    { 'data': 'EventModule' },
                        {
                            'data': 'EventDate',
                            'render': function (data, type, row) {
                                return moment(data).format("DD MMM YYYY");
                            }
                        }]

                })




        }
    });


    $.ajax({
        url: '/VolunteerManagement/GetVolunteerDetailsById?UserId=' + userid,
        type: 'POST',
        success: function (result) {
            console.log(result);
            //$("#ExitForm-name").empty();
            //$("#ExitForm-Mobile").empty();
            //$("#ExitForm-Email").empty();
            //$("#ExitForm-UserID").empty();
            //console.log(result);
            $("#ExitForm-name").val(result.fullname);
            $("#ExitForm-Mobile").val(result.mobile);
            $("#ExitForm-Email").val(result.email);
            $("#ExitForm-UserID").append(result.userId);
            $("#view-PD-status").append(result.status);
            $("#view-PD-type").append(result.type);

            //if (result.Role == 4) {
            //    document.querySelector('#view-PD-role').textContent = 'Pre-Register Volunteer'
            //} else if (result.Role == 3) {
            //    document.querySelector('#view-PD-role').textContent = 'CBP Volunteer'
            //} else if (result.Role == 5) {
            //    document.querySelector('#view-PD-role').textContent = 'Volunteer Admin'
            //} else if (result.Role == 6) {
            //    document.querySelector('#view-PD-role').textContent = 'Volunteer'
            //} else if (result.Role == 51) {
            //    document.querySelector('#view-PD-role').textContent = 'CBP Volunteer Leader'
            //}

            document.querySelector('#view-PD-role').textContent = result.roles;




        }
    });

    var volunteerid = userid
   
    $('#assign-evt-btn').click(() => {
        var eventid = parseInt($('#AssignEventModel_Type option:selected').val())
        addEventPtcp(eventid, volunteerid)
    })



    $('#ViewVolunteerJourneyModal').on('hidden.bs.modal', function () {
        location.reload();
    })

    $('#update-volunteer-journey-btn').click(() => {
        console.log(userid)
        updateVolunteerJourney(userid)
    })
}

function addEventPtcp(eventid,userid) {
    $.ajax({
        url: '/VolunteerManagement/AddEditEventParticipants?EventId=' + eventid + '&UserId=' + userid,
        type: 'GET',
        success: function (result) {
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
                        window.location.reload();
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
            //console.log(result)




        }
    });
}



function OnSuccessForm(result) {
    //console.log(result.data.message)
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
                window.location.reload();
            }
        })

    } else {

        Swal.fire({
            icon: 'error',
            title: 'Error',
            /*            text: result.data.message*/
            text: "The user has submitted the form before"

        })
        return;
    }

}

/*CreateVolunteerExit*/
function CreateExitForm() {

    var userID = $('#ExitForm-UserID').text();
    //document.getElementById('boxExitForm').style.display = "block";
    console.log(userID)

    $.ajax({
        url: '/VolunteerManagement/ViewExitFormByID?userID=' + userID,
        type: 'POST',
        data: { id: userID },

        success: function (result) {

            if (result.dateCreated == null) {
                document.getElementById('boxExitForm').style.display = "block";
                if (document.getElementById('boxExitForm').style.display = "block") {

                    document.getElementById('ViewExitForm').style.display = "none";
                    document.getElementById('ViewExitForm').style.display = "none";
                }
            } else {
                alert("You are already submited the form");
                document.getElementById('boxExitForm').style.display = "none";
            }




        }


    });


}
function OnSuccess(result) {
    //console.log(result.data.message)
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
                window.location.reload();
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
    //console.log(result);
}

//$("#btnEditSubmit").on("click", function () {   

function ViewExitForm() {
    var userID = $('#ExitForm-UserID').text();
    /* var userID = "";*/
    console.log(userID);

    //document.getElementById('ViewExitForm').style.display = "block";


    //document.getElementById("ViewExitForm").style.visibility = 'visible';

    $.ajax({
        url: '/VolunteerManagement/ViewExitFormByID?userID=' + userID,
        type: 'POST',
        data: { id: userID },

        success: function (result) {

            if (result.dateCreated != null) {
                document.getElementById('ViewExitForm').style.display = "block";
                if (document.getElementById('ViewExitForm').style.display = "block") {

                    document.getElementById('boxExitForm').style.display = "none";
                    document.getElementById('EditExitForm').style.display = "none";
                }
            } else {
                document.getElementById('ViewExitForm').style.display = "none";
                alert("Not Record Found");
            }





            $("#ViewExitForm-name").val(result.name);
            $("#ViewExitForm-Mobile").val(result.contactNumber);
            $("#ViewExitForm-Email").val(result.emailAddress);
            $("#ViewExitForm-Role").val(result.role);
            //var newDate = Date(parseInt(result.startVolunteerDate.replace('/Date(', '')));
            //var newDate = Date(result.startVolunteerDate.match(/\d+/)[0] * 1);
            $("#ViewExitForm-StartDate").val(result.startVolunteerDateString);
            $("#ViewExitForm-EndDate").val(result.exitInterViewDateString);




            if (result.areaOfPositiveExp !== "") {

                var data = result.areaOfPositiveExp.split(",");

                $('#ViewareaPositiveExp input[type=checkbox]').each(function () {
                    if ($.inArray($(this).val(), data) > -1) {
                        this.checked = true;
                    }
                });
            }



            if (result.otherPositiveExp !== "") {
                var data = result.otherPositiveExp;

                if (document.getElementById("Viewotherq1").checked == true) {
                    document.getElementById("Viewother_q1").style.visibility = 'visible';
                    $("#Viewother_q1").val($("#Viewother_q1").val() + data);
                }

            }

            if (result.areaOfNegativeExp !== "") {
                var data = result.areaOfNegativeExp.split(",");

                $('#ViewareaNegativeExp input[type=checkbox]').each(function () {
                    if ($.inArray($(this).val(), data) > -1) {
                        this.checked = true;
                    }
                });
            }
            if (result.otherNegativeExp !== "") {
                var data = result.otherNegativeExp;

                if (document.getElementById("Viewotherq2").checked == true) {
                    document.getElementById("Viewother_q2").style.visibility = 'visible';
                    $("#Viewother_q2").val($("#Viewother_q2").val() + data);
                }

            }


            if (result.areaReasonLeavingAgency !== "") {
                var data = result.areaReasonLeavingAgency.split(",");

                $('#AreaReasonLeavingAgency input[type=checkbox]').each(function () {
                    if ($.inArray($(this).val(), data) > -1) {
                        this.checked = true;
                    }
                });
            }
            if (result.otherReasonLeavingAgency !== "") {
                var data = result.otherReasonLeavingAgency;

                if (document.getElementById("ViewReason-checkbox8").checked == true) {
                    document.getElementById("Viewother-reason").style.visibility = 'visible';
                    $("#Viewother-reason").val($("#Viewother-reason").val() + data);
                }

            }

            if (result.areaImprove !== "") {
                var data = result.areaImprove.split(",");

                $('#AreaImproveExp input[type=checkbox]').each(function () {
                    if ($.inArray($(this).val(), data) > -1) {
                        this.checked = true;
                    }
                });
            }
            if (result.otherImprove !== "") {
                var data = result.otherImprove;

                if (document.getElementById("ViewExperience-checkbox6").checked == true) {
                    document.getElementById("ViewOther-experience").style.visibility = 'visible';
                    $("#ViewOther-experience").val($("#ViewOther-experience").val() + data);
                }

            }

            if (result.yesNoInterestDesc !== "") {
                var data = result.yesNoInterestDesc;

                $('#ViewinterestedYesNo input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        /* if ($.inArray($(this).val() == "Yes")) {*/
                        this.checked = true;
                    }
                    //if ($.inArray($(this).val() == "No")) {
                    //    this.checked = true;
                    //}
                });
            }
            if (result.yesDesc !== "") {
                var data = result.yesDesc;

                if (document.getElementById("Viewinterest-radio1").checked == true) {
                    document.getElementById("Viewother-interest1").style.visibility = 'visible';
                    $("#Viewother-interest1").val($("#Viewother-interest1").val() + data);
                }

            }
            if (result.noDesc !== "") {
                var data = result.noDesc;

                if (document.getElementById("Viewinterest-radio2").checked == true) {
                    document.getElementById("Viewother-interest").style.visibility = 'visible';
                    $("#Viewother-interest").val($("#Viewother-interest").val() + data);
                }

            }



            if (result.areaAppropriate !== "") {
                var data = result.areaAppropriate;

                $('#ViewareaAppropriateTraining input[type=radio]').each(function () {


                    if (($(this).val() == data)) {
                        /*if ($.inArray($(this).val() == "Yes")) {*/
                        this.checked = true;
                    }
                    //if ($.inArray($(this).val() == "No") ){
                    //    this.checked = true;
                    //}

                });
            }
            if (result.noAppropriate !== "") {
                var data = result.noAppropriate;

                if (document.getElementById("Viewtraining-radio2").checked == true) {
                    document.getElementById("ViewradioNoTraining1").style.visibility = 'visible';
                    $("#ViewradioNoTraining1").val($("#ViewradioNoTraining1").val() + data);
                }

            }


            if (result.areaAppropriateRes !== "") {
                var data = result.areaAppropriateRes;

                $('#ViewareaAppropriateResources input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        //if ($.inArray($(this).val() == "Yes")) {
                        this.checked = true;
                    }
                    //if ($.inArray($(this).val() == "No")) {
                    //    this.checked = true;
                    //}
                });
            }
            if (result.noAppropriateRes !== "") {
                var data = result.noAppropriateRes;

                if (document.getElementById("Viewresources-radio2").checked == true) {
                    document.getElementById("ViewradioNoResources").style.visibility = 'visible';
                    $("#ViewradioNoResources").val($("#ViewradioNoResources").val() + data);
                }

            }


            if (result.areaFeelSupervision !== "") {
                var data = result.areaFeelSupervision;

                $('#ViewareaFeelSupervision input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        //if ($.inArray($(this).val() == "Yes")) {
                        this.checked = true;
                    }
                    //if ($.inArray($(this).val() == "No")) {
                    //    this.checked = true;
                    //}
                });
            }
            if (result.noFeelSupervision !== "") {
                var data = result.noFeelSupervision;

                if (document.getElementById("Viewfeel-radio2").checked == true) {
                    document.getElementById("ViewradioNoFeel").style.visibility = 'visible';
                    $("#ViewradioNoFeel").val($("#ViewradioNoFeel").val() + data);
                }

            }

            if (result.areaFeelPositiveImpact !== "") {
                var data = result.areaFeelPositiveImpact;

                $('#ViewareaFeelPositiveImpact input[type=radio]').each(function () {
                    //if ($.inArray($(this).val() == "Yes")) {
                    if (($(this).val() == data)) {
                        this.checked = true;
                    }
                    //if ($.inArray($(this).val() == "No")) {
                    //    this.checked = true;
                    //}
                });
            }
            if (result.noFeelPositiveImpact !== "") {
                var data = result.noFeelPositiveImpact;

                if (document.getElementById("Viewimpact-radio2").checked == true) {
                    document.getElementById("ViewradioNoImpact").style.visibility = 'visible';
                    $("#ViewradioNoImpact").val($("#ViewradioNoImpact").val() + data);
                }

            }


            if (result.areaRecommend !== "") {
                var data = result.areaRecommend;

                $('#ViewareaRecommend input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        //if ($.inArray($(this).val() == "Yes")) {
                        this.checked = true;
                    }
                    //if ($.inArray($(this).val() == "No")) {
                    //    this.checked = true;
                    //}
                });
            }
            if (result.noRecommend !== "") {
                var data = result.noRecommend;

                if (document.getElementById("Viewrecommend-radio2").checked == true) {
                    document.getElementById("ViewradioNoRecommend").style.visibility = 'visible';
                    $("#ViewradioNoRecommend").val($("#ViewradioNoRecommend").val() + data);
                }

            }

        }


    });
}

function EditExitForm() {
    var userID = $('#ExitForm-UserID').text();
    /* var userID = "";*/
    console.log(userID);

    //document.getElementById('EditExitForm').style.display = "block";

    $.ajax({
        url: '/VolunteerManagement/ViewExitFormByID?userID=' + userID,
        type: 'POST',
        data: { id: userID },

        success: function (result) {
            if (result.dateCreated != null) {
                document.getElementById('EditExitForm').style.display = "block";
                if (document.getElementById('EditExitForm').style.display = "block") {

                    document.getElementById('boxExitForm').style.display = "none";
                    document.getElementById('ViewExitForm').style.display = "none";
                }
            } else {
                document.getElementById('EditExitForm').style.display = "none";
                alert("Not Record Found");
            }




            //$("#VolunteerExit_name").append(result.Name);
            //$("#EditExitForm-CreatedTime").val(result.UserId);
            $("#EditExitForm-UserID").val(result.userId);
            $("#EditExitForm-name").val(result.name);
            //$("#EditExitForm-name").append(result.Name);
            $("#EditExitForm-Mobile").val(result.contactNumber);
            $("#EditExitForm-Email").val(result.emailAddress);

            //$("#EditExitForm-Role").val($("#EditExitForm-Role").val() + result.Role);

            $("#EditExitForm-Role").val(result.role);

            $('#EditExitForm-StartDate').datepicker({
                startView: 2,
                format: 'dd/mm/yyyy'

            });
            $("#EditExitForm-StartDate").val(result.startVolunteerDateString);

            $('#EditExitForm-EndDate').datepicker({
                startView: 2,
                format: 'dd/mm/yyyy'

            });
            $("#EditExitForm-EndDate").val(result.exitInterViewDateString);

            $("#EditExitForm-DateCreated").val(result.dateCreatedString);


            //set value into checkBox

            if (result.areaOfPositiveExp !== "") {
                var data = result.areaOfPositiveExp.split(",");

                $('#EditareaPositiveExp input[type=checkbox]').each(function () {
                    if ($.inArray($(this).val(), data) > -1) {
                        this.checked = true;
                    }
                });
            }

            if (result.otherPositiveExp !== "") {
                var data = result.otherPositiveExp;

                if (document.getElementById("Editotherq1").checked == true) {
                    document.getElementById("Editother_q1").style.visibility = 'visible';
                    $("#Editother_q1").val($("#Editother_q1").val() + data);
                }
                if (document.getElementById("Editotherq1").checked == false) {
                    document.getElementById("Editother_q1").style.visibility = 'hidden';
                }

            }



            if (result.areaOfNegativeExp !== "") {
                var data = result.areaOfNegativeExp.split(",");

                $('#EditareaNegativeExp input[type=checkbox]').each(function () {
                    if ($.inArray($(this).val(), data) > -1) {
                        this.checked = true;
                    }
                });
            }
            if (result.otherNegativeExp !== "") {
                var data = result.otherNegativeExp;

                if (document.getElementById("Editotherq2").checked == true) {
                    document.getElementById("Editother_q2").style.visibility = 'visible';
                    $("#Editother_q2").val($("#Editother_q2").val() + data);
                }

                if (document.getElementById("Editotherq2").checked == false) {
                    document.getElementById("Editother_q2").style.visibility = 'hidden';
                }

            }


            if (result.areaReasonLeavingAgency !== "") {
                var data = result.areaReasonLeavingAgency.split(",");

                $('#EditAreaReasonLeavingAgency input[type=checkbox]').each(function () {
                    if ($.inArray($(this).val(), data) > -1) {
                        this.checked = true;
                    }
                });
            }
            if (result.otherReasonLeavingAgency !== "") {
                var data = result.otherReasonLeavingAgency;

                if (document.getElementById("Editreason-checkbox8").checked == true) {
                    document.getElementById("Editother-reason").style.visibility = 'visible';
                    $("#Editother-reason").val($("#Editother-reason").val() + data);
                }

            }

            if (result.areaImprove !== "") {
                var data = result.areaImprove.split(",");

                $('#EditAreaImproveExp input[type=checkbox]').each(function () {
                    if ($.inArray($(this).val(), data) > -1) {
                        this.checked = true;
                    }
                });
            }
            if (result.otherImprove !== "") {
                var data = result.otherImprove;

                if (document.getElementById("EditExperience-checkbox6").checked == true) {
                    document.getElementById("EditOther-experience").style.visibility = 'visible';
                    $("#EditOther-experience").val($("#EditOther-experience").val() + data);
                }

            }

            if (result.yesNoInterestDesc !== "") {
                var data = result.yesNoInterestDesc;

                $('#EditinterestedYesNo input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        this.checked = true;
                    }

                });
            }
            if (result.yesDesc !== "") {
                var data = result.yesDesc;

                if (document.getElementById("Editinterest-radio1").checked == true) {
                    document.getElementById("Editother-interest1").style.visibility = 'visible';
                    $("#Editother-interest1").val($("#Editother-interest1").val() + data);
                }

            }
            if (result.noDesc !== "" && result.noDesc !== null) {
                var data = result.noDesc;

                if (document.getElementById("Editinterest-radio2").checked == true) {
                    document.getElementById("Editother-interest").style.visibility = 'visible';
                    $("#Editother-interest").val($("#Editother-interest").val() + data);
                }

            }



            if (result.areaAppropriate !== "") {
                var data = result.areaAppropriate;

                $('#EditareaAppropriateTraining input[type=radio]').each(function () {


                    if (($(this).val() == data)) {
                        this.checked = true;
                    }


                });
            }
            if (result.noAppropriate !== "") {
                var data = result.noAppropriate;

                if (document.getElementById("Edittraining-radio2").checked == true) {
                    document.getElementById("EditradioNoTraining1").style.visibility = 'visible';
                    $("#EditradioNoTraining1").val($("#EditradioNoTraining1").val() + data);
                }

            }


            if (result.areaAppropriateRes !== "") {
                var data = result.areaAppropriateRes;

                $('#EditareaAppropriateResources input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        this.checked = true;
                    }

                });
            }
            if (result.noAppropriateRes !== "") {
                var data = result.noAppropriateRes;

                if (document.getElementById("Editresources-radio2").checked == true) {
                    document.getElementById("EditradioNoResources").style.visibility = 'visible';
                    $("#EditradioNoResources").val($("#EditradioNoResources").val() + data);
                }

            }


            if (result.areaFeelSupervision !== "") {
                var data = result.areaFeelSupervision;

                $('#EditareaFeelSupervision input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        this.checked = true;
                    }

                });
            }
            if (result.noFeelSupervision !== "") {
                var data = result.noFeelSupervision;

                if (document.getElementById("Editfeel-radio2").checked == true) {
                    document.getElementById("EditradioNoFeel").style.visibility = 'visible';
                    $("#EditradioNoFeel").val($("#EditradioNoFeel").val() + data);
                }

            }

            if (result.areaFeelPositiveImpact !== "") {
                var data = result.areaFeelPositiveImpact;

                $('#EditareaFeelPositiveImpact input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        this.checked = true;
                    }

                });
            }
            if (result.noFeelPositiveImpact !== "") {
                var data = result.noFeelPositiveImpact;

                if (document.getElementById("Editimpact-radio2").checked == true) {
                    document.getElementById("EditradioNoImpact").style.visibility = 'visible';
                    $("#EditradioNoImpact").val($("#EditradioNoImpact").val() + data);
                }

            }


            if (result.areaRecommend !== "") {
                var data = result.areaRecommend;

                $('#EditareaRecommend input[type=radio]').each(function () {
                    if (($(this).val() == data)) {
                        this.checked = true;
                    }

                });
            }
            if (result.noRecommend !== "") {
                var data = result.noRecommend;

                if (document.getElementById("Editrecommend-radio2").checked == true) {
                    document.getElementById("EditradioNoRecommend").style.visibility = 'visible';
                    $("#EditradioNoRecommend").val($("#EditradioNoRecommend").val() + data);
                }

            }
        }

    });

}

/*View Volunteer Exit*/
//function ShowViewOtherExp() {
//    if (document.getElementById("Viewotherq1").checked == true) {
//        document.getElementById("Viewother_q1").style.visibility = 'visible';
//    }
//    else {
//        document.getElementById("Viewother_q1").style.visibility = 'hidden';
//    }
//};

//function ShowViewOtherQ2() {
//    if (document.getElementById("Viewotherq2").checked == true) {
//        document.getElementById("Viewother_q2").style.visibility = 'visible';
//    }
//    else {
//        document.getElementById("Viewother_q2").style.visibility = 'hidden';
//    }
//};
//function ShowViewOtherReason() {
//    if (document.getElementById("Viewreason-checkbox8").checked == true) {
//        document.getElementById("Viewother-reason").style.visibility = 'visible';
//    }
//    else {
//        document.getElementById("Viewother-reason").style.visibility = 'hidden';
//    }
//};
//function ShowViewOtherExperience() {
//    if (document.getElementById("ViewExperience-checkbox6").checked == true) {
//        document.getElementById("ViewOther-experience").style.visibility = 'visible';
//    }
//    else {
//        document.getElementById("ViewOther-experience").style.visibility = 'hidden';
//    }
//};
//function ShowViewOtherInterest() {

//    if (document.getElementById("Viewinterest-radio1").checked == true && document.getElementById("Viewinterest-radio2").checked == false) {
//        document.getElementById("Viewother-interest1").style.visibility = 'visible';
//        document.getElementById("Viewother-interest").style.visibility = 'hidden';
//    }
//    if (document.getElementById("Viewinterest-radio1").checked == false && document.getElementById("Viewinterest-radio2").checked == true) {
//        document.getElementById("Viewother-interest").style.visibility = 'visible';
//        document.getElementById("Viewother-interest1").style.visibility = 'hidden';
//    }
//};

//function ViewShowNoTraining() {

//    if (document.getElementById("Viewtraining-radio2").checked == true && document.getElementById("Viewtraining-radio1").checked == false) {
//        document.getElementById("ViewradioNoTraining1").style.visibility = 'visible';

//    }
//    else {

//        document.getElementById("ViewradioNoTraining1").style.visibility = 'hidden';
//    }


//};
//function ShowViewNoResources() {

//    if (document.getElementById("Viewresources-radio2").checked == true && document.getElementById("Viewresources-radio1").checked == false) {
//        document.getElementById("ViewradioNoResources").style.visibility = 'visible';
//    }

//    else {
//        document.getElementById("ViewradioNoResources").style.visibility = 'hidden';
//    }
//};
//function ShowViewNoFeel() {

//    if (document.getElementById("Viewfeel-radio2").checked == true && document.getElementById("Viewfeel-radio1").checked == false) {
//        document.getElementById("ViewradioNoFeel").style.visibility = 'visible';
//    }

//    else {
//        document.getElementById("ViewradioNoFeel").style.visibility = 'hidden';
//    }
//};
//function ShowViewNoImpact() {

//    if (document.getElementById("Viewimpact-radio2").checked == true && document.getElementById("Viewimpact-radio1").checked == false) {
//        document.getElementById("ViewradioNoImpact").style.visibility = 'visible';
//    }

//    else {
//        document.getElementById("ViewradioNoImpact").style.visibility = 'hidden';
//    }
//};
//function ShowViewNoRecommend() {

//    if (document.getElementById("Viewrecommend-radio2").checked == true && document.getElementById("Viewrecommend-radio1").checked == false) {
//        document.getElementById("ViewradioNoRecommend").style.visibility = 'visible';
//    }

//    else {
//        document.getElementById("ViewradioNoRecommend").style.visibility = 'hidden';
//    }
//};





/*EDIT Volunteer Edit*/
function ShowEditOtherExp() {
    if (document.getElementById("Editotherq1").checked == true) {
        document.getElementById("Editother_q1").style.visibility = 'visible';
    }
    else {
        document.getElementById("Editother_q1").style.visibility = 'hidden';
    }
};

function ShowEditOtherQ2() {
    if (document.getElementById("Editotherq2").checked == true) {
        document.getElementById("Editother_q2").style.visibility = 'visible';
    }
    else {
        document.getElementById("Editother_q2").style.visibility = 'hidden';
    }
};
function ShowEditOtherReason() {
    if (document.getElementById("Editreason-checkbox8").checked == true) {
        document.getElementById("Editother-reason").style.visibility = 'visible';
    }
    else {
        document.getElementById("Editother-reason").style.visibility = 'hidden';
    }
};
function ShowEditOtherExperience() {
    if (document.getElementById("EditExperience-checkbox6").checked == true) {
        document.getElementById("EditOther-experience").style.visibility = 'visible';
    }
    else {
        document.getElementById("EditOther-experience").style.visibility = 'hidden';
    }
};
function ShowEditOtherInterest() {

    if (document.getElementById("Editinterest-radio1").checked == true && document.getElementById("Editinterest-radio2").checked == false) {
        document.getElementById("Editother-interest1").style.visibility = 'visible';
        document.getElementById("Editother-interest").style.visibility = 'hidden';
    }
    if (document.getElementById("Editinterest-radio1").checked == false && document.getElementById("Editinterest-radio2").checked == true) {
        document.getElementById("Editother-interest").style.visibility = 'visible';
        document.getElementById("Editother-interest1").style.visibility = 'hidden';
    }
};

function EditShowNoTraining() {

    if (document.getElementById("Edittraining-radio2").checked == true && document.getElementById("Edittraining-radio1").checked == false) {
        document.getElementById("EditradioNoTraining1").style.visibility = 'visible';

    }
    else {

        document.getElementById("EditradioNoTraining1").style.visibility = 'hidden';
    }


};
function ShowEditNoResources() {

    if (document.getElementById("Editresources-radio2").checked == true && document.getElementById("Editresources-radio1").checked == false) {
        document.getElementById("EditradioNoResources").style.visibility = 'visible';
    }

    else {
        document.getElementById("EditradioNoResources").style.visibility = 'hidden';
    }
};
function ShowEditNoFeel() {

    if (document.getElementById("Editfeel-radio2").checked == true && document.getElementById("Editfeel-radio1").checked == false) {
        document.getElementById("EditradioNoFeel").style.visibility = 'visible';
    }

    else {
        document.getElementById("EditradioNoFeel").style.visibility = 'hidden';
    }
};
function ShowEditNoImpact() {

    if (document.getElementById("Editimpact-radio2").checked == true && document.getElementById("Editimpact-radio1").checked == false) {
        document.getElementById("EditradioNoImpact").style.visibility = 'visible';
    }

    else {
        document.getElementById("EditradioNoImpact").style.visibility = 'hidden';
    }
};
function ShowEditNoRecommend() {

    if (document.getElementById("Editrecommend-radio2").checked == true && document.getElementById("Editrecommend-radio1").checked == false) {
        document.getElementById("EditradioNoRecommend").style.visibility = 'visible';
    }

    else {
        document.getElementById("EditradioNoRecommend").style.visibility = 'hidden';
    }
};

$("#SubmitEditVolunteer").on("click", function () {

    var QualificationData = [];
    var LanguageData = [];
    var EditGetKnownByData = [];
    var EditareaInterestData = [];
    var Editmonday = [];
    var Edittuesday = [];
    var Editwednesday = [];
    var Editthursday = [];
    var Editfriday = [];
    var Editsaturday = [];
    var Editsunday = [];

    $('#EditQualification input[type=checkbox]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            QualificationData.push($(this).val()); //push data to array
        }
        //if ($(this).attr("id") == "OthersQualification") {

        //    data2.push(document.getElementById("other_Qualification").value);
        //}


    });
    $('#AddEditVolunteer_PersonalDetails_Qualification').val(QualificationData.join()); //join array into hiddent text

    $('#EditLanguage input[type=checkbox]').each(function () {
        if (this.checked) { // if checked
            LanguageData.push($(this).val()); //push data to array
        }
    });
    $('#AddEditVolunteer_PersonalDetails_Language').val(LanguageData.join());


    $('#EditGetKnownBy input[type=checkbox]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            EditGetKnownByData.push($(this).val()); //push data to array
        }


    });
    $('#AddEditVolunteer_VoluntaryDetails_GetKnownBy').val(EditGetKnownByData.join()); //join array into hiddent text

    $('#EditareaInterest1 input[type=checkbox]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            EditareaInterestData.push($(this).val()); //push data to array
        }


    });
    $('#AddEditVolunteer_VoluntaryDetails_VolunteeringArea').val(EditareaInterestData.join()); //join array into hiddent text


    $('#Edittime-availability tbody input[type=checkbox]').each(function () {
        if (this.checked) {
            if (this.className === 'a') {
                Editmonday.push($(this).val());
            }
            if (this.className === 'b') {
                Edittuesday.push($(this).val());
            }
            if (this.className === 'c') {
                Editwednesday.push($(this).val());
            }
            if (this.className === 'd') {
                Editthursday.push($(this).val());
            }
            if (this.className === 'e') {
                Editfriday.push($(this).val());
            }
            if (this.className === 'f') {
                Editsaturday.push($(this).val());
            }
            if (this.className === 'g') {
                Editsunday.push($(this).val());
            }

        }
    });
    $('#AddEditVolunteer_VoluntaryDetails_MonAvailability').val(Editmonday.join());
    $('#AddEditVolunteer_VoluntaryDetails_TueAvailability').val(Edittuesday.join());
    $('#AddEditVolunteer_VoluntaryDetails_WedAvailability').val(Editwednesday.join());
    $('#AddEditVolunteer_VoluntaryDetails_ThuAvailability').val(Editthursday.join());
    $('#AddEditVolunteer_VoluntaryDetails_FriAvailability').val(Editfriday.join());
    $('#AddEditVolunteer_VoluntaryDetails_SatAvailability').val(Editsaturday.join());
    $('#AddEditVolunteer_VoluntaryDetails_SunAvailability').val(Editsunday.join());


});






$("#btnSubmit").on("click", function () {

    // Combine Edited data from checkbox/radio
    var data1 = []; // declare array
    var data2 = [];
    var data3 = [];
    var data4 = [];
    var data5 = []; // declare array
    var data6 = [];
    var data7 = [];
    var data8 = [];
    var data9 = [];
    var data10 = [];
    var yesNo = [];
    var data11 = [];
    var yesNo1 = [];
    var data12 = [];
    var yesNo2 = [];
    var data13 = [];
    var yesNo3 = [];
    var data14 = [];
    var yesNo4 = [];
    var data15 = [];
    var yesNo5 = [];



    $('#EditareaPositiveExp input[type=checkbox]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            data1.push($(this).val()); //push data to array
        }
        if ($(this).attr("id") == "Editotherq1") {

            data2.push(document.getElementById("Editother_q1").value);
        }


    });
    $('#hiddenAreaOfPositiveExp').val(data1.join()); //join array into hiddent text
    $('#hiddenotherPositiveExp').val(data2.join());



    $('#EditareaNegativeExp input[type=checkbox]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            data3.push($(this).val()); //push data to array
        }
        if ($(this).attr("id") == "Editotherq2") {

            data4.push(document.getElementById("Editother_q2").value);
        }


    });

    $('#hiddenAreaOfNegativeExp').val(data3.join()); //join array into hiddent text
    $('#hiddenotherNegativeExp').val(data4.join());


    $('#EditAreaReasonLeavingAgency input[type=checkbox]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            data5.push($(this).val()); //push data to array
        }
        if ($(this).attr("id") == "Editreason-checkbox8") {

            data6.push(document.getElementById("Editother-reason").value);
        }

    });
    $('#hiddenAreaReasonLeavingAgency').val(data5.join()); //join array into hiddent text
    $('#hiddenotherReasonLeavingAgency').val(data6.join());


    $('#EditAreaImproveExp input[type=checkbox]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            data7.push($(this).val()); //push data to array
        }
        if ($(this).attr("id") == "EditExperience-checkbox6") {

            data8.push(document.getElementById("EditOther-experience").value);
        }


    });
    $('#hiddenAreaImprove').val(data7.join()); //join array into hiddent text
    $('#hiddenotherImprove').val(data8.join());


    $('#EditinterestedYesNo input[type=radio]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            if ($(this).attr("id") == "Editinterest-radio1") {

                data9.push(document.getElementById("Editother-interest1").value);
                yesNo.push(document.getElementById("Editinterest-radio1").value);

            }
            if ($(this).attr("id") == "Editinterest-radio2") {

                data10.push(document.getElementById("Editother-interest").value);
                yesNo.push(document.getElementById("Editinterest-radio2").value);

            }
        }


    });

    $('#hiddenyesDesc').val(data9.join()); //join array into hiddent text
    $('#hiddennoDesc').val(data10.join());
    $('#hiddenyesNoInterestDesc').val(yesNo.join());


    $('#EditareaAppropriateTraining input[type=radio]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            if ($(this).attr("id") == "Edittraining-radio1") {
                yesNo1.push(document.getElementById("Edittraining-radio1").value);
            }
            if ($(this).attr("id") == "Edittraining-radio2") {
                yesNo1.push(document.getElementById("Edittraining-radio2").value);

                data11.push(document.getElementById("EditradioNoTraining1").value);

            }
        }
    });
    $('#hiddenAreaAppropriate').val(yesNo1.join()); //join array into hiddent text
    $('#hiddennoAppropriate').val(data11.join());


    $('#EditareaAppropriateResources input[type=radio]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            if ($(this).attr("id") == "Editresources-radio1") {
                yesNo2.push(document.getElementById("Editresources-radio1").value);
            }
            if ($(this).attr("id") == "Editresources-radio2") {
                yesNo2.push(document.getElementById("Editresources-radio2").value);

                data12.push(document.getElementById("EditradioNoResources").value);

            }
        }

    });
    $('#hiddenAreaAppropriateRes').val(yesNo2.join()); //join array into hiddent text
    $('#hiddennoAppropriateRes').val(data12.join());




    $('#EditareaFeelSupervision input[type=radio]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            if ($(this).attr("id") == "Editfeel-radio1") {
                yesNo3.push(document.getElementById("Editfeel-radio1").value);
            }
            if ($(this).attr("id") == "Editfeel-radio2") {
                yesNo3.push(document.getElementById("Editfeel-radio2").value);

                data13.push(document.getElementById("EditradioNoFeel").value);

            }
        }

    });
    $('#hiddenAreaFeelSupervision').val(yesNo3.join()); //join array into hiddent text
    $('#hiddennoFeelSupervision').val(data13.join());


    $('#EditareaFeelPositiveImpact input[type=radio]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            if ($(this).attr("id") == "Editimpact-radio1") {
                yesNo4.push(document.getElementById("Editimpact-radio1").value);
            }
            if ($(this).attr("id") == "Editimpact-radio2") {
                yesNo4.push(document.getElementById("Editimpact-radio2").value);

                data14.push(document.getElementById("EditradioNoImpact").value);

            }
        }

    });
    $('#hiddenAreaFeelPositiveImpact').val(yesNo4.join()); //join array into hiddent text
    $('#hiddennoFeelPositiveImpact').val(data14.join());

    $('#EditareaRecommend input[type=radio]').each(function () { //loop every checkbox

        if (this.checked) { // if checked
            if ($(this).attr("id") == "Editrecommend-radio1") {
                yesNo5.push(document.getElementById("Editrecommend-radio1").value);
            }
            if ($(this).attr("id") == "Editrecommend-radio2") {
                yesNo5.push(document.getElementById("Editrecommend-radio2").value);

                data15.push(document.getElementById("EditradioNoRecommend").value);

            }
        }



    });
    $('#hiddenAreaRecommend').val(yesNo5.join()); //join array into hiddent text
    $('#hiddenNoRecommend').val(data15.join());

});



function open_AnnualFeedback(userid) {
    $("#ViewAnualFeedbackModal").modal("show");

    $.ajax({
        url: '/VolunteerManagement/GetVolunteerDetailsById?UserId=' + userid,
        type: 'POST',
        success: function (result) {
            console.log(result);
            $("#FormclientName").val(result.Fullname);
            $("#FormclientId").val(userid)

        }
    });
}
function calculate_age(dob) {
    var diff_ms = Date.now() - dob.getTime();
    var age_dt = new Date(diff_ms);

    return Math.abs(age_dt.getUTCFullYear() - 1970);
}



let dobArray = document.querySelectorAll('.dob-col')

dobArray.forEach(volunteer => {

    var dob = new Date(volunteer.textContent.trim().slice(0,-12).split('/').reverse())
    //var dob = new Date(volunteer.textContent);
    volunteer.textContent = calculate_age(dob).toString()
})

$('#EditVolunteerJourney_FirstContactDate').datepicker({
    useCurrent: true,
    format: 'dd MM yyyy',
    changeYear: true
})




function updateVolunteerJourney(userId) {

    //$('#EditVolunteerJourney_Id').rules("add", {
    //    number:true
    //})

    //$('#EditVolunteerJourney_Id').val(userId)


    $.ajax({
        url: '/VolunteerManagement/UpdateVolunteerJourney',
        data: JSON.stringify({
            "Id": userId,
            "Status": $('#EditVolunteerJourney_Status').val(),
            "Type": $('#EditVolunteerJourney_Type').val(),
            "FirstContactDate": $('#EditVolunteerJourney_FirstContactDate').val(),
            "Role": $('#EditVolunteerJourney_Role').val()
        }),
        type: 'POST',

        contentType: 'application/json; charset=utf-8',

        success: function (result) {
            let timerInterval
            console.log(result);
            if (result.success == true) {
                Swal.fire({
                    icon: 'success',
                    title: result.data.message,
                    timerProgressBar: true,
                    timer: 1500,
                    willClose: () => {
                        clearInterval(timerInterval)
                    }
                }).then((result) => {
                    if (result.dismiss === Swal.DismissReason.timer) {

                        window.location.reload();
                    }
                })
            } else {
                Swal.fire({
                    icon: 'error',
                    title: result.data.message,
                    timerProgressBar: true,
                    timer: 1500,
                    willClose: () => {
                        clearInterval(timerInterval)
                    }
                }).then((result) => {
                    if (result.dismiss === Swal.DismissReason.timer) {

                        window.location.reload();
                    }
                })
            }





        }
    })
}

$(document).ready(function () {
    $("#SubmitEditVolunteer").submit(function (event) {
        event.preventDefault(); // Prevent the default form submission
        // Make an AJAX request to submit the form data
        $.ajax({
            url: $(this).attr("action"),
            type: $(this).attr("method"),
            data: $(this).serialize(),
            success: function (data) {
                console.log(data);
                if (data.success == true) {
                    OnSuccess(data);
                }
                else {
                    OnFailure(data);
                }
            }
        });
    });
})
