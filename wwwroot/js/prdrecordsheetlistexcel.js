$(document).ready(function () {
  var today = new Date();
  console.log(today);
  document.getElementById("start_dt").value = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
  document.getElementById("end_dt").value = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
  GetProductionTimeSheet($("#start_dt").val(), $("#end_dt").val());
  /*
  $.ajax({
    type: "GET",
    url: "/SGAExcel/GetPRDRecordSheetListExcel",
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
  GetProductionTimeSheet($("#start_dt").val(), $("#end_dt").val());
});

function GetProductionTimeSheet(_dt_st, _dt_en) {
  const query = new URLSearchParams({
    dt_st: _dt_st,
    dt_en: _dt_en
  });

  $.ajax({
    type: "GET",
    url: `/SGAExcel/GetExportTimeSheetData?${query.toString()}`,
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
          /*
          { data: 'mas_wo' },
          { data: 'mas_opr' },
          { data: 'mas_resource' },

          { data: 'rec_setup' },
          { data: 'rec_mc' },
          { data: 'rec_lab' },
          { data: 'rec_aqty' }*/
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
  
  const response = await fetch(`/SGAExcel/GetExportTimeSheetData?${query.toString()}`);

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
  //- น้องเตยขอปิดบางเซล
  /*
  const headerRow = [
    "Date",
    "WO",
    "Item No.",
    "Item Name.",
    "Operation",
    "Qty.",
    " ",
    "Std. Time.",
    "Resource.",
    "MC.",
    "LAB.",
    "Emp Code",
    " ",
    "[Setup",
    "Machine",
    "Lab",
    "aqty",
    "atotal]",
    "eff"
  ];
  */
  const headerRow = [
    "WO",
    "Operation",
    "Resource.",
    "Setup",
    "Machine",
    "Lab",
    "aqty"
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
  /* น้องเตย ขอปิดบางเซล
  const dataRows = data.map(row => [
    row.rec_date,
    row.mas_wo,
    row.mas_itemno,
    row.mas_itemname,
    row.mas_opr,
    row.mas_qty,
    '',
    row.mas_stdtime,
    row.mas_resource,
    row.mas_mc,
    row.mas_lab,
    row.emp_code,
    '',
    row.rec_setup,
    row.rec_mc,
    row.rec_lab,
    row.rec_aqty,
    row.rec_atotal,
    row.rec_eff
  ]);
  */
  const dataRows = data.map(row => [
    
    row.mas_wo,
    row.mas_opr,
    row.mas_resource,
    row.rec_setup,
    row.rec_mc,
    row.rec_lab,
    row.rec_aqty
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
  XLSX.writeFile(workbook, `TimeSheet ${txt_date_st}_${txt_end_dt}.xlsx`);

}
