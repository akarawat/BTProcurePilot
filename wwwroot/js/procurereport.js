$(document).ready(function () {

  fetch('/CTLAdmin/GetAllPRData').then(r => r.json()).then(d => {
    const data = d.data;
    console.log("Total rows:", data.length);

    // นับตาม prstatus_txt
    const byStatus = {};
    data.forEach(r => { byStatus[r.prstatus_txt] = (byStatus[r.prstatus_txt] || 0) + 1; });
    console.log("--- by prstatus_txt ---");
    console.table(byStatus);

    // นับ rows ที่มี field ใดๆ ใน 4 ตัวนี้ = "Pending"
    const pendingApprover = data.filter(r =>
      r.appEmp_txt === "Pending" || r.appEmp2_txt === "Pending" ||
      r.countFlag_txt === "Pending" || r.authEmp_txt === "Pending"
    );
    console.log("Rows with any approver = 'Pending':", pendingApprover.length);

    // ตัวอย่าง 5 แถวแรกของ pendingApprover เพื่อดู prstatus_txt คู่กัน
    console.log("--- sample pendingApprover rows ---");
    console.table(pendingApprover.slice(0, 10).map(r => ({
      prno: r.prno, prstatus_txt: r.prstatus_txt, codelog: r.codelog,
      appEmp_txt: r.appEmp_txt, appEmp2_txt: r.appEmp2_txt,
      countFlag_txt: r.countFlag_txt, authEmp_txt: r.authEmp_txt
    })));
  });

  $('#bindDataTable').DataTable({
    ajax: {
      url: '/CTLAdmin/GetAllPRData',
      dataSrc: function (json) {
        //console.log("Full JSON Response:", json); // This works perfectly here
        return json.data;
      }
    },
    order: [[0, 'desc']],
    pageLength: 20,
    columns: [
      { data: 'create_dt', visible: false },
      { data: 'create_dt_txt' },
      {
        data: 'prno',
        render: function (data, type, row) {
          let rfq_prefix = row.purpose_type == 3 ? "(RFQ)" : "";
          let proj_prefix = (row.projectno && row.projectno.length >= 3) ? "(P)" : "";
          return `<span class="badge rounded-pill text-success me-1" onclick="javascript:OpenDetail('${row.prno}');" style="cursor:pointer">${rfq_prefix}${proj_prefix}${data}</span>`;
        }
      },
      {
        data: 'prstatus_txt',
        render: function (data, type, row) {
          return data == "Submited" ? "Pending" : data;
        }
      },

      // --- FIXED SECTION BELOW ---
      { data: 'appEmp_txt' },      // Added 'data:'
      { data: 'appEmp2_txt' },     // Added 'data:'
      { data: 'countFlag_txt' },   // Added 'data:'
      { data: 'authEmp_txt' },     // Added 'data:'
      
      {
        data: 'total_disc',
        render: function (data, type, row) {
          if (type === 'display' && data != null) {
            // Converts 1250.5 to "1,250.50"
            return parseFloat(data).toLocaleString('en-US', {
              minimumFractionDigits: 2,
              maximumFractionDigits: 2
            });
          }
          return data;
        }
      },
      {
        data: 'total_exp',
        render: function (data, type, row) {
          if (type === 'display' && data != null) {
            // Converts 1250.5 to "1,250.50"
            return parseFloat(data).toLocaleString('en-US', {
              minimumFractionDigits: 2,
              maximumFractionDigits: 2
            });
          }
          return data;
        }
      },
      { data: 'prcurrency' },       // Added 'data:'
      // ---------------------------

      { data: 'projectno' },
      {
        data: 'empcode_txt',
        render: function (data, type, row) {
          return `${data} [${row.reqDepCode}]`;
        }
      },
      { data: 'pr_reason' },
      { data: 'approx_dt_txt' },
      { data: 'update_dt_txt' },
      { data: 'pr_recvdt_txt' },
      { data: 'pr_recvpono' }
    ]
  });
   
  $("#id_lbl_submited").html("USER <br/> SUBMITED ");
  $("#id_lbl_inprocess").html("APPROVAL INPROCESS ");
  $("#id_lbl_procure_rec").html("PROCUREMENT RECEIVED ");


  $("#id_lbl_all_pr").html("PR. INCOMPLETE");
  $("#id_lbl_completed").html("PR. COMPLETED");
  $("#id_lbl_rfq").html("ALL RFQ. ");
  let lbl_perc = 0;
  lbl_perc = ($("#id_completed").html() / $("#id_all_pr").html()) * 100;
  $("#lbl_perc").html(formatNumberPrn(lbl_perc, 2) + " % 🚀");

});
function OpenDetail(prno) {
  location.href = "/CTLAdmin/PRApproval?docno=" + prno;
}

// Toggle full screen
document.getElementById("toggleFullscreenBtn").addEventListener("click", function () {
  const isFull = localStorage.getItem("isFullscreen") === "true";
  localStorage.setItem("isFullscreen", (!isFull).toString());
  // Reload เพื่อใช้ Razor Layout ใหม่
  window.location.href = "?fullscreen=" + isFull;

});

function formatNumberPrn(number) {
  return number.toLocaleString('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
}

document.getElementById("btnExport").addEventListener("click", async () => {
  if (confirm("ยืนยันการ Export ข้อมูล")) {
    await exportToExcel();
  }
});
const formattedDateTime = formatDateTime(now);
async function exportToExcel() {
  let emp_code = 'Procurement';

  const response = await fetch(`/CTLAdmin/GetAllPRData`);
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
    [`Date : ${formattedDateTime}`],               // Row 2
    [``]     // Row 3
  ];

  const headerRow = [
    "Req. Date",
    "PR No.",
    "PR Status",

    "Approval 1",
    "Approval 2",
    "MGR.",
    "MD.",

    "Total Discount",
    "Total",
    "Currency",
    "Proj No.",
    "Emp. Req.",

    "Reason",
    "Approx Date",
    "Update Date",
    "Procure Recv.",
    "PO. No."
  ];

  //-> preprocess
  const formattedPRNo = (row) => {
    let rfq = row.purpose_type == 3 ? "(RFQ)" : "";
    let proj = (row.projectno && row.projectno.length >= 3) ? "(P)" : "";
    return `${rfq}${proj}${row.prno}`;
  };
  const formattedTotalExp = (row) => {
    return parseFloat(row.total_exp).toLocaleString('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });
  };
  const formattedEmpTxt = (row) => {
    return `${row.empcode_txt} [${row.reqDepCode}]`;
  };

  const dataRows = data.map(row => [
    row.create_dt_txt,
    formattedPRNo(row), 
    row.prstatus_txt,

    row.appEmp_txt,
    row.appEmp2_txt,
    row.countFlag_txt,
    row.authEmp_txt,

    row.total_disc,
    formattedTotalExp(row),
    row.prcurrency,
    row.projectno,
    formattedEmpTxt(row),

    row.pr_reason,
    row.approx_dt_txt,
    row.update_dt_txt,
    row.pr_recvdt_txt,
    row.pr_recvpono
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
  XLSX.utils.book_append_sheet(workbook, worksheet, "PR Report");

  // บันทึกเป็นไฟล์ Excel
  XLSX.writeFile(workbook, `Procure PR Report ${formattedDateTime}.xlsx`);

}
