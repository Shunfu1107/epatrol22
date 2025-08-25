$(function () {
    initHomePage();
    initSubmit();
})

var eventList = [];
var defaultEvent = null;
var selectedEvent = null;
var name = null;
var nric = null;
var id = null;
var role = null;
var editModel = null;
var timer = null;
var submitTimer = null;
var countdownTimer = null;
var i = 5;
function initHomePage() {

    //Prevent checkbox change the status
    $('input[type=checkbox]').click(function (e) {
        e.stopPropagation();
    })

    //active UI
    $('#home-page .card').click(function () {

        //Remove default when unselect event
        if ($(this).hasClass("default")) {
            $(this).removeClass("default");
            $(this).find('input[type=checkbox]').prop('checked', false);
        }
        $(this).toggleClass("text-white bg-success active");
    });

    //Only check 1 default
    $('#home-page input[type=checkbox]').change(function () {

        //Uncheck all checkbox
        $('#home-page input[type=checkbox]').each(function () {
            $(this).prop('checked', false);
            $(this).parent().parent().parent().removeClass("default");
        });

        //Check the selected checkbox
        $(this).prop('checked', true);
        $(this).parent().parent().parent().addClass("default");

        //If selected default event not selected yet then run script below
        if (!$(this).parent().parent().parent().hasClass('active')) {
            $(this).parent().parent().parent().toggleClass("active text-white bg-success");
        }
    })

    //Next button Event
    $('#home-page #home-page-next').click(function () {
        if ($('#event-list .active').length === 0) {
            text = "Please select at least 1 event";
            showMessage(text);

            return;
        }

        if ($('#event-list .default').length === 0 && $('#event-list .active').length === 1) {
            $('#event-list .active').first().addClass("default");
            saveData();
            return;
        }

        if ($('#event-list .default').length === 0) {
            text = "Please select default event";
            showMessage(text);

            return;
        }

        saveData();
    });

    function saveData() {

        //Reset event list array
        eventList = [];
        $('#event-list .active').each(function () {
            //if ($(this).hasClass("default")) {
            //    return;
            //}
            eventList.push(Number($(this).data('id')));
        });

        //Get Details of Selected Event if array not null
        if (eventList.length != 0) {
            $.ajax({
                type: 'POST',
                data: JSON.stringify({ "viewmodel": eventList.toString() }),
                contentType: 'application/json',
                url: '/Attendance/GetSelectedEventDetails',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    if (data.success == true) {
                        event = data.data;
                        event.forEach(renderSelectedEvent);
                        initEventPage();
                    }
                },
            });
        }

        $('#event-list .default').each(function () {
            //Reset default event
            defaultEvent = $(this).data('id');
        });

        //Get Details of Selected Event
        //$.ajax({
        //    type: 'GET',
        //    url: '/Attendance/GetDefaultDetails?defaultEventId=' + defaultEvent,
        //    dataType: 'json',
        //    contentType: 'application/json; charset=utf-8',
        //    success: function (data) {
        //        if (data.success == true) {
        //            event = data.data;
        //            renderDefaultEvent(event);
        //            initEventPage();
        //        }
        //    },
        //});

        $('#home-page').hide();
        $('#event-page').show();
    }
}

function initEventPage() {

    //Hide Select List if only 1 event selected
    if (eventList.length == 1) {
        $('#selected-event-list').hide();
    }else {
        $('#selected-event-list').show();
    }

    if (defaultEvent !== null) {
        $('#event-pic .event-picture').each(function () {
            if ($(this).data('id') === defaultEvent) {
                $(this).show();
            } else {
                $(this).hide();
            }
        })
        $('#event-page #selected-event-list .card').each(function () {
            if ($(this).data('id') === defaultEvent) {
                $(this).addClass("text-white bg-success active");
            } else {
                $(this).removeClass("text-white bg-success active");
            }
        })
    }

    //Event to set the default role as Participant to variable
    $('#confirm-attendance').on('show.bs.modal', function (event) {
        role = "Member";
        $('#confirm-attendance #btn-add-attendance-member').removeClass("btn-secondary").addClass("btn-warning");
        $('#confirm-attendance #btn-add-attendance-volunteer').removeClass("btn-warning").addClass("btn-secondary");
    });

    //Set the selected event to default
    selectedEvent = defaultEvent;

    //Event to save the default event to variable when modal closed
    $('#tap-card-modal').on('hide.bs.modal', function (event) {
        selectedEvent = defaultEvent;
    });

    //Event to save the selected event to variable
    $('#tap-card-modal').on('show.bs.modal', function (event) {

        var card = $(event.relatedTarget)
        var schedule = card.data('id');

        selectedEvent = schedule;
    });

    $('#event-page #selected-event-list .card').click(function () {
        $('#event-pic .event-picture').hide();
        $('#event-page #selected-event-list .card').removeClass("text-white bg-success active");

        let id = $(this).data('id');
        selectedEvent = id;

        $(this).addClass("text-white bg-success active");

        $('#event-pic .event-picture').each(function () {
            if ($(this).data('id') === id) {
                $(this).show();
            }
        })

    })

    $('#message-box').on('hide.bs.modal', function (event) {
        clearTimeout(timer);
    });

    //Event clear all input when name attendance closed
    $('#enter-name-attendace-modal').on('hide.bs.modal', function (event) {
        $('#name-attendance-form')[0].reset();
    });

    //Time out for submit attendance
    $('#confirm-attendance').on('shown.bs.modal', function (event) {
        i = 5;
        countdownTimer = setInterval(countDown, 1000);
    });

    //Time out for submit attendance
    $('#confirm-attendance').click(function (event) {
        clearInterval(countdownTimer);
        $('#confirm-attendance #btn-submit-attendance').text('Submit');
    });

    //Flow open modal
    $('#edit-attendance').on('show.bs.modal', function (event) {
        $('#exist-attendance-modal').modal('hide');
    });

    //Flow open modal
    $('#exist-attendance-modal').on('show.bs.modal', function (event) {
        $('#edit-attendance').modal('hide');
        if (editModel.RequireTapOut === true) {
            $('#check-out-div').show()
        } else {
            $('#check-out-div').hide()
        }
        if (editModel.EnableEdit === true) {
            $('#btn-edit-attendance').show()
        } else {
            $('#btn-edit-attendance').hide()
        }
    });

    //Event checkin attendance by name
    $('#btn-submit-name').click(function () {
        var form = $("#name-attendance-form");
        form.validate();
        if (!form.valid()) {
            $('.input-validation-error').first().focus();
            return;
        }
        selectedEvent = $('#enter-name-event-selection').val();
        name = $('#attendanceName').val();
        nric = $('#attendanceNric').val();
        validateName();
        $('#enter-name-attendace-modal').modal('hide');
    })

    //Event select role in confirm attendance
    $('#confirm-attendance .btn-role').click(function () {
        //Reset all button and remove active class
        $('#confirm-attendance .btn-role').removeClass("btn-warning").addClass("btn-secondary");

        $(this).toggleClass("btn-warning btn-secondary");
        role = $(this).data("role");
    })

    //Event select role in edit attendance
    $('#edit-attendance .btn-role').click(function () {
        //Reset all button and remove active class
        $('#edit-attendance .btn-role').removeClass("btn-warning").addClass("btn-secondary");

        $(this).toggleClass("btn-warning btn-secondary");
        editModel.Role = $(this).data("role");
    })

    //Event edit attendance
    $('#exist-attendance-modal #btn-edit-attendance').click(function () {
        if (editModel.Role === "Member") {
            $('#edit-attendance #btn-edit-role-member').removeClass("btn-secondary").addClass("btn-warning");
            $('#edit-attendance #btn-edit-role-volunteer').removeClass("btn-warning").addClass("btn-secondary");
        } else if (editModel.Role === "Volunteer") {
            $('#edit-attendance #btn-edit-role-volunteer').removeClass("btn-secondary").addClass("btn-warning");
            $('#edit-attendance #btn-edit-role-member').removeClass("btn-warning").addClass("btn-secondary");
        }
        $('#edit-attendance select').val(editModel.ScheduleId);
        $('#edit-attendance').modal('show');
    })

    //Event submit edit attendance
    $('#edit-attendance select[id=event-selection]').change(function () {
        editModel.ScheduleId = $('#edit-attendance select[id=event-selection]').val();
    })

    //Event submit edit attendance
    $('#edit-attendance #btn-submit-edit-attendance').click(function () {
        validateRole("edit");
    })

    $('#btn-open-name').click(function () {
        $('#startCard').show();
        stopReader();
    })

    $('#startCard').click(function () {
        resetDefault();
    })

    $('#btn-delete-attendance').click(function () {
        deleteAttendance();
    })

    $('#btn-check-out').click(function () {
        checkOutAttendance();
    })

    //Start NFC Reader
    startReader();
}

function initSubmit() {
    $('#btn-submit-attendance').click(function () {
        validateRole("add");
    })

    $('#attendanceName').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Attendance/GetName',
                type: "POST",
                dataType: "json",
                data: { name: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.Name, value: item.Name };
                    }))
                }
            })
        },
        minLength: 2
    })
}

function renderSelectedEvent(item, index) {
    var date = moment(item.Date).format('DD/MM/YYYY');
    var start = moment(item.StartTime).format('hh:mm A');
    var end = moment(item.EndTime).format('hh:mm A');

    var photo = null;

    if (item.PhotoPath == null) {
        photo = "<div class='img-replace'></div>";
    } else {
        photo = "<img src='/ActivityImages/" + item.PhotoPath + "' class='img-act'>";
    }

    var EventView = "<div class='event-picture' data-id='{5}'><h5>{0} &nbsp; <span><br/>{4} {1} ~ {2}</span></h5><div class='img-flex div-img'>{3}</div></div>".format(item.Name, start, end, photo, date, item.Id);

    $('#event-page #event-pic').append(EventView);



    var card = "<div class='card clickable mb-2' data-id='" + item.Id + "'><div class='card-header row'><div class='col'><h5>" + item.Name + "</h5></div></div><div class='card-body'><div class='row'><div class='col'><span>Date: " + date + "<br />Time: " + start + " ~ " + end + "</span></div></div></div></div>";

    $('#event-page #selected-event-list').append(card);

    //Insert event in Edit Event
    var option = "<option value='{0}'>{1}</option>".format(item.Id, item.Name);
    $('.event-selection').append(option);
}

function validateName() {
    let registerName = name;
    let registerNric = nric

    var dto = {
        registerName: registerName,
        registerNric: registerNric,
        scheduleId: selectedEvent
    };
    $.ajax({
        type: "GET",
        data: dto,
        url: '/Attendance/ValidateName',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.success === true) {

                var data = result.data;
                var text = null;

                if (data.checkExisting === false) {
                    text = "User not found. Please register first at 'Initial Registration' tablet."
                    showMessage(text);

                } else if (data.checkExisting === true && data.checkAttendance === false) {  //If user registered and not check-in yet

                    id = data.AttendantId;

                    //Welcome title
                    text = "Welcome " + data.AttendantName;
                    $('#confirm-attendance .modal-title').empty();
                    $('#confirm-attendance .modal-title').append(text);

                    //Description
                    text = data.AttendantName;
                    $('#confirm-attendance #participant-name').empty();
                    $('#confirm-attendance #participant-name').append(text);
                    text = data.NRIC;
                    $('#confirm-attendance #participant-nric').empty();
                    $('#confirm-attendance #participant-nric').append(text);

                    //Show modal
                    $('#confirm-attendance').modal('show');
                } else if (data.checkExisting === true && data.checkAttendance === true) {  //If user registered and checked-in

                    editModel = data;

                    text = data.ScheduleName;
                    $('#exist-attendance-modal #event-name').empty();
                    $('#exist-attendance-modal #event-name').append(text);

                    text = data.Role;
                    $('#exist-attendance-modal #participant-role').empty();
                    $('#exist-attendance-modal #participant-role').append(text);

                    //Show modal
                    $('#exist-attendance-modal').modal('show');
                }
            } else {
                text = "Authenticate failed. Please try again later"
                showMessage(text);
            }
        },
        error: function () { }
    });
}

function validateCard(cardId) {
    console.log(selectedEvent)
    var dto = {
        cardId: cardId,
        scheduleId: selectedEvent
       
    };

    $.ajax({
        type: "GET",
        data: dto,
        url: '/Attendance/ValidateCard',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            console.log(result)
            if (result.success === true) {
                console.log('yes')
                var data = result.data;
                var text = null;

                if (data.checkExisting === false) {
                    text = "Please register first at 'Initial Registration' tablet"
                    showMessage(text);

                } else if (data.checkExisting === true && data.checkAttendance === false) {  //If user registered and not check-in yet

                    id = data.AttendantId;

                    //Welcome title
                    //alert("Welcome " + data.AttendantName)
                    text = "Welcome " + data.AttendantName;
                    $('#confirm-attendance .modal-title').empty();
                    $('#confirm-attendance .modal-title').append(text);

                    //Description
                    text = data.AttendantName;
                    $('#confirm-attendance #participant-name').empty();
                    $('#confirm-attendance #participant-name').append(text);
                    text = data.NRIC;
                    $('#confirm-attendance #participant-nric').empty();
                    $('#confirm-attendance #participant-nric').append(text);

                    //Show modal
                    $('#confirm-attendance').modal('show');
                } else if (data.checkExisting === true && data.checkAttendance === true) {  //If user registered and checked-in

                    editModel = data;
                    console.log(data.ScheduleName)

                    text = data.ScheduleName;
                    $('#exist-attendance-modal #event-name').empty();
                    $('#exist-attendance-modal #event-name').append(text);

                    text = data.Role;
                    $('#exist-attendance-modal #participant-role').empty();
                    $('#exist-attendance-modal #participant-role').append(text);

                    //Show modal
                    //alert('duplicate attendance')
                    $('#exist-attendance-modal').modal('show');
                }
            } else {
                text = "Authenticate failed. Please try again later"
                showMessage(text);
            }
        },
        error: function (err) {
            console.log(err)
        }
    });
}

function validateRole(type) {
    if (type === "add") {
        var dto = {
            userId: id,
            role: role
        }
    } else {
        var dto = {
            userId: editModel.AttendantId,
            role: editModel.Role
        }
    }

    $.ajax({
        type: "GET",
        data: dto,
        url: '/Attendance/ValidateUserRole',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.success === true) {

                var data = result.data;

                //Only return false while user do not have volunteer role. Account Exist checked before.
                if (data.checkRole === false) {

                    $('#addRoleModal').off('show.bs.modal');
                    $('#edit-attendance').off('show.bs.modal');
                    $('#btn-cancel-add-role').off('click');
                    $('#addRoleModal #btn-add-role').off('click');

                    $('#addRoleModal').on('show.bs.modal', function (event) {
                        if (type === "add") {
                            $('#confirm-attendance').modal('hide');
                        } else {
                            $('#edit-attendance').modal('hide');
                        }
                    });

                    $('#addRoleModal').on('hide.bs.modal', function (event) {
                        if (type === "add") {
                            $('#confirm-attendance').modal('show');
                        }
                    });

                    $('#edit-attendance').on('show.bs.modal', function (event) {
                        if (type === "edit") {
                            $('#addRoleModal').modal('hide');
                        }
                    });

                    if (type === "edit") {
                        $('#btn-cancel-add-role').click(function () {
                            $('#edit-attendance').modal('show');
                        })
                    }

                    var message = "Do you want add a '" + data.Role + "' role to this user?";
                    //Clear the message inside message box
                    $('#addRoleModal .modal-body p').empty();
                    $('#addRoleModal .modal-body p').append(message);
                    

                    $('#addRoleModal').modal('show');
                    

                    $('#addRoleModal #btn-add-role').click(function () {
                        $.ajax({
                            type: 'POST',
                            data: JSON.stringify(dto),
                            url: '/Attendance/AddRoleByUser',
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8',
                            success: function (data) {
                                if (data.success == true) {
                                    if (type === "add") {
                                        submitAttendance();
                                    } else {
                                        updateAttendance();
                                    }
                                } else {
                                    text = "Error, try again later."
                                    showMessage(text);
                                }
                                $('#addRoleModal').off('hide.bs.modal');
                                $('#addRoleModal').modal('hide');
                            },
                            error: function () {}
                        });
                    });
                } else {
                    if (type === "add") {
                        submitAttendance();
                    } else {
                        updateAttendance();
                    }
                }
            }
        },
        error: function () { }
    });
}

function submitAttendance() {

    var dto = {
        ScheduleId: selectedEvent,
        UserId: id,
        Role: role
    };

    $.ajax({
        type: 'POST',
        data: JSON.stringify(dto),
        url: '/Attendance/SubmitAttendance',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data.success == true) {
                text = "Attendance Submitted Successfully"

                $('#enter-name-attendace-modal').modal('hide');
                $('#confirm-attendance').modal('hide');
                showMessage(text);
                resetDefault();
            }
        },
    });
}

function updateAttendance() {
    let scheduleId = editModel.ScheduleId;
    let role = editModel.Role;
    let attendanceId = editModel.AttendanceId;

    var dto = {
        ScheduleId : scheduleId,
        Role : role,
        AttendanceId : attendanceId
    }

    $.ajax({
        type: 'POST',
        data: JSON.stringify(dto),
        url: '/Attendance/UpdateAttendance',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data.success == true) {
                text = "Attendance Update Successfully"

                $('#enter-name-attendace-modal').modal('hide');
                //Show modal
                $('#exist-attendance-modal').modal('hide');
                $('#edit-attendance').modal('hide');
                showMessage(text);
                resetDefault();
            }
        },
    });
}

function checkOutAttendance() {
    let attendanceId = editModel.AttendanceId;

    var dto = {
        AttendanceId: attendanceId
    }

    $.ajax({
        type: 'POST',
        data: JSON.stringify(dto),
        url: '/Attendance/CheckOutAttendance',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data.success == true) {
                text = "Attendance Checked Out Successfully"

                $('#enter-name-attendace-modal').modal('hide');
                //Show modal
                $('#exist-attendance-modal').modal('hide');
                showMessage(text);
                resetDefault();
            }
        },
    });
}

function resetDefault() {
    var selected = null;

    $('#event-page #selected-event-list .card').each(function () {
        if ($(this).hasClass("active")) {
            selected = $(this).data('id');
        }
    })

    selectedEvent = selected;
    name = null;
    nric = null;
    id = null;
    role = null;
    editModel = null;
    $('#check-out-div').hide();
    startReader();
    $('#startCard').hide();
}

function deleteAttendance() {
    let attendanceId = editModel.AttendanceId;

    var dto = {
        AttendanceId: attendanceId
    }

    $.ajax({
        type: 'POST',
        data: JSON.stringify(dto),
        url: '/Attendance/DeleteAttendance',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data.success == true) {
                text = "Attendance Delete Successfully"
                showMessage(text);

                $('#enter-name-attendace-modal').modal('hide');
                //Show modal
                $('#exist-attendance-modal').modal('hide');
                resetDefault();
                timer = setTimeout(function () {
                    $('#message-box').modal('hide');
                }, 3000);
            }
        },
    });
}

function countDown() {
    $('#confirm-attendance #btn-submit-attendance').text('Submit(' + i + ')');
    i--;
    if (i === -1) { clearInterval(countdownTimer); validateRole("add");}
}

function showMessage(message) {
    //Clear the message inside message box
    $('#message-box .modal-body h5').empty();
    $('#message-box .modal-body h5').append(message);

    //show the message box
    $('#message-box').modal('show');
    //alert(message)
    //console.log(message)
    //Set timer to close modal after 3 sec
    timer = setTimeout(function () {
        $('#message-box').modal('hide');
    }, 3000);
    //return message
}

//String Format
String.prototype.format = function () {
    var a = this;
    for (var k in arguments) {
        a = a.replace(new RegExp("\\{" + k + "\\}", 'g'), arguments[k]);
    }
    return a
}


//Section for android reader
function onNumberRead(card_num) {
    console.log("card number = " + card_num);
    //document.getElementById('card_number').value = card_num;
    if (card_num.length == 0) {
        //callReadCardID();
    } else {
        console.log(card_num);
        validateCard(card_num);
    }
}
function startReader() {
    console.log("doing start reader");
    console.log("reader = " + JSON.stringify(AndroidCardReader));
    AndroidCardReader.startReader();
}
function stopReader() {
    console.log("doing stop reader");
    AndroidCardReader.stopReader();
}

$(document).ready(function () {
    $('div.callAndroidReader').each(function (i, d) {
        startReader.call();
    });
});

//$(document).ajaxStart(function () {
//    $('#loading').show();
//});

//$(document).ajaxStop(function () {
//    $('#loading').hide();
//});

//function renderDefaultEvent(data) {
//    var date = moment(data.Date).format('DD/MM/YYYY');
//    var start = moment(data.StartTime).format('hh:mm A');
//    var end = moment(data.EndTime).format('hh:mm A');
//    var photo = null;

//    if (data.PhotoPath == null) {
//        photo = "<div class='img-replace'></div>";
//    } else {
//        photo = "<img src='/ActivityImages/" + data.PhotoPath + "'>";
//    }

//    var defaultEventView = "<h5>{0} &nbsp; <span>{4} {1} ~ {2}</span></h5><div class='img-flex'>{3}</div>".format(data.Name, start, end, photo, date);

//    $('#event-page #event-pic').append(defaultEventView);

//    //Insert event in Edit Attendance Modal
//    var option = "<option value='{0}'>{1}</option>".format(defaultEvent, data.Name);
//    $('.event-selection').append(option);

//    $('#enter-name-event-selection').val(defaultEvent);
//}