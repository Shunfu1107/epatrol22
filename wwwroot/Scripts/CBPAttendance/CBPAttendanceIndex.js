
var recordLiveLocation;
var searchTime;
var currentCard = 1;
var izlink = '';
//var recordTime = [];
var recordLocation = [];
var preventdoubleclick = "";
var logInNum = 100;

window.onload = function () {

    document.getElementById("myApp").style = "opacity:1;transition:.5s;font-family: Arial, Helvetica, sans-serif;color:#333;overscroll-behavior: contain;";
    var d = new Date();
    var hours = d.getHours();
    var mins = d.getMinutes();
    var secs = (60 - d.getSeconds()) * 1000;
    if (hours < 10) {
        hours = '0' + hours;
    }
    if (mins < 10) {
        mins = '0' + mins;
    }
    document.getElementById("timeData").innerHTML = hours + '<a style="animation:ball 1s infinite;"> : </a>' + mins;
    document.getElementById("dateData").innerHTML = d.toDateString();
    setTimeout(function () {
        d = new Date();
        hours = d.getHours();
        mins = d.getMinutes();
        if (hours < 10) {
            hours = '0' + hours;
        }
        if (mins < 10) {
            mins = '0' + mins;
        }
        document.getElementById("timeData").innerHTML = hours + '<a style="animation:ball 1s infinite;"> : </a>' + mins;
        if (hours == '00' && mins == '00') {
            document.getElementById("dateData").innerHTML = d.toDateString();
        }
        setInterval(function () {
            d = new Date();
            hours = d.getHours();
            mins = d.getMinutes();
            if (hours < 10) {
                hours = '0' + hours;
            }
            if (mins < 10) {
                mins = '0' + mins;
            }
            document.getElementById("timeData").innerHTML = hours + '<a style="animation:ball 1s infinite;"> : </a>' + mins;
            if (hours == '00' && mins == '00') {
                document.getElementById("dateData").innerHTML = d.toDateString();
            }
        }, 60000);
    }, secs);
    currentCard = 1;

    
    

    //$("#ddlMood").chosen({
    //    placeholder_text_multiple: "Select Befriendee Moods",
    //    width:"75%"

    //});
    $("#ddlMood").select2({
        width: "75%",

        placeholder: "Select a mood",
    });
    $('#ddlMood').on('select2:opening select2:closing', function (event) {
        var $searchfield = $(this).parent().find('.select2-search__field');
        $searchfield.prop('disabled', true);
    });
    $("#ddlType").chosen({
        /*placeholder_text_multiple: "Select Visit Type",*/
        width: "75%",
        disable_search: true
    });

    $("#ddlAirCirculation").chosen({
        width: "75%",
        disable_search: true
    });

    $("#dllHouseBrightness").chosen({
        width: "75%",
        disable_search: true
    });

    $("#dllHouseCleanliness").chosen({
        width: "75%",
        disable_search: true
    });

    $('.ddlCBPUser').selectpicker();
    //$("#ddlSignCondition").chosen({
    //    placeholder_text_multiple: "Select Signs of Condition",
    //    width: "75%"
    //});

    $("#ddlSignCondition").select2({
        width: "75%",

        placeholder: "Select a sign",
    });

    $("#ddlSpecialRequest").chosen({
        /*placeholder_text_multiple: "Select Visit Type",*/
        width: "75%",
        disable_search: true
    });

    $('#ddlSignCondition').on('select2:opening select2:closing', function (event) {
        var $searchfield = $(this).parent().find('.select2-search__field');
        $searchfield.prop('disabled', true);
    });



};

//Initialize the ng-app declare in CBPAttendanceIndex
var app = angular.module('myApp', []);
app.controller('myCtrl', function ($scope, $timeout, $interval) {
    $scope.loading = $scope.logInAccount = false;
    $scope.logInNum = 0;
    $scope.uploadProgress = -1;
    $scope.account = $scope.password = $scope.searchText = '';
    $scope.m = screen.width >= screen.height ? false : true;
    $scope.cardReaderAccess = true;
    $scope.details = 0;
    $scope.AdHocdetails = 0;
    $scope.feedbackForm = 0;
    $scope.feedbackDisplay = "none";
    $scope.addkakiformDisplay = "none";
    $scope.editkakiformDisplay = "none";

    $scope.MainAppt = 1;
    $scope.CBPAdHocAppt = 1;
    $scope.SearchBar = false;
    $scope.CardClientSelection = 1;
    $scope.CardVolunteerSelection = 0;
    $scope.navMainAppt = 1;
    $scope.navAdHocAppt = 0;


    $scope.preLoad = function () {
        try {
            if (localStorage.length >= 1) {
                var n = localStorage.getItem("n");
                var p = localStorage.getItem("p");

                function logInAjax(i, ii) {

                    $.ajax({
                        url: 'CBPAttendance/LogIn',
                        data: {
                            Account: i,
                            Password: ii,
                            search: ""
                        },
                        type: "POST",
                        complete: function (res) {
                            console.log(res);
                            //responseText == return json result
                            //var refInterval = window.setTimeout('log()', 30000);
                            if (res.status == 200 && res.responseText != '') {
                                $scope.logInNum = 10;
                                /*console.log(res.responseJSON.Remarks);*/
                                $scope.CBPAppointmentList = res.responseJSON;
                                console.log(res.responseJSON);
                                $scope.CBPUserList = res.responseJSON.cbpUserList;
                                var userid = res.responseJSON.userid;
                                //console.log(userid);
                                $scope.ReloadCBPAdHocAppt(userid)
                                /*alert(res.responseJSON.UserName);*/
                                $timeout(function () {
                                    //console.log($scope.CBPAppointmentList);
                                }, 150);

                            }

                        }

                    });

                }
                function initClientDDL() {
                    try {
                        $.ajax({
                            url: 'CBPAttendance/GetCBPClient',
                            type: 'GET',
                            success: function (Json) {
                                $scope.CBPClientUserList = Json;
                            }
                        });
                    } catch (err) {
                        console.log(err);
                    }
                }
                function initVolunteerDDL() {
                    try {
                        $.ajax({
                            url: 'CBPAttendance/GetCBPVolunteers',
                            type: 'GET',
                            success: function (Json) {
                                $scope.CBPVolunteerUserList = Json;
                            }
                        });
                    } catch (err) {
                        console.log(err);
                    }
                }
                logInAjax(n, p);
                initClientDDL();
                initVolunteerDDL();
                //console.log("A")    
                //console.log(CBPAppointmentList['Userid'].UserName);
            }
        }
        catch (e) { }


    }

    $scope.windowResize = function () {
        $scope.m = screen.width >= screen.height ? false : true;
    };

    $scope.changeLogInType = function () {
        $scope.account = $scope.password = "";
        $scope.logInAccount = !$scope.logInAccount;
        if ($scope.logInAccount && $scope.cardReaderAccess) {
            try {
                stopReader();
            }
            catch (e) {
            }
        } else if (!$scope.logInAccount && $scope.cardReaderAccess) {
            currentCard = 1;

            try {
                startReader();
            }
            catch (e) {
            }
        }
    }

    $scope.logIn = function (i) {
        //Log in via account 
        if ($scope.logInAccount) {
            if ($scope.account != "" && $scope.password != "") {
                $scope.logInAjax($scope.account, $scope.password);
            } else {
                showToast('Please fill the account and password.', 3000, 'red');
            }
        } else { //login via card
            $scope.scanCard('logIn');
        }
    };

    $scope.logOut = function () {
        localStorage.clear();
        $scope.hideSetting();
        //if (recordLiveLocation != undefined) {
        //    showToast('Sorry you have trip still in progress.', 3000, 'red');
        //} else {
        if (!$scope.logInAccount && $scope.cardReaderAccess) {
            currentCard = 1;
            try {
                startReader();
            } catch (e) {
            }

        }
        izlink = $scope.account = $scope.password = $scope.searchText = '';
        //$scope.tripList = {};
        $scope.logInNum = 0;
        //$scope.stopRecordLiveLocation();
        showToast('Logout Successful', 3000, 'skyblue');
        //}
    }

    $scope.scanCard = function (i) {
        $scope.scanCardOpacity = 0;
        $scope.scanCardShow = true;
        $timeout(function () {
            $scope.scanCardOpacity = 1;
        }, 1);
        if ($scope.cardReaderAccess) {
            try {
                startReader();
            } catch (e) {

            }
            if (i == 'logIn') {
                currentCard = 1;
            } else {
                currentCard = 2;
            }
        } else {
            $timeout(function () {
                izlink = prompt("Please enter your EZLink Card");
                if (izlink != null) {
                    $scope.scanCardShow = false;
                    $scope.scanCardOpacity = 0;
                    if (i == 'logIn') {
                        $scope.logInAjax(null, izlink);
                    } else {
                        $scope.updateData[i] = izlink;
                    }
                } else {
                    $scope.hideScanCard();
                }
            }, 500);
        }
    }

    $scope.showAppointmentDetails = function (userid, username, cbpSchedulingId, Id) {
        $scope.detailsOpacity = 0;
        $scope.FeedbackOpacity = 0;
        $scope.AddKakiFormOpacity = 0;
        $scope.details = 1;
        $scope.loading = true;
        $("#AdHocId").val("");
        $.ajax({
            url: 'CBPAttendance/CBPAppointmentDetails',
            data: {
                UserId: userid, //UserID example 331 
                UserName: username,
                CBPSchedulingId: cbpSchedulingId,
                CBPAppointmentsId: Id

            },
            type: "POST",
            complete: function (res) {
                console.log(res)
                /*console.log(res.responseJSON)*/
                $timeout(function () {
                    $scope.loading = false;
                    if (res.status == 200 && typeof res.responseJSON !== 'undefined') {
                        $scope.CBPAppointmentDetails = res.responseJSON;
                        $scope.detailsOpacity = 1;
                        //$scope.resetfeedback(); //remove reset feedback function to avoid remove
                    }
                    else {
                        showToast('Please check your connection and try again later.', 3000, 'red');
                        $scope.details = 0;
                    }
                }, 150);

            }
        });
    }

    /*For outside button to get kakilist*/
    //$scope.getKakiList = setInterval(function (userid, username, cbpSchedulingId, Id) {

    //    $.ajax({
    //        url: 'CBPAttendance/CBPAppointmentDetails',
    //        data: {
    //            UserId: userid, //UserID example 331 
    //            UserName: username,
    //            CBPSchedulingId: cbpSchedulingId,
    //            CBPAppointmentsId: Id

    //        },
    //        type: "POST",
    //        complete: function (res) {
    //            /*console.log(res)*/
    //            //console.log(res.responseJSON)
    //            $timeout(function () {
    //                $scope.loading = false;
    //                $scope.CBPAppointmentDetails = res.responseJSON;

    //            }, 150);

    //        }
    //    })


    //}, 2000);

    $scope.hideDetails = function () {
        $scope.detailsOpacity = 0;
        $timeout(function () {
            $scope.details = 0;
        }, 500);

    }
    $scope.hideFeedback = function () {
        /*alert();*/
        $scope.FeedbackOpacity = 0;
        $timeout(function () {
            $scope.feedbackDisplay = "none";
        }, 500);

    }


    $scope.showFeedback = function () {
        //hideDetails();
        //$scope.feedbackForm = 1;
        $scope.FeedbackOpacity = 1;
        $scope.feedbackDisplay = "flex";
        $scope.feedbackDisplay2 = "initial";
    }

    $scope.empty = function () {
        /*alert("的");*/
        event.stopPropagation();
    }

    $scope.hideKakiForm = function () {
        /*alert();*/
        $scope.AddKakiFormOpacity = 0;
        $timeout(function () {
            $scope.addkakiformDisplay = "none";
        }, 500);

    }
    $scope.showAddkakiForm = function () {
        //hideDetails();
        //$scope.feedbackForm = 1;
        $scope.AddKakiFormOpacity = 1;
        $scope.addkakiformDisplay = "flex";
        $scope.addkakiformDisplay2 = "initial";
        $("#DDLVolunteer")[0].selectedIndex = 0;
    }

    $scope.hideEditKakiForm = function () {
        /*alert();*/
        $scope.editKakiFormOpacity = 0;
        $timeout(function () {
            $scope.editkakiformDisplay = "none";
        }, 500);

    }
    $scope.showEditkakiForm = function (k1, k2, k3, k4, k5) {
        $scope.editKakiFormOpacity = 1;
        $scope.editkakiformDisplay = "flex";
        $scope.editkakiformDisplay2 = "initial";
        //console.log(k1, k2, k3, k4, k5);
        var kaki1 = document.getElementById('DDLVolunteer_Kaki1');
        kaki1.selectedIndex = 0;
        if (k1 != "-") {

            getSelectedOption(kaki1, k1);
        }
        var kaki2 = document.getElementById('DDLVolunteer_Kaki2');
        kaki2.selectedIndex = 0;
        if (k2 != "-") {

            getSelectedOption(kaki2, k2);
        }
        var kaki3 = document.getElementById('DDLVolunteer_Kaki3');
        kaki3.selectedIndex = 0;
        if (k3 != "-") {

            getSelectedOption(kaki3, k3);
        }
        var kaki4 = document.getElementById('DDLVolunteer_Kaki4');
        kaki4.selectedIndex = 0;
        if (k4 != "-") {

            getSelectedOption(kaki4, k4);
        }
        var kaki5 = document.getElementById('DDLVolunteer_Kaki5');
        kaki5.selectedIndex = 0;
        if (k5 != "-") {

            getSelectedOption(kaki5, k5);
        }
    }

    $scope.setting = function () {
        $scope.settingOpacity = 0;
        $scope.settingShow = true;
        $timeout(function () {
            $scope.settingOpacity = 1;
        }, 1);
        var a = $scope.MainAppt
        var b = $scope.CBPAdHocAppt;
    }


    $scope.hideSetting = function () {
        $scope.settingOpacity = 0;
        $timeout(function () {
            $scope.settingShow = false;
        }, 500);
    }

    //Trigger when search
    $scope.search = function () {
        try {
            $timeout.cancel(searchTime);
            searchTime = $timeout(function () {
                if (localStorage.length >= 1) {
                    var n = localStorage.getItem("n");
                    var p = localStorage.getItem("p");
                    if (n != null) {
                        $scope.filterByClientName(n, p);
                    } else {
                        $scope.filterByClientName(null, p);
                    }

                }
                else {
                    if ($scope.logInAccount) {

                        $scope.filterByClientName($scope.account, $scope.password);

                    } else {
                        $scope.logInAjax(null, izlink);
                    }
                }
            }, 500);
        } catch (e) {
            alert(e);
        }
    }

    $scope.filterByClientName = function (i, ii) {
        //$scope.loading = true;
        $.ajax({
            url: 'CBPAttendance/LogIn',
            data: {
                Account: i,
                Password: ii,
                search: $scope.searchText
            },
            type: "POST",
            complete: function (res) {
                //console.log($scope.CBPAppointmentList)
                $scope.CBPAppointmentList = res.responseJSON;
                console.log(res.responseJSON);
                //console.log($scope.CBPAppointmentList)
                $timeout(function () {

                    $scope.logInNum = 10;
                }, 50);
            }
        });

    }

    $("textarea").keypress(function () {
        total_words = this.value.split(/[\s\.\?\,]+/).length;
        //console.log(total_words);
        if (total_words >= 5000) {
            $('textarea').keydown(function (e) {
                e.preventDefault();
                return false;
            });
        }
        //$("#txtAreaWords").html(5 - total_words);

    });

    $scope.logInAjax = function (i, ii) {
        $scope.loading = true;
        $.ajax({
            url: 'CBPAttendance/LogIn',
            data: {
                Account: i,
                Password: ii,
                search: $scope.searchText
            },
            type: "POST",
            complete: function (res) {
                //responseText == return json result
                //var refInterval = window.setTimeout('log()', 30000);
                if (res.status == 200 && res.responseText != '') {
                    /*console.log(res.responseJSON.Remarks);*/
                    $scope.CBPAppointmentList = res.responseJSON;
                    console.log(res.responseJSON);
                    $scope.CBPUserList = res.responseJSON.CBPUserList;
                    //console.log(res);
                    if (i != null) {
                        window.localStorage.setItem("n", i);
                    }
                    window.localStorage.setItem("p", ii);
                    /*alert(res.responseJSON.UserName);*/
                    $timeout(function () {
                        showToast('Login Successful', 3000, 'skyblue');
                        $scope.logInNum = 10;
                    }, 150);
                    $scope.loading = false;
                }
                else if (res.status == 200 && res.responseText == '') {
                    if (!$scope.logInAccount && $scope.cardReaderAccess) {
                        currentCard = 1;
                        try {
                            startReader();
                        } catch (e) {
                        }
                    }
                    $timeout(function () {
                        showToast('Account Not Found', 3000, 'red');
                        $scope.loading = false;
                    }, 150);

                }
                else {
                    if (!$scope.logInAccount && $scope.cardReaderAccess) {
                        currentCard = 1;
                        try {
                            startReader();
                        } catch (e) {
                        }
                    }
                    $timeout(function () {
                        showToast('Please check your connection and try again later.', 3000, 'red');
                        $scope.loading = false;
                    }, 150);
                }
            }

        });
    }

    //$scope.reloadCBPAppointment = setInterval(function () {
    //    if ($scope.account != "" && $scope.password != "") {
    //        var account = $scope.account;
    //        var password = $scope.password;
    //    }
    //    else {

    //    }
    //}, 3000)

    $scope.checkIn = function (selectedKaki, cbpAppointmentsId, cbpSchedulingId) {
        //selectedKaki, cbpAppointmentsId, cbpSchedulingId
        //process taking value from dropdownlist

        //alert(selectedKaki + "," + cbpAppointmentsId + "," + cbpSchedulingId);

        $.ajax({
            url: 'CBPAttendance/SubmitCBPAttendance',
            data: {
                UserId: selectedKaki,
                CBPAppointmentsId: cbpAppointmentsId,
                CBPSchedulingId: cbpSchedulingId

            },
            type: "POST",
            complete: function (res) {
                console.log(res);
                var status = res.responseJSON.status;
                var statusMessage = res.responseJSON.statusMessage;
                if (status == "100" && statusMessage == "Success Check In") {
                    $timeout(function () {
                        //refill again the data for cbpappointmentdetails
                        reloadCBPAppointmentDetails();
                        //var a = res.responseJSON.StatusMessage;
                        //alert(a);
                        showToast('Check In Successfully', 3000, 'skyblue');
                    }, 50);
                }
                else if (status == "100" && statusMessage == "Success Check Out") {
                    $timeout(function () {
                        //refill again the data for cbpappointmentdetails
                        reloadCBPAppointmentDetails();
                        //var a = res.responseJSON.StatusMessage;
                        //alert(a);
                        showToast('Check Out Successfully', 3000, 'skyblue');
                    }, 50);
                } else if (status == "100" && statusMessage == "Finish attendance") {
                    $timeout(function () {
                        //refill again the data for cbpappointmentdetails
                        reloadCBPAppointmentDetails();
                        //var a = res.responseJSON.StatusMessage;
                        //alert(a);
                        showToast('You have done your attendance', 3000, 'red');
                    }, 50);
                } else if (status == "100" && statusMessage == "No kaki check in") {
                    $timeout(function () {
                        //refill again the data for cbpappointmentdetails
                        reloadCBPAppointmentDetails();
                        //var a = res.responseJSON.StatusMessage;
                        //alert(a);
                        showToast('At least one kaki must check in', 3000, 'red');
                    }, 50);
                }
                //console.log(res.responseJSON)
                //$scope.CBPAppointmentList = res.responseJSON;

            }
        });
    }

    function reloadCBPAppointmentDetails() {

        var userid = document.getElementById("userid").value;
        var username = document.getElementById("username").value;
        var cbpSchedulingId = document.getElementById("cbpSchedulingId").value;
        var CBPAId = document.getElementById("CBPAId").value;
        $.ajax({
            url: 'CBPAttendance/CBPAppointmentDetails',
            data: {
                UserId: userid, //UserID example 331 
                UserName: username,
                CBPSchedulingId: cbpSchedulingId,
                CBPAppointmentsId: CBPAId
            },
            type: "POST",
            complete: function (res) {

                $timeout(function () {
                    $scope.CBPAppointmentDetails = res.responseJSON;

                }, 150);

            }
        });

    }


    //$('#btnclosetap').on("click", function () {
    //    $scope.detailsOpacity = 0;
    //    $timeout(function () {
    //        $scope.details = 0;
    //    }, 500);
    //    var CBPAId = $scope._CBPAId;
    //    var cbpSchedulingId = $scope._cbpSchedulingId;
    //    alert(CBPAId);
    //    alert(cbpSchedulingId);
    //});

    $('#closeTapCard').on("click", function () {

        try {
            stopReader();
        }
        catch (e) {
        }
        $('#tapCardModal').modal('hide');
    });

    $scope.ScanCardForCheckInOut = function (card_num) {
        /*alert()*/
        var userid = card_num;
        if (testingForm !== null) {
            testingForm.push({ name: 'UserId', value: userid })
        }

        //alert("usercardno: " + userid + ",CBPAId:" + CBPAId + ",cbpSchedulingId: " + cbpSchedulingId);
        //alert(JSON.stringify(testingForm));
        $.ajax({
            url: 'CBPAttendance/ScanCardCheckInOut',
            data: testingForm,
            type: "POST",
            complete: function (res) {

                var status = res.responseJSON.Status;
                var statusMessage = res.responseJSON.StatusMessage;

                if (status == "100" && statusMessage == "Success Check In") {
                    try {
                        stopReader();
                    }
                    catch (e) {
                    }
                    $('#tapCardModal').modal('hide');
                    $timeout(function () {
                        showToast('Check In Successfully', 3000, 'skyblue');
                    }, 50);
                } else if (status == "100" && statusMessage == "Success Check Out") {
                    try {
                        stopReader();
                    }
                    catch (e) {
                    }
                    $('#tapCardModal').modal('hide');
                    $timeout(function () {
                        showToast('Check Out Successfully.', 3000, 'skyblue');
                    }, 50);
                }
                else if (status == "100" && statusMessage == "Finish attendance") {
                    try {
                        stopReader();
                    }
                    catch (e) {
                    }
                    $('#tapCardModal').modal('hide');
                    $timeout(function () {
                        showToast('You have done your attendance.', 3000, 'red');
                    }, 50);
                }
                else if (status == "100" && statusMessage == "No kaki check in") {
                    try {
                        stopReader();
                    }
                    catch (e) {
                    }
                    $('#tapCardModal').modal('hide');
                    $timeout(function () {
                        showToast('At least one kaki must check in.', 3000, 'red');
                    }, 50);
                } else if (status == "100" && statusMessage == "This card is not Register") {
                    try {
                        stopReader();
                    }
                    catch (e) {
                    }
                    $('#tapCardModal').modal('hide');
                    $timeout(function () {
                        showToast('Invalid card.', 3000, 'red');
                    }, 50);
                }
                else if (status == "100" && statusMessage == "Invalid Scan") {
                    try {
                        stopReader();
                    }
                    catch (e) {
                    }
                    $('#tapCardModal').modal('hide');
                    $timeout(function () {
                        showToast('Card User is not related to this appointment.', 3000, 'red');
                    }, 50);
                }

            }
        });
    }

    var testingForm = null;

    $scope.tapCardforCheckinOut = function (idx) {

        testingForm = $('.btnCheckInOut').closest('#form-' + idx).serializeArray();
        //this.ScanCardForCheckInOut('123');

        $scope.detailsOpacity = 0;
        $timeout(function () {
            $scope.details = 0;
        }, 500);
        currentCard = 3;
        try {
            startReader();
        } catch (e) {
        }
    }
    $scope.resetfeedback = function () {

        document.getElementById("feedbackForm").reset();

        $('#ddlMood').val([]).trigger("change");
        $('#ddlType').val('').trigger("chosen:updated");
        $('#ddlAirCirculation').val('').trigger("chosen:updated");
        $('#dllHouseBrightness').val('').trigger("chosen:updated");
        $('#dllHouseCleanliness').val('').trigger("chosen:updated");
        $('#ddlSignCondition').val([]).trigger("change");
        $('#ddlSpecialRequest').val('').trigger("chosen:updated");

        $("#ddlType").val('').trigger("chosen:updated");

    }

    $scope.checkFeedbackParas = function () {
        //console.log($("#ddlMood").chosen().val());
        //console.log($("#ddlType").chosen().val());
        //console.log($("#ddlAirCirculation").chosen().val());
        //console.log($("#dllHouseBrightness").chosen().val());
        //console.log($("#dllHouseCleanliness").chosen().val());
        //console.log($("#ddlSignCondition").chosen().val());
        //console.log($('#txtFeedbackRemark').val());
        console.log($("#ddlSpecialRequest").chosen().val());
        /*$scope.hideFeedback();*/

    }

    $scope.submitFeedback = function (userId, cbpId) {
        var AdHocId = $("#AdHocId").val();
        console.log(userId, "and ", cbpId)

        $('#ddlMood').trigger("chosen:updated");
        $('#ddlSignCondition').trigger("chosen:updated");


        var SelectedMood = $("#ddlMood").chosen().val();
        var SelectedVisitType = $("#ddlType").chosen().val();
        var SelectedAirCirculation = $("#ddlAirCirculation").chosen().val();
        var SelectedBrightness = $("#dllHouseBrightness").chosen().val();
        var SelectedCleanliness = $("#dllHouseCleanliness").chosen().val();
        var SelectedSignofWorsening = $("#ddlSignCondition").chosen().val();
        var SelectedSpecialRequester = $("#ddlSpecialRequest").chosen().val();
        var txtSpecialRequestRemark = $('#txtSpecialRequestRemark').val();
        var txtFeedbackRemark = $('#txtFeedbackRemark').val();
        if ((SelectedMood.length == 0) || (SelectedVisitType == null) || (SelectedBrightness == null) || (SelectedCleanliness == null || SelectedSpecialRequester == null)) {
            showToast('Please fill in the required field.', 3000, 'red');
            return;
        }
        $scope.loading = true;
        if (AdHocId == "") {//means this is feedback form in cbp appointment
            $.ajax({
                url: 'CBPAttendance/SubmitFeedback',
                data: {

                    CBPAppointmentsId: cbpId,
                    Mood: SelectedMood,
                    VisitType: SelectedVisitType,
                    AirCirculation: SelectedAirCirculation,
                    Brightness: SelectedBrightness,
                    Cleanliness: SelectedCleanliness,
                    SignofWorsening: SelectedSignofWorsening,
                    Remark: txtFeedbackRemark,
                    CreatedBy: userId,
                    AdHocId: 0
                },
                type: "POST",
                complete: function (res) {
                    var status = res.responseJSON.status;
                    var statusMessage = res.responseJSON.statusMessage;
                    if (status == "100" && statusMessage == "Successfully submitted feedback form") {
                        $scope.loading = false;
                        $timeout(function () {
                            showToast('Thank you, your feedback was submitted.', 3000, 'skyblue');
                        }, 50);
                        $scope.resetfeedback();
                        $scope.hideFeedback();
                        reloadCBPAppointmentDetails();
                    }
                }
            });
        } else {//submit in adhoc cbp appointment only
            $.ajax({
                url: 'CBPAttendance/SubmitFeedback',
                data: {

                    CBPAppointmentsId: 0,
                    Mood: SelectedMood,
                    VisitType: SelectedVisitType,
                    AirCirculation: SelectedAirCirculation,
                    Brightness: SelectedBrightness,
                    Cleanliness: SelectedCleanliness,
                    SignofWorsening: SelectedSignofWorsening,
                    Remark: txtFeedbackRemark,
                    CreatedBy: userId,
                    AdHocId: AdHocId,
                    SpecialRequest: SelectedSpecialRequester,
                    SpecialRequest_Remark: txtSpecialRequestRemark

                },
                type: "POST",
                complete: function (res) {
                    var status = res.responseJSON.status;
                    var statusMessage = res.responseJSON.statusMessage;
                    if (status == "100" && statusMessage == "Successfully submitted feedback form") {
                        $scope.loading = false;
                        $timeout(function () {
                            showToast('Thank you, your feedback was submitted.', 3000, 'skyblue');
                        }, 50);
                        $scope.resetfeedback();
                        $scope.hideFeedback();
                        //reloadCBPAppointmentDetails();
                        //reload appt
                        $scope.hideAdHocDetails(userId);
                        $scope.ReloadAdHocApptDetails(AdHocId);
                    }
                }
            });
        }

    }

    $scope.UpdateCard = function () {
        $scope.details2Opacity = 1;
        $scope.details2 = 3;
        if ($scope.cardReaderAccess) {
            currentCard = 4;

            try {
                startReader();
            } catch (e) {

            }
        }
        $scope.hideSetting();
        $scope.updateData = {
            'Card': null
        };
    }
    $scope.registerCard = function () {
        //console.log("Client" + $scope.CardClientSelection);
        //console.log("Volunteer" + $scope.CardVolunteerSelection);
        //console.log($scope.updateData);
        if ($scope.CardClientSelection == 1) {
            var e = document.getElementById("ddlCBPClientUser");
            var selectedCardUservalue = e.options[e.selectedIndex].value;
        }
        if ($scope.CardVolunteerSelection == 1) {
            var e = document.getElementById("ddlCBPVolunteerUser");
            var selectedCardUservalue = e.options[e.selectedIndex].value;
        }

        //console.log(selectedCardUservalue);
        if ($scope.updateData.Card == null || selectedCardUservalue == "") {
            showToast('Please fill in the required information.', 3000, 'red');
            return;
        }
        $scope.loading = true;

        $.ajax({
            url: '/MET/CardRegistration',
            data: {
                UserId: selectedCardUservalue,
                EzlinkCardNo: $scope.updateData['Card']
            },
            type: "POST",
            complete: function (res) {
                $timeout(function () {
                    $scope.hideDetails2();
                    showToast(res.responseJSON['Message'], 3000, res.responseJSON['Color']);
                    $scope.loading = false;
                }, 150);
            }
        });
    }

    $scope.hideDetails2 = function () {
        $scope.allUserList = false;
        $scope.updateDataPosition = null;
        if ($scope.cardReaderAccess) {
            currentCard = 0;

            try {
                stopReader();
            } catch (e) {
            }
        }
        $scope.details2Opacity = 0;
        if ($scope.details21 == 0) {
            $scope.details21 = 1;
        }
        $timeout(function () {
            $scope.details2 = 0;
        }, 500);
    }
    $scope.AddKaki = function (CBPId) {
        var newKaki = $('#DDLVolunteer option:selected').val();

        //console.log(CBPId);
        try {
            $.ajax({
                url: 'CBPAttendance/AddKakiFromApp',
                type: 'POST',
                data: {
                    CBPAppointmentsId: CBPId,
                    newAddedKaki: newKaki
                },
                success: function (result) {
                    //console.log(result);
                    if (result.success) {
                        $timeout(function () {
                            showToast('New kaki Added.', 3000, 'skyblue');
                        }, 50);
                        $scope.hideKakiForm();
                        reloadCBPAppointmentDetails()
                    } else {
                        $timeout(function () {
                            showToast(result.data, 3000, 'skyblue');
                        }, 50);
                    }
                }
            });
        } catch (err) {
            console.log(err);
        }
    }

    $scope.EditKaki = function (CBPId) {
        var newKaki1 = $('#DDLVolunteer_Kaki1 option:selected').val();
        var newKaki2 = $('#DDLVolunteer_Kaki2 option:selected').val();
        var newKaki3 = $('#DDLVolunteer_Kaki3 option:selected').val();
        var newKaki4 = $('#DDLVolunteer_Kaki4 option:selected').val();
        var newKaki5 = $('#DDLVolunteer_Kaki5 option:selected').val();

        try {
            $.ajax({
                url: 'CBPAttendance/EditKakiFromApp',
                type: 'POST',
                data: {
                    CBPAppointmentsId: CBPId,
                    newAddedKaki1: newKaki1,
                    newAddedKaki2: newKaki2,
                    newAddedKaki3: newKaki3,
                    newAddedKaki4: newKaki4,
                    newAddedKaki5: newKaki5
                },
                success: function (result) {
                    //console.log(result);
                    if (result.success) {
                        $timeout(function () {
                            showToast('Kakis Edited.', 3000, 'skyblue');
                        }, 50);
                        $scope.hideEditKakiForm();
                        reloadCBPAppointmentDetails()
                    } else {
                        $timeout(function () {
                            showToast(result.data, 3000, 'skyblue');
                        }, 50);
                    }
                }
            });
        } catch (err) {
            console.log(err);
        }

    }

    $scope.RedirectAdHocAppt = function (userid) {
        $scope.SearchBar = false;
        $scope.MainAppt = 1;
        $scope.CBPAdHocAppt = 1;
        $scope.navAdHocAppt = 0;
        $scope.navMainAppt = 1;

        //document.getElementById("searchbar").style.visibility = "hidden";
        //document.getElementById("CBPApptMenu").style.background = "Transparent";
        //document.getElementById("CBPApptMenu").style.color = "#888";

        //document.getElementById("AdHocApptMenu").style.background = "rgba(73, 25, 158,.05)";
        //document.getElementById("AdHocApptMenu").style.color = "#3296fa";

        //$('.mainAppt').addClass("removeWhiteBox");
        //$('.CBPAdhocAppt').removeClass("removeWhiteBox");
        //get adhoc appt
        try {
            $.ajax({
                url: 'CBP_AdHocScheduling/GetAppointmentsforCBPApp?userid=' + userid,
                type: 'GET',
                success: function (Json) {
                    console.log("geeeting adhoc")
                    $scope.CBPAdHocAppointment = Json;
                    console.log($scope.CBPAdHocAppointment);

                }
            });
        } catch (err) {
            console.log(err);
        }
        $scope.hideSetting();
    }

    $scope.ReloadCBPAdHocAppt = function (UserId) {

        try {
            $.ajax({
                url: 'CBPAttendance/GetAppointmentsforCBPApp?userid=' + UserId,
                type: 'GET',
                success: function (Json) {
                    console.log("geeeting adhoc")
                    console.log(Json);
                    $scope.CBPAdHocAppointment = Json;
                    //var start = $scope.CBPAdHocAppointment.strStartTime
                    //start.slice(10,-11)
                    var results = $scope.CBPAdHocAppointment
                    //var array = document.querySelectorAll('.start-time')
                    console.log(results);
                    results.forEach(result => {
                        console.log(result)
                        result.strStartTime.slice(10,-11)
                        //console.log(result.strendtime.slice(10,-11))
                    })
                   
                    $scope.$apply();
                    
                }
            });
        } catch (err) {
            console.log(err);
        }
       
    }

    $scope.OpenAddApptModal = function (UserId) {
        $('#AddAdHocApptModal').modal("show");

    }

    $scope.SubmitCBPAdHocAppointment = function (UserID) {

        var validation = AdHocApptFormValidation()
        if (validation == "false") {
            return false
        }

        var AdHocAppt_Date = $('#AdHocAppt_BookingDate').val();
        var AdHocAppt_StartTime = $('#AdHocAppt_StartTime').val();
        var AdHocAppt_EndTime = $('#AdHocAppt_EndTime').val();
        var AdHocAppt_Activity = $('#DDLActivity option:selected').val();
        var AdHocAppt_Client = $('#DDLClient option:selected').val();
        var AdHocAppt_Kaki1 = $('#AdHocKaki1 option:selected').val();
        var AdHocAppt_Kaki2 = $('#AdHocKaki2 option:selected').val();
        var AdHocAppt_Kaki3 = $('#AdHocKaki3 option:selected').val();
        var AdHocAppt_Kaki4 = $('#AdHocKaki4 option:selected').val();
        var AdHocAppt_Kaki5 = $('#AdHocKaki5 option:selected').val();
        var AdHocAppt_Remarks = $('#AdHocRemark').val();
        //$scope.loading = true;
        try {
            $.ajax({
                url: 'CBP_AdHocScheduling/Add_AdHocAppt_viaCBPApp',

                data: {
                    BookingDate: AdHocAppt_Date,
                    StartTime: AdHocAppt_StartTime,
                    EndTime: AdHocAppt_EndTime,
                    Activity: AdHocAppt_Activity,
                    ClientId: AdHocAppt_Client,
                    Kaki1: AdHocAppt_Kaki1,
                    Kaki2: AdHocAppt_Kaki2,
                    Kaki3: AdHocAppt_Kaki3,
                    Kaki4: AdHocAppt_Kaki4,
                    Kaki5: AdHocAppt_Kaki5,
                    Remarks: AdHocAppt_Remarks
                },
                type: 'POST',
                success: function (result) {
                    //console.log(result);
                    //$scope.loading = false;
                    if (result.Success) {
                        $('#AddAdHocApptModal').modal("hide");
                        showToast(result.data.message, 3000, 'skyblue');
                        //console.log(UserID + 'dddd');
                        $scope.ReloadCBPAdHocAppt(UserID);

                    } else {

                        $('#AddAdHocApptModal').modal("hide");
                        showToast(result.data.message, 3000, 'red');
                    }
                }
            });
        } catch (err) {
            console.log(err);
        }

    }

    $scope.ShowAdHocApptDetails = function (Id) {
        
        $.ajax({
            url: 'CBPAttendance/GetDetail?Id=' + Id,
            type: 'GET',
            success: function (Json) {
                console.log(Json);
                if (Json.success) {

                    var data = Json.data;
                    //console.log(data);
                    $scope.AdHocAppointmentDetail = Json.data;
                    $('.AdHocAppt-details').each(function () {
                        $(this).empty();
                    });

                    $("CBPAdHocApptId").val(data.id)
                    //var start = new Date(parseInt((data.StartTime).match(/\d+/)[0]))
                    //var end = new Date(parseInt((data.EndTime).match(/\d+/)[0]))
                    $("#CBPAdHocAppt_Activity").append(data.activityName);
                    $("#CBPAdHocAppt_Time").append(moment(data.startTime).format(" hh:mm ") + " - " + moment(data.endTime).format(" hh:mm "));


                    $("#CBPAdHocAppt_Client").append(data.clientName);
                    $("#CBPAdHocAppt_Mobile").append(data.clientPhone);
                    $("#CBPAdHocAppt_Kaki1").append(data.kaki1);
                    $("#CBPAdHocAppt_Kaki2").append(data.kaki2);
                    $("#CBPAdHocAppt_Kaki3").append(data.kaki3);
                    $("#CBPAdHocAppt_Kaki4").append(data.kaki4);
                    $("#CBPAdHocAppt_Kaki5").append(data.kaki5);

                    $("#CBPAdHocAppt_Remarks").append(data.remarks);
                    $("#CBPAdHocAppt_Status").append(data.status);
                    if (data.status == "Pending") {
                        $("#CBPAdHocAppt_Status").css('color', 'green');
                        //document.getElementById("CancelAdHocAppt").style.visibility = "visible";
                        //$("#CBPAdHocAppt_Status").css("color", "green");
                        //if (data.FeedbackStatus == "Incomplete") {
                        //    document.getElementById("FeedbackOpenbtn").style.visibility = "visible";
                        //    document.getElementById("FeedbackSubmittedText").style.visibility = "hidden";
                        //} else {
                        //    document.getElementById("FeedbackOpenbtn").style.visibility = "hidden";
                        //    document.getElementById("FeedbackSubmittedText").style.visibility = "visible";
                        //}
                        $("#CancelAdHocAppt").show();
                        $("#FeedbackOpenbtn").show();
                        $("#FeedbackSubmittedText").hide();
                    }
                    if (data.status == "Completed") {
                        $("#CBPAdHocAppt_Status").css('color', 'red');
                        //document.getElementById("CancelAdHocAppt").style.visibility = "hidden";
                        //$("#CBPAdHocAppt_Status").css("color", "red");
                        //if (data.FeedbackStatus == "Incomplete") {
                        //    document.getElementById("FeedbackOpenbtn").style.visibility = "visible";
                        //    document.getElementById("FeedbackSubmittedText").style.visibility = "hidden";
                        //} else {
                        //    document.getElementById("FeedbackOpenbtn").style.visibility = "hidden";
                        //    document.getElementById("FeedbackSubmittedText").style.visibility = "visible";
                        //}
                        $("#CancelAdHocAppt").hide();
                        $("#FeedbackOpenbtn").hide();
                        $("#FeedbackSubmittedText").show();
                    }
                    if (data.status == "Cancelled") {
                        $("#CBPAdHocAppt_Status").css('color', 'grey');
                        //document.getElementById("CancelAdHocAppt").style.visibility = "hidden";
                        //$("#CBPAdHocAppt_Status").css("color", "grey");
                        //document.getElementById("FeedbackOpenbtn").style.visibility = "hidden";
                        //document.getElementById("FeedbackSubmittedText").style.visibility = "hidden";
                        $("#CancelAdHocAppt").hide();
                        $("#feedbackArea").hide();
                        
                    }

                    $scope.AdHocFeedbackAvailable = 0;
                    //assign adhoc id in feedback form
                    $("#AdHocId").val(data.id);

                    $scope.AdHocdetails = 1;
        $scope.AdHocdetailsOpacity = 1;


                } else {
                    showToast(Json.data.message, 3000, 'red');
                }

            }
        });
    }

    $scope.ReloadAdHocApptDetails = function (Id) {
        $.ajax({
            url: 'CBP_AdHocScheduling/GetDetail?Id=' + Id,
            type: 'GET',
            success: function (Json) {
                if (Json.success) {
                    var data = Json.data;
                    //console.log(data);
                    $scope.AdHocAppointmentDetail = Json.data;
                    $('.AdHocAppt-details').each(function () {
                        $(this).empty();
                    });

                    $("CBPAdHocApptId").val(data.id)
                    var start = new Date(parseInt((data.startTime).match(/\d+/)[0]))
                    var end = new Date(parseInt((data.endTime).match(/\d+/)[0]))
                    $("#CBPAdHocAppt_Activity").append(data.activityName);
                    $("#CBPAdHocAppt_Time").append(formatDataDate(start) + " - " + formatDataDate(end));


                    $("#CBPAdHocAppt_Client").append(data.clientName);
                    $("#CBPAdHocAppt_Mobile").append(data.clientPhone);
                    $("#CBPAdHocAppt_Kaki1").append(data.kaki1);
                    $("#CBPAdHocAppt_Kaki2").append(data.kaki2);
                    $("#CBPAdHocAppt_Kaki3").append(data.kaki3);
                    $("#CBPAdHocAppt_Kaki4").append(data.kaki4);
                    $("#CBPAdHocAppt_Kaki5").append(data.kaki5);

                    $("#CBPAdHocAppt_Remarks").append(data.remarks);
                    $("#CBPAdHocAppt_Status").append(data.status);
                    if (data.status == "Pending") {
                        $("#CBPAdHocAppt_Status").css("color", "green");
                    }

                    $scope.AdHocFeedbackAvailable = 0;
                    //assign adhoc id in feedback form
                    $("#AdHocId").val(data.id);
                    if (data.feedbackStatus == "Incomplete") {
                        $("#FeedbackOpenbtn").show();
                        $("#FeedbackSubmittedText").hide();
                        //document.getElementById("FeedbackOpenbtn").style.visibility = "visible";
                        //document.getElementById("FeedbackSubmittedText").style.color = "hidden";
                    } else {
                        $("#FeedbackOpenbtn").hide();
                        $("#FeedbackSubmittedText").show();
                        //document.getElementById("FeedbackOpenbtn").style.visibility = "hidden";
                        //document.getElementById("FeedbackSubmittedText").style.color = "visible";
                    }

                } else {
                    showToast(Json.data.message, 3000, 'red');
                }

            }
        });

    }
    $scope.hideAdHocDetails = function (id) {
        $scope.AdHocdetailsOpacity = 0;
        $scope.AdHocdetails = 0;
        try {
            $.ajax({
                url: 'CBP_AdHocScheduling/GetAppointmentsforCBPApp?userid=' + id,
                type: 'GET',
                success: function (Json) {
                    console.log("geeeting adhoc")
                    $scope.CBPAdHocAppointment = Json;
                    //console.log($scope.CBPAdHocAppointment);
                }
            });
        } catch (err) {
            console.log(err);
        }
    }
    $scope.RedirectMainAppt = function () {
        $scope.SearchBar = true;
        $scope.MainAppt = 0;
        $scope.CBPAdHocAppt = 0;
        //toggle button show
        $scope.navAdHocAppt = 1;
        $scope.navMainAppt = 0;

        //document.getElementById("AdHocApptMenu").style.background = "Transparent";
        //document.getElementById("AdHocApptMenu").style.color = "#888";

        //document.getElementById("CBPApptMenu").style.background = "rgba(73, 25, 158,.05)";
        //document.getElementById("CBPApptMenu").style.color = "#3296fa";


        //$('.mainAppt').removeClass("removeWhiteBox");
        //$('.CBPAdhocAppt').addClass("removeWhiteBox");
        //document.getElementById("searchbar").style.visibility = "visible";
        $scope.hideSetting();
    }

    $scope.ToggleCardRegistrationUserDDL = function (x) {

        if (x == 1) {
            $scope.CardClientSelection = 1;
            $scope.CardVolunteerSelection = 0;
            $("#btnToggleClientList").prop("disabled", true);
            $("#btnToggleVolunteerList").prop("disabled", false);
        }
        if (x == 2) {
            $scope.CardClientSelection = 0;
            $scope.CardVolunteerSelection = 1;
            $("#btnToggleClientList").prop("disabled", false);
            $("#btnToggleVolunteerList").prop("disabled", true);
        }
        //console.log("Client" + $scope.CardClientSelection);
        //console.log("Volunteer" + $scope.CardVolunteerSelection);
    }

    $scope.CancelAdHoc = function (CreatedBy, ApptId) {
        //alert(CreatedBy + "and" + ApptId);
        try {
            $.ajax({
                url: 'CBP_AdHocScheduling/CancelAdHocAppointment?ApptId=' + ApptId,
                type: 'POST',
                success: function (info) {
                    if (info.Success) {

                        $scope.hideAdHocDetails(CreatedBy);
                        $scope.ReloadCBPAdHocAppt(CreatedBy);
                        showToast('Successfully cancel appointment ', 3000, 'skyblue');
                    }
                }
            });
        } catch (err) {
            console.log(err);
        }

    }
});



function windowResize() {
    var scope = angular.element(document.getElementById("myApp")).scope();
    scope.$apply(function () {
        scope.windowResize();
    });
}

function onNumberRead(card_num) {
    try {

        try {
            stopReader();
        }
        catch (e) {
        }
        var scope = angular.element(document.getElementById("myApp")).scope();
        if (currentCard == 1) {
            izlink = card_num;
            scope.$apply(function () {
                scope.logInAjax(null, card_num);
            });
        }
        //else if (currentCard == 2) {
        //    scope.tripScanCard(card_num);
        //}
        else if (currentCard == 3) {
            scope.ScanCardForCheckInOut(card_num);
        }
        else if (currentCard == 4) {
            scope.$apply(function () {
                scope.updateData['Card'] = card_num;
            });
        }
        currentCard = 0;
    } catch (err) {
        var scope = angular.element(document.getElementById("myApp")).scope();
        scope.cardReaderAccess = false;
    }
}

function startReader() {
    AndroidCardReader.startReader();
}

function stopReader() {
    AndroidCardReader.stopReader();
};

function androidMSG(from, GPSnTime, numPlate) {
    if (from == 1) {
        var scope = angular.element(document.getElementById("myApp")).scope();
        scope.$apply(function () {
            scope.cardReaderAccess = true;
        });
        try {
            startReader();
        }
        catch (e) {

        }
    }
}

var toastTimeOut;
function showToast(i, ii, iii) {
    document.getElementById("toast").style = "padding:5px 50px;background-color:" + iii + ";border-radius:0 0 100px 100px;transform:translateY(0%);opacity:0;transition:0;";
    document.getElementById("toast").innerHTML = i;
    clearTimeout(toastTimeOut);
    window.setTimeout(function () {
        document.getElementById("toast").style = "padding:5px 50px;background-color:" + iii + ";border-radius:0 0 100px 100px;transform:translateY(100%);opacity:1;transition:1s;";
        toastTimeOut = window.setTimeout(function () {
            document.getElementById("toast").style = "padding:5px 50px;background-color:" + iii + ";border-radius:0 0 100px 100px;transform:translateY(0%);opacity:0;transition:1s;";
        }, ii);
    }, 1);
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


function onchangeStartTime() {
    var StartTime = moment($('#AdHocAppt_StartTime').val(), "HH:mm").format("HH:mm");
    var EndTime = moment($('#AdHocAppt_StartTime').val(), "HH:mm").format("HH:mm");
    $('#AdHocAppt_StartTime').val(StartTime).change();
    $('#AdHocAppt_EndTime').val(EndTime);
    $('#AdHocAppt_EndTime').change();
}
function checkDate(id) {
    if (id === 0) {
        startDate = $('#AdHocAppt_StartTime').val();
        endDate = $('#AdHocAppt_EndTime').val();
        //console.log(startDate)
        if (startDate != null && startDate != 'undefined') {
            //console.log("e")
            //console.log(endDate < startDate)
            if (endDate < startDate) {
                showToast('Start Time should not greater than end time ', 3000, 'red');
                $('#AdHocAppt_EndTime').focus();
            }
        }

    }
}
function AdHocApptFormValidation() {
    var AdHocAppt_Date = $('#AdHocAppt_BookingDate').val();
    var AdHocAppt_StartTime = $('#AdHocAppt_StartTime').val();
    //var AdHocAppt_EndTime = $('#AdHocAppt_EndTime').val();
    var AdHocAppt_Activity = $('#DDLActivity option:selected').val();
    var AdHocAppt_Client = $('#DDLClient option:selected').val();

    if (AdHocAppt_Date == "") {
        showToast('Date field is required ', 3000, 'red');
        $('#AdHocAppt_BookingDate').focus();
        return "false";
    }

    if (AdHocAppt_StartTime == "") {
        showToast('Start Time is required ', 3000, 'red');
        $('#AdHocAppt_StartTime').focus();
        return "false";
    }
    //activity is required
    if (AdHocAppt_Activity == "") {
        showToast('Activity field is required ', 3000, 'red');
        $('#DDLActivity').focus();
        return "false";
    }
    //Client field is required
    if (AdHocAppt_Client == "") {
        showToast('Client field is required ', 3000, 'red');
        $('#DDLClient').focus();
        return "false";
    }
    //check at least one kaki
    if (!($("#AdHocKaki1").val() != null || $("#AdHocKaki2").val() != null || $("#AdHocKaki3").val() != null || $("#AdHocKaki4").val() != null || $("#AdHocKaki5").val() != null)) {
        showToast('Atleast one kaki is required ', 3000, 'red');
        $('#AdHocKaki1').focus();
        return "false";
    }
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

