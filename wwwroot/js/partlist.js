$(document).ready(function () {
  $.ajax({
    type: "GET",
    url: "/PPStock/GetPartList",
    success: function (response) {
      console.log(response);
      $('#bindDataTable').DataTable({
        data: response,
        columns: [
          { data: 'id', className: 'hidNameClass' },
          { data: 'itemno' },
          { data: 'toolsname' },
          { data: 'itemdoc' },
          { data: 'itemname' },
          { data: 'batchno', className: 'hidBatchClass' },
          { data: 'operation' },
          { data: 'locateno' },
          { defaultContent: '<ul class="list-unstyled m-0 avatar-group d-flex align-items-center"><li data-bs-toggle="tooltip" data-popup="tooltip-custom" data-bs-placement="top" class="avatar avatar-xs pull-up" title="Settings"><img src="../img/avatars/setting.jpg" alt="Avatar" class="rounded-circle"></li></ul>' }

        ]
      });

      $('#bindDataTable').on('click', 'ul', function (e) {
        var data = $(this).parents('tr').find('.hidNameClass')[0].innerHTML;
        var dataBatch = $(this).parents('tr').find('.hidBatchClass')[0].innerHTML;
        //console.log(data); 
        //console.log(dataBatch); return;
        if (data == "") return;
        $("#e_id").val(data);
        let obj = {
          id: data,
          batchno: dataBatch
        }
        $.ajax({
          url: "/PPStock/GetPartsManualEdit",
          type: "GET",
          data: obj,
          success: function (response) {
            //console.log(response);
            //console.log(response[0].id);

            $("#e_id").val(data);
            $("#e_avaiphy").val(response[0].avaiphy);
            $("#e_batchno").val(dataBatch);
            $("#e_dimension1").val(response[0].dimension1);
            $("#e_dimension2").val(response[0].dimension2);
            $("#e_par_itemname").val(response[0].docname);
            $("#e_fincostamont").val(response[0].fincostamont);
            $("#e_par_itemdoc").val(response[0].itemdoc);
            $("#e_box_itemlocate").val(response[0].itemlocate);
            $("#e_mas_itemno").val(response[0].itemno);

            $("#e_box_locno").val(response[0].locno);
            $("#e_onlocat").val(response[0].onlocat);
            $("#e_onorder").val(response[0].onorder);
            $("#e_par_operation").val(response[0].operation);
            $("#e_orderreserv").val(response[0].orderreserv);
            $("#e_ordertotal").val(response[0].ordertotal);
            $("#e_phyinventory").val(response[0].phyinventory);
            $("#e_phyreserv").val(response[0].phyreserv);
            $("#e_mas_itemnoname").val(response[0].prodname);
            $("#e_searchname").val(response[0].searchname);
            $("#e_par_toollife").val(response[0].toollife);
            $("#e_totalavailable").val(response[0].totalavailable);
            $("#e_warehouse").val(response[0].warehouse);

          },
          error: function (request, status, error) {
            alert(request.responseText);
          }
        });
        EditManualParts();
        //alert(data);
        //console.log(e.target.closest('tr').find('.testNameClass').val());
        //let data = table.row(e.target.closest('tr')).data();
        //alert(data[0] + "'s salary is: " + data[1]);
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

//$("#btnSubmit").click(function () {
//});

function AddManualParts() {
  var myModal = new bootstrap.Modal(document.getElementById("modalAddManPart"));
  myModal.show();
}
function SaveNewParts() {
  var mas_itemno = $("#mas_itemno").val();
  var mas_itemnoname = $("#mas_itemnoname").val();
  if (mas_itemno == undefined || mas_itemnoname == undefined) return;
  if (!confirm('Confirm save new Partstool, ยืนยันการบันทึก ?')) return;
  let obj = {
    mas_itemno: $("#mas_itemno").val(),
    mas_itemnoname: $("#mas_itemnoname").val(),
    box_locno: $("#box_locno").val(),
    box_itemlocate: $("#box_itemlocate").val(),
    par_itemdoc: $("#par_itemdoc").val(),
    par_itemname: $("#par_itemname").val(),
    par_operation: $("#par_operation").val(),
    par_toollife: $("#par_toollife").val(),
    par_itemqty: 1,
    searchname: $("#searchname").val(),
    dimension1: $("#dimension1").val(),
    dimension2: $("#dimension2").val(),
    batchno: $("#batchno").val(),
    batchno: $("#batchno").val(),
    onlocat: $("#onlocat").val(),
    fincostamont: $("#fincostamont").val(),
    phyinventory: $("#phyinventory").val(),
    phyreserv: $("#phyreserv").val(),
    avaiphy: $("#avaiphy").val(),
    ordertotal: $("#ordertotal").val(),
    onorder: $("#onorder").val(),
    orderreserv: $("#orderreserv").val(),
    totalavailable: $("#totalavailable").val()
  }
  //console.log(obj); //AddnewPartsItem
  $.ajax({
    url: "/PPStock/AddnewPartsItem",
    type: "POST",
    data: obj,
    success: function (response) {
      //console.log(response);
      if (response[1] == "success") {
        alert("บันทึกเสร็จสมบูรณ์");
      }
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
}
function ResetForm() {

}
function EditManualParts() {
  var myModal = new bootstrap.Modal(document.getElementById("modalEditManPart"));
  myModal.show();
}
function ResetEditForm() {

}
function SaveEditParts() {
  var e_id = $("#e_id").val();
  var mas_itemno = $("#e_mas_itemno").val();
  var mas_itemnoname = $("#e_mas_itemnoname").val();
  if (mas_itemno == undefined || mas_itemnoname == undefined) return;
  if (!confirm('Confirm save Edit Manual Partstool, ยืนยันบันทึกการแก้ไข ?')) return;
  let obj = {
    id: e_id,
    itemno: $("#e_mas_itemno").val(),

    locno: $("#e_box_locno").val(),
    itemlocate: $("#e_box_itemlocate").val(),

    itemdoc: $("#e_par_itemdoc").val(),
    docname: $("#e_par_itemname").val(),

    operation: $("#e_par_operation").val(),
    toollife: $("#e_par_toollife").val()

  }
  //console.log(obj); return;
  $.ajax({
    url: "/PPStock/UpdatePartsListTools",
    type: "POST",
    data: obj,
    success: function (response) {
      //console.log(response);
      if (response[1] == "success") {
        alert("บันทึกเสร็จสมบูรณ์");
        window.location.reload();
      }
    },
    error: function (request, status, error) {
      alert(request.responseText);
    }
  });
}
