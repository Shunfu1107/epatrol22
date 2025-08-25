/// <reference path="../../Assets/Plugins/extended-user-identity/mel.core.js" />

$(window).on("load", function () {
    initCalendar();
});

var calendar = null;
function UpdateCalendar() {
    var selectedLocation = $("#CalendarFilterLocation").children("option:selected").val()
    var selectedArea = $("#CalendarFilterArea").children("option:selected").val()
    //console.log(calendar);
    if (selectedArea == 0) {
        calendar.removeAllEventSources()
        calendar.addEventSource("/FacilityBooking/GetBookings")
    }
    else {
        calendar.removeAllEventSources()
        calendar.addEventSource("/FacilityBooking/GetBookings?Area=" + selectedArea + "&Location=" + selectedLocation);
    }

    calendar.removeAllEventSources();
    calendar.addEventSource("/FacilityBooking/GetBookings");
}
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
                    text: 'New Booking',
                    click: function () {
                        $('#AddBookingModal').modal("show");
                    }
                },
                print: {
                    text: 'Print PDF',
                    click: function () {
                        Downprint();
                        //UpdateCalendar();
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
                url: '/FacilityBooking/GetBookings',
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
                //alert('You Clicked an event');
                /*alert(info.event.id);*/
                if (info.event.extendedProps.type !== -1) {
                    //console.log(info.event.id);
                    UpdateBookingDetail(info.event.id);

                    $('#ViewBookingModal').modal('show');
                    $('.bookingId').val(info.event.id);
                } else {
                    //console.log(info.event.id);
                    UpdateCentreEventDetail(info.event.id);
                    $('#CentreScheduleModal').modal('show');
                }
                //console.log(info);

            },
            eventRender: function (info) {
                //console.log(info);
                info.el.querySelector('.fc-title').innerHTML = info.event.title;
                //info.el.querySelector('.fc-time').innerHTML = info.event.start;
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
                url: '/FacilityBooking/GetBookings',
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
                //alert('You Clicked an event');
                /*alert(info.event.id);*/
                if (info.event.extendedProps.type !== -1) {
                    //console.log(info.event.id);
                    UpdateBookingDetail(info.event.id);

                    $('#ViewBookingModal').modal('show');
                    $('.bookingId').val(info.event.id);
                } else {
                    //console.log(info.event.id);
                    UpdateCentreEventDetail(info.event.id);
                    $('#CentreScheduleModal').modal('show');
                }
                //console.log(info);

            },
            eventRender: function (info) {
                //console.log(info);
                info.el.querySelector('.fc-title').innerHTML = info.event.title;
                //info.el.querySelector('.fc-time').innerHTML = info.event.start;
            }

        });
    }
    
    calendar.render();
    //calendar.updateSize();

    $("#AddEditBookingModel_LocationId").change(function () {
        var selectedLocation = $(this).children("option:selected").val();
        if (selectedLocation > 0) {
            var Sublocations = LocationNSublocations.find(location => location.id == selectedLocation).SubLocations;
            //    $('#sublocationList').empty();
            //    $('#sublocationList').multiselect('rebuild');
            //    $.each(Sublocations, function (index, value) {
            //        $("#sublocationList").append('<option value="' + value.id + '">' + value.Name + '</option>');

            //    });
            //    $('#sublocationList').multiselect('rebuild');
            //} else {
            //    $('#sublocationList').empty();
            //    $('#sublocationList').multiselect('rebuild');
            if (Sublocations != null) {
                $("#AddEditBookingModel_Sublocation").find('option').not(':first').remove();
                $.each(Sublocations, function (index, value) {
                    $("#AddEditBookingModel_Sublocation").append('<option value="' + value.id + '">' + value.Name + '</option>');
                });
            }

        }
    });
    $("#CalendarFilterLocation").change(function () {
        var selectedLocation = $(this).children("option:selected").val();
        if (selectedLocation > 0) {
            var Sublocations = LocationNSublocations.find(location => location.id == selectedLocation).SubLocations;
            if (Sublocations != null) {
                $("#CalendarFilterArea").find('option').not(':first').remove();
                $.each(Sublocations, function (index, value) {
                    $("#CalendarFilterArea").append('<option value="' + value.id + '">' + value.Name + '</option>');
                });
            }
            calendar.removeAllEventSources();
            calendar.addEventSource("/FacilityBooking/GetBookings?Area=0&Location=" + selectedLocation);
        }
        else {
            $("#CalendarFilterArea").find('option').not(':first').remove();
            calendar.removeAllEventSources()
            calendar.addEventSource("/FacilityBooking/GetBookings")
        }
    });
    $("#CalendarFilterArea").change(function () {
        var selectedLocation = $("#CalendarFilterLocation").children("option:selected").val()
        var selectedArea = $(this).children("option:selected").val()
        if (selectedArea == "") {
            calendar.removeAllEventSources()
            calendar.addEventSource("/FacilityBooking/GetBookings")
        }
        else {
            calendar.removeAllEventSources()
            calendar.addEventSource("/FacilityBooking/GetBookings?Area=" + selectedArea + "&Location=" + selectedLocation);
        }

    });

}

function UpdateBookingDetail(bookingid) {
    //clear detail    
    $('#ViewBookingModal .bookingmodal-details').each(function () {
        $(this).empty();
    });

    //$('#EditBookingModal .editbookingmodal-details').each(function () {
    //    $(this).empty();
    //});

    try {
        $.ajax({
            url: 'FacilityBooking/Detail?Id=' + bookingid,
            type: 'GET',
            success: function (Json) {
                console.log(Json);
                $("#delete").show();
                $("#cancel").show();
                $("#edit").show();
                $("#btnExtendBooking").show();
                $("#btnCloseBooking").show();
                var data = Json.bookingSchedule;
                //console.log(data);
                //console.log(formatAMPM(data.StartTime))
                //console.log(data.StartTime);
                var start = new Date(data.startTime)
                console.log(formatAMPM(start))
                var end = new Date(data.endTime)
                //console.log(formatDataDate(end))
                $("#ActivityName").append(data.activityName);
                $('#BookingStringId').append(data.bookingId);
                $("#Date").append($.datepicker.formatDate('dd M yy', start));
                $("#Purpose").append(data.purpose);
                $("#Time").append(formatDataDate(start) + " - " + formatDataDate(end));
                $("#Location").append(data.locationName);
                $("#Area").append(data.sublocation);
                $("#Remarks").append(data.remarks);
                $("#PIC").append(data.personInCharge);
                $("#Status").append(data.status);
                $("#BookedById").append(data.bookedBy);

                $("#bookingIdEdit").val(bookingid);
                //console.log(moment(start).format('DD/MM/YYYY'));
                $("#edit-bookingdate").val(moment(start).format('DD/MM/YYYY'));
                $("#edit-booking-starttime").val(moment(start).format('hh:mm A'));
                
                $("#edit-booking-endtime").val(moment(end).format('hh:mm A'));
                $("#edit-booking-purpose").val(data.purpose);

                var editactivity = document.getElementById('edit-booking-activity');
                editactivity.selectedIndex = 0;
                getSelectedOption(editactivity, data.activityName);

                var editLocation = document.getElementById('edit-booking-location');
                editLocation.selectedIndex = 0;
                getSelectedOption(editLocation, data.locationName);

                UpdateEditSublocation()
                var editArea = document.getElementById('edit-booking-sublocation');
                editArea.selectedIndex = 0;
                getSelectedOption(editArea, data.sublocation);

                var editPIC = document.getElementById('edit-booking-pic');
                editPIC.selectedIndex = 0;
                //console.log(data.PersonInCharge);
                getSelectedOption(editPIC, data.personInCharge);

                $("#edit-booking-remark").val(data.remarks);
                if (data.status == "Cancelled" || data.status == "Completed") {
                    
                    $("#delete").hide();
                    $("#cancel").hide();
                    $("#edit").hide();
                    $("#btnExtendBooking").hide();
                    $("#btnCloseBooking").hide();
                }
                if (data.status == "Cancelled") {
                    $("#btnExtendBooking").hide();
                }

                $("#scheduleIdTrainee").val(Json.scheduleId);

                TraineeListTable(Json.traineeEventList);
            }
        })
    } catch (e) {
        console.log(e);
    }

}

function TraineeListTable(data) {
    console.log(data);
    var p = $('#TraineeTable').DataTable();
    p.clear();
    p.draw();
    var counter = 1;
    $.each(data, function (index, value) {

        p.row.add([counter, value.name, "*****" + value.nric.substring(value.nric.length - 4), value.groupName, value.lane, value.status, "<a href='#' onclick='EditTrainee(" + value.userId + ", " + value.scheduleId + ")'>Edit</a>", "<a href='#' onclick='DeleteTrainee(" + value.userId + ", " + value.scheduleId + ", \"" + value.name + "\")'>Delete</a>"]);
        
        p.on('order.dt search.dt', function () {
            p.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();
    });
   
}


function UpdateCentreEventDetail(scheduleid) {
    $('#CentreScheduleModal .centremodal-details').each(function () {
        $(this).empty();
    });
    $.ajax({
        url: '/Scheduling/detail?scheduleId=' + scheduleid,
        type: 'GET',
        success: function (Json) {
            var data = Json.scheduleDetailModel;
            //console.log(data);
            //var today = new Date();
            var totalo = data.organizationNameS.length;
            var start = new Date(data.startTime)
            //console.log(start);
            var end = new Date(data.endTime)
            //console.log(end);
            $("#ctr_Date").append($.datepicker.formatDate('dd M yy', start));
            $("#ctr_Time").append(formatDataDate(start) + " - " + formatDataDate(end));
            //$("#EditSchedule_Date").val(moment(start).format('DD/MM/YYYY'));
            //$("#editstart").val(moment(start).format('hh:mm A'));
            //$("#editend").val(moment(end).format('hh:mm A'));
            $("#ctr_Location").append(data.locationName);
            //var editlo = document.getElementById('editlo');
            //editlo.selectedIndex = 0;
            //getSelectedOption(editlo, data.LocationName);
            //UpdateEditSublocation();
            $("#ctr_ActivityName").append(data.activityName);
            //var editactivity = document.getElementById('editactivity');
            //editactivity.selectedIndex = 0;
            //getSelectedOption(editactivity, data.ActivityName);
            //$('#manualorg option:not(:first)').hide();
            //document.getElementById('manualorg').selectedIndex = 0;
            //ManualOrg();
            //$('#filtervolunteer option:not(:first)').hide();
            $.each(data.organizationNameS, function (index, value) {

                if (index != totalo - 1) {
                    $("#ctr_Organization").append(value + " <br> ");
                } else {
                    $("#ctr_Organization").append(value);
                }
            });
            //var editor = document.getElementById('editor');
            //editor.selectedIndex = -1;
            //getOrgSelectedOption(editor, data.OrganizationNameS);
            //$('#editor').multiselect('refresh');
            $("#ctr_Area").append(data.sublocation);
            //var editsub = document.getElementById('editsub');
            //editsub.selectedIndex = -1;
            //getSubSelectedOption(editsub, data.Sublocation);
            //$('#editsub').multiselect('refresh');
            $("#ctr_Description").append(data.description);
            $("#ctr_PIC").append(data.pic);

        }
    });

}
function UpdateEditSublocation() {
    var selectedLocation = $('#edit-booking-location').children("option:selected").val();
    if (selectedLocation > 0) {
        var Sublocations = LocationNSublocations.find(location => location.id == selectedLocation).SubLocations;
        $('#edit-booking-sublocation').empty();
        
        $.each(Sublocations, function (index, value) {
            $("#edit-booking-sublocation").append('<option value="' + value.id + '">' + value.Name + '</option>');
        });
        //$('#editsub').multiselect('rebuild');
        var Areas = document.getElementById('Location').innerHTML;
        var editsub = document.getElementById('edit-booking-sublocation');
        editsub.selectedIndex = -1;
        getSubSelectedOption(editsub, Areas);
        //$('#editsub').multiselect('refresh');
    } else {
        $('#edit-booking-sublocation').empty();
        
    }
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
function GetEditModal() {//trigger when clicking edit button
    $('#ViewBookingModal').modal('hide');
    var bookingId = $(".bookingId").val();
    UpdateBookingDetail(bookingId);
    $('#EditBookingModal').modal('show');
}
$("#EditBookingModal").on('hide.bs.modal', function () {//trigger when edit modal close
    var bookingId = $(".bookingId").val();
    //$('#EditBookingModal').modal('show');
    var form = $(MEL.toSelector('EditBookingForm'));
    form.formReset();
    //UpdateBookingDetail(bookingId);
});

$('.bs-timepicker').datetimepicker({ format: 'HH:mm', defaultDate: moment().hours(0).minutes(0) });
function submitExtendBooking() {
    var bookingid = $("#bookingIdforExtendBooking").val();
    var ExtendedTime = $("#extendBookingbyMin").val();
    
    try {
        $.ajax({
            url: 'FacilityBooking/ExtendBooking',
            data: {
                BookingId: bookingid,
                ExtendedTime: ExtendedTime
            },
            type: 'POST',
            success: function (Json) {
                //console.log(Json);
                if (Json.success == true) {
                    bootbox.alert({
                        size: "small",
                        title: "Message",
                        message: "Booking end time extended",
                        callback: function () {
                            $('#extendBookingModal').modal('hide');
                            UpdateCalendar();

                        }
                    });
                } else {
                    var array = Json.data.dates;
                    var row = [];
                    array.forEach(showElement);
                    function showElement(item, index) {
                        //var num = index + 1;
                        //var location = (item.Sublocation != null) ? item.Sublocation : "";
                        var x =item.type;
                        row.push(x);
                    }
                    bootbox.alert({
                        size: "small",
                        title: "Message",
                        message: row[0]
                    });
                }
            }
        });
    } catch (e) {
        console.log(e);
    }
}


function CloseBooking() {
    var bookingid = $(".bookingId").val();
    //console.log(bookingid);
    bootbox.confirm({
        message: "Are you sure to end this booking?",
        size:"small",
        buttons: {
            confirm: {
                label: 'End',
                className: 'btn-success'
            },
            cancel: {
                label: 'No',
                className: 'btn-danger'
            }
        },
        callback: function (result) {
            if (result == true) {
                //if want to end
                try {
                    $.ajax({
                        url: 'FacilityBooking/CloseBooking?id=' + bookingid,
                        type: 'POST',
                        success: function (result) {
                            //console.log(result);
                            $('#ViewBookingModal').modal('hide');
                            UpdateCalendar();
                        }
                    });
                } catch (e) {
                    console.log(e);
                }
            }
        }
    });
}

function DeleteTrainee(UserId, ScheduleId, Name) {
    $('#ViewBookingModal').modal('hide');
    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: "Are you sure want to delete trainee '" + Name + "' from this activity?",
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
                $.ajax({
                    type: "POST",
                    url: '/FacilityBooking/DeleteTraineeEvent?ScheduleId=' + ScheduleId + '&UserId=' + UserId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        bootbox.alert({
                            size: "small",
                            title: "Message",
                            message: result,
                            callback: function () {
                                var bookingId = $(".bookingId").val();
                                UpdateBookingDetail(bookingId);
                                $('#ViewBookingModal').modal('show');
                            }
                        });
                    },
                    error: function () { }
                });
            } else {
                $('#ViewBookingModal').modal('show');
            }
        }
    });

}
