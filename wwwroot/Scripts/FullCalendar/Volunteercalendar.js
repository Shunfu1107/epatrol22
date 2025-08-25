/// <reference path="../../Assets/Plugins/extended-user-identity/mel.core.js" />

$(window).on("load", function () {
    initCalendar();
});

function initCalendar() {
    var calendarEl = document.getElementById('calendar');

    var today = new Date();
    var date = today.getFullYear() + '-' + (today.getMonth() + 1) + '-' + today.getDate();

    var calendar = new FullCalendar.Calendar(calendarEl, {
        height: 'auto',
        plugins: ['interaction', 'dayGrid', 'timeGrid'],
        buttonText: {
            today: 'Today',
            month: 'Month',
            week: 'Week',
            day: 'Day',
            list: 'List'
        },
        //customButtons: {
        //    create: {
        //        text: 'New Schedule',
        //        click: function () {
        //            $('#AddScheduleModal').modal("show");
        //        }
        //    },
        //    print: {
        //        text: 'Print PDF',
        //        click: function () {
        //            Downprint();
        //        }
        //    }
        //},
        header: {
            right: 'create print',
            center: 'title',
            left: 'dayGridMonth,timeGridWeek,timeGridDay,listMonth prev,next'
        },
        defaultView: 'dayGridMonth',
        defaultDate: today,
        navLinks: true, // can click day/week names to navigate views
        businessHours: true, // display business hours
        editable: false,
        eventSources: [{
            url: '/Scheduling/GetSchedule',
            method: 'GET',
            failure: function () {
                bootbox.alert({
                    size: "small",
                    title: "Error Message",
                    message: "There was an error when fetching events."
                });
            }
        }],
        eventTimeFormat: { // like '01:30 PM'
            hour: '2-digit',
            minute: '2-digit',
            meridiem: true
        },
        eventClick: function (info) {
            if (info.event.extendedProps.type !== -1) {
                UpdateDetail(info.event.id);
                $('#ViewSchedule').click();
                $('.scheduleId').val(info.event.id);
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
                $('#load').show();
            }
            else {
                $('#load').hide();
                calendar.updateSize()
            }
        }
    });

    calendar.render();

    $("#CalendarFilter").change(function () {

        var selectedArea = $(this).children("option:selected").val()
        if (selectedArea == "") {
            calendar.removeAllEventSources()
            calendar.addEventSource("/Scheduling/GetSchedule")
        }
        else {
            calendar.removeAllEventSources()
            calendar.addEventSource("/Scheduling/GetSchedule?Area=" + selectedArea);
        }

    });

}
function UpdateDetail(scheduleId) {
    $("#ActivityName").empty();
    $("#Date").empty();
    $("#Time").empty();
    $("#Location").empty();
    $("#Area").empty();
    $("#Organization").empty();
    $("#Description").empty();
    $("#PIC").empty();
    $.ajax({
        url: '/Scheduling/detail?scheduleId=' + scheduleId,
        type: 'GET',
        success: function (Json) {
            var data = Json.ScheduleDetailModel;
            var today = new Date();
            var totalo = data.OrganizationNameS.length;
            var start = new Date(parseInt((data.StartTime).match(/\d+/)[0]))
            //if (start.getFullYear() < today.getFullYear()) {
            //    $('#edit').hide();
            //}
            //else if (start.getMonth() < today.getMonth()) {
            //    $('#edit').hide();
            //} else if (start.getMonth() > today.getMonth()) {
            //    $('#edit').show();
            //}
            //else if (start.getDate() < today.getDate()) {
            //    $('#edit').hide();
            //} else {
            //    $('#edit').show();
            //}
            $('#edit').hide();
            var end = new Date(parseInt((data.EndTime).match(/\d+/)[0]))
            $("#Date").append($.datepicker.formatDate('dd M yy', start));
            $("#Time").append(formatAMPM(start) + " - " + formatAMPM(end));
            //$("#EditSchedule_Date").val(moment(start).format('DD/MM/YYYY'));
            //$("#editstart").val(moment(start).format('hh:mm A'));
            //$("#editend").val(moment(end).format('hh:mm A'));
            $("#Location").append(data.LocationName);
            //var editlo = document.getElementById('editlo');
            //editlo.selectedIndex = 0;
            //getSelectedOption(editlo, data.LocationName);
            $("#ActivityName").append(data.ActivityName);
            //var editactivity = document.getElementById('editactivity');
            //editactivity.selectedIndex = 0;
            //getSelectedOption(editactivity, data.ActivityName);
            //$('#manualorg option').hide();
            //$('#filtervolunteer option:not(:first)').hide();
            $.each(data.OrganizationNameS, function (index, value) {
                $('#manualorg option:contains("' + value + '")').show();
                $('#filtervolunteer option:contains("' + value + '")').show();
                if (index != totalo - 1) {
                    $("#Organization").append(value + " , ");
                } else {
                    $("#Organization").append(value);
                }
            });
            //var editor = document.getElementById('editor');
            //editor.selectedIndex = -1;
            //getOrgSelectedOption(editor, data.OrganizationNameS);
            //$('#editor').multiselect('refresh');
            $("#Area").append(data.Sublocation);
            //var editsub = document.getElementById('editsub');
            //editsub.selectedIndex = -1;
            //getSubSelectedOption(editsub, data.Sublocation);
            //$('#editsub').multiselect('refresh');
            $("#Description").append(data.Description);
            $("#PIC").append(data.PIC);
            //var p = $('#ParticipantTable').DataTable();
            //p.clear();
            //p.draw();
            //$.each(Json.ParticipantModels, function (index, value) {
            //    p.row.add([(index + 1), value.Name, value.NRIC, value.Contact])
            //    p.draw();
            //});
            //var v = $('#VolunteerTable').DataTable();
            //v.clear();
            //v.draw();
        }
    });
    getAllEquipment(scheduleId);
}
function getAllEquipment(scheduleId) {
    $.ajax({
        url: '/Scheduling/GetAllEquipments?ScheId=' + scheduleId,
        type: 'GET',
        success: function (Json) {
            var p = $('#EquipmentTable').DataTable();
            p.clear();
            p.draw();
            $.each(Json, function (index, value) {
                p.row.add([(index + 1), value.Name, value.Quantity, '<a class="fa fa-edit" style="cursor:pointer;" onclick="EditEquipment(' + value.Id + ',\'' + value.Name + '\',' + value.Quantity + ')"></a> | <a class="fa fa-trash" style="cursor:pointer;" onclick="DeleteEquipment(' + value.Id + ',\'' + value.Name + '\')"></a>'])
                p.draw();

            });
        }
    });
}
function formatAMPM(date) {
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
function getSelectedOption(sel, datatext) {
    var opt;
    for (var i = 0, len = sel.options.length; i < len; i++) {
        opt = sel.options[i];
        if (opt.text == datatext) {
            opt.selected = true;
        }
    }
}
function getSubSelectedOption(sel, datatext) {
    var dataarray = datatext.split(' & ');
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
function getOrgSelectedOption(sel, datatext) {
    $.each(datatext, function (index, value) {
        var opt;
        for (var i = 0, len = sel.options.length; i < len; i++) {
            opt = sel.options[i];
            if (opt.text == value) {
                opt.selected = true;
            }
        }
    });
}