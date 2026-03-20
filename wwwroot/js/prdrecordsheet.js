const MAIL_FLG = 1; //0=Debug, 1 =PILOT
let FApp1 = null;
let FApp2 = null;
let FApp3 = null;
$(document).ready(function () {
  //$("#key_apprv_stock").val("WO-582899 : 0083187200");
  var strWo = $("#strWo").val();
  //console.log(strWo);
  if (strWo != "") {
    console.log(strWo);
    //#debug--> getWODetail(strWo);
  }

  //
  //var date = new Date();
  //var today = new Date(date.getFullYear(), date.getMonth(), date.getDate());
  
  var today = new Date();
  console.log(today);
  document.getElementById("create_dt").value = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
  document.getElementById("rec_date").value = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);

  //---> ดึงข้อมูลแสดงในตารางของแต่ละ Operation
  ReadAllTask();
  ReadAllScrap();
  // ตรวจสอบว่ามีการส่ง ID ของรายการ Scrap มาด้วย
  let loginMail = $("#id_emp_stock_email").html();
  var styrowCk1 = document.getElementById("rowCk1");
  var styrowCk2 = document.getElementById("rowCk2");
  var styrowCk3 = document.getElementById("rowCk3");
  //styrowCk1.style.display = "none";
  //styrowCk2.style.display = "none";
  //styrowCk3.style.display = "none";
  
  document.getElementById("ckApp1").disabled = true;
  document.getElementById("ckApp2").disabled = true;
  document.getElementById("ckApp3").disabled = true;
    
  var strScrapId = $("#scrap_id").html();
  if (strScrapId != "") {
    styrowCk1.style.display = "block";
    styrowCk2.style.display = "block";
    styrowCk3.style.display = "block";
    handleActionScrap(strScrapId, loginMail);
  }
  const adm_docrole = $("#adm_docrole").html();
  const wo_status = $("#wo_status").html();
  var button = jQuery(document.getElementById("btnCloseDoc"));
  if (wo_status == 1) {
    $('#btnCloseDoc').text('CLOSED');
    button.removeClass("btn btn-warning").addClass("btn btn-success");
  } else {
    $('#btnCloseDoc').text('OPEN');
    button.removeClass("btn btn-success").addClass("btn btn-warning");
  }
  if (adm_docrole == 1) {
    $("#btnCloseDoc").attr("style", "display:inline");
  } else {
    $("#btnCloseDoc").attr("style", "display:none!important;");
  }
  
});

function ReadAllTask() {
  var tabTaskTables = [];
  // ค้นหา table ที่ id ขึ้นต้นด้วย 'tabTask_'
  $("table[id^='tabTask_']").each(function () {
    var tableId = $(this).attr("id");
    tabTaskTables.push(tableId);
  });
  // แสดงรายชื่อ table ด้วย console.log
  tabTaskTables.forEach(function (id) {
    //console.log(id);
    // Start ดึงข้อมูลทีละตาราง
    let arr_opr = id.split('_');
//    console.log(arr_opr[1]);
    var wo = $("#title_wo").html();
    var item = $("#det_itemno").html();
    var obj = {
      mas_wo: wo,
      mas_itemno: item,
      mas_opr: arr_opr[1]
    }

    $.ajax({
      url: "/SGA/GetTimeSheetRecsSum",
      type: "POST",
      data: obj,
      success: function (response) {
        //console.log(response);
        //-- Start DataTables
        
        if ($.fn.DataTable.isDataTable('#' + id)) {
          $('#' + id).DataTable().clear().destroy();
        }

        $('#' + id).DataTable({
          data: response,
          paging: false,
          info: false,
          searching: false,
          columns: [
            //rec_date	rec_setup	rec_mc	rec_lab	rec_aqty	rec_atotal, ng_qty. ng_total
            {
              data: 'rec_date_txt',
              render: function (data, type, row) {
                return `<B>${data}</B>`;
              }
            },
            { data: 'rec_setup' },
            { data: 'rec_mc' },
            { data: 'rec_lab' },
            { data: 'rec_aqty' },
            { data: 'rec_atotal' },
            { data: 'ng_qty' }
            /*{ data: 'ng_total' }*/
          ]
        });
        
        //-- End DataTables

      },
      error: function (request, status, error) {
        alert(request.responseText);
      }
    });
    // End ดึงข้อมูลทีละตาราง
  });

}
function ReadAllScrap() {
  var tabTaskTables = [];
  // ค้นหา table ที่ id ขึ้นต้นด้วย 'tabScrap_'
  $("table[id^='tabScrap_']").each(function () {
    var tableId = $(this).attr("id");
    tabTaskTables.push(tableId);
  });
  // แสดงรายชื่อ table ด้วย console.log
  tabTaskTables.forEach(function (id) {
    //console.log(id);
    // Start ดึงข้อมูลทีละตาราง
    let arr_opr = id.split('_');
    //console.log(arr_opr[1]);
    var wo = $("#title_wo").html();
    var item = $("#det_itemno").html();
    var obj = {
      mas_wo: wo,
      mas_itemno: item,
      mas_opr: arr_opr[1]
    }

    $.ajax({
      url: "/SGA/GetScrapSheetRecsSum",
      type: "POST",
      data: obj,
      success: function (response) {
        //console.log(response);
        //-- Start DataTables
        
        if ($.fn.DataTable.isDataTable('#' + id)) {
          $('#' + id).DataTable().clear().destroy();
        }

        $('#' + id).DataTable({
          data: response,
          paging: false,
          info: false,
          searching: false,
          columns: [
            //rec_date	rec_setup	rec_mc	rec_lab	rec_aqty	rec_atotal, ng_qty. ng_total
            {
              data: 'rec_date_txt',
              render: function (data, type, row) {
                return `<B>${data}</B>`;
              }
            },
            { data: 'prod_count' },
            { data: 'vedd_count' },
            { data: 'approval_txt' }
          ]
        });
        
        //-- End DataTables

      },
      error: function (request, status, error) {
        alert(request.responseText);
      }
    });
    // End ดึงข้อมูลทีละตาราง
  });

}

$("#key_apprv_stock").change(function () {
  var val = $(this).val();
  //#debug--> getWODetail(val);
});
function getWODetail(strWo) {
  var arr = strWo.split(':');
  if (arr.length == 2) {
    var str_wo = arr[0];
    var str_itemno = arr[1];
    $("#title_wo").html(str_wo);
    $("#det_itemno").html(str_itemno);

    /*
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
    */
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
            console.log(response[i]["oprQty"]);
            $("#det_itemno_info").html(response[i]["itemName"]);
            $("#det_qty").html(response[i]["procQty"]);


            /*
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
            */
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

  /*
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
  */

}
let SumATotal = 0;
function AddTimeSheet(woqty, stdtime, resources, oprtype, setup, mctime, oprtime) {

  console.log(oprtype);
  $("#rec_mc").val(0);
  $("#rec_lab").val(0);
  $("#rec_aqty").val(0);
  $("#rec_eff").val(0);

  var obj = {
    mas_wo: $("#title_wo").html(),
    mas_itemno: $("#det_itemno").html(),
    mas_opr: oprtype
  }
  $.ajax({
    url: "/SGA/FUNCGetSumAct",
    type: "POST",
    data: obj,
    success: function (response) {
      console.log(response[2]);
      $("#rec_atotal").val(response[2]);
      SumATotal = response[2];
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });

  //$("#rec_atotal").val(999);

  var myModal = new bootstrap.Modal(document.getElementById("modalAddRecord"));
  myModal.show();
  $("#mod_orp").html(oprtype);
  $("#mod_qty").html(woqty);
  //if (oprtype == 10) {
  //  $("#mod_std").html($("#det_stdtime10").html());
  //  $("#mod_workc").html($("#det_workcenter10").html());
  //}else if (oprtype == 20) {
  //  $("#mod_std").html($("#det_stdtime10").html());
  //  $("#mod_workc").html($("#det_workcenter10").html());
  //}

  //-->เตย
  $("#mod_std").html(stdtime);

  $("#mod_workc").html(resources);

  $("#emp_code").val($("#id_emp_stock").html());
  //$("#rec_setup").val(setup);
  $("#rec_setup").val(0); // ที่ประชุมขอปิด เอาเลข 0 แทน
  //-->เตย
  $("#mod_mctime").html(mctime);
  //-->เตย
  $("#mod_labtime").html(oprtime);

}
function CalATotal() {
  var mas_qty = $("#mod_qty").html() * 1;
  var mas_std = $("#mod_std").html() * 1;

  var set_mc = $("#rec_setup").val();
  var qty = ($("#mod_qty").html() * 1);
  var rec_aqty = ($("#rec_aqty").val() * 1); //จำนวนที่ทำได้จริง
  var sum_atotal = ((SumATotal*1) + (rec_aqty*1)); //รวมจำนวนกับยอดเก่า
  $("#rec_atotal").val(sum_atotal);

  //(A)#rec_aqty, (B)#rec_mc, (C)#mod_std // Eff ที่สรุปกับน้องเตย
  var rec_mc = ($("#rec_mc").val() * 1);
  console.log("AQty:" + rec_aqty);
  console.log("Lab:" + rec_mc);
  console.log("Std:" + mas_std);
  var eff_1 = ((rec_aqty / rec_mc) * 100) / (mas_std);
  console.log("Eff:" + eff_1);
  $("#rec_eff").val(eff_1.toFixed(2));

}
function ResetCalA() {
  $("#rec_mc").val(0);
  $("#rec_lab").val(0);
  $("#rec_aqty").val(0);
  $("#rec_eff").val(0);
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
function AddTimeSheetScrap(woqty, stdtime, resources, oprtype, setup, mctime, oprtime) {
  //alert(oprtype);
  var myModal = new bootstrap.Modal(document.getElementById("modalScrapRec"));
  myModal.show();
  $("#mod_orp_scr").html(oprtype);
  $("#emp_code_scr").val($("#id_emp_stock").html());
}
function ViewTimeSheetScrap(oprtype) {
  let txtBtnClose = $('#btnCloseDoc').text();
  var myModal = new bootstrap.Modal(document.getElementById("modalViewScrap"));
  myModal.show();
  $("#mod_orp_scr").html(oprtype);
  //scrapTable
  var wo = $("#title_wo").html();
  var item = $("#det_itemno").html();
  var obj = {
    mas_wo: wo,
    mas_itemno: item,
    mas_opr: oprtype
  }

  $.ajax({
    url: "/SGA/GetTimeSheetRecsScrap",
    type: "POST",
    data: obj,
    success: function (response) {
      console.log(response);
      //-- Start DataTables
      if ($.fn.DataTable.isDataTable('#scrapTable')) {
        $('#scrapTable').DataTable().clear().destroy();
      }

      $('#scrapTable').DataTable({
        data: response,
        paging: false,
        info: false,
        searching: false,
        columns: [
          { data: 'rec_date_txt' },
          { data: 'emp_code' },
          //{ data: 'mas_wo' },
          //{ data: 'mas_itemno' },
          { data: 'prd_setup' },
          { data: 'prd_tools' },
          { data: 'prd_surf' },
          { data: 'prd_dimout' },
          { data: 'prd_other' },
          { data: 'ven_hardness' },
          { data: 'ven_dimout' },
          { data: 'ven_surf' },
          { data: 'ven_other' },

          {
            //rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff
            data: null,
            orderable: false,
            render: function (data, type, row) {
              if (txtBtnClose == "CLOSED" || row.opr_stat == 1) {
                return `Locked`;
              } else {
                return `<button class="btn btn-sm btn-info" onclick="handleActionScrap(
                        '${row.ID}',''
                    )">ตรวจสอบ</button>
                    <button class="btn btn-sm btn-warning" onclick="deleteRowScrap('${row.ID}','${row.rec_date_txt}','${row.emp_code}')" style="margin-left:5px; color:red;">
                        ลบ
                    </button>
                    `;
              }
              

            }
          }
        ]
      });
      //-- End DataTables

    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
}
function ViewTimeSheet(oprtype, std_time) {
  let txtBtnClose = $('#btnCloseDoc').text();
  console.log(std_time);
  var myModal = new bootstrap.Modal(document.getElementById("modalViewRecord"));
  myModal.show();
  $("#mod_orp_view").html(oprtype);
  //GetTimeSheetRecs
  var wo = $("#title_wo").html();
  var item = $("#det_itemno").html();
  var obj = {
    mas_wo: wo,
    mas_itemno: item,
    mas_opr: oprtype
  }

  $.ajax({
    url: "/SGA/GetTimeSheetRecs",
    type: "POST",
    data: obj,
    success: function (response) {
      console.log(response);
      //-- Start DataTables
      if ($.fn.DataTable.isDataTable('#productionTable')) {
        $('#productionTable').DataTable().clear().destroy();
      }

      $('#productionTable').DataTable({
        data: response,
        paging: false,
        info: false,
        searching: false,
        columns: [
          { data: 'rec_date_txt' },
          { data: 'emp_code' },
          //{ data: 'mas_wo' },
          //{ data: 'mas_itemno' },
          { data: 'mas_opr' },
          {
            data: 'rec_setup',
            render: function (data, type, row) {
              return `<input type="text" class="rec_setup" value="${data}" data-rowid="${row.ID}">`;
            }
          },
          {
            data: 'rec_mc',
            render: function (data, type, row) {
              return `<input type="text" class="rec_mc" value="${data}" data-rowid="${row.ID}">`;
            }
          }, {
            data: 'rec_lab',
            render: function (data, type, row) {
              return `<input type="text" class="rec_lab" value="${data}" data-rowid="${row.ID}" onchange="changQty('${row.ID}', ${data},'${std_time}')">`;
            }
          },
          {
            data: 'rec_aqty',
            render: function (data, type, row) {
              return `<input type="text" class="rec_aqty" value="${data}" data-rowid="${row.ID}" onchange="changQty('${row.ID}', ${data},'${std_time}')">`;
            }
          },
          {
            data: 'rec_atotal',
            render: function (data, type, row) {
              return `<input type="text" class="rec_atotal" value="${data}" data-rowid="${row.ID}">`;
            }
          },
          {
            data: 'rec_eff',
            render: function (data, type, row) {
              return `<input type="text" class="rec_eff" value="${data}" data-rowid="${row.ID}">`;
            }
          },
          {
            //rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff
            data: null,
            orderable: false,
            render: function (data, type, row) {
              console.log(row.opr_stat);
              if (txtBtnClose == "CLOSED" || row.opr_stat == 1) {
                return `Locked`;
              } else {
                return `<button onclick="handleAction(
                        '${row.ID}'
                    )">อัพเดท</button>
                    <button onclick="deleteRow('${row.ID}','${row.rec_date_txt}','${row.emp_code}')" style="margin-left:5px; color:red;">
                        ลบ
                    </button>
                    `;
              }
              

            }
          }
        ]
      });
      //-- End DataTables

    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });

}
function handleAction(id) {
  //, rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff
  // ดึงค่าจาก input ตาม row ID
  const rec_setup = document.querySelector(`input.rec_setup[data-rowid='${id}']`)?.value || '';
  const rec_mc = document.querySelector(`input.rec_mc[data-rowid='${id}']`)?.value || '';
  const rec_lab = document.querySelector(`input.rec_lab[data-rowid='${id}']`)?.value || '';
  const rec_aqty = document.querySelector(`input.rec_aqty[data-rowid='${id}']`)?.value || '';
  const rec_atotal = document.querySelector(`input.rec_atotal[data-rowid='${id}']`)?.value || '';
  const rec_eff = document.querySelector(`input.rec_eff[data-rowid='${id}']`)?.value || '';

  // ส่งค่าทั้งหมด
  let obj = {
    ID: id,
    rec_setup: rec_setup,
    rec_mc: rec_mc,
    rec_lab: rec_lab,
    rec_aqty: rec_aqty,
    rec_atotal: rec_atotal,
    rec_eff: rec_eff
  };

  $.ajax({
    url: `/SGA/UPDTimeSheetRecord/?ID=${id}`,
    method: 'PUT',
    data: obj,
    success: function (response) {
      alert("บันทึกสำเร็จ");
      location.reload();
      // reloadDataTable(...); // ถ้าต้องรีเฟรชข้อมูล
    }
  });

  //alert(`ส่งข้อมูล:\nID: ${id}\nWO: ${wo}\nItem: ${itemno}\nOPR: ${opr}\nAQty: ${aqty}\nSetup: ${setup}\nEff: ${eff}`);
}
function handleActionScrap(id, loginMail) {
  //console.log(id +", "+ loginMail);
  var loginMail = $("#id_emp_stock_email").html();
  //modalScrapRecEdit, ID_scr, GetTimeSheetRecsScrapID
  var myModal = new bootstrap.Modal(document.getElementById("modalScrapRecEdit"));
  myModal.show();
  $("#ID_scr").html(id);
  var styID_scr = document.getElementById("ID_scr");
  styID_scr.style.display = "none";

  let obj = {
    ID: id
  }
  $.ajax({
    url: `/SGA/GetTimeSheetRecsScrapID`,
    method: 'POST',
    data: obj,
    success: function (response) {
      console.log(response[0]);
      //$("#edi_apprv2_scrap").val(response[0].app1_mail);
      let dt_rec = response[0].rec_date.split('T');

      $("#edi_mod_orp_scr").html(response[0].mas_opr);
      //$("#mod_orp_scr").html(response[0].mod_orp_scr);
      $("#edit_emp_code").val(response[0].emp_code);
      if (dt_rec.length == 2) {
        $("#edi_rec_date").val(dt_rec[0]);
      }      
      $("#edi_prd_setup").val(response[0].prd_setup);
      $("#edi_prd_tools").val(response[0].prd_tools);
      $("#edi_prd_surf").val(response[0].prd_surf);
      $("#edi_prd_dimout").val(response[0].prd_dimout);
      $("#edi_prd_other").val(response[0].prd_other);
      $("#edi_scrap_remark").val(response[0].scrap_remark);

      $("#edi_ven_hardness").val(response[0].ven_hardness);
      $("#edi_ven_dimout").val(response[0].ven_dimout);
      $("#edi_ven_surf").val(response[0].ven_surf);
      $("#edi_ven_other").val(response[0].ven_other);
      $("#edi_Supplier_remark").val(response[0].vendor_remark);
      $("#other_remark").val(response[0].other_remark);

      $("#edi_apprv1_scrap").val(response[0].app1_mail);
      $("#edi_apprv2_scrap").val(response[0].app2_mail);
      $("#edi_apprv3_scrap").val(response[0].app3_mail);

      $("#spn_apprv1_scrap").html(response[0].app1_mail);
      $("#spn_apprv2_scrap").html(response[0].app2_mail);
      $("#spn_apprv3_scrap").html(response[0].app3_mail);

      // กำหนด Checkbox
      if (response[0].app1_stat == 1) {
        $("#ckApp1").prop('checked', true);
        FApp1 = 1;
      } else {
        $("#ckApp1").prop('checked', false);
        FApp1 = 0;
      }
      if (response[0].app2_stat == 1) {
        $("#ckApp2").prop('checked', true);
        FApp2 = 1;
      } else {
        $("#ckApp2").prop('checked', false);
        FApp2 = 0;
      }
      if (response[0].app3_stat == 1) {
        $("#ckApp3").prop('checked', true);
        FApp3 = 1;
      } else {
        $("#ckApp3").prop('checked', false);
        FApp3 = 0;
      }

      // ตรวจสอบ อีเมล์ login กับอนุมัติคนเดียวกัน
      //document.getElementById("ckApp1").disabled = true;
      //document.getElementById("ckApp2").disabled = true;
      //document.getElementById("ckApp3").disabled = true;
      //console.log(loginMail +", "+ $("#spn_apprv1_scrap").html());
      if (loginMail != "" && loginMail == $("#spn_apprv1_scrap").html()) {
        document.getElementById("ckApp1").disabled = false;
      } else {
        document.getElementById("ckApp1").disabled = true;
      }
      if (loginMail != "" && loginMail == $("#spn_apprv2_scrap").html()) {
        document.getElementById("ckApp2").disabled = false;
      } else {
        document.getElementById("ckApp2").disabled = true;
      }
      if (loginMail != "" && loginMail == $("#spn_apprv3_scrap").html()) {
        document.getElementById("ckApp3").disabled = false;
      } else {
        document.getElementById("ckApp3").disabled = true;
      }
      
    }
  });

}
function changQty(id, oval, std_time) {
  //console.log(id + ", " + ", " + oval + ", " + std_time);
  var rec_aqty = document.querySelector(`input.rec_aqty[data-rowid='${id}']`)?.value || '';
  var rec_atotal = document.querySelector(`input.rec_atotal[data-rowid='${id}']`)?.value || '';
  //console.log(id + '-' + oval + '-' + rec_atotal);
  let nval = 0;
  if (oval < (rec_aqty * 1)) {
    nval = (rec_aqty * 1) - oval;
    document.querySelector(`input.rec_atotal[data-rowid='${id}']`).value = (rec_atotal*1) + (nval*1);
    console.log('Case 1:' + nval);
  } else if (oval > (rec_aqty * 1)) {
    nval = oval - (rec_aqty * 1);
    document.querySelector(`input.rec_atotal[data-rowid='${id}']`).value = (rec_atotal * 1) - (nval * 1);
    console.log('Case 2:' + nval);
  } else {
    //document.querySelector(`input.rec_atotal[data-rowid='${id}']`).value = rec_atotal;
    return;
  }

  // Call EFF
  var rec_mc = document.querySelector(`input.rec_mc[data-rowid='${id}']`)?.value || '';
  let eff = ((rec_aqty / rec_mc) * 100) / std_time;
  console.log("[" + rec_aqty + "][" + rec_mc + "]" + "[" + std_time + "]");
  document.querySelector(`input.rec_eff[data-rowid='${id}']`).value = eff;
}
function deleteRow(id, rec_date_txt, emp_code) {
  
  // ตัวอย่างแสดงข้อมูลก่อนลบ
  if (confirm(`คุณแน่ใจหรือไม่ว่าต้องการลบรายการ Timesheet \n วันที่: ${rec_date_txt} \n รหัสพนักงาน: ${emp_code}?`)) {
    console.log("ลบรายการ:", id);

    // 🔻 คุณสามารถเรียก API ลบ หรือทำอย่างอื่นที่ต้องการ
    // เช่น ส่ง AJAX ไปที่ Controller
    $.ajax({
        url: `/SGA/DELTimeSheetRecord/${id}`,
        method: 'DELETE',
        success: function(response) {
            alert("ลบสำเร็จ");
          location.reload();
        }
    });
    
  }
}
function deleteRowScrap(id, rec_date_txt, emp_code) {
  
  // ตัวอย่างแสดงข้อมูลก่อนลบ
  if (confirm(`คุณแน่ใจหรือไม่ว่าต้องการลบรายการ Scrap \n วันที่: ${rec_date_txt} \n รหัสพนักงาน: ${emp_code}?`)) {
    console.log("ลบรายการ:", id);

    // 🔻 คุณสามารถเรียก API ลบ หรือทำอย่างอื่นที่ต้องการ
    // เช่น ส่ง AJAX ไปที่ Controller
    $.ajax({
      url: `/SGA/DelScrapTimeSheetRecsID/${id}`,
        method: 'DELETE',
        success: function(response) {
            alert("ลบสำเร็จ");
          location.reload();
        }
    });
    
  }
}
function ConfirmScrapSaveEdit() {
  var SCRID = $("#ID_scr").html();
  if (SCRID == null) return;
  if (!confirm("ยืนยันการบันทึก")) return;

  var opr = $("#mod_orp").html();
  var wo = $("#title_wo").html();
  var item = $("#det_itemno").html();
  var emp_code = $("#emp_code").val();
  var urlApprove = $("#urlApprove").html();

  //rec_date: $("#rec_date").val(),
  var apprv_1 = $('[name="edi_apprv1_scrap"]').val();
  var arrAprov1 = apprv_1.split(':');
  let App1 = "";
  if (arrAprov1.length == 2) {
    App1 = arrAprov1[1].trim();
  }else {
    App1 = $("#edi_apprv1_scrap").val();
  }
  var apprv_2 = $('[name="edi_apprv2_scrap"]').val();
  var arrAprov2 = apprv_2.split(':');
  let App2 = "";
  if (arrAprov2.length == 2) {
    App2 = arrAprov2[1].trim();
  } else {
    App2 = $("#edi_apprv2_scrap").val();
  }
  var apprv_3 = $('[name="edi_apprv3_scrap"]').val();
  var arrAprov3 = apprv_3.split(':');
  let App3 = "";
  if (arrAprov3.length == 2) {
    App3 = arrAprov3[1].trim();
  } else {
    App3 = $("#edi_apprv3_scrap").val();
  }
  let stat1 = 0;
  let stat2 = 0;
  let stat3 = 0;
  if ($('#ckApp1').is(":checked")) {
    stat1 = 1;
  }
  if ($('#ckApp2').is(":checked")) {
    stat2 = 1;
  } if ($('#ckApp3').is(":checked")) {
    stat3 = 1;
  }
  //console.log("Stat1: " + stat1 +", "+ FApp1);
  var mgrBody = "Dear, Manager.";
  var hdrBody = "Dear, Approval.";
  var Body = "<br/>Part production scrap records <br/>create by <b>" + emp_code + "</b><br/>"
    + " Workorder: " + wo
    + " Itemno: " + item
    + " Operation: " + opr
    + "<br/>The document is ready for review as link <a href='" + urlApprove
    + "?search=" + wo + ":" + item + "&scrid=" + SCRID + "'>Workorder No: " + wo
    + "</a><br/>Best Regards,<br/>-Powered by IT  Department-";
  var Subject = "ขออนุมัติ Scrap, Operation: " + opr + ", WO: " + wo + ", Item No:" + item;

  if (stat1 != FApp1) {
    //# console.log("Send mail 1 to 2, 3");
    //# Start Send mail
    //Addresses = approve_email + ";sakulchai_p@berninathailand.com";
    if (MAIL_FLG == 1) {
      if (App1 != "") {
        var strBody = hdrBody + Body;
        var strSubj = hdrBody + Subject;
        sendEmailAsync(strBody, strSubj, App1);
      }
      if (App2 != "") {
        var strBody = hdrBody + Body;
        var strSubj = hdrBody + Subject;
        sendEmailAsync2(strBody, strSubj, App2);
      }
      if (App3 != "") {
        //sendEmailAsync3(mgrBody + Body, mgrBody + Subject, App3);
      }
      
    }
    //# End Send mail
  }
  //console.log("Stat2: " + stat2 + ", " + FApp2);
  if (stat2 != FApp2) {
    //# console.log("Send mail 3");
    if (App3 != "") {
      var strBody = hdrBody + Body;
      var strSubj = hdrBody + Subject;
      sendEmailAsync3(strBody, strSubj, App3);
    }
  }
  //console.log("Stat3: " + stat3 + ", " + FApp3);
  if (stat3 != FApp3) {
    //# console.log("Send mail 1, 2");
  }

  let obj = {
    ID: $("#ID_scr").html(),
    rec_date: $("#edi_rec_date").val(),
    prd_setup: $("#edi_prd_setup").val(),
    prd_tools: $("#edi_prd_tools").val(),
    prd_surf: $("#edi_prd_surf").val(),
    prd_dimout: $("#edi_prd_dimout").val(),
    prd_other: $("#edi_prd_other").val(),
    scrap_remark: $("#edi_scrap_remark").val(),
    ven_hardness: $("#edi_ven_hardness").val(),
    ven_dimout: $("#edi_ven_dimout").val(),
    ven_surf: $("#edi_ven_surf").val(),
    ven_other: $("#edi_ven_other").val(),
    vendor_remark: $("#edi_Supplier_remark").val(),
    other_remark: $("#other_remark").val(),
    app1_stat: stat1,
    app2_stat: stat2,
    app3_stat: stat3,
    app1_mail: App1,
    app2_mail: App2,
    app3_mail: App3
  }
  
  $.ajax({
    url: "/SGA/ConfirmScrapSaveEdit",
    type: "PUT",
    data: obj,
    success: function (response) {
      //-->
      //location.reload();
      //location.href("/SGA/PRDRecordSheet?search=" + wo + ":" + item);
      window.location.href = "/SGA/PRDRecordSheet?search=" + wo + ":" + item;
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
}
function ConfirmSave() {
  var opr = $("#mod_orp").html();
  var wo = $("#title_wo").html();
  var item = $("#det_itemno").html();
  var mas_qty = $("#mod_qty").html();
  var mas_stdtime = $("#mod_std").html();
  var mas_resource = $("#mod_workc").html();
  var mas_mc = $("#mod_mctime").html();
  var mas_lab = $("#mod_labtime").html();

  var emp_code = $("#emp_code").val();
  var create_dt = $("#create_dt").val();
  var rec_setup = $("#rec_setup").val();
  var rec_mc = $("#rec_mc").val();
  var rec_lab = $("#rec_lab").val();
  var rec_aqty = $("#rec_aqty").val();
  var rec_atotal = $("#rec_atotal").val();
  var rec_eff = $("#rec_eff").val();
  if (emp_code == "" || (rec_mc == 0 || rec_mc == NaN)) {
    alert("ข้อมูลไม่ครบ");
    return;
    }
    
  var obj = {
      mas_wo: wo,
      mas_itemno: item,
      mas_opr: opr,
      mas_qty: mas_qty,
      mas_stdtime: mas_stdtime, 
      mas_resource: mas_resource,
      mas_mc: mas_mc,
      mas_lab: mas_lab,
      emp_code: emp_code,
      rec_date: create_dt,
      rec_setup: rec_setup,
      rec_mc: rec_mc,
      rec_lab: rec_lab,
      rec_aqty: rec_aqty,
      rec_atotal: rec_atotal,
      rec_eff: rec_eff
  }
  console.log(obj); 
  if (!confirm("ยืนยันบันทึกข้อมูล Time Sheet")) return;
  //--> Controller AddInsTimeSheetRecord
  $.ajax({
    url: "/SGA/AddInsTimeSheetRecord",
    type: "POST",
    data: obj,
    success: function (response) {
      //console.log(response);
      alert("\n บันทึกข้อมูล เสร็จสมบูรณ์ \n");
      location.reload();
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
}
function ConfirmScrapSave() {
  var wo = $("#title_wo").html();
  var item = $("#det_itemno").html();
  var opr = $("#mod_orp_scr").html();
  var emp_code = $("#emp_code_scr").val();
  var urlApprove = $("#urlApprove").html();

  var apprv_1 = $('[name="apprv1_scrap"]').val();
  var arrAprov1 = apprv_1.split(':');
  let App1 = "";
  if (arrAprov1.length == 2) {
    App1 = arrAprov1[1].trim();
  }
  var apprv_2 = $('[name="apprv2_scrap"]').val();
  var arrAprov2 = apprv_2.split(':');
  let App2 = "";
  if (arrAprov2.length == 2) {
    App2 = arrAprov2[1].trim();
  }
  var apprv_3 = $('[name="apprv3_scrap"]').val();
  var arrAprov3 = apprv_3.split(':');
  let App3 = "";
  if (arrAprov3.length == 2) {
    App3 = arrAprov3[1].trim();
  }

  if (App1 == "" || $("#emp_code_scr").val() == "") return;
  if (!confirm("ยืนยันการบันทึก Scrap")) return;

  var obj = {
    mas_wo: wo,
    mas_itemno: item,
    mas_opr: opr, 
    emp_code: $("#emp_code_scr").val(),
    rec_date: $("#rec_date").val(),
    prd_setup: $("#prd_setup").val(),

    prd_tools: $("#prd_tools").val(),
    prd_surf: $("#prd_surf").val(),
    prd_dimout: $("#prd_dimout").val(),
    prd_other: $("#prd_other").val(),
    scrap_remark: $("#scrap_remark").val(),
    ven_hardness: $("#ven_hardness").val(),
    ven_dimout: $("#ven_dimout").val(),
    ven_surf: $("#ven_surf").val(),
    ven_other: $("#ven_other").val(),
    vendor_remark: $("#Supplier_remark").val(),
    other_remark: $("#add_other_remark").val(),
    app1_stat: 0,
    app2_stat: 0,
    app3_stat: 0,
    app1_mail: App1,
    app2_mail: App2,
    app3_mail: App3
  }
  console.log(obj);
  //--- Controller --> AddScrapTimeSheetRecord
  
  $.ajax({
    url: "/SGA/AddScrapTimeSheetRecord",
    type: "POST",
    data: obj,
    success: function (response) {
      console.log(response);
      // Send mail after success
      var mgrBody = "Dear, Manager.";
      var hdrBody = "Dear, Approval.";
      var Body = "<br/>Part production scrap records <br/>create by <b>" + emp_code + "</b><br/>"
        + "Workorder: " + wo
        + "Itemno: " + item
        + "Operation: " + opr
        + "<br/>The document is ready for review as link <a href='" + urlApprove
        + "?search=" + wo + ":" + item + "&scrid=" + response[2] + "'>Workorder No: " + wo
        + "</a><br/>Best Regards,<br/>-Powered by IT  Department-";
      var Subject = "ขออนุมัติ Scrap, Operation: " + opr + ", WO: " + wo + ", Item No:" + item;

      if (MAIL_FLG == 1) {
        if (App1 != "") {
          var strBody = hdrBody + Body;
          var strSubj = hdrBody + Subject;
          console.log(strBody);
          console.log(strSubj);
          sendEmailAsync(strBody, strSubj, App1);
        }
      }
      // /Send mail after success
      //location.reload();
      window.location.href = "/SGA/PRDRecordSheet?search=" + wo + ":" + item;
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
}

function sendEmailAsync(_Body, _Subject, _Addresses) {
  var url = "/SendMail/MailSenderMessage";
  $.post(url, { Body: _Body, Form: "", Subject: _Subject, Addresses: _Addresses }, function (data) {
    console.log(data);
  });
}
function sendEmailAsync2(_Body2, _Subject2, _Addresses2) {
  var url = "/SendMail/MailSenderMessage";
  $.post(url, { Body: _Body2, Form: "", Subject: _Subject2, Addresses: _Addresses2 }, function (data) {
    console.log(data);
  });
}
function sendEmailAsync3(_Body2, _Subject2, _Addresses2) {
  var url = "/SendMail/MailSenderMessage";
  $.post(url, { Body: _Body2, Form: "", Subject: _Subject2, Addresses: _Addresses2 }, function (data) {
    console.log(data);
  });
}

$("#btnBack").click(function () {
  var url = "/SendMail/PRDRecordSheetList";
  window.location.href = "/SGA/PRDRecordSheetList";
});

//$("#btnExport").click(function () {
//  await exportToExcel();
//});
document.getElementById("btnCloseDoc").addEventListener("click", async () => {
  var wo = $("#title_wo").html();
  var item = $("#det_itemno").html();

  var button = jQuery(document.getElementById("btnCloseDoc"));
  let txtBtnClose = $('#btnCloseDoc').text();
  let txtBtn = "";
  let staBtn = 0;
  if (txtBtnClose == "CLOSED") {
    $('#btnCloseDoc').text("OPEN");
    button.removeClass("btn btn-warning").addClass("btn btn-success");
    txtBtn = "เปิด";
    staBtn = 0;
  } else {
    $('#btnCloseDoc').text("CLOSED");
    button.removeClass("btn btn-success").addClass("btn btn-warning");
    txtBtn = "ปิด";
    staBtn = 1;
  }
  var obj = {
    ppWo: wo,
    itemParts: item,
    wo_stat: staBtn
  }
  if (confirm("ยืนยันการ " + txtBtn +" \n เอกสารรายการ : " + wo)) {
    //UpdWorkOrderPartsStat
    $.ajax({
      url: `/SGA/UpdWorkOrderPartsStat`,
      method: 'PUT',
      data: obj,
      success: function (response) {
        alert("บันทึกสำเร็จ");
        window.location.href = "/SGA/PRDRecordSheet?search=" + wo + ":" + item;
      }
    });
  }

});
document.getElementById("btnExport").addEventListener("click", async () => {
  if (confirm("ยืนยันการ Export ข้อมูล")) {
    await exportToExcel();
  }
});

function exportToExcel_bak() {
  document.getElementById('loading').style.display = 'block';

  const mas_wo = $("#title_wo").html();
  const mas_itemno = $("#det_itemno").html();

  const query = new URLSearchParams({
    mas_wo: mas_wo,
    mas_item: mas_itemno
  });
  //console.log(mas_wo + ", " + mas_item); return;
  fetch(`/SGA/GetExportTimeSheetData?${query.toString()}`)
    .then(response => response.json())
    .then(data => {
      const worksheet = XLSX.utils.json_to_sheet(data);
      const workbook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(workbook, worksheet, "ExportedData");
      XLSX.writeFile(workbook, "ExportedData.xlsx");
    })
    .catch(error => {
      console.error("Error exporting data:", error);
      alert("เกิดข้อผิดพลาดในการดึงข้อมูล");
    })
    .finally(() => {
      document.getElementById('loading').style.display = 'none';
    });
}

//-- ไม่แยก OPR
function exportToExcel_2() {
  const mas_wo = $("#title_wo").html();
  const mas_itemno = $("#det_itemno").html();

  const query = new URLSearchParams({
    mas_wo: mas_wo,
    mas_item: mas_itemno
  });

  fetch(`/SGA/GetExportTimeSheetData?${query.toString()}`)
    .then(response => response.json())
    .then(data => {
      if (data.length === 0) {
        alert("ไม่พบข้อมูล");
        return;
      }

      // กำหนดแถวส่วนหัวเอง
      const customHeaderRows = [
        [`Work Order: ${mas_wo}`],                // Row 1
        [`Item No: ${mas_itemno}`],               // Row 2
        [`Item Name: ${mas_itemno}`]     // Row 3
      ];

      // กำหนดชื่อคอลัมน์ Excel (Row 4)
      const headerRow = [
        "Work Order", "Item No", "Operation", "Quantity", "Efficiency", "Create Date"
      ];

      // แปลงข้อมูล JSON เป็น Array สำหรับ export
      const dataRows = data.map(row => [
        row.mas_wo,
        row.mas_itemno,
        row.mas_opr,
        row.mas_qty,
        row.rec_eff,
        row.create_dt
      ]);

      // ✅ คำนวณรวม mas_qty
      const totalQty = rows.reduce((sum, row) => sum + (parseFloat(row.mas_qty) || 0), 0);

      // ✅ แถว Total
      const totalRow = ["", "", "Total", totalQty, "", ""];

      // รวมทั้งหมด
      const finalSheetData = [
        ...customHeaderRows,
        headerRow,
        ...dataRows,
        totalRow  // ✅ ใส่ท้ายสุด
      ];

      // สร้าง worksheet
      const worksheet = XLSX.utils.aoa_to_sheet(finalSheetData);

      // สร้าง workbook
      const workbook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(workbook, worksheet, "FilteredData");

      // บันทึกเป็นไฟล์ Excel
      XLSX.writeFile(workbook, `Export_${mas_wo}_${mas_itemno}.xlsx`);
    })
    .catch(error => {
      console.error("Export error:", error);
      alert("เกิดข้อผิดพลาดในการ export ข้อมูล");
    });
}
//--- แยก Sheet ตาม OPR
async function exportToExcel() {
  const mas_wo = $("#title_wo").html();
  const mas_itemno = $("#det_itemno").html();

  const query = new URLSearchParams({
    mas_wo: mas_wo,
    mas_item: mas_itemno
  });

  //const query = new URLSearchParams({ mas_wo, mas_itemno });
  const response = await fetch(`/SGA/GetExportTimeSheetData?${query.toString()}`);

  if (!response.ok) {
    alert("Export failed.");
    return;
  }

  const result = await response.json();

  const itemName = result.item_name;  // ✅ ดึง item_name
  const data = result.data;           // ✅ ดึงข้อมูล

  console.log("Item Name:", itemName);
  console.log("Data:", data);

  // ตัวอย่างการนำ item_name ไปใช้ใน Excel header
  //const customHeader = [
  //  [`Work Order: ${mas_wo}`],
  //  [`Item No: ${mas_itemno}`],
  //  [`Item Name: ${itemName}`] // ✅ ใช้ที่นี่
  //];
  // กำหนดแถวส่วนหัวเอง
  const customHeaderRows = [
    [`Work Order: ${mas_wo}`],                // Row 1
    [`Item No: ${mas_itemno}`],               // Row 2
    [`Item Name: ${itemName}`]     // Row 3
  ];

  // กำหนดชื่อคอลัมน์ Excel (Row 4)
  const headerRow = [
    "Operation",
    "Quantity",
    "Std. Time",
    "Resource",
    "Mast. MC.",
    "Mast. Lab",
    "Emp Code",
    "Rec. Date",
    "[Setup",
    "Machine",
    "Lab",
    "aqty",
    "atotal]",
    "[Prd. setup",
    "Prd. tools",
    "Prd. surf",
    "Prd. dimout",
    "Prd. other]",
    "[Supp. hardness",
    "Supp. dimout",
    "Supp. surf",
    "Supp. other]",
    "eff",
    "Create Dt.",
    "Scrap Remark",
    "Supplier Remark",
    "Other Remark"
  ];

  // แปลงข้อมูล JSON เป็น Array สำหรับ export
  //const dataRows = data.map(row => [
  //  row.mas_wo,
  //  row.mas_itemno,
  //  row.mas_opr,
  //  row.mas_qty,
  //  row.rec_eff,
  //  row.create_dt
  //]);
  const dataRows = data.map(row => [
    row.mas_opr,
    row.mas_qty,
    row.mas_stdtime,
    row.mas_resource,
    row.mas_mc,
    row.mas_lab,
    row.emp_code,
    row.rec_date,
    row.rec_setup,
    row.rec_mc,
    row.rec_lab,
    row.rec_aqty,
    row.rec_atotal,
    row.prd_setup,
    row.prd_tools,
    row.prd_surf,
    row.prd_dimout,
    row.prd_other,
    row.ven_hardness,
    row.ven_dimout,
    row.ven_surf,
    row.ven_other,
    row.rec_eff,
    row.create_dt,
    row.scrap_remark,
    row.vendor_remark,
    row.other_remark
  ]);

  // * กรณี อยากใส่ท้ายเอกสาร
  /*
  // ✅ คำนวณรวม mas_qty
  const totalQty = dataRows.reduce((sum, row) => sum + (parseFloat(row.mas_qty) || 0), 0);
  // ✅ แถว Total
  const totalRow = ["", "", "Total", totalQty, "", ""];
  */
  // ✅ แถว Total
  const totalRow = ["", "", "Total", dataRows.length, "รายการ", ""];

  // รวมทั้งหมด
  const finalSheetData = [
    ...customHeaderRows,
    headerRow,
    ...dataRows,
    totalRow  // ✅ ใส่ท้ายสุด
  ];

  // สร้าง worksheet
  const worksheet = XLSX.utils.aoa_to_sheet(finalSheetData);

  // สร้าง workbook
  const workbook = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(workbook, worksheet, "FilteredData");

  // บันทึกเป็นไฟล์ Excel
  XLSX.writeFile(workbook, `Export_${mas_wo}_${mas_itemno}.xlsx`);

}

function exportToExcel_split_opr() {
  const mas_wo = $("#title_wo").html();
  const mas_itemno = $("#det_itemno").html();

  const query = new URLSearchParams({
    mas_wo: mas_wo,
    mas_item: mas_itemno
  });

    fetch(`/SGA/GetExportTimeSheetData?${query.toString()}`)
    .then(response => response.json())
    .then(data => {
      if (data.length === 0) {
        alert("ไม่พบข้อมูล");
        return;
      }

      // จัดกลุ่มข้อมูลตาม mas_opr
      const groupedData = {};
      data.forEach(row => {
        const opr = row.mas_opr || "No_Operation";
        if (!groupedData[opr]) groupedData[opr] = [];
        groupedData[opr].push(row);
      });

      // สร้าง workbook
      const workbook = XLSX.utils.book_new();

      // สร้างแต่ละ sheet ตามกลุ่ม mas_opr
      for (const opr in groupedData) {
        const rows = groupedData[opr];

        // แถวพิเศษก่อนตาราง
        const customHeader = [
          [`Work Order: ${mas_wo}`],
          [`Item No: ${mas_itemno}`],
          [`Operation: ${opr}`]
        ];

        // Header columns
        const headerRow = [
          "Work Order", "Item No", "Operation", "Quantity", "Efficiency", "Create Date"
        ];

        const dataRows = rows.map(row => [
          row.mas_wo,
          row.mas_itemno,
          row.mas_opr,
          row.mas_qty,
          row.rec_eff,
          row.create_dt
        ]);

        const finalSheetData = [
          ...customHeader,
          headerRow,
          ...dataRows
        ];

        const worksheet = XLSX.utils.aoa_to_sheet(finalSheetData);
        XLSX.utils.book_append_sheet(workbook, worksheet, opr);
      }

      // บันทึกไฟล์
      XLSX.writeFile(workbook, `Export_By_Operation_${mas_wo}.xlsx`);
    })
    .catch(error => {
      console.error("Export error:", error);
      alert("เกิดข้อผิดพลาดในการ export ข้อมูล");
    });
}

function SubmitTime(trid, oprstat) {
  var wo = $("#title_wo").html();
  var item = $("#det_itemno").html();
  let newStat = 0;
  if (oprstat == 0) {
    newStat = 1;
    if (!confirm('Confirm to *Send time sheet, ยืนยันการส่ง Time sheet?')) return;
  } else if (oprstat == 1) {
    newStat = 0;
    if (!confirm('Confirm to *Re-Send time sheet, ยืนยันการ ดึงข้อมูลการส่ง Time sheet กลับ?')) return;
  }
  //alert(trid);
  // ส่งค่าทั้งหมด
  let obj = {
    ID: trid,
    opr_stat: newStat,
    ppWo: wo,
    itemParts: item
  };

  $.ajax({
    url: `/SGA/UPDTimeSheetOprStat`,
    method: 'PUT',
    data: obj,
    success: function (response) {
      alert("บันทึกสำเร็จ");
      window.location.href = "/SGA/PRDRecordSheet?search=" + wo + ":" + item;
    }
  });

}
