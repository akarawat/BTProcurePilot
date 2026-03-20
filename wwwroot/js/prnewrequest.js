$(document).ready(function () {
  //modalAuth
  var myModal = new bootstrap.Modal(document.getElementById('modalAuth'));
  let empCode = $("#empCode").val();
  let adminRole = $("#adminRole").val();
  //console.log(empCode == '');
  if (empCode == '') {
    alert("📌 Please login. \n 🔐 กรุณาลอกอินเข้าระบบ");
    location.href = "/";
  }
  //if (adminRole != '') {
  //  var role = [1, 2].includes(Number(adminRole));
  //  if (!role) {
  //    myModal.show();
  //  }
  //}
  
  $.ajax({
    type: "GET",
    url: "/CTLAdmin/GetUsersList",
    success: function (response) {
      //console.log(response);
      //VenName, VenCode, Vencurrency

      $('#bindDataTable').DataTable({
        data: response,
        columns: [
          { data: 'EMP_CODE' },
          { data: 'DISPNAME' },
          { data: 'USERROLE_TXT' },
          {
            data: 'ID',
            render: function (data, type, row) {
              //console.log(row);
              return `<div class="dropdown">
                          <button type="button" class="btn p-0 dropdown-toggle hide-arrow" data-bs-toggle="dropdown"><i class="ri-more-2-line"></i></button>
                          <div class="dropdown-menu">
                                      <a class="dropdown-item" href="javascript:EditUserProcure('${data}', '${row.EMP_CODE}', '${row.USRROLE}');">
                                <i class="ri-pencil-line me-1"></i> Edit</a>
                              <a class="dropdown-item" href="javascript:DelUserProcure('${data}');"><i class="ri-delete-bin-6-line me-1"></i> Delete</a>
                          </div>
                      </div>`;
            }
          }

        ]
      });

      //$(".btnTest").on("click", function () {
      //  /*console.log($(this).parents('tr').find('.testNameClass').val());*/
      //  console.log($(this).parents('tr'));
      //});

    },
    error: function (response) {
      alert(response.responseText);
    }
  });
});
//document.getElementById('id_emp_procure').addEventListener('input', function () {
//  var input = this.value;
//  var options = document.getElementById('opt_emp_procure').options;

//  for (var i = 0; i < options.length; i++) {
//    if (options[i].value === input) {
//      console.log("Selected value:", options[i].value);
//      console.log("Display name:", options[i].text);

//      // ถ้าต้องการเก็บค่าอื่นใส่ hidden field
//      document.getElementById('selected_displayname').value = options[i].text;
//      break;
//    }
//  }
//});
function EditUserProcure(data, EMP_CODE, USRROLE) {
  //console.log(data, EMP_CODE, USRROLE);
  $("#tmpID").val(data);
  var datalist = document.getElementById('opt_emp_procure');
  var options = datalist.options;

  // Loop หา option.value ที่ขึ้นต้นด้วย EMP_CODE
  for (var i = 0; i < options.length; i++) {
    if (options[i].value.startsWith(EMP_CODE)) {
      document.getElementById('id_emp_procure').value = options[i].value;
      break;
    }
  }
  document.getElementById('usrRole').value = USRROLE;
}

$("#btnSave").click(function () {
  let tmp_ID = $("#tmpID").val();
  let tmp_samacc = $("#id_emp_procure").val();
  let emp_role = $("#usrRole").val();
  if (tmp_samacc == '' || emp_role == '') return;
  var arr = tmp_samacc.split(':');
  let emp_code = arr[0].split('-')[0];
  var arr_samm = arr[1].split('@');
  let emp_samacc = arr_samm[0].trim();

  if (!confirm('ยืนยันการบันทึกข้อมูล')) return;
  let obj = {
    ID: tmp_ID,
    EMP_CODE: emp_code,
    USERLOGON: emp_samacc,
    USRROLE: emp_role
  }
  if (tmp_ID == '') { // Insert
    $.ajax({
      url: "/CTLAdmin/InsAddUserProcure",
      type: "POST",
      data: obj,
      success: function (response) {
        if (response[1] == "success" && response[2] == "1") {
          location.href = "/CTLAdmin/UserSetting";
        } else if (response[1] == "success" && response[2] == "0") {
          alert("User ที่กำหนด มีข้อมูลนี้แล้ว");
        } else {
          alert(response[1]);
        }
      },
      error: function (request, status, error) {
        alert(request.responseText);
      }
    });
  } else { // Update
    $.ajax({
      url: "/CTLAdmin/UpdateAddUserProcure",
      type: "PUT",
      data: obj,
      success: function (response) {
        if (response[1] == "success" && response[2] == "1") {
          location.href = "/CTLAdmin/UserSetting";
        } else if (response[1] == "success" && response[2] == "0") {
          alert("User ที่กำหนด มีข้อมูลนี้แล้ว");
        } else {
          alert(response[1]);
        }
      },
      error: function (request, status, error) {
        alert(request.responseText);
      }
    });
  }
});
function DelUserProcure(ID) {
  if (!confirm('ยืนยันการลบข้อมูล')) return;
  let tmp_ID = ID;
  let obj = {
    ID: tmp_ID
  }
  $.ajax({
    url: "/CTLAdmin/DeleteAddUserProcure",
    type: "DELETE",
    data: obj,
    success: function (response) {
      location.href = "/CTLAdmin/UserSetting";
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
}
$("#btnModalClose").click(function () {
  location.href = "/";
})

//$(document).on('change', 'input', function () {
//  var options = $('datalist')[0].options;
//  var val = $(this).val();
//  for (var i = 0; i < options.length; i++) {
//    if (options[i].value === val) {
//      console.log(val);
//      break;
//    }
//  }
//});


// Toggle full screen
document.getElementById("toggleFullscreenBtn").addEventListener("click", function () {
  const isFull = localStorage.getItem("isFullscreen") === "true";
  localStorage.setItem("isFullscreen", (!isFull).toString());
  // Reload เพื่อใช้ Razor Layout ใหม่
  window.location.href = "?fullscreen=" + isFull;

});

$(document).ready(function () {

});
function EmptySupplierData(sucgid) {
  $("#id_supp" + sucgid).val('');
  $("#name_supp" + sucgid).val('');
  $("#vc_supp" + sucgid).val('');
  $("#contact_supp" + sucgid).val('');
  $("#email_supp" + sucgid).val('');
  $("#tel_supp" + sucgid).val('');
  $("#remark_supp" + sucgid).val('');
  $("#quoref_supp" + sucgid).val('');
  $("#refnodt_supp" + sucgid).val('');
}
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
