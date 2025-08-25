/// <reference path="../../Assets/Plugins/extended-user-identity/mel.core.js" />    


$(window).on("load", function () {
    MEL.startup(function () {
        this.load();
    });
    initCalendar();
    CBPScheduling();
});
var ID_CBP_VIEW = 'cbp-home'
    , ID_CALENDAR = 'calendar'
    , ID_BTN_ADD = 'btn-cbp-add'
    , ID_BTN_ADD_CANCEL = 'btn-cbp-add-cancel'
    , ID_BTN_TO_EDIT = 'btn-cbp-to-edit'
    , ID_BTN_EDIT = 'btn-cbp-edit'
    , ID_BTN_EDIT_CANCEL = 'btn-cbp-edit-cancel'        
    , ID_BTN_DELETE = 'btn-cbp-delete'
    , ID_BTN_PRINT = 'btn-print-pdf'
    , ID_BTN_ADD_VOLUNTEER = 'btn-add-volunteer'
    , ID_FORM_ADD_CBP = 'form-add-cbp'
    , ID_FORM_EDIT_CBP = 'form-edit-cbp'
    , ID_FORM_DELETE_VOLUNTEER = 'form-delete-volunteer'
    , ID_MEL_LAYER = 'mel-layer'
    , ID_CBP_DETAILS_SECTION = 'show-cbp-details'
    , ID_CBP_EDIT_SECTION = 'edit-cbp-details'
    , ID_CBP_EDIT_GROUPLEADER = 'edit-groupleader-field'
    , ID_CBP_EDIT_DATE = 'edit-date-field'
    , ID_CBP_EDIT_REMARK = 'edit-remark-field'
    //Appointment    
    , ID_BTN_ADD_APPOINTMENT = 'btn-cbp-add-appointment'
    , ID_BTN_ADD_APPOINTMENT_CANCEL = 'btn-cbp-add-cancel-appointment'
    , ID_BTN_EDIT_APPOINTMENT = 'btn-cbp-edit-appointment'
    , ID_BTN_EDIT_CANCEL_APPOINTMENT = 'btn-cbp-edit-cancel-appointment'
    , ID_FORM_EDIT_CBP_APPOINTMENT = 'form-edit-cbp-appointment'
    , ID_BTN_DELETE_APPOINTMENT = 'CBP_Delete_Appointment_BTN'
    , ID_FORM_ADD_CBP_APPOINTMENT = 'form-add-cbp-appointment'
    //URL
    , URL_ADD_CBP = '/CBPScheduling/Create'
    , URL_EDIT_CBP = '/CBPScheduling/Edit'
    , URL_DELETE_CBP = '/CBPScheduling/Delete'
    , URL_CANCEL_CBP = '/CBPScheduling/Cancel'
    , URL_ADD_CBP_APPOINTMENT = '/CBPScheduling/CreateAppointment'
    , URL_EDIT_CBP_APPOINTMENT = '/CBPScheduling/EditAppointment'
    , URL_DELETE_CBP_APPOINTMENT = '/CBPScheduling/DeleteAppointment'
    , URL_CANCEL_CBP_APPOINTMENT = '/CBPScheduling/CancelAppointment'

    , addCbp = null
    , editCbp = null
    , addCbpAppointment = null
    , editCbpAppointment = null
    , deleteCbp = null
    , addVolunteer = null;

function CBPScheduling() {
    
    function init() {
        addCbp = _addCbp();
        editCbp = _editCBP();
        addCbpAppointment = _addCbpAppointment();
        editCbpAppointment = _editCBPAppointment();
        wireupEvent();
        $(MEL.toSelector(ID_MEL_LAYER)).css('display', '');
    }

    function wireupEvent() {

        //Wire up for print button
        var btnprint = $(MEL.toSelector(ID_BTN_PRINT));

        btnprint.off();

        btnprint.click(function () {
            Downprint();
        })

        ////Wire up for edit button
        //var btntoedit = $(MEL.toSelector(ID_BTN_TO_EDIT));
        //var btncanceledit = $(MEL.toSelector(ID_BTN_EDIT_CANCEL));
        //var cbpdetailssection = $(MEL.toSelector(ID_CBP_DETAILS_SECTION));
        //var cbpeditssection = $(MEL.toSelector(ID_CBP_EDIT_SECTION));

        //cbpeditssection.hide();

        //btntoedit.off();
        //btncanceledit.off();

        //btntoedit.click(function () {
        //    cbpdetailssection.hide();
        //    cbpeditssection.show();
        //    btntoedit.hide();
        //    setDatatoInput(cbpEditDetails);
        //});

        //btncanceledit.click(function () {
        //    var form = $(MEL.toSelector('form-edit-cbp'));
        //    form.formReset();
        //    cbpdetailssection.show();
        //    cbpeditssection.hide();
        //    btntoedit.show();
        //});

        ////Create schedule time validation
        //$('#AddNewCBP_EndTime').focusout(function () {
        //    startDate = $('#datetimepicker1').datetimepicker("viewDate");
        //    endDate = $('#datetimepicker2').datetimepicker("viewDate");

        //    if (startDate != null && startDate != 'undefined') {
        //        if (endDate < startDate) {
        //            bootbox.alert({
        //                size: "small",
        //                title: "Message",
        //                message: "Please enter valid End Time.",
        //                callback: function () {
        //                    $('#AddNewCBP_EndTime').val("");
        //                }
        //            });
        //        }
        //    }
        //});

        ////Edit schedule time validation
        //$('#EditCBPModel_EndTime').focusout(function () {
        //    startDate = $('#datetimepicker3').datetimepicker("viewDate");
        //    endDate = $('#datetimepicker4').datetimepicker("viewDate");

        //    if (startDate != null && startDate != 'undefined') {
        //        if (endDate < startDate) {
        //            bootbox.alert({
        //                size: "small",
        //                title: "Message",
        //                message: "Please enter valid End Time.",
        //                callback: function () {
        //                    $('#EditCBPModel_EndTime').val("");
        //                }
        //            });
        //        }
        //    }
        //})
    }

    function _addCbp() {
        
        function init() {
            wireupEvent();
        }

        function wireupEvent() {
            var btnadd = $(MEL.toSelector(ID_BTN_ADD));

            btnadd.off();

            btnadd.click(function () {
                event.preventDefault();
                var el = $(this);
                el.prop('disabled', true);                
                doadd();
                setTimeout(function () { el.prop('disabled', false); }, 3000);
            })
        }

        function clearForm() {
            var form = $(MEL.toSelector(ID_FORM_ADD_CBP));
            form.formReset();
        }

        function doadd() {
            var form = $(MEL.toSelector(ID_FORM_ADD_CBP));
            var btncancel = $(MEL.toSelector(ID_BTN_ADD_CANCEL));

            if (!form.valid()) {
                $('.input-validation-error').first().focus();
                return;
            }

            var dto = MEL.toDTO(form);
            dto["AddNewCBP.ScheduleDate"] = moment(dto["AddNewCBP.ScheduleDate"], 'DD/MM/YYYY').format('YYYY-MM-DD HH:mm:ss');
            
            MEL.load({
                url: URL_ADD_CBP,
                dto: dto,
                callback: function (info) {
                    console.log(info);
                    if (info.available == false && info.success == false) {
                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: info.exception
                        });
                    } else if (info.success == false) {
                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: info.exception
                        });
                    } else {
                        var result = info.message.split("|"); 
                        if (result.length > 1) {
                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: result[1],
                                callback: function () {
                                    $('.scheduleId').val(result[0]);
                                    GetCBPDetails(result[0]);
                                    $('#ViewCBPModal').modal('show');
                                    UpdateCalendar();
                                    clearForm();
                                    btncancel.click();
                                }
                            });
                        } else {
                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: info.message,
                                callback: function () {                                    
                                }
                            });
                        }
                    }
                }
            });            
        }
        init();
    }

    function _editCBP() {

        function init() {
            wireupEvent();
        }

        function wireupEvent() {
            var btnedit = $(MEL.toSelector(ID_BTN_EDIT));

            clearForm();

            btnedit.click(function () {
                event.preventDefault();
                var el = $(this);
                el.prop('disabled', true);
                doedit();
                setTimeout(function () { el.prop('disabled', false); }, 3000);                
            })
        }

        function clearForm() {
            var form = $(MEL.toSelector(ID_FORM_EDIT_CBP));
            form.formReset();
        }

        function doedit() {
            var form = $(MEL.toSelector(ID_FORM_EDIT_CBP));
            var btntocanceledit = $(MEL.toSelector(ID_BTN_EDIT_CANCEL));
            
            if (!form.valid()) {
                $('.input-validation-error').first().focus();
                return;
            }

            var dto = MEL.toDTO(form);            

            MEL.load({
                url: URL_EDIT_CBP,
                dto: dto,
                callback: function (info) {
                    console.log(info);
                    if (info.success == false) {
                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: info.exception
                        });
                    } else {
                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: info.message,
                            callback: function () {
                                GetCBPDetails(dto["EditCBPModel.Id"]);
                                btntocanceledit.click();
                                UpdateCalendar();
                            }
                        });
                    }

                }
            });            
        }
        init();
    }

    function _addCbpAppointment() {

        function init() {
            wireupEvent();
        }

        function wireupEvent() {
            var btnadd = $(MEL.toSelector(ID_BTN_ADD_APPOINTMENT));

            //btnadd.off();

            btnadd.click(function () {
                event.preventDefault();
                var el = $(this);
                el.prop('disabled', true);
                doadd();
                setTimeout(function () { el.prop('disabled', false); }, 3000);                   
            })
        }

        function clearForm() {
            var form = $(MEL.toSelector(ID_FORM_ADD_CBP_APPOINTMENT));
            form.formReset();
        }

        function doadd() {
            var ScheduleId = $('.scheduleId').val();
            var form = $(MEL.toSelector(ID_FORM_ADD_CBP_APPOINTMENT));
            var btncancel = $(MEL.toSelector(ID_BTN_ADD_APPOINTMENT_CANCEL));

            if (!form.valid()) {
                $('.input-validation-error').first().focus();
                return;
            }

            var dto = MEL.toDTO(form);
            dto["AddAppointmentModel.StartTime"] = moment(dto["AddAppointmentModel.StartTime"], 'hh:mm A').format('YYYY-MM-DD HH:mm:ss');
            dto["AddAppointmentModel.EndTime"] = moment(dto["AddAppointmentModel.EndTime"], 'hh:mm A').format('YYYY-MM-DD HH:mm:ss');
            dto["AddAppointmentModel.CBPSchedulingId"] = ScheduleId;
            MEL.load({
                url: URL_ADD_CBP_APPOINTMENT,
                dto: dto,
                callback: function (info) {
                    if (info.success == false) {
                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: info.exception
                        });
                    } else if (info.available == false) {
                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: info.message
                        });
                    }else {
                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: info.message,
                            callback: function () {
                                GetCBPDetails(ScheduleId);
                                UpdateCalendar();
                                //btncancel.click();                                                                
                            }
                        });
                    }
                }
            });            
        }
        init();
    }

    function _editCBPAppointment() {

        function init() {
            wireupEvent();
        }

        function wireupEvent() {
            var btnedit = $(MEL.toSelector(ID_BTN_EDIT_APPOINTMENT));

            clearForm();

            btnedit.click(function () {
                event.preventDefault();
                var el = $(this);
                el.prop('disabled', true);
                doedit();
                setTimeout(function () { el.prop('disabled', false); }, 3000);                                   
            })
        }

        function clearForm() {
            var form = $(MEL.toSelector(ID_FORM_EDIT_CBP_APPOINTMENT));
            form.formReset();
        }

        function doedit() {
            var form = $(MEL.toSelector(ID_FORM_EDIT_CBP_APPOINTMENT));
            var btntocanceledit = $(MEL.toSelector(ID_BTN_EDIT_CANCEL_APPOINTMENT));

            if (!form.valid()) {
                $('.input-validation-error').first().focus();
                return;
            }

            var dto = MEL.toDTO(form);

            MEL.load({
                url: URL_EDIT_CBP_APPOINTMENT,
                dto: dto,
                callback: function (info) {
                    console.log(info);
                    if (info.success == false) {
                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: info.exception
                        });
                    } else if (info.available == false) {
                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: info.message
                        });
                    } else {
                        bootbox.alert({
                            centerVertical: true,
                            size: "small",
                            message: info.message,
                            callback: function () {
                                GetCBPDetails(dto["EditAppointmentModel.CBPSchedulingId"]);
                                btntocanceledit.click();
                                UpdateCalendar();
                            }
                        });
                    }
                }
            });            
        }
        init();
    }

    init();
}

function checkDate(id) {
    if (id === 0) {
        startDate = $('#datetimepicker1').datetimepicker("viewDate");
        endDate = $('#datetimepicker2').datetimepicker("viewDate");

        if (startDate != null && startDate != 'undefined') {
            if (endDate < startDate) {
                bootbox.alert({
                    size: "small",
                    title: "Message",
                    message: "Please enter valid End Time.",
                    callback: function () {
                        $('#AddA-EndTime').val("");
                    }
                });
            }
        }
    }
    //else {
    //    startDate = $('#datetimepicker3').datetimepicker("viewDate");
    //    endDate = $('#datetimepicker4').datetimepicker("viewDate");

    //    if (startDate != null && startDate != 'undefined') {
    //        if (endDate < startDate) {
    //            bootbox.alert({
    //                size: "small",
    //                title: "Message",
    //                message: "Please enter valid End Time.",
    //                callback: function () {
    //                    $('#editend').val("");
    //                }
    //            });
    //        }
    //    }
    //}
}

var calendar = null;
function initCalendar() {
    var calendarEl = document.getElementById('calendar');

    var today = new Date();
    var date = today.getFullYear() + '-' + (today.getMonth() + 1) + '-' + today.getDate();
    var AddValue = document.getElementById("AddValue").value;
    console.log(AddValue);

    if (AddValue == "True") {
        calendar = new FullCalendar.Calendar(calendarEl, {
            height: 'auto',
            //plugins: ['interaction', 'dayGrid', 'timeGrid'], with week and day
            plugins: ['interaction', 'dayGrid'], //with month only
            buttonText: {
                today: 'Today',
                month: 'Month',
                //week: 'Week',
                //day: 'Day',
                list: 'List'
            },
            customButtons: {
                create: {
                    text: 'New Schedule',
                    click: function () {
                        $('#AddScheduleModal').modal("show");
                    }
                },
                print: {
                    text: 'Print PDF',
                    click: function () {
                        Downprint();
                    }
                }
            },
            header: {
                right: 'create print',
                center: 'title',
                left: 'dayGridMonth,timeGridWeek,timeGridDay,listMonth prev,next'
            },
            defaultView: 'dayGridMonth',
            defaultDate: today,
            displayEventTime: false,
            navLinks: true, // can click day/week names to navigate views
            businessHours: true, // display business hours
            editable: true,
            //eventSources: [{
            //    url: '/CBPScheduling/GetSchedule',
            //    method: 'GET',
            //    failure: function () {
            //        bootbox.alert({
            //            size: "small",
            //            title: "Error Message",
            //            message: "There was an error when fetching events."
            //        });
            //    }
            //}],
            //eventTimeFormat: { // like '01:30 PM'
            //    hour: '2-digit',
            //    minute: '2-digit',
            //    meridiem: true
            //},
            eventClick: function (info) {
                console.log(info);
                if (info.event.extendedProps.type !== -1) {
                    GetCBPDetails(info.event.id);
                    $('.scheduleId').val(info.event.id);
                    $('#ViewCBPModal').modal('show');
                }

            },
            eventLimit: true, // for all non-TimeGrid views
            views: {
                dayGrid: {
                    eventLimit: 4 // adjust to 2 only for timeGridWeek/timeGridDay
                }
            },
            loading: function (bool) {
                if (bool) {
                    //alert('I am populating the calendar with events');
                    $('#load').show();
                }
                else {
                    //alert('W00t, I have finished!');
                    $('#load').hide();
                    calendar.updateSize()
                }
            },
            eventRender: function (info) {
                info.el.querySelector('.fc-title').innerHTML = info.event.title;
            }
            //eventRender: function (event, element) {
            //    element.find('span.fc-event-title').html(element.find('span.fc-event-title').text());
            //}
        });
    }
    else {
        calendar = new FullCalendar.Calendar(calendarEl, {
            height: 'auto',
            //plugins: ['interaction', 'dayGrid', 'timeGrid'], with week and day
            plugins: ['interaction', 'dayGrid'], //with month only
            buttonText: {
                today: 'Today',
                month: 'Month',
                //week: 'Week',
                //day: 'Day',
                list: 'List'
            },
            customButtons: {
                
                print: {
                    text: 'Print PDF',
                    click: function () {
                        Downprint();
                    }
                }
            },
            header: {
                right: 'create print',
                center: 'title',
                left: 'dayGridMonth,timeGridWeek,timeGridDay,listMonth prev,next'
            },
            defaultView: 'dayGridMonth',
            defaultDate: today,
            displayEventTime: false,
            navLinks: true, // can click day/week names to navigate views
            businessHours: true, // display business hours
            editable: true,
            //eventSources: [{
            //    url: '/CBPScheduling/GetSchedule',
            //    method: 'GET',
            //    failure: function () {
            //        bootbox.alert({
            //            size: "small",
            //            title: "Error Message",
            //            message: "There was an error when fetching events."
            //        });
            //    }
            //}],
            //eventTimeFormat: { // like '01:30 PM'
            //    hour: '2-digit',
            //    minute: '2-digit',
            //    meridiem: true
            //},
            eventClick: function (info) {
                console.log(info);
                if (info.event.extendedProps.type !== -1) {
                    GetCBPDetails(info.event.id);
                    $('.scheduleId').val(info.event.id);
                    $('#ViewCBPModal').modal('show');
                }

            },
            eventLimit: true, // for all non-TimeGrid views
            views: {
                dayGrid: {
                    eventLimit: 4 // adjust to 2 only for timeGridWeek/timeGridDay
                }
            },
            loading: function (bool) {
                if (bool) {
                    //alert('I am populating the calendar with events');
                    $('#load').show();
                }
                else {
                    //alert('W00t, I have finished!');
                    $('#load').hide();
                    calendar.updateSize()
                }
            },
            eventRender: function (info) {
                info.el.querySelector('.fc-title').innerHTML = info.event.title;
            }
            //eventRender: function (event, element) {
            //    element.find('span.fc-event-title').html(element.find('span.fc-event-title').text());
            //}
        });
    }
   
        
   

    calendar.render();

    $("#CalendarFilterLeader").change(UpdateCalendar);
    $("#CalendarFilterLeader").change();
}

var cbpEditDetails = null;

function getAppointmentAndDisplayDetail(item, index) {
    //1Id,2CBPSchedulingId,3ClientId,4ClientName,5ClientAddress,6Kaki1,7Kaki1Name ,8Kaki2 ,9Kaki2Name ,10Kaki3 ,11Kaki3Name,12Kaki4 ,13Kaki4Name,14Kaki5 ,15Kaki5Name,16CBPAppointmentId,
    //17StartTime, 18EndTime, 19Remarks, 20Status, 21RequireTapOut 
    //(ID, AID, CID, Start, End, K1, K2, K3, K4, K5, Remark, Status)
    index['startTime'] = moment(index['startTime']).format(" hh:mm A")
    
    index['endTime'] = moment(index['endTime']).format(" hh:mm A")
    if (index['status'] == "Cancelled") {
        var navtab = "<li class='Appointment'><a data-toggle='tab' href='#Appointment-details' onclick='ShowAppointmentDetail("
            + index['id'] + ",\"" + index['cbpAppointmentId'] + "\"," + index['clientId'] + ",\"" + index['startTime'] + "\",\"" + index['endTime'] +
            "\",\"" + index['kaki1Name'] + "\",\"" + index['kaki2Name'] + "\",\"" + index['kaki3Name'] + "\",\"" + index['kaki4Name'] + "\",\"" + index['kaki5Name'] +
            "\",\"" + index['kaki1'] + "\",\"" + index['kaki2'] + "\",\"" + index['kaki3'] + "\",\"" + index['kaki4'] + "\",\"" + index['kaki5'] +
            "\",\"" + index['remarks'] + "\",\"" + index['status'] + "\",\"" + index['requireTapOut'] + "\");'><del>" + index['clientName'] + "</del></a></li>";
        $('#CBP_Detail_TabList').append(navtab);
    } else {
        var navtab = "<li class='Appointment'><a data-toggle='tab' href='#Appointment-details' onclick='ShowAppointmentDetail("
            + index['id'] + ",\"" + index['cbpAppointmentId'] + "\"," + index['clientId'] + ",\"" + index['startTime'] + "\",\"" + index['endTime'] +
            "\",\"" + index['kaki1Name'] + "\",\"" + index['kaki2Name'] + "\",\"" + index['kaki3Name'] + "\",\"" + index['kaki4Name'] + "\",\"" + index['kaki5Name'] +
            "\",\"" + index['kaki1'] + "\",\"" + index['kaki2'] + "\",\"" + index['kaki3'] + "\",\"" + index['kaki4'] + "\",\"" + index['kaki5'] +
            "\",\"" + index['remarks'] + "\",\"" + index['status'] + "\",\"" + index['requireTapOut'] + "\",\"" + index['feedbackisNull'] + "\",\"" + index['newKaki1'] + "\",\"" + index['newKaki2'] + "\",\"" + index['newKaki3'] + "\",\"" + index['newKaki4'] + "\",\"" + index['newKaki5'] + "\");'>" + index['clientName'] + "</a></li>";
        $('#CBP_Detail_TabList').append(navtab);
    }
}

function GetCBPDetails(id) {
    //clear detail    
    $('#ViewCBPModal .modal-details').each(function () {
        $(this).empty();
    });    
    document.getElementById("tbl_LatestApptDetails").style.display = "none";
    //clear tabs 
    $('#CBP_Detail_TabList').empty();

    $('#Empty-Tab-Link').trigger('click');
    
    //call ajax
    $.ajax({
        url: '/CBPScheduling/Detail?CBBPId=' + id,
        type: 'GET',
        success: function (info) {
            console.log(info);
            if (info.success == true) {                
                //push data to CBP Detail
                var CBPDetail = Object.values(info.data['cbpDetail']); //0 = Id, 1 = GroupLeaderId, 2 = ScheduleDate , 3 = CBPScheduleId(string),4 = GroupLeaderName , 5 = Remarks , 6 = Status            

                //if status is cancelled hide button                
                if (CBPDetail[6] == "Cancelled") {
                    $("#edit").hide();
                    $("#cancel").hide();                    
                } else {
                    $("#edit").show();
                    $("#cancel").show();
                }

                //Convert DateTime Format
                //CBPDetail[2] = moment(parseInt((CBPDetail[2]).match(/\d+/)[0])).format('DD/MM/YYYY');
                
                
                var j = 2; //b'cuz have id and group leader id so start with 2
                $('#Schedule_Detail  .modal-details').each(function () {
                    $(this).append(CBPDetail[j]);
                    j++;
                });
                $('#GroupLeaderId').val(CBPDetail[1]);

                //Update Edit Schedule Model detail
                GetEditCBPScheduleDetail();

                //erase all appointments first
                $('#CBP_Detail_TabList .Appointments').remove();

                //push data to CBP Appointment
                //1Id,2CBPSchedulingId,3ClientId,4ClientName,5ClientAddress,6Kaki1,7Kaki1Name ,8Kaki2 ,9Kaki2Name ,10Kaki3 ,11Kaki3Name,12Kaki4 ,13Kaki4Name,14Kaki5 ,15Kaki5Name,16CBPAppointmentId,17StartTime,18EndTime,19Remarks,20Status,21RequireTapOut 
                var CBPDetailAppointment = Object.values(info.data['cbpAppointments']);
                if (CBPDetailAppointment != null && CBPDetail.length > 1) {                    
                    $(CBPDetailAppointment).each(getAppointmentAndDisplayDetail);
                    //alert(CBPDetailAppointment.length);
                    //set maximum of the appointment to be 8 then stop showing the add appointment tab.
                    if (CBPDetailAppointment.length < 8 && CBPDetail[6] != "Cancelled") {
                        var navtab = "<li class='Appointment' onclick='ClickAddAppointment();' id='CBP_Detail_Tab'><a data-toggle='tab' href='#Add_Appointment_Tab' id='CBP_Detail_Add_Appointment_Tab'>Add Appointment</a></li>";
                        $('#CBP_Detail_TabList').append(navtab);
                    }                    
                }
            } else {
                bootbox.alert({
                    centerVertical: true,
                    size: "small",
                    message: info.exception
                });
            }
        }
    })
}


//set data to edit field
//function setDatatoInput(value) {
    
//    var x = 0;
//    $("#form-delete-volunteer input").val(value[x]);
//    $('#edit-cbp-details input').each(function () {
//        $(this).val(value[x]);
//        x++;
//    });
//    $('#edit-cbp-details textarea').each(function () {
//        $(this).val(value[x]);
//        x++;
//    });

//}

function GetEditCBPScheduleDetail() {
    var groupleaderId = $('#GroupLeaderId').val();
    var scheduleDate = $('#CBP_Detail_Date').text();
    var scheduleRemarks = $('#CBP_Detail_Remarks').text();
    $('#edit-groupleader-field').val(groupleaderId);
    $('#edit-date-field').val(scheduleDate);
    $('#edit-remark-field').val(scheduleRemarks);    
}

function ShowAppointmentDetail(ID, AID, CID, Start, End, K1N, K2N, K3N, K4N, K5N, K1, K2, K3, K4, K5, Remark, Status, TapOut, FeebackisNull, newKaki1, newKaki2, newKaki3, newKaki4, newKaki5) {
    //reset add and edit form
    var Editform = $(MEL.toSelector('form-edit-cbp-appointment'));
    Editform.formReset();

    //("#tbl_LatestApptDetails").style.display = "none";
    document.getElementById("tbl_LatestApptDetails").style.display = "none";

    if (newKaki1 != "" || newKaki2 != "" || newKaki3 != "" || newKaki4 != "" || newKaki5 != "") {
        document.getElementById("tbl_LatestApptDetails").style.display = "block";
        $('.latestCBPAppt_Kaki1').each(function () {
                $(this).empty();
            });
        $('.latestCBPAppt_Kaki2').each(function () {
                $(this).empty();
            });
        $('.latestCBPAppt_Kaki3').each(function () {
                $(this).empty();
            });
        $('.latestCBPAppt_Kaki4').each(function () {
                $(this).empty();
            });
        $('.latestCBPAppt_Kaki5').each(function () {
                $(this).empty();
            });
        $(".latestCBPAppt_Kaki1").append(newKaki1);
        $(".latestCBPAppt_Kaki2").append(newKaki2);
        $(".latestCBPAppt_Kaki3").append(newKaki3);
        $(".latestCBPAppt_Kaki4").append(newKaki4);
        $(".latestCBPAppt_Kaki5").append(newKaki5);
    }



    //hide button if status is cancelled
    if (Status == "Cancelled") {
        $("#CBP_Cancel_Appointment_BTN").hide();
        $("#CBP_Edit_Appointment_BTN").hide();
    } else {
        $("#CBP_Cancel_Appointment_BTN").show();
        $("#CBP_Edit_Appointment_BTN").show();
    }

    //clear appointment detail.
    $('.appointment-details').each(function () {
        $(this).empty();
    });    

    //alert(ID + ' | ' + AID + ' | ' + CID + ' | ' + Start + ' | ' + End + ' | ' + K1 + ' | ' + K2 + ' | ' + K3 + ' | ' + K4 + ' | ' + K5 + ' | ' + Remark + ' | ' + Status);
    $('#ClientId').append(CID);
    $('#CBP_Appointment_Id').append(AID);
    $('#CBP_Appointment_Time').append(Start + ' - ' + End);
    $('#CBP_Appointment_Kaki1Name').append(K1N);
    $('#CBP_Appointment_Kaki2Name').append(K2N);
    $('#CBP_Appointment_Kaki3Name').append(K3N);
    $('#CBP_Appointment_Kaki4Name').append(K4N);
    $('#CBP_Appointment_Kaki5Name').append(K5N);
    if (Remark != 'null') {        
        $('#CBP_Appointment_Remarks').append(Remark);
    }
    $('#CBP_Appointment_Status').append(Status);
    $('.appointmentId').val(ID);
    if (TapOut == 1) {
        $('#CBP_Appointment_TapOut').append("Yes");
        $('#EditA-Yes').prop('checked', true);
        $('#EditA-No').prop('checked', false);
    } else {
        $('#CBP_Appointment_TapOut').append("No");
        $('#EditA-No').prop('checked', true);
        $('#EditA-Yes').prop('checked', false);
    }
   
    //update Edit model value
    $("#EditA-ClientId").val(CID);
    if (Remark != 'null') {            
        $("#EditA-Remark").val(Remark);
    }    
    $("#EditA-StartTime").val(Start);
    $("#EditA-EndTime").val(End);
    if (K1 != "null") {
        $("#EditA-Kaki1").val(K1);
    }
    if (K2 != "null") {
        $("#EditA-Kaki2").val(K2);
    }
    if (K3 != "null") {
        $("#EditA-Kaki3").val(K3);
    }
    if (K4 != "null") {
        $("#EditA-Kaki4").val(K4);
    }
    if (K5 != "null") {
        $("#EditA-Kaki5").val(K5);
    }

    $("#EditCBPAppointmentModal").on('hide.bs.modal', function () {
        //update Edit model value
        $("#EditA-ClientId").val(CID);
        $("#EditA-Remark").val(Remark);
        $("#EditA-StartTime").val(Start);
        $("#EditA-EndTime").val(End);
        if (K1 != "null") {
            $("#EditA-Kaki1").val(K1);
        }
        if (K2 != "null") {
            $("#EditA-Kaki2").val(K2);
        }
        if (K3 != "null") {
            $("#EditA-Kaki3").val(K3);
        }
        if (K4 != "null") {
            $("#EditA-Kaki4").val(K4);
        }
        if (K5 != "null") {
            $("#EditA-Kaki5").val(K5);
        }
    });

    $('#ViewFeedback').css("visibility", "visible");
    if (FeebackisNull == 1) {
        //$('#ViewFeedback').style.visibility = 'hidden';
        //document.getElementById('ViewFeedback').style.visibility = 'hidden';
        $('#ViewFeedback').css("visibility", "hidden");
        $('#CBP_Appointment_FeeedbackStatus').append("Incomplete");
    }
    else {
        $('#CBP_Appointment_FeeedbackStatus').append("Completed");
    }
    $('#ViewFeedback').click(function () {
        window.location = '/CBPScheduling/GetFeedbackDetails?ID='+ ID;
    });

}

function ClickAddAppointment() {
    var Addform = $(MEL.toSelector(ID_FORM_ADD_CBP_APPOINTMENT));
    Addform.formReset();
    $("#AddAppointmentRadioYes").attr('checked', true);
    document.getElementById("tbl_LatestApptDetails").style.display = "none";
}

function DeleteCBPSchedule() {

    var ScheduleId = $('.scheduleId').val();
    var StringScheduleId = $('#CBP_Detail_CBPScheduleId').text();
    var dto = {
        CBPId: ScheduleId
    }

    var message = "Are you confirm to delete schedule with ID '" + StringScheduleId + "'?"

    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: message,
        buttons: {
            cancel: {
                label: 'No'
            },
            confirm: {
                label: "Yes"
            }
        },
        callback: function (result) {
            if (result) {
                MEL.load({
                    url: URL_DELETE_CBP,
                    dto: dto,
                    callback: function (info) {
                        if (info.Success == false) {
                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: info.Exception
                            });
                        } else {
                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: info.message,
                                callback: function () {
                                    $('#ViewCBPModal').modal('hide');
                                    UpdateCalendar();
                                }
                            });
                            
                        }
                    }
                })
            }
        }
    });
}

function CancelCBPSchedule() {

    var ScheduleId = $('.scheduleId').val();
    var StringScheduleId = $('#CBP_Detail_CBPScheduleId').text();
    var dto = {
        CBPId: ScheduleId
    }

    var message = "Are you confirm to cancel schedule with ID '" + StringScheduleId + "'?"

    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: message,
        buttons: {
            cancel: {
                label: 'No'
            },
            confirm: {
                label: "Yes"
            }
        },
        callback: function (result) {
            if (result) {
                MEL.load({
                    url: URL_CANCEL_CBP,
                    dto: dto,
                    callback: function (info) {
                        if (info.success == false) {
                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: info.exception
                            });
                        } else {
                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: info.message,
                                callback: function () {
                                    $('#ViewCBPModal').modal('hide');
                                    UpdateCalendar();
                                }
                            });                            
                        }
                    }
                })
            }
        }
    });
}

function DeleteCBPAppointment() {

    var CBPAppointmentId = $('.appointmentId').val();
    var StringAppointmentId = $('#CBP_Appointment_Id').text();
    var dto = {
        CBPAppointmentId: CBPAppointmentId
    }

    var message = "Are you confirm to delete Appointment with ID '" + StringAppointmentId + "'?"

    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: message,
        buttons: {
            cancel: {
                label: 'No'
            },
            confirm: {
                label: "Yes"
            }
        },
        callback: function (result) {
            if (result) {
                MEL.load({
                    url: URL_DELETE_CBP_APPOINTMENT,
                    dto: dto,
                    callback: function (info) {
                        if (info.success == false) {
                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: info.exception
                            });
                        } else {
                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: info.message,
                                callback: function () {    
                                    var ScheduleId = $('.scheduleId').val();
                                    GetCBPDetails(ScheduleId);
                                    UpdateCalendar();
                                }
                            });                            
                        }
                    }
                })
            }
        }
    });
}

function CancelCBPAppointment() {

    var CBPAppointmentId = $('.appointmentId').val();
    var StringAppointmentId = $('#CBP_Appointment_Id').text();
    var dto = {
        CBPAppointmentId: CBPAppointmentId
    }

    var message = "Are you confirm to Cancel Appointment with ID '" + StringAppointmentId + "'?"

    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: message,
        buttons: {
            cancel: {
                label: 'No'
            },
            confirm: {
                label: "Yes"
            }
        },
        callback: function (result) {
            if (result) {
                MEL.load({
                    url: URL_CANCEL_CBP_APPOINTMENT,
                    dto: dto,
                    callback: function (info) {
                        if (info.success == false) {
                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: info.exception
                            });
                        } else {
                            bootbox.alert({
                                centerVertical: true,
                                size: "small",
                                message: info.message,
                                callback: function () {
                                    $('#ViewCBPModal').modal('hide');
                                    UpdateCalendar();
                                }
                            });                            
                        }
                    }
                })
            }
        }
    });
}

function ViewCBPAttendance() {
    var CBPAppointmentId = $('.appointmentId').val();
    var CBPScheduleId = $('.scheduleId').val();
    $.ajax({
        url: '/CBPAttendance/CBPAppointmentDetails',
        data: {
            CBPSchedulingId: CBPScheduleId,
            CBPAppointmentsId: CBPAppointmentId

        },
        type: 'POST',
        success: function (data) {
            $("#ViewCBPModal").modal('hide');
            $("#ViewCBPAttendanceProgress").modal('show');
            //console.log(data);
            $(".Attendance_ClientName").empty();
            $(".Attendance_ClientName").append(data.clientName);
            if (data.clientCheckInStatus == 0) {
                document.getElementById("Attendance_Client_CI_NO").style.display = "block";
                document.getElementById("Attendance_Client_CI_YES").style.display = "none";
            } else {
                document.getElementById("Attendance_Client_CI_NO").style.display = "none";
                document.getElementById("Attendance_Client_CI_YES").style.display = "block";
            }
            if (data.clientCheckOutStatus == 0) {
                document.getElementById("Attendance_Client_CO_NO").style.display = "block";
                document.getElementById("Attendance_Client_CO_YES").style.display = "none";
            } else {
                document.getElementById("Attendance_Client_CO_NO").style.display = "none";
                document.getElementById("Attendance_Client_CO_YES").style.display = "block";
            }

            //adding kakis attendance status to table body
            var table = document.getElementById("kakiAttendanceList");
            $("#kakiAttendanceList tr").remove();
            var ddlkakilist = data.ddlKakiList;

            for (var i = 0; i < ddlkakilist.length; i++) {
                var row = document.createElement("tr");
                table.appendChild(row);
                var cell1 = row.insertCell(0);
                cell1.innerHTML = '<td>' + ddlkakilist[i].kakiName + '</td>';

                var cell2 = row.insertCell(1);
                if (ddlkakilist[i].checkInStatus == 1) {
                    cell2.innerHTML = '<p class="glyphicon glyphicon-ok" style="color:greenyellow"></p>';
                } else {
                    cell2.innerHTML = '<p class="glyphicon glyphicon-remove" style="color:red"></p>';
                }

                var cell3 = row.insertCell(2);
                if (ddlkakilist[i].checkOutStatus == 1) {
                    cell3.innerHTML = '<p class="glyphicon glyphicon-ok" style="color:greenyellow"></p>';
                } else {
                    cell3.innerHTML = '<p class="glyphicon glyphicon-remove" style="color:red"></p>';
                }


                
            }
        }
    });
}

function onchangeStartTime() {    
    var StartTime= moment($('#AddA-StartTime').val(), "hh:mm A").format("hh:mm A");    
    var EndTime = moment($('#AddA-StartTime').val(), "hh:mm A").add(1, 'hours').format("hh:mm A");    
    $('#AddA-StartTime').val(StartTime).change();
    $('#AddA-EndTime').val(EndTime);
    $('#AddA-EndTime').change();
}

function UpdateCalendar() {
    var selectedLeader = $('#CalendarFilterLeader').children("option:selected").val()
    if (selectedLeader == "") {
        calendar.removeAllEventSources()
        calendar.addEventSource("/CBPScheduling/GetSchedule")
    }
    else {
        calendar.removeAllEventSources()
        calendar.addEventSource("/CBPScheduling/GetSchedule?CBPLeaderId=" + selectedLeader);
    }
}
//Print Calendar to PDF
function Downprint() {
    html2canvas(document.getElementById("calendar")).then(function (canvas) {
        var pdf = new jsPDF('l', 'mm', 'a4');
        pdf.addImage(canvas.toDataURL('image/png'), 'PNG', 10, 10, 277, 190);
        pdf.save('CBPSchedulePDF.pdf');
    });
}
