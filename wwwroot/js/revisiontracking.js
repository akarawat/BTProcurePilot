$(document).ready(function () {
  const roleAdmin = $("#sUROLEADMIN").val();
  const isProcureUser = roleAdmin == "1" || roleAdmin == "91";

  if (isProcureUser) {
    $.ajax({
      type: "GET",
      url: "/CTLAdmin/GetAllPRData",
      success: function (response) {
        const rows = (response.data || []).filter(r => r.revision_no > 0);
        rows.forEach(r => r.role_txt = "-");
        renderTable(rows);
      },
      error: function () {
        alert("❌ Error loading revision tracking data");
      }
    });
    return;
  }

  // Requester + Approver: fetch both, tag each row with its role, then merge/dedupe by prno
  $.when(
    $.ajax({ type: "GET", url: "/Dashboards/GetMyPRReq" }),
    $.ajax({ type: "GET", url: "/Dashboards/GetMyPRApproval" })
  ).done(function (reqResp, appResp) {
    const myReq = (reqResp[0] || []).filter(r => r.revision_no > 0);
    const myApp = (appResp[0] || []).filter(r => r.revision_no > 0);

    const byPrno = {};
    myReq.forEach(r => {
      byPrno[r.prno] = Object.assign({}, r, { role_txt: "Requester" });
    });
    myApp.forEach(r => {
      if (byPrno[r.prno]) {
        byPrno[r.prno].role_txt = "Requester, Approver";
      } else {
        byPrno[r.prno] = Object.assign({}, r, { role_txt: "Approver" });
      }
    });

    renderTable(Object.values(byPrno));
  }).fail(function () {
    alert("❌ Error loading revision tracking data");
  });
});

function renderTable(rows) {
  $('#bindDataTable').DataTable({
    data: rows,
    order: [[0, 'desc']],
    pageLength: 20,
    columns: [
      { data: 'create_dt', visible: false },
      {
        render: function (data, type, row) {
          let rfq_prefix = row.purpose_type == 3 ? "(RFQ)" : "";
          let proj_prefix = (row.projectno && row.projectno.length >= 3) ? "(P)" : "";
          return `<span class="badge rounded-pill text-success me-1" onclick="javascript:OpenDetail('${row.prno}');" style="cursor:pointer">${rfq_prefix}${proj_prefix}${row.prno}</span>`;
        }
      },
      {
        render: function (data, type, row) {
          return row.empcode_txt || "-";
        }
      },
      { data: 'role_txt' },
      {
        render: function (data, type, row) {
          return row.reqDate_txt || row.create_dt_txt || "-";
        }
      },
      { data: 'prstatus_txt' },
      {
        render: function (data, type, row) {
          return `<span class="text-danger">Rev:${row.revision_no}</span>`;
        }
      },
      {
        render: function (data, type, row) {
          return row.revision_dt_txt || "-";
        }
      }
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
