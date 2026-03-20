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
    url: "/CTLAdmin/GetItempartList",
    success: function (response) {
      //console.log(response);
      //VenName, VenCode, Vencurrency

      $('#bindDataTable').DataTable({
        data: response,
        columns: [
          {
            data: 'itemno',
            render: function (data, type, row) {
              let txtlink;
              txtlink = `
              <span class="badge rounded-pill text-success me-1" onclick="javascript:EditItempart('${data}','${row.itemname}','${row.itemdoc}');" style="cursor:pointer">${data}</span>`;
              return txtlink;
            }
          },
          { data: 'itemname', className: 'testHidClass' },
          {
            data: 'itemdoc',
            render: function (data, type, row) {
              return `${data}`;
            }
          }

        ]
      });

    },
    error: function (response) {
      alert(response.responseText);
    }
  });
});

$("#btnModalClose").click(function () {
  location.href = "/";
})

function DeleteItem() {
  
  var itemno = $("#itemno").val();
  var itemname = $("#itemname").val();
  if (itemno == '') return;
  if (!confirm('❌ Confirm delete vendor code "[' + itemno + '] ' + itemname + '" ❔')) return;
  $.ajax({
    url: "/CTLAdmin/DelItempart",
    type: "DELETE",
    data: { itemno: itemno },
    success: function (response) {
      console.log(response);
      location.href = "/CTLAdmin/ItemPartList";
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
}
function EditItempart(itemno, itemname, itemdoc) {
  document.getElementById("btnItemEdit").style.display = "block";
  document.getElementById("btnItemDel").style.display = "block";
  //console.log(ActiveTo);
  $("#itemnoTmp").val(itemno);
  $("#itemno").val(itemno);
  $("#itemname").val(itemname);
  $("#itemdoc").val(itemdoc);
}

function SaveEditItem() {
  var itemnoTmp = $("#itemnoTmp").val();
  var itemno = $("#itemno").val();
  var itemname = $("#itemname").val();
  var itemdoc = $("#itemdoc").val();
  if (itemnoTmp == '' || itemno == '' || itemname == '') return;
  if (!confirm('Confirm update vendor code "[' + itemno + '] ' + itemname + '" ❔')) return;
  const obj = {
    itemnoTmp: itemnoTmp,
    itemno: itemno,
    itemname: itemname,
    itemdoc: itemdoc
  };
  console.log(obj);
  $.ajax({
    url: "/CTLAdmin/SaveItempart",
    type: "POST",
    data: obj,
    success: function (response) {
      location.href = "/CTLAdmin/ItemPartList";
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
};

$("#btnSubmit").click(function () {
  var itemno = $("#itemno").val();
  var itemname = $("#itemname").val();
  var itemdoc = $("#itemdoc").val();
  if (itemno == '' || itemname == '' || itemdoc == '') return;
  if (!confirm('Confirm add new BT. Item Part "[' + itemno + '] ' + itemname + '" ❔')) return;
  const obj = {
    itemno: itemno,
    itemname: itemname,
    itemdoc: itemdoc
  };

  $.ajax({
    url: "/CTLAdmin/AddNewBTItempart",
    type: "POST",
    data: obj,
    success: function (response) {
      //console.log(response);
      location.href = "/CTLAdmin/ItemPartList";
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
