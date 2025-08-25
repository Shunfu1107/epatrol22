/// <reference path="../../Assets/Plugins/extended-user-identity/mel.core.js" />

$(window).on("load", function () {
    initCalendar();
    initPhoneCountryList();
    initAddParticipant();
});
var calendar = null;

function UpdateCalendar() {
    var selectedLocation = $("#CalendarFilterLocation").children("option:selected").val()
    var selectedArea = $("#CalendarFilterArea").children("option:selected").val()
    if (selectedArea == 0) {
        calendar.removeAllEventSources()
        calendar.addEventSource("/Scheduling/GetSchedule")
    }
    else {
        calendar.removeAllEventSources()
        calendar.addEventSource("/Scheduling/GetSchedule?Area=" + selectedArea + "&Location=" + selectedLocation);
    }
    //calendar.removeAllEventSources();
    //calendar.addEventSource("/Scheduling/GetSchedule");
}
function initCalendar() {
    var calendarEl = document.getElementById('calendar');

    var today = new Date();
    var date = today.getFullYear() + '-' + (today.getMonth() + 1) + '-' + today.getDate();
    var AddValue = document.getElementById("AddValue").value;
    console.log(AddValue);

    if (AddValue == "True") {
        calendar = new FullCalendar.Calendar(calendarEl, {
            height: 'auto',
            plugins: ['interaction', 'dayGrid', 'timeGrid'],
            buttonText: {
                today: 'Today',
                month: 'Month',
                week: 'Week',
                day: 'Day',
                list: 'List'
            },
            customButtons: {
                create: {
                    text: 'New Schedule',
                    click: function () {
                        $('#AddScheduleModal').modal('show');
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
    }
    else {
        calendar = new FullCalendar.Calendar(calendarEl, {
            height: 'auto',
            plugins: ['interaction', 'dayGrid', 'timeGrid'],
            buttonText: {
                today: 'Today',
                month: 'Month',
                week: 'Week',
                day: 'Day',
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
    }
    
    

    calendar.render();
   
    $("#CalendarFilterArea").change(function () {
        var selectedLocation = $("#CalendarFilterLocation").children("option:selected").val()
        var selectedArea = $(this).children("option:selected").val()
        if (selectedArea == "") {
            calendar.removeAllEventSources()
            calendar.addEventSource("/Scheduling/GetSchedule")
        }
        else {
            calendar.removeAllEventSources()
            calendar.addEventSource("/Scheduling/GetSchedule?Area=" + selectedArea + "&Location=" + selectedLocation);
        }
       
    });
    $("#CalendarFilterLocation").change(function () {
        var selectedLocation = $(this).children("option:selected").val();
        if (selectedLocation > 0) {
            console.log("CalendarFilterLocation Change - " + LocationNSublocations);
            var Sublocations = LocationNSublocations.find(location => location.id == selectedLocation).SubLocations;
            if (Sublocations != null) {
                $("#CalendarFilterArea").find('option').not(':first').remove();
                $.each(Sublocations, function (index, value) {
                    $("#CalendarFilterArea").append('<option value="' + value.id + '">' + value.Name + '</option>');
                });
            }
            calendar.removeAllEventSources()
            calendar.addEventSource("/Scheduling/GetSchedule?Area=0&Location=" + selectedLocation);
        } else {
            $("#CalendarFilterArea").find('option').not(':first').remove();
            calendar.removeAllEventSources()
            calendar.addEventSource("/Scheduling/GetSchedule")
        }
    });
    //$("#editlo").change(function () {
    
    $("#AddNewSchedule_LocationId").change(function () {
        var selectedLocation = $(this).children("option:selected").val();
        if (selectedLocation > 0) {
            console.log("AddNewSchedule_LocationId Change - " + LocationNSublocations);
            var Sublocations = LocationNSublocations.find(location => location.id == selectedLocation).SubLocations;
            $('#sublocationList').empty();
            $('#sublocationList').multiselect('rebuild');
            $.each(Sublocations, function (index, value) {
                $("#sublocationList").append('<option value="' + value.id + '">' + value.Name + '</option>');
            });
            $('#sublocationList').multiselect('rebuild');
        } else {
            $('#sublocationList').empty();
            $('#sublocationList').multiselect('rebuild');
        }
    });
}
function UpdateEditSublocation() {
    var selectedLocation = $('#editlo').children("option:selected").val();
    if (selectedLocation > 0) {
        console.log("UpdateEditSublocation - " + LocationNSublocations);
        var Sublocations = LocationNSublocations.find(location => location.id == selectedLocation).SubLocations;
        $('#editsub').empty();
        $('#editsub').multiselect('rebuild');
        $.each(Sublocations, function (index, value) {
            $("#editsub").append('<option value="' + value.id + '">' + value.Name + '</option>');
        });
        $('#editsub').multiselect('rebuild');
        var Areas = document.getElementById('Area').innerHTML;
        var editsub = document.getElementById('editsub');
        editsub.selectedIndex = -1;
        getSubSelectedOption(editsub, Areas);
        $('#editsub').multiselect('refresh');
    } else {
        $('#editsub').empty();
        $('#editsub').multiselect('rebuild');
    }
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
    var Equipmenttable = $('#EquipmentTable').DataTable();
    Equipmenttable.clear();
    Equipmenttable.draw();
    var volunteertable = $('#VolunteerTable').DataTable();
    volunteertable.clear();
    volunteertable.draw();
    var participanttable = $('#ParticipantTable').DataTable();
    participanttable.clear();
    participanttable.draw();
    $.ajax({
        url: '/Scheduling/Detail?scheduleId=' + scheduleId,
        type: 'GET',
        success: function (Json) {
            var data = Json.scheduleDetailModel;
            console.log(data);
            var today = new Date();
            var totalo = data.organizationNameS.length;
            //var start = new Date(parseInt((data.startTime).match(/\d+/)[0]));
            var start = new Date(data.startTime);
            if (start < today.setDate(today.getDate() -5)) {
                $('#edit').hide();
                $('#deleteAll').hide();
                $('#delete').hide();
            } else {
                $('#edit').show();
                $('#deleteAll').show();
                $('#delete').show();
            }
            //var end = new Date(parseInt((data.endTime).match(/\d+/)[0]));
            var end = new Date(data.endTime);
            $("#Date").append($.datepicker.formatDate('dd M yy', start));
            $("#Time").append(formatAMPM(start) + " - " + formatAMPM(end));
            $("#EditSchedule_Date").val(moment(start).format('DD/MM/YYYY'));
            $("#editstart").val(moment(start).format('hh:mm A'));
            $("#editend").val(moment(end).format('hh:mm A'));
            $("#Location").append(data.locationName);
            var editlo = document.getElementById('editlo');
            editlo.selectedIndex = 0;
            getSelectedOption(editlo, data.locationName);
            UpdateEditSublocation();
            $("#ActivityName").append(data.activityName);
            var editactivity = document.getElementById('editactivity');
            editactivity.selectedIndex = 0;
            getSelectedOption(editactivity, data.activityName);
            $('#manualorg option:not(:first)').hide();
            document.getElementById('manualorg').selectedIndex = 0;
            ManualOrg();
            $('#filtervolunteer option:not(:first)').hide();    
            $.each(data.organizationNameS, function (index, value) {
                $('#manualorg option:contains("' + value + '")').show();
                $('#filtervolunteer option:contains("' + value + '")').show();
                if (index != totalo - 1) {
                    $("#Organization").append(value + " <br> ");
                } else {
                    $("#Organization").append(value);
                }
            });
            var editor = document.getElementById('editor');
            editor.selectedIndex = -1;
            getOrgSelectedOption(editor, data.organizationNameS);
            $('#editor').multiselect('refresh');
            $("#Area").append(data.sublocation);
            var editsub = document.getElementById('editsub');
            editsub.selectedIndex = -1;
            getSubSelectedOption(editsub, data.sublocation);
            $('#editsub').multiselect('refresh');
            $("#Description").append(data.description);
            $("#PIC").append(data.pic);
            var p = $('#ParticipantTable').DataTable();
            p.clear();
            p.draw();
            $.each(Json.participantModels, function (index, value) {
                if (value.attendanceId != 0) {
                    //p.row.add([(index + 1), value.Name, value.NRIC, value.Contact, "<a href='/Scheduling/TogglePersonRole?AttendanceId=" + value.AttendanceId + "'>" + value.Role + "</a>"])
                    p.row.add([(index + 1), value.name, value.nric, value.contact,
                        "<a style=\"cursor: pointer;\" onclick=\"TogglePersonRole('" + value.attendanceId + "','" + value.role + "','" + value.name + "')\">" + value.role + "</a>",
                        "<a style=\"cursor: pointer;\" onclick=\"DeleteAttendance('" + value.attendanceId + "','" + value.name + "')\">Delete</a>"])
                }
                else
                {
                    p.row.add([(index + 1), value.name, value.nric, value.contact, value.role,"Cannot Delete"])
                }
                p.on('order.dt search.dt', function () {
                    p.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                        cell.innerHTML = i + 1;
                    });
                }).draw();
            });
            $("#SubmitOfGetVolunteerList").trigger("click");
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
                p.row.add([(index + 1), value.name, value.quantity, '<a class="fa fa-edit" style="cursor:pointer;" onclick="EditEquipment(' + value.id + ',\'' + value.name + '\',' + value.quantity + ')"></a> | <a class="fa fa-trash" style="cursor:pointer;" onclick="DeleteEquipment(' + value.id + ',\'' + value.name + '\')"></a>'])
                p.on('order.dt search.dt', function () {
                    p.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                        cell.innerHTML = i + 1;
                    });
                }).draw();
                $("table").removeAttr("style");
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
        case 0 : var monthname = 'Jan'; break;
        case 1 : var monthname = 'Feb'; break;
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
    var strTime = DATE +' ' + monthname + ' ' + year + ' ' + hours + ':' + minutes + ' ' + ampm;
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
            d.getMinutes() < 10 ? '0'+d.getMinutes() : d.getMinutes()].join(':') +' '+ ampm;
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
function initPhoneCountryList() {
    var input = document.querySelector('#PhoneNumber');
    var phone = window.intlTelInput(input, {
        onlyCountries: ["sg", "my", "id", "cn"],
        utilsScript: "/js/utils.js",
        initialCountry: "sg",
        separateDialCode: true,
        autoPlaceholder: "off",
        geoIpLookup: function (callback) {
            $.get('https://ipinfo.io', function () { }, "jsonp").always(function (resp) {
                var countryCode = (resp && resp.country) ? resp.country : "";
                callback(countryCode);
            });
        }
    });

    $('#PhoneNumber').change(function () {
        $('#AddOrganizationMember_Telephone').val(phone.getNumber());
    });

    $('#PhoneNumber').attr('maxlength', 8);

    input.addEventListener("countrychange", function () {
        var x = phone.getSelectedCountryData().dialCode;
        if (x == "65") {
            $('#PhoneNumber').attr('maxlength', 8);
        } else {
            $('#PhoneNumber').attr('maxlength', 12);
        }
        $('#PhoneNumber').val("");

        $('#AddOrganizationMember_Telephone').val(phone.getNumber());
    });

    var validator = $("#form4").validate();

    $("#PhoneNumber").rules("add", {
        number: true
    });
}

function initAddParticipant() {

    $('#AddParticipantModel_ParticipantName').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Scheduling/GetName',
                type: "POST",
                dataType: "json",
                data: { name: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.name, value: item.name };
                    }))
                }
            })
        },
        minLength: 2
    })

    $("#add-participant-btn").click(function (e) {
        document.getElementById("add-participant-form").reset();
        $('#ViewScheduleModal').modal('hide');
    })

    $('#add-participant-form').on('reset', function (e) {
        $('#ViewScheduleModal').modal('show');
        $('#AddParticipant').modal('hide');
    })

    $('#add-participant-form').submit(function (e) {
        e.preventDefault();

        var form = $(MEL.toSelector("add-participant-form"));
        if (!form.valid()) {
            $('.input-validation-error').first().focus();
            return;
        }
        validateName();
        //Request by Chrestin from blossom seed to remove the password verification in adding participation proccess . Below is the logic
        //bootbox.prompt({
        //    title: "Please enter your password.",
        //    inputType: 'password',
        //    callback: function (result) {
        //        $.ajax({
        //            type: "POST",
        //            data: JSON.stringify({ viewmodel: result }),
        //            url: '/Scheduling/VerifyPassword',
        //            contentType: "application/json; charset=utf-8",
        //            dataType: "json",
        //            success: function (info) {
        //                if (info.Success === true) {

        //                    var data = info.Data
        //                    if (data.verified === true) {
        //                        validateName();
        //                    } else {
        //                        showMessage("Wrong Password. Please try again.")
        //                    }
        //                } else {
        //                    showMessage("Error. Please contact technician.")
        //                }
        //            },
        //            error: function () { }
        //        });
        //    }
        //});

        
    })
}

function validateName() {
    var dtv = {
        registerName: $('#add-participant-form #AddParticipantModel_ParticipantName').val(),
        registerNric: $('#add-participant-form #AddParticipantModel_NRIC').val(),
        scheduleId: $('#add-participant-form #AddParticipantModel_ScheduleId').val()
    }

    $.ajax({
        type: "GET",
        data: dtv,
        url: '/Scheduling/ValidateName',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.success === true) {

                var data = result.data;

                if (data.checkExisting === false) {
                    showMessage("User not found.");

                } else if (data.checkExisting === true && data.checkAttendance === false) {  //If user registered and not check-in yet
                    let dtoRole = {
                        userId: data.attendantId,
                        role: $('#add-participant-form #AddParticipantModel_Role').val()
                    }

                    let dto = {
                        ScheduleId: parseInt($('#add-participant-form #AddParticipantModel_ScheduleId').val()),
                        UserId: data.attendantId,
                        Role: $('#add-participant-form #AddParticipantModel_Role').val()
                    }
                    console.log(dtoRole);
                    checkRole(dto, dtoRole);

                } else if (data.checkExisting === true && data.checkAttendance === true) {  //If user registered and checked-in
                    showMessage("Existing Record Detected.")

                }
            } else {
                showMessage("Authenticate failed. Please try again later");
            }
        },
        error: function () { }
    });
}

function showMessage(message) {
    $('#AddParticipant').modal('hide');
    bootbox.alert({
        size: 'small',
        title: "Message",
        message: message,
        callback: function () {
            $('#ViewScheduleModal').modal('show');
        }
    });
}

function addParticipant(dto) {
    console.log(dto);
    var scheduleId = dto["ScheduleId"];
    $.ajax({
        type: "GET",
        data: {
            UserId: dto["UserId"],
            ScheduleId: dto["ScheduleId"],
            Role: dto["Role"],
            NRIC: dto["NRIC"],
            ParticipantName: dto["ParticipantName"]
        }
,
        url: '/Scheduling/AddParticipant',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.success === false) {
                showMessage(info.exception);
            }
            console.log(dto);
            UpdateDetail(scheduleId);
            showMessage("Attendance Added Successfully.")
        },
        error: function () { }
    });
}

function checkRole(dto, dtoRole) {
    $.ajax({
        type: "GET",
        data: dtoRole,
        url: '/Scheduling/ValidateUserRole',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.success === true) {

                var data = result.data;

                //Only return false while user do not have volunteer role. Account Exist checked before.
                if (data.checkRole === false) {

                    $('#AddParticipant').modal('hide');

                    bootbox.confirm({
                        centerVertical: true,
                        size: "large",
                        message: "Do you want to add " + data.role + " to this user?",
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
                                addRole(dto, dtoRole)
                            } else {
                                $('#ViewScheduleModal').modal('show');
                            }
                        }
                        });
                } else {
                    addParticipant(dto);
                }
            }
        },
        error: function () { }
    });
}

function addRole(dto, dtoRole) {
    var userId = dtoRole["userId"].toString();
    var role = dtoRole["role"];
    $.ajax({
        type: 'Get',
        data: {
            userId: userId,
            role: role
        },
        url: '/Scheduling/AddRoleByUser',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data.success == true) {
                addParticipant(dto);
            } else {
                showMessage("Error, try again later.");
            }
        },
        error: function () { }
    });
}

function TogglePersonRole(AttendanceId,lastrole,Name)
{
    $('#ViewScheduleModal').modal('hide');
    var role;
    if (lastrole == "Member") {
        role = "Volunteer";
    }
    else if (lastrole == "Volunteer")
    {
        role = "Member";

    }
    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: "Are you sure want to change event role from '" + lastrole + "' to '" + role + "' for user '"+Name+"' ?",
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
                    type: "GET",                    
                    url: '/Scheduling/TogglePersonRole?AttendanceId='+AttendanceId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        bootbox.alert({
                            size: "small",
                            title: "Message",
                            message: result,
                            callback: function () {
                                ScheId = $('.scheduleId').val();
                                UpdateDetail(ScheId);
                                $('#ViewScheduleModal').modal('show');
                            }
                        });                        
                    },
                    error: function () { }
                });
            } else {      
                $('#ViewScheduleModal').modal('show');
            }
        }
    });
    
}
function DeleteAttendance(AttendanceId, Name) {
    $('#ViewScheduleModal').modal('hide');   
    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: "Are you sure want to delete attendance for user '" + Name + "' ?",
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
                    type: "GET",
                    url: '/Scheduling/DeleteAttendance?AttendanceId=' + AttendanceId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        bootbox.alert({
                            size: "small",
                            title: "Message",
                            message: result,
                            callback: function () {
                                ScheId = $('.scheduleId').val();
                                UpdateDetail(ScheId);
                                $('#ViewScheduleModal').modal('show');
                            }
                        });
                    },
                    error: function () { }
                });
            } else {
                $('#ViewScheduleModal').modal('show');
            }
        }
    });

}