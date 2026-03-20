/**
 * Account Settings - Account
 */

'use strict';

document.addEventListener('DOMContentLoaded', function (e) {
  (function () {
    const deactivateAcc = document.querySelector('#formAccountDeactivation');

    // Update/reset user image of account page
    let accountUserImage = document.getElementById('uploadedAvatar');
    const fileInput = document.querySelector('.account-file-input'),
      resetFileInput = document.querySelector('.account-image-reset');

    if (accountUserImage) {
      const resetImage = accountUserImage.src;
      fileInput.onchange = () => {
        if (fileInput.files[0]) {
          accountUserImage.src = window.URL.createObjectURL(fileInput.files[0]);
        }
      };
      resetFileInput.onclick = () => {
        fileInput.value = '';
        accountUserImage.src = resetImage;
      };
    }
  })();
});

function ConfirmApproveOrReject(docNo, stat) {
  if (stat == 1) {
    if (!confirm('Confirm to Approve')) return;
  } else if (stat == 3) {
    if (!confirm('Confirm to Approve')) return;
    //--> Send Mail to Stock controller
    var email_controller = $("#email_controller").val();
    if (email_controller != '') {

      //# Start Send mail
      let Body = "<br/><b>Dear Stock controller.<br/></b>"
        + "<br/>PP Tools stock ordering online request <br/>create by <b>" + $("#spnEmpOrder").html() + "</b><br/>"
        + "<br/>The document is ready for review as link <a href='" + $('#lblUrlPath').val() + "PPStock/OrderDetail?docNo=" + docNo + "'>Document No: " + docNo
        + "</a><br/>Best Regards,<br/>-Powered by IT  Department-";
      let Subject = "[FOR TOOLS STOCK CONTROLLER] Document No:" + docNo;

      let Addresses = "";
      //Addresses = approve_email + ";sakulchai_p@berninathailand.com";
      Addresses = email_controller;
      sendEmailAsync(Body, Subject, Addresses);
      //# End Send mail

    }

  } else if (stat == 9) {
    if (!confirm('Confirm to Cancel')) return;
  } else if (stat == 5) {
    if (!confirm('Confirm to Close job')) return;
  }
  
  $.ajax({
    url: "/PPStock/SetOrderApproveOrCancel?docNo=" + docNo + "&stat=" + stat,
    type: "GET",
    data: null,
    success: function (response) {
      //console.log(response);
      location.href = "/PPStock/OrderList";
    }, error: function (request, status, error) { }
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

async function SaveQtyRevise(itemno, tr_id) {
  var aval = $('#aval_' + itemno).html();
  var qty = $('#qty_' + itemno).val();
  console.log(aval, ':', qty);
  if (qty == undefined || qty == '') return;
  var iAval = (aval * 1);
  var iQty = (qty * 1);
  if (iQty > iAval) {
    alert("จำนวนไม่เพียงพอ [Wrong Avaliable Qty.]"); return;
  } else {
    if (!confirm("ยืนยันการแก้ไขจำนวนเบิก. Confirm to revise items.")) return;
      var obj = {
        ID: tr_id,
        Itemqty: iQty
      }
      $.ajax({
        url: "/PPStock/UpdateQtyOrderDetail",
        type: "POST",
        data: obj,
        success: function (response) {
          //console.log(response);
          location.reload();
        }, error: function (request, status, error) { }
      });
  }
}

function reviseApproval(docNo) {
  //alert(docNo);
  $("#lblTRID").val(docNo);
  var myModal = new bootstrap.Modal(document.getElementById("exampleModalEdit"));
  myModal.show();
}

function SaveEditEmpApprov() {
  var apprv_stock = $('[name="edit_apprv_stock"]').val();
  var docNo = $("#lblTRID").val();
  var arrAprov = apprv_stock.split(':');

  if (arrAprov.length != 2) return;
  if (!confirm("ยืนยันการแก้ไขผู้อนุมัติ")) return;
  var obj = {
    ordNo: docNo,
    emp_app: arrAprov[0]
  }

  //# Start Send mail
  var Body = "<br/><b>Dear Stock Approval.<br/></b>"
    + "<br/>PP Tools stock ordering online request <br/>create by <b>" + $("#spnEmpOrder").html() + "</b><br/>"
    + "<br/>The document is ready for review as link <a href='" + $('#lblUrlPath').val() + "PPStock/OrderDetail?docNo=" + docNo + "'>Document No: " + docNo
    + "</a><br/>Best Regards,<br/>-Powered by IT  Department-";
  var Subject = "[ระบบเบิกวัสดุ TOOLS STOCK CONTROLLER] Document No:" + docNo;

  var Addresses = "";
  //Addresses = approve_email + ";sakulchai_p@berninathailand.com";
  Addresses = arrAprov[1];
  sendEmailAsync2(Body, Subject, Addresses);
  //# End Send mail


  $.ajax({
    url: "/PPStock/EditUserApprovOrder",
    type: "PUT",
    data: obj,
    success: function (response) {
      //console.log(response);
      location.reload();
    }, error: function (request, status, error) { }
  });
  
}


