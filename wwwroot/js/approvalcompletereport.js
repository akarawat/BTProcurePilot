// Shows only PRs that have received all approvals required for their amount tier.
// Reuses /CTLAdmin/GetAllPRData (same source as ProcureReport) and filters/sorts
// entirely client-side — no new SP.
//
// Rule (confirmed): appEmp_txt/countFlag_txt/authEmp_txt come back as "Approved"
// only when the corresponding appFlag/countFlag/authFlag = 1 AND the approver
// was actually assigned, so checking for the string "Approved" is equivalent to
// checking the underlying flag.
//   THB:      total_exp <= 2000                          -> Approval 1 only
//             2000 < total_exp <= 60000                   -> + Manager
//             total_exp >= 60000                          -> + MD
//   Non-THB:  total_exp <= 2000                            -> Approval 1 only
//             total_exp > 20000                            -> Approval 1 + Manager + MD
function isApprovalComplete(row) {
  // Exclude RFQ-purpose PRs (rendered with a "(RFQ)" prefix on the PR No.)
  if (row.purpose_type == 3) return false;

  const total = row.total_exp || 0;
  const appOk = row.appEmp_txt === "Approved";
  const countOk = row.countFlag_txt === "Approved";
  const authOk = row.authEmp_txt === "Approved";

  if (row.prcurrency === "THB") {
    if (total <= 2000) return appOk;
    if (total <= 60000) return appOk && countOk;
    return appOk && countOk && authOk;
  }

  // Any currency other than THB (per confirmed spec, includes blank/null)
  if (total <= 2000) return appOk;
  if (total > 20000) return appOk && countOk && authOk;
  return false;
}

$(document).ready(function () {
  $.ajax({
    type: "GET",
    url: "/CTLAdmin/GetAllPRData",
    success: function (response) {
      const rows = (response.data || []).filter(isApprovalComplete);
      renderTable(rows);
    },
    error: function () {
      alert("❌ Error loading report data");
    }
  });
});

function renderTable(rows) {
  $('#bindDataTable').DataTable({
    data: rows,
    order: [[0, 'desc']],
    pageLength: 20,
    columns: [
      {
        // Hidden sortable key: update_dt_txt ("dd/MM/yyyy") -> "yyyy-MM-dd" so
        // the default sort ("เรียงลำดับจาก update_dt ก่อน") sorts chronologically.
        data: null,
        visible: false,
        render: function (data, type, row) {
          const parts = (row.update_dt_txt || '').split('/');
          return parts.length === 3 ? `${parts[2]}-${parts[1]}-${parts[0]}` : '';
        }
      },
      { data: 'create_dt_txt' },
      {
        data: 'prno',
        render: function (data, type, row) {
          let rfq_prefix = row.purpose_type == 3 ? "(RFQ)" : "";
          let proj_prefix = (row.projectno && row.projectno.length >= 3) ? "(P)" : "";
          let revision_txt = row.revision_no > 0 ? ` <span class="text-danger">[Rev:${row.revision_no}]</span>` : "";
          return `<span class="badge rounded-pill text-success me-1" onclick="javascript:OpenDetail('${row.prno}');" style="cursor:pointer">${rfq_prefix}${proj_prefix}${data}</span>${revision_txt}`;
        }
      },
      { data: 'pr_recvpono' },
      { data: 'pr_recvdt_txt' },
      { data: 'update_dt_txt' },
      {
        data: 'prstatus_txt',
        visible: false,
        render: function (data, type, row) {
          return data == "Submited" ? "Pending" : data;
        }
      },
      { data: 'appEmp_txt' },
      { data: 'appEmp2_txt' },
      { data: 'countFlag_txt' },
      { data: 'authEmp_txt' },
      {
        data: 'total_disc',
        render: function (data, type, row) {
          if (type === 'display' && data != null) {
            return parseFloat(data).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
          }
          return data;
        }
      },
      {
        data: 'total_exp',
        render: function (data, type, row) {
          if (type === 'display' && data != null) {
            return parseFloat(data).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
          }
          return data;
        }
      },
      { data: 'prcurrency' },
      { data: 'projectno' },
      {
        data: 'empcode_txt',
        render: function (data, type, row) {
          return `${data} [${row.reqDepCode}]`;
        }
      },
      { data: 'pr_reason' },
      { data: 'approx_dt_txt' }
      
    ]
  });
}

function OpenDetail(prno) {
  location.href = "/CTLAdmin/PRApproval?docno=" + prno;
}

document.getElementById("toggleFullscreenBtn").addEventListener("click", function () {
  const isFull = localStorage.getItem("isFullscreen") === "true";
  localStorage.setItem("isFullscreen", (!isFull).toString());
  window.location.href = "?fullscreen=" + isFull;
});
