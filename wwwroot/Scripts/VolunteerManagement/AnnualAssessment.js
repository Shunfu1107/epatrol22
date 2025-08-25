$(document).ready(function () {
//    var t = $('#Volunteer').DataTable({
//        "pagingType": "full_numbers"
//    });
//    t.on('order.dt search.dt', function () {
//        t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
//            cell.innerHTML = i + 1;
//        });
//    }).draw();
//    $("table").removeAttr("style");

    
})




let nameArray = []
let emailArray = []

document.querySelectorAll('.checkbox').forEach(volunteer => {
    volunteer.addEventListener('change', () => {
        if (volunteer.checked == true) {
            let name = volunteer.closest('td').nextElementSibling.textContent.trim()
            let email = volunteer.closest('td').nextElementSibling.nextElementSibling.textContent.trim()
            if (name && email) {

                nameArray.push(name)
                emailArray.push(email)
            } else {
                alert('Please select valid email address')
                volunteer.checked = false
            }

        }
    })


})

$("#submit-btn").click(() => {
    console.log(nameArray)
    console.log(emailArray)

    document.querySelectorAll('.checkbox').forEach(volunteer => {
        volunteer.checked = false
    })

    for (let x = 0; x < nameArray.length; x++) {

        $.ajax({
            url: '/VolunteerManagement/SendAnnualAssessment?fullname=' + nameArray[x] + '&email=' + emailArray[x],
            type: 'POST',
            success: function (result) {
                console.log(result)
                OnSuccess(result)
            }
        })
    }
    

    nameArray = []
    emailArray = []
})

$("#annual-assesstment").submit(function (event) {
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

function OnSuccess(status) {
    
   
    let timerInterval

    if (status == 'True') {

        Swal.fire({
            icon: 'success',
            title: 'Completed',
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
            title: 'Unsucceessful',
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
    }

    

        
        return;
    }






