$(document).ready(function () {
    var t = $('#tableList').DataTable({
        "pagingType": "full_numbers",
        "aocolumnDefs": [{ "bSortable": false, "aTargets": [0] }],
        "columnDefs": [{ "targets": 3, "type": "date-eu" }],
    });

    t.on('order.dt search.dt', function () {
        t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
});

function Delete(id) {
    var x = $(location).attr('pathname');

    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: "Do you wish to remove the User (" + id + ") ?",
        buttons: {
            cancel: {
                label: 'Cancel'
            },
            confirm: {
                label: 'Remove'
            }
        },
        callback: function (result) {
            if (result) {
                $.post(x + "/Delete", { id: id }, function () {
                    location.reload(true);
                });
                return true;
            }
        }
    });
}


function DeleteStaffs(id, name) {

    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: "Do you wish to remove Staff role for User(" + name + ") ?",
        buttons: {
            cancel: {
                label: 'Cancel'
            },
            confirm: {
                label: 'Remove'
            }
        },
        callback: function (result) {
            if (result) {
                $.post("/Staff/Delete", { id: id }, function () {
                    location.reload(true);
                });
                return true;
            }
            else {
                return true;
            }
        }
    });

}

function DeleteVolunteers(id, name) {

    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: "Do you wish to remove Volunteer role for User(" + name + ") ?",
        buttons: {
            cancel: {
                label: 'Cancel'
            },
            confirm: {
                label: 'Remove'
            }
        },
        callback: function (result) {
            if (result) {
                $.post("/Volunteers/Delete", { id: id }, function () {
                    location.reload(true);
                });
                return true;
            }
            else {
                return true;
            }
        }
    });

}

function DeleteVolunteerDetails(id) {

    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: "Do you wish to remove Volunteer ?",
        buttons: {
            cancel: {
                label: 'Cancel'
            },
            confirm: {
                label: 'Remove'
            }
        },
        callback: function (result) {
            if (result) {
                $.post("/VolunteerManagement/DeleteVolunteer", { id: id }, function () {

                    location.reload(true);
                    window.alert("Delete Successful");
                });
                return true;
            }
            else {
                return true;
            }
        }
    });

}
function DeleteClients(id, name) {

    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: "Do you wish to remove Client role for User(" + name + ") ?",
        buttons: {
            cancel: {
                label: 'Cancel'
            },
            confirm: {
                label: 'Remove'
            }
        },
        callback: function (result) {
            if (result) {
                $.post("/Client/Delete", { id: id }, function () {
                    location.reload(true);
                });
                return true;
            }
            else {
                return true;
            }
        }
    });

}

function DeleteMembers(id, name) {

    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: "Do you wish to remove Member role for User(" + name + ") ?",
        buttons: {
            cancel: {
                label: 'Cancel'
            },
            confirm: {
                label: 'Remove'
            }
        },
        callback: function (result) {
            if (result) {
                $.post("/Member/Delete", { id: id }, function () {
                    location.reload(true);
                });
                return true;
            }
            else {
                return true;
            }
        }
    });

}

function DeleteOrganization(id, name) {

    bootbox.confirm({
        centerVertical: true,
        size: "small",
        message: "Do you wish to remove Organization(" + name + ") ?",
        buttons: {
            cancel: {
                label: 'Cancel'
            },
            confirm: {
                label: 'Remove'
            }
        },
        callback: function (result) {
            if (result) {
                $.post("/Organizations/Delete", { id: id }, function () {
                    location.reload(true);
                });
                return true;
            }
            else {
                return true;
            }
        }
    });

}