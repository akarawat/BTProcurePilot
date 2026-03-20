$(document).ready(function () {
  //modalAuth
  var myModal = new bootstrap.Modal(document.getElementById('modalAuth'));
  let adminRole = $("#adminRole").val();
  if (adminRole != '') {
    var role = [1, 2].includes(Number(adminRole));
    //if (!role) {
    //  myModal.show();
    //}
    if (role) {
      document.getElementById("spanAdmin").style.display = "block";
    }
  }

  $.ajax({
    type: "GET",
    url: "/CTLAdmin/GetVendorList",
    success: function (response) {
      //console.log(response);
      //VenName, VenCode, Vencurrency

      $('#bindDataTable').DataTable({
        data: response,
        columns: [
          {
            data: 'VenName',
            render: function (data, type, row) {
              let txtlink;
              txtlink = `
              <span class="badge rounded-pill text-success me-1" onclick="javascript:EditVendor('${data}','${row.VenCode}','${row.Vencurrency}');" style="cursor:pointer">${data}</span>`;
              return txtlink;
            }
          },
          { data: 'VenCode', className: 'testHidClass' },
          {
            data: 'Vencurrency',
            render: function (data, type, row) {
              //return `<input type="text" class="rec_eff" value="${data}" data-rowid="${row.wo_stat}">`;
              return `${row.Vencurrency}`;
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

$("#btnModalClose").click(function () {
  location.href = "/";
})

//$(document).ready(function () {
//  $.ajax({
//    url: "/PPStock/GetToolItemList",
//    type: "GET",
//    data: null,
//    success: function (response) {
//      console.log(response);
//    },
//    error: function (request, status, error) {
//      alert(request.responseText);
//    }
//  });
//});
function DeleteVen() {
  var VenCode = $("#VenCode").val();
  var VenName = $("#VenName").val();
  if (VenCode == '') return;
  if (!confirm('❌ Confirm delete vendor code "[' + VenCode + '] ' + VenName + '" ❔')) return;
  $.ajax({
    url: "/CTLAdmin/DelVendor",
    type: "DELETE",
    data: { VenCode: VenCode },
    success: function (response) {
      console.log(response);
      location.href = "/CTLAdmin/VendorList";
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
}
function EditVendor(VenName, VenCode, Vencurrency) {
  document.getElementById("btnVenEdit").style.display = "block";
  document.getElementById("btnVenDel").style.display = "block";
  //console.log(ActiveTo);
  $("#VenCodeTmp").val(VenCode);
  $("#VenCode").val(VenCode);
  $("#VenName").val(VenName);
  $("#Vencurrency").val(Vencurrency);
}

function SaveEditVen() {
  var VenCodeTmp = $("#VenCodeTmp").val();
  var VenCode = $("#VenCode").val();
  var VenName = $("#VenName").val();
  var Vencurrency = $("#Vencurrency").val();
  if (VenCodeTmp == '' || VenCode == '' || VenName == '') return;
  if (!confirm('Confirm update vendor code "[' + VenCode + '] ' + VenName + '" ❔')) return;
  const obj = {
    VenCodeTmp: VenCodeTmp,
    VenCode: VenCode,
    VenName: VenName,
    Vencurrency: Vencurrency
  };
  console.log(obj);
  $.ajax({
    url: "/CTLAdmin/SaveVenCode",
    type: "POST",
    data: obj,
    success: function (response) {
      location.href = "/CTLAdmin/VendorList";
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
};

$("#btnSubmit").click(function () {
  var VenCode = $("#VenCode").val();
  var VenName = $("#VenName").val();
  var Vencurrency = $("#Vencurrency").val();
  if (VenCode == '' || VenName == '' || Vencurrency == '') return;
  if (!confirm('Confirm add new vendor code "[' + VenCode + '] ' + VenName + '" ❔')) return;
  const obj = {
    VenCode: VenCode,
    VenName: VenName,
    Vencurrency: Vencurrency
  };

  $.ajax({
    url: "/CTLAdmin/AddNewVenCode",
    type: "POST",
    data: obj,
    success: function (response) {
      //console.log(response);
      location.href = "/CTLAdmin/VendorList";
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
});

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
