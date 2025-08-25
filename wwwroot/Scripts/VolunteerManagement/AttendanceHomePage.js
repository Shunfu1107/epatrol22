$(function () {
    initHomePage();
    // initSubmit();
})
var id = null;


$(document).ready(function () {


    $('.dataTable').DataTable({
        ajax: {
            url: '/VolunteerManagementTablet/GetAllEventsToday',
            type: 'POST',

            dataSrc: 'data',
            success: function (data) {


                data.forEach(function (item) {

                    $('.dataTable').DataTable().row.add([
                        item.eventModule,
                        item.eventName,
                        item.eventDate,
                        item.start + '-' + item.end,
                        '<button class="waves-effect waves-light btn modal-trigger select-btn" data-id="' + item.EventId + '" href="#exampleModal" >select</button>'

                    ]).draw();

                });
                handleSelectClick();
            },

        }


    });

    const modal = document.querySelector('.modal');
    M.Modal.init(modal);



});

function handleSelectClick() {

    selectBtn = document.querySelectorAll('.select-btn')


    selectBtn.forEach(btn => {
        btn.addEventListener('click', () => {
            //console.log(btn.dataset.id)
            generateQR(btn.dataset.id)
            initEventPage()




        })
    })
}

function generateQR(id) {
    //console.log('generate qr')
fetch('/VolunteerManagement/QRIndex?qrText= http://45.32.105.34:5587/VolunteerManagement/ActivityAttendance?eventId=' + id + '')
        .then(response => response.blob())

        .then(blob => {
            //console.log(blob)
            const imageUrl = URL.createObjectURL(blob);
            const imageContainer = document.getElementById('imageContainer');
            const img = document.createElement('img');
            img.src = imageUrl;
            imageContainer.appendChild(img);

            $.ajax({
                url: '/VolunteerManagement/GetEventDetailsById?Id=' + id + '&multipleScheduleId=' + 0,
                type: 'POST',
                success: function (data) {
                    console.log(data)
                    
                    
                    if (data.AddEditModule != null ) {

                        $('#event-name').append('Event Name : ' + data.AddEditModule.Topic)
                        $('#event-date').append('Date : ' + moment(data.AddEditModule.Dates).format("DD MMM YYYY"))
                        $('#event-time').append('Time : ' + moment(data.AddEditModule.StartTime).format("hh:mm A") + ' - ' + moment(data.AddEditModule.EndTime).format("hh:mm A"))
                    }

                    if (data.AddEditModule2 != null) {

                        $('#event-name').append('Event Name : ' + data.AddEditModule2.Name)
                        $('#event-date').append('Date : ' + moment(data.AddEditModule2.Dates).format("DD MMM YYYY"))
                        $('#event-time').append('Time : ' + moment(data.AddEditModule2.Start).format("hh:mm A") + ' - ' + moment(data.AddEditModule2.Start).format("hh:mm A"))
                    }

                    if (data.AddEditModule3 != null) {

                        $('#event-name').append('Event Name : ' + data.AddEditModule3.Name)
                        $('#event-date').append('Date : ' + moment(data.AddEditModule3.Dates).format("DD MMM YYYY"))
                        $('#event-time').append('Time : ' + moment(data.AddEditModule3.Start).format("hh:mm A") + ' - ' + moment(data.AddEditModule3.Start).format("hh:mm A"))
                    }

                    if (data.AddEditModule6 != null) {

                        $('#event-name').append('Event Name : ' + data.AddEditModule6.Name)
                        $('#event-date').append('Date : ' + moment(data.AddEditModule6.Dates).format("DD MMM YYYY"))
                        $('#event-time').append('Time : ' + moment(data.AddEditModule6.Start).format("hh:mm A") + ' - ' + moment(data.AddEditModule6.Start).format("hh:mm A"))
                    }

                }
        });
        });

    document.querySelector('#event-id').textContent = id
}










function closeModal() {

    var imageContainer = document.getElementById("imageContainer");

    while (imageContainer.firstChild) {
        imageContainer.removeChild(imageContainer.firstChild);
    }

    $('#event-name').empty()
    $('#event-date').empty()
    $('#event-time').empty()
   
}

function printModal() {
    window.print()
    closeModal()
}

function saveModal() {
    const modalWindow = window.open('', '', 'height=700,width=700');
    modalWindow.document.write(document.querySelector('#modal1').innerHTML);
    modalWindow.print();
    closeModal()

}

function initHomePage() {
    //initEventPage()

}

function initEventPage() {
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

    $('#startCard').click(function () {
        resetDefault();
    })




    //Start NFC Reader
    startReader();
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



//String Format
String.prototype.format = function () {
    var a = this;
    for (var k in arguments) {
        a = a.replace(new RegExp("\\{" + k + "\\}", 'g'), arguments[k]);
    }
    return a
}


function validateCard(cardNo) {
    //alert(cardNo)
    $.ajax({
        url: '/VolunteerManagement/ValidateCard?CardNo=' + cardNo,
        type: 'POST',


        success: function (data) {
            console.log(data)
            if (data.VolunteerId == 0) {
                text = "User not found. Please scan QR Code for attendance."
                Swal.fire({
                    title: 'Error',
                    text: text,

                })


            } else {

                //let name = data.Name
                //Swal.fire({
                //    title: 'Success',
                //    text: 'Welcome ' + name,
                //    timer: 2000
                //}).then((result) => {
                //    if (result.dismiss == swal.DismissReason.timer) {
                //        console.log('close');
                //        updateAttendance(document.querySelector('#event-id').textContent, data.VolunteerId)
                //    }
                //})

                //$('#confirm-attendance').on('show.bs.modal', function (event) {
                //    role = "Member";
                //    $('#confirm-attendance #btn-add-attendance-member').removeClass("btn-secondary").addClass("btn-warning");
                //    $('#confirm-attendance #btn-add-attendance-volunteer').removeClass("btn-warning").addClass("btn-secondary");
                //});

                ////Time out for submit attendance
                //$('#confirm-attendance').on('shown.bs.modal', function (event) {
                //    i = 5;
                //    countdownTimer = setInterval(countDown, 1000);
                //});

                ////Time out for submit attendance
                //$('#confirm-attendance').click(function (event) {
                //    clearInterval(countdownTimer);
                //    $('#confirm-attendance #btn-submit-attendance').text('Submit');
                //});
                ////Welcome title
                
                //text = "Welcome " + data.Name;
                //$('#confirm-attendance .modal-title').empty();
                //$('#confirm-attendance .modal-title').append(text);

                ////Description
                //text = data.Name;
                //$('#confirm-attendance #participant-name').empty();
                //$('#confirm-attendance #participant-name').append(text);


                //$('#confirm-attendance').toggle();
                //$('#exampleModal').toggle()


                Swal.fire({
                    title: 'Please choose your role',
                    showCloseButton: true,
                    showCancelButton: true,
                    confirmButtonText: 'Volunteer',
                    cancelButtonText: 'Member',
                }).then((result) => {
                    if (result.isConfirmed) {
                        role = "Volunteer";
                      
                    } else if (result.isDismissed) {
                        role = "Member";
                       
                    } else {
                        return;
                    }

                    text = "Welcome " + data.Name 
                    Swal.fire({
                        title: 'Success',
                        text: text,
                    });
                    if (role) {
                        updateAttendance(document.querySelector('#event-id').textContent, data.VolunteerId, role);

                    }
                });



            }
        }


    })
    localStorage.clear();



}





function updateAttendance(eventId, userId,role) {
    $.ajax({
        url: '/VolunteerManagement/UpdateAttendanceForEvent?EventId=' + eventId + '&UserId=' + userId + '&role=' + role,
        type: 'POST',


        success: function (data) {
            console.log(data)
            if (data == 'True') {
                Swal.fire({
                    title: 'Success',
                    text: 'Attendance Recorded',

                })
            }
            else {
                Swal.fire({
                    title: 'Welcome',
                    text: 'Duplicate Attendance ',

                })
            }
        },
        error: function error(_error) {
            //return "Error(\"" + _error.message + "\")";
            console.log(_error)
        }

    });
}

//Section for android reader
function onNumberRead(card_num) {
    console.log("card number = " + card_num);
    document.querySelector('#card_num').textContent = card_num
    if (card_num.length == 0) {
        alert('no card detected')
    } else {
        //alert("card number = " + card_num)
        console.log(card_num)
        validateCard(card_num)
    }
}
function startReader() {
    console.log("doing start reader");
    console.log("reader = " + JSON.stringify(AndroidCardReader));
    //alert(JSON.stringify(AndroidCardReader))
    //alert(AndroidCardReader.startReader())
    AndroidCardReader.startReader()
}
function stopReader() {
    console.log("doing stop reader");
    AndroidCardReader.stopReader();
}


$(document).ready(function () {
    $('div.callAndroidReader').each(function (i, d) {
        startReader.call();
    });

    $('#confirm-attendance').modal({
        backdrop: 'static',
        keyboard: false
});
});



//section end 
////////////////////////////////////////////////////////////////////////////////




