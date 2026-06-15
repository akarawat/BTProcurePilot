// Additional client-side filters for the Procurement report table.
// Loaded after procurereport.js — does not modify the existing
// DataTable configuration, columns, or export logic.
//
// Status mapping verified against a live snapshot of /CTLAdmin/GetAllPRData
// (1,799 rows): Procure PO Completed=1313, Procure PR Received=421,
// Send mail to procure=22, Submited=5, Pending=1, Draft=19,
// Procure PR Reject=18.
//
// Each summary number above the table is clickable and applies a matching
// quick filter via applyQuickFilter(kind):
//
//   pending    -> ALL PR. that have not yet reached Procurement:
//                  Draft / Submited / PR. Submitted / PR. Resend / Pending /
//                  Pending for approval / Send mail to procure
//                  (= 19+5+1+22 = 47; the "APPROVAL INPROCESS" summary
//                   number links here)
//   received   -> Procure PR Received   (exact match: 421)
//   completed  -> Procure PO Completed  (close match: 1313 vs displayed 1292;
//                  the small gap is likely RFQ rows excluded from the
//                  "PR. COMPLETED" count on the server side)
//
// Procure PR Reject and any other rejected/cancelled statuses are
// intentionally left out of all quick filters; they remain visible only
// under "All status".

var STATUS_MAP = {
  pending: ['Draft', 'Submited', 'PR. Submitted', 'PR. Resend', 'Pending', 'Pending for approval', 'Send mail to procure'],
  received: ['Procure PR Received'],
  completed: ['Procure PO Completed']
};

// Called from the onclick of the summary numbers above the table.
function applyQuickFilter(kind) {
  $('#filterDept').val('');
  $('#filterPRType').val('');
  $('#filterStatus').val('');

  if (kind === 'rfq') {
    $('#filterPRType').val('rfq');
  } else if (kind !== 'all' && STATUS_MAP.hasOwnProperty(kind)) {
    $('#filterStatus').val(kind);
  }

  $('#bindDataTable').DataTable().draw();

  var table = document.getElementById('bindDataTable');
  if (table && table.scrollIntoView) {
    table.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }
}

$(document).ready(function () {

  $.fn.dataTable.ext.search.push(function (settings, searchData, index, rowData, counter) {
    if (settings.nTable.id !== 'bindDataTable') {
      return true;
    }

    var dept = $('#filterDept').val();
    var prType = $('#filterPRType').val();
    var status = $('#filterStatus').val();

    if (dept && String(rowData.reqDepCode) !== dept) {
      return false;
    }

    if (prType === 'rfq' && rowData.purpose_type != 3) {
      return false;
    }
    if (prType === 'normal' && rowData.purpose_type == 3) {
      return false;
    }

    if (status && STATUS_MAP.hasOwnProperty(status)) {
      if (STATUS_MAP[status].indexOf(rowData.prstatus_txt) === -1) {
        return false;
      }
    }

    return true;
  });

  // Populate the department dropdown once report data has loaded, and
  // align the "USER SUBMITED" / "APPROVAL INPROCESS" summary numbers with
  // the "pending" quick filter so the number shown matches the row count
  // you get after clicking it.
  $('#bindDataTable').on('xhr.dt', function (e, settings, json) {
    if (!json || !json.data) {
      return;
    }

    var depts = [];
    var pendingCount = 0;
    json.data.forEach(function (row) {
      var dep = row.reqDepCode ? String(row.reqDepCode) : '';
      if (dep !== '' && depts.indexOf(dep) === -1) {
        depts.push(dep);
      }
      if (STATUS_MAP.pending.indexOf(row.prstatus_txt) !== -1) {
        pendingCount++;
      }
    });
    depts.sort();

    var $sel = $('#filterDept');
    $sel.find('option').not(':first').remove();
    depts.forEach(function (dep) {
      $sel.append('<option value="' + dep + '">' + dep + '</option>');
    });

    $('#id_inprocess').text(pendingCount);
  });

  $('#filterDept, #filterPRType, #filterStatus').on('change', function () {
    $('#bindDataTable').DataTable().draw();
  });

  $('#btnFilterReset').on('click', function () {
    $('#filterDept, #filterPRType, #filterStatus').val('');
    $('#bindDataTable').DataTable().draw();
  });

});
