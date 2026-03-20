
$(document).ready(function () {
  //$("#key_apprv_stock").val("WO-582899 : 0083187200");
  var strWo = $("#strWo").val();
  //console.log(strWo);
  if (strWo != "") {
    console.log(strWo);
    getWODetail(strWo);
  }

  //
  //var date = new Date();
  //var today = new Date(date.getFullYear(), date.getMonth(), date.getDate());
  
  var today = new Date();
  console.log(today);
  document.getElementById("create_dt").value = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);

});

/*
$(document).on('change', 'input', function () {
  var options = $('datalist')[0].options;
  var val = $(this).text();
  //var val = $(this).text();
  console.log(val);
  //console.log($(this).find("option:selected").val());
  //alert($(this).find("option:selected").text());
  //for (var i = 0; i < options.length; i++) {
  //  console.log(options[i].text());
  //  //if (options[i].value === val) {
  //  //  console.log(val);
  //  //  break;
  //  //}

  //  if (options[i].text === val) {
  //    console.log(val);
  //    break;
  //  }

  //}

});
*/

$("#key_apprv_stock").change(function () {
  var val = $(this).val();
  getWODetail(val);
});
function getWODetail(strWo) {
  var arr = strWo.split(':');
  if (arr.length == 2) {
    var str_wo = arr[0];
    var str_itemno = arr[1];
    $("#title_wo").html(str_wo);
    $("#det_itemno").html(str_itemno);

    var styOpr10 = document.getElementById("div_opr10");
    var styOpr20 = document.getElementById("div_opr20");
    var styOpr30 = document.getElementById("div_opr30");
    var styOpr40 = document.getElementById("div_opr40");
    var styOpr50 = document.getElementById("div_opr50");
    styOpr10.style.display = "none";
    styOpr20.style.display = "none";
    styOpr30.style.display = "none";
    styOpr40.style.display = "none";
    styOpr50.style.display = "none";
    //--- Get information WO detail //
    var obj = {
      itemParts: str_itemno
    }
    $.ajax({
      url: "/SGA/GetWOTimeSheetDet",
      type: "GET",
      data: obj,
      success: function (response) {
        //styOpr10.style.display = "block";
        if (response.length >= 1) {
          let i = 0;
          Object.keys(response).forEach(key => {
            //holidays.push(new Date(response[i]["holiday_dt"]));
            //console.log(response[i]["oprQty"]);
            $("#det_itemno_info").html(response[i]["itemName"]);
            $("#det_qty").html(response[i]["procQty"]);

            var OPR = (response[i]["oprQty"] * 1);
            if (OPR == 10) {
              styOpr10.style.display = "block";
              $("#det_workcenter10").html(response[i]["oprCost"]); $("#det_stdtime10").html(response[i]["oprRuntime"]);
            }
            if (OPR == 20) {
              styOpr20.style.display = "block";
              $("#det_workcenter20").html(response[i]["oprCost"]); $("#det_stdtime20").html(response[i]["oprRuntime"]);
            }
            if (OPR == 30) {
              styOpr30.style.display = "block";
              $("#det_workcenter30").html(response[i]["oprCost"]); $("#det_stdtime30").html(response[i]["oprRuntime"]);
            }
            if (OPR == 40) {
              styOpr40.style.display = "block";
              $("#det_workcenter40").html(response[i]["oprCost"]); $("#det_stdtime40").html(response[i]["oprRuntime"]);
            }
            if (OPR == 50) {
              styOpr50.style.display = "block";
              $("#det_workcenter50").html(response[i]["oprCost"]); $("#det_stdtime50").html(response[i]["oprRuntime"]);
            }
            i++;
          });
        }
      },
      error: function (request, status, error) {
        alert(request.responseText);
      }
    });

  }
}
function clear_wolist() {
  $("#strWo").val("");
  $("#key_apprv_stock").val("");
  $("#title_wo").html("");
  $("#det_itemno").html("");
  $("#det_itemno_info").html("");
  $("#det_qty").html("");
  //$("#det_opr").html("");
  $("#det_workcenter").html("");
  $("#det_stdtime").html("");

  $("#det_workcenter10").html(""); $("#det_stdtime10").html("");
  $("#det_workcenter20").html(""); $("#det_stdtime20").html("");
  $("#det_workcenter30").html(""); $("#det_stdtime30").html("");
  $("#det_workcenter40").html(""); $("#det_stdtime40").html("");
  $("#det_workcenter50").html(""); $("#det_stdtime50").html("");


  var styOpr10 = document.getElementById("div_opr10");
  var styOpr20 = document.getElementById("div_opr20");
  var styOpr30 = document.getElementById("div_opr30");
  var styOpr40 = document.getElementById("div_opr40");
  var styOpr50 = document.getElementById("div_opr50");
  styOpr10.style.display = "none";
  styOpr20.style.display = "none";
  styOpr30.style.display = "none";
  styOpr40.style.display = "none";
  styOpr50.style.display = "none";

}

function AddTimeSheet(oprtype) {
  console.log(oprtype);

  var myModal = new bootstrap.Modal(document.getElementById("modalAddRecord"));
  myModal.show();
  $("#mod_orp").html(oprtype);
  if (oprtype == 10) {
    $("#mod_std").html($("#det_stdtime10").html());
    $("#mod_workc").html($("#det_workcenter10").html());
  }else if (oprtype == 20) {
    $("#mod_std").html($("#det_stdtime10").html());
    $("#mod_workc").html($("#det_workcenter10").html());
  }

  
  $("#emp_code").val($("#id_emp_stock").html());

}

//$(function () {
//  $("#key_apprv_stock").on("input", function (event) {
//    var val = $(this).val();
//    console.log("Input: " + val);
//    var opt = $("#search_wo option[value='" + val + "']");
//    console.log("Option: " + opt.text());
//    $(this).val(opt.text());
//  });
//});

function ViewTimeSheet(oprtype) {
  var myModal = new bootstrap.Modal(document.getElementById("modalViewRecord"));
  myModal.show();
  $("#mod_orp_view").html(oprtype);
}

function ConfirmSave() {
  var opr = $("#mod_orp").html();
  var wo = $("#title_wo").html();
  var item = $("#det_itemno").html();

  var emp_code = $("#emp_code").val();
  var create_dt = $("#create_dt").val();
  var rec_setup = $("#rec_setup").val();
  var rec_mc = $("#rec_mc").val();
  var rec_lab = $("#rec_lab").val();
  var rec_aqty = $("#rec_aqty").val();
  var rec_atotal = $("#rec_atotal").val();
  var rec_ngqty = $("#rec_ngqty").val();
  var rec_ngtotal = $("#rec_ngtotal").val();
  var rec_eff = $("#rec_eff").val();
  if (emp_code == "") return;
  var obj = {
      opr: opr,
      wo: wo,
      item: item,
      emp_code: emp_code,
      create_dt: create_dt,
      rec_setup: rec_setup,
      rec_mc: rec_mc,
      rec_lab: rec_lab,
      rec_aqty: rec_aqty,
      rec_atotal: rec_atotal,
      rec_ngqty: rec_ngqty,
      rec_ngtotal: rec_ngtotal,
      rec_eff: rec_eff
  }
  console.log(obj);
  if (!confirm("ยืนยันบันทึกข้อมูล Time Sheet")) return;

}
