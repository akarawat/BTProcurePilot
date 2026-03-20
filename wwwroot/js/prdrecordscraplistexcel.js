$(document).ready(function () {
  var today = new Date();
  console.log(today);
  document.getElementById("start_dt").value = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
  document.getElementById("end_dt").value = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
  GetProductionScrapRecs($("#start_dt").val(), $("#end_dt").val());
  /*
  $.ajax({
    type: "GET",
    url: "/SGAScrapExcel/GetPRDRecordSheetListExcel",
    success: function (response) {
      console.log(response);
      //ppWo	itemParts	itemName
      $('#bindDataTable').DataTable({
        data: response.data,
        columns: [
          { data: 'rec_date' },
          { data: 'mas_wo' },
          { data: 'mas_itemno' },
          { data: 'mas_itemname' },
          { data: 'mas_opr' },
          { data: 'mas_qty' },
          { data: 'mas_stdtime' },
          { data: 'mas_resource' },
          { data: 'mas_mc' },
          { data: 'mas_lab' },
          { data: 'emp_code' },
          { data: 'rec_setup' },
          { data: 'rec_mc' },
          { data: 'rec_lab' },
          { data: 'rec_aqty' },
          { data: 'rec_atotal' },
          { data: 'rec_eff' }   
        ]
      });

    },
    error: function (response) {
      alert(response.responseText);
    }
  });
  */
});

$("#btnView").click(function () {
  GetProductionScrapRecs($("#start_dt").val(), $("#end_dt").val());
});

function GetProductionScrapRecs(_dt_st, _dt_en) {
  const query = new URLSearchParams({
    dt_st: _dt_st,
    dt_en: _dt_en
  });

  $.ajax({
    type: "GET",
    url: `/SGAScrapExcel/GetExportScrapData?${query.toString()}`,
    success: function (response) {
      console.log(response);
      //ppWo	itemParts	itemName
      if ($.fn.DataTable.isDataTable('#bindDataTable')) {
        $('#bindDataTable').DataTable().clear().destroy();
      }
      $('#bindDataTable').DataTable({
        data: response.data,
        columns: [          
          { data: 'rec_date' },
          { data: 'txt_app1' },
          { data: 'txt_app2' },
          { data: 'txt_app3' },
          { data: 'mas_wo' },
          { data: 'mas_itemno' },
          { data: 'mas_itemname' },
          { data: 'mas_opr' },
          { data: 'emp_code' },
          { data: 'prd_setup' },
          { data: 'prd_tools' },
          { data: 'prd_surf' },
          { data: 'prd_dimout' },
          { data: 'prd_other' },
          { data: 'scrap_remark' },
          { data: 'ven_hardness' },
          { data: 'ven_dimout' },
          { data: 'ven_surf' },
          { data: 'ven_other' },
          { data: 'vendor_remark' },
          { data: 'other_remark' }
        ]
      });

    },
    error: function (response) {
      alert(response.responseText);
    }
  });
}

document.getElementById("btnExport").addEventListener("click", async () => {
  if (confirm("ยืนยันการ Export ข้อมูล")) {
    await exportToExcel();
  }
});

async function exportToExcel() {
  let emp_code = $("#SAMNAME").html();
  let txt_date_st = $("#start_dt").val().slice(-2) + '-' + $("#start_dt").val().slice(5, 7) + '-' + $("#start_dt").val().slice(0, 4);
  let txt_end_dt = $("#end_dt").val().slice(-2) + '-' + $("#end_dt").val().slice(5, 7) + '-' + $("#end_dt").val().slice(0, 4);
  
  const query = new URLSearchParams({
    dt_st: $("#start_dt").val(),
    dt_en: $("#end_dt").val()
  });
  
  const response = await fetch(`/SGAScrapExcel/GetExportScrapData?${query.toString()}`);

  if (!response.ok) {
    alert("Export failed.");
    return;
  }

  const result = await response.json();
  const data = result.data;           // ✅ ดึงข้อมูล
  if (result.data.length == 0) {
    return;
  }

  // กำหนดแถวส่วนหัวเอง $("#start_dt").val(), $("#end_dt").val()
  const customHeaderRows = [
    [`Export By: ${emp_code}`],                // Row 1
    [`Date : ${txt_date_st} to ${txt_end_dt}`],               // Row 2
    [``]     // Row 3
  ];

  // กำหนดชื่อคอลัมน์ Excel (Row 4)
  const headerRow = [
    "Date",
    "App1",
    "App2",
    "App3",
    "WO",
    "Item No.",
    "Item Name.",
    "Operation",
    "Emp Code",
    "prd.setup",
    "prd.tools",
    "prd.surf",
    "prd.dimout",
    "prd.other",
    "scrap remark",
    "ven. hardness",
    "ven. dimout",
    "ven. surf",
    "ven. other",
    "vendor remark",
    "other remark"
  ];

  // แปลงข้อมูล JSON เป็น Array สำหรับ export
  //const dataRows = data.map(row => [
  //  row.mas_wo,
  //  row.mas_itemno,
  //  row.mas_opr,
  //  row.mas_qty,
  //  row.rec_eff,
  //  row.create_dt
  //]);

  //rec_date, mas_wo, mas_itemno, mas_opr, mas_qty, mas_stdtime, mas_resource, mas_mc, mas_lab, emp_code, rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff
  const dataRows = data.map(row => [
    row.rec_date,
    row.txt_app1,
    row.txt_app2,
    row.txt_app3,
    row.mas_wo,
    row.mas_itemno,
    row.mas_itemname,
    row.mas_opr,
    row.emp_code,
    
    row.prd_setup,
    row.prd_tools,
    row.prd_surf,
    row.prd_dimout,
    row.prd_other,
    row.scrap_remark,
    row.ven_hardness,
    row.ven_dimout,
    row.ven_surf,
    row.ven_other,
    row.vendor_remark,
    row.other_remark

  ]);

  // * กรณี อยากใส่ท้ายเอกสาร
  /*
  // ✅ คำนวณรวม mas_qty
  const totalQty = dataRows.reduce((sum, row) => sum + (parseFloat(row.mas_qty) || 0), 0);
  // ✅ แถว Total
  const totalRow = ["", "", "Total", totalQty, "", ""];
  */
  // ✅ แถว Total
  const totalRow = ["", "", "Total", dataRows.length, "รายการ", ""];

  // รวมทั้งหมด
  const finalSheetData = [
    ...customHeaderRows,
    headerRow,
    ...dataRows,
    totalRow  // ✅ ใส่ท้ายสุด
  ];

  // สร้าง worksheet
  const worksheet = XLSX.utils.aoa_to_sheet(finalSheetData);

  // สร้าง workbook
  const workbook = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(workbook, worksheet, "TimeSheet");

  // บันทึกเป็นไฟล์ Excel
  XLSX.writeFile(workbook, `ScrapRreport ${txt_date_st}_${txt_end_dt}.xlsx`);

}
