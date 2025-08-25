////$('#question1check').change(function () {
////    var data = [];
////    $('#question1check input[type=checkbox]').each(function () {
////        if (this.checked) {
////            data.push($(this).val());
////        }
////    });
////    $('#IAForm_q1').val(data.join());
////});
////function OnSuccessForm(result) {
////    //console.log(result.Data.message)
////    if (result.Success) {
////        let timerInterval
////        Swal.fire({
////            icon: 'success',
////            title: result.Data.message,
////            timer: 1300,
////            timerProgressBar: true,
////            didOpen: () => {
////                Swal.showLoading()
////                const b = Swal.getHtmlContainer().querySelector('b')
////                timerInterval = setInterval(() => {
////                    //b.textContent = Swal.getTimerLeft()
////                }, 100)
////            },
////            willClose: () => {
////                clearInterval(timerInterval)
////            }
////        }).then((result) => {
////            if (result.dismiss === Swal.DismissReason.timer) {
////                window.location.reload();
////            }
////        })

////    } else {

////        Swal.fire({
////            icon: 'error',
////            title: 'Error',
////            /*            text: result.Data.message*/
////            text: "The user has submitted the form before"

////        })
////        return;
////    }

////}
////function OnFailure(result) {
////    console.log("failure");
////    //console.log(result);
////}
////function fieldValidation() {

////    //Validation at each page
////    $('[data-toggle="tab"]').on('show.bs.tab', function (e) {
////        var form = $(MEL.toSelector('btnSubmit'));

////        if (!form.valid()) {
////            $('.input-validation-error').first().focus();
////            e.preventDefault();
////            return;
////        }
////    })
////}
////function rdClick(name) {
////    var Val;
////    if (name == "q3") {
////        Val = $("input[name='IAForm.q3']:checked").val();
////        var txt = document.getElementById('IAForm_q3no');
////        if (Val == "No") {
////            txt.style = "display:block;margin-top:10px";
////        }
////        else {
////            txt.style = "display:none;";
////        }
////    }
////    if (name == "q7") {
////        Val = $("input[name='IAForm.q7']:checked").val();
////        var txt = document.getElementById('IAForm_q7no');
////        if (Val == "No") {
////            txt.style = "display:block;margin-top:10px";
////        }
////        else {
////            txt.style = "display:none;";
////        }
////    }
////    if (name == "q8") {
////        Val = $("input[name='IAForm.q8']:checked").val();
////        var txt = document.getElementById('IAForm_q8no');
////        if (Val == "No") {
////            txt.style = "display:block;margin-top:10px";
////        }
////        else {
////            txt.style = "display:none;";
////        }
////    }
////    if (name == "q9") {
////        Val = $("input[name='IAForm.q9']:checked").val();
////        var txt = document.getElementById('IAForm_q9no');
////        if (Val == "No") {
////            txt.style = "display:block;margin-top:10px";
////        }
////        else {
////            txt.style = "display:none;";
////        }
////    }
////    if (name == "q10") {
////        Val = $("input[name='IAForm.q10']:checked").val();
////        var txt = document.getElementById('IAForm_q10no');
////        if (Val == "No") {
////            txt.style = "display:block;margin-top:10px";
////        }
////        else {
////            txt.style = "display:none;";
////        }
////    }
////    if (name == "q11") {
////        Val = $("input[name='IAForm.q11']:checked").val();
////        var txt = document.getElementById('IAForm_q11no');
////        if (Val == "No") {
////            txt.style = "display:block;margin-top:10px";
////        }
////        else {
////            txt.style = "display:none;";
////        }
////    }
////    if (name == "q13") {
////        Val = $("input[name='IAForm.q13']:checked").val();
////        var txt = document.getElementById('IAForm_q13no');
////        if (Val == "No") {
////            txt.style = "display:block;margin-top:10px";
////        }
////        else {
////            txt.style = "display:none;";
////        }
////    }
////    if (name == "q14") {
////        Val = $("input[name='IAForm.q14']:checked").val();
////        var txt = document.getElementById('IAForm_q14yes');
////        if (Val == "Yes") {
////            txt.style = "display:block;margin-top:10px";
////        }
////        else {
////            txt.style = "display:none;";
////        }
////    }
////}