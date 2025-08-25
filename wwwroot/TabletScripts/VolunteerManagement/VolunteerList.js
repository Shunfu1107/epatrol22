$(document).ready(function () {
    var t = $('#Volunteer').DataTable({
        "pagingType": "full_numbers"
    });
    t.on('order.dt search.dt', function () {
        t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
    $("table").removeAttr("style");
})
function rdClick(name) {
    var Val;
    if (name == "q3") {
        Val = $("input[name='IAForm.q3']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q3");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q7") {
        Val = $("input[name='IAForm.q7']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q7");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q8") {
        Val = $("input[name='IAForm.q8']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q8");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q9") {
        Val = $("input[name='IAForm.q9']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q9");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q10") {
        Val = $("input[name='IAForm.q10']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q10");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q11") {
        Val = $("input[name='IAForm.q11']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q11");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q13") {
        Val = $("input[name='IAForm.q13']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q13");
        if (Val == "No") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
    if (name == "q14") {
        Val = $("input[name='IAForm.q14']:checked").val();
        var txt = document.getElementById("txt_AnnualForm_Q14");
        if (Val == "Yes") {
            txt.style = "display:block;margin-top:10px";
        }
        else {
            txt.style = "display:none;";
        }
    }
}