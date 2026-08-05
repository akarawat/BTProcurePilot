var docno;

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

$(document).ready(function () {
  // อ่านค่าจาก URL เช่น ?docno=PR25680723001
  const urlParams = new URLSearchParams(window.location.search);
  docno = urlParams.get('docno');

  let UserDepCode = $("#reqDepCode").val();
  if (UserDepCode == 310) {
    document.getElementById("DivAppEmp2").style.display = "block";
  } else {
    document.getElementById("DivAppEmp2").style.display = "none";
  }
  
  if (docno) {
    $.ajax({
      url: '/CTLAdmin/GetPRByDocNo?docno=' + encodeURIComponent(docno),
      type: 'GET',
      success: function (response) {
        console.log(response);
        if (response.id == null) {
          location.href = "/";
        }
        console.log(response.prstatus);
        // Scoped to Procurement reject only — this feature is Procurement (Reject) + Requester (Resend), no Approver involvement.
        let isRejected = response.procure_flag == 7 || response.procure_flag == 9;
        if (response.prstatus >= 1 && !isRejected) {
          window.location.href = "/CTLAdmin/PRApproval?docno=" + docno;
          return;
        }


        loadFileAttachTable(response.id);

        $("#prid").val(response.id);
        $("#ref_prid").val(response.id);
        $("#prno").val(response.prno);
        $("#prno_draft").val(response.prno);
        $("#id_projno").val(response.projectno);
        setDateInput('approx_dt', response.approx_dt);

        if (response.approx_type == 3) {
          $("#rdoApprox").prop("checked", true);
        } else {
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
          SupPurpose(3);
        }
        $("#reference").val(response.ref_docs);
        $("#reason").val(response.pr_reason);
        setDateInput('prReceivedDate', response.pr_recvdt);
        $("#poNumber").val(response.pr_recvpono);
        $("#procRemark").html(response.procure_remark);
        
        let lblReqDate = moment(response.reqDate).format('DD-MM-YYYY');
        $("#lblReqDate").html(lblReqDate);
        $("#lblRequester").html(response.empcode_txt);
        $("#lblReqStatus").html(response.prstatus_txt);
        $("#pub_remark").text(response.pub_remark);

        $('#prcurrency').val(response.prcurrency);

        $('#lblAuthEmpDate').text(response.authDate_txt);
        $('#approve_step').val(response.approve_step);

        if (response.appEmp != '') {
          $("#lblAppEmpMail").html(response.appEmail);
          $("#lblAppEmpDisp").html(response.appEmp_txt);
          $('#id_appEmp').val(response.appEmp);
          $("#lblAppEmpDate").html(response.appDate_txt);
        }
        if (response.appEmp2 != '') {
          $("#lblAppEmp2Mail").html(response.appEmail2);
          $("#lblAppEmp2Disp").html(response.appEmp2_txt);
          $('#id_appEmp2').val(response.appEmp2);
          $("#lblAppEmp2Date").html(response.appDate2_txt);
        }

        if (response.countEmp != '') {
          $("#lblCountEmpMail").html(response.CountEmail);
          $("#lblCountEmpDisp").html(response.countEmp_txt);
          $('#id_countEmp').val(response.countEmp);
          $("#lblCountEmpDate").html(response.countDate_txt);
        }
        if (response.authEmp != '') {
          $("#lblAuthEmpMail").html(response.authEmail);
          $("#lblAuthEmpDisp").html(response.authEmp_txt);
          $('#id_authEmp').val(response.authEmp);
          $("#lblAuthEmpDate").html(response.authDate_txt);
        }
        
        $('#appFlag').val(response.appFlag);
        $('#countFlag').val(response.countFlag);
        $('#authFlag').val(response.authFlag);

        $('#lblRemarkEmp').html(response.remarkEmp);
        $('#lblRemarkCount').html(response.remarkCount);
        $('#lblRemarkAuth').html(response.remarkAuth);

        $('#item_disc').prop('placeholder', '0');
        // ENd Approve info

        loadSuggestedVendor(docno, 1);
        loadSuggestedVendor(docno, 2);
        loadSuggestedVendor(docno, 3);
        loadSuggestedVendor(docno, 4);
        loadSuggestedVendor(docno, 5);
        loadPRItemTable(docno);
        setIndividualPR(response.empcode);
        loadPRHisRemark(docno);
        //$("#myForm :input").prop("disabled", true);
        //$("#myForm :textarea").prop("disabled", true);

        let loginEmpCode = $("#empCode").val();
        if (loginEmpCode == response.empcode && isRejected) {
          document.getElementById("divBtnSubmit").style.display = "none";
          document.getElementById("divBtnResubmitSave").style.display = "block";
        }

      },
      error: function () {
        alert("Error loading PR document");
      }
    });

    
    
  } else {
    alert("❌ ไม่พบ parameter docno ใน URL");
  }
});

// Toggle full screen
document.getElementById("toggleFullscreenBtn").addEventListener("click", function () {
  const urlParams = new URLSearchParams(window.location.search);
  const docno = urlParams.get('docno');
  console.log(docno);
  const isFull = localStorage.getItem("isFullscreen") === "true";
  localStorage.setItem("isFullscreen", (!isFull).toString());
  // Reload เพื่อใช้ Razor Layout ใหม่
  window.location.href = "?docno=" + docno +"&fullscreen=" + isFull;

});
function loadSuggestedVendor(docno, suggItem) {
  $.ajax({
    url: '/CTLAdmin/GetSuppByDocno',
    type: 'GET',
    data: {
      docno: docno,
      sugg_item: suggItem
    },
    success: function (data) {
      //console.log("Suggested Vendors:", data);

      if (data.length > 0) {
        let vendor = data[0]; // ถ้าแสดงแค่รายเดียว
        $('#sucgid' + suggItem).val(vendor.id);
        $('#id_supp' + suggItem).val(vendor.vencode);
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
        console.log("No data found for supplier " + suggItem);
      }
    },
    error: function () {
      alert("❌ Error loading suggested vendor");
    }
  });
}
function loadPRItemTable(docno) {
  
    $.ajax({
      url: '/CTLAdmin/GetPRItemDetailByDocno?prno=' + docno,
      type: 'GET',
      success: function (data) {
        let html = "";
        let currency = $("#prcurrency").val();
        item_count = data.length;
        console.log(data.length);
        let sumTotal = 0, spnDisc = 0;
        $.each(data, function (i, row) {
          
          spnDisc += row.item_disc;
          sumTotal += row.item_amount;
          html += `<tr>
          <td>${row.item_btnumber}</td>
          <td>${row.item_descript}</td>
          <td>${row.item_model}</td>
          <td>${row.item_acccode}</td>
          <td>${row.item_costdep}</td>
          <td align='right'>${row.item_qty}</td>
          <td align='right'>${row.item_unit}</td>
          <td>${formatNumber(row.item_unitprice, 4)} ` + currency + `</td>
          <td>${formatNumber(row.item_disc, 2)}</td>
          <td align='right'>(${formatNumber(row.item_amount, 2)})</td>
          <td>
              <i class="ri-edit-box-fill text-warning" onclick="editItem('${row.id}')" style="cursor:pointer; font-size: 24px;" title='Edit'></i> 
              <i class="ri-delete-bin-2-fill text-danger" onclick="deleteItem('${row.id}')" style="cursor:pointer; font-size: 24px;" title='Delete'></i>
          </td>
        </tr>`;
        });
        
        $("#spnDisc").html(formatNumber(spnDisc, 2));
        $("#spnTotal").html(formatNumber(sumTotal, 2));
        $("#bindDataTable tbody").html(html);
        //id_appEmp
        //id_countEmp
        //id_authEmp

        //console.log(currency, ':', sumTotal);
        //---> # Verify for Non RFQ
        if ($("#rdoPurpose3").prop("checked")) {
          // RFQ No Approval 2-3
          console.log("RFQ");
          $("#id_appEmp").prop('disabled', false);
          $("#id_countEmp").prop('disabled', true);
          $("#id_authEmp").prop('disabled', true);
        } else {
          // Start None RFQ
          console.log("None RFQ");
          if (currency == "USD" ||
            currency == "EUR" ||
            currency == "CHF") {
            console.log("MD: " + sumTotal);
            if (sumTotal <= 1999) {
              console.log("Less 2K");
              $("#id_appEmp").prop('disabled', false);
              $("#id_countEmp").prop('disabled', false);
              $("#id_authEmp").prop('disabled', true);
              //--> USD. Reset MD
              ResetApproval("md");
            } else if (sumTotal >= 2000) {
              console.log("More 2K");
              $("#id_appEmp").prop('disabled', false);
              $("#id_countEmp").prop('disabled', false);
              $("#id_authEmp").prop('disabled', false);
            } else {
              //--> USD. Reset MGR
              ResetApproval("mgr");
              //--> USD. Reset MD
              ResetApproval("md");
            }
          } else {
            if (sumTotal <= 5000) {
              console.log("Oth Section: " + sumTotal);
              $("#id_appEmp").prop('disabled', false);
              $("#id_countEmp").prop('disabled', true);
              $("#id_authEmp").prop('disabled', true);
              //--> USD. Reset MGR
              ResetApproval("mgr");
              //--> USD. Reset MD
              ResetApproval("md");
            } else if (sumTotal >= 5001 && sumTotal <= 59999) {
              console.log("Oth MGR: " + sumTotal);
              $("#id_appEmp").prop('disabled', false);
              $("#id_countEmp").prop('disabled', false);
              $("#id_authEmp").prop('disabled', true);
              //--> USD. Reset MD
              ResetApproval("md");
            } else if (sumTotal >= 60000) {
              console.log("Oth MD: " + sumTotal);
              $("#id_appEmp").prop('disabled', false);
              $("#id_countEmp").prop('disabled', false);
              $("#id_authEmp").prop('disabled', false);
            }
          }
          // End None RFQ
        }
        

      }
    });
}
function ResetApproval(role) {
  if (role == 'sec1') {
    $("#lblAppEmpMail").html(null);
    $("#lblAppEmpDisp").html(null);
    $('#id_appEmp').val(null);
    $("#lblAppEmpDate").html(null);
  }
  if (role == 'sec2') {
    $("#lblAppEmp2Mail").html(null);
    $("#lblAppEmp2Disp").html(null);
    $('#id_appEmp2').val(null);
    $("#lblAppEmp2Date").html(null);
  }
  if (role == 'mgr') {
    $("#lblCountEmpMail").html(null);
    $("#lblCountEmpDisp").html(null);
    $('#id_countEmp').val(null);
    $("#lblCountEmpDate").html(null);
  }
  if (role == 'md') {
    $("#lblAuthEmpMail").html(null);
    $("#lblAuthEmpDisp").html(null);
    $('#id_authEmp').val(null);
    $("#lblAuthEmpDate").html(null);
  }
}
function loadFileAttachTable(prid) {
  $.ajax({
    url: '/CTLAdmin/loadFileAttachTable?prid=' + prid,
    type: 'GET',
    success: function (data) {
      let html = "";
      $.each(data, function (i, row) {
        html += `<tr>
          <td>${i+1}. ${row.filename}</td>
          <td>${row.filetype_txt}</td>
          <td><a href='${row.filepath}' target='_blank'><i class="ri-inbox-archive-fill text-info" style="font-size: 24px;"></i></a></td>
          <td>
              <i class="ri-delete-bin-2-fill text-danger" onclick="deleteFileItem('${row.id}', '${row.filepath}')" style="cursor:pointer; font-size: 24px;" title='Delete'></i>
          </td>
        </tr>`;
      });

      $("#tblFiles tbody").html(html);
    }
  });
}
function deleteItem(id) {
  if (confirm("❌ Are you sure to delete this item ❔")) {
    $.ajax({
      url: '/CTLAdmin/DeletePRItemDetailById',
      type: 'POST',
      data: { id: id },
      success: function (response) {
        if (response[0] === "0") {
          alert("✔ Item deleted successfully");
          let prno = $("#prno").val();
          loadPRItemTable(prno);
        } else {
          alert("Delete failed");
        }
      }
    });
  }
}
function deleteFileItem_Bak(id, filepath) {
  if (confirm("❌ Are you sure to delete this file item❔")) {
    $.ajax({
      url: '/CTLAdmin/DeleteFileItemById',
      type: 'POST',
      data: { id: id },
      success: function (response) {
        if (response[0] === "0") {
          alert("✔ Item deleted successfully");
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

  if (!confirm("❔❌ Are you sure to delete this file item?")) return;

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
        alert("✔ Updated Successfully");
        $('#editItemModal').modal('hide');
        loadPRItemTable($("#prno").val()); // โหลดใหม่
      } else {
        alert("❌ Update Failed: " + res[1]);
      }
    }
  });
}
function ClearSupplierData(sucgid) {
  let id_supp1 = $("#sucgid" + sucgid).val();
  if (id_supp1 == '') return;
  if (!confirm('⁉️ Confirm sugges supplier clear information?')) return;
  $.ajax({
    url: '/CTLAdmin/DeleteSuggSupplierById',
    type: 'POST',
    data: { id: id_supp1 },
    success: function (response) {
      if (response[0] === "0") {
        alert("✔ Suggess supplier deleted successfully"); 
        let prno = $("#prno").val();
        window.location.href = "/CTLAdmin/PRResultRequest?docno=" + prno;
      } else {
        alert("Delete failed");
      }
    }
  });
}
function ReviseSupplierData(sucgid) {
  if ($("#id_supp" + sucgid).val() == '') {
    alert('❌ Is empty sugges supplier information?');
    return;
  }
  const obj = {
    prno: $("#prno").val(),
    sugg_item: sucgid,
    id_supp: $("#id_supp" + sucgid).val(),
    name_supp: $("#name_supp" + sucgid).val(),
    vc_supp: $("#vc_supp" + sucgid).val(),
    contact_supp: $("#contact_supp" + sucgid).val(),
    email_supp: $("#email_supp" + sucgid).val(),
    tel_supp: $("#tel_supp" + sucgid).val(),
    remark_supp: $("#remark_supp" + sucgid).val(),
    quoref_supp: $("#quoref_supp" + sucgid).val(),
    refnodt_supp: $("#refnodt_supp" + sucgid).val()
  };
  
  if (!confirm('❔ Confirm Revise sugges supplier information?')) return;
  $.ajax({
    url: '/CTLAdmin/ReviseSuggSupplierById',
    type: 'PUT',
    data: obj,
    success: function (response) {
      if (response[0] === "0") {
        alert("✔ Suggess supplier updated successfully"); 
        let prno = $("#prno").val();
        window.location.href = "/CTLAdmin/PRResultRequest?docno=" + prno;
      } else {
        alert("Revise failed");
      }
    }
  });
}
// Function Upload file
$(function () {
  const MAX_SIZE = 7 * 1024 * 1024; // 5 MB

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
          window.location.href = "/CTLAdmin/PRResultRequest?docno=" + prno;
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


$("#btnOwnerCancel").click(function () {
  if (!confirm('❔ Confirm to cancel this PR.')) return;
  updateCodeLogPr('700', $("#prno").val());
  const obj = {
    prno: $("#prno").val(),
    app_role: 7,
    app_status: 7
  }
  $.ajax({
    url: '/CTLAdmin/PRApprovalStatus',
    type: 'PUT',
    data: obj,
    success: function (response) {
      if (response[0] === "0") {
        alert("✔ PR Cancel successfully");
        let prno = $("#prno").val();
        window.location.href = "/CTLAdmin/PRResultRequest?docno=" + prno;
      } else {
        alert("Cancel failed");
      }
    }
  });
});
$("#btnResubmitSave").click(function () {
  if (!confirm('❔ Confirm to Resend this PR to Procurement?')) return;
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
        window.location.href = "/CTLAdmin/PRApproval?docno=" + prno;
      } else {
        alert("Resend failed");
      }
    }
  });
});
function setIndividualPR(empCode) {
  const myButton = document.getElementById('divBtnOwnerCancel');
  //myButton.style.display = 'none';
  console.log($("#empCode").val().length, ':', empCode.length);
  if ($("#empCode").val() == empCode) {
    console.log('True: ', $("#empCode").val(), ':', empCode);
    myButton.style.display = 'block';
  } else {
    console.log('False: ', $("#empCode").val(), ':', empCode);
    myButton.style.display = 'none';
  }
}

$('#item_btnumber').on('input', function () {
  var inputVal = $(this).val();
  let arrItem = inputVal.split(':');
  if (arrItem.length == 2) {
    $("#item_btnumber").val(arrItem[0].trim());
    $("#item_descript").val(arrItem[1].trim());
  }
});

function updateCodeLogPr(codelog, prno) {
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

function SupPurpose(suppur) {
  //console.log(suppur);
  if (suppur == '3') {
    document.getElementById("card-supp1").style.display = "block";
    document.getElementById("card-supp2").style.display = "block";
    document.getElementById("card-supp3").style.display = "block";
    document.getElementById("card-supp4").style.display = "block";
    document.getElementById("card-supp5").style.display = "block";
  }
  else {
    document.getElementById("card-supp1").style.display = "block";
    document.getElementById("card-supp2").style.display = "none";
    document.getElementById("card-supp3").style.display = "none";
    document.getElementById("card-supp4").style.display = "none";
    document.getElementById("card-supp5").style.display = "none";
  }

}


$("#btnSaveRemark").click(function () {
  if ($("#pub_remark_txt").val() == '') return;
  if (!confirm('Confirm to update remark❔')) return;
  const obj = {
    prno: $("#prno").val(),
    pub_remark: $("#pub_remark_txt").val()
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
  console.log(docno);
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
