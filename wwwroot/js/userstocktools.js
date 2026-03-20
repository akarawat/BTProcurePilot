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

function ModalAddNewUser() {
  var myModal = new bootstrap.Modal(document.getElementById("exampleModal"));
  myModal.show();
}
function ModalEditUser(trid, empcode, userfname, usrfunc, apprv_stock) {
  //alert('Under construction'); return;
  //'@rowVal.ID','@rowVal.EMPCODE','@rowVal.USERFNAME','@rowVal.USRFUNC','@rowVal.apprv_stock'
  var myModal = new bootstrap.Modal(document.getElementById("exampleModalEdit"));
  myModal.show();
  $("#lblTRID").val(trid);
  //$('[name="lst_emp_stock"]').val(empcode);
  
  $("#edit_emp_code").html(empcode);
  //$("#edit_lst_emp_samacc").html(userfname);
  $("#edit_lst_emp_samacc").val(userfname);
  $("#edit_lblSelectUserFunc").html(usrfunc);
  $('[name="edit_apprv_stock"]').val(apprv_stock);

}
function SaveEditEmp() {
  alert('Edit');
  var trid = $("#lblTRID").val();
  var edit_lst_emp_samacc = $("#edit_lst_emp_samacc").val();
  var apprv_stock = $('[name="edit_apprv_stock"]').val();
  var arrAprov = apprv_stock.split(':');
  //console.log(arrAprov[0].substr(0, 2).toupper());
  let str_apprv_stock;
  if (arrAprov[0].substr(0, 2) == "S0") {
    str_apprv_stock = arrAprov[0];
  } else {
    str_apprv_stock = null;
  }
  
  if (!confirm("ยืนยันการบันทึกข้อมูล")) return;
  var obj = {
    ID: trid,
    apprv_stock: str_apprv_stock,
    TMP_FNAME: edit_lst_emp_samacc
  }

  $.ajax({
    url: "/PPStock/EditUserStock",
    type: "PUT",
    data: obj,
    success: function (response) {
      //console.log(response);
      location.reload();
    }, error: function (request, status, error) { }
  });
}

$("#searchItems").keyup(function (e) {
  var code = e.key; // recommended to use e.key, it's normalized across devices and languages
  if (code === "Enter") e.preventDefault();
  if (code === " " || code === "Enter" || code === "," || code === ";") {
    let itemNo = $("#searchItems").val();
    if (itemNo.length >= 3) {
      console.log(itemNo);
      location.href = "/PPStock/UserStockTools?search=" + itemNo;
      //$.ajax({
      //  url: "/PPStock/StockTools?search=" + itemNo,
      //  type: "GET",
      //  data: null,
      //  success: function (response) {
      //    console.log(response);
      //  }, error: function (request, status, error) { }
      //});

    }
  } // missing closing if brace
});
function getEmpName() {

}
function AddNewEmp(){
  var emp_code = $('[name="lst_emp_stock"]').val();
  var user_func = $("#lblSelectUserFunc").val();
  var emp_fname = $("#id_dispname_stock").val();
  var sel_spp = $('[name="lst_apprv_stock"]').val();
  console.log(emp_code, ", ", sel_spp, ", ", user_func, ", ", emp_fname);
  var arrTxt = emp_code.split(":");
  if (arrTxt.length >= 2) emp_code = arrTxt[0];
  var arrAppTxt = sel_spp.split(":");
  if (arrAppTxt.length >= 2) sel_spp = arrAppTxt[0];

  var obj = {
    EMPCODE: emp_code,
    TMP_FNAME: emp_fname,
    USRROLE: 1,
    USRFUNC: user_func,
    ORDMAIL: "",
    apprv_stock: sel_spp
  }
  $.ajax({
    url: "/PPStock/AddnewUserStock",
    type: "POST",
    data: obj,
    success: function (response) {
      //console.log(response);
      location.reload();
    }, error: function (request, status, error) { }
  });
}
function ModalDeleteUser(trid){
  if (!confirm('ยืนยันการลบข้อมูลรายการนี้')) return;
  var obj = {
    ID: trid
  }
  $.ajax({
    url: "/PPStock/DeleteStockUser",
    type: "POST",
    data: obj,
    success: function (response) {
      //console.log(response);
      location.reload();
    }, error: function (request, status, error) { }
  });
}
/*
$(document).on('change', 'input', function () {
  var options = $('datalist')[0].options;
  //var options = $('#opt_apprv_stock').options;
  var val = $(this).val();
  for (var i = 0; i < options.length; i++) {
    if (options[i].value === val) {
      console.log(val);
      break;
    }
  }
});
*/
//---> Data List Start

function qs(query, context) {
  return (context || document).querySelector(query);
}

function qsa(query, context) {
  return (context || document).querySelectorAll(query);
}

qs("#id_emp_stock").addEventListener('change', function (e) {
  var settxt;
  var options = qsa('#' + e.target.getAttribute('list') + ' > option'),
    values = [];
  [].forEach.call(options, function (option) {
    //console.log(option);
    values.push(option.value);
  });
  var currentValue = e.target.value;
  
  if (values.indexOf(currentValue) !== -1) {
    //console.log('evento "change" %s', currentValue);
    var arrTxt = currentValue.split(":");
    
    if (arrTxt.length >= 2)
      $("#id_dispname_stock").val(arrTxt[1]);
    else
      $("#id_dispname_stock").val(arrTxt[0]);
  }
  
});
qs("#id_apprv_stock").addEventListener('change', function (e) {
  var options = qsa('#' + e.target.getAttribute('list') + ' > option'),
    values = [];
  [].forEach.call(options, function (option) {
    values.push(option.value)
  });
  var currentValue = e.target.value;
  if (values.indexOf(currentValue) !== -1) {
    console.log('evento "change" %s', currentValue);
  }
});
//---> Data List End
function CancelAddEmp() {

}
function SelectUserFunc(role) {
  alert(role);
  if (role == 1) {
    $("#lblSelectUserFunc").val("ACK");
  } else if (role == 2) {
    $("#lblSelectUserFunc").val("STOCK");
  } else if (role == 3) {
    $("#lblSelectUserFunc").val("USER");
  } else if (role == 4) {
    $("#lblSelectUserFunc").val("APPROV");
  } else if (role == 5) {
    $("#lblSelectUserFunc").val("ADMINDOC");
  } else if (role == 7) {
    $("#lblSelectUserFunc").val("MANAGER");
  } else if (role == 9) {
    $("#lblSelectUserFunc").val("CANCEL");
  }
  
}
