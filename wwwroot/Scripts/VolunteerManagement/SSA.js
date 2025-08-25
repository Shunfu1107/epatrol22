
$(document).ready(function () {
    var t = $('#_ssaTbl').DataTable({
        "pagingType": "full_numbers"
    });
    t.on('order.dt search.dt', function () {
        t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
    $("table").removeAttr("style");

    var s = new Date();
    var e = new Date();
    s.setMonth(s.getMonth() - 2);
    e.setMonth(e.getMonth() + 24);

    $('.datepicker').datetimepicker({
        minDate: s,
        maxDate: e,
        useCurrent: false,
        format: 'DD/MM/YYYY'
    });
    $('#UpdateSSAModal').on('hide.bs.modal', function () {
        var form = $(MEL.toSelector('_formSSA_Update'));
        form.formReset();
        $("#edit2stcontactDetails").addClass("hidden");
        $("#edit_AddPIC").removeClass("hidden")
    });

    attactmentfile = $('#AttachmentFilesTbl').DataTable({
        searching: false,
        paging: false,
        info: false,
        //"columnDefs": [
        //    { width: "5%", targets: 0 },
        //    { width: "65%", targets: 1 },

        //    { width: "30%", targets: 2 }]

    });
    attactmentfile1 = $('#AttachmentFilesTbl1').DataTable({
        searching: false,
        paging: false,
        info: false,
        //"columnDefs": [
        //    { width: "5%", targets: 0 },
        //    { width: "65%", targets: 1 },

        //    { width: "30%", targets: 2 }]

    });
    //initialize phone number fields
    initPhoneFieldwithPara("#contactNo_Mobile_1", "#SSADetails_PIC_Details_0__Contact_Mobile",1);
    initPhoneFieldwithPara("#contactNo_Mobile_2", "#SSADetails_PIC_Details_1__Contact_Mobile",1);

    initPhoneFieldwithPara("#contactNo_Office_1", "#SSADetails_PIC_Details_0__Contact_Office",1);
    initPhoneFieldwithPara("#contactNo_Office_2", "#SSADetails_PIC_Details_1__Contact_Office",1);

})


function initPhoneFieldwithPara(fieldID, HiddenID,x) {
    //initialize contact no mobile
    var input = document.querySelector(fieldID);
    var phone = window.intlTelInput(input, {
        onlyCountries: ["sg", "my", "id", "cn"],
        utilsScript: "/js/utils.js",
        initialCountry: "sg",
        formatOnDisplay: false,
        separateDialCode: true,
        autoPlaceholder: "off",
        geoIpLookup: function (callback) {
            $.get('https://ipinfo.io', function () { }, "jsonp").always(function (resp) {
                var countryCode = (resp && resp.country) ? resp.country : "";
                callback(countryCode);
            });
        }
    });

    //Set value to input
    if (x == 1) {//initialize only
        phone.setNumber($(fieldID).val());
    } else {
        phone.setNumber($(HiddenID).val());//for edit purpose:initialize back phone number
    }
    

    $(fieldID).change(function () {
        const fullNumber = $(fieldID).intlTelInput("getNumber")
        if (fullNumber[0].value != "") {
            $(HiddenID).val(fullNumber[0].parentElement.innerText + fullNumber[0].value);

        } else {
            $(HiddenID).val("");

        }
    });

    var z = phone.getSelectedCountryData().dialCode;
    if (z == "65") {
        $(fieldID).attr('maxlength', 8);
    } else {
        $(fieldID).attr('maxlength', 12);
    }

    input.addEventListener("countrychange", function () {
        var x = phone.getSelectedCountryData().dialCode;
        if (x == "65") {
            $(fieldID).attr('maxlength', 8);
        } else {
            $(fieldID).attr('maxlength', 12);
        }
        $(fieldID).val("");

        const fullNumber = $(fieldID).intlTelInput("getNumber")
        if (fullNumber[0].value != "") {
            $(HiddenID).val(fullNumber[0].parentElement.innerText + fullNumber[0].value);

        } else {
            $(HiddenID).val("");

        }
    });

    $(fieldID).rules("add", {
        number: true
    });
}

function toggleContactDetails() {

    if ($("#2stcontactDetails").is(":hidden")) {

        $("#2stcontactDetails").removeClass("hidden");
        $("#hideToggle").removeClass("hidden");
        $(".divBtnContact").addClass("hidden");
    } else {
      
    }



}

function togglehideContactDetails() {

    //if ($("#2stcontactDetails").is(":hidden") == true) {
    //    $("#hideToggle").addClass("hidden");
    //}


    if ($("#2stcontactDetails").is(":hidden")) {

    } 

    else {
      /*  $("#3stcontactDetails").addClass("hidden");*/
        $(".divBtnContact").removeClass("hidden");
        $("#2stcontactDetails").addClass("hidden");
        $("#hideToggle").addClass("hidden");
    }

}

function edit_toggleContactDetails() {



    if ($("#edit2stcontactDetails").is(":hidden")) {

        $("#edit2stcontactDetails").removeClass("hidden");
        $("#edithideToggle").removeClass("hidden");
        $(".editdivBtnContact").addClass("hidden");
        } else {

        }



    }

function edit_togglehideContactDetails() {

        //if ($("#2stcontactDetails").is(":hidden") == true) {
        //    $("#hideToggle").addClass("hidden");
        //}


    if ($("#edit2stcontactDetails").is(":hidden")) {

        }

        else {
            /*  $("#3stcontactDetails").addClass("hidden");*/
        $(".editdivBtnContact").removeClass("hidden");
        $("#edit2stcontactDetails").addClass("hidden");
        $("#edithideToggle").addClass("hidden");
        }

}


function OnSuccess(result) {
    console.log(result.data.message)
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
    console.log(result);
}
function updateSSA(id) {
    $("#UpdateSSAModal").modal("show");

    $.ajax({
        url: '/VolunteerManagement/GetSSADetailsById?Id=' + id,
        type: 'POST',
        success: function (result) {
            attactmentfile1.clear();
            attactmentfile1.draw();
            console.log(result.id);

            //intialize normal details
            $("#SSADetails_Id").val(result.id)
            $("#edit_Name").val(result.name);
            $("#edit_Constituency").val(result.constituency);
            $("#edit_Zone").val(result.zone);

            $("select#edit_Agency").prop('selectedIndex', 0);
            $("#edit_Agency").val(result.agency);

            //$("#edit_CommunityPartner").val(result.communityPartner);
            $("#edit_Address").val(result.address);
            $("#edit_Postal").val(result.postal);
            $("#edit_TypeOfService").val(result.typeofService);

            $("select#edit_ServiceClientele").prop('selectedIndex', 0);
            $("#edit_ServiceClientele").val(result.serviceClientele);

            $("#edit_ProgramAndService").val(result.programandService);

            if (result.firstContactDate != null) {

                //var firstcontactdate = moment(new Date(parseInt((result.firstContactDate).match(/\d+/)[0]))).format('DD/MM/YYYY');
                var firstcontactdate = moment(new Date(result.firstContactDate)).format('YYYY-MM-DDTHH:mm');
            }
            if (result.firstMeetingDate != null) {

                //var firstmeetingdate = moment(new Date(parseInt((result.firstMeetingDate).match(/\d+/)[0]))).format('DD/MM/YYYY')
                var firstmeetingdate = moment(new Date(result.firstMeetingDate)).format('YYYY-MM-DDTHH:mm');

            }
            $("#edit_1stContactDate").val(firstcontactdate);
            $("#edit_1stMeetingDate").val(firstmeetingdate);

            if (result.active == true) {
                $("#edit_Active").prop('checked', true);
            } else {
                $("#edit_Active").prop('checked', false);
            }

            //initialize back pic details
           
            $("#SSADetails_PIC_Details_0__Id").val(result.piC_Details[0].Id);

            $("select#edit_pic1_title").prop('selectedIndex', 0);
            $("#edit_pic1_title").val(result.piC_Details[0].title);

            $("#edit_pic1_name").val(result.piC_Details[0].name);
            $("#edit_pic1_email").val(result.piC_Details[0].email);
            $("#edit_pic1_position").val(result.piC_Details[0].position);
            
            $("#edit_PICDetails_Mobile1").val(result.piC_Details[0].contact_Mobile);
            initPhoneFieldwithPara("#edit_pic1_mobile", "#edit_PICDetails_Mobile1",2);

            $("#edit_PICDetails_Office1").val(result.piC_Details[0].contact_Office);
            initPhoneFieldwithPara("#edit_pic1_office", "#edit_PICDetails_Office1", 2);


            if (result.piC_Details.length > 1) {
                $("#edit2stcontactDetails").removeClass("hidden");
                $("#edit_AddPIC").addClass("hidden");

                //initialize back pic details
                $("#SSADetails_PIC_Details_1__Id").val(result.piC_Details[1].id);

                $("select#edit_pic2_title").prop('selectedIndex', 0);
                $("#edit_pic2_title").val(result.piC_Details[1].title);

                $("#edit_pic2_name").val(result.piC_Details[1].name);
                $("#edit_pic2_email").val(result.piC_Details[1].email);
                $("#edit_pic2_position").val(result.piC_Details[1].position);
                $("#edit_PICDetails_Mobile2").val(result.piC_Details[1].contact_Mobile);
                initPhoneFieldwithPara("#edit_pic2_mobile", "#edit_PICDetails_Mobile2", 2);

                $("#edit_PICDetails_Office2").val(result.piC_Details[1].contact_Office);
                initPhoneFieldwithPara("#edit_pic2_office", "#edit_PICDetails_Office2", 2);


            } else {
                
                initPhoneFieldwithPara("#edit_pic2_mobile", "#edit_PICDetails_Mobile2", 2);

                
                initPhoneFieldwithPara("#edit_pic2_office", "#edit_PICDetails_Office2", 2);

            } 

            var getFileSplitted = [];
            getFileSplitted = result.fullDocName.split(",");  //change to modifiedName

            console.log(getFileSplitted)
            if (getFileSplitted != "") {
                $.each(getFileSplitted, function (index, value) {
                    // console.log(value);

                    var fileSplit = [];
                    fileSplit = value.split("#");
                    //var valueFile = fileSplit[3];
                    //console.log(valueFile)

                    attactmentfile1.row.add([index + 1, fileSplit[3], "<button onclick='btnDelete(this)' data-string='" + value + ">" + id + "'  style='cursor:pointer;' id='downloadFile' class='fa fa-trash' ></button> "]).node();
                    // attactmentfile.row.add([index + 1, fileSplit[3], '<button onclick="btnDownload("' + value + '")">btn</button>']);
                    //  attactmentfile.row.add([index + 1, value, "<button type='button 'style='cursor:pointer;' id='GetFile' class='fa fa-download'  ></button>"]).node();

                }); attactmentfile1.draw(false);
            }
           
        }
    });


}
function btnDelete(value) {
    var mystring = $(value).data("string");

    var fileSplit = [];
    fileSplit = mystring.split("#");
    var fileName = fileSplit[3];
    console.log(mystring);

    $.ajax({
        /*  type: 'POST',*/
        url: '/VolunteerManagement/DeleteUploadedSSAFile?',
        data: { value: mystring },
        type: 'POST',
    })
}

function btnDownload(value) {

    //alert('btnDownload');

    console.log(value);
    var mystring = $(value).data("string");
    var fileSplit = [];
    fileSplit = mystring.split("#");
    var fileName = fileSplit[3];
    console.log(mystring);


    $.ajax({
        // Data:'mystring',
        // filename=mystring,
       
        url: '/VolunteerManagement/DownloadSSADoc?',
        data: { value: mystring },
        type: 'POST',
        //success: function (result) {
        //    if (result == "Success") {
        //        location.href = '/VolunteerManagement/DownloadCSV';
        //    }
        //}
        success: function (r) {
            //Convert Base64 string to Byte Array.
            var bytes = Base64ToBytes(r);

            //Convert Byte Array to BLOB.
            var blob = new Blob([bytes], { type: "application/octetstream" });

            //Check the Browser type and download the File.
            var isIE = false || !!document.documentMode;
            if (isIE) {
                window.navigator.msSaveBlob(blob, fileName);
            } else {
                var url = window.URL || window.webkitURL;
                link = url.createObjectURL(blob);
                var a = $("<a />");
                a.attr("download", fileName);
                a.attr("href", link);
                $("body").append(a);
                a[0].click();
               // $("body").remove(a);
            }
        }
    })

}
function Base64ToBytes(base64) {
    var s = window.atob(base64);
    var bytes = new Uint8Array(s.length);
    for (var i = 0; i < s.length; i++) {
        bytes[i] = s.charCodeAt(i);
    }
    return bytes;
};

function viewSSA(id) {
    $("#ViewSSAModal").modal("show");

    $.ajax({
        url: '/VolunteerManagement/GetSSADetailsById?Id=' + id,
        type: 'POST',
        success: function (result)
        {
            console.log(result);
            attactmentfile.clear();
            attactmentfile.draw();
            //console.log(result);
            //console.log(result.contactDetails[0]);
            //clear row
            $("#ssaDetailsTbl tr td").each(function () {
                $(this).empty();

            });
            $("#view-ssaDetails-name").append(result.name == null ? "-" : result.name);
            $("#view-ssaDetails-constituency").append(result.name == null ? "-" : result.constituency);
            $("#view-ssaDetails-zone").append(result.name == null ? "-" : result.zone);
            $("#view-ssaDetails-agency").append(result.name == null ? "-" : result.agency);
            $("#view-ssaDetails-cP").append(result.name == null ? "-" : result.communityPartner);
            $("#view-ssaDetails-add").append(result.name == null ? "-" : result.address);
            $("#view-ssaDetails-post").append(result.name == null ? "-" : result.postal);
            $("#view-ssaDetails-toS").append(result.name == null ? "-" : result.typeofService);
            $("#view-ssaDetails-sC").append(result.name == null ? "-" : result.serviceClientele);
            $("#view-ssaDetails-poc").append(result.name == null ? "-" : result.programandService);

            
            
            
            if (result.firstContactDate != null) {
                //var CDate = new Date(parseInt((result.firstContactDate).match(/\d+/)[0]));
                var CDate = new Date(result.firstContactDate);
                var formated1stCDate = $.datepicker.formatDate('dd M yy', CDate);
                $("#view-ssaDetails-1stCDate").append(formated1stCDate);
            } else {
                $("#view-ssaDetails-1stCDate").append("-");
            }

            if (result.firstMeetingDate != null) {
                //var MDate = new Date(parseInt((result.firstMeetingDate).match(/\d+/)[0]));
                var MDate = new Date(result.firstMeetingDate)
                var formated1stMDate = $.datepicker.formatDate('dd M yy', MDate);
                $("#view-ssaDetails-1stMDate").append(formated1stMDate);
            } else {
                $("#view-ssaDetails-1stMDate").append("-");
            }

            $("#view-ssaDetails-status").append(result.active == false ? "Inactive" :"Active");


            $(" #2picDetailsTbl tr td").each(function () {
                $(this).empty();
            });
            $(" #1picDetailsTbl tr td").each(function () {
                $(this).empty();
            });
            if (result.piC_Details.length > 1) {
                $("#pic2details").removeClass("hidden");

                $("#view-1picDetails-title").append(result.piC_Details[0].title == null ? "-" : result.piC_Details[0].title);
                $("#view-1picDetails-name").append(result.piC_Details[0].name == null ? "-" : result.piC_Details[0].name);
                $("#view-1picDetails-position").append(result.piC_Details[0].position == null ? "-" : result.piC_Details[0].position);
                $("#view-1picDetails-email").append(result.piC_Details[0].email == null ? "-" : result.piC_Details[0].email);
                $("#view-1picDetails-mobile").append(result.piC_Details[0].contact_Mobile == null ? "-" : result.piC_Details[0].contact_Mobile);
                $("#view-1picDetails-office").append(result.piC_Details[0].contact_Office == null ? "-" : result.piC_Details[0].contact_Office);

                $("#view-2picDetails-title").append(result.piC_Details[1].title == null ? "-" : result.piC_Details[1].title);
                $("#view-2picDetails-name").append(result.piC_Details[1].name == null ? "-" : result.piC_Details[1].name);
                $("#view-2picDetails-position").append(result.piC_Details[1].position == null ? "-" : result.piC_Details[1].position);
                $("#view-2picDetails-email").append(result.piC_Details[1].email == null ? "-" : result.piC_Details[1].email);
                $("#view-2picDetails-mobile").append(result.piC_Details[1].contact_Mobile == null ? "-" : result.piC_Details[1].contact_Mobile);
                $("#view-2picDetails-office").append(result.piC_Details[1].contact_Office == null ? "-" : result.piC_Details[1].contact_Office);

            } else {
                $("#pic2details").addClass("hidden");
                $("#view-1picDetails-title").append(result.piC_Details[0].title == null ? "-" : result.piC_Details[0].title);
                $("#view-1picDetails-name").append(result.piC_Details[0].name == null ? "-" : result.piC_Details[0].name);
                $("#view-1picDetails-position").append(result.piC_Details[0].position == null ? "-" : result.piC_Details[0].position);
                $("#view-1picDetails-email").append(result.piC_Details[0].email == null ? "-" : result.piC_Details[0].email);
                $("#view-1picDetails-mobile").append(result.piC_Details[0].contact_Mobile == null ? "-" : result.piC_Details[0].contact_Mobile);
                $("#view-1picDetails-office").append(result.piC_Details[0].contact_Office == null ? "-" : result.piC_Details[0].contact_Office);

                
            }
            var getFileSplitted = [];
            getFileSplitted = result.fullDocName.split(",");  //change to modifiedName

            console.log(getFileSplitted)
            if (getFileSplitted != "") {
                $.each(getFileSplitted, function (index, value) {
                    // console.log(value);

                    var fileSplit = [];
                    fileSplit = value.split("#");
                    //var valueFile = fileSplit[3];
                    //console.log(valueFile)

                    attactmentfile.row.add([index + 1, fileSplit[3], "<button onclick='btnDownload(this)' data-string='" + value + "' style='cursor:pointer;' id='downloadFile' class='fa fa-download'  ></button>"]).node();
                    // attactmentfile.row.add([index + 1, fileSplit[3], '<button onclick="btnDownload("' + value + '")">btn</button>']);
                    //  attactmentfile.row.add([index + 1, value, "<button type='button 'style='cursor:pointer;' id='GetFile' class='fa fa-download'  ></button>"]).node();
                });
                attactmentfile.draw(false);
            }

        }
    });


}

function deleteSSA(id) {
    Swal.fire({
        title: 'Do you want to delete this ssa/partner?',
        showCancelButton: true,
        confirmButtonText: 'Confirm',
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            $.ajax({
                url: '/VolunteerManagement/DeleteSSA?Id=' + id,
                type: 'POST',
                success: function (result) {
                    let timerInterval
                    Swal.fire({
                        icon: 'success',
                        title: result.data.message,
                        timer: 1500,
                        timerProgressBar: true,
                        didOpen: () => {
                            Swal.showLoading()
                            const b = Swal.getHtmlContainer().querySelector('b')
                            timerInterval = setInterval(() => {
                              //  b.textContent = Swal.getTimerLeft()
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
                }
            });
        }
    })
}

//function for initialize back selected ddl values
function getSelectedOption(sel, datatext) {
var opt;
for (var i = 0, len = sel.options.length; i < len; i++) {
    opt = sel.options[i];
    if (opt.text == datatext) {
        opt.selected = true;
    }
}
}


function addNewDoc() {

    if ($("#newDocument").is(":hidden")) {
        $("#newDocument").removeClass("hidden");
        $("#editDocToggle").removeClass("hidden");
    }


}
function toggleEditHideDoc() {

    if ($("#newDocument").not(":hidden")) {

        $("#newDocument").addClass("hidden");
        $("#editDocToggle").addClass("hidden");
        //$("#edithideToggle").removeClass("hidden");
    }



}