$(document).ready(function () {
  //modalAuth
  //var myModal = new bootstrap.Modal(document.getElementById('modalAuth'));
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
    url: "/CTLAdmin/GetProjCodeList",
    success: function (response) {
      //console.log(response);
      //[ProjNo], [ProjName], [ProjStat], [ActiveTo]

      $('#bindDataTable').DataTable({
        data: response,
        columns: [
          {
            data: 'ProjNo',
            render: function (data, type, row) {
              let txtlink;
              txtlink = `
              <span class="badge rounded-pill text-success me-1" onclick="javascript:EditProject('${data}','${row.ProjName}','${row.ProjStat}','${row.ActiveTo}');" style="cursor:pointer">${data}</span>`;
              return txtlink;
            }
          },
          { data: 'ProjName', className: 'testHidClass' },
          { data: 'ProjStat' },
          {
            data: 'ActiveToTxt',
            render: function (data, type, row) {
              //return `<input type="text" class="rec_eff" value="${data}" data-rowid="${row.wo_stat}">`;
              return `${row.ActiveToTxt}`;
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
function DeleteProject() {
  var ProjNo = $("#ProjNo").val();
  if (ProjNo == '') return;
  if (!confirm('❌ Confirm delete project code "' + ProjNo + '" ❔')) return;
  $.ajax({
    url: "/CTLAdmin/DelProjCode",
    type: "DELETE",
    data: { ProjNo: ProjNo },
    success: function (response) {
      console.log(response);
      location.href = "/CTLAdmin/ProjCodeList";
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
}
function EditProject(ProjNo, ProjName, ProjStat, ActiveTo) {
  let adminRole = $("#adminRole").val();
  if (adminRole != '') {
    document.getElementById("btnProjEdit").style.display = "block";
    document.getElementById("btnProjDel").style.display = "block";
  }
  
  //console.log(ActiveTo);
  $("#ProjNoTmp").val(ProjNo);
  $("#ProjNo").val(ProjNo);
  $("#ProjName").val(ProjName);
  $("#ActiveTo").val(ActiveTo?.split('T')[0]);
  if (ProjStat == 'Active') {
    $('#ProjStat1').prop('checked', true);
  } else {
    $('#ProjStat2').prop('checked', true);
  }
}

function SaveEditAccount() {
  var projStat = '';
  if ($("input[name='ProjStat']:checked").val() == 1) {
    projStat = 'Active';
  } else {
    projStat = 'Closed';
  }

  var ProjNoTmp = $("#ProjNoTmp").val();
  var ProjNo = $("#ProjNo").val();
  var ProjName = $("#ProjName").val();
  var ActiveTo = $("#ActiveTo").val();
  if (ProjNoTmp == '' || ProjNo == '' || ProjName == '') return;
  if (!confirm('Confirm update project code "[' + ProjNo + '] ' + ProjName + '" ❔')) return;
  const obj = {
    ProjNoTmp: ProjNoTmp,
    ProjNo: ProjNo,
    ProjName: ProjName,
    ProjStat: projStat,
    ActiveTo: ActiveTo
  };
  console.log(obj);
  $.ajax({
    url: "/CTLAdmin/SaveProjCode",
    type: "POST",
    data: obj,
    success: function (response) {
      location.href = "/CTLAdmin/ProjCodeList";
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
};

$("#btnSubmit").click(function () {
  var projStat = '';
  if ($("input[name='ProjStat']:checked").val() == 1) {
    projStat = 'Active';
  } else {
    projStat = 'Closed';
  }
  var ProjNo = $("#ProjNo").val();
  var ProjName = $("#ProjName").val();
  if (ProjName == '' || ProjNo == '') return;
  if (!confirm('Confirm add new project code "[' + ProjNo + '] ' + ProjName + '" ❔')) return;
  const obj = {
    ProjNo: ProjNo,
    ProjName: ProjName,
    ProjStat: projStat,
    ActiveTo: $("#ActiveTo").val()
  };
  
  $.ajax({
    url: "/CTLAdmin/AddNewProjCode",
    type: "POST",
    data: obj,
    success: function (response) {
      //console.log(response);
      location.href = "/CTLAdmin/ProjCodeList";
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
