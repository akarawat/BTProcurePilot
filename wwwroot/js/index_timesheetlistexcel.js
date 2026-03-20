$(document).ready(function () {
  var today = new Date();
  console.log(today);
  let dt_st = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
  let dt_en = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
  GetProductionTimeSheet(dt_st, dt_en);
});
function GetProductionTimeSheet(_dt_st, _dt_en) {
  const query = new URLSearchParams({
    dt_st: _dt_st,
    dt_en: _dt_en
  });

  $.ajax({
    type: "GET",
    url: `/SGAExcel/GetExportTimeSheetDataDash?${query.toString()}`,
    success: function (response) {
      console.log(response);
      //ppWo	itemParts	itemName
      if ($.fn.DataTable.isDataTable('#bindDataTableTimeSheet')) {
        $('#bindDataTableTimeSheet').DataTable().clear().destroy();
      }
      $('#bindDataTableTimeSheet').DataTable({
        data: response.data,
        paging: false,
        info: false,
        searching: false,
        columns: [
          {
            data: null,
            render: function (data, type, row) {
              return `<span>${row.rec_date} <br/>
                  <a href='/SGA/PRDRecordSheet?search=${row.mas_wo}:${row.mas_itemno}'>${row.mas_wo} : ${row.mas_itemno}</a>
                  <br/> ${row.mas_itemname}</span>`;
            }
          },
          {
            data: null,
            render: function (data, type, row) {
              return `<span>Operate: ${row.mas_opr} <br/>Qty: ${row.mas_qty} <br/>Cost: ${row.mas_resource}</span>`;
            }
          }
        ]
      });

    },
    error: function (response) {
      alert(response.responseText);
    }
  });
}
