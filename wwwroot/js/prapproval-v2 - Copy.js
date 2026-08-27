const flagProcureEmailSent = false; 
function setDateInput(inputId, dateValue) {
  if (dateValue) {
    $('#' + inputId).val(dateValue.split('T')[0]);
  } else {
    $('#' + inputId).val('');
  }
}
function formatNumber(value, fix) {
  if (isNaN(value)) return '';

  const rounded = parseFloat(value).toFixed(fix);
  const parts = rounded.split(".");
  const integerPart = parseInt(parts[0]).toLocaleString();
  const decimalPart = parts[1];

  return `${integerPart}.${decimalPart}`;
}
function saveEmailLog(mailFrom, mailTo, mailSubject) {
  $.ajax({
    url: '/CTLAdmin/SaveEmailLog',
    type: 'PUT',
    contentType: 'application/json',
    data: JSON.stringify({
      mailFrom: mailFrom,
      mailTo: mailTo,
      subject: mailSubject
    }),
    success: function (res) {
      console.log("Email log saved.");
    },
    error: function (err) {
      console.error("Log error:", err);
    }
  });
}
$(document).ready(function () {
  // อ่านค่าจาก URL เช่น ?docno=PR25680723001
  const urlParams = new URLSearchParams(window.location.search);
  const docno = urlParams.get('docno');
  var sMAILGROUPADMIN;
  let strQuoReturn = "";
  if (docno) {

    //let UserDepCode = $("#reqDepCode").val();
    //if (UserDepCode == 310) {
    //  document.getElementById("DivAppEmp2").style.display = "block";
    //} else {
    //  document.getElementById("DivAppEmp2").style.display = "none";
    //}
    document.getElementById("DivAppEmp2").style.display = "block";

    $.ajax({
      url: '/CTLAdmin/GetPRByDocNo?docno=' + encodeURIComponent(docno),
      type: 'GET',
      success: function (response) {
        console.log(response);
        if (response.id == null) {
          location.href = "/";
        }
        let rfq_prefix = "";
        let proj_prefix = "";
        loadFileAttachTable(response.id);

        $("#prid").val(response.id);
        $("#ref_prid").val(response.id);

        $("#prno").val(response.prno);
        $("#prno_draft").val(response.prno);
        $("#id_projno").val(response.projectno);
        $("#id_projno_lbl").html(response.projectname);
        if (response.projectno.length >= 3) {
          proj_prefix = "(P)";
        }
        if (response.approx_dt != null) {
          setDateInput('approx_dt', response.approx_dt.split('T')[0]);
        }

        if (response.approx_type == 3) {
          console.log("3");
          $("#rdoApprox").prop("checked", true);
        } else if (response.approx_type == 1) {
          console.log("1");
          $("#rdoApprox2").prop("checked", true);
        }

        $("#InvNo").val(response.invcreditno);
        if (response.purpose_type == 1) {
          $("#rdoPurpose1").prop("checked", true);
          SupPurpose(1);
        } else if (response.purpose_type == 2) {
          $("#rdoPurpose2").prop("checked", true);
          SupPurpose(2);
        } else if (response.purpose_type == 3) {
          $("#rdoPurpose3").prop("checked", true);
          rfq_prefix = "(RFQ)";
          SupPurpose(3);
        }

        if (response.quo_return == 1) {
          strQuoReturn = " (Procure send qoutation)";
          rdoQuoRtn1.checked = true;
        }

        $("#reference").val(response.ref_docs);
        $("#reason").val(response.pr_reason);
        setDateInput('prReceivedDate', response.pr_recvdt);
        $("#poNumber").val(response.pr_recvpono);

        $("#prno_preix").val(rfq_prefix + proj_prefix + response.prno);

        let lblReqDate = moment(response.reqDate).format('DD-MM-YYYY');
        $("#lblReqDate").html(lblReqDate);
        $("#lblRequester").html(response.empcode_txt);

        $("#lblReqStatus").html(response.prstatus_txt + " " + strQuoReturn);

        $("#lblLastupdate").html(response.update_dt_txt);
        $("#lblRevisionNo").html(response.revision_no);
        $("#pub_remark").text(response.pub_remark);

        $('#prcurrency').html(response.prcurrency);

        $('#lblReqMail').val(response.reqEmail);

        $('#lblAppEmpMail').val(response.appEmail);
        $('#lblAppEmp2Mail').val(response.appEmail2);

        $('#lblCountEmpMail').val(response.CountEmail);
        $('#lblAuthEmpMail').val(response.authEmail);

        $('#lblAppEmpDisp').text(response.appEmp_txt);
        $('#lblAppEmpDate').text(response.appDate_txt);

        $('#lblAppEmp2Disp').text(response.appEmp2_txt);
        $('#lblAppEmp2Date').text(response.appDate2_txt);

        $('#lblCountEmpDisp').text(response.countEmp_txt);
        $('#lblCountEmpDate').text(response.countDate_txt);
        $('#lblAuthEmpDisp').text(response.authEmp_txt);
        $('#lblAuthEmpDate').text(response.authDate_txt);

        $('#approve_step').val(response.approve_step);

        $('#appFlag').val(response.appFlag);
        $('#appFlag2').val(response.appFlag2);
        $('#countFlag').val(response.countFlag);
        $('#authFlag').val(response.authFlag);

        $('#lblRemarkEmp').html(response.remarkEmp);
        $('#lblRemarkEmp2').html(response.remarkEmp2);
        $('#lblRemarkCount').html(response.remarkCount);
        $('#lblRemarkAuth').html(response.remarkAuth);

        if (response.appFlag == 1) { $("#rdo1AppEmp").prop('checked', true); } else if (response.appFlag == 9) { $("#rdo9AppEmp").prop('checked', true); }
        if (response.appFlag2 == 1) { $("#rdo1AppEmp2").prop('checked', true); } else if (response.appFlag2 == 9) { $("#rdo9AppEmp2").prop('checked', true); }

        if (response.countFlag == 1) { $("#rdo1CountEmp").prop('checked', true); } else if (response.countFlag == 9) { $("#rdo9CountEmp").prop('checked', true); }
        if (response.authFlag == 1) { $("#rdo1AuthEmp").prop('checked', true); } else if (response.authFlag == 9) { $("#rdo9AuthEmp").prop('checked', true); }
        if (response.procure_flag == 1) {
          $("#rdoProcApp1").prop('checked', true);
          $("#lblReqStatus").html("Recieve PR." + " " + strQuoReturn);
        }
        else if (response.procure_flag == 9) {
          $("#rdoProcApp9").prop('checked', true);
          $("#lblReqStatus").html("Reject PR. All approval");
        }
        else if (response.procure_flag == 7) {
          $("#rdoProcApp7").prop('checked', true);
          $("#lblReqStatus").html("Reject PR. To Requester");
        }
        if (response.pr_recvpono != '') {
          $("#lblReqStatus").html("Procure PO Completed.");
        }

        $("#procRemark").val(response.procure_remark);
        let lblReceivedDate = moment(response.pr_recvdt).format('DD-MM-YYYY');
        $("#prReceivedDate").html(lblReceivedDate);

        let loginRole = $('#sUROLEADMIN').val();
        let loginEmpCode = $('#empCode').val();
        console.log("Login: ", response.empcode, ":", loginEmpCode);

        if (loginEmpCode == response.empcode || loginRole == 1
          || loginEmpCode == response.appEmp
          || loginEmpCode == response.countEmp
          || loginEmpCode == response.authEmp) {
          //console.log("Remarks");
          document.getElementById("divHisRemark").style.display = "block";
        } else {
          //console.log("None");
          document.getElementById("divHisRemark").style.display = "none";
        }

        loadSuggestedVendor(docno, 1);
        loadSuggestedVendor(docno, 2);
        loadSuggestedVendor(docno, 3);
        loadSuggestedVendor(docno, 4);
        loadSuggestedVendor(docno, 5);
        loadPRItemTable(docno);
        setVisiblePr();
        setIndividualPR(response.empcode, response.prstatus);
        setVisibleProcureAdmin();
        setVisibleEmail();
        setVisibleResubmit(response);
        loadApprovalSignatures(response);
        loadPRHisRemark(docno);
        sMAILGROUPADMIN = $('#sMAILGROUPADMIN').val();

        if (response.prstatus == 10) {
          console.log("prstatus: ", response.prstatus);
          GUIPRCompleted();
        }

        if ((response.approve_step == 35 || response.approve_step == 25 || response.approve_step == 15)
          && (response.flagm_proc == 0 && response.procure_flag == 0 && sMAILGROUPADMIN != undefined)) {
          const obj = {
            prno: $("#prno").val(),
            flagm_proc: 1
          }
          console.log("Send mail to procurement: ", obj);
          console.log(obj);

          $.ajax({
            url: '/CTLAdmin/UpdateProcureMailFlag',
            type: 'PUT',
            data: obj,
            success: function (response) {
              let id_projno = $("#id_projno").val();
              let lblRequester = $("#lblRequester").text();
              let urlApprove = $("#urlApprove").val();
              let prno = $("#prno").val();

              var hdrSubject = "PR Approved. Dear, Procurement. " + prno;
              var Body = "<br/>PR Online, Requeste create by <b>" + lblRequester + "</b><br/>"
                + " <br/> "
                + " <font color='green'>PR Approved</font> "
                + "<br/>The document is ready for review as link <a href='" + urlApprove
                + "?docno=" + prno + "'>Click here PR No: " + prno
                + " <<< </a><br/>Best Regards,<br/>-[201] Powered by IT  Department-";
              updateCodeLog("201");
              if (flagProcureEmailSent) {
                sendEmailAsync(Body, hdrSubject, sMAILGROUPADMIN);
                saveEmailLog("Log mail " + prno + "", sMAILGROUPADMIN, "Send 201");
              }
              if (response[0] === "0") {
                //-->alert("✔ PR Approval successfully and send Email to procurement. (121)");
              }
            }
          });

          //alert('Sen mail to Procurement');
        }

      },
      error: function () {
        alert("Error loading PR document");
      }
    });

  } else {
    alert("❌ ไม่พบ parameter docno ใน URL");
  }

  document.getElementById("approve_step").style.display = "none";
  document.getElementById("sUROLEADMIN").style.display = "none";
  document.getElementById("sMAILGROUPADMIN").style.display = "none";
});

function GUIPRCompleted() {

  //$("#rdo1AppEmp").prop('disabled', true);
  //$("#rdo1CountEmp").prop('disabled', true);
  //$("#rdo1AuthEmp").prop('disabled', true);
  //$("#rdo9AppEmp").prop('disabled', true);
  //$("#rdo9CountEmp").prop('disabled', true);
  //$("#rdo9AuthEmp").prop('disabled', true);

  $("#btnOwnerCancel").prop('disabled', true);
  $("#btnSaveRemark").prop('disabled', true);

  $("#myForm :input").prop("disabled", true);
  $("#myForm :textarea").prop("disabled", true);
  $("#btnAddHisRemark").prop("disabled", true);
  $("#hisremark").prop("disabled", true);
}

$("#btnProcApprove").click(function () {
  let reqEmail = $('#lblReqMail').val();
  let appEmail = $('#lblAppEmpMail').val();
  let countEmail = $('#lblCountEmpMail').val();
  let authEmail = $('#lblAuthEmpDisp').val();
  let sMAILGROUPADMIN = $('#sMAILGROUPADMIN').val();
  // Close Email for Management P'Ju Req
  //let concatEmail = sMAILGROUPADMIN + ';' + reqEmail + ';' + appEmail + ';' + countEmail + ';' + authEmail;
  let concatEmail = "";
  if (flagProcureEmailSent) {
    concatEmail = sMAILGROUPADMIN + ';' + reqEmail;
  } else {
    concatEmail = reqEmail;
  }
  

  let proc_flag = $("input[name='rdoProcApp']:checked").val();
  let poNumber = $('#poNumber').val();
  let prReceivedDate = $('#prReceivedDate').val();
  let procRemark = $('#procRemark').val();
  console.log(reqEmail, ':', proc_flag, ':', poNumber, ':', prReceivedDate, ':', procRemark);
  if (reqEmail == null) return;
  if (proc_flag == undefined) {
    alert("❌ Please select Recieve status"); return;
  }
  //if (poNumber == '') {
  //  alert("❌ Please select PO No."); return;
  //}
  //if (prReceivedDate == '') {
  //  alert("❌ Please select PR Receive Date."); return;
  //}
  if (procRemark == '') {
    if (!confirm('❔✔ Confirm to without remark?')) return;
  }

  if (!confirm('✔ Confirm to Update procurement information❔ \n')) return;
  //console.log('After Conf: ', reqEmail, ':', proc_flag, ':', poNumber, ':', prReceivedDate, ':', procRemark);
  const obj = {
    prno: $("#prno").val(),
    procure_flag: proc_flag,
    pr_recvdt: prReceivedDate,
    pr_recvpono: poNumber,
    procure_remark: procRemark
  }
  console.log(obj);

  $.ajax({
    url: '/CTLAdmin/UpdateProcStatus',
    type: 'PUT',
    data: obj,
    success: function (response) {

      var id_projno = $("#id_projno").val();
      var lblRequester = $("#lblRequester").text();
      var urlApprove = $("#urlApprove").val();
      var urlResult = $("#urlResult").val();
      var prno = $("#prno").val();
      var hdrSubject = "";
      var bodyConcat = "";
      var procRemark = $("#procRemark").val();

      if (proc_flag == 1) {
        var msgPo = "";
        if (poNumber != "") {
          msgPo = "Procurement PO. No.";
        } else {
          msgPo = "Procurement PR. Received";
        }
        hdrSubject = "PR Success. " + msgPo + ". " + poNumber;
        bodyConcat = "<font color='green'>Procurement PR. Received. " + poNumber + "</font>";
        var Body = "<br/>Attention, PR Online Requeste create by <b>" + lblRequester + "</b><br/>"
          + " <br/> "
          + bodyConcat
          + " <u>" + procRemark + "</u> "
          + "<br/>The document is ready for review as link <a href='" + urlApprove
          + "?docno=" + prno + "'>Click here PR No: " + prno
          + " <<< </a><br/>Best Regards,<br/>-[202] Powered by IT  Department-";
        updateCodeLog("202");
        sendEmailAsync(Body, hdrSubject, concatEmail);
        saveEmailLog("Log mail " + prno + "", concatEmail, "Send 202");

        if (response[0] === "0") {
          //-->alert("✔ PR Online send Email completed. (208)");
        }
        window.location.href = "/";
      } else {
        hdrSubject = "PR Reject. PR NO. " + prno;
        bodyConcat = "<font color='red'>Procurement reject PR No. " + prno + "</font>";
        var Body = "<br/>Attention, PR Online Requeste create by <b>" + lblRequester + "</b><br/>"
          + " <br/> "
          + bodyConcat
          + "<br/>The document is ready for review as link <a href='" + urlResult
          + "?docno=" + prno + "'>Click here PR No: " + prno
          + " <<< </a><br/>Best Regards,<br/>-[203] Powered by IT  Department-";
        updateCodeLog("203");
        sendEmailAsync(Body, hdrSubject, concatEmail);
        saveEmailLog("Log mail " + prno + "", concatEmail, "Send 203");
        if (response[0] === "0") {
          //-->alert("✔ PR Online send Email completed. (222)");
        }
        window.location.href = "/";
      }

    }
  });


})
function setVisibleProcureAdmin() {
  let roleAdmin = $("#sUROLEADMIN").val();
  //console.log(roleAdmin);
  //alert(roleAdmin);
  const divProcControl = document.getElementById('divProcControl');
  const adminUpload = document.getElementById('adminUpload');
  if (roleAdmin == 1) {
    $("#prReceivedDate").prop('disabled', false);
    $("#poNumber").prop('disabled', false);
    $("#rdoProcApp1").prop('disabled', false);
    $("#rdoProcApp9").prop('disabled', false);
    $("#rdoProcApp7").prop('disabled', false);
    $("#procRemark").prop('disabled', false);
    $("#btnProcApprove").prop('disabled', false);

    $("#rdoQuoRtn0").prop('disabled', false);
    $("#rdoQuoRtn1").prop('disabled', false);

    divProcControl.style.display = 'block';
    adminUpload.style.display = 'block';
  } else {
    divProcControl.style.display = 'none';
    adminUpload.style.display = 'none';
  }
}
function setVisibleEmail() {
  let LoginEmpEmail = $("#LoginEmpEmail").val();
  let mailApprove = $("#lblAppEmpMail").val();
  let mailApprove2 = $("#lblAppEmp2Mail").val();
  let CountEmpMail = $("#lblCountEmpMail").val();

  if (LoginEmpEmail != mailApprove) {
    $("#rdo1AppEmp").prop('disabled', true);
    $("#rdo9AppEmp").prop('disabled', true);
  }
  if (LoginEmpEmail != mailApprove2) {
    $("#rdo1AppEmp2").prop('disabled', true);
    $("#rdo9AppEmp2").prop('disabled', true);
  }
  if (LoginEmpEmail != CountEmpMail) {
    $("#rdo1CountEmp").prop('disabled', true);
    $("#rdo9CountEmp").prop('disabled', true);
  }

}
function setVisibleResubmit(response) {
  let loginEmpCode = $("#empCode").val();
  // Scoped to Procurement reject only — this feature is Procurement (Reject) + Requester (Resend), no Approver involvement.
  let isRejected = response.procure_flag == 7 || response.procure_flag == 9;

  if (loginEmpCode == response.empcode && isRejected) {
    document.getElementById("divBtnEditResubmit").style.display = "block";
  } else {
    document.getElementById("divBtnEditResubmit").style.display = "none";
  }
}
function loadSignature(empcode, imgId) {
  if (!empcode) return;
  $.ajax({
    url: '/CTLAdmin/GetSignatureImage',
    type: 'GET',
    data: { empcode: empcode },
    success: function (response) {
      if (response && response.image) {
        $('#' + imgId).attr('src', 'data:image/png;base64,' + response.image).show();
      }
    }
  });
}
function loadApprovalSignatures(response) {
  // Requester "signs" by submitting; show once the PR has left draft status.
  if (response.prstatus >= 1) {
    loadSignature(response.empcode, 'imgSigRequester');
  }
  if (response.appFlag == 1) {
    loadSignature(response.appEmp, 'imgSigAppEmp');
  }
  if (response.appFlag2 == 1) {
    loadSignature(response.appEmp2, 'imgSigAppEmp2');
  }
  if (response.countFlag == 1) {
    loadSignature(response.countEmp, 'imgSigCountEmp');
  }
  if (response.authFlag == 1) {
    loadSignature(response.authEmp, 'imgSigAuthEmp');
  }
}
$("#btnEditResubmit").click(function () {
  $("#rdoApprox").prop('disabled', false);
  $("#rdoApprox2").prop('disabled', false);
  $("#approx_dt").prop('disabled', false);
  $("#InvNo").prop('disabled', false);
  $("#rdoPurpose1").prop('disabled', false);
  $("#rdoPurpose2").prop('disabled', false);
  $("#rdoPurpose3").prop('disabled', false);
  $("#reference").prop('disabled', false);
  $("#reason").prop('disabled', false);

  document.getElementById("divBtnEditResubmit").style.display = "none";
  document.getElementById("divBtnResubmitSave").style.display = "block";

  loadPRItemTable($("#prno").val(), true);
});
function editItem(id) {
  $.ajax({
    url: '/CTLAdmin/GetPRItemDetailById?id=' + id,
    type: 'GET',
    success: function (item) {
      $("#edit_id").val(item.id);
      $("#edit_item_btnumber").val(item.item_btnumber);
      $("#edit_item_descript").val(item.item_descript);
      $("#edit_item_model").val(item.item_model);
      $("#edit_item_acccode").val(item.item_acccode);
      $("#edit_item_costdep").val(item.item_costdep);
      $("#edit_item_qty").val(item.item_qty);
      $("#edit_item_unit").val(item.item_unit);
      $("#edit_item_unitprice").val(item.item_unitprice);
      $("#edit_item_amount").val(item.item_amount);
      $("#edit_item_disc").val(item.item_disc);
      new bootstrap.Modal(document.getElementById('editItemModal')).show();
    }
  });
}
function saveEditItem() {
  if (!confirm('❔ Confirm to save Item detail.')) { return; }
  const obj = {
    id: $("#edit_id").val(),
    item_btnumber: $("#edit_item_btnumber").val(),
    item_descript: $("#edit_item_descript").val(),
    item_model: $("#edit_item_model").val(),
    item_acccode: $("#edit_item_acccode").val(),
    item_costdep: $("#edit_item_costdep").val(),
    item_qty: parseFloat($("#edit_item_qty").val()),
    item_unit: $("#edit_item_unit").val(),
    item_unitprice: $("#edit_item_unitprice").val(),
    item_amount: parseFloat($("#edit_item_amount").val()),
    item_disc: parseFloat($("#edit_item_disc").val())
  };

  $.ajax({
    url: '/CTLAdmin/UpdatePRItemDetail',
    type: 'POST',
    data: obj,
    success: function (res) {
      if (res[0] === "0") {
        $('#editItemModal').modal('hide');
        loadPRItemTable($("#prno").val(), true);
      } else {
        alert("❌ Update Failed: " + res[1]);
      }
    }
  });
}
function deleteItem(id) {
  if (confirm("❌ Are you sure to delete this item?")) {
    $.ajax({
      url: '/CTLAdmin/DeletePRItemDetailById',
      type: 'POST',
      data: { id: id },
      success: function (response) {
        if (response[0] === "0") {
          loadPRItemTable($("#prno").val(), true);
        } else {
          alert("Delete failed");
        }
      }
    });
  }
}
function CalTotalAmountModal() {
  let uqty = $("#edit_item_qty").val();
  let uprice = $("#edit_item_unitprice").val();
  let disc = $("#edit_item_disc").val();
  if (disc == '') { disc = 0; }
  if (uqty == '' || uprice == '') { $("#edit_item_amount").val(0); return; }
  if (uqty == 0 || uprice == 0) { $("#edit_item_amount").val(0); return; }

  let cal_total = (uqty * uprice).toFixed(2);
  $("#edit_item_amount").val((cal_total - disc));
}
$("#btnResubmitSave").click(function () {
  if (!confirm('❔ Confirm to Resubmit this PR to Procurement?')) return;
  const obj = {
    prno: $("#prno").val(),
    approx_type: $("input[name='rdoApprox']:checked").val(),
    approx_dt: $("#approx_dt").val(),
    invcreditno: $("#InvNo").val(),
    purpose_type: $("input[name='rdoPurpose']:checked").val(),
    ref_docs: $("#reference").val(),
    pr_reason: $("#reason").val()
  }
  $.ajax({
    url: '/CTLAdmin/ResubmitPR',
    type: 'PUT',
    data: obj,
    success: function (response) {
      if (response[0] === "0") {
        let prno = $("#prno").val();
        window.location.href = "?docno=" + prno + "&fullscreen=false";
      } else {
        alert("Resubmit failed");
      }
    }
  });
});
function setIndividualPR(empCode, prstatus) {
  const myButton = document.getElementById('divBtnOwnerCancel');
  //myButton.style.display = 'none';

  if (prstatus == 7) {
    //$("#rdo1AppEmp").prop('disabled', true);
    //$("#rdo1CountEmp").prop('disabled', true);
    //$("#rdo1AuthEmp").prop('disabled', true);
    //$("#rdo9AppEmp").prop('disabled', true);
    //$("#rdo9CountEmp").prop('disabled', true);
    //$("#rdo9AuthEmp").prop('disabled', true);

    $("#btnOwnerCancel").prop('disabled', true);
    $("#btnSaveRemark").prop('disabled', true);

    $("#myForm :input").prop("disabled", true);
    $("#myForm :textarea").prop("disabled", true);
  } else {
    if ($("#empCode").val() == empCode) {
      console.log('True: ', $("#empCode").val(), ':', empCode);
      myButton.style.display = 'block';
    } else {
      console.log('False: ', $("#empCode").val(), ':', empCode);
      myButton.style.display = 'none';
    }
  }

}

// Toggle full screen
document.getElementById("toggleFullscreenBtn").addEventListener("click", function () {
  const urlParams = new URLSearchParams(window.location.search);
  const docno = urlParams.get('docno');
  console.log(docno);
  const isFull = localStorage.getItem("isFullscreen") === "true";
  localStorage.setItem("isFullscreen", (!isFull).toString());
  // Reload เพื่อใช้ Razor Layout ใหม่
  window.location.href = "?docno=" + docno + "&fullscreen=" + isFull;

});
function loadSuggestedVendor(docno, suggItem) {
  const divSuppCard = document.getElementById('card-supp' + suggItem);
  $.ajax({
    url: '/CTLAdmin/GetSuppByDocno',
    type: 'GET',
    data: {
      docno: docno,
      sugg_item: suggItem
    },
    success: function (data) {
      //console.log("Suggested Vendors:", data);
      //console.log("Data length:", data.length);

      if (data.length != 0) {
        let vendor = data[0]; // ถ้าแสดงแค่รายเดียว
        $('#sucgid' + suggItem).val(vendor.id);
        $('#id_supp' + suggItem).val(vendor.vencode);

        // console.log("Suggested Name:", vendor.name_supp);
        if (vendor.name_supp.length == 0) {
          divSuppCard.style.display = 'none';
        } else {
          divSuppCard.style.display = 'block';
        }
        $('#name_supp' + suggItem).val(vendor.name_supp);

        $('#vc_supp' + suggItem).val(vendor.venvc);
        $('#contact_supp' + suggItem).val(vendor.vencontact);
        $('#email_supp' + suggItem).val(vendor.venemail);
        $('#tel_supp' + suggItem).val(vendor.ventelfax);
        $('#remark_supp' + suggItem).val(vendor.venremark);
        $('#quoref_supp' + suggItem).val(vendor.quoref_supp);

        //if (suggItem == 1 && vendor.currency != "-") {
        //  $('#prcurrency').val(vendor.currency);
        //}

        $('#refnodt_supp' + suggItem).val(vendor.refnodt_supp?.split('T')[0]);
      } else {
        //console.log("Suggested Vendors:", data.length);
        if (data.length == 0) {
          divSuppCard.style.display = 'none';
        }
        //console.log("No data found for supplier " + suggItem);
      }
    },
    error: function () {
      alert("❌ Error loading suggested vendor");
    }
  });
}
function loadPRItemTable(docno, editable) {
  $.ajax({
    url: '/CTLAdmin/GetPRItemDetailByDocno?prno=' + docno,
    type: 'GET',
    success: function (data) {
      let html = "";
      let currency = $("#lblCurency").text();
      //console.log(currency);
      let sumTotal = 0, spnDisc = 0;
      $.each(data, function (i, row) {
        spnDisc += row.item_disc;
        sumTotal += row.item_amount;
        let actionCell = editable ? `<td>
              <i class="ri-edit-box-fill text-warning" onclick="editItem('${row.id}')" style="cursor:pointer; font-size: 24px;" title='Edit'></i>
              <i class="ri-delete-bin-2-fill text-danger" onclick="deleteItem('${row.id}')" style="cursor:pointer; font-size: 24px;" title='Delete'></i>
          </td>` : `<td></td>`;
        html += `<tr>
          <td>${row.item_btnumber}</td>
          <td>${row.item_descript}</td>
          <td>${row.item_model}</td>
          <td>${row.item_acccode}</td>
          <td>${row.item_costdep}</td>
          <td align='right'>${row.item_qty}</td>
          <td align='right'>${row.item_unit}</td>
          <td>${row.item_unitprice} ` + currency + `</td>
          <td align='right'>${row.item_disc}</td>
          <td align='right'>${row.item_amount}</td>
          ${actionCell}
        </tr>`;
      });
      $("#spnDisc").html(formatNumber(spnDisc, 2));
      $("#spnTotal").html(currency + ' ' + formatNumber(sumTotal, 2));
      $("#bindDataTable tbody").html(html);
      //id_appEmp
      //id_countEmp
      //id_authEmp

      let prcurrency = $("#prcurrency").val();
      if (prcurrency == "USD" ||
        prcurrency == "EUR" ||
        prcurrency == "CHF") {
        if (sumTotal <= 1999) {
          $("#id_appEmp").prop('disabled', false);
          $("#id_countEmp").prop('disabled', false);
          $("#id_authEmp").prop('disabled', true);
        } else if (sumTotal >= 2000) {
          $("#id_appEmp").prop('disabled', false);
          $("#id_countEmp").prop('disabled', false);
          $("#id_authEmp").prop('disabled', false);

        }
      } else {
        if (sumTotal <= 4999) {
          $("#id_appEmp").prop('disabled', false);
          $("#id_countEmp").prop('disabled', false);
          $("#id_authEmp").prop('disabled', true);
        } else if (sumTotal >= 5000 && sumTotal <= 59999) {
          $("#id_appEmp").prop('disabled', false);
          $("#id_countEmp").prop('disabled', false);
          $("#id_authEmp").prop('disabled', true);
        } else if (sumTotal >= 60000) {
          $("#id_appEmp").prop('disabled', false);
          $("#id_countEmp").prop('disabled', false);
          $("#id_authEmp").prop('disabled', false);
        }
      }

    }
  });
}
//function loadFileAttachTable(prid) {
//  $.ajax({
//    url: '/CTLAdmin/loadFileAttachTable?prid=' + prid,
//    type: 'GET',
//    success: function (data) {
//      let html = "";
//      $.each(data, function (i, row) {
//        html += `<tr>
//          <td>${i+1}. ${row.filename}</td>
//          <td>${row.filetype_txt}</td>
//          <td><a href='${row.filepath}' target='_blank'><i class="ri-inbox-archive-fill text-info" style="font-size: 24px;"></i></a></td>
//        </tr>`;
//      });

//      $("#tblFiles tbody").html(html);
//    }
//  });
//}
function loadFileAttachTable(prid) {
  let loginRole = $('#sUROLEADMIN').val();
  $.ajax({
    url: '/CTLAdmin/loadFileAttachTable?prid=' + prid,
    type: 'GET',
    success: function (data) {
      let html = "";
      $.each(data, function (i, row) {

        if (loginRole == 1) {
          html += `<tr>
          <td>${i + 1}. ${row.filename}</td>
          <td>${row.filetype_txt}</td>
          <td><a href='${row.filepath}' target='_blank'><i class="ri-inbox-archive-fill text-info" style="font-size: 24px;"></i></a></td>
          <td>
              <i class="ri-delete-bin-2-fill text-danger" onclick="deleteFileItem('${row.id}', '${row.filepath}')" style="cursor:pointer; font-size: 24px;" title='Delete'></i>
          </td>
        </tr>`;
        } else {
          html += `<tr>
          <td>${i + 1}. ${row.filename}</td>
          <td>${row.filetype_txt}</td>
          <td><a href='${row.filepath}' target='_blank'><i class="ri-inbox-archive-fill text-info" style="font-size: 24px;"></i></a></td>
          <td></td>
        </tr>`;
        }
      });

      $("#tblFiles tbody").html(html);
    }
  });
}
function deleteFileItem_Bak(id, filepath) {
  if (confirm("❌ Are you sure to delete this file item?")) {
    $.ajax({
      url: '/CTLAdmin/DeleteFileItemById',
      type: 'POST',
      data: { id: id },
      success: function (response) {
        if (response[0] === "0") {
          //-->alert("✔ Item deleted successfully");
          const ref_prid = $("#ref_prid").val();
          loadFileAttachTable(ref_prid);
        } else {
          alert("Delete failed");
        }
      }
    });
  }
}
function deleteFileItem(id, filepath) {
  if (!id) {
    alert("ไม่พบรหัสไฟล์ (id).");
    return;
  }
  if (!filepath) {
    alert("ไม่พบพาธไฟล์ (filepath).");
    return;
  }

  if (!confirm("❌ Are you sure to delete this file item?")) return;

  // ปิดปุ่มทุกปุ่มลบชั่วคราว (กันคลิกซ้ำ)
  const $buttons = $(".btn-delete-file");
  $buttons.prop("disabled", true);

  // (ถ้ามี Anti-forgery token)
  const token = $('input[name="__RequestVerificationToken"]').val();

  $.ajax({
    url: '/CTLAdmin/DeleteFileItemById',
    type: 'POST',
    data: {
      id: id,
      filepath: encodeURIComponent(filepath) // ปลอดภัยขึ้น
    },
    headers: token ? { 'RequestVerificationToken': token } : {},
    success: function (response) {
      if (response[0] === "0") {
        // สำเร็จ
        const ref_prid = $("#ref_prid").val();
        loadFileAttachTable(ref_prid); // reload ตารางไฟล์แนบ
      } else {
        alert("Delete failed: " + (response[1] || ""));
      }
    },
    error: function (xhr, status, err) {
      alert("⛔ Error deleting file: " + err);
    },
    complete: function () {
      $buttons.prop("disabled", false);
    }
  });
}

function ClearSupplierData(sucgid) {
  let id_supp1 = $("#sucgid" + sucgid).val();
  if (id_supp1 == '') return;
  if (!confirm('❌ Confirm sugges supplier clear information?')) return;
  $.ajax({
    url: '/CTLAdmin/DeleteSuggSupplierById',
    type: 'POST',
    data: { id: id_supp1 },
    success: function (response) {
      if (response[0] === "0") {
        //-->alert("✔ Suggess supplier deleted successfully");
        let prno = $("#prno").val();
        window.location.href = "/CTLAdmin/PRResultRequest?docno=" + prno;
      } else {
        alert("Delete failed");
      }
    }
  });
}
// Function Upload file
$(function () {
  const MAX_SIZE = 2 * 1024 * 1024; // 2 MB

  $("#btnUpload").on("click", function () {
    forceUpload();
  });

  window.forceUpload = function () {
    const fileInput = $("#file")[0];
    const file = fileInput.files && fileInput.files[0];
    const ref_prid = $("#ref_prid").val();
    const filetype = $("#filetype").val();

    if (!file) {
      $("#uploadResult").text("กรุณาเลือกไฟล์ก่อนอัปโหลด");
      return;
    }
    if (file.size > MAX_SIZE) {
      $("#uploadResult").text("ไฟล์มีขนาดเกิน 2 MB");
      return;
    }
    if (!ref_prid) {
      $("#uploadResult").text("ไม่พบรหัสอ้างอิงเอกสาร (ref_prid)");
      return;
    }

    if (!confirm("❔ Confirm to upload file?")) return;
    const formData = new FormData();
    formData.append("file", file);
    formData.append("ref_prid", ref_prid);
    formData.append("filetype", filetype);

    $.ajax({
      url: "/CTLAdmin/UploadAttachment",
      type: "POST",
      processData: false,
      contentType: false,
      data: formData,
      success: function (res) {
        if (res.code === 0) {
          $("#uploadResult").text("อัปโหลดสำเร็จ: " + res.filename);
          let prno = $("#prno").val();
          window.location.href = "/CTLAdmin/PRApproval?docno=" + prno;
          $("#file").val(""); // reset
        } else {
          $("#uploadResult").text("อัปโหลดไม่สำเร็จ: " + res.message);
        }
      },
      error: function (xhr, status, err) {
        $("#uploadResult").text("เกิดข้อผิดพลาดในการอัปโหลด: " + err);
      }
    });
  };
});
// End Function Upload file

function SupPurpose(suppur) {
  console.log(suppur);
  if (suppur == 3) {
    document.getElementById("card-supp1").style.display = "block";
    document.getElementById("card-supp2").style.display = "block";
    document.getElementById("card-supp3").style.display = "block";
    document.getElementById("card-supp4").style.display = "block";
    document.getElementById("card-supp5").style.display = "block";
  }
  else {
    console.log(suppur);
    document.getElementById("card-supp1").style.display = "block";
    document.getElementById("card-supp2").style.display = "none";
    document.getElementById("card-supp3").style.display = "none";
    document.getElementById("card-supp4").style.display = "none";
    document.getElementById("card-supp5").style.display = "none";
  }

}
function setVisiblePr() {
  let prstep = $('#approve_step').val();
  let appFlag = $('#appFlag').val();
  let countFlag = $('#countFlag').val();
  let authFlag = $('#authFlag').val();

  console.log("approve_step : " + prstep);
  console.log($("#lblAppEmpMail").val() + ":" + $("#LoginEmpEmail").val());

  //if (($("#lblAppEmpMail").val().toUpperCase() == $("#LoginEmpEmail").val().toUpperCase()) && prstep >= 11) {
  if ($("#lblAppEmpMail").val().toUpperCase() == $("#LoginEmpEmail").val().toUpperCase()) {
    console.log("Section Head Approval");
    $("#rdo1AppEmp").prop('disabled', false);
    $("#rdo9AppEmp").prop('disabled', false);
  } else {
    $("#rdo1AppEmp").prop('disabled', true);
    $("#rdo9AppEmp").prop('disabled', true);
  }
  console.log($("#lblCountEmpMail").val().toUpperCase() + ", " + $("#LoginEmpEmail").val().toUpperCase() + ", " + prstep + ", " + appFlag);
  /* Debug
  if ($("#lblAppEmp2Mail").val().toUpperCase() == $("#LoginEmpEmail").val().toUpperCase()) {
    console.log("Section Head Approval");
    $("#rdo1AppEmp2").prop('disabled', false);
    $("#rdo9AppEmp2").prop('disabled', false);
  } else {
    $("#rdo1AppEmp2").prop('disabled', true);
    $("#rdo9AppEmp2").prop('disabled', true);
  }
  */

  //if (($("#lblCountEmpMail").val().toUpperCase() == $("#LoginEmpEmail").val().toUpperCase()) && (prstep >= 22 || prstep == 0) && appFlag == 1) {
  if ($("#lblCountEmpMail").val().toUpperCase() == $("#LoginEmpEmail").val().toUpperCase()) {
    console.log("MGR Approval");
    $("#rdo1CountEmp").prop('disabled', false);
    $("#rdo9CountEmp").prop('disabled', false);
  } else {
    $("#rdo1CountEmp").prop('disabled', true);
    $("#rdo9CountEmp").prop('disabled', true);
  }

  console.log($("#lblAuthEmpMail").val().toUpperCase() + ", " + $("#LoginEmpEmail").val().toUpperCase() + ", " + prstep + ", " + appFlag);
  //if (($("#lblAuthEmpMail").val().toUpperCase() == $("#LoginEmpEmail").val().toUpperCase()) && prstep >= 31 && (appFlag == 1 && countFlag == 1)) {
  if ($("#lblAuthEmpMail").val().toUpperCase() == $("#LoginEmpEmail").val().toUpperCase()) {
    console.log("MD Approval");
    $("#rdo1AuthEmp").prop('disabled', false);
    $("#rdo9AuthEmp").prop('disabled', false);
  } else {
    $("#rdo1AuthEmp").prop('disabled', true);
    $("#rdo9AuthEmp").prop('disabled', true);
  }


}
function setPrStatus(role, status) {
  $("#personStatus").val(0);
  $("#approvalRole").val(role);
  $("#approvalStatus").val(status);
  if (status == 1) {
    $("#lblConfirmWarning").text("Confirm to Approve❔").addClass("text-success");
  } else if (status == 9) {
    $("#lblConfirmWarning").text("Confirm to Reject❔").addClass("text-warning");
  }
  let role_status = role + '' + status;
  if (role_status == '11' || role_status == '19') {
    $("#approval_remark").val($("#lblRemarkEmp").html());
  }
  if (role_status == '21' || role_status == '29') {
    $("#approval_remark").val($("#lblRemarkCount").html());
  }
  if (role_status == '31' || role_status == '39') {
    $("#approval_remark").val($("#lblRemarkAuth").html());
  }
  //console.log("PR Set :", role_status); return;
  new bootstrap.Modal(document.getElementById('modalPrStatus')).show();
}

function setPrStatus2App(role, status, person) {
  $("#person").val(person);
  // 88
  $("#personStatus").val(88);
  $("#approvalRole").val(role);
  $("#approvalStatus").val(status);
  // ### Importance Check for Approval 2 person
  let remark = $("#lblRemarkEmp").html();
  let remark2 = $("#lblRemarkEmp2").html();
  let mailApprove = $("#lblAppEmpMail").val();
  let mailApprove2 = $("#lblAppEmp2Mail").val();
  console.log(remark + ":" + remark2);
  if (mailApprove != "" && mailApprove2 != "") {
    if (person == 1) {
      $("#person").val("1");
      $("#approval_remark").val(remark);
    } else if (person == 2) {
      $("#person").val("2");
      $("#approval_remark").val(remark2);
    }
  }
  $("#lblConfirmWarning").text("Confirm to Approve❔").addClass("text-success");
  new bootstrap.Modal(document.getElementById('modalPrStatus')).show();
}
$("#btnApproveClose").click(function () {
  let role_status = $("#approvalRole").val() + '' + $("#approvalStatus").val();
  //console.log("PR Set :", role_status);
  if (role_status == '11' || role_status == '19') {
    $("#rdo1AppEmp").prop('checked', false);
    $("#rdo9AppEmp").prop('checked', false);

    $("#rdo1AppEmp2").prop('checked', false);
    $("#rdo9AppEmp2").prop('checked', false);
  }
  if (role_status == '21' || role_status == '29') {
    $("#rdo1CountEmp").prop('checked', false);
    $("#rdo9CountEmp").prop('checked', false);
  }
  if (role_status == '31' || role_status == '39') {
    $("#rdo1AuthEmp").prop('checked', false);
    $("#rdo9AuthEmp").prop('checked', false);
  }

});
function setPrStatusReject(role, status, person) {
  let remark = $("#lblRemarkEmp").html();
  let remark2 = $("#lblRemarkEmp2").html();
  console.log(remark + ":" + remark2);
  if (person == 1) {
    console.log("A");
    $("#person").val("1");
    $("#approval_remarkMulti").val(remark);
  } else if (person == 2) {
    console.log("B");
    $("#person").val("2");
    $("#approval_remarkMulti").val(remark2);
  }
  //setPrStatus(role, status); //modalPrStatusMulti
  $("#lblConfirmWarningMulti").text("Confirm to Reject❔").addClass("text-warning");
  new bootstrap.Modal(document.getElementById('modalPrStatusMulti')).show();
}
$("#btnPrApprovalRejMulti").click(function () {
  let person = $("#person").val();
  const obj2 = {
    prno: $("#prno").val(),
    person: person,
    approval_remark: $("#approval_remarkMulti").val()
  }
  $.ajax({
    url: '/CTLAdmin/UpdatePRMultiSectionheadReject',
    type: 'PUT',
    data: obj2,
    success: function (response) {
      window.location.href = "/";
    }
  });
});

$("#btnPrApproval").click(function () {
  // ### Importance check 2 Approval before update
  let person = $("#person").val();
  let personStatus = $("#personStatus").val();
  let mailApprove = $("#lblAppEmpMail").val();
  let mailApprove2 = $("#lblAppEmp2Mail").val();
  let nextApproval = true;
  let doubleApproval = false;
  let app_role = '';
  let app_status = '';
  let approval_remark = '';
  console.log(mailApprove + ", " + mailApprove2);
  if (mailApprove != "" && mailApprove2 != "" && $("#approvalRole").val() == 1) {
    //PRApprovalStatus2Approval
    if ($("#rdo1AppEmp").prop("checked") == true && $("#rdo1AppEmp2").prop("checked") == true) {
      nextApproval = true;
      doubleApproval = true;
      personStatus = 0; // set approval completed
      $("#approvalRole").val(1);
      $("#approvalStatus").val(1);
      //approval_remark = $("#approval_remark").val();
    } else {
      nextApproval = false;
    }
  } else if (mailApprove != "" && mailApprove2 != "" && $("#approvalRole").val() == 2) {
    //PRApprovalStatus2Approval
    if ($("#rdo1AppEmp").prop("checked") == true && $("#rdo1AppEmp2").prop("checked") == true) {
      nextApproval = true;
      personStatus = 0; // set approval completed
        app_role = $("#approvalRole").val();
        app_status = $("#approvalStatus").val();
        approval_remark = $("#approval_remark").val();
      //approval_remark = $("#approval_remark").val();
    } else {
      nextApproval = false;
    }
  }
  console.log(person + ":" + personStatus + ":" + $("#approval_remark").val());
  //return;
  if (!nextApproval && personStatus == 88) {
    // set approval 2 person
    const obj2 = {
      prno: $("#prno").val(),
      person: person,
      approval_remark: $("#approval_remark").val()
    }
    $.ajax({
      url: '/CTLAdmin/UpdatePRMultiSectionhead',
      type: 'PUT',
      data: obj2,
      success: function (response) {
        window.location.href = "/";
      }
    });
  }

  //if ($("#approvalRole").val() == '' && $("#approvalStatus").val() == '') {
  //  app_role = $("#approvalRole").val();
  //  app_status = $("#approvalStatus").val();
  //  approval_remark = $("#approval_remark").val();
  //}

  if (nextApproval) {

    // Sendmail by Role 11,21,31 * Check Role Money by appEmp, countEmp, authEmp is blank value
    let reqEmail = $('#lblReqMail').val();
    let appEmail = $('#lblAppEmpMail').val();
    let countEmail = $('#lblCountEmpMail').val();
    let authEmail = $('#lblAuthEmpMail').val();
    let sMAILGROUPADMIN = $('#sMAILGROUPADMIN').val();
    let concatFlag = $("#approvalRole").val() + $("#approvalStatus").val();

    let lblRequester = $("#lblRequester").text();
    let urlApprove = $("#urlApprove").val();
    var urlResult = $("#urlResult").val();
    let prno = $("#prno").val();

    const obj = {
      prno: $("#prno").val(),
      person: person,
      app_role: $("#approvalRole").val(),
      app_status: $("#approvalStatus").val(),
      approval_remark: $("#approval_remark").val()
    }
    //console.log(obj); return;
    $.ajax({
      url: '/CTLAdmin/PRApprovalStatus',
      type: 'PUT',
      data: obj,
      success: function (response) {
        if (response[0] === "0") {
          console.log("PR Approval Status updated successfully:", response);
          if ($("#approvalStatus").val() == 1) {
            // Sendmail by Role 11,21,31
            /* Last version before update 2 approval
            if (concatFlag == "11" && countEmail != "") { // Send to Count Manager
              var hdrSubject = "PR Approved. Dear Manager. " + prno;
              var Body = "<br/>PR Online, Requeste create by <b>" + lblRequester + "</b><br/>"
                + " <br/> "
                + " <font color='green'>PR Requested</font> "
                + "<br/>The document is ready for review as link <a href='" + urlApprove
                + "?docno=" + prno + "'>Click here PR No: " + prno
                + " <<< </a><br/>Best Regards,<br/>-[204] Powered by IT  Department-";
              updateCodeLog("204" + concatFlag);
              sendEmailAsync(Body, hdrSubject, countEmail);
              saveEmailLog("Log mail " + prno + "", countEmail, "Send 204.1");
            } else if (concatFlag == "11" && countEmail == "") { // If Count Emp is blank never send email to authorize, then send to Procurement immediately
              sendMailtoProcure(prno, lblRequester, urlApprove);
              saveEmailLog("Log mail " + prno + "", urlApprove, "Send 204.2");
            }
            */
            // Start version before update 2 approval
            if ((concatFlag == "11" && countEmail != "") || doubleApproval == true) { // Send to Count Manager (Add Double 2 approval)
              let txtDouble = doubleApproval == true ? " (Supervisor Double Approval)" : " (Supervisor Single Approval)";
              var hdrSubject = "PR Approved. Dear Manager. " + prno + " " + txtDouble;
              var Body = "<br/>PR Online, Requeste create by <b>" + lblRequester + "</b><br/>"
                + " <br/> "
                + " <font color='green'>PR Requested</font> "
                + "<br/>The document is ready for review as link <a href='" + urlApprove
                + "?docno=" + prno + "'>Click here PR No: " + prno
                + " <<< </a><br/>Best Regards,<br/>-[204] Powered by IT  Department-";
              updateCodeLog("204" + concatFlag);
              sendEmailAsync(Body, hdrSubject, countEmail);

              if (doubleApproval == false) {
                saveEmailLog("Log mail " + prno + "", countEmail, "Send 204.1A Single Approval to MGR");
              } else {
                saveEmailLog("Log mail " + prno + "", countEmail, "Send 204.1B Double Approval to MGR");
              }
            } else if (concatFlag == "11" && countEmail == "") { // If Count Emp is blank never send email to authorize, then send to Procurement immediately
              sendMailtoProcure(prno, lblRequester, urlApprove);
              saveEmailLog("Log mail " + prno + "", urlApprove, "Send 204.2");
            }
            // End version before update 2 approval

            if (concatFlag == "21" && authEmail != "") { // Send to Auth M.D.
              var hdrSubject = "PR Approved. Dear Management Director. " + prno;
              var Body = "<br/>PR Online, Requeste create by <b>" + lblRequester + "</b><br/>"
                + " <br/> "
                + " <font color='green'>PR Approved by manager</font> "
                + "<br/>The document is ready for review as link <a href='" + urlApprove
                + "?docno=" + prno + "'>Click here PR No: " + prno
                + " <<< </a><br/>Best Regards,<br/>-[205] Powered by IT  Department-";
              updateCodeLog("205" + concatFlag);
              sendEmailAsync(Body, hdrSubject, authEmail);
              saveEmailLog("Log mail " + prno + "", authEmail, "Send 205");
            } else if ((concatFlag == "21" && authEmail == "") || concatFlag == "22") { // If Auth Emp is blank never send email to authorize, then send to Procurement immediately
              sendMailtoProcure(prno, lblRequester, urlApprove);
              saveEmailLog("Log mail " + prno + "", urlApprove, "Send 205.2");
            }
            if (concatFlag == "31") { // Send to Procurement
              sendMailtoProcure(prno, lblRequester, urlApprove);
              saveEmailLog("Log mail " + prno + "", urlApprove, "Send 205.3");
            }

            if (concatFlag == "32" && authEmail != "") { // Send to M.D.
              var hdrSubject = "PR Approved. Dear Management Director. " + prno;
              var Body = "<br/>PR Online, Requeste create by <b>" + lblRequester + "</b><br/>"
                + " <br/> "
                + " <font color='green'>PR Approved by manager</font> "
                + "<br/>The document is ready for review as link <a href='" + urlApprove
                + "?docno=" + prno + "'>Click here PR No: " + prno
                + " <<< </a><br/>Best Regards,<br/>-[206] Powered by IT  Department-";
              updateCodeLog("206" + concatFlag);
              sendEmailAsync(Body, hdrSubject, authEmail);
              saveEmailLog("Log mail " + prno + "", authEmail, "Send 206");
            }

            if (concatFlag == "11" || concatFlag == "21" || concatFlag == "31") {
              //-->alert("✔ PR Approval successfully and send Email to next step.");
            }
            window.location.href = "/";
          } else if ($("#approvalStatus").val() == 9) {
            // Sendmail by Role 19,29,39
            let msgFrom = "";
            let msgReson = "";
            if (concatFlag == "19") {
              msgFrom = "from Approval";
              msgReson = $("#lblRemarkAuth").html();
            } else if (concatFlag == "29") {
              msgFrom = "from Manager";
              msgReson = $("#lblRemarkCount").html();
            } else if (concatFlag == "39") {
              msgFrom = "from M.D.";
              msgReson = $("#lblRemarkAuth").html();
            }
            var hdrSubject = "PR Reject. Dear Requester. " + prno;
            var Body = "<br/>PR Online, Requeste create by <b>" + lblRequester + "</b><br/>"
              + " <br/> "
              + "<br/><font color='red'>The document is reject " + msgFrom + "</font> "
              + "<u>" + msgReson + "</u>"
              + " Please review as link <a href='" + urlResult
              + "?docno=" + prno + "'>Click here PR No: " + prno
              + " <<< </a><br/>Best Regards,<br/>-[207" + concatFlag + "] Powered by IT  Department-";
            updateCodeLog("207" + concatFlag);
            sendEmailAsync(Body, hdrSubject, reqEmail);
            saveEmailLog("Log mail " + prno + "", reqEmail, "Send 207");
            //-->alert("✔ PR Reject successfully");
            window.location.href = "/";
          }

        } else {
          alert("Cancel failed");
        }
      }
    });
  }
});
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
function sendEmailAsync(_Body, _Subject, _Addresses) {
  var url = "/SendMail/MailSenderMessage";
  console.log(url, ':', _Body, ':', _Subject, ':', _Addresses);
  $.post(url, { Body: _Body, Form: "", Subject: _Subject, Addresses: _Addresses }, function (data) {
    console.log(data);
  });
}
function sendMailtoProcure(prno, requester, urlApprove) {
  var hdrSubject = "PR Approved. Dear, Procurement. " + prno;
  var Body = "<br/>PR Online, Requeste create by <b>" + requester + "</b><br/>"
    + " <br/> "
    + " <font color='green'>PR Approved</font> "
    + "<br/>The document is ready for review as link <a href='" + urlApprove
    + "?docno=" + prno + "'>Click here PR No: " + prno
    + " <<< </a><br/>Best Regards,<br/>-[200] Powered by IT  Department-";
  //console.log(Body + ':' + hdrSubject + ':' + sMAILGROUPADMIN);
  updateCodeLog("200");
  if (flagProcureEmailSent) {
    let sMAILGROUPADMIN = $('#sMAILGROUPADMIN').val();
    sendEmailAsync(Body, hdrSubject, sMAILGROUPADMIN);
    saveEmailLog("Log mail " + prno + "", sMAILGROUPADMIN, "Send 200");
  }
}

$("#btnOwnerCancel").click(function () {
  if (!confirm('❌❔ Confirm to cancel this PR.')) return;
  const obj = {
    prno: $("#prno").val(),
    person: 0,
    app_role: 7,
    app_status: 7,
    approval_remark: ''
  }
  $.ajax({
    url: '/CTLAdmin/PRApprovalStatus',
    type: 'PUT',
    data: obj,
    success: function (response) {
      if (response[0] === "0") {
        //-->alert("✔ PR Cancel successfully");
        let prno = $("#prno").val();
        window.location.href = "/CTLAdmin/PRResultRequest?docno=" + prno;
      } else {
        alert("Cancel failed");
      }
    }
  });
});

$("#btnSaveRemark").click(function () {
  if ($("#pub_remark").val() == '') return;
  if (!confirm('Confirm to update remark❔')) return;
  const obj = {
    prno: $("#prno").val(),
    pub_remark: $("#pub_remark").val()
  }
  $.ajax({
    url: '/CTLAdmin/UpdatePRRemark',
    type: 'PUT',
    data: obj,
    success: function (response) {
      if (response[0] === "0") {
        //-->alert("✔ PR Update remark successfully");
        let prno = $("#prno").val();
        loadPRHisRemark(docno);
        //window.location.href = "/CTLAdmin/PRApproval?docno=" + prno;
      } else {
        alert("Update failed");
      }
    }
  });
});

$("#btnAddHisRemark").click(function () {
  if ($("#hisremark").val() == '') return;
  if (!confirm('Confirm to Add remark❔')) return;
  const obj = {
    prno: $("#prno").val(),
    remarks: $("#hisremark").val()
  }
  $.ajax({
    url: '/CTLAdmin/AddHisPRRemark',
    type: 'PUT',
    data: obj,
    success: function (response) {
      if (response[0] === "0") {
        //-->alert("✔ PR Update remark successfully");
        $("#hisremark").val("");
        let prno = $("#prno").val();
        loadPRHisRemark(prno);
        //window.location.href = "/CTLAdmin/PRApproval?docno=" + prno;
      } else {
        alert("Update failed");
      }
    }
  });
});
function loadPRHisRemark(docno) {
  let loginRole = $('#sUROLEADMIN').val();
  $.ajax({
    url: '/CTLAdmin/GetPRHisRemark?prno=' + docno,
    type: 'GET',
    success: function (data) {
      console.log(data);
      let html = "";
      if (loginRole == 1) { // Delete for Admin only
        $.each(data, function (i, row) {
          html += `<tr>
          <td>${i + 1}. ${row.remarks} <i>${row.create_dt_txt}</i> </td>
          <td>${row.empcode}</td>
          <td><i class="ri-chat-delete-fill text-danger" style="font-size: 24px; cursor: pointer;" onclick='JavaScript:DelMsgRemark("${row.id}")'></i></td>
        </tr>`;
        });
      } else {
        $.each(data, function (i, row) {
          html += `<tr>
          <td>${i + 1}. ${row.remarks} <i>${row.create_dt_txt}</i> </td>
          <td>${row.empcode}</td>
          <td></td>
        </tr>`;
        });
      }

      $("#bindDataTableRemark tbody").html(html);
    }
  });
}
function DelMsgRemark(id) {
  if (!confirm('❌ Confirm to delete remark ❔')) return;
  $.ajax({
    url: '/CTLAdmin/DeleteHisPRRemark',
    type: 'DELETE',
    data: { id: id },
    success: function (response) {
      if (response[0] === "0") {
        let prno = $("#prno").val();
        loadPRHisRemark(prno);
      } else {
        alert("Delete failed");
      }
    }
  });

}

$("#btnSndQuotation").click(function () {
  if (!confirm('❔ Confirm to send Quotation Return?')) return;
  let rdoQuoRtn = $("input[name='rdoQuoRtn']:checked").val();
  let Obj = {
    prno: $("#prno").val(),
    quo_return: rdoQuoRtn
  }
  $.ajax({
    url: '/CTLAdmin/SaveQuoReturn',
    type: 'PUT',
    data: Obj,
    success: function (response) {
      if (response[0] === "0") {
        //location.href = "/";
        let prno = $("#prno").val();
        window.location.href = "?docno=" + prno + "&fullscreen=false";
      } else {
        alert("Update failed");
      }
    }
  });
});
