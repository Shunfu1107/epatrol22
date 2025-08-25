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
                $('#AddNewWithdrawal').modal('hide');
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


function view_Inventory(id) {

    $.ajax({
        url: '/Inventory/GetInventoryDetails?Id=' + id,
        type: 'POST',
        success: function (data) {
            $('#ViewInventoryModal .InventoryModal-Details').each(function () {
                $(this).empty();
            });
            console.log(data);
            $("#TagCode").append(data.tagCode);
            $("#InventoryName").append(data.name);
            $("#Department").append(data.department);
            $("#Type").append(data.type);
            $("#Usage").append(data.usage);
            $("#Description").append(data.description);
            $("#InventoryQuantity").append(data.initialQuantity + " " + data.initialQuantityUnit);
            $('#ThresholdValue').append(data.thresholdValue);
            $("#PIC").append(data.personInCharge);
            var CreateDate = moment(data.createdDate).format("DD/MM/YY");
            var ExpiryDate = moment(data.expiryDate).format("DD/MM/YY");
            $("#CreatedDate").append(CreateDate);

            if (data.expiryDate != null) {
                $("#ExpiryDate").append(ExpiryDate);
            } else {
                $("#ExpiryDate").append("-");
            }
            

            $("#Source").append(data.source);
            if (data.source == "Donor") {
                $("#SourceInfo").append("<b>Donor:</b> " + data.donor);
            } else {
                $("#SourceInfo").append("<b>Purchase.No:</b> " + data.purchaseNo);
            }
            $("#RentingStatus").append(data.rentingStatus);
            $("#Status").append(data.status);
            $("#ItemValue").append(data.itemValue);
            $("#Location").append(data.location + ", " + data.subLocation);
            if (data.maintenanceMode != null) {
                $("#MaintainanceMode").append(data.maintenanceMode);
            } else {
                $("#MaintainanceMode").append("-");
            }


            if (data.lastMaintenance != null) {
                var lastMaintenance = moment(data.lastMaintenance).format("DD/MM/YY");
                $("#lastMaintenance").append(lastMaintenance);
            } else {
                $("#lastMaintenance").append("-");
            }
            if (data.nextMaintenance != null) {
                var nextMaintenance = moment(data.nextMaintenance).format("DD/MM/YY");
                $("#NextMaintenance").append(nextMaintenance);
            } else {
                $("#NextMaintenance").append("-");
            }

            if (data.remarks != null) {
                $("#Remark").append(data.remarks);
            } else {
                $("#Remark").append("-");
            }



            $('#ViewInventoryModal').modal('show');


        }
    });
}

function SuccessLoan(result) {
    console.log(result.data.message)
    bootbox.alert({
        size: "small",
        title: "Message",
        message: result.data.message,
        callback: function () {
            if (result.success) {
                //var form = $(MEL.toSelector('form0'));
                //form.formReset();
                $('#LoanInventory').modal('hide');
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

$("#AddWithdrawalForm").submit(function (event) {
    event.preventDefault(); // Prevent the default form submission
    // Make an AJAX request to submit the form data
    $.ajax({
        url: $(this).attr("action"),
        type: $(this).attr("method"),
        data: $(this).serialize(),
        success: function (data) {
            console.log(data); 
            if (data.success == true) {
                SuccessWithdraw(data);
            }
            else {
                OnFailure(data);
            }
        }
    });
});


$("#LoanForm").submit(function (event) {
    event.preventDefault(); // Prevent the default form submission

    // Make an AJAX request to submit the form data
    $.ajax({
        url: $(this).attr("action"),
        type: $(this).attr("method"),
        data: $(this).serialize(),
        success: function (data) {
            console.log(data);
            if (data.success == true) {
                SuccessLoan(data);
            }
            else {
                OnFailure(data);
            }
        }
    });
});

$("#TopUpForm").submit(function (event) {
    event.preventDefault(); // Prevent the default form submission

    // Make an AJAX request to submit the form data
    $.ajax({
        url: $(this).attr("action"),
        type: $(this).attr("method"),
        data: $(this).serialize(),
        success: function (data) {
            console.log(data);
            if (data.success == true) {
                SuccessTopUp(data);
            }
            else {
                OnFailure(data);
            }
        }
    });
});

$("#ReturnForm").submit(function (event) {
    event.preventDefault(); // Prevent the default form submission

    // Make an AJAX request to submit the form data
    $.ajax({
        url: $(this).attr("action"),
        type: $(this).attr("method"),
        data: $(this).serialize(),
        success: function (data) {
            console.log(data);
            if (data.success == true) {
                SuccessReturned(data);
            }
            else {
                OnFailure(data);
            }
        }
    });
});

function SuccessTopUp(result) {
    console.log(result.data.message)
    bootbox.alert({
        size: "small",
        title: "Message",
        message: result.data.message,
        callback: function () {
            if (result.success) {
                //var form = $(MEL.toSelector('form0'));
                //form.formReset();
                $('#TopUpInventory').modal('hide');
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

function SuccessReturned(result) {
    console.log(result.data.message)
    bootbox.alert({
        size: "small",
        title: "Message",
        message: result.data.message,
        callback: function () {
            if (result.success) {
                //var form = $(MEL.toSelector('form0'));
                //form.formReset();
                $('#TopUpInventory').modal('hide');
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

function OnSuccess(results) {
    console.log(results.data.message)
    bootbox.alert({
        size: "small",
        title: "Message",
        message: results.data.message,
        callback: function () {

            $('#AddNewWithdrawal').modal('hide');

            window.location.reload();

        }
    });
}
function OnFailure(result) {
    console.log("failure");
    console.log(result);
}
