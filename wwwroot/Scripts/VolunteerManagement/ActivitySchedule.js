var multiSchedule;
//initialize participants or volunteer table in create activity schedule
var ptcpvlt;
var editptcpvlt;
var edit6ptcpvlt;
var editbrptcpvltTbl;
var archivedTbl;
var briefingRehersalTbl;
var edit_briefingRehersalTbl;
var view_briefingRehersalTbl;
var viewptcpvltTbl;
var viewBriefingRehersalTbl;
var viewSSAPIC;
var viewCSRPIC;
$(document).ready(function () {



    multiSchedule = $('#multischedule_module').DataTable({
        searching: false, paging: false, info: false,
        'fnCreatedRow': function (nRow, aData, iDataIndex) {
            $(nRow).attr('id', 'my' + iDataIndex); // or whatever you choose to set as the id
        },
    });
    multiSchedule2 = $('#multischedule_module2').DataTable({
        searching: false, paging: false, info: false,
        'fnCreatedRow': function (nRow, aData, iDataIndex) {
            $(nRow).attr('id', 'my' + iDataIndex); // or whatever you choose to set as the id
        },
    });
    viewSSAPIC = $('#view_ssaPIC').DataTable({
        searching: false, paging: false, info: false,
        'fnCreatedRow': function (nRow, aData, iDataIndex) {
            $(nRow).attr('id', 'my' + iDataIndex); // or whatever you choose to set as the id
        },
    });
    viewCSRPIC = $('#view_csrPIC').DataTable({
        searching: false, paging: false, info: false,
        'fnCreatedRow': function (nRow, aData, iDataIndex) {
            $(nRow).attr('id', 'my' + iDataIndex); // or whatever you choose to set as the id
        },
    });
    //participant and volunteer table
    ptcpvlt = $('#ptcp_vlt_table').DataTable({

        columnDefs: [
            { targets: [-1], visible: false, "searchable": false },
        ]

    });
    //Archived Table
    archivedTbl = $('#ArchivedTable').DataTable({

        "columnDefs": [
            { width: "5%", targets: 0 },
            { width: "25%", targets: 1 },
            { width: "25%", targets: 2 },
            { width: "25%", targets: 3 },
            { width: "20%", targets: 4 },
        ]
    }

    );

    getArchivedTbl();
    //briefing and rehersal table
    briefingRehersalTbl = $('#briefingRehersalTbl').DataTable();
    edit_briefingRehersalTbl = $('#edit_briefingRehersalTbl').DataTable();

    viewptcpvltTbl = $('#view_ptcpvlttable').DataTable();
    editptcpvltTbl = $('#edit_ptcp_vlt_table').DataTable({

        columnDefs: [
            { targets: [-1], visible: false, "searchable": false },
        ]

    });
    edit6ptcpvltTbl = $('#edit6_ptcp_vlt_table').DataTable({

        columnDefs: [
            { targets: [-1], visible: false, "searchable": false },
        ]

    });

    editbrptcpvltTbl = $('#edit_br_ptcp_vlt_table').DataTable({

        columnDefs: [
            { targets: [-1], visible: false, "searchable": false },
        ]

    });

    viewBriefingRehersalTbl = $('#view_rehersaltbl').DataTable();
    view_briefingRehersalTbl = $('#view_briefingRehersalTbl').DataTable();
    //initialize for table to be able to delete row
    $('#multischedule_module tbody').on('click', 'a.fas.fa-trash', function () {
        multiSchedule
            .row($(this).parents('tr'))
            .remove()
            .draw();
    });
    $('#multischedule_module2 tbody').on('click', 'a.fas.fa-trash', function () {
        multiSchedule2
            .row($(this).parents('tr'))
            .remove()
            .draw();
    });
    $('#ptcp_vlt_table tbody').on('click', 'a.fas.fa-trash', function () {
        ptcpvlt
            .row($(this).parents('tr'))
            .remove()
            .draw();
    });
    $('#edit_ptcp_vlt_table tbody').on('click', 'a.fas.fa-trash', function () {
        editptcpvltTbl
            .row($(this).parents('tr'))
            .remove()
            .draw();
    });
    $('#briefingRehersalTbl tbody').on('click', 'a.fas.fa-trash', function () {
        briefingRehersalTbl
            .row($(this).parents('tr'))
            .remove()
            .draw();
    });

    //toggle events visibilities
    $("#aacCheckbox").change(function () { $(".AACSchedule").toggle(); });
    $("#metCheckbox").change(function () { $(".METSchedule").toggle(); });
    $("#cbpCheckbox").change(function () { $(".CBPSchedule").toggle(); });
    $("#eventsCheckbox").change(function () { $(".EventsSchedule").toggle(); });

    $('#ddlLocation').change(function () {
        var selectedCountry = $(this).children("option:selected").val();
        var locationName = $(this).children("option:selected").text();

        if (selectedCountry == 1) {
            $("#otherlocations").removeClass("hidden");

        }
        else {
            $("#otherlocations").addClass("hidden");
            $("#locationName").val(locationName);
        }

    });

    $('#module1-ddlLocation').change(function () {
        var selectedCountry = $(this).children("option:selected").val();
        var locationName = $(this).children("option:selected").text();

        if (selectedCountry == 1) {
            $("#module1-otherlocations").removeClass("hidden");

        }
        else {
            $("#module1-otherlocations").addClass("hidden");
            $("#module1-locationName").val(locationName);
        }

    });

    $('#otherlocation').change(() => {
        console.log('typing other location')
        var otherlocationame = $('#otherlocation').val()
        $('#otherLocationName').val(otherlocationame)
    })


    $('#edit_ddlLocation').change(function () {
        var selectedCountry = $(this).children("option:selected").val();

        var locationName = $(this).children("option:selected").text();
        console.log(selectedCountry)
        console.log(locationName)
        if (selectedCountry == 1) {
            $("#edit_otherlocation").removeClass("hidden");
        }
        else {
            $("#edit_otherlocation").addClass("hidden");
            $('#edit_locationame').val(locationName)
        }

    });


    $('#edit_module1_ddlLocation').change(function () {
        var selectedCountry = $(this).children("option:selected").val();

        var locationName = $(this).children("option:selected").text();
        console.log(selectedCountry)
        console.log(locationName)
        if (selectedCountry == 1) {
            $("#edit_module1_otherlocation").removeClass("hidden");
        }
        else {
            $("#edit_module1_otherlocation").addClass("hidden");
            $('#edit_module1_locationame').val(locationName)
        }

    });

    //$('#edit_otherlocation').change(() => {
    //    var otherlocationame = $('#edit_otherlocation').val()
    //    $('#edit_otherlocationame').val(otherlocationame)
    //})

    $('#BriefingRehersal_Location').change(function () {
        var selectedCountry = $(this).children("option:selected").val();
        if (selectedCountry == 1) {
            $("#br_otherlocation").removeClass("hidden");
        }
        else {
            $("#br_otherlocation").addClass("hidden");
        }

    });

    $('#edit_br_location').change(function () {
        var selectedCountry = $(this).children("option:selected").val();
        if (selectedCountry == 1) {
            $("#edit_br_otherlocation").removeClass("hidden");
        }
        else {
            $("#edit_br_otherlocation").addClass("hidden");
        }

    });


    $('#edit_module6_location').change(function () {
        var selectedCountry = $(this).children("option:selected").val();
        if (selectedCountry == 1) {
            $("#edit_m6_otherlocation").removeClass("hidden");
        }
        else {
            $("#edit_m6_otherlocation").addClass("hidden");
        }

    });

    $(document).on('change', '#check-volunteer', function () {
        var checkboxes = document.querySelectorAll("#check-volunteer");
        var selectAll = document.getElementById("select-all-volunteers");
        var allChecked = true;
        for (var i = 0; i < checkboxes.length; i++) {
            if (!checkboxes[i].checked) {
                allChecked = false;
                break;
            }
        }

        selectAll.checked = allChecked;
        updateBrVltTbl();
    });





    //change form fields when selecting activity type to create
    $("#ddlActivityType").change(function () {
        var type = $('#ddlActivityType :selected').text();
        $("#module2").addClass("hidden");
        $("#module1").addClass("hidden");
        $("#module3").addClass("hidden");

        if (type == "Orientation") {
            $("#module2").removeClass("hidden");
            $("#module2-title").text("Orientation");
            $("#briefing-rehersal-tab").addClass("hidden");
            $("#BriefingRehersalSelection").addClass("hidden");

        }
        if (type == "Training") {
            $("#module1").removeClass("hidden");
            $("#module-title").text("Training");
            $("#briefing-rehersal-tab").addClass("hidden");
            $("#BriefingRehersalSelection").addClass("hidden");
        }
        if (type == "Get Together") {
            $("#module2").removeClass("hidden");
            $("#module2-title").text("Get Together");
            $("#briefing-rehersal-tab").addClass("hidden");
            $("#BriefingRehersalSelection").addClass("hidden");
        }
        if (type == "Service & Event") {
            $("#module2").removeClass("hidden");
            $("#module2-title").text("Service & Event");
            $("#briefing-rehersal-tab").addClass("hidden");
            $("#BriefingRehersalSelection").removeClass("hidden");
            $("#briefingrehersalForm").slideUp();
        }
        if (type == "Annual Assessment") {
            $("#module3").removeClass("hidden");
            $("#module3-title").text("Annual Assessment");
            $("#briefing-rehersal-tab").addClass("hidden");
            $("#BriefingRehersalSelection").addClass("hidden");
        }

        if (type != "---Select Activity Type---") {
            $("#btnSubmitCreateAct").removeClass("hidden")
            $("#participantandVolunteerTab").removeClass("hidden");


        }
        else {
            $("#btnSubmitCreateAct").addClass("hidden");
            $("#participantandVolunteerTab").addClass("hidden");

        }

        resetCreateActivityForm();


        ptcpvlt.clear();
        ptcpvlt.draw();

        //initialize hidden field formModule 1 or 2
        $("#formModuleNumber").val(2)
        if (type == "Training")
            $("#formModuleNumber").val(1)
        if (type == "Annual Assessment")
            $("#formModuleNumber").val(3)
        $("#ddlActivityType").val(type);
        $(".ddlcollaboration").trigger("change");
        $(".edit_ddlcollaboration").trigger("change");
        $("#multipleCheckBox_module1").trigger("change");
        $("#multipleCheckBox").trigger("change");
        $("#ptcp_vlt-tab").trigger("click");
    });
    $(".autoinviteCheckbox").change(function () {

        var type = $('#ddlActivityType :selected').text();
        var orientation;
        var training;
        if (type == "Orientation" || type == "Get Together" || type == "Service & Event") {
            orientation = 0;
            training = 3;
        }
        if (type == "Training") {
            orientation = 0;
            training = 3;
        }


        if ($(this).prop("checked") == true) {
            $.ajax({
                url: '/VolunteerManagement/GetVolunteers?Orientation=' + orientation + "&Training=" + training,
                type: 'POST',
                success: function (info) {
                    //console.log(info)
                    ptcpvlt.clear();


                    $.each(info, function (index, value) {
                        //var splitted = [];
                        //splitted = Textvalues[index].split("| ");
                        //var Textvalues = $.map($("#ddlParticipants option:selected"), function (option) { return option.text; });
                        /*                        ptcpvlt.row.add([(index + 1), splitted[0], splitted[1], splitted[2], "<a class='fas fa-trash' style='cursor:pointer;'></a>", value]).node();*/
                        ptcpvlt.row.add([(index + 1), value.name, value.nRIC, value.Email, "<a class='fas fa-trash' style='cursor:pointer;'></a>", value.userId]).node()

                    });
                    ptcpvlt.draw(false);
                }
            });


        } else {
            //$("#participantandVolunteerTab").removeClass("hidden");
            ptcpvlt.clear();
            ptcpvlt.draw();
        }

    });
    $("#multipleCheckBox").change(function () {
        if ($(this).prop("checked") == true) {
            $("#CreateActivity_Module2_SingleDateSelection").addClass("hidden");
            $("#CreateActivity_Module2_MultipleDateSelection").removeClass("hidden");


            multiSchedule.clear();

            multiSchedule.draw();
        } else {
            $("#CreateActivity_Module2_SingleDateSelection").removeClass("hidden");
            $("#CreateActivity_Module2_MultipleDateSelection").addClass("hidden");
        }
    });
    //trigger when adding values into schedule table
    $(".multiScheduleAddBtn").click(function () {
        if ($("#formModuleNumber").val() == 1) {
            var date = $("#rptDates_module1").val()
            var strttime = $("#module1_Mstarttime").val()
            var endtime = $("#module1_Mendtime").val()
            //var aDate = $("#module1_Mstarttime").data("DateTimePicker").date()
            //var bDate = $("#module1_Mendtime").data("DateTimePicker").date()
            ////var bDate = new Date(endtime).getTime();
            //console.log(aDate>bDate)
            //console.log(endtime)
            //validation
            var validation = validateMultipleScheduleTable(date, strttime, endtime, 1);
            if (!validation)
                return;
            //add the row data into the table
            multiSchedule.row.add([date, strttime, endtime, "<a class='fas fa-trash'></a>"]).node();
            multiSchedule.draw(false);

            var form_data = multiSchedule.rows().data();
            var data = new Array();
            $.each(form_data, function (index, value) {
                //console.log(value[0])
                var multiSche = {
                    "Date": value[0],
                    "StartTime": value[1],
                    "EndTime": value[2]
                }
                data.push(multiSche);
            });
            if (data.length != 0)
                $("#AddEditModule_MultipleDatetimeSchedule").val(JSON.stringify(data))
        } else {
            var date = $("#rptDates_module2").val()
            var strttime = $("#module2_Mstarttime").val()
            var endtime = $("#module2_Mendtime").val()

            //validation
            var validation = validateMultipleScheduleTable(date, strttime, endtime, 2);
            if (!validation)
                return;
            //add the row data into the table
            multiSchedule2.row.add([date, strttime, endtime, "<a class='fas fa-trash'></a>"]).node();
            multiSchedule2.draw(false);

            var form_data = multiSchedule2.rows().data();
            var data = new Array();
            $.each(form_data, function (index, value) {
                //console.log(value[0])
                var multiSche = {
                    "Date": value[0],
                    "StartTime": value[1],
                    "EndTime": value[2]
                }
                data.push(multiSche);
            });
            if (data.length != 0)
                $("#AddEditModule2_MultipleDatetimeSchedule").val(JSON.stringify(data))
            console.log($("#AddEditModule2_MultipleDatetimeSchedule").val())
        }


    });
    function validateMultipleScheduleTable(date, start, end, x) {

        if (date == "" || start == "" || end == "") {

            dialog("Repeating dates, Start Time and End Time could not be empty.", "error")
            return false;
        }
        var form_data;
        if (x == 1) {
            form_data = multiSchedule.rows().data();
        } else {
            form_data = multiSchedule2.rows().data();
        }
        for (let x = 0; x < form_data.length; x++) {
            if (form_data[x][0] == date && form_data[x][1] == start && form_data[x][2] == end) {

                dialog("Same duration schedule have been added.", "error")
                return false;
            }
            if (form_data[x][0] == date) {
                dialog("Same day only can have one duration.", "error")
                return false;
            }


        }
        return true
    }
    $("#multipleCheckBox_module1").change(function () {
        if ($(this).prop("checked") == true) {
            $("#CreateActivity_Module1_SingleDateSelection").addClass("hidden");
            $("#CreateActivity_Module1_MultipleDateSelection").removeClass("hidden");
        } else {
            $("#CreateActivity_Module1_SingleDateSelection").removeClass("hidden");
            $("#CreateActivity_Module1_MultipleDateSelection").addClass("hidden");
        }
    });

    $("#multipleCheckBox_module1").change(function () {
        if ($(this).prop("checked") == true) {
            $("#CreateActivity_Module1_SingleDateSelection").addClass("hidden");
            $("#CreateActivity_Module1_MultipleDateSelection").removeClass("hidden");
        } else {
            $("#CreateActivity_Module1_SingleDateSelection").removeClass("hidden");
            $("#CreateActivity_Module1_MultipleDateSelection").addClass("hidden");
        }
    });

    $('.briefing-rehersal-tab').click(() => {
        $("#edit_briefingrehersalForm").slideUp();

    })
    $("#edit_btnOpenBFForm").click(function () {
        $("#edit_briefingrehersalForm").slideToggle();
    });

    $("#btnOpenBFForm").click(function () {
        $("#briefingrehersalForm").slideToggle();
    });
    $("#btnSubmitBF").click(function () {


        var name = $("#BriefingRehersal_Name").val();
        var date = $("#BriefingRehersal_Date").val();
        var start = $("#BF_start").val();
        var end = $("#BF_end").val();
        var location = $("#BriefingRehersal_Location").val();
        var otherLoc = $("#BriefingRehersal_OtherLocation").val();
        var mode = $("#BriefingRehersal_ConductMode").val();
        var link = $("#BriefingRehersal_Link").val();
        var remark = $("#BriefingRehersal_Remarks").val();
        //validation
        var validation = validatebriefingrehersal(name, start, end);
        if (!validation) {
            return false;
        }
        if (otherLoc != null) {
            briefingRehersalTbl.row.add([name, date, start, end, otherLoc, mode, link, remark, "<a class='fas fa-trash'></a>"]).node();
        } else {

            briefingRehersalTbl.row.add([name, date, start, end, location, mode, link, remark, "<a class='fas fa-trash'></a>"]).node();
        }
        briefingRehersalTbl.draw(false);
        console.log(date)

        $("#BriefingRehersal_Name").val('')
        $("#BF_start").val('')
        $("#BriefingRehersal_Date").val('');
        $("#BF_end").val('')
        $("#BriefingRehersal_Location").val('');
        $("#BriefingRehersal_ConductMode").val('');
        $("#BriefingRehersal_Link").val('');
        $("#BriefingRehersal_OtherLocation").val('');
        $("#BriefingRehersal_Remarks").val('')
    });

    $("#edit_btnSubmitBF").click(function () {
        var name = $("#edit_bf_name").val();
        var date = $("#edit_Bf_date").val();
        var start = $("#edit_BF_start").val();
        var end = $("#edit_BF_end").val();
        var location = $('#edit_br_location').val()
        var otherLoc = $('#edit_br_otherlocation').val()
        var cm = $('#edit_br_cm').val()
        var link = $('#edit_br_link').val()
        var remark = $("#edit_br_remark").val();
        //validation
        var validation = validatebriefingrehersal(name, start, end);
        if (!validation) {
            return false;
        }
        if (otherLoc != "") {
            edit_briefingRehersalTbl.row.add([name, date, start, end, otherLoc, cm, link, remark]).node();
        } else {

            edit_briefingRehersalTbl.row.add([name, date, start, end, location, cm, link, remark]).node();
        }
        edit_briefingRehersalTbl.draw(false);


        $("#edit_bf_name").val('');
        $("#edit_Bf_date").val('');
        $("#edit_BF_start").val('');
        $("#edit_BF_end").val('');
        $('#edit_br_location').val('')
        $('#edit_br_cm').val('')
        $('#edit_br_link').val('')
        $("#edit_br_remark").val('');

    });

    function validatebriefingrehersal(name, start, end) {

        if (name == "" || start == "" || end == "") {

            dialog("Briefing Rehersal Name, Start Time and End Time could not be empty.", "error")
            return false;
        }
        var form_data = briefingRehersalTbl.rows().data();
        for (let x = 0; x < form_data.length; x++) {
            if (form_data[x][0] == name && form_data[x][2] == start && form_data[x][3] == end) {

                dialog("Same duration schedule have been added.", "error")
                return false;
            }
        }
        return true
    }
    $(".ddlcollaboration").change(function () {
        if (($(this).val()) == "Yes") {
            $("#CSRorPartnerModule").removeClass("hidden");
        } else {
            $("#CSRorPartnerModule").addClass("hidden");
        }
    });
    $(".edit_ddlcollaboration").change(function () {
        if (($(this).val()) == "Yes") {
            $("#edit_CSRorPartnerModule").removeClass("hidden");
        } else {
            $("#edit_CSRorPartnerModule").addClass("hidden");
        }
    });
    $("#AddEditModule2_BriefingRehersal").change(function () {
        if ($(this).prop("checked") == true) {

            $("#briefing-rehersal-tab").removeClass("hidden");
        } else {
            $("#briefing-rehersal-tab").addClass("hidden");
        }

    });
    $("#ddlTopic").change(function () {
        if (($(this).val()) == "Others") {
            $("#othertopic").removeClass("hidden");
        } else {
            $("#othertopic").addClass("hidden");
        }

    });
    $("#edit-ddlTopic").change(function () {
        if (($(this).val()) == "Others") {
            $("#edit-othertopic").removeClass("hidden");
        } else {
            $("#edit-othertopic").addClass("hidden");
        }

    });
    $("#ddlPrerequisite").change(function () {
        if (($(this).val()) == "Yes") {
            $("#prerequisiteinfo").removeClass("hidden");
        } else {
            $("#prerequisiteinfo").addClass("hidden");
        }

    });
    $("#ddlPayment").change(function () {
        if ($(this).val() == "Free") {
            $("#AddEditModule_Cost").val("");
        }
    });

    $('#ddlParticipants').on('change', function (e) {
        UpdateParticipantsTbl();
    });
    $('#edit_ddlParticipants').on('change', function (e) {
        UpdateEditParticipantsTbl();
    });
    var s = new Date();
    var e = new Date();
    s.setMonth(s.getMonth() - 2);
    e.setMonth(e.getMonth() + 24);

    $('.timepicker').datetimepicker({
        format: 'hh:mm A'
        /*useCurrent: true*/
    });
    $(".rptDates").datepicker({
        //endDate: '14/09/2022',
        format: 'dd/mm/yyyy',
        //multidate: 3,
        //multidateSeparator: ", ",
        todayHighlight: true,
        maxViewMode: 2,
        autoclose: true
    });
    $(".datepicker").datepicker({
        //endDate: '14/09/2022',
        format: 'dd/mm/yyyy',
        todayHighlight: true,
        maxViewMode: 2,
        todayBtn: true,
        autoclose: true
    });
    $('.datetimepicker').datetimepicker({
        format: 'DD/MM/yyyy hh:mm A',
        useCurrent: true
    });

    $("#ddlParticipants").multiselect({
        enableCaseInsensitiveFiltering: true,
        maxHeight: 200,
        dropRight: true,
        buttonWidth: '50%',
        includeSelectAllOption: true
    });
    $("#ddlSSA").multiselect({
        nonSelectedText: '---Select SSA/Partner---',
        dropRight: true
    });
    $("#editddlSSA").multiselect({
        nonSelectedText: '---Select SSA/Partner---',
        dropRight: true
    });
    $("#ddlCSR").multiselect({
        nonSelectedText: '---Select CSR---',
        dropRight: true
    });
    $("#editddlCSR").multiselect({
        nonSelectedText: '---Select CSR---',
        dropRight: true
    });
    $("#edit_ddlParticipants").multiselect({
        enableCaseInsensitiveFiltering: true,
        maxHeight: 200,
        dropRight: true,
        buttonWidth: '50%',
        includeSelectAllOption: true
    });



    initCalendar();
})
function initCalendar() {
    var calendarEl = document.getElementById('calendar');
    var today = new Date();
    var date = today.getFullYear() + '-' + (today.getMonth() + 1) + '-' + today.getDate();
    var AddValue = document.getElementById("AddValue").value;

    if (AddValue == "True") {
        calendar = new FullCalendar.Calendar(calendarEl, {
            height: 'auto',
            //contentHeight: 600,
            plugins: ['interaction', 'dayGrid'],
            buttonText: {
                //today: 'Today',
                month: 'Month',
                week: 'Week',
                //day: 'Day',
                //list: 'List',
                dayGrid: 'Day',


            },
            customButtons: {
                create: {
                    text: 'Add New Activity',
                    click: function () {
                        var form = $(MEL.toSelector('createActivity-form'));
                        form.formReset();
                        //$("#participantandVolunteerTab").addClass("hidden");

                        $("#ddlActivityType").trigger("change");
                        $("#ddlActivityType").prop('selectedIndex', 0);
                        $("#ddlcollaboration").trigger("change");
                        $("#edit_ddlcollaboration").trigger("change");
                        $("#module2").addClass("hidden");
                        $("#module1").addClass("hidden");
                        $("#CSRorPartnerModule").addClass("hidden");
                        $("edit_CSRrPartnerModule").addClass("hidden");

                        $("#ddlParticipants option:selected").removeAttr("selected");
                        $('#AddActivityModal').modal("show");
                    }

                },

            },

            header: {
                right: 'create',
                center: 'title',
                left: 'dayGridMonth,dayGrid, prev,next'
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
            eventLimit: true, // for all non-TimeGrid views
            views: {
                //timeGrid: {
                //    eventLimit: 4 // adjust to 3 only for timeGridWeek/timeGridDay
                //},

                dayGrid: {
                    eventLimit: false// options apply to dayGridMonth, dayGridWeek, and dayGridDay views
                },
                //day: {
                //    eventLimit: 2// options apply to dayGridDay and timeGridDay views
                //},
                //week: {
                //    eventLimit: 2// options apply to dayGridWeek and timeGridWeek views
                //},
                dayGridMonth: {

                    eventLimit: 5
                }
            },
            eventSources: [{
                url: '/VolunteerManagement/GetActivitySchedule',
                method: 'POST',
                //success: function (re) { console.log(re)},
                failure: function () {
                    bootbox.alert({
                        size: "small",
                        title: "Error Message",
                        message: "There was an error when fetching events."
                    });
                }
            }],
            eventClick: function (info) {
                //console.log(info.event.extendedProps.type);//1=aac,2=met,3=cbp
                var type = info.event.extendedProps.type;
                var _id = info.event.id;
                var _externalId = info.event.extendedProps.externalId
                if (type == 1) {
                    //console.log("AAC");
                    GetAACDetails(_id);
                    $("#ViewAACScheduleModal").modal('show');
                } else if (type == 2) {
                    //console.log("MET");
                    GetMETDetails(_id);
                    $("#ViewMETDetailsModal").modal('show');
                } else if (type == 3) {
                    //console.log("CBP");
                    ShowCBPDetails(_id);
                    $("#ViewCBPModal").modal("show");
                } else if (type == 4) {
                    //console.log("Events");
                    $("#viewEvent_Id").val(_id);
                    ViewEvent(_id, _externalId, 0);
                }
            },
            eventRender: function (info) {
                info.el.querySelector('.fc-title').innerHTML = info.event.title;
            },


        });
    }
    else {
        calendar = new FullCalendar.Calendar(calendarEl, {
            height: 'auto',
            //contentHeight: 600,
            plugins: ['interaction', 'dayGrid'],
            buttonText: {
                //today: 'Today',
                month: 'Month',
                week: 'Week',
                //day: 'Day',
                //list: 'List',
                dayGrid: 'Day',


            },
            

            header: {
                right: 'create',
                center: 'title',
                left: 'dayGridMonth,dayGrid, prev,next'
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
            eventLimit: true, // for all non-TimeGrid views
            views: {
                //timeGrid: {
                //    eventLimit: 4 // adjust to 3 only for timeGridWeek/timeGridDay
                //},

                dayGrid: {
                    eventLimit: false// options apply to dayGridMonth, dayGridWeek, and dayGridDay views
                },
                //day: {
                //    eventLimit: 2// options apply to dayGridDay and timeGridDay views
                //},
                //week: {
                //    eventLimit: 2// options apply to dayGridWeek and timeGridWeek views
                //},
                dayGridMonth: {

                    eventLimit: 5
                }
            },
            eventSources: [{
                url: '/VolunteerManagement/GetActivitySchedule',
                method: 'POST',
                //success: function (re) { console.log(re)},
                failure: function () {
                    bootbox.alert({
                        size: "small",
                        title: "Error Message",
                        message: "There was an error when fetching events."
                    });
                }
            }],
            eventClick: function (info) {
                console.log(info.event);
                //console.log(info.event.extendedProps.type);//1=aac,2=met,3=cbp
                var type = info.event.extendedProps.type;
                var _id = info.event.id;
                var _externalId = info.event.extendedProps.externalId
                if (type == 1) {
                    //console.log("AAC");
                    GetAACDetails(_id);
                    $("#ViewAACScheduleModal").modal('show');
                } else if (type == 2) {
                    //console.log("MET");
                    GetMETDetails(_id);
                    $("#ViewMETDetailsModal").modal('show');
                } else if (type == 3) {
                    //console.log("CBP");
                    ShowCBPDetails(_id);
                    $("#ViewCBPModal").modal("show");
                } else if (type == 4) {
                    //console.log("Events");
                    $("#viewEvent_Id").val(_id);
                    ViewEvent(_id, _externalId, 0);
                }
            },
            eventRender: function (info) {
                info.el.querySelector('.fc-title').innerHTML = info.event.title;
            },


        });
    }
    
    calendar.render();

}
//#region CBP
function ShowAppointmentDetail(ID, AID, CID, Start, End, K1N, K2N, K3N, K4N, K5N, K1, K2, K3, K4, K5, Remark, Status, TapOut, FeebackisNull, newKaki1, newKaki2, newKaki3, newKaki4, newKaki5) {
    //reset add and edit form
    //var Editform = $(MEL.toSelector('form-edit-cbp-appointment'));
    //Editform.formReset();

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
    //console.log(ID)
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
        //$('#EditA-Yes').prop('checked', true);
        //$('#EditA-No').prop('checked', false);
    } else {
        $('#CBP_Appointment_TapOut').append("No");
        //$('#EditA-No').prop('checked', true);
        //$('#EditA-Yes').prop('checked', false);
    }


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
        ViewFeedbackDetails(ID);
    });

}

function getAppointmentAndDisplayDetail(item, index) {
    //1Id,2CBPSchedulingId,3ClientId,4ClientName,5ClientAddress,6Kaki1,7Kaki1Name ,8Kaki2 ,9Kaki2Name ,10Kaki3 ,11Kaki3Name,12Kaki4 ,13Kaki4Name,14Kaki5 ,15Kaki5Name,16CBPAppointmentId,
    //17StartTime, 18EndTime, 19Remarks, 20Status, 21RequireTapOut 
    //(ID, AID, CID, Start, End, K1, K2, K3, K4, K5, Remark, Status)
    console.log(index['StartTime'])
    console.log(index['EndTime'])
    index['StartTime'] = moment(parseInt((index['StartTime']).match(/\d+/)[0])).format('hh:mm A');
    index['EndTime'] = moment(parseInt((index['EndTime']).match(/\d+/)[0])).format('hh:mm A');
    if (index['Status'] == "Cancelled") {
        var navtab = "<li class='Appointment'><a data-toggle='tab' href='#Appointment-details' onclick='ShowAppointmentDetail("
            + index['Id'] + ",\"" + index['CBPAppointmentId'] + "\"," + index['ClientId'] + ",\"" + index['StartTime'] + "\",\"" + index['EndTime'] +
            "\",\"" + index['Kaki1Name'] + "\",\"" + index['Kaki2Name'] + "\",\"" + index['Kaki3Name'] + "\",\"" + index['Kaki4Name'] + "\",\"" + index['Kaki5Name'] +
            "\",\"" + index['Kaki1'] + "\",\"" + index['Kaki2'] + "\",\"" + index['Kaki3'] + "\",\"" + index['Kaki4'] + "\",\"" + index['Kaki5'] +
            "\",\"" + index['Remarks'] + "\",\"" + index['Status'] + "\",\"" + index['RequireTapOut'] + "\");'><del>" + index['ClientName'] + "</del></a></li>";
        $('#CBP_Detail_TabList').append(navtab);
    } else {
        var navtab = "<li class='Appointment'><a data-toggle='tab' href='#Appointment-details' onclick='ShowAppointmentDetail("
            + index['Id'] + ",\"" + index['CBPAppointmentId'] + "\"," + index['ClientId'] + ",\"" + index['StartTime'] + "\",\"" + index['EndTime'] +
            "\",\"" + index['Kaki1Name'] + "\",\"" + index['Kaki2Name'] + "\",\"" + index['Kaki3Name'] + "\",\"" + index['Kaki4Name'] + "\",\"" + index['Kaki5Name'] +
            "\",\"" + index['Kaki1'] + "\",\"" + index['Kaki2'] + "\",\"" + index['Kaki3'] + "\",\"" + index['Kaki4'] + "\",\"" + index['Kaki5'] +
            "\",\"" + index['Remarks'] + "\",\"" + index['Status'] + "\",\"" + index['RequireTapOut'] + "\",\"" + index['FeedbackisNull'] + "\",\"" + index['newKaki1'] + "\",\"" + index['newKaki2'] + "\",\"" + index['newKaki3'] + "\",\"" + index['newKaki4'] + "\",\"" + index['newKaki5'] + "\");'>" + index['ClientName'] + "</a></li>";
        $('#CBP_Detail_TabList').append(navtab);
    }
}



function ShowCBPDetails(id) {

    //console.log(id)
    $.ajax({
        url: '/CBPScheduling/Detail?CBBPId=' + id,
        type: 'GET',
        success: function (info) {
            //clear detail    
            $('#ViewCBPModal .modal-details').each(function () {
                $(this).empty();
            });
            document.getElementById("tbl_LatestApptDetails").style.display = "none";
            //clear tabs 
            $('#CBP_Detail_TabList').empty();

            $('#Empty-Tab-Link').trigger('click');
            if (info.success == true) {
                //push data to CBP Detail
                var CBPDetail = Object.values(info.data['CBPDetail']); //0 = Id, 1 = GroupLeaderId, 2 = ScheduleDate , 3 = CBPScheduleId(string),4 = GroupLeaderName , 5 = Remarks , 6 = Status            

                //if status is cancelled hide button                
                if (CBPDetail[6] == "Cancelled") {
                    $("#edit").hide();
                    $("#cancel").hide();
                } else {
                    $("#edit").show();
                    $("#cancel").show();
                }

                //Convert DateTime Format
                CBPDetail[2] = moment(parseInt((CBPDetail[2]).match(/\d+/)[0])).format('DD/MM/YYYY');

                var j = 2; //b'cuz have id and group leader id so start with 2
                $('#Schedule_Detail  .modal-details').each(function () {
                    $(this).append(CBPDetail[j]);
                    j++;
                });
                $('#GroupLeaderId').val(CBPDetail[1]);

                //Update Edit Schedule Model detail
                ///GetEditCBPScheduleDetail();

                //erase all appointments first
                $('#CBP_Detail_TabList .Appointments').remove();

                //push data to CBP Appointment
                //1Id,2CBPSchedulingId,3ClientId,4ClientName,5ClientAddress,6Kaki1,7Kaki1Name ,8Kaki2 ,9Kaki2Name ,10Kaki3 ,11Kaki3Name,12Kaki4 ,13Kaki4Name,14Kaki5 ,15Kaki5Name,16CBPAppointmentId,17StartTime,18EndTime,19Remarks,20Status,21RequireTapOut 
                var CBPDetailAppointment = Object.values(info.data['CBPAppointments']);
                if (CBPDetailAppointment != null && CBPDetail.length > 1) {
                    $(CBPDetailAppointment).each(getAppointmentAndDisplayDetail);
                    //alert(CBPDetailAppointment.length);
                    //set maximum of the appointment to be 8 then stop showing the add appointment tab.
                    $("#CBP_Detail_TabsContent").removeClass("hidden");
                    $("#Tag_noAppt").addClass("hidden");
                    if (CBPDetailAppointment.length == 0) {
                        //var navtab = "<li class='Appointment' id='CBP_Detail_Tab'><a data-toggle='tab' href='#Add_Appointment_Tab' id='CBP_Detail_Add_Appointment_Tab'>Add Appointment</a></li>";
                        //$('#CBP_Detail_TabList').append(navtab);
                        $("#CBP_Detail_TabsContent").addClass("hidden");
                        $("#Tag_noAppt").removeClass("hidden");
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
            console.log(data)
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
            var ddlkakilist = data.DDLKakiList;

            for (var i = 0; i < ddlkakilist.length; i++) {
                var row = document.createElement("tr");
                table.appendChild(row);
                var cell1 = row.insertCell(0);
                cell1.innerHTML = '<td>' + ddlkakilist[i].KakiName + '</td>';

                var cell2 = row.insertCell(1);
                if (ddlkakilist[i].CheckInStatus == 1) {
                    cell2.innerHTML = '<p class="glyphicon glyphicon-ok" style="color:greenyellow"></p>';
                } else {
                    cell2.innerHTML = '<p class="glyphicon glyphicon-remove" style="color:red"></p>';
                }

                var cell3 = row.insertCell(2);
                if (ddlkakilist[i].CheckOutStatus == 1) {
                    cell3.innerHTML = '<p class="glyphicon glyphicon-ok" style="color:greenyellow"></p>';
                } else {
                    cell3.innerHTML = '<p class="glyphicon glyphicon-remove" style="color:red"></p>';
                }



            }
        }
    });
}

function ViewFeedbackDetails(id) {
    $.ajax({
        url: '/CBPScheduling/GetFeedbackDetailsById?Id=' + id,
        type: 'POST',
        success: function (info) {
            $("#ViewCBPModal").modal('hide');
            $("#ViewCBPFeedback").modal('show');
            //clear detail    
            $('#feedbackTbl .feedback-details').each(function () {
                $(this).empty();
            });
            $("#apptStrId").empty();
            //console.log(info);
            var feedbacks = Object.values(info.getFeedbackDetails);
            //console.log(feedbacks)
            if (info.ErrorMessage != null)
                bootbox.alert({
                    centerVertical: true,
                    size: "small",
                    message: info.errorMessage
                });
            $("#apptStrId").append(feedbacks[1]);
            var mood = ForNullofEmpty(feedbacks[2]);
            var visittype = ForNullofEmpty(feedbacks[3]);
            var circulation = ForNullofEmpty(feedbacks[4]);
            var brightness = ForNullofEmpty(feedbacks[5]);
            var cleanliness = ForNullofEmpty(feedbacks[6]);
            var sign = ForNullofEmpty(feedbacks[7]);
            var remark = ForNullofEmpty(feedbacks[8]);
            var createdby = ForNullofEmpty(feedbacks[9]);

            $("#feedback-details-mood").append(mood);
            $("#feedback-details-type").append(visittype);
            $("#feedback-details-circulation").append(circulation);
            $("#feedback-details-brightness").append(brightness);
            $("#feedback-details-cleanliness").append(cleanliness);
            $("#feedback-details-sign").append(sign);
            $("#feedback-details-remark").append(remark);
            $("#feedback-details-createdby").append(createdby);
        }
    });
}
//#endregion

//#region AAC
function GetAACDetails(id) {
    $.ajax({
        url: '/Scheduling/Detail?ScheduleId=' + id,
        type: 'GET',
        success: function (res) {
            console.log(res);
            $(".aacdetails").each(function () {
                $(this).empty();
            });
            //initialize equitment table
            var Equipmenttable = $('#aac_EquipmentTable').DataTable();
            Equipmenttable.clear();
            Equipmenttable.draw();

            //initialize volunteer table
            var volunteerTable = $('#aac_VolunteerTable').DataTable();
            volunteerTable.clear();
            volunteerTable.draw();

            //initialize equitment table
            var perticipantTable = $('#aac_ParticipantTable').DataTable();
            perticipantTable.clear();
            perticipantTable.draw();

            //console.log(res);
            var data = res.scheduleDetailModel;
            console.log(data);
            //console.log(data.activityName);
            $('#aac_ActivityName').append(data.activityName);
            if (data.activityType == 2) {
                $('#aac_ActivityName').empty();
                $('#aac_ActivityName').append(data.activityName + " (Ad Hoc)");
            }

            //var date = new Date(parseInt((data.startTime).match(/\d+/)[0]))
            var date = new Date(data.startTime);
            $("#aac_Date").append($.datepicker.formatDate('dd M yy', date));
            $("#aac_Location").append(data.locationName);
            $("#aac_Area").append(data.sublocation);


            $.each(data.organizationNameS, function (index, value) {
               
                if (index != data.organizationNameS.length - 1) {
                    $("#aac_Org").append(value + " <br> ");
                } else {
                    $("#aac_Org").append(value);
                }

            });
            $("#aac_Desc").append(data.description == null ? "-" : data.description);
            $("#aac_PIC").append(data.pic == null ? "-" : data.pic);

            //fill in data to participant table
            if (res.participantModels != null)
                $.each(res.participantModels, function (index, value) {
                    console.log(value);
                    perticipantTable.row.add([(index + 1), value.name, value.nRIC, value.Contact, value.Role])
                    perticipantTable.on('order.dt search.dt', function () {
                        perticipantTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                            cell.innerHTML = i + 1;
                        });
                    }).draw();
                });
            getAACAllEquipment(id);
            GetAACVolunteerList(id);
        }
    });

}
function getAACAllEquipment(scheduleId) {
    $.ajax({
        url: '/Scheduling/GetAllEquipments?ScheId=' + scheduleId,
        type: 'GET',
        success: function (Json) {
            var p = $('#aac_EquipmentTable').DataTable();
            p.clear();
            p.draw();
            $.each(Json, function (index, value) {
                p.row.add([(index + 1), value.name, value.Quantity])
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
function GetAACVolunteerList(scheduleId) {

    $.ajax({
        url: '/VolunteerManagement/GetAACVolunteerList',
        type: 'GET',
        data: {
            ScheduleId: scheduleId
        },
        success: function (data) {
            var p = $('#aac_VolunteerTable').DataTable();
            console.log(data);
            p.clear();
            p.draw();
            var counter = 1;
            if (data.centerVolunteers.length > 0) {
                $.each(data.centerVolunteers, function (index, value) {
                    console.log(value);
                    if (value.AttendanceId != 0) {
                        p.row.add([counter, value.name, value.nRIC, value.Contact, "Center", "-",
                        ]);
                    }
                    else {
                        p.row.add([counter, value.name, value.nRIC, value.Contact, "Center(Not attended)", "-"]);
                    }
                    p.on('order.dt search.dt', function () {
                        p.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                            cell.innerHTML = i + 1;
                        });
                    }).draw();
                });
            }
            if (data.corporateVolunteers.length > 0) {
                $.each(data.corporateVolunteers, function (index, value) {
                    p.row.add([counter, value.name, value.nRIC, value.Contact, "Corporate", value.Organization]);
                    p.on('order.dt search.dt', function () {
                        p.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                            cell.innerHTML = i + 1;
                        });
                    }).draw();
                    counter = counter + 1;
                });
            }

        }
    });
}
//#endregion

//#region MET
function GetMETDetails(apptId) {
    $(".metappt-details").each(function () {
        $(this).empty();
    });
    $(".tripdetails").each(function () {
        $(this).empty();
    });
    $.ajax({
        url: '/VolunteerManagement/getAppointmentTripRecords',
        type: 'GET',
        data: {
            scheduleId: apptId
            //4969
        },
        success: function (data) {
            //console.log(data);

            //client details
            $("#metClient-name").append(data.clientName);
            $("#metClient-address").append(data.clientLocation);
            $("#metClient-unitno").append(data.unitNo);
            $("#metClient-mobileno").append(data.clientPhone);
            $("#metClient-homeno").append(ForNullofEmpty(data.homePhone));
            $("#metClient-wheelchair").append(data.wheelChair);
            $("#metClient-language").append(ForNullofEmpty(data.clientLanguage));

            //appt details
            $("#metAppt-apptDate").append(data.dateAppointment);
            $("#metAppt-apptStart").append(data.startTime1);
            $("#metAppt-status").append(data.status);
            $("#metAppt-clinic").append(data.buildingOrClinic);
            $("#metAppt-designation").append(data.designation);
            $("#metAppt-instruc").append(ForNullofEmpty(data.specialInstruction));
            $("#metAppt-remark").append(ForNullofEmpty(data.remark));

            //volunteer details

            $("#metVolunt-k1name").append(data.kaki1);
            $("#metVolunt-k1phone").append(data.kaki1Phone);
            $("#metVolunt-k2name").append(data.kaki2);
            $("#metVolunt-k2phone").append(data.kaki2Phone);
            $("#metVolunt-d1name").append(data.driver1);
            $("#metVolunt-d1phone").append(data.driver1Phone);
            $("#metVolunt-d2name").append(data.driver2);
            $("#metVolunt-d2phone").append(data.driver2Phone);

            if (data.appointmentRecords[0] != undefined) {
                //tripdetails(start)
                var createdtime = moment(data.appointmentRecords[0].created).format("hh:mm A")
                var clienttime = moment(data.appointmentRecords[0].clientTime).format("hh:mm A")
                $("#tripdetails-start-driver").append(data.appointmentRecords[0].driver1 + " - Tap card/confirm time: " + createdtime);

                $("#tripdetails-start-client").append(data.clientName + " - Tap card/confirm time: " + clienttime);
                var startEscort = "";
                if (data.appointmentRecords[0].peoples.length > 1) {
                    var mulEscort = "";
                    var currentLine = "";
                    $.each(data.appointmentRecords[0].peoples, function (index, value) {
                        var checkinTime = new Date(parseInt((value.Time).match(/\d+/)[0]))
                        currentLine = value.name + " - Tap card/confirm time:" + formatAMPM(checkinTime, 1) + "</br>";
                        mulEscort = mulEscort + currentLine;
                    });
                    startEscort = mulEscort;
                } else {
                    var checkinTime = new Date(parseInt((data.appointmentRecords[0].peoples[0].time).match(/\d+/)[0]))
                    startEscort = data.appointmentRecords[0].Peoples[0].Name + " - Tap card/confirm time:" + formatAMPM(checkinTime, 1);

                }
                $("#tripdetails-start-escort").append(startEscort);
                $("#tripdetails-start-odo").append(data.appointmentRecords[0].OdometerReading);


                const PwC = data.appointmentRecords[0].Reason.split("{<Content>}");
                const reasonArray = PwC[0].split(" ");
                var reason = reasonArray.join(", ")
                $("#tripdetails-start-personwithoutcard").append(reason);
                $("#tripdetails-start-remark").append(ForNullofEmpty(PwC[1]));

                var questionList = $.parseJSON(data.appointmentRecords[0].Questions);
                var questions = "";
                //console.log(questionList)
                $.each(questionList[0].AnswersList, function (index, value) {

                    var x = "<tr><th class='col-md-8'>" + value.Answers + ":" + "<th>" + "<td class='col-md-4'>" + (value.Status == true ? "Yes" : "No") + "<td>" + "</tr>";
                    questions = questions + x;
                });
                $("#tripdetails-start-status").append("<table>" + questions + "</table>");
            } else {
                $(".startTD").each(function () {
                    $(this).append("-");
                });
            }

            if (data.appointmentRecords[1] != undefined) {
                //trip details(Reach)
                $("#tripdetails-reach-driver").append(data.appointmentRecords[1].Driver1 + " - Tap card/confirm time: " + createdtime);

                $("#tripdetails-reach-client").append(data.ClientName + " - Tap card/confirm time: " + clienttime);
                var reachEscort = "";
                if (data.appointmentRecords[1].Peoples.length > 1) {
                    var mulEscort = "";
                    var currentLine = "";
                    $.each(data.appointmentRecords[1].Peoples, function (index, value) {
                        var checkinTime = new Date(parseInt((value.Time).match(/\d+/)[0]))
                        currentLine = value.name + " - Tap card/confirm time:" + formatAMPM(checkinTime, 1) + "</br>";
                        mulEscort = mulEscort + currentLine;
                    });
                    reachEscort = mulEscort;
                } else {
                    var checkinTime = new Date(parseInt((data.appointmentRecords[1].Peoples[0].Time).match(/\d+/)[0]))
                    reachEscort = data.appointmentRecords[1].Peoples[0].Name + " - Tap card/confirm time:" + formatAMPM(checkinTime, 1);

                }
                $("#tripdetails-reach-escort").append(reachEscort);
                $("#tripdetails-reach-odo").append(data.appointmentRecords[1].OdometerReading);

                const PwC = data.appointmentRecords[1].Reason.split("{<Content>}");
                const reasonArray = PwC[0].split(" ");
                var reason = reasonArray.join(", ")
                $("#tripdetails-reach-personwithoutcard").append(reason);
                $("#tripdetails-reach-remark").append(ForNullofEmpty(PwC[1]));

            } else {
                $(".reachTD").each(function () {
                    $(this).append("-");
                });
            }


            if (data.appointmentRecords[2] != undefined) {
                //tripdetails(return)
                $("#tripdetails-return-driver").append(data.appointmentRecords[2].Driver1 + " - Tap card/confirm time: " + createdtime);

                $("#tripdetails-return-client").append(data.ClientName + " - Tap card/confirm time: " + clienttime);
                var returnEscort = "";
                if (data.appointmentRecords[2].Peoples.length > 1) {
                    var mulEscort = "";
                    var currentLine = "";
                    $.each(data.appointmentRecords[2].Peoples, function (index, value) {
                        var checkinTime = new Date(parseInt((value.Time).match(/\d+/)[0]))
                        currentLine = value.name + " - Tap card/confirm time:" + formatAMPM(checkinTime, 1) + "</br>";
                        mulEscort = mulEscort + currentLine;
                    });
                    returnEscort = mulEscort;
                } else {
                    var checkinTime = new Date(parseInt((data.appointmentRecords[2].Peoples[0].Time).match(/\d+/)[0]))
                    returnEscort = data.appointmentRecords[2].Peoples[0].Name + " - Tap card/confirm time:" + formatAMPM(checkinTime, 1);

                }
                $("#tripdetails-return-escort").append(returnEscort);
                $("#tripdetails-return-odo").append(data.appointmentRecords[2].OdometerReading);


                const PwC = data.appointmentRecords[2].Reason.split("{<Content>}");
                const reasonArray = PwC[0].split(" ");
                var reason = reasonArray.join(", ")
                $("#tripdetails-return-personwithoutcard").append(reason);
                $("#tripdetails-return-remark").append(ForNullofEmpty(PwC[1]));
            } else {
                $(".returnTD").each(function () {
                    $(this).append("-");
                });
            }

            if (data.appointmentRecords[3] != undefined) {
                $("#tripdetails-end-driver").append(data.appointmentRecords[3].Driver1 + " - Tap card/confirm time: " + createdtime);

                $("#tripdetails-end-client").append(data.ClientName + " - Tap card/confirm time: " + clienttime);
                var endEscort = "";
                if (data.appointmentRecords[3].Peoples.length > 1) {
                    var mulEscort = "";
                    var currentLine = "";
                    $.each(data.appointmentRecords[3].Peoples, function (index, value) {
                        var checkinTime = new Date(parseInt((value.Time).match(/\d+/)[0]))
                        currentLine = value.name + " - Tap card/confirm time:" + formatAMPM(checkinTime, 1) + "</br>";
                        mulEscort = mulEscort + currentLine;
                    });
                    endEscort = mulEscort;
                } else {
                    var checkinTime = new Date(parseInt((data.appointmentRecords[3].Peoples[0].Time).match(/\d+/)[0]))
                    endEscort = data.appointmentRecords[3].Peoples[0].Name + " - Tap card/confirm time:" + formatAMPM(checkinTime, 1);

                }
                $("#tripdetails-end-escort").append(endEscort);
                $("#tripdetails-end-odo").append(data.appointmentRecords[3].OdometerReading);


                const PwC = data.appointmentRecords[2].Reason.split("{<Content>}");
                const reasonArray = PwC[0].split(" ");
                var reason = reasonArray.join(", ")
                $("#tripdetails-end-personwithoutcard").append(reason);
                $("#tripdetails-end-remark").append(ForNullofEmpty(PwC[1]));
                $("#tripdetails-end-mood").append(ForNullofEmpty(data.appointmentRecords[3].Mood));
            } else {
                $(".endTD").each(function () {
                    $(this).append("-");
                });
            }

            //$("#tripdetails-start-status").append(data.appointmentRecords[0].OdometerReading);
        }
    });

}
//#endregion

//#region Event
function RefreshCalendar() {
    calendar.removeAllEventSources();
    calendar.addEventSource("/VolunteerManagement/GetActivitySchedule");

}

function getvltParticipantTable(x) {
    if (x == 1) {//use when form creation
        var form_data = ptcpvlt.rows().data();
        var data = new Array();
        var arrayId = new Array();
        var emailArray = new Array();
        var nameArray = new Array();
        var contactArray = new Array();

        //console.log(form_data.length)
        if (form_data.length > 0) {

            $.each(form_data, function (index, value) {
                console.log(value)
                var Array = {
                    "UserId": value[6],
                    //Status:[Pending Sign Up ,Signed Up]
                    "Status": "Pending Sign Up",
                    //SignUpTime default empty
                    "SignUpTime": "",
                    //default be empty
                    "AttendanceTime": ""

                }

                data.push(Array);
                arrayId.push((value[6].toString()))
                emailArray.push((value[3]))
                nameArray.push((value[1]))
                contactArray.push(value[4])



            });

        }
        console.log(contactArray)
        //check if last compile array length
        if (data.length > 0) {
            $("#Jsonstr_vltParticipantsList").val(JSON.stringify(data));
            $("#VolunteerIdArray").val([arrayId])
            $("#emailArray").val([emailArray])
            $("#nameArray").val([nameArray])
            $("#contactArray").val([contactArray])


        } else {
            $("#Jsonstr_vltParticipantsList").val("");
            $("#VolunteerIdArray").val("")
        }
    }
    else if (x == 3) {
        var form_data = editbrptcpvltTbl.rows().data();
        var data = new Array();
        var arrayId = new Array();
        var emailArray = new Array();
        var nameArray = new Array();
        var contactArray = new Array();
        if (form_data.length > 0) {

            $.each(form_data, function (index, value) {
                var Array = {
                    "UserId": value[5],
                    //Status:[Pending Sign Up ,Signed Up]
                    "Status": "Pending Sign Up",
                    //SignUpTime default empty
                    "SignUpTime": "",
                    //default be empty
                    "AttendanceTime": ""

                }
                data.push(Array);


                arrayId.push((value[5].toString()))
                emailArray.push((value[3]))
                nameArray.push((value[1]))
                contactArray.push(value[4])
            });
            //console.log(output);
        }

        //check if last compile array length
        if (data.length > 0) {
            $("#edit_Jsonstr_vltParticipantsList").val(JSON.stringify(data));
            $("#edit_VolunteerIdArray").val([arrayId])
            $("#edit_emailArray").val([emailArray])
            $("#edit_nameArray").val([nameArray])
            $("#edit_contactArray").val([contactArray])
        } else {
            $("#edit_Jsonstr_vltParticipantsList").val("");
            $("#edit_VolunteerIdArray").val("")
        }
        console.log(arrayId)
    }
    else {
        var form_data = editptcpvltTbl.rows().data();
        var data = new Array();
        var arrayId = new Array();
        var emailArray = new Array();
        var nameArray = new Array();
        var contactArray = new Array();
        if (form_data.length > 0) {

            $.each(form_data, function (index, value) {
                var Array = {
                    "UserId": value[6],
                    //Status:[Pending Sign Up ,Signed Up]
                    "Status": "Pending Sign Up",
                    //SignUpTime default empty
                    "SignUpTime": "",
                    //default be empty
                    "AttendanceTime": ""

                }
                data.push(Array);


                arrayId.push((value[6].toString()))
                emailArray.push((value[3]))
                nameArray.push((value[1]))
                contactArray.push(value[4])
            });

        }

        //check if last compile array length
        if (data.length > 0) {
            $("#edit_Jsonstr_vltParticipantsList").val(JSON.stringify(data));
            $("#edit_VolunteerIdArray").val([arrayId])
            $("#edit_emailArray").val([emailArray])
            $("#edit_nameArray").val([nameArray])
            $("#edit_contactArray").val([contactArray])
        } else {
            $("#edit_Jsonstr_vltParticipantsList").val("");
            $("#edit_VolunteerIdArray").val("")
        }
        console.log(arrayId)
    }
}
function getBriefingRehersalTable(x) {
    if (x == 1) {//use when form creation
        var form_data = briefingRehersalTbl.rows().data();
        var data = new Array();
        if (form_data.length > 0) {

            $.each(form_data, function (index, value) {
                var Array = {
                    "Name": value[0],
                    "Date": value[1],
                    "Start": value[2],
                    "End": value[3],
                    "Location": value[4],
                    "ConductMode": value[5],
                    "Link": value[6],
                    "Remarks": value[7],
                }
                data.push(Array);
            });
        }

        console.log(data)

        //check if last compile array length
        if (data.length > 0) {
            $("#Jsonstr_BriefingRehersal").val(JSON.stringify(data));

        } else {
            $("#Jsonstr_BriefingRehersal").val("");
        }
    } else {
        var form_data = edit_briefingRehersalTbl.rows().data();

        var data = new Array();
        if (form_data.length > 0) {

            $.each(form_data, function (index, value) {
                var Array = {
                    "Name": value[0],
                    "Date": value[1],
                    "Start": value[2],
                    "End": value[3],
                    "Location": value[4],

                    "ConductMode": value[5],
                    "Link": value[6],
                    "Remarks": value[7]
                }
                data.push(Array);
            });
        }

        //console.log(data)
        //console.log(data.length)
        //check if last compile array length
        if (data.length > 0) {
            console.log('here')
            //console.log(JSON.stringify(data))
            $("#edit_Jsonstr_BriefingRehersal").val(JSON.stringify(data));


        } else {
            console.log('less than 0')
            $("#edit_Jsonstr_BriefingRehersal").val("");
        }
    }
}
function RestoreEvent(id) {
    Swal.fire({
        title: 'Do you want to restore this event?',
        showCancelButton: true,
        confirmButtonText: 'Confirm',
    }).then((result) => {

        if (result.isConfirmed) {
            $.ajax({
                url: '/VolunteerManagement/RestoreEvent?Id=' + id,
                type: 'POST',
                success: function (result) {
                    if (result.success) {//success
                        dialog(result.data.message, "success");
                        getArchivedTbl();

                        RefreshCalendar()
                    } else {
                        dialog(info.data.message, "error");
                    }
                }
            });
        }
    })
}
function DeleteEvent() {
    const id = $("#viewEvent_Id").val();

    Swal.fire({
        title: 'Do you want to delete this event?',
        showCancelButton: true,
        confirmButtonText: 'Confirm',
        width: 400,
        height: 400
    }).then((result) => {

        if (result.isConfirmed) {
            $.ajax({
                url: '/VolunteerManagement/DeleteEvent?Id=' + id,
                type: 'POST',
                success: function (result) {
                    if (result.success) {//success
                        dialog(result.data.message, "success");
                        $("#ViewEventModal").modal("hide");
                        //resetCreateActivityForm();
                        RefreshCalendar();
                        getArchivedTbl();
                    } else {
                        dialog(info.data.message, "error");
                    }
                }
            });
        }
    })
}
//function SendFeedback() {
//    const id = $("#viewEvent_Id").val();
//    var form_data = viewptcpvltTbl.rows().data();
//    var emailArray = new Array();
//    var nameArray = new Array();
//    if (form_data.length > 0) {

//        $.each(form_data, function (index, value) {
//            console.log(value)

//            emailArray.push((value[5]))
//            nameArray.push((value[1]))
//        });

//    }
//    Swal.fire({
//        title: 'Do you want to Send Feedback Email?',
//        showCancelButton: true,
//        confirmButtonText: 'Confirm',
//        width: 400,
//        height: 400
//    }).then((result) => {

//        if (result.isConfirmed) {
//            for (let x = 0; x < nameArray.length; x++) {
//                $.ajax({
//                    url: '/VolunteerManagement/SendFeedbackEmail?email=' + emailArray[x] + '&eventid=' + id + '&vltname=' + nameArray[x],
//                    type: 'POST',
//                    success: function (result) {
//                        console.log(result)
//                        OnSuccess(result)
//                        $("#ViewEventModal").modal("hide");
//                        resetCreateActivityForm();
//                        RefreshCalendar();
//                        getArchivedTbl();
//                    }
//                });
//            }
//        }
//    })
//}
function ViewEvent(id, multipleScheduleId, viewonly) {
    //console.log(multipleScheduleId)s
    //intialize data goes here
    $("#ViewEventActionSelection").removeClass("hidden");
    if (viewonly == 1) {//view only without edit and delete button
        $("#ViewEventActionSelection").addClass("hidden");
    }

    /* new QRCode(document.getElementById("qrcode"), "https://202.73.50.124:5587/VolunteerManagement/ActivityAttendance?eventId=" + id);*/
    //GetEventDetailsById
    console.log(id)
    console.log(multipleScheduleId)
    $.ajax({
        url: '/VolunteerManagement/GetEventDetailsById?Id=' + id + '&multipleScheduleId=' + 0,
        type: 'POST',
        success: function (data) {
            $(".eventDetails").each(function () {
                $(this).empty();

            });
            //if(data.Type)
            console.log(data);
            $("#ActivityType_span").html(data.activityType);
            if (data.module == 1) {
                $("#view_EventDetails_module1").show();
                $("#view_EventDetails_module2").hide();
                $("#view_EventDetails_module6").hide();
                $('#linked-event-table').hide();
                $('.main-event-header').addClass('hidden')
                $('#briefing-rehersal-view-tab').removeClass('hidden')

                $("#view-module1-date").append(moment(data.addEditModule.dates).format("Do MMM YYYY, dddd"));
                $("#view-module1-start").append(moment(data.addEditModule.startTime).format("hh:mm A"));
                $("#view-module1-end").append(moment(data.addEditModule.endTime).format("hh:mm A"));
                $("#view-module1-serviceprovider").append(ForNullofEmpty(data.addEditModule.serviceProvider));
                $("#view-module1-name").append(ForNullofEmpty(data.addEditModule.topic));
                $("#view-module1-desc").append(ForNullofEmpty(data.addEditModule.description));
                $("#view-module1-prerequisiteinfo").append(ForNullofEmpty(data.addEditModule.prerequisiteInfo));
                $("#view-module1-payment").append(ForNullofEmpty(data.addEditModule.payment));
                $("#view-module1-cost").append(ForNullofEmpty(data.addEditModule.cost));
                $("#view-module1-createdby").append(ForNullofEmpty(data.addEditModule.createdBy_Name));
                if (data.addEditModule.location == "Other") {
                    $("#view-module1-location").append(ForNullofEmpty(data.addEditModule.otherLocation));
                }
                else {
                    $("#view-module1-location").append(ForNullofEmpty(data.addEditModule.location));
                }


            }
            if (data.module == 2) {
                $("#view_EventDetails_module1").hide();
                $("#view_EventDetails_module2").show();
                $("#view_EventDetails_module6").hide();
                $('#linked-event-table').hide();
                $('.main-event-header').addClass('hidden')
                $('#briefing-rehersal-view-tab').removeClass('hidden')

                $("#view-module2-date").append(moment(data.addEditModule2.dates).format("Do MMM YYYY, dddd"))
                $("#view-module2-start").append(moment(data.addEditModule2.startTime).format("hh:mm A"))
                $("#view-module2-end").append(moment(data.addEditModule2.endTime).format("hh:mm A"))
                $("#view-module2-organiser").append(ForNullofEmpty(data.addEditModule2.organiserName));
                $("#view-module2-name").append(ForNullofEmpty(data.addEditModule2.name));
                $("#view-module2-desc").append(ForNullofEmpty(data.addEditModule2.description));
                $("#view-module2-pic").append(ForNullofEmpty(data.addEditModule2.pic));
                $("#view-module2-picContact").append(ForNullofEmpty(data.addEditModule2.piC_Contact));
                $("#view-module2-clientele").append(ForNullofEmpty(data.addEditModule2.clientele));
                $("#view-module2-type").append(ForNullofEmpty(data.addEditModule2.type));
                $("#view-module2-conductmode").append(ForNullofEmpty(data.addEditModule2.conductMode));
                $("#view-module2-createdby").append(ForNullofEmpty(data.addEditModule2.createdBy_Name));
                if (data.addEditModule2.location == "Other") {
                    $("#view-module2-location").append(ForNullofEmpty(data.addEditModule2.otherLocation));
                }
                else {
                    $("#view-module2-location").append(ForNullofEmpty(data.addEditModule2.location));
                }



            }

            if (data.module == 6) {
                $("#view_EventDetails_module1").hide();
                $("#view_EventDetails_module2").hide();
                $("#view_EventDetails_module6").show();
                $('.main-event-header').removeClass('hidden')
                $('#briefing-rehersal-view-tab').addClass('hidden')
                $('#linked-event-table').show();

                $("#view-module6-date").append(moment(data.addEditModule6.dates).format("Do MMM YYYY, dddd"))
                $("#view-module6-start").append(moment(data.addEditModule6.startTime).format("hh:mm A"))
                $("#view-module6-end").append(moment(data.addEditModule6.endTime).format("hh:mm A"))
                $('#view-module6-remark').append(ForNullofEmpty(data.addEditModule6.remarks))
                $("#view-module6-name").append(ForNullofEmpty(data.addEditModule6.name));
                //$("#view-module6-location").append(ForNullofEmpty(data.addEditModule6.location));
                if (data.addEditModule6.location == "Other") {
                    $("#view-module6-location").append(ForNullofEmpty(data.addEditModule6.otherLocation));
                }
                else {
                    $("#view-module6-location").append(ForNullofEmpty(data.addEditModule6.location));
                }
                $("#view-module6-conductmode").append(ForNullofEmpty(data.addEditModule6.conductMode));
                $("#view-module6-link").append(ForNullofEmpty(data.addEditModule6.link));
                $("#view-module6-createdby").append(ForNullofEmpty(data.addEditModule6.createdBy_Name));

                $.ajax({
                    url: '/VolunteerManagement/GetEventDetailsById?Id=' + data.addEditModule6.linkedEventId + '&multipleScheduleId=0',
                    type: "POST",
                    success: function (data) {
                        $('#linked-event-table').removeClass('hidden')
                        $("#view-module6-mainevent").append(ForNullofEmpty(data.addEditModule2.name));
                        $("#view-module6-mainevent-date").append(ForNullofEmpty(moment(data.addEditModule2.dates).format("Do MMM YYYY, dddd")));
                    }

                })


            }



            if (data.addEditModule3 != null) {
                $("#viewCSRSSASpan").show();
                $("#view_EventDetails_collaboration").show();

                $("#view-collaboration-modeCollab").append(data.addEditModule3.modeofCollab);
                $("#view-collaboration-activityrole").append(data.addEditModule3.activityRole);
                if (data.addEditModule3.SSA == null) {
                    $(".ViewSSATable").hide();
                } else {
                    $(".ViewSSATable").show();
                    viewSSAPIC.clear().draw();
                    $.each(data.addEditModule3.SSA_PIC, function (index, value) {
                        viewSSAPIC.row.add([index + 1, value.Title, value.name, ForNullofEmpty(value.Email), ForNullofEmpty(value.Position), ForNullofEmpty(value.ContactNo_Mobile), ForNullofEmpty(value.ContactNo_Office)]).node();
                    })
                    viewSSAPIC.draw(false);
                }
                if (data.addEditModule3.CSR == null) {
                    $(".ViewCSRTable").hide();
                } else {
                    $(".ViewCSRTable").show();
                    viewCSRPIC.clear().draw();
                    $.each(data.addEditModule3.CSR_PIC, function (index, value) {
                        viewCSRPIC.row.add([index + 1, value.Title, value.name, ForNullofEmpty(value.Email), ForNullofEmpty(value.ContactNo_Mobile), ForNullofEmpty(value.ContactNo_Office)]).node();
                    })
                    viewCSRPIC.draw(false);
                }
                $("#view-collaboration-SSA").append(data.addEditModule3.SSA);
                $("#view-collaboration-CSR").append(data.addEditModule3.CSR);
            } else {
                $("#view_EventDetails_collaboration").hide();
                $("#viewCSRSSASpan").hide();
            }




            //INITIALIZE PARTCIPANTS TABLE
            $.ajax({
                url: '/VolunteerManagement/GetVolunteerListByEvent?EventId=' + id + '&status=2',
                type: 'POST',
                success: function (result) {
                    //console.log(result)
                    $("#view_ptcpvlttable").DataTable(
                        {
                            bLengthChange: true,
                            AutoWidth: false,
                            lengthMenu: [[5, 10, -1], [5, 10, "All"]],
                            bFilter: true,
                            bSort: true,
                            bPaginate: true,
                            responsive: true,
                            data: result,
                            bDestroy: true,

                            columns: [{ 'data': 'Name' },
                            { 'data': 'NRIC' },
                            { 'data': 'Email' },
                            { 'data': 'Contact' },
                            { 'data': 'SignedUp' },
                            { 'data': 'Attended' },


                            ],

                        })
                }

            })
            //INITALIZE BRIEFINGREHERSALLIST TABLE

            viewBriefingRehersalTbl.clear().draw();
            if (data.briefingRehersalList != null) {
                if (data.briefingRehersalList.length > 0) {
                    $.each(data.briefingRehersalList, function (index, value) {
                        date = moment(value.date).format("DD MMM yyyy, dddd")
                        start = moment(value.start).format("HH:mm ")
                        end = moment(value.end).format("HH:mm ")
                        viewBriefingRehersalTbl.row.add([index + 1, value.name, date, start, end, value.location, value.ConductMode, value.Link, value.Remarks]).node();
                    });

                    viewBriefingRehersalTbl.draw(false);
                }
            }

        }
    });

    $("#ViewEventModal").modal("show");

}
function getArchivedTbl() {
    $.ajax({
        url: 'GetArchivedEventTable',
        method: "GET",
        success: function (data) {
            archivedTbl.clear().draw();
            //console.log(data)
            $.each(data, function (index, value) {
                archivedTbl.row.add([(index + 1), value.name, moment(value.start).format("DD/MM/yyyy hh:mm A"), moment(value.end).format("DD/MM/yyyy hh:mm A"),
                "<a class='fa fa-eye' style='cursor:pointer;margin-right:5px;' title='Details' onclick='ViewEvent(" + value.id + ",0)'></a>" +
                "<a class='fas fa-trash-restore' style='cursor:pointer;' title='Restore' onclick='RestoreEvent(" + value.id + ")'></a>"
                ]).node()

            });
            archivedTbl.draw(false);

            //$(".display").DataTable();
        }
    });
}
function UpdateEvent() {
    const id = $("#viewEvent_Id").val();

    getvltParticipantTable(2);
    getBriefingRehersalTable(2);
    $.ajax({
        url: '/VolunteerManagement/GetEventDetailsByIdOnly2?Id=' + id,
        type: 'POST',
        success: function (data) {

            $(".eventDetails").each(function () {
                $(this).empty();

            });
            //if(data.Type)
            console.log(data);
            $("#ActivityType_span").html(data.activityType);
            if (data.module == 1) {
                $("#edit-ModuleId").val(data.module);
                $("#edit_module1").show();
                $("#edit_module2").hide();
                $("#edit_module6").hide();
                $('#main-event-vlt-tbl').hide()
                $('#edit_briefing-rehersal-tab').removeClass('hidden')
                $('#selected-br-vlt-tbl').hide()
                $('#participantandVolunteerTab2').show()


                $("#edit-Id").val(data.addEditModule.id);
                $("#edit-created").val(data.addEditModule.createdBy);
                $("#edit-ActivityType").val(data.activityType);
                $("#edit-module1-date").val(moment(data.addEditModule.dates).format("DD/MM/YYYY"));

                $("#edit-startTime").val(moment(data.addEditModule.startTime).format("hh:mm A"));
                $("#edit-endTime").val(moment(data.addEditModule.endTime).format("hh:mm A"));
                $("#edit_module1_ddlLocation").val(data.addEditModule.location);
                if (data.addEditModule.otherLocation != null) {
                    $("#edit_module1_otherlocation").removeClass("hidden");
                    $("#edit_module1_otherlocation").val(data.addEditModule.otherLocation)
                }
                $("#edit_module1_locationame").val(data.addEditModule.locationName)
                $("#edit-ServiceProvider").val(data.addEditModule.serviceProvider);
                $("#edit-ddlTopic").val(data.addEditModule.topic);
                $("#edit-Desc").val(data.addEditModule.description);
                $("#edit-prerequisiteinfo").val(data.addEditModule.prerequisiteInfo);
                $("#edit_ddlPayment").val(data.addEditModule.payment);
                $("#ddlCost").val(data.addEditModule.cost);
                $("#edit-module1-createdby").val(data.addEditModule.createdBy_Name);
                $(".edit_ddlcollaboration").val(data.addEditModule.collaboration);
                if (data.addEditModule.collaboration == "Yes") {
                    $("#edit_CSRorPartnerModule").removeClass("hidden");
                    //$("#edit_ddlModeCollab").val(result.AddEditModule3.modeofCollab);
                    //$("#edit_ddlSSA").val(result.AddEditModule3.SSA);
                    //$("#edit_ddlCSR").val(result.AddEditModule3.CSR);

                }
                else {
                    $("#edit_CSRorPartnerModule").addClass("hidden");
                }

            }
            else if (data.module == 2) {
                console.log("here is module2");
                $("#edit_module1").hide();
                $("#edit_module2").show();
                $("#edit_module6").hide();
                $('#main-event-vlt-tbl').hide()
                $('#selected-br-vlt-tbl').hide()
                $('#edit_briefing-rehersal-tab').removeClass('hidden')

                $('#participantandVolunteerTab2').show()



                /*                $("#edit2-ModuleId").val(data.module);*/
                $("#edit-ModuleId").val(data.module);
                $("#edit-ActivityType").val(data.activityType);
                $("#edit2-Id").val(data.addEditModule2.id);
                $("#edit2-created").val(data.addEditModule2.createdBy);
                /*            $("#edit2-ActivityType").val(data.activityType);*/
                $("#edit_module2_date").val(moment(data.addEditModule2.dates).format("DD/MM/YYYY"));
                $("#edit_module2_starttime").val(moment(data.addEditModule2.startTime).format("hh:mm A"));
                $("#edit_module2_endtime").val(moment(data.addEditModule2.endTime).format("hh:mm A"));
                $("#edit_ddlorganiser").val(data.addEditModule2.organiser);
                $("#edit_module2_name").val(data.addEditModule2.name);
                $("#edit_module2_desc").val(data.addEditModule2.description);
                $("#editPIC").val(data.addEditModule2.pic);
                $("#edit_picContact").val(data.addEditModule2.piC_Contact);
                $("#edit_ddlclientele").val(data.addEditModule2.clientele);
                $("#edit_ddlActivityType").val(data.addEditModule2.type);
                $("#edit_ddlLocation").val(data.addEditModule2.location);
                if (data.addEditModule2.otherLocation != null) {
                    $("#edit_otherlocation").removeClass("hidden");
                    $("#edit_otherlocation").val(data.addEditModule2.otherLocation)
                }
                $("#edit_locationame").val(data.addEditModule2.locationName)
                $("#edit_ddlConductMode").val(data.addEditModule2.conductMode);
                $("#view-module2-createdby").val(ForNullofEmpty(data.addEditModule2.createdBy_Name));
                $(".edit_ddlcollaboration").val(data.addEditModule2.collaboration);
                if (data.addEditModule2.collaboration == "Yes") {
                    $("#edit_CSRorPartnerModule").removeClass("hidden");
                    //$("#edit_ddlModeCollab").val(result.AddEditModule3.modeofCollab);
                    //$("#edit_ddlSSA").val(result.AddEditModule3.SSA);
                    //$("#edit_ddlCSR").val(result.AddEditModule3.CSR);

                }
                else {
                    $("#edit_CSRorPartnerModule").addClass("hidden");
                }
            }
            else if (data.module == 6) {
                $("#edit-ModuleId").val(data.module);
                $("#edit_module1").hide();
                $("#edit_module2").hide();
                $("#edit_module6").show();
                $('#main-event-vlt-tbl').hide()
                $('#selected-br-vlt-tbl').show()
                $('#edit_briefing-rehersal-tab').addClass('hidden')
                $('#main-event-vlt-header').removeClass('hidden')
                $('#main-event-vlt-tbl').show()
                $('#participantandVolunteerTab2').hide()



                //hidden field

                $("#edit6-ActivityType").val(data.activityType);
                $("#edit6-Id").val(data.addEditModule6.id);
                $("#edit6-created").val(data.addEditModule6.createdBy);
                $('#edit6-linked-Id').val(data.addEditModule6.linkedEventId);


                //visible column

                $("#edit_module6_date").val(moment(data.addEditModule6.dates).format("DD/MM/YYYY"));
                $("#edit_module6_starttime").val(moment(data.addEditModule6.startTime).format("hh:mm A"));
                $("#edit_module6_endtime").val(moment(data.addEditModule6.endTime).format("hh:mm A"));
                $("#edit_module6_name").val(data.addEditModule6.name);
                $("#edit6_ddlConductMode").val(data.addEditModule6.conductMode);
                $("#edit_module6_location").val(data.addEditModule6.location);
                $("#edit_module6_link").val(data.addEditModule6.link);
                $("#edit_module6_remark").val(data.addEditModule6.remarks);

                function getBrVlt(data) {
                    $('#edit6_ptcp_vlt_table').DataTable().columns.adjust().draw(false);
                    let idArray = []

                    $.ajax({
                        // Your URL, type, data, etc.
                        url: '/VolunteerManagement/GetEventDetailsById?Id=' + data + '&multipleScheduleId=0',
                        type: 'POST',
                        success: function (result) {



                            // Your success logic here
                            console.log('populate second table');
                            console.log(result);
                            editbrptcpvltTbl.clear().draw();

                            $.each(result.vltParticipantsList, function (index, value) {
                                if (value.userId) {
                                    $("#check-volunteer[data-id='" + value.userId + "']").prop("checked", true);
                                }
                                if (value.Email == null) {
                                    editbrptcpvltTbl.row.add([index + 1, value.name, value.nRIC, 'NA', value.contactNo, value.userId,]).node();
                                } else {
                                    editbrptcpvltTbl.row.add([index + 1, value.name, value.nRIC, value.Email, value.contactNo, value.userId,]).node();
                                }

                                idArray.push(value.userId)
                            });
                            editbrptcpvltTbl.draw(false);
                        }
                    });
                    return idArray
                }


                let array = getBrVlt(data.addEditModule6.id);




                function getMainEventVlt(array) {



                    $.ajax({
                        url: '/VolunteerManagement/GetEventDetailsById?Id=' + data.addEditModule6.linkedEventId + '&multipleScheduleId=0',
                        type: 'POST',
                        success: function (data) {
                            console.log('populate main event volunteer list')
                            console.log(data)

                            $('#edit6_ptcp_vlt_table').DataTable().draw();
                            edit6ptcpvltTbl.clear().draw();

                            if (data.vltParticipantsList != null) {

                                var selectAll = "<input type='checkbox' id='select-all-volunteers'> Select All";
                                edit6ptcpvltTbl.row.add(["", "", "", "", "", selectAll, ""]).node();



                                if (data.vltParticipantsList.length > 0) {
                                    $.each(data.vltParticipantsList, function (index, value) {
                                        var checkbox = "<input type='checkbox' data-id='" + value.userId + "' data-text='" + value.name + "|" + value.nRIC + "|" + value.Email + "|" + value.contactNo + "' id='check-volunteer'>";

                                        if (array.indexOf(value.userId) !== -1) {
                                            //checkbox = "<input type='checkbox' data-id='" + value.userId + "' id='check-volunteer' checked>";
                                            checkbox = "<input type='checkbox' data-id='" + value.userId + "' data-text='" + value.name + "|" + value.nRIC + "|" + value.Email + "|" + value.contactNo + "' id='check-volunteer' checked>";

                                        }
                                        if (value.Email == null) {
                                            edit6ptcpvltTbl.row.add([index + 1, value.name, value.nRIC, 'NA', value.contactNo, checkbox, value.userId,]).node();
                                        } else {
                                            edit6ptcpvltTbl.row.add([index + 1, value.name, value.nRIC, value.Email, value.contactNo, checkbox, value.userId,]).node();
                                        }
                                    });
                                    edit6ptcpvltTbl.draw(false);
                                }

                                function selectAllBoxes(check) {
                                    var checkboxes = document.querySelectorAll('#check-volunteer');
                                    for (var i = 0; i < checkboxes.length; i++) {
                                        checkboxes[i].checked = check;
                                    }
                                    updateBrVltTbl()
                                }


                                $("#select-all-volunteers").click(function () {
                                    var isChecked = $(this).prop("checked");
                                    selectAllBoxes(isChecked);
                                    console.log('click select all')
                                });
                            }




                        }

                    });

                }

                getMainEventVlt(array)



            }

            if (data.addEditModule3 != null) {
                $("#edit_CSRorPartnerModule").show();

                $("#edit_ddlModeCollab").val(data.addEditModule3.modeofCollab);
                $("#edit_ddlActivtiyRole").val(data.addEditModule3.activityRole);
                $("#editddlSSA").val(data.addEditModule3.SSAId);
                $("#editddlCSR").val(data.addEditModule3.CSRId);
                $("#edit_NoOfVolunteer").val(data.addEditModule3.NoOfVolunteer);

                // JSON data
                var jsonValue = data.addEditModule3.SSAId.toString();
                var jsonText = data.addEditModule3.SSA.toString();

                // Find the <select> element by its ID
                var selectElement = document.getElementById("editddlSSA");

                // Find the <option> element with the specified value
                var optionToSelect = selectElement.querySelector("option[value='" + jsonValue + "']");

                if (optionToSelect) {
                    // Set the "selected" attribute to select the option
                    optionToSelect.selected = true;
                } else {
                    console.log("Option not found with value: " + jsonValue);
                }

                // Find all the button elements with the specified title
                var buttonElements = document.querySelectorAll(".multiselect-option[title='" + jsonText + "']");

                // Loop through all matching buttons and perform the desired actions
                buttonElements.forEach(function (buttonElement) {
                    // Find the checkbox inside the button
                    var checkboxElement = buttonElement.querySelector(".form-check-input");

                    // Check the checkbox
                    checkboxElement.checked = true;

                    // Update the selected text in the dropdown
                    var selectedTextElement = buttonElement.closest(".multiselect-native-select").querySelector(".multiselect-selected-text");
                    selectedTextElement.textContent = jsonText;
                });

                //CSR
                // JSON data
                jsonValue = data.addEditModule3.CSRId.toString();
                jsonText = data.addEditModule3.CSR.toString();

                selectElement = document.getElementById("editddlCSR");

                optionToSelect = selectElement.querySelector("option[value='" + jsonValue + "']");

                if (optionToSelect) {
                    // Set the "selected" attribute to select the option
                    optionToSelect.selected = true;
                } else {
                    console.log("Option not found with value: " + jsonValue);
                }

                buttonElements = document.querySelectorAll(".multiselect-option[title='" + jsonText + "']");

                // Loop through all matching buttons and perform the desired actions
                buttonElements.forEach(function (buttonElement) {
                    // Find the checkbox inside the button
                    var checkboxElement = buttonElement.querySelector(".form-check-input");

                    // Check the checkbox
                    checkboxElement.checked = true;

                    // Update the selected text in the dropdown
                    var selectedTextElement = buttonElement.closest(".multiselect-native-select").querySelector(".multiselect-selected-text");
                    selectedTextElement.textContent = jsonText;
                });
            }



            //INITIALIZE PARTCIPANTS TABLE
            //console.log(data);



            editptcpvltTbl.clear().draw();

            if (data.vltParticipantsList != null) {
                if (data.vltParticipantsList.length > 0) {


                    $.each(data.vltParticipantsList, function (index, value) {

                             editptcpvltTbl.row.add([index + 1, value.name, value.nric, value.email, value.contactNo, "<a class='fas fa-trash' style='cursor:pointer;'></a>", value.userId,]).node();
      

                    });
                    editptcpvltTbl.draw(false);
                }
            }

            if (data.module != 6) {

                //INITALIZE BRIEFINGREHERSALLIST TABLE

                view_briefingRehersalTbl.clear().draw();
                if (data.briefingRehersalList.length > 0) {
                    $.each(data.briefingRehersalList, function (index, value) {
                        date = moment(value.date).format("DD MMM yyyy, dddd")
                        start = moment(value.start).format("HH:mm ")
                        end = moment(value.end).format("HH:mm ")
                        view_briefingRehersalTbl.row.add([value.name, date, start, end, value.location, value.ConductMode, value.Link, value.Remarks]).node();
                    });



                    view_briefingRehersalTbl.draw(false);
                }
            }



        }
    });

    $("#UpdateActivityModal").modal("show");



}





///show qr code with event id
function GenerateQRCode() {
    const id = $("#viewEvent_Id").val();
    var div = document.getElementById("qrcode");
    new QRCode(document.getElementById("qrcode"), "http://45.32.105.34:5587/VolunteerManagement/ActivityAttendance?eventId=" + id, image = "https://202.73.50.124:5585/assets/image/emailHeader.png");
    div.removeChild(div.firstChild);



}
function resetQR() {
    var div = document.getElementById("qrcode");
    div.removeChild(div.firstChild);
}


// manual attendance table
$("#manual-attendance").on('shown.bs.modal', function () {
    var eventId = $("#viewEvent_Id").val();
    $.ajax({
        url: '/VolunteerManagement/GetEventDetailsById?Id=' + eventId + '&multipleScheduleId=0',
        type: 'POST',
        success: function (data) {
            //console.log(data)
            if (data.module == 1) {
                var name = data.addEditModule.serviceProvider
                $('#event-name').text(name)
            }
            else if (data.module == 2) {
                var name = data.addEditModule2.name
                $('#event-name').text(name)
            }
        }
    })


    getUnvalidatedVolunteer(eventId)
    getManualAttendanceList(eventId)

});


// clear the volunteer table everytime we close the modal and click another event
$("#manual-attendance").on('hidden.bs.modal', function (e) {
    var table = $('#attendance-table').DataTable();
    //var table2 = $('#unattended-volunteer-table').DataTable();
    table.destroy();
    $('#unvalidated-volunteer-table').DataTable().destroy();
    //table2.destroy();

    //window.location.reload();

})





//function handleAddAttendance(eventId) {
//    var buttons = document.querySelectorAll('#add-attd-btn')
//    buttons.forEach(button => {
//        button.addEventListener('click', e => {
//            var id = button.dataset.id
//            console.log(id)
//            console.log(eventId)
//            $.ajax({

//                url: '/VolunteerManagement/CheckDuplicateAttendance?EventId=' + eventId + '&UserId=' + id,
//                type: 'POST',
//                success: function (result) {


//                    if (result == 'False') {
//                        $.ajax({
//                            url: '/VolunteerManagement/UpdateAttendanceForEvent?EventId=' + eventId + '&UserId=' + id,
//                            type: 'POST',
//                            success: function (result) {
//                                console.log(result)
//                                if (result) {


//                                    let timerinterval
//                                    swal.fire({
//                                        icon: 'success',
//                                        title: 'Attendance Added',
//                                        timer: 1300,


//                                    }).then((result) => {

//                                        $("#manual-attendance").modal('toggle')
//                                    })

//                                    $("#manual-attendance").modal('toggle');



//                                } else {

//                                    swal.fire({
//                                        icon: 'error',
//                                        title: 'error',
//                                        text: 'Error Please Try Again'

//                                    })
//                                    return;
//                                }


//                            }
//                        })

//                    }
//                    else if (result == 'True') {
//                        swal.fire({
//                            icon: 'warning',
//                            text: 'Duplicate Attendance',
//                            title: 'error'
//                        })
//                    }
//                }
//            })


//        })
//    })

//}

function handleAddAttendance(eventId, userId) {
    console.log(userId);
    console.log(eventId);
    $.ajax({
        url: '/VolunteerManagement/CheckDuplicateAttendance?EventId=' + eventId + '&UserId=' + userId,
        type: 'POST',
        success: function (result) {
            if (result == 'False') {
                $.ajax({
                    url: '/VolunteerManagement/UpdateAttendanceForEvent?EventId=' + eventId + '&UserId=' + userId,
                    type: 'POST',
                    success: function (result) {
                        if (result) {
                            swal.fire({
                                icon: 'success',
                                title: 'Attendance Added',
                                timer: 1300,
                            }).then((result) => {
                                $("#manual-attendance").modal('toggle');
                            });
                            $("#manual-attendance").modal('toggle');
                        } else {
                            swal.fire({
                                icon: 'error',
                                title: 'error',
                                text: 'Error Please Try Again'
                            });
                            return;
                        }
                    }
                });
            } else if (result == 'True') {
                swal.fire({
                    icon: 'warning',
                    text: 'Duplicate Attendance',
                    title: 'error'
                });
            }
        }
    });
}


function getManualAttendanceList(eventId) {
    var emailArray = new Array();
    var nameArray = new Array();
    $.ajax({
        url: '/VolunteerManagement/GetVolunteerListByEvent?EventId=' + eventId + '&status=1',
        type: 'POST',
        success: function (attendedResult) {
            for (let i = 0; i < attendedResult.length; i++) {
                attendedResult[i].Attendance = '<span class="text-success"><i class="fas fa-check"></i></span>';
                emailArray.push(attendedResult[i].Email);
                nameArray.push(attendedResult[i].Name);
            }
            $("#attendance-table").DataTable(
                {
                    bLengthChange: true,
                    AutoWidth: false,
                    lengthMenu: [[5, 10, -1], [5, 10, "All"]],
                    bFilter: true,
                    bSort: true,
                    bPaginate: true,
                    responsive: true,
                    data: attendedResult,
                    columns: [
                        { 'data': 'Name' },
                        { 'data': 'NRIC' },
                        { 'data': 'Contact' },
                        { 'data': 'SignedUp' },
                        {
                            'data': 'Attendance'
                        },

                    ],
                }
            );

            $.ajax({
                url: '/VolunteerManagement/GetVolunteerListByEvent?EventId=' + eventId + '&status=0',
                type: 'POST',
                success: function (unattendedResult) {
                    for (let i = 0; i < unattendedResult.length; i++) {
                        unattendedResult[i].Attendance = '<button type="button" class="btn btn-info add-attd-btn" data-id="' + unattendedResult[i].UserId + '">Add </button>';
                    }
                    $("#attendance-table").DataTable().rows.add(unattendedResult).draw();
                    $("#attendance-table").off("click", ".add-attd-btn").on("click", ".add-attd-btn", function () {
                        var id = $(this).data("id"); // get the volunteer ID from the button's data attribute
                        handleAddAttendance(eventId, id); // pass the event ID and volunteer ID to the handleAddAttendance function
                    });
                },

            })

        }
    });
}



//function handleAddAttendance(eventId) {
//    var buttons = document.querySelectorAll('#add-attd-btn')
//    buttons.forEach(button => {
//        button.addEventListener('click', e => {
//            var id = button.dataset.id;
//            console.log('click')
//            $.ajax({
//                url: '/VolunteerManagement/CheckDuplicateAttendance?EventId=' + eventId + '&UserId=' + id,
//                type: 'POST',
//                success: function (result) {
//                    if (result == 'False') {
//                        $.ajax({
//                            url: '/VolunteerManagement/UpdateAttendanceForEvent?EventId=' + eventId + '&UserId=' + id,
//                            type: 'POST',
//                            success: function (result) {
//                                if (result) {
//                                    swal.fire({
//                                        icon: 'success',
//                                        title: 'Attendance Added',
//                                        timer: 1300,
//                                    }).then(() => {
//                                        // close the modal
//                                        $("#manual-attendance").modal('toggle');
//                                        // clear the table

//                                        // reload the attendance table with updated data
//                                        //getManualAttendanceList(eventId);
//                                    });
//                                        $("#manual-attendance").modal('toggle');
//                                } else {
//                                    swal.fire({
//                                        icon: 'error',
//                                        title: 'error',
//                                        text: 'Error Please Try Again'
//                                    });
//                                    return;
//                                }
//                            }
//                        });
//                    } else if (result == 'True') {
//                        swal.fire({
//                            icon: 'warning',
//                            text: 'Duplicate Attendance',
//                            title: 'error'
//                        });
//                    }
//                }
//            });
//        });
//    });
//}







function tryAutoEmail() {
    const id = $("#viewEvent_Id").val();
    const end = $("#view-module1-end").val();
    var emailArray = new Array();
    var nameArray = new Array();
    var idArray = new Array();
    var contactArray = new Array();
    Swal.fire({
        title: 'Do you want to Send Feedback Email?',
        showCancelButton: true,
        confirmButtonText: 'Confirm',
        width: 400,
        height: 400,
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/VolunteerManagement/GetVolunteerListByEvent?EventId=' + id + '&status=4',
                type: 'POST',
                success: function (result) {
                    console.log(result)
                    for (let i = 0; i < result.length; i++) {
                        emailArray.push(result[i].Email)
                        nameArray.push(result[i].Name)
                        idArray.push(result[i].id)
                        contactArray.push(result[i].Contact)
                    }
                    for (let x = 0; x < nameArray.length; x++) {

                        $.ajax({
                            url: '/VolunteerManagement/SendFeedbackEmail?email=' + emailArray[x] + '&eventid=' + id + '&vltname=' + nameArray[x] + '&eventEnd=',
                            type: 'POST',
                            success: function (result) {
                                console.log(result)
                                OnSuccess(result)
                                RefreshCalendar();
                                getArchivedTbl();
                            }
                        });

                    }


                }

            })

        }
    })
}
//#endregion



function ddlSSA_change() {
    var selected = [];
    for (var option of document.getElementById('ddlSSA').options) {
        if (option.selected) {
            selected.push(option.value);
        }
    }
    $('#ssaArray').val(selected);
}
function ddlCSR_change() {
    var selected = [];
    for (var option of document.getElementById('ddlCSR').options) {
        if (option.selected) {
            selected.push(option.value);
        }
    }
    $('#csrArray').val(selected);
}

function getUnvalidatedVolunteer(eventId) {
    $('#unvalidated-volunteer-table').DataTable({
        ajax: {
            url: '/VolunteerManagement/ListOfUnvalidatedVolunteerAttendedEvent?EventId=' + eventId,
            type: 'POST',
            dataSrc: 'data',
            success: function (data) {
                console.log(data)
                data.forEach(item => {
                    $('#unvalidated-volunteer-table').DataTable().row.add([
                        item.name,
                        item.NRIC,
                        item.contactNo
                    ]).draw();
                })
            },

            fixedHeader: true,
            responsive: true,

        },



    })
}

$('#submit-nric').click((e) => {
    e.preventDefault();
    console.log('click')
    var eventId = $("#viewEvent_Id").val();
    var NRIC = document.querySelector('#nric-input').value;
    checkDuplicateNRICForEvent(eventId, NRIC)


})

function checkDuplicateNRICForEvent(eventId, NRIC) {
    $.ajax({
        url: '/VolunteerManagement/CheckDuplicateAttendanceByNRIC?EventId=' + eventId + '&NRIC=' + NRIC,
        type: 'POST',
        success: function (result) {
            console.log(result)
            if (result == 'True') {
                Swal.fire({
                    text: 'Attendance Existed'
                })
            } else {
                validateNRIC(eventId, NRIC)

            }
        }
    })

}

function validateNRIC(eventId, NRIC) {

    $.ajax({
        url: '/VolunteerManagement/ValidateNRICForAttendance?Ic=' + NRIC,
        type: 'POST',
        success: function (data) {
            if (data.VolunteerId == 0) {
                Swal.fire({
                    text: 'User Not Found'
                })
            } else {
                var userId = data.VolunteerId
                var name = data.Name
                var event = eventId
                Swal.fire({
                    text: 'Add ' + data.Name + ' To Activity Attendance?',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.value) {
                        //new swal('added')
                        console.log(event)
                        console.log(userId)
                        $.ajax({
                            url: '/VolunteerManagement/UpdateAttendanceForEvent?EventId=' + event + '&UserId=' + userId,
                            type: 'POST',


                            success: function (data) {
                                console.log(data)
                                if (data == 'True') {
                                    Swal.fire({
                                        title: 'Success',
                                        text: 'Attendance Recorded',

                                    }).then(result => {
                                        $("#manual-attendance").modal('toggle');
                                        document.querySelector('#nric-input').value = ''
                                    })

                                    $("#manual-attendance").modal('toggle');
                                }
                                else {
                                    Swal.fire({
                                        title: 'Welcome',
                                        text: 'Duplicate Attendance ',

                                    }).then(result => {
                                        document.querySelector('#nric-input').value = ''
                                    })
                                }
                            },
                            error: function error(_error) {
                                //return "Error(\"" + _error.message + "\")";
                                console.log(_error)
                            }

                        });
                    } else if (result.dismiss === Swal.DismissReason.cancel) {
                        new swal('Cancelled')
                    }
                })

            }
        }
    })

}

// #end region 

//return to "-" if value is empty of null
function ForNullofEmpty(value) {
    var result;
    result = value;
    if (value == null || value == "")
        result = "-";

    return result;
}
function formatAMPM(dt, type) {
    var date = new Date(dt);
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
    if (type = 2) {
        strTime = hours + ":" + minutes + " " + ampm;
    }

    return strTime;
}

function onchangeStartTime() {
    startDate = $('#module2_startDate').val();
    endDate = $('#module2_endDate').val();
    var a = $.datepicker.formatDate('dd, mm, yyyy', new Date(endDate))
    console.log(a)
    console.log(endDate)
    console.log(endDate > startDate)
    console.log(endDate == startDate)
}
function OnSuccess(result) {
    if (result.success) {
        let timerInterval
        Swal.fire({
            icon: 'success',
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
            title: 'Error',
            text: result.data.message

        })
        return;
    }

}
function OnFailure(result) {
    console.log("failure");
    console.log(result);
}



function resetCreateActivityForm() {
    var form = $(MEL.toSelector('createActivity-form'));
    form.formReset();
    $('#otherlocation').val('')
    $('#locationName').val('')
    $("#ddlTopic").trigger("change");//hide or show ddl others field
    $("#ddlPrerequisite").trigger("change");
    $("#ddlParticipants").multiselect('refresh');
    $("#edit_ddlParticipants").multiselect('refresh');
    $("#ddlSSA").multiselect('refresh');
    $("#ddlCSR").multiselect('refresh');
    


    ptcpvlt.clear().draw();
    editptcpvltTbl.clear().draw();
    briefingRehersalTbl.clear().draw();
    edit_briefingRehersalTbl.clear().draw();
    multiSchedule.clear().draw();
    multiSchedule2.clear().draw();

}
function UpdatteActivity(button) {
    if ($('#edit-ModuleId').val() == '6') {
        getvltParticipantTable(3)

    } else {

        getvltParticipantTable(2);
        getBriefingRehersalTable(2);

    }

    var form = $(MEL.toSelector('updateActivity-form'));
    $.ajax({
        url: '/VolunteerManagement/UpdateActivity',
        type: 'POST',
        data: form.serializeArray(),
        success: function (info) {
            console.log(info)
            console.log(info.data.message)
            if (info.success) {//success
                dialog(info.data.message, "success");
                $("#UpdateActivityModal").modal("hide");
                $("#ViewEventModal").modal("hide");
                form.formReset();
                resetCreateActivityForm();
                RefreshCalendar()
            } else {
                dialog(info.data.message, "error");
            }
        }
    });
}
function CreateActivity(button) {
    button.setAttribute("disabled", "");
    var form = $(MEL.toSelector('createActivity-form'));
    var countDownDate = new Date("Jan 5, 2023 15:15:25").getTime();
    if (!form.valid()) {
        button.disabled = false;
        return;
    }
    getvltParticipantTable(1);
    getBriefingRehersalTable(1);
    $.ajax({
        url: '/VolunteerManagement/CreateActivity',
        type: 'POST',
        data: form.serializeArray(),
        success: function (info) {

            //console.log(info)
            //console.log(info.data.message)
            if (info.success) {//success
                dialog(info.data.message, "success");
                $("#AddActivityModal").modal("hide");
                resetCreateActivityForm();
                RefreshCalendar()
            } else {
                dialog(info.data.message, "error");
            }
            //var x = setInterval(function () {

            //    // Get today's date and time
            //    var now = new Date().getTime();
            //    //var count = data.addEditModule.endTime.format("MM/DD/YYYY hh:mm");
            //    //var count2 = count.getTime();
            //    // Find the distance between now and the count down date
            //    var distance = countDownDate - now;

            //    // Time calculations for days, hours, minutes and seconds
            //    var days = Math.floor(distance / (1000 * 60 * 60 * 24));
            //    var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            //    var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            //    var seconds = Math.floor((distance % (1000 * 60)) / 1000);

            //    // Output the result in an element with id="demo"
            //    document.getElementById("demo").innerHTML = days + "d " + hours + "h "
            //        + minutes + "m " + seconds + "s ";

            //    // If the count down is over, write some text 
            //    if (distance < 0) {
            //        clearInterval(x);
            //        document.getElementById("demo").innerHTML = "EXPIRED";
            //        /*                    tryAutoEmail()*/
            //    }
            //}, 1000);

        }
    });
    button.disabled = false;
}
function dialog(message, status) {
    let timerInterval
    Swal.fire({
        icon: status == 'success' ? 'success' : 'error',
        title: message,
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
    })
}



//dialog("halo","success")
//Swal.hideLoading()

function UpdateParticipantsTbl() {
    var Textvalues = $.map($("#ddlParticipants option:selected"), function (option) { return option.text; });
    //console.log(Textvalues)
    //console.log($("#ddlParticipants option:selected").text())
    var Values = $("#ddlParticipants").val();
    //console.log(Values)
    ptcpvlt.clear().draw();
    if (Values.length == 0) {
        ptcpvlt.clear();
    }
    $.each(Values, function (index, value) {
        var formData = ptcpvlt.rows().data();
        //console.log(formData)
        var lastRow = ptcpvlt.row(':last').data();
        var splitted = [];
        splitted = Textvalues[index].split("| ");
        //console.log(splitted)
        var validation = validate(value, formData);
        if (!validation) {
            return;
        }

        if (formData.length < 1) {
            if (lastRow == undefined) {

                ptcpvlt.row.add([(index + 1), splitted[0], splitted[1], splitted[2], splitted[3], "<a class='fas fa-trash' style='cursor:pointer;'></a>", value]).node();
                ptcpvlt.draw(false);
            } else {

                ptcpvlt.row.add([(lastRow[0] + 1), splitted[0], splitted[1], splitted[2], splitted[3], "<a class='fas fa-trash' style='cursor:pointer;'></a>", value]).node();
                ptcpvlt.draw(false);
            }
            ptcpvlt.draw(false);
        } else {
            if (validation) {
                if (lastRow == undefined) {

                    ptcpvlt.row.add([(index + 1), splitted[0], splitted[1], splitted[2], splitted[3], "<a class='fas fa-trash' style='cursor:pointer;'></a>", value]).node();
                    ptcpvlt.draw(false);
                } else {

                    ptcpvlt.row.add([(lastRow[0] + 1), splitted[0], splitted[1], splitted[2], splitted[3], "<a class='fas fa-trash' style='cursor:pointer;'></a>", value]).node();
                    ptcpvlt.draw(false);
                }

            }
        }

    });
    function validate(value) {
        var formData = ptcpvlt.rows().data();
        var boolean;
        boolean = true;
        if (formData.length > 0) {
            $.each(formData, function (index) {
                if (formData[index][4] == value) {
                    boolean = false;
                }


            });

        }
        return boolean;
    }
}
function UpdateEditParticipantsTbl() {

    var Textvalues = $.map($("#edit_ddlParticipants option:selected"), function (option) { return option.text; });
    console.log(Textvalues)
    console.log($("#ddlParticipants option:selected").text())
    var Values = $("#edit_ddlParticipants").val();
    //console.log(Values)
    if (Values.length == 0) {
        editptcpvltTbl.clear().draw();
    }
    $.each(Values, function (index, value) {
        var formData = editptcpvltTbl.rows().data();
        //console.log(formData)
        var lastRow = editptcpvltTbl.row(':last').data();
        var splitted = [];
        splitted = Textvalues[index].split("| ");
        //console.log(splitted)
        var validation = validate(value, formData);
        if (!validation) {
            return;
        }

        if (formData.length < 1) {
            if (lastRow == undefined) {

                editptcpvltTbl.row.add([(index + 1), splitted[0], splitted[1], splitted[2], splitted[3], "<a class='fas fa-trash' style='cursor:pointer;'></a>", value]).node();
                editptcpvltTbl.draw(false);
            } else {

                editptcpvltTbl.row.add([(lastRow[0] + 1), splitted[0], splitted[1], splitted[2], splitted[3], "<a class='fas fa-trash' style='cursor:pointer;'></a>", value]).node();
                editptcpvltTbl.draw(false);
            }
            editptcpvltTbl.draw(false);
        } else {
            if (validation) {
                if (lastRow == undefined) {

                    editptcpvltTbl.row.add([(index + 1), splitted[0], splitted[1], splitted[2], splitted[3], "<a class='fas fa-trash' style='cursor:pointer;'></a>", value]).node();
                    editptcpvltTbl.draw(false);
                } else {
                    ;
                    editptcpvltTbl.row.add([(lastRow[0] + 1), splitted[0], splitted[1], splitted[2], splitted[3], "<a class='fas fa-trash' style='cursor:pointer;'></a>", value]).node();
                    editptcpvltTbl.draw(false);
                }

            }
        }

    });
    function validate(value) {
        var formData = editptcpvltTbl.rows().data();
        var boolean;
        boolean = true;
        if (formData.length > 0) {
            $.each(formData, function (index) {
                if (formData[index][6] == value) {
                    boolean = false;
                }


            });

        }
        return boolean;
    }
}

function updateBrVltTbl() {
    var textValues = $.map($("#check-volunteer:checked"), function (checkbox) {
        return $(checkbox).data("text");
    });
    console.log(textValues)

    var Values = $.map($("#check-volunteer:checked"), function (checkbox) {
        return $(checkbox).data("id");
    });
    console.log(Values)
    if (Values.length == 0) {
        editbrptcpvltTbl.clear().draw();
        return;
    }

    let data = editbrptcpvltTbl.rows().data()
    for (var i = 0; i < data.length; i++) {
        let d = data[i]
        if (d[5] != null) {
            console.log(d[5])
            if (!Values.includes(d[5])) {
                editbrptcpvltTbl.row(i).remove().draw();
            }
        }
    }


    $.each(Values, function (index, value) {
        var formData = editbrptcpvltTbl.rows().data();
        var lastRow = editbrptcpvltTbl.row(':last').data();
        var splitted = textValues[index].split("|");

        if (!validate(value, formData)) {
            return;
        }

        var newRowIndex = formData.length < 1 && lastRow == undefined ?
            (index + 1) : (lastRow[0] + 1);
        editbrptcpvltTbl.row.add([newRowIndex, splitted[0], splitted[1], splitted[2], splitted[3], value]).node();
    });
    editbrptcpvltTbl.draw(false);

    function validate(value, formData) {
        for (var i = 0; i < formData.length; i++) {
            if (formData[i][5] == value) {
                return false;
            }
        }
        return true;
    }

    $("#createActivity-form").submit(function (event) {
        event.preventDefault(); // Prevent the default form submission
        // Make an AJAX request to submit the form data
        $.ajax({
            url: $(this).attr("action"),
            type: $(this).attr("method"),
            data: $(this).serialize(),
            success: function (data) {
                console.log(data);
                if (data.success == true) {
                    OnCreateActivitySuccess(data);
                }
                else {
                    OnFailure(data);
                }
            }
        });
    });
    function OnCreateActivitySuccess(results) {
        console.log(results.data.message)
        bootbox.alert({
            size: "small",
            title: "Message",
            message: results.data.message,
            callback: function () {

                $('#createActivity-form').modal('hide');

                window.location.reload();

            }
        });
    }
    function OnFailure(result) {
        console.log("failure");
        console.log(result);
    }
}

