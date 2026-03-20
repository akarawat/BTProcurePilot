let item_count;
const flagProcureEmailSent = false; // ตัวแปรสถานะการส่งอีเมลไปยังฝ่ายจัดซื้อ 
$(document).ready(function () {
  $('#approx_dt').on('change', function () {
    let selectedDate = new Date($(this).val());
    let today = new Date();

    // ล้างเวลาออก (ให้เทียบเฉพาะวันที่)
    selectedDate.setHours(0, 0, 0, 0);
    today.setHours(0, 0, 0, 0);

    let diffTime = selectedDate - today;
    let diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24)); // คำนวณความต่างเป็นวัน
    if (diffDays <= -1) {
      alert('Fail date select');
      $('#approx_dt').val(null);
      return;
    }
    if (diffDays <= 3) {
      $('#rdoApprox').prop('checked', true);
    } else {
      $('#rdoApprox2').prop('checked', true);
    }
  });

  $('#id_supp1').on('input', function () {
    var inputVal = $(this).val();
    var displayText = "";
    $('#opt_supp1 option').each(function () {
      if ($(this).val() === inputVal) {
        displayText = $(this).text(); // ✅ เอา text display
      }
    });
    $("#name_supp1").val(displayText);
    let arrCurrency = displayText.split(' ');
    //console.log(arrCurrency);
    if (arrCurrency.length != 0) {
      let curr = arrCurrency[0].replace('[','').replace(']','');
      console.log(curr);
      $("#prcurrency").val(curr);
      $("#lblCurency").text(arrCurrency[0]);
    }
    //console.log("Value:", inputVal);
    //console.log("Display Text:", displayText);
  });
  $('#id_supp2').on('input', function () {
    var inputVal = $(this).val();
    var displayText = "";
    $('#opt_supp2 option').each(function () {
      if ($(this).val() === inputVal) {
        displayText = $(this).text(); // ✅ เอา text display
      }
    });
    $("#name_supp2").val(displayText);
    //console.log("Value:", inputVal);
    //console.log("Display Text:", displayText);
  });
  $('#id_supp3').on('input', function () {
    var inputVal = $(this).val();
    var displayText = "";
    $('#opt_supp3 option').each(function () {
      if ($(this).val() === inputVal) {
        displayText = $(this).text(); // ✅ เอา text display
      }
    });
    $("#name_supp3").val(displayText);
    //console.log("Value:", inputVal);
    //console.log("Display Text:", displayText);
  });

  // ### Start List all of Approval 
  $('#id_appEmp').on('input', function () {
    var inputVal = $(this).val();
    if (inputVal != "") {
      var displayText = "";
      $('#opt_appEmp option').each(function () {
        if ($(this).val() === inputVal) {
          displayText = $(this).text(); // ✅ เอา text display
        }
      });
      $("#lblAppEmpMail").html(inputVal.split(':')[1]);
      $("#lblAppEmpDisp").html(displayText);
    }
  });
  $('#id_appEmp2').on('input', function () {
    var inputVal = $(this).val();
    if (inputVal != "") {
      var displayText = "";
      $('#opt_appEmp2 option').each(function () {
        if ($(this).val() === inputVal) {
          displayText = $(this).text(); // ✅ เอา text display
        }
      });
      $("#lblAppEmp2Mail").html(inputVal.split(':')[1]);
      $("#lblAppEmp2Disp").html(displayText);
    }
  });
  $('#id_countEmp').on('input', function () {
    var inputVal = $(this).val();
    if (inputVal != "") {
      var displayText = "";
      $('#opt_countEmp option').each(function () {
        if ($(this).val() === inputVal) {
          displayText = $(this).text(); // ✅ เอา text display
        }
      });
      $("#lblCountEmpMail").html(inputVal.split(':')[1]);
      $("#lblCountEmpDisp").html(displayText);
    }
  });
  $('#id_authEmp').on('input', function () {
    var inputVal = $(this).val();
    if (inputVal != "") {
      var displayText = "";
      $('#opt_authEmp option').each(function () {
        if ($(this).val() === inputVal) {
          displayText = $(this).text(); // ✅ เอา text display
        }
      });
      $("#lblAuthEmpMail").html(inputVal.split(':')[1]);
      $("#lblAuthEmpDisp").html(displayText);
    }
  });
  // End List name Approval
  
});
function SavePRHeader(flag) {
  let SumTotal = $("#spnTotal").html();
  if ($("#prno_draft").val() == '' || $("#reason").val() == '') {
    alert("❌ Plese fill PR information");
    return;
  }
  //if ($("#id_supp1").val() == '') {
  //  alert("❌ Plese fill Supplier information");
  //  return;
  //}
  console.log("Date= " + $("#approx_dt").val());
  if ($("#approx_dt").val() == '') {
    alert("❌ Plese select Date require (กรุณาเลือกวันที่)");
    return;
  }
  if (flag == 1 && (SumTotal == '' || SumTotal == undefined)) {
    alert("❌ Plese Add PR items detail");
    return;
  }

  if (flag == 0 && !confirm("Confirm to PR Save draft?")) return;

  let stsAppEmp = $("#id_appEmp").prop('disabled');
  let stsAppEmp2 = $("#id_appEmp2").prop('disabled');
  let stsCountEmp = $("#id_countEmp").prop('disabled');
  let stsAuthEmp = $("#id_authEmp").prop('disabled');
  //let valAppEmp = $("#lblAppEmpMail").val();
  //let valCountEmp = $("#lblCountEmpMail").val();
  //let valAuthEmp = $("#lblAuthEmpMail").val();

  let valAppEmp = $("#id_appEmp").val();
  let valAppEmp2 = $("#id_appEmp2").val();
  let valCountEmp = $("#id_countEmp").val();
  let valAuthEmp = $("#id_authEmp").val();
  //console.log("App Val: ", valAppEmp, ':', valAppEmp2, ':', valCountEmp, ':', valAuthEmp);
  //ตรวจสอบการเลือก Approval ถ้ามีการเลือก Approval จาก dropdown
  let empApp = ""; // $("#lblAppEmpDisp").html();
  let empApp2 = ""; 
  let empCount = $("#lblCountEmpDisp").html();
  let empAuth = $("#lblAuthEmpDisp").html();

  let mailApp = ""; // $("#lblAppEmpMail").html();
  let mailApp2 = "";
  let mailCount = $("#lblCountEmpMail").html();
  let mailAuth = $("#lblAuthEmpMail").html();
  //console.log(valAppEmp.length, ':', valAppEmp.trim());
  //-->if (flag == 1) {
    //console.log('Test Status:', stsAppEmp, ':', stsCountEmp, ':', stsAuthEmp);
    //console.log('Test Val:', valAppEmp, ':', valCountEmp, ':', valAuthEmp);
    //console.log('Test Split:', valAppEmp.split(':').length);
    if (valAppEmp != undefined) {
      // เช็คก่อนว่ามีข้อมูลใน valAppEmp หรือไม่
      console.log(valAppEmp);
      if (valAppEmp.split(':').length == 2) {
        empApp = valAppEmp.split(':', 2)[0];
        mailApp = valAppEmp.split(':', 2)[1];
        console.log("Con 11:", valAppEmp, ', ', empApp, ', ', mailApp);
      } else {
        empApp = valAppEmp.trim();
        mailApp = $("#lblAppEmpMail").html();
        console.log("Con 12:", valAppEmp, ', ', empApp, ', ', mailApp);
      }
      if (valAppEmp2.split(':').length == 2) {
        empApp2 = valAppEmp2.split(':', 2)[0];
        mailApp2 = valAppEmp2.split(':', 2)[1];
        console.log("Con 112:", valAppEmp2, ', ', empApp2, ', ', mailApp2);
      } else {
        empApp2 = valAppEmp2.trim();
        mailApp2 = $("#lblAppEmp2Mail").html();
        console.log("Con 122:", valAppEmp2, ', ', empApp2, ', ', mailApp2);
      }

      if (!stsAppEmp && mailApp == "") {
        alert("❌ Please select approval"); return;
      }
    }

    if (valCountEmp != undefined) {
      if (valCountEmp.split(':').length == 2) {
        empCount = valCountEmp.split(':', 2)[0];
        mailCount = valCountEmp.split(':', 2)[1];
        console.log("Con 21:", valCountEmp, ', ', empCount, ', ', mailCount);
      } else {
        empCount = valCountEmp.trim();
        mailCount = $("#lblCountEmpMail").html();
        console.log("Con 22:", valCountEmp, ', ', empCount, ', ', mailCount);
      }
      if (!stsCountEmp && mailCount == "") {
        alert("❌ Please select Manager approval"); return;
      }
    }
    if (valAuthEmp != undefined) {
      if (valAuthEmp.split(':').length == 2) {
        empAuth = valAuthEmp.split(':', 2)[0];
        mailAuth = valAuthEmp.split(':', 2)[1];
        console.log("Con 31:", valAuthEmp, ', ', empAuth, ', ', mailAuth);
      } else {
        empAuth = valAuthEmp.trim();
        mailAuth = $("#lblAuthEmpMail").html();
        console.log("Con 32:", valAuthEmp, ', ', empAuth, ', ', mailAuth);
      }
      if (!stsAuthEmp && mailAuth == "") {
        alert("❌ Plese select MD approval"); return;
      }
    }

    //if (!stsCountEmp && valCountEmp.split(':').length < 2) {
    //  alert("❌ Plese select Manager approval"); return;
    //}
    //if (!stsAuthEmp && valAuthEmp.split(':').length < 2) {
    //  alert("❌ Plese select MD approval"); return;
    //}

  //-->}

  //if (!stsAppEmp && valAppEmp.split(':').length < 2) {
  //  alert("❌ Plese select approval"); return;
  //}
  //if (valCountEmp != null) {
  //  empCount = valCountEmp.split(':', 2)[0];
  //  mailCount = valCountEmp.split(':', 2)[1];
  //}
  //if (valAuthEmp != null) {
  //  empAuth = valAuthEmp.split(':', 2)[0];
  //  mailAuth = valAuthEmp.split(':', 2)[1];
  //}
  console.log("Emp Req", $("#empCode").val());
  console.log("Mail App:", mailApp, ':', mailCount, ':', mailAuth);
  console.log("Final App:", empApp, ':', empCount, ':', empAuth);
  let rfq = $("input[name='rdoPurpose']:checked").val();
  console.log(rfq);
  if (empApp != undefined) {
    if (empApp.trim() == $("#empCode").val().trim()) {
      if (empApp.trim() == 'S03434' || empApp.trim() == 'S01371') {
        
      } else {
        if (rfq == 1 || rfq == 2) {
          alert("❌ You cannot select yourself as approver. [101] \n ไม่อนุญาตให้ ผู้ขอและผู้อนุมัติเป็นคนเดียวกัน");
        return;
        }        
      }
    }
  }
  if (empCount != undefined) {
    if (empCount.trim() == $("#empCode").val().trim()) {
      if (empCount.trim() == 'S03434' || empCount.trim() == 'S01371') {

      } else {
        if (rfq == 1 || rfq == 2) {
          alert("❌ You cannot select yourself as approver. [102] \n ไม่อนุญาตให้ ผู้ขอและผู้อนุมัติเป็นคนเดียวกัน");
        return;
        }
      }
    }
  }
  if (empAuth != undefined) {
    if (empAuth.trim() == $("#empCode").val().trim()) {
      if (empAuth.trim() == 'S03434' || empAuth.trim() == 'S01371') {

      } else {
        if (rfq == 1 || rfq == 2) {
          alert("❌ You cannot select yourself as approver. [103] \n ไม่อนุญาตให้ ผู้ขอและผู้อนุมัติเป็นคนเดียวกัน");
        return;
        }
      }
    }
  }
  if (item_count == 0) {
    alert("❌ Plaese Add Items Detail require. \n เพิ่มรายการที่ต้องการและรายละเอียดอย่างน้อย 1 รายการ");
    return;
  }

  if (flag == 1 && !confirm("Confirm to PR Submit?")) {
    return;
  }
  
  let objForm = {
    prno: $("#prno_draft").val(),
    projectno: $("#id_projno").val(),
    empcode: $("#empCode").val(), // สมมติว่ามี field
    approx_type: $("input[name='rdoApprox']:checked").val(),
    approx_dt: $("#approx_dt").val(),
    invcreditno: $("#InvNo").val(),
    purpose_type: $("input[name='rdoPurpose']:checked").val(),
    ref_docs: $("input[name='reference']").val(),
    pr_reason: $("#reason").val(),
    pr_recvdt: '',
    pr_recvpono: '',
    attach_flag: '00000',
    reqDepCode: $("#reqDepCode").val(), // สมมติว่ามี field
    reqDate: '',
    reqFlag: flag,
    appEmp: empApp,
    //appDate: null,
    appFlag: ($('#appFlag').val() === undefined) ? 0 : $('#appFlag').val(),
    appEmp2: empApp2,
    //appDate: null,
    appFlag2: ($('#appFlag2').val() === undefined) ? 0 : $('#appFlag2').val(),

    countEmp: empCount,
    //countDate: null,
    countFlag: ($('#countFlag').val() === undefined) ? 0 : $('#countFlag').val(),
    authEmp: empAuth,
    //authDate: null,
    authFlag: ($('#authFlag').val() === undefined) ? 0 : $('#authFlag').val(),
    prstatus: flag,
    pub_remark: $("#pub_remark").val(),
    prcurrency: $("#prcurrency").val(),

    id_supp1: $("#id_supp1").val(),
    name_supp1: $("#name_supp1").val(),
    vc_supp1: $("#vc_supp1").val(),
    contact_supp1: $("#contact_supp1").val(),
    email_supp1: $("#email_supp1").val(),
    tel_supp1: $("#tel_supp1").val(),
    remark_supp1: $("#remark_supp1").val(),
    quoref_supp1: $("#quoref_supp1").val(),
    refnodt_supp1: $("#refnodt_supp1").val(),
  
    id_supp2: $("#id_supp2").val(),
    name_supp2: $("#name_supp2").val(),
    vc_supp2: $("#vc_supp2").val(),
    contact_supp2: $("#contact_supp2").val(),
    email_supp2: $("#email_supp2").val(),
    tel_supp2: $("#tel_supp2").val(),
    remark_supp2: $("#remark_supp2").val(),
    quoref_supp2: $("#quoref_supp2").val(),
    refnodt_supp2: $("#refnodt_supp2").val(),
  
    id_supp3: $("#id_supp3").val(),
    name_supp3: $("#name_supp3").val(),
    vc_supp3: $("#vc_supp3").val(),
    contact_supp3: $("#contact_supp3").val(),
    email_supp3: $("#email_supp3").val(),
    tel_supp3: $("#tel_supp3").val(),
    remark_supp3: $("#remark_supp3").val(),
    quoref_supp3: $("#quoref_supp3").val(),
    refnodt_supp3: $("#refnodt_supp3").val(),

    id_supp4: $("#id_supp4").val(),
    name_supp4: $("#name_supp4").val(),
    vc_supp4: $("#vc_supp4").val(),
    contact_supp4: $("#contact_supp4").val(),
    email_supp4: $("#email_supp4").val(),
    tel_supp4: $("#tel_supp4").val(),
    remark_supp4: $("#remark_supp4").val(),
    quoref_supp4: $("#quoref_supp4").val(),
    refnodt_supp4: $("#refnodt_supp4").val(),

    id_supp5: $("#id_supp5").val(),
    name_supp5: $("#name_supp5").val(),
    vc_supp5: $("#vc_supp5").val(),
    contact_supp5: $("#contact_supp5").val(),
    email_supp5: $("#email_supp5").val(),
    tel_supp5: $("#tel_supp5").val(),
    remark_supp5: $("#remark_supp5").val(),
    quoref_supp5: $("#quoref_supp5").val(),
    refnodt_supp5: $("#refnodt_supp5").val()

  };
  //console.log(objForm);
  //return;

  let mailApprove = $("#lblAppEmpMail").html();
  let mailApprove2 = $("#lblAppEmp2Mail").html();

  let emp_code = $("#empCode").val();
  let emp_name = $("#lblRequester").text();
  let proj_code = $("#id_projno").val();
  let urlApprove = $("#urlApprove").val();
  let sMAILGROUPADMIN = $('#sMAILGROUPADMIN').val();

  $.ajax({
    url: "/CTLAdmin/SaveNewPRHeader",
    type: "POST",
    data: objForm,
    success: function (response) {
      console.log(flag);
      console.log(response);
      if (response[0] == "0") {
        //alert("✔ Save Success full PRNo: " + response[2]);
        var prno = response[2];
        let combindAppMail = "";
        // Verify first time Or Revise All process
        // ### Importance Check for Approval 2 person
        if (flag == 1 && (mailApprove != "" && mailApprove2 != "")) {
          combindAppMail = mailApprove + ";" + mailApprove2;
        }

        // ### Importance Check for Approval 1 person only
        if (flag == 1 && (mailApprove != "" && mailApprove2 == "")) {
          combindAppMail = mailApprove;          
        }

        // #### Send Email to Approval
        if (flag == 1 && $('#appFlag').val() == 0) {
          // Send Email to Approval
          console.log("Send Email to Approval");
          var hdrSubject = "PR Request. Dear, Approval. PR No: " + prno;
          var Body = "<br/>PR Online, Requeste create by <b>" + emp_code + ": " + emp_name + "</b><br/>"
            + " <br/> "
            + " Project No: <b>" + proj_code + "</b>"
            + " <font color='green'>PR Submited</font> "
            + "<br/>The document is ready for review as link <a href='" + urlApprove
            + "?docno=" + prno + "'>PR No: " + prno
            + "</a><br/>Best Regards,<br/>[101] Powered by IT  Department-";
          //console.info(Body);
          //console.log(Body);
          sendEmailAsync(Body, hdrSubject, mailApprove);
          saveEmailLog("Log mail " + prno + "", mailApprove, "Send 101");
          //-->alert("✔ PR Sunmit Successfully, Pendding to approve");
          //setTimeout(redirectToPage(prno), 3000);

          updateCodeLog("101");
          setTimeout(redirectToIndex(), 3000);
        }
        //return;
        // If Submit Second time skip to Procurement
        if (flag == 1 && $('#appFlag').val() > 0) {
          // Resend Email to Procurement
          console.log("Resend Email to Procurement");
          var hdrSubject = "PR Resend. Dear, Procurement. " + prno;
          var Body = "<br/>PR Online, Requeste create by <b>" + emp_code + ": " + emp_name + "</b><br/>"
            + " <br/> "
            + " <font color='blue'>PR Resend.</font> "
            + "<br/>The document is ready for review as link <a href='" + urlApprove
            + "?docno=" + prno + "'>Click here PR No: " + prno
            + " <<< </a><br/>Best Regards,<br/>[102] Powered by IT  Department-";
          updateCodeLog("102");
          if (flagProcureEmailSent) {
            sendEmailAsync(Body, hdrSubject, sMAILGROUPADMIN);
            saveEmailLog("Log mail " + prno + "", sMAILGROUPADMIN, "Send 102");
          }
          //-->alert("✔ PR Resend Successfully, Email send to Procurement");
          setTimeout(redirectToIndex(), 3000);
        }

        //else {
        if (flag == 0) {
          updateCodeLogDraftPr("100", prno);
          window.location.href = "/CTLAdmin/PRResultRequest?docno=" + prno;
        }
        //}
      }

    },
    error: function (request, status, error) {
      alert("⛔ Server error: " + error);
    }
  });
}
function redirectToPage(prno) {
  window.location.href = "/CTLAdmin/PRApproval?docno=" + prno;
}
function redirectToIndex() {
  window.location.href = "/";
}

$("#btnAdd").click(function () {
  let PRrdoPurpose = parseInt($("input[name='rdoPurpose']:checked").val());
  console.log(PRrdoPurpose);

  if ($("#item_btnumber").val() == '' ||
    $("#item_descript").val() == '' ||
    //$("#item_model").val() == '' ||
    $("#item_acccode").val() == '' ||
    $("#item_costdep").val() == '' 
  ) { alert('⛔ Error Item Information, * Please add detail or - in require column. \n ระบุรายละเอียด รายการที่ต้องการ *ถ้าไม่มีให้ใส่ - \n\n (*Require, ช่องที่จะต้องใส่: BT No., Description/Spec, ACC Code, Cost Dept, Quantity, Unit)'); return; }

  let uqty = $("#item_qty").val();
  let uprice = $("#item_unitprice").val();
  //if (uqty == '' || uprice == '') { alert('⛔ Error Quantity unit price.'); return; }
  if (PRrdoPurpose == 1 || PRrdoPurpose == 2) {
    if (uqty == 0 || uprice == 0) { alert('⛔ Error Quantity unit price.'); return; }
  }

  if (!confirm('Confirm to save Item detail.')) { return; }
  const item = {
    item_btnumber: $("#item_btnumber").val(),
    item_descript: $("#item_descript").val(),
    item_model: $("#item_model").val(),
    item_acccode: $("#item_acccode").val(),
    item_costdep: $("#item_costdep").val(),
    item_qty: parseFloat($("#item_qty").val()),
    item_unit: $("#item_unit").val(),
    item_unitprice: $("#item_unitprice").val(),
    item_amount: parseFloat($("#item_amount").val()),
    prno: $("#prno").val()
  };

  $.ajax({
    url: '/CTLAdmin/AddPRItemDetail',
    type: 'POST',
    data: item,
    success: function (response) {
      if (response[0] === "0") {
        alert("✔ Item Added Successfully: " + response[2]);
        loadPRItemTable($("#prno").val()); // โหลดใหม่

        $("#item_btnumber").val('');
        $("#item_descript").val('');
        $("#item_model").val('');
        $("#item_acccode").val('');
        $("#item_costdep").val('');

        $("#item_qty").val('');
        $("#item_unit").val('');
        $("#item_unitprice").val('');
        $("#item_amount").val('');


      } else {
        alert("❌ Insert failed: " + response[1]);
      }
    },
    error: function (xhr, status, err) {
      console.error(err);
      alert("⛔ Error adding item");
    }
  });
});
function CalTotalAmountMaster() {
  let uqty = $("#item_qty").val();
  let uprice = $("#item_unitprice").val();
  let disc = $("#item_disc").val();
  if (disc == '') { disc = 0; }
  //if (uqty == '' || uprice == '') { alert('⛔ Error Quantity unit price.'); return; }
  //if (uqty == 0 || uprice == 0) { alert('⛔ Error Quantity unit price.'); return; }
  if (uqty == '' || uprice == '') { $("#item_amount").val(0); return; }
  if (uqty == 0 || uprice == 0) { $("#item_amount").val(0); return; }
  let cal_total = (uqty * uprice).toFixed(2);
  $("#item_amount").val((cal_total - disc));
  
}
function CalTotalAmountModal() {
  let uqty = $("#edit_item_qty").val();
  let uprice = $("#edit_item_unitprice").val();
  let disc = $("#edit_item_disc").val();
  if (disc == '') { disc = 0; }
  console.log(disc);
  //if (uqty == '' || uprice == '') { alert('⛔ Error Quantity unit price.'); return; }
  //if (uqty == 0 || uprice == 0) { alert('⛔ Error Quantity unit price.'); return; }
  if (uqty == '' || uprice == '') { $("#edit_item_amount").val(0); return; }
  if (uqty == 0 || uprice == 0) { $("#edit_item_amount").val(0); return; }

  let cal_total = (uqty * uprice).toFixed(2);
  $("#edit_item_amount").val((cal_total - disc));
  
}
function sendEmailAsync(_Body, _Subject, _Addresses) {
  var url = "/SendMail/MailSenderMessage";
  $.post(url, { Body: _Body, Form: "", Subject: _Subject, Addresses: _Addresses }, function (data) {
    console.log(_Addresses);
  });
}
function updateCodeLog(codelog) {
  let prno = $("#prno").val();
  let item = {
    prno: prno,
    codelog: codelog
  };
  $.ajax({
    url: '/CTLAdmin/UpdatePRCodeLog',
    type: 'PUT',
    data: item,
    success: function (response) { },
    error: function (xhr, status, err) { console.error(err); }
  });
}

function updateCodeLogDraftPr(codelog, prno) {
  let item = {
    prno: prno,
    codelog: codelog
  };
  $.ajax({
    url: '/CTLAdmin/UpdatePRCodeLog',
    type: 'PUT',
    data: item,
    success: function (response) { },
    error: function (xhr, status, err) { console.error(err); }
  });
}

