/// <reference path="../../Assets/Plugins/extended-user-identity/mel.core.js" />
$(window).on("load", function () {
    initCalendar();
});
var calendar = null;
function initCalendar() {
    var calendarEl = document.getElementById('calendar');
    var today = new Date();
    var date = today.getFullYear() + '-' + (today.getMonth() + 1) + '-' + today.getDate();
    var AddValue = document.getElementById("AddValue").value;

    if (AddValue == "True") {
        calendar = new FullCalendar.Calendar(calendarEl, {
            height: 'auto',
            contentHeight: 600,
            plugins: ['interaction', 'dayGrid'],
            buttonText: {
                today: 'Today',
                month: 'Month',
                //week: 'Week',
                //day: 'Day',
                list: 'List'
            },
            customButtons: {
                create: {
                    text: 'Add New Ad-Hoc Appointment',
                    click: function () {
                        $('#AddAdHocApptModal').modal("show");
                    }
                },
                //print: {
                //    text: 'Print PDF',
                //    click: function () {
                //        Downprint();
                //        //UpdateCalendar();
                //    }
                //}
            },
            header: {
                right: 'create print',
                center: 'title',
                left: 'dayGridMonth,timeGridWeek,timeGridDay,listMonth prev,next'
            },
            defaultView: 'dayGridMonth',
            defaultDate: today,
            displayEventTime: true,
            navLinks: true, // can click day/week names to navigate views
            businessHours: true,
            editable: false,
            eventTimeFormat: { // like '01:30 PM'
                hour: '2-digit',
                minute: '2-digit',
                meridiem: true
            },
            eventSources: [{
                url: '/CBP_AdHocScheduling/GetAppointments',
                method: 'GET',
                failure: function () {
                    bootbox.alert({
                        size: "small",
                        title: "Error Message",
                        message: "There was an error when fetching events."
                    });
                }
            }],
            eventClick: function (info) {
                //console.log(info.event.id);
                UpdateAppointmentDetails(info.event.id)

                $('#ViewAppointmentModal').modal('show');
                $('.apptId').val(info.event.id);
            },
            eventRender: function (info) {
                info.el.querySelector('.fc-title').innerHTML = info.event.title;
            }

        });
    }
    else {
        calendar = new FullCalendar.Calendar(calendarEl, {
            height: 'auto',
            contentHeight: 600,
            plugins: ['interaction', 'dayGrid'],
            buttonText: {
                today: 'Today',
                month: 'Month',
                //week: 'Week',
                //day: 'Day',
                list: 'List'
            },
            customButtons: {
                //create: {
                //    text: 'Add New Ad-Hoc Appointment',
                //    click: function () {
                //        $('#AddAdHocApptModal').modal("show");
                //    }
                //},
                //print: {
                //    text: 'Print PDF',
                //    click: function () {
                //        Downprint();
                //        //UpdateCalendar();
                //    }
                //}
            },
            header: {
                right: 'create print',
                center: 'title',
                left: 'dayGridMonth,timeGridWeek,timeGridDay,listMonth prev,next'
            },
            defaultView: 'dayGridMonth',
            defaultDate: today,
            displayEventTime: true,
            navLinks: true, // can click day/week names to navigate views
            businessHours: true,
            editable: false,
            eventTimeFormat: { // like '01:30 PM'
                hour: '2-digit',
                minute: '2-digit',
                meridiem: true
            },
            eventSources: [{
                url: '/CBP_AdHocScheduling/GetAppointments',
                method: 'GET',
                failure: function () {
                    bootbox.alert({
                        size: "small",
                        title: "Error Message",
                        message: "There was an error when fetching events."
                    });
                }
            }],
            eventClick: function (info) {
                //console.log(info.event.id);
                UpdateAppointmentDetails(info.event.id)

                $('#ViewAppointmentModal').modal('show');
                $('.apptId').val(info.event.id);
            },
            eventRender: function (info) {
                info.el.querySelector('.fc-title').innerHTML = info.event.title;
            }

        });
    }
    
    calendar.render();

}
function UpdateAppointmentDetails(apptId) {
    $('#ViewAppointmentModal .appointmentmodal-details').each(function () {
        $(this).empty();
    });
    try {
        $.ajax({
            url: 'CBP_AdHocScheduling/GetDetail?Id=' + apptId,
            type: 'GET',
            success: function (Json) {
                

                var data = Json.data;
                console.log(data);
                //console.log(data.StartTime);
                //console.log(formatAMPM(data.StartTime))
                //console.log(data.StartTime);
                //var start = new Date(parseInt((data.startTime).match(/\d+/)[0]))
                var start = new Date(data.bookingDate);
                //var end = new Date(parseInt((data.EndTime).match(/\d+/)[0]))
                $("#Activity").append(data.activityName);
                $('#ApptStringId').append(data.appointmentID);
                $("#Date").append($.datepicker.formatDate('dd M yy', start));
                $("#Time").append(moment(data.startTime).format("DD/MM/yyyy hh:mm A") + " - " + moment(data.endTime).format("DD/MM/yyyy hh:mm A"));
                

                $("#Client").append(data.clientName);
                $("#Kaki1").append(data.kaki1);
                $("#Kaki2").append(data.kaki2);
                $("#Kaki3").append(data.kaki3);
                $("#Kaki4").append(data.kaki4);
                $("#Kaki5").append(data.kaki5);

                $("#Remarks").append(data.remarks);
                $("#Status").append(data.status);
                $("#FeedbackStatus").append(data.feedbackStatus);
                if (data.feedbackStatus == "Completed") {
                    document.getElementById("btnViewFeedback").style.visibility = "visible";
                } else {
                    document.getElementById("btnViewFeedback").style.visibility = "hidden";
                }
                //$("#bookingIdEdit").val(bookingid);
                ////console.log(moment(start).format('DD/MM/YYYY'));
                //$("#edit-bookingdate").val(moment(start).format('DD/MM/YYYY'));
                //$("#edit-booking-starttime").val(moment(start).format('hh:mm A'));

                //$("#edit-booking-endtime").val(moment(end).format('hh:mm A'));
                //$("#edit-booking-purpose").val(data.Purpose);

                //var editactivity = document.getElementById('edit-booking-activity');
                //editactivity.selectedIndex = 0;
                //getSelectedOption(editactivity, data.ActivityName);

                //var editLocation = document.getElementById('edit-booking-location');
                //editLocation.selectedIndex = 0;
                //getSelectedOption(editLocation, data.LocationName);

                //UpdateEditSublocation()
                //var editArea = document.getElementById('edit-booking-sublocation');
                //editArea.selectedIndex = 0;
                //getSelectedOption(editArea, data.Sublocation);

                //var editPIC = document.getElementById('edit-booking-pic');
                //editPIC.selectedIndex = 0;
                //console.log(data.PersonInCharge);
                //getSelectedOption(editPIC, data.PersonInCharge);

                //$("#edit-booking-remark").val(data.Remarks);
                //if (data.Status == "Cancelled" || data.Status == "Completed") {

                //    $("#delete").hide();
                //    $("#cancel").hide();
                //    $("#edit").hide();
                //    $("#btnExtendBooking").hide();
                //    $("#btnCloseBooking").hide();
                //}
                //if (data.Status == "Cancelled") {
                //    $("#btnExtendBooking").hide();
                //}
            }
        })
    } catch (e) {
        console.log(e);
    }

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
}

function onchangeStartTime() {
    var StartTime = moment($('#AddA-StartTime').val(), "hh:mm A").format("hh:mm A");
    var EndTime = moment($('#AddA-StartTime').val(), "hh:mm A").format("hh:mm A");
    $('#AddA-StartTime').val(StartTime).change();
    $('#AddA-EndTime').val(EndTime);
    $('#AddA-EndTime').change();
}

function getSubSelectedOption(sel, datatext) {
    var dataarray = datatext.split(' <br> ');
    var opt;
    for (var j = 0, len = dataarray.length; j < len; j++) {
        for (var i = 0, len = sel.options.length; i < len; i++) {
            opt = sel.options[i];
            if (opt.text == dataarray[j]) {
                opt.selected = true;
            }
        }
    }
}
function getSelectedOption(sel, datatext) {
    var opt;
    for (var i = 0, len = sel.options.length; i < len; i++) {
        opt = sel.options[i];
        if (opt.text == datatext) {
            opt.selected = true;
        }
    }
}
function formatAMPM(date) {
    var date = new Date();
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var DATE = date.getDate();
    var month = date.getMonth();
    switch (month) {
        case 0: var monthname = 'Jan'; break;
        case 1: var monthname = 'Feb'; break;
        case 2: var monthname = 'Mar'; break;
        case 3: var monthname = 'Apr'; break;
        case 4: var monthname = 'May'; break;
        case 5: var monthname = 'Jun'; break;
        case 6: var monthname = 'Jul'; break;
        case 7: var monthname = 'Aug'; break;
        case 8: var monthname = 'Sep'; break;
        case 9: var monthname = 'Oct'; break;
        case 10: var monthname = 'Nov'; break;
        case 11: var monthname = 'Dec'; break;
        default:
        // code block
    }
    var year = date.getFullYear();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = DATE + ' ' + monthname + ' ' + year + ' ' + hours + ':' + minutes + ' ' + ampm;
    return strTime;
}
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
function OnSuccess(result) {
    //console.log(result);
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
                    b.textContent = Swal.getTimerLeft()
                }, 100)
            },
            willClose: () => {
                clearInterval(timerInterval)
            }
        }).then((result) => {
            if (result.dismiss === Swal.DismissReason.timer) {
                $('#AddCBP_AdHocActivity').modal('hide');
                window.location.reload();
            }
        })

    } else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: result.data.message,
        }).then((result) => {
            return;
        })
        
    }

}
function OnFailure(result) {
    console.log(result);
    console.log("fail");
}
function DeleteAppointment() {
    var appointmentId = $(".apptId").val();
    Swal.fire({
        title: 'Do you want to delete this appointment?',
        showCancelButton: true,
        confirmButtonText: 'Confirm',
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            $.ajax({
                url: '/CBP_AdHocScheduling/DeleteAppointment?Id=' + appointmentId,
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
                                b.textContent = Swal.getTimerLeft()
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

function ViewFeedback() {
    var appointmentId = $(".apptId").val();
    $.ajax({
        url: '/CBP_AdHocScheduling/GetFeedback?Id=' + appointmentId,
        type: 'POST',
        success: function (Json) {
            var data = Json.data;
            console.log(data);
            $('#ViewFeedbackModal .feedbackmodal-details').each(function () {
                $(this).empty();
            });
            $('#ViewAppointmentModal').modal('hide');
            $('#ViewFeedbackModal').modal('show');

            $("#AdHocAppt_Feedback_Mood").append(data.mood);
            $("#AdHocAppt_Feedback_VisitType").append(data.visitType);
            if (data.airCirculation == "") {
                $("#AdHocAppt_Feedback_HouseAirCirculation").append("-");
            } else {
                $("#AdHocAppt_Feedback_HouseAirCirculation").append(data.airCirculation);
            }
            
            $("#AdHocAppt_Feedback_HouseBrightness").append(data.brightness);
            $("#AdHocAppt_Feedback_HouseCleanliness").append(data.cleanliness);
            if (data.signofWorsening == "") {
                $("#AdHocAppt_Feedback_SignofWorsening").append("-");
            } else {
                $("#AdHocAppt_Feedback_SignofWorsening").append(data.signofWorsening);
            }
            if (data.remark == "" || data.remark == null) {
                $("#AdHocAppt_Feedback_Remark").append("-");
            } else {
                $("#AdHocAppt_Feedback_Remark").append(data.remark);
            }
            
            $("#AdHocAppt_Feedback_CreatedBy").append(data.createdBy);
            $("#AdHocAppt_Feedback_SpecialRequest").append(data.specialRequest);
            $("#AdHocAppt_Feedback_SpecialRequestRemark").append(data.specialRequest_Remark);
        }
    });

}