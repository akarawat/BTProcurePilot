$(document).ready(function () {
  //modalAuth
  //var myModal = new bootstrap.Modal(document.getElementById('modalAuth'));
  let adminRole = $("#adminRole").val();
  if (adminRole != '') {
    var role = [1, 2].includes(Number(adminRole));
      if (role) {
          document.getElementById("spanAdmin").style.display = "block";
      }
    //if (!role) {
    //  myModal.show();
    //}
  }

  $.ajax({
    type: "GET",
    url: "/CTLAdmin/GetAccCodeList",
    success: function (response) {
      //console.log(response);
      //
      //AccMain
      //AccName
      //AccType
      //AccCat 
      
      $('#bindDataTable').DataTable({
        data: response,
        columns: [
          {
            data: 'AccMain',
            render: function (data, type, row) {
              let txtlink;
              txtlink = `
              <span class="badge rounded-pill text-success me-1" onclick="javascript:EditAccount('${data}','${row.AccName}','${row.AccType}','${row.AccCat}');" style="cursor:pointer">${data}</span>`;
              return txtlink;
            }
          },
          { data: 'AccName', className: 'testHidClass' },
          { data: 'AccType' },
          {
            data: 'AccCat',
            render: function (data, type, row) {
              //return `<input type="text" class="rec_eff" value="${data}" data-rowid="${row.wo_stat}">`;
              return `${row.AccCat}`;
            }
          }
          //{ defaultContent: '<ul class="list-unstyled m-0 avatar-group d-flex align-items-center"><li data-bs-toggle="tooltip" data-popup="tooltip-custom" data-bs-placement="top" class="avatar avatar-xs pull-up" title="Settings"><img src="../img/avatars/setting.jpg" alt="Avatar" class="rounded-circle"></li></ul>' }
          

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
function DeleteAccount() {
  var accMainTmp = $("#accMainTmp").val();
  if (accMainTmp == '') return;
  if (!confirm('❌ Confirm delete account code "' + accMainTmp + '" ❔')) return;
  $.ajax({
    url: "/CTLAdmin/DelAccCode",
    type: "DELETE",
    data: { AccMain: accMainTmp },
    success: function (response) {
      console.log(response);
      location.href = "/CTLAdmin/AccCodeList";
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
}
function EditAccount(accMain, accName, accType, accCategory) {
  let adminRole = $("#adminRole").val();
  if (adminRole != '') {
    document.getElementById("btnAccEdit").style.display = "block";
    document.getElementById("btnAccDel").style.display = "block";
  }
  
  $("#accMainTmp").val(accMain);
  $("#accMain").val(accMain);
  $("#accName").val(accName);
  $("#accType").val(accType);
  $("#accCategory").val(accCategory);
}

function SaveEditAccount() {
  var accMainTmp = $("#accMainTmp").val();
  var accMain = $("#accMain").val();
  var accName = $("#accName").val();
  var accType = $("#accType").val();
  var accCategory = $("#accCategory").val();
  if (accMainTmp == '' || accMain == '' || accName == '') return;
  if (!confirm('Confirm update account code "' + accMainTmp + '"❔')) return;
  const obj = {
    AccMainTmp: accMainTmp,
    AccMain: accMain,
    AccName: accName,
    AccType: accType,
    AccCat: accCategory
  };
  console.log(obj);
  $.ajax({
    url: "/CTLAdmin/SaveAccCode",
    type: "POST",
    data: obj,
    success: function (response) {
      console.log(response);
      location.href = "/CTLAdmin/AccCodeList";
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
};

$("#btnSubmit").click(function () {
  var accMain = $("#accMain").val();
  var accName = $("#accName").val();
  var accType = $("#accType").val();
  var accCategory = $("#accCategory").val();
  if (accMain == undefined || accMain == '' || accName == '') return;
  if (!confirm('Confirm add new account code "' + accMain + '"❔')) return;
  const obj = {
    AccMain: accMain,
    AccName: accName,
    AccType: accType,
    AccCat: accCategory
  };
  console.log(obj);
  $.ajax({
    url: "/CTLAdmin/AddNewAccCode",
    type: "POST",
    data: obj,
    success: function (response) {
      //console.log(response);
      location.href = "/CTLAdmin/AccCodeList";
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
$("#btnModalClose").click(function () {
  location.href = "/";
})
