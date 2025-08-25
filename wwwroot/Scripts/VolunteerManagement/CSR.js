var attactmentfile;
$(document).ready(function () {
    var t = $('#_csrTbl').DataTable({
        "pagingType": "full_numbers"
    });
    t.on('order.dt search.dt', function () {
        t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
    $("table").removeAttr("style");
    $('#CreateCSRModal').on('hide.bs.modal', function () {
        var form = $(MEL.toSelector('CreateCSR-form'));
        form.formReset();
        $("#2stcontactDetails").addClass("hidden");
        $("#3stcontactDetails").addClass("hidden");
        $("#AddPIC").removeClass("hidden")
    });
    $('#EditCSRModal').on('hide.bs.modal', function () {
        var form = $(MEL.toSelector('EditCSR-form'));
        form.formReset();
        $("#edit2stcontactDetails").addClass("hidden");
        $("#edit3stcontactDetails").addClass("hidden");
        $("#edit_AddPIC").removeClass("hidden");

        //remove phone input plugin to avoid dropdown become conflict issues
        let y;
        for (y = 1; y < 4; y++) {
            destroyPhoneInputField("#editcontactNo_Mobile_" + y);
            destroyPhoneInputField("#editcontactNo_Office_" + y);
        }
        
    });

    //$("#EditCSR-form").click(function () {
       
    //    $.ajax({
          
    //        type: 'POST',
    //        url: '/VolunteerManagement/EditCSR?',
         
    //        type: 'POST',
    //    })
    //});

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
    //Initialize phone field
    //initPhoneField();
    initPhoneFieldwithPara("#contactNo_Mobile_1", "#AddEditCSR_ContactDetails_0__ContactNo_Mobile",1);
    initPhoneFieldwithPara("#contactNo_Mobile_2", "#AddEditCSR_ContactDetails_1__ContactNo_Mobile",1);
    initPhoneFieldwithPara("#contactNo_Mobile_3", "#AddEditCSR_ContactDetails_2__ContactNo_Mobile",1);

    
    initPhoneFieldwithPara("#contactNo_Office_1", "#AddEditCSR_ContactDetails_0__ContactNo_Office",1);
    initPhoneFieldwithPara("#contactNo_Office_2", "#AddEditCSR_ContactDetails_1__ContactNo_Office",1);
    initPhoneFieldwithPara("#contactNo_Office_3", "#AddEditCSR_ContactDetails_2__ContactNo_Office", 1);
    
      
      
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
    //$(fieldID).rules("add", {
    //    number: true

    //});
    if (z == "65") {
        //$(fieldID).attr('maxlength', 8);
        $(fieldID).rules("add", {
            /*number: true,*/
            maxlength: 8

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
                $(fieldID).rules("remove","maxlength" );
                $(fieldID).rules("add", {
                    maxlength: 8
                });
        }
        else
        {
                $(fieldID).rules("remove", "maxlength");
                $(fieldID).rules("add", {
                    maxlength: 12

                });
        }
    });
    $(fieldID).rules("add", {
        required: false
    });

}

//remove entire plugin field for phone input
function destroyPhoneInputField(fieldID) {
    var init = new intlTelInput(document.querySelector(fieldID));
    init.destroy();
    init.destroy();

}
function tog() {
    //console.log($("#editcontactNo_Mobile_1").rules())
    //console.log($("#editcontactNo_Mobile_2").rules())
    //console.log($("#editcontactNo_Mobile_3").rules())
}

function toggleContactDetails() {

    if ($("#2stcontactDetails").is(":hidden")) {
       
        $("#2stcontactDetails").removeClass("hidden");
        $("#hideToggle").removeClass("hidden");
    } else {
        $("#3stcontactDetails").removeClass("hidden");
        $("#hideToggle").removeClass("hidden");
    } 

   
    
    if (($("#2stcontactDetails").is(":hidden") || $("#3stcontactDetails").is(":hidden"))) {
        
        //alert("quan dou show le")
    } else {
        $("#hideToggle").removeClass("hidden");
        $(".divBtnContact").addClass("hidden");
    }
}

function togglehideContactDetails() {

    //if ($("#2stcontactDetails").is(":hidden") == true) {
    //    $("#hideToggle").addClass("hidden");
    //}


    if ($("#2stcontactDetails").is(":hidden")) {
 
      
    } else if ($("#3stcontactDetails").is(":hidden") && $("#2stcontactDetails").not(":hidden") ){
        $("#2stcontactDetails").addClass("hidden");
        $("#hideToggle").addClass("hidden");
        $(".divBtnContact").removeClass("hidden");
    }
        
        
        
        else {
        $("#3stcontactDetails").addClass("hidden");
        $(".divBtnContact").removeClass("hidden");
        //$("#2stcontactDetails").addClass("hidden");
        //$("#hideToggle").addClass("hidden");
    }



    if ($("#3stcontactDetails").is(":hidden") ) {
      
    } else if ($("#3stcontactDetails").is(":hidden") && $("#2stcontactDetails").not(":hidden"))
    {
        $("#2stcontactDetails").removeClass("hidden");
        //$(".divBtnContact").removeClass("hidden");
    } else {
       
        $("#3stcontactDetails").addClass("hidden");
        $("#hideToggle").removeClass("hidden");
        //$("#hideToggle").addClass("hidden");
    }




  
}
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
    //console.log("failure");
    //console.log(result);
}

function btnDownload(value) {
    
    //alert('btnDownload');
    
    //console.log(value);
    var mystring = $(value).data("string");
    var fileSplit = [];
    fileSplit = mystring.split("#");
    var fileName = fileSplit[3];
    //console.log(mystring);


    $.ajax({
       // Data:'mystring',
       // filename=mystring,
      //  type: 'POST',
        url: '/VolunteerManagement/DownloadPDF?',
        data:{value:mystring},
        type: 'POST',
     
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
                //$("body").remove(a);
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

function btnDelete(value) {
    var mystring = $(value).data("string");
   
    var fileSplit = [];
    fileSplit = mystring.split("#");
    var fileName = fileSplit[3];
    console.log(mystring);

    $.ajax({
        type: 'POST',
        url: '/VolunteerManagement/DeleteUploadedFile?',
        data: { value: mystring },

    });
}

function viewCSR(id) {
    $("#ViewCSRDetailsModal").modal("show");
    $("#pic1details").addClass("hidden");
    $("#pic2details").addClass("hidden");
    $("#pic3details").addClass("hidden");
    $.ajax({
        url: '/VolunteerManagement/GetCSRDetailsById?Id=' + id,
        type: 'POST',
        success: function (result) {
            //console.log(result);
            //console.log(result.ContactDetails[0]);
            attactmentfile.clear();
            attactmentfile.draw();
            //clear row
            $("#csrDetailsTbl tr td").each(function () {
                $(this).empty();
            });
            $("#AttachmentFilesTbl tr td td").each(function () {
                $(this).empty();
            });
            $("#view-csrDetails-name").append(result.name == null ? "-" : result.name);


          /*  $("#view-csrDetails-AttachmentFiles").text(result.AttachmentFiles);*/
            $("#view-csrDetails-add1").append(result.address1 == null ? "-" : result.address1);
        
            $("#view-csrDetails-add2").append(result.address2 == null ? "-" : result.address2);
            $("#view-csrDetails-postal").append(result.postal==null?"-":result.postal);

            $(" #3picDetailsTbl tr td").each(function () {
                $(this).empty();
            });
            $(" #2picDetailsTbl tr td").each(function () {
                $(this).empty();
            });
            $(" #1picDetailsTbl tr td").each(function () {
                $(this).empty();
            });
            $(" #picDetailsTbl tr td").each(function () {
                $(this).empty();
            });


            if (result.contactDetails.length == 0) {
                $("#pic3details").addClass("hidden");
                $("#pic2details").addClass("hidden");
                $("#pic1details").addClass("hidden");
                $("#picdetails").removeClass("hidden");

                $("#view-picDetails-name").append(result.PIC == null ? "-" : result.PIC);
                $("#view-picDetails-mobile").append(result.contact == null ? "-" : result.contact);
            }

            else if (result.contactDetails.length == 1) {
                $("#picdetails").addClass("hidden");
                $("#pic1details").removeClass("hidden");
                //initliaze fields
                $("#view-1picDetails-title").append(result.contactDetails[0].title == null ? "-" : result.contactDetails[0].title);
                $("#view-1picDetails-name").append(result.contactDetails[0].name == null ? "-" : result.contactDetails[0].name);
                $("#view-1picDetails-email").append(result.contactDetails[0].email == null ? "-" : result.contactDetails[0].email);
                $("#view-1picDetails-mobile").append(result.contactDetails[0].contactNo_Mobile == null ? "-" : result.contactDetails[0].contactNo_Mobile);
                $("#view-1picDetails-office").append(result.contactDetails[0].contactNo_Office == null ? "-" : result.contactDetails[0].contactNo_Office);
                
            }
            else if (result.contactDetails.length == 2) {
                $("#picdetails").addClass("hidden");
                $("#pic1details").removeClass("hidden");
                $("#pic2details").removeClass("hidden");

                $("#view-1picDetails-title").append(result.contactDetails[0].title == null ? "-" : result.contactDetails[0].title);
                $("#view-1picDetails-name").append(result.contactDetails[0].name == null ? "-" : result.contactDetails[0].name);
                $("#view-1picDetails-email").append(result.contactDetails[0].email == null ? "-" : result.contactDetails[0].email);
                $("#view-1picDetails-mobile").append(result.contactDetails[0].contactNo_Mobile == null ? "-" : result.contactDetails[0].contactNo_Mobile);
                $("#view-1picDetails-office").append(result.contactDetails[0].contactNo_Office == null ? "-" : result.contactDetails[0].contactNo_Office);

                $("#view-2picDetails-title").append(result.contactDetails[1].title == null ? "-" : result.contactDetails[1].title);
                $("#view-2picDetails-name").append(result.contactDetails[1].name == null ? "-" : result.contactDetails[1].name);
                $("#view-2picDetails-email").append(result.contactDetails[1].email == null ? "-" : result.contactDetails[1].email);
                $("#view-2picDetails-mobile").append(result.contactDetails[1].contactNo_Mobile == null ? "-" : result.contactDetails[1].contactNo_Mobile);
                $("#view-2picDetails-office").append(result.contactDetails[1].contactNo_Office == null ? "-" : result.contactDetails[1].contactNo_Office);


            }
            else {
                $("#picdetails").addClass("hidden");
                $("#pic3details").removeClass("hidden");
                $("#pic2details").removeClass("hidden");
                $("#pic1details").removeClass("hidden");

                $("#view-1picDetails-title").append(result.contactDetails[0].title == null ? "-" : result.contactDetails[0].title);
                $("#view-1picDetails-name").append(result.contactDetails[0].name == null ? "-" : result.contactDetails[0].name);
                $("#view-1picDetails-email").append(result.contactDetails[0].email == null ? "-" : result.contactDetails[0].email);
                $("#view-1picDetails-mobile").append(result.contactDetails[0].contactNo_Mobile == null ? "-" : result.contactDetails[0].contactNo_Mobile);
                $("#view-1picDetails-office").append(result.contactDetails[0].contactNo_Office == null ? "-" : result.contactDetails[0].contactNo_Office);

                $("#view-2picDetails-title").append(result.ContactDetails[1].title == null ? "-" : result.ContactDetails[1].title);
                $("#view-2picDetails-name").append(result.ContactDetails[1].name == null ? "-" : result.ContactDetails[1].name);
                $("#view-2picDetails-email").append(result.contactDetails[1].email == null ? "-" : result.contactDetails[1].email);
                $("#view-2picDetails-mobile").append(result.contactDetails[1].contactNo_Mobile == null ? "-" : result.contactDetails[1].contactNo_Mobile);
                $("#view-2picDetails-office").append(result.contactDetails[1].contactNo_Office == null ? "-" : result.contactDetails[1].contactNo_Office);

                $("#view-3picDetails-title").append(result.contactDetails[2].title == null ? "-" : result.contactDetails[2].title);
                $("#view-3picDetails-name").append(result.contactDetails[2].name == null ? "-" : result.contactDetails[2].name);
                $("#view-3picDetails-email").append(result.contactDetails[2].email == null ? "-" : result.contactDetails[2].email);
                $("#view-3picDetails-mobile").append(result.contactDetails[2].contactNo_Mobile == null ? "-" : result.contactDetails[2].contactNo_Mobile);
                $("#view-3picDetails-office").append(result.contactDetails[2].contactNo_Office == null ? "-" : result.contactDetails[2].contactNo_Office);
                         
            }

            var getFileSplitted = [];
            getFileSplitted = result.fullDocName.split(",");  //change to modifiedName

            //console.log(getFileSplitted)
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

//$(document).ready(function () {
//    $("#downloadFile").click(function (e) {
//        e.preventDefault();

//        window.location.href
//            = "";

//    });
//});

//$(document).ready(function () {
//    $('btnViewImg').click(function () {
       
//    });
//});

function delete_CSR(id) {
    Swal.fire({
        title: 'Do you want to delete this organization?',
        showCancelButton: true,
        confirmButtonText: 'Confirm',
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            $.ajax({
                url: '/VolunteerManagement/DeleteCSR?Id=' + id,
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

function editCSR(id) {
    $.ajax({
        url: '/VolunteerManagement/GetCSRDetailsById?Id=' + id,
        type: 'POST',
        success: function (info) {
            attactmentfile1.clear();
            attactmentfile1.draw();
            console.log(info)

            $("#edit_Id").val(info.id);
            $("#edit-name").val(info.name);
            $("#edit-address1").val(info.address1);
            $("#edit-address2").val(info.address2);
            $("#edit-postal").val(info.postal);

            //console.log(info.ContactDetails.length);
            if (info.contactDetails.length == 2) {
                $("#edit2stcontactDetails").removeClass("hidden");
            }
            else if (info.contactDetails.length == 3) {
                $("#edit3stcontactDetails").removeClass("hidden");
                $("#edit2stcontactDetails").removeClass("hidden");
                $("#edit_AddPIC").addClass("hidden");
            }
            //toggle pic forms displays

            let x = 0;
            if (info.contactDetails.length == 0) {
                let y;
                for (y = 1; y < 4; y++) {
                    $("#editcontactNo_Mobile_"+y).val("");
                    $("#editcontactNo_Office_"+y).val("");
                    $("#edit_hiddenmobile"+y).val("");
                    $("#edit_hiddenoffice"+y).val("");
                }

            }
            $.each(info.contactDetails, function (index, value) {
                //console.log(index)
                //console.log(value.Title)
                $("#AddEditCSR_ContactDetails_"+x+"__Id").val(value.id)
                $("#editpic" + (x+1) + "-title").val(value.title);
                $("#editpic" + (x+1) + "-name").val(value.name);
                $("#editpic" + (x+1) + "-email").val(value.email);
                $("#edit_hiddenmobile" + (x+1)).val(value.contactNo_Mobile);
                $("#edit_hiddenoffice" + (x+1)).val(value.contactNo_Office);
                x++;
            });


               var getFileSplitted = [];
            getFileSplitted = info.fullDocName.split(",");  //change to modifiedName

            //console.log(getFileSplitted)
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
          
          //  attactmentfile1.row.add(["<button style='padding:center;'>ADD</button>"])
        
            let y;
            for (y = 1; y < 4; y++) {
                initPhoneFieldwithPara("#editcontactNo_Mobile_" + y, "#edit_hiddenmobile" + y,2);
                initPhoneFieldwithPara("#editcontactNo_Office_" + y, "#edit_hiddenoffice" + y,2);
            }


            
        }
    });
    $("#EditCSRModal").modal('show');
}

//function toggleEditContactDetails() {
//    if ($("#edit2stcontactDetails").is(":hidden")) {
//        $("#edit2stcontactDetails").removeClass("hidden");
//    } else {
//        $("#edit3stcontactDetails").removeClass("hidden");
//    }

//    if (($("#edit2stcontactDetails").is(":hidden") || $("#edit3stcontactDetails").is(":hidden")) == false) {

//        $("#edit_AddPIC").addClass("hidden");
//        //alert("quan dou show le")
//    }
//}



function toggleEditContactDetails() {

    if ($("#edit2stcontactDetails").is(":hidden")) {

        $("#edit2stcontactDetails").removeClass("hidden");
        $("#edithideToggle").removeClass("hidden");
    } else {
        $("#edit3stcontactDetails").removeClass("hidden");
        $("#edithideToggle").removeClass("hidden");
    }



    if (($("#edit2stcontactDetails").is(":hidden") || $("#edit3stcontactDetails").is(":hidden"))) {

        //alert("quan dou show le")
    } else {
        $("#edithideToggle").removeClass("hidden");
        $(".editdivBtnContact").addClass("hidden");
    }
}

function toggleEdithideContactDetails() {

    //if ($("#2stcontactDetails").is(":hidden") == true) {
    //    $("#hideToggle").addClass("hidden");
    //}


    if ($("#edit2stcontactDetails").is(":hidden")) {


    } else if ($("#edit3stcontactDetails").is(":hidden") && $("#edit2stcontactDetails").not(":hidden")) {
        $("#edit2stcontactDetails").addClass("hidden");
        $("#edithideToggle").addClass("hidden");
        $(".editdivBtnContact").removeClass("hidden");
    }



    else {
        $("#edit3stcontactDetails").addClass("hidden");
        $(".editdivBtnContact").removeClass("hidden");
        //$("#2stcontactDetails").addClass("hidden");
        //$("#hideToggle").addClass("hidden");
    }



    if ($("#edit3stcontactDetails").is(":hidden")) {

    } else if ($("#edit3stcontactDetails").is(":hidden") && $("#edit2stcontactDetails").not(":hidden")) {
        $("#edit2stcontactDetails").removeClass("hidden");
        //$(".divBtnContact").removeClass("hidden");
    } else {

        $("#edit3stcontactDetails").addClass("hidden");
        $("#edithideToggle").removeClass("hidden");
        //$("#hideToggle").addClass("hidden");
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




function SuccessCreate(result) {
    console.log(result.data.message)
    bootbox.alert({
        size: "small",
        title: "Message",
        message: result.data.message,
        callback: function () {
            if (result.success) {
                //var form = $(MEL.toSelector('form0'));
                //form.formReset();
                $('#CreateCSR-form').modal('hide');
                //var table = $('#CategoryTable').DataTable();
                //console.log(table);
                window.location.reload();
                //table.refresh();
                //table = $("#CategoryTable").dataTable();
                //$('#CategoryTable').DataTable().ajax.reload()
            } else {
                return;
            }
        }
    });
}
function SuccessWithdraw(result) {
    console.log(result.data.message)
    bootbox.alert({
        size: "small",
        title: "Message",
        message: result.data.message,
        callback: function () {
            if (result.success) {
                //var form = $(MEL.toSelector('form0'));
                //form.formReset();
                $('#EditCSR-form').modal('hide');
                //var table = $('#CategoryTable').DataTable();
                //console.log(table);
                window.location.reload();
                //table.refresh();
                //table = $("#CategoryTable").dataTable();
                //$('#CategoryTable').DataTable().ajax.reload()
            } else {
                return;
            }
        }
    });
}

$("#EditCSR-form").submit(function (event) {
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


$("#CreateCSR-form").submit(function (event) {
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
