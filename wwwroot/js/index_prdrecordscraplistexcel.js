$(document).ready(function () {
  var today = new Date();
  console.log(today);
  let dt_st = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
  let dt_en = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
  GetProductionScrapRecs(dt_st, dt_en);
});

function GetProductionScrapRecs(_dt_st, _dt_en) {
  const query = new URLSearchParams({
    dt_st: _dt_st,
    dt_en: _dt_en
  });

  $.ajax({
    type: "GET",
    url: `/SGAScrapExcel/GetExportScrapDataDash?${query.toString()}`,
    success: function (response) {
      console.log(response);
      //ppWo	itemParts	itemName
      if ($.fn.DataTable.isDataTable('#bindDataTableScrap')) {
        $('#bindDataTableScrap').DataTable().clear().destroy();
      }
      $('#bindDataTableScrap').DataTable({
        data: response.data,
        paging: false,
        info: false,
        searching: false,
        columns: [          
          //{ data: 'rec_date' },
          //{ data: 'txt_app1' },
          //{ data: 'txt_app2' },
          //{ data: 'txt_app3' },
          //{ data: 'mas_wo' },
          //{ data: 'mas_itemno' },
          //{ data: 'mas_itemname' },
          //{ data: 'mas_opr' },
          //{ data: 'emp_code' },
          //{ data: 'prd_setup' },
          //{ data: 'prd_tools' },
          //{ data: 'prd_surf' },
          //{ data: 'prd_dimout' },
          //{ data: 'prd_other' },
          //{ data: 'scrap_remark' },
          //{ data: 'ven_hardness' },
          //{ data: 'ven_dimout' },
          //{ data: 'ven_surf' },
          //{ data: 'ven_other' },
          //{ data: 'vendor_remark' },
          //{ data: 'other_remark' },
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
              return `<span>Remarks: ${row.scrap_remark} <br/>${row.vendor_remark} <br/>${row.other_remark}</span>`;
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
