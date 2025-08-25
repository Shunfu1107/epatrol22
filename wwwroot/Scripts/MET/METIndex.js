
var recordLiveLocation;
var searchTime;
var currentCard = 1;
var izlink = '';
//var recordTime = [];
var recordLocation = [];
var preventdoubleclick = "";
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
}

window.onbeforeunload = function (e) {
    if (recordLiveLocation != undefined) {
        return true;
    } else {
        return null;
    }
}

var app = angular.module('myApp', []);
app.controller('myCtrl', function ($scope, $timeout, $interval) {
    $scope.loading = $scope.logInAccount = $scope.allUserList = false;
    $scope.logInNum = $scope.details = $scope.details2 = $scope.details3 = 0;
    $scope.uploadProgress = -1;
    $scope.account = $scope.password = $scope.searchText = '';
    $scope.m = screen.width >= screen.height ? false : true;
    $scope.cardReaderAccess = true;

    $scope.windowResize = function () {
        $scope.m = screen.width >= screen.height ? false : true;
    }

    $scope.recordLiveLocation = function () {
        if (angular.isDefined(recordLiveLocation)) return;
        recordLiveLocation = $interval(function () {
            //recordTime.push(new Date().getTime().toString());
            try {
                checkGPS(2);
            } catch (e) {
                //recordLocation();
            }
        }, 5000);
    }

    $scope.stopRecordLiveLocation = function () {
        if (angular.isDefined(recordLiveLocation)) {
            $interval.cancel(recordLiveLocation);
            recordLiveLocation = undefined;
        }
    }

    $scope.changeLogInType = function () {
        $scope.account = $scope.password = "";
        $scope.logInAccount = !$scope.logInAccount;
        if ($scope.logInAccount && $scope.cardReaderAccess) {

            try {
                stopReader();
            } catch (e) {

            }
        } else if (!$scope.logInAccount && $scope.cardReaderAccess) {
            currentCard = 1;

            try {
                startReader();
            } catch (e) {

            }
        }
    }

    $scope.showTripDetails = function (i) {//Triger when first detail are clicked
        $scope.detailsOpacity = 0;
        $scope.details = 1;
        $scope.loading = true;
        $.ajax({
            url: '/MET/TripDetails',
            data: {
                Id: i, //appointment row id
                UserId: $scope.tripList.id //login user id
            },
            type: "POST",
            complete: function (res) {

                $timeout(function () {
                    $scope.loading = false;
                    if (res.status == 200 && typeof res.responseJSON !== 'undefined' && typeof res.responseJSON.UserId !== 'undefined') {
                        $scope.tripDetails = res.responseJSON;
                        console.log($scope.tripDetails);
                        $scope.detailsOpacity = 1;
                    } else {
                        showToast('Please check your connection and try again later.', 3000, 'red');
                        $scope.details = 0;
                    }
                }, 150);
            }
        });
    }

    $scope.showOtherTrip = function () {
        $scope.hideSetting();
        $scope.detailsOpacity = 0;
        $scope.details = 2;
        $scope.details2Opacity = 0;
        $scope.details2 = 2;
        $scope.details21 = 1;
        $scope.loading = true;
        $.ajax({
            url: '/MET/OtherTrip',
            data: {
                Id: $scope.tripList.id
            },
            type: "POST",
            complete: function (res) {
                $timeout(function () {
                    $scope.loading = false;
                    if (res.status == 200 && res.responseJSON.Id == 0) {
                        $scope.details = 0;
                        $scope.showDetails2(2);
                    } else if (res.status == 200 && typeof res.responseJSON !== 'undefined' && typeof res.responseJSON.Status !== 'undefined') {
                        $scope.tripDetails = res.responseJSON;
                        $scope.detailsOpacity = 1;
                        $scope.details2 = 0;
                    } else {
                        showToast('Please check your connection and try again later.', 3000, 'red');
                    }
                }, 150);
            }
        });
    }

    $scope.showDetails3 = function (i, ii, iii) {
        $scope.userList = iii;
        if ($scope.details3 == 0) {
            $scope.details3Opacity = 0;
        }
        $scope.details3 = i;
        if (i == 1 || i == 3) {
            $scope.details3User = ii;
            $scope.details31 = 1;
            $scope.other = {};
            if (iii == 'All') {
                $scope.allUserList = true;
                let tempList = $scope.tripList['UserList']['Client'].concat($scope.tripList['UserList']['Volunteer']).concat($scope.tripList['UserList']['Driver']).concat($scope.tripList['UserList']['Staff']);
                let hash = {};
                $scope.details3UserList = tempList.filter(function (word) {
                    if (hash[word.Id]) {
                        return false;
                    } else {
                        hash[word.Id] = true;
                        return true;
                    }
                });
            } else {
                $scope.details3UserList = $scope.tripList['UserList'][iii];
            }
            $scope.selectUser(typeof $scope.details3UserList === 'undefined' || $scope.details3UserList.length == 0 ? ($scope.details3 == 3 ? '{<Empty>}' : ($scope.updateDataPosition.length >= 0 ? '{<Cancel>}' : 'Other')) : 0);
        } else if (i == 2) {
            $scope.details3User = ii;
            $scope.details31 = 1;
            $scope.selectUser(0);
        }
        $timeout(function () {
            if (i == 1 || i == 2 || i == 3) {
                $scope.details31 = 0;
            }
            $scope.details3Opacity = 1;
        }, 1);
    }

    $scope.hideDetails3 = function () {
        $scope.details3Opacity = 0;
        if ($scope.details31 == 0) {
            $scope.details31 = 1;
        } else if ($scope.details32 == 0) {
            $scope.details32 = 1;
        }
        $timeout(function () {
            $scope.details3 = 0;
        }, 500);
    }

    $scope.showDetails2 = function (i, ii, iii) {
        $scope.details2Opacity = 0;
        $scope.details2 = i;
        $scope.details21 = 1;
        //start Trip button 1,0,0
        if (i == 1) {
            $scope.loading = true;
            $scope.photoShow = iii == 2 ? true : false;
            $scope.questionShow = iii == 0 ? true : false;
            $scope.moodShow = iii == 3 ? true : false;
            $.ajax({
                url: '/MET/getProcessingTrip',
                data: {
                    Account: $scope.logInAccount ? $scope.account : null,
                    Password: $scope.logInAccount ? $scope.password : izlink,
                    Id: $scope.tripList['id'], //login user id
                    tripId: $scope.tripDetails['Id'],//appointment row id 
                    recordId: ii
                },
                type: "POST",
                complete: function (res) {
                    $timeout(function () {
                        $scope.loading = false;
                        if (res.status == 200 && typeof res.responseJSON !== 'undefined' && typeof res.responseJSON.Client !== 'undefined' && res.responseJSON.ErrorMessage == null) {
                            var resp = res.responseJSON; //MetProcessingTrip model
                            console.log(resp);
                            if ($scope.cardReaderAccess) {
                                currentCard = 2;
                                try {
                                    startReader();
                                } catch (e) {
                                }
                            }
                            $scope.fileArray = null;
                            $scope.updateDataName = {
                                'Client': resp.Client == null ? $scope.tripDetails.UserName : resp.Client,
                                'Kaki1': resp.Kaki1 == null ? $scope.tripDetails.Kaki1Name : resp.Kaki1,
                                'Kaki2': resp.Kaki2 == null ? $scope.tripDetails.Kaki2Name : resp.Kaki2,
                                'Kaki3': resp.Kaki3 == null ? $scope.tripDetails.Kaki3Name : resp.Kaki3,
                                'Kaki4': resp.Kaki4 == null ? $scope.tripDetails.Kaki4Name : resp.Kaki4,
                                'Driver': resp.Driver == null ? (iii <= 1 ? $scope.tripDetails.Driver1Name : $scope.tripDetails.Driver2Name) : resp.Driver,
                                'Mood': resp.Mood
                            };
                            console.log($scope.updateDataName);
                            $scope.updateData = {
                                'tripId': $scope.tripDetails['Id'],
                                'recordId': ii,
                                'recordNum': iii,
                                'Client': resp.Client,
                                'Kaki1': resp.Kaki1,
                                'Kaki2': resp.Kaki2,
                                'Kaki3': resp.Kaki3,
                                'Kaki4': resp.Kaki4,
                                'Driver': resp.Driver,
                                'Odometer': resp.OdometerReading,
                                'ImageUrl': resp.ImageUrl,
                                'Mood': resp.Mood,
                                'Remark': resp.Remark,
                                'NoCard': resp.NoCard,
                                'QuestionsList': resp.QuestionsList
                            };
                            console.log($scope.updateData);
                            $scope.loading = false;
                            $timeout(function () {
                                $scope.details21 = 0;
                                $scope.details2Opacity = 1;
                            }, 150);
                        } else if (typeof res.responseJSON !== 'undefined' && typeof res.responseJSON.ErrorMessage !== 'undefined' && res.responseJSON.ErrorMessage != null) {
                            showToast(res.responseJSON.ErrorMessage, 3000, 'red');
                        } else {
                            showToast('Please check your connection and try again later.', 3000, 'red');
                        }
                    }, 150);
                }
            });
        }
        else if (i == 2) {
            $scope.updateDataName = [];
            $scope.updateDataPosition = [];
            $scope.updateData = [null, null];
            $timeout(function () {
                $scope.details21 = 0;
                $scope.details2Opacity = 1;
            }, 1);
        }
        else if (i == 3) {
            if ($scope.cardReaderAccess) {
                currentCard = 4;

                try {
                    startReader();
                } catch (e) {

                }
            }
            $scope.hideSetting();
            $scope.updateData = {
                'UserId': null,
                'UserName': null,
                'Card': null
            };
            $timeout(function () {
                $scope.details21 = 0;
                $scope.details2Opacity = 1;
            }, 1);
        }
    }


    //focussing on input field when keyboard pop up
    focusMethod = function getFocus() {

        var elmnt = document.getElementById("txtOdometer");
        elmnt.scrollIntoView();
    }

    focusMethod2 = function getFocus() {

        var elmnt = document.getElementById("txtRemark");
        elmnt.scrollIntoView();
    }
    focusMethod3 = function getFocus() {

        var elmnt = document.getElementById("txtOtherOdo");
        elmnt.scrollIntoView();
    }
    focusMethod4 = function getFocus() {

        var elmnt = document.getElementById("txtOtherrmk");
        elmnt.scrollIntoView();
    }
    btnGrey = function btnGrey() {
        showToast("Please complete the form before submit", "3000", "red");
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

    $scope.hideDetails = function () {
        $scope.detailsOpacity = 0;
        $timeout(function () {
            $scope.details = 0;
        }, 500);
    }

    $scope.logOut = function () {
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
        $scope.tripList = {};
        $scope.logInNum = 0;
        $scope.stopRecordLiveLocation();
        showToast('Logout Successful', 3000, 'skyblue');
        //}
    }

    $scope.empty = function () {
        event.stopPropagation();
    }

    $scope.addUser = function () {
        if ($scope.tripDetails.Kaki1 == null && typeof $scope.updateData["add1"] === 'undefined' && $scope.updateData["Kaki1"] == null) {
            $scope.updateData["add1"] = true;
        } else if ($scope.tripDetails.Kaki2 == null && typeof $scope.updateData["add2"] === 'undefined' && $scope.updateData["Kaki2"] == null) {
            $scope.updateData["add2"] = true;
        } else if ($scope.tripDetails.Kaki3 == null && typeof $scope.updateData["add3"] === 'undefined' && $scope.updateData["Kaki3"] == null) {
            $scope.updateData["add3"] = true;
        } else if ($scope.tripDetails.Kaki4 == null && typeof $scope.updateData["add4"] === 'undefined' && $scope.updateData["Kaki4"] == null) {
            $scope.updateData["add4"] = true;
        }
    }

    //old submit function for main trip submission
    //$scope.submit = function (i) {
    //    /* alert("reach submit function"+","+typeof i);*/
    //    if (typeof i !== 'undefined') {
    //        /*alert("entered if statement in submit" + $scope.updateData["Driver"]);*/
    //        if ($scope.cardReaderAccess) {
    //            currentCard = 0;
    //            try {
    //                stopReader();
    //            } catch (e) {
    //            }
    //        }
    //        var formData = new FormData();

    //        if (document.getElementById("photo") != null && document.getElementById("photo").files.length != 0) {
    //            if (document.getElementById("photo").files[0].type.includes("image")) {
    //                formData.set("file", document.getElementById("photo").files[0]);
    //            } else {
    //                showToast('Photo is not image format.', 3000, 'red');
    //                $scope.loading = false;
    //                return;
    //            }
    //        }
    //        //$scope.loading = true;
    //        formData.set("Account", $scope.logInAccount ? $scope.account : null);
    //        formData.set("Password", $scope.logInAccount ? $scope.password : izlink);
    //        formData.set("tripId", $scope.updateData["tripId"]);
    //        formData.set("recordId", $scope.updateData["recordId"]);
    //        formData.set("recordNum", $scope.updateData["recordNum"]);
    //        formData.set("Client", $scope.updateData["Client"]);
    //        formData.set("Kaki1", $scope.updateData["Kaki1"]);
    //        formData.set("Kaki2", $scope.updateData["Kaki2"]);
    //        formData.set("Kaki3", $scope.updateData["Kaki3"]);
    //        formData.set("Kaki4", $scope.updateData["Kaki4"]);
    //        formData.set("Driver", $scope.updateData["Driver"]);
    //        formData.set("Odometer", $scope.updateData["Odometer"]);
    //        formData.set("Mood", $scope.updateData["Mood"]);
    //        formData.set("Remark", $scope.updateData["Remark"]);
    //        formData.set("NoCard", $scope.updateData["NoCard"]);
    //        formData.set("Location", i);
    //        formData.set("QuestionsList", JSON.stringify($scope.updateData["QuestionsList"]));
    //        $.ajax({
    //            xhr: function () {
    //                var xhr = new window.XMLHttpRequest();
    //                xhr.upload.addEventListener("progress", function (evt) {
    //                    if (evt.lengthComputable && document.getElementById("photo") != null && document.getElementById("photo").files.length != 0) {
    //                        $scope.uploadProgress = ((evt.loaded / evt.total) * 100).toFixed(0);
    //                    }
    //                }, false);
    //                return xhr;
    //            },
    //            type: "POST",
    //            url: '/MET/Submit',
    //            processData: false,
    //            contentType: false,
    //            data: formData,
    //            beforeSend: function () {
    //                if (document.getElementById("photo") != null && document.getElementById("photo").files.length != 0) {
    //                    $scope.uploadProgress = 0;
    //                }
    //            },
    //            complete: function (res) {
    //                $timeout(function () {
    //                    $scope.loading = false;
    //                    $scope.uploadProgress = -1;
    //                    if (res.status == 200 && res.responseJSON.Resp == 'Success') {
    //                        if (res.responseJSON.liveLocation == 0) {
    //                            //$scope.stopRecordLiveLocation();
    //                        } else if (res.responseJSON.liveLocation == 1) {
    //                            //$scope.recordLiveLocation();
    //                        }
    //                        showToast('Update Successful', 3000, 'skyblue');
    //                        var tripId = $scope.tripDetails['Id'];
    //                        $scope.showTripDetails(tripId);
    //                        $scope.search();
    //                        $scope.hideDetails2();
    //                    } else if (res.status == 200 && res.responseJSON.Resp != '') {
    //                        showToast(res.responseJSON.Resp, 5000, 'red');
    //                        $scope.hideDetails2();
    //                    } else if (res.status == 200 && res.responseJSON.Resp == 'Multi Click') {
    //                        $scope.hideDetails2();
    //                    } else if (res.status != 200) {
    //                        showToast('Please check your connection and try again later.', 3000, 'red');
    //                    }
    //                    submitLoading = true;
    //                    preventdoubleclick = "";
    //                }, 150);
    //            }
    //        });
    //    } else {
    //        showToast('Please open your GPS Location and try again later.', 3000, 'red');
    //    }
    //}

    //new submit function for hadling submission for saving trip
    $scope.submit = function (i) {
        try {
            /* alert("reach submit function"+","+typeof i);*/

            /*alert("entered if statement in submit" + $scope.updateData["Driver"]);*/
            if ($scope.cardReaderAccess) {
                currentCard = 0;
                try {
                    stopReader();
                } catch (e) {
                }
            }
            var formData = new FormData();

            if (document.getElementById("photo") != null && document.getElementById("photo").files.length != 0) {
                if (document.getElementById("photo").files[0].type.includes("image")) {
                    formData.set("file", document.getElementById("photo").files[0]);
                } else {
                    showToast('Photo is not image format.', 3000, 'red');
                    $scope.loading = false;
                    return;
                }
            }
            //$scope.loading = true;
            formData.set("Account", $scope.logInAccount ? $scope.account : null);
            formData.set("Password", $scope.logInAccount ? $scope.password : izlink);
            formData.set("tripId", $scope.updateData["tripId"]);
            formData.set("recordId", $scope.updateData["recordId"]);
            formData.set("recordNum", $scope.updateData["recordNum"]);
            formData.set("Client", $scope.updateData["Client"]);
            formData.set("Kaki1", $scope.updateData["Kaki1"]);
            formData.set("Kaki2", $scope.updateData["Kaki2"]);
            formData.set("Kaki3", $scope.updateData["Kaki3"]);
            formData.set("Kaki4", $scope.updateData["Kaki4"]);
            formData.set("Driver", $scope.updateData["Driver"]);
            formData.set("Odometer", $scope.updateData["Odometer"]);
            formData.set("Mood", $scope.updateData["Mood"]);
            formData.set("Remark", $scope.updateData["Remark"]);
            formData.set("NoCard", $scope.updateData["NoCard"]);
            formData.set("Location", i);
            formData.set("QuestionsList", JSON.stringify($scope.updateData["QuestionsList"]));
            $.ajax({
                xhr: function () {
                    var xhr = new window.XMLHttpRequest();
                    xhr.upload.addEventListener("progress", function (evt) {
                        if (evt.lengthComputable && document.getElementById("photo") != null && document.getElementById("photo").files.length != 0) {
                            $scope.uploadProgress = ((evt.loaded / evt.total) * 100).toFixed(0);
                        }
                    }, false);
                    return xhr;
                },
                type: "POST",
                url: '/MET/Submit',
                processData: false,
                contentType: false,
                data: formData,
                beforeSend: function () {
                    if (document.getElementById("photo") != null && document.getElementById("photo").files.length != 0) {
                        $scope.uploadProgress = 0;
                    }
                },
                complete: function (res) {
                    $timeout(function () {
                        $scope.loading = false;
                        $scope.uploadProgress = -1;
                        if (res.status == 200 && res.responseJSON.Resp == 'Success') {
                            if (res.responseJSON.liveLocation == 0) {
                                //$scope.stopRecordLiveLocation();
                            } else if (res.responseJSON.liveLocation == 1) {
                                //$scope.recordLiveLocation();
                            }
                            showToast('Update Successful', 3000, 'skyblue');
                            var tripId = $scope.tripDetails['Id'];
                            $scope.showTripDetails(tripId);
                            $scope.search();
                            $scope.hideDetails2();
                        } else if (res.status == 200 && res.responseJSON.Resp != '') {
                            showToast(res.responseJSON.Resp, 5000, 'red');
                            $scope.hideDetails2();
                        } else if (res.status == 200 && res.responseJSON.Resp == 'Multi Click') {
                            $scope.hideDetails2();
                        } else if (res.status != 200) {
                            showToast('Please check your connection and try again later.', 3000, 'red');
                        }
                        submitLoading = true;
                        preventdoubleclick = "";
                    }, 150);
                }
            });

        } catch (e) {

            console.log(e);
        }

    }
	
	$scope.submitwithoutgps = function (i) {
        try {
            /* alert("reach submit function"+","+typeof i);*/

            /*alert("entered if statement in submit" + $scope.updateData["Driver"]);*/
            if ($scope.cardReaderAccess) {
                currentCard = 0;
                try {
                    stopReader();
                } catch (e) {
                }
            }
            var formData = new FormData();

            if (document.getElementById("photo") != null && document.getElementById("photo").files.length != 0) {
                if (document.getElementById("photo").files[0].type.includes("image")) {
                    formData.set("file", document.getElementById("photo").files[0]);
                } else {
                    showToast('Photo is not image format.', 3000, 'red');
                    $scope.loading = false;
                    return;
                }
            }
            //$scope.loading = true;
            formData.set("Account", $scope.logInAccount ? $scope.account : null);
            formData.set("Password", $scope.logInAccount ? $scope.password : izlink);
            formData.set("tripId", $scope.updateData["tripId"]);
            formData.set("recordId", $scope.updateData["recordId"]);
            formData.set("recordNum", $scope.updateData["recordNum"]);
            formData.set("Client", $scope.updateData["Client"]);
            formData.set("Kaki1", $scope.updateData["Kaki1"]);
            formData.set("Kaki2", $scope.updateData["Kaki2"]);
            formData.set("Kaki3", $scope.updateData["Kaki3"]);
            formData.set("Kaki4", $scope.updateData["Kaki4"]);
            formData.set("Driver", $scope.updateData["Driver"]);
            formData.set("Odometer", $scope.updateData["Odometer"]);
            formData.set("Mood", $scope.updateData["Mood"]);
            formData.set("Remark", $scope.updateData["Remark"]);
            formData.set("NoCard", $scope.updateData["NoCard"]);
            formData.set("Location", i);
            formData.set("QuestionsList", JSON.stringify($scope.updateData["QuestionsList"]));
            $.ajax({
                xhr: function () {
                    var xhr = new window.XMLHttpRequest();
                    xhr.upload.addEventListener("progress", function (evt) {
                        if (evt.lengthComputable && document.getElementById("photo") != null && document.getElementById("photo").files.length != 0) {
                            $scope.uploadProgress = ((evt.loaded / evt.total) * 100).toFixed(0);
                        }
                    }, false);
                    return xhr;
                },
                type: "POST",
                url: '/MET/Submit',
                processData: false,
                contentType: false,
                data: formData,
                beforeSend: function () {
                    if (document.getElementById("photo") != null && document.getElementById("photo").files.length != 0) {
                        $scope.uploadProgress = 0;
                    }
                },
                complete: function (res) {
                    $timeout(function () {
                        $scope.loading = false;
                        $scope.uploadProgress = -1;
                        if (res.status == 200 && res.responseJSON.Resp == 'Success') {
                            if (res.responseJSON.liveLocation == 0) {
                                //$scope.stopRecordLiveLocation();
                            } else if (res.responseJSON.liveLocation == 1) {
                                //$scope.recordLiveLocation();
                            }
                            showToast('Update Successful without gps', 3000, 'skyblue');
                            var tripId = $scope.tripDetails['Id'];
                            $scope.showTripDetails(tripId);
                            $scope.search();
                            $scope.hideDetails2();
                        } else if (res.status == 200 && res.responseJSON.Resp != '') {
                            showToast(res.responseJSON.Resp, 5000, 'red');
                            $scope.hideDetails2();
                        } else if (res.status == 200 && res.responseJSON.Resp == 'Multi Click') {
                            $scope.hideDetails2();
                        } else if (res.status != 200) {
                            showToast('Please check your connection and try again later.', 3000, 'red');
                        }
                        submitLoading = true;
                        preventdoubleclick = "";
                    }, 150);
                }
            });

        } catch (e) {

            console.log(e);
        }

    }

    $scope.submitOtherTrip = function () {
        //var formDataOtherTrip = new FormData();
        //formDataOtherTrip.set("tripId", $scope.updateData["tripId"]);
        //formDataOtherTrip.set("tripId", $scope.updateData["tripId"]);
        //formDataOtherTrip.set("tripId", $scope.updateData["tripId"]);
        //formDataOtherTrip.set("tripId", $scope.updateData["tripId"]);
        $scope.loading = true;
        $.ajax({
            url: '/MET/SubmitOtherTrip',
            data: {
                Id: $scope.tripList['id'],
                updateData: $scope.updateData,
                updateDataPosition: $scope.updateDataPosition,
            },
            type: "POST",
            complete: function (res) {
                $timeout(function () {
                    $scope.loading = false;
                    if (res.status == 200 && (res.responseJSON.liveLocation == 0 || res.responseJSON.liveLocation == 1)) {
                        if (res.responseJSON.liveLocation == 0) {
                            $scope.hideDetails();
                            //$scope.stopRecordLiveLocation();
                        } else if (res.responseJSON.liveLocation == 1) {
                            //$scope.recordLiveLocation();
                            $scope.showOtherTrip();
                        }
                        showToast(res.responseJSON.Resp, 3000, 'skyblue');
                        $scope.hideDetails2();
                    } else {
                        showToast('Please check your connection and try again later.', 3000, 'red');
                    }
                }, 150);
            }
        });
    }

    $scope.logInAjax = function (i, ii) {
        $scope.loading = true;
        $.ajax({
            url: '/MET/LogIn',
            data: {
                Account: i,
                Password: ii,
                search: $scope.searchText
            },
            type: "POST",
            complete: function (res) {

                if (res.status == 200 && res.responseText != '') {
                    $scope.tripList = res.responseJSON;
                    console.log(res);
                    $scope.tripList['UserList']['Mood'] = [{ 'Id': 'Happy', 'Name': 'Happy' }, { 'Id': 'Angry', 'Name': 'Angry' }, { 'Id': 'Sad', 'Name': 'Sad' }];
                    $timeout(function () {
                        if ($scope.logInNum != 10) {
                            if ($scope.tripList.liveLocation == 0) {
                                //$scope.stopRecordLiveLocation();
                            } else {
                                //$scope.recordLiveLocation();
                                if ($scope.tripList.liveLocationId == "OtherTrip") {
                                    $scope.showOtherTrip();
                                } else {
                                    $scope.showTripDetails($scope.tripList.liveLocationId);
                                }
                            }
                            $scope.recordLiveLocation();
                            showToast('Login Successful', 3000, 'skyblue');
                            $scope.logInNum = 10;
                        }
                        $scope.loading = false;
                    }, 150);
                } else if (res.status == 200 && res.responseText == '') {
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
                } else {
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

    //when login via id and ps
    $scope.logIn = function (i) {
        if ($scope.logInAccount) {
            if ($scope.account != "" && $scope.password != "") {
                $scope.logInAjax($scope.account, $scope.password);
            } else {
                showToast('Please fill the account and password.', 3000, 'red');
            }
        } else {
            $scope.scanCard('logIn');
        }
    };
    //Fire when scan card during login
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
        }
        else {
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

    $scope.setting = function () {
        $scope.settingOpacity = 0;
        $scope.settingShow = true;
        $timeout(function () {
            $scope.settingOpacity = 1;
        }, 1);
    }

    $scope.hideSetting = function () {
        $scope.settingOpacity = 0;
        $timeout(function () {
            $scope.settingShow = false;
        }, 500);
    }

    $scope.hideScanCard = function () {
        currentCard = 0;
        $scope.scanCardOpacity = 0;
        $timeout(function () {
            $scope.scanCardShow = false;
        }, 500);
    }

    $scope.filterUser = function () {
        var filter = document.getElementById("myInput").value.toUpperCase();
        if ($scope.allUserList) {
            let tempList = $scope.tripList['UserList']['Client'].concat($scope.tripList['UserList']['Volunteer']).concat($scope.tripList['UserList']['Driver']).concat($scope.tripList['UserList']['Staff']);
            let hash = {};
            $scope.details3UserList = tempList.filter(function (word) {
                if (hash[word.Id]) {
                    return false;
                } else if (word.Name.toUpperCase().indexOf(filter) > -1) {
                    hash[word.Id] = true;
                    return true;
                }
            });
        } else {
            $scope.details3UserList = $scope.tripList['UserList'][$scope.userList].filter(word => word.Name.toUpperCase().indexOf(filter) > -1);
        }
        $scope.selectUser($scope.details3UserList.length == 0 ? 'Other' : 0);
    }

    $scope.selectUser = function (i, ii) {
        if (i == 'Select') {
            if ($scope.details2 == 1 && !$scope.allUserList) {
                if ($scope.userSelect == 'Other' && ($scope.other.user == undefined || $scope.other.user == "")) {
                    showToast('Please fill the Name.', 3000, 'red');
                } else {
                    if ($scope.userSelect == '{<Empty>}') {
                        $scope.updateDataName[$scope.details3User] == 'Empty';
                        $scope.updateData[$scope.details3User] = $scope.userSelect;
                    } else {
                        $scope.updateDataName[$scope.details3User] = $scope.userSelect == 'Other' ? $scope.other.user : $scope.details3UserList[$scope.userSelect].Name;
                        $scope.updateData[$scope.details3User] = $scope.userSelect == 'Other' ? $scope.other.user : $scope.details3UserList[$scope.userSelect].Id;
                    }
                    if ($scope.details3User != "Mood") {
                        $scope.updateData['NoCard'] = $scope.updateData['NoCard'] == null ? $scope.details3User.toString() : $scope.updateData['NoCard'] + ' ' + $scope.details3User;
                    }
                    $scope.hideDetails3();
                }
            } else if ($scope.details2 == 2 && !$scope.allUserList) {
                if ($scope.details3User == 'new') {
                    $scope.updateDataPosition.push($scope.userSelect == 0 ? 'Client' : $scope.userSelect == 1 ? 'Volunteer' : $scope.userSelect == 2 ? 'Driver' : 'Other')
                    $scope.updateData.push(null)
                    if ($scope.cardReaderAccess) {
                        currentCard = 3;
                        try {
                            startReader();
                        } catch (e) {

                        }
                    }
                } else if ($scope.userSelect == '{<Cancel>}') {
                    $scope.updateDataPosition.pop();
                    $scope.updateData.pop();
                } else {
                    $scope.updateData[$scope.updateDataPosition.length + 1] = $scope.userSelect == 'Other' ? $scope.other.user : $scope.details3UserList[$scope.userSelect].Name;
                    $scope.updateDataName[$scope.updateDataPosition.length + 1] = $scope.updateData[$scope.updateDataPosition.length + 1];
                }
                $scope.hideDetails3();
            } else if ($scope.allUserList) {
                $scope.updateData['UserId'] = $scope.details3UserList[$scope.userSelect].Id;
                $scope.updateData['UserName'] = $scope.details3UserList[$scope.userSelect].Name;
                $scope.hideDetails3();
            }
        }
        else if (i == 'Confirm') {
            $scope.updateData['NoCard'] = $scope.updateData['NoCard'] == null ? ii : $scope.updateData['NoCard'] + ' ' + ii;
            $scope.updateData[ii] = ii == 'Client' ? $scope.tripDetails['UserId'] : $scope.tripList.id;
            if (ii == 'Driver') {
                $scope.updateData['Driver'] = $scope.tripDetails.Driver1;
            }


        }
        else {
            $scope.userSelect = i;
        }
    }

    //New added met confirm button
    $scope.confirmEscort = function (i, ii) {
        $scope.updateData['NoCard'] = $scope.updateData['NoCard'] == null ? ii : $scope.updateData['NoCard'] + ' ' + ii;
        if (ii == 'Kaki1') {
            $scope.updateData['Kaki1'] = parseInt($scope.tripDetails.Kaki1);
        }
        else if (ii == 'Kaki2') {
            $scope.updateData['Kaki2'] = $scope.tripDetails.Kaki2;
        }
        else if (ii == 'Kaki3') {
            $scope.updateData['Kaki3'] = $scope.tripDetails.Kaki3;
        }
        else if (ii == 'Kaki4') {
            $scope.updateData['Kaki4'] = $scope.tripDetails.Kaki4;
        }
    }

    $scope.recordQuestionCheckbox = function (ques, ans) {
        console.log(ques, ans)
        $scope.updateData['QuestionsList'][ques]['AnswersList'][ans].Status = !$scope.updateData['QuestionsList'][ques]['AnswersList'][ans].Status;
        console.log($scope.updateData['QuestionsList']);
    }

    $scope.search = function () {
        $timeout.cancel(searchTime);
        searchTime = $timeout(function () {
            if ($scope.logInAccount) {
                $scope.logInAjax($scope.account, $scope.password);
            } else {
                $scope.logInAjax(null, izlink);
            }
        }, 500);
    }

    $scope.showToast = function (i, ii, iii) {
        showToast(i, ii, iii);
    }

    $scope.regesterAjax = function (i, ii, iii) {
        $scope.loading = true;
        $.ajax({
            url: '/MET/CardRegistration',
            data: {
                UserId: $scope.updateData['UserId'],
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

    $scope.tripScanCard = function (i) {
        $scope.loading = true;
        $.ajax({
            url: '/MET/TripScanCard',
            data: {
                Id: $scope.tripList['id'],
                tripId: $scope.tripDetails['Id'],
                updateData: [i]
            },
            type: "POST",
            complete: function (res) {
                currentCard = 2;
                try {
                    startReader();
                } catch (e) {

                }
                if (res.status == 200 && res.responseText < 100) {
                    $scope.updateData[res.responseText == 0 ? 'Client' : (res.responseText == 5 ? 'Driver' : 'Kaki' + res.responseText)] = i;
                    $timeout(function () {
                        showToast('Scan Successful', 3000, 'skyblue');
                        $scope.loading = false;
                    }, 150);
                } else if (res.status == 200 && res.responseText > 100) {
                    $timeout(function () {
                        showToast(res.responseText == 101 ? 'Different Driver' : 'Account Not Found', 3000, 'red');
                        $scope.loading = false;
                    }, 150);
                } else {
                    $timeout(function () {
                        showToast('Please check your connection and try again later.', 3000, 'red');
                        $scope.loading = false;
                    }, 150);
                }
            }
        });
    }

    $scope.tripOtherScanCard = function (i) {
        $scope.updateData[$scope.updateData.length - 1] = i;
        $scope.updateDataName[$scope.updateData.length - 1] = i;
    }


    $scope.aaa = function () {
        console.log($scope.updateDataName);
        console.log($scope.updateData);
        console.log($scope.updateDataPosition);
    }
});

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

function getLocation(i) {
    //if (navigator.geolocation && i == 3) {
    var scope = angular.element(document.getElementById("myApp")).scope();
    if (i == 3) {
        try {
            if (preventdoubleclick != "") {
                return;
            }
            preventdoubleclick = "submiting.";
            scope.$apply(function () {
                scope.loading = true;
            });
            /*alert("clicked getlocation method");*/
            /*checkGPS(3);*/ /*comment out because weak gps affect*/
            androidMSG(3, "1|1", "1");
        } catch (e) {
            //navigator.geolocation.getCurrentPosition(showPosition, showError);
        }
        scope.loading = false;
        //} else if (navigator.geolocation && i == 1) {
    } else if (i == 1) {
        var scope = angular.element(document.getElementById("myApp")).scope();
        scope.$apply(function () {
            scope.submitOtherTrip();
        });
        scope.loading = false;
    } else {
        showToast("Geolocation is not supported by this browser.", 3000, 'red');
        scope.loading = false;
    }
}

//function showPosition(position) {
//    var scope = angular.element(document.getElementById("myApp")).scope();
//    scope.$apply(function () {
//        scope.loading = true;
//        scope.submit(position.coords.latitude + ',' + position.coords.longitude);
//    });
//}

//function recordLocation() {
//    if (navigator.geolocation) {
//        navigator.geolocation.getCurrentPosition(recordPosition, showError);
//    } else {
//        showToast("Geolocation is not supported by this browser.", 3000, 'red');
//    }
//}

//function recordPosition(position) {
//    $.ajax({
//        url: '/MET/RecordLocation',
//        data: {
//            Id: angular.element(document.getElementById("myApp")).scope().tripList['id'],
//            Location: position.coords.latitude + ',' + position.coords.longitude
//        },
//        type: "POST"
//    })
//}

function showError(error) {
    var scope = angular.element(document.getElementById("myApp")).scope();
    scope.$apply(function () {
        scope.loading = false;
    });
    switch (error.code) {
        case error.PERMISSION_DENIED:
            showToast("Please open the GPS and try again later.", 3000, 'red');
            break;
        case error.POSITION_UNAVAILABLE:
            showToast("Location information is unavailable.", 3000, 'red');
            break;
        case error.TIMEOUT:
            showToast("The request to get user location timed out, please try again later.", 3000, 'red');
            break;
        case error.UNKNOWN_ERROR:
            showToast("An unknown error occurred.", 3000, 'red');
            break;
    }
}

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
        } catch (e) {

        }
        var scope = angular.element(document.getElementById("myApp")).scope();
        if (currentCard == 1) {
            izlink = card_num;
            scope.$apply(function () {
                scope.logInAjax(null, card_num);
            });
        }
        else if (currentCard == 2) {
            scope.tripScanCard(card_num);
        }
        else if (currentCard == 3) {
            scope.tripOtherScanCard(card_num);
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
}

function androidMSG(from, GPSnTime, numPlate) {
    //change to 3 bcs 0 in android studio is default value
    //if (from == 0) {
    var GPSnTimeStr = GPSnTime.split(" | ");
    var msg = GPSnTimeStr[0]; // last location
    var newGPSmsg = recordLocation[recordLocation.length - 1];
    /*
    /*alert(newGPSmsg);

    /*var newGPSmsg = "1,1";*/
    if (from == 3) {
        //if (newGPSmsg == "undefined") {
        //    showToast("Please open your GPS location and submit again.", 3000, 'red');
        //} else {
        //    var scope = angular.element(document.getElementById("myApp")).scope();
        //    scope.$apply(function () {
        //        //scope.loading = true; 
        //        /*alert("going to submit");*/
        //        scope.submit(newGPSmsg);
        //    });
        //}
        if (newGPSmsg == undefined) {//if cannot get gps just send a message indicating submitting without gps
            showToast("Submitting without gps, may due to weak gps issue", 2000, "red");
            var scope = angular.element(document.getElementById("myApp")).scope();
            scope.$apply(function () {
                //scope.loading = true; 
                /*alert("going to submit");*/
                scope.submitwithoutgps(newGPSmsg);
            });
        } else {
            var scope = angular.element(document.getElementById("myApp")).scope();
            scope.$apply(function () {
                //scope.loading = true; 
                /*alert("going to submit");*/
                scope.submit(newGPSmsg);
            });
        }
    } else if (from == 1) {
        var scope = angular.element(document.getElementById("myApp")).scope();
        scope.$apply(function () {
            scope.cardReaderAccess = true;
        });
        try {
            startReader();
        } catch (e) {

        }
    } else if (from == 2 && msg != "false" && angular.element(document.getElementById("myApp")).scope().tripList != {}) {
        recordLocation.push(msg);
        //var time = recordTime.shift();
        var time = GPSnTimeStr[1]; //lastlocation time
        //if (recordLocation.length > 50 && isGPSDisUnder10M(recordLocation.shift(), msg)) {
        //    return;
        //}
        if (typeof time != 'undefined') {
            $.ajax({
                url: '/MET/RecordLocation',
                data: {
                    Id: angular.element(document.getElementById("myApp")).scope().tripList['id'],//userid
                    Location: msg,//latitud longitud
                    NumberPlate: numPlate,
                    DateTimeOld: time
                },
                type: "POST",

            })
        }
    }
}

function isGPSDisUnder10M(oldGPS, newGPS) {
    var oldGPS = oldGPS.split(",");
    var newGPS = newGPS.split(",");
    var latDistance = (Math.abs(oldGPS[0] - newGPS[0])) * (Math.PI / 180);
    var lngDistance = (Math.abs(oldGPS[1] - newGPS[1])) * (Math.PI / 180);
    var a = (Math.sin(latDistance / 2) * Math.sin(latDistance / 2)) +
        (Math.cos((oldGPS[0]) * (Math.PI / 180))) *
        (Math.cos((newGPS[0]) * (Math.PI / 180))) *
        (Math.sin(lngDistance / 2)) *
        (Math.sin(lngDistance / 2));

    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));

    var dist = 6371 * c;

    if (dist < 0.01) {
        return;
    }
    if (dist < 0.01) {
        return true;
    } else {
        return false;
    }
}

function checkGPS(from) {

    AndroidCardReader.checkGPS(from);
}

function checkStartReader() {
    try {
        startReader();
    } catch (error) {
        var scope = angular.element(document.getElementById("myApp")).scope();
        scope.$apply(function () {
            scope.cardReaderAccess = false;
        });
    }
}

function checklalo() {//checking gps string
    showToast("Please wait 5 sec", 5000, 'skyblue');
    setTimeout(function () {
        getLastLocationtest();
    }, 5000);
}

function getLastLocationtest() {
    if (recordLocation[recordLocation.length - 1] != undefined) {
        var lastgps = recordLocation[recordLocation.length - 1];
        showToast(lastgps, 3000, 'skyblue');
    } else {
        showToast('No GPS detected/ GPS signal weak.', 3000, 'red');
    }
}