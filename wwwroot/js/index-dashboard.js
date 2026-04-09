$(document).ready(function () {

  $.ajax({
    type: "GET",
    url: "/Dashboards/GetMyPRReq",
    success: function (response) {
      if (response.length > 0) {
        console.log(response);
        $("#cntDraft").html(response[0].count_draft);
        $("#cntActive").html(response[0].count_ongoing);
        $("#cntApproved").html(response[0].count_approved);

        $('#bindDataTable').DataTable({
          "searching": true,
          //"bLengthChange": false,
          //"dom": 'lfrtip',
          "bLengthChange": false,
          "bFilter": false,
          data: response,
          order: [[0, 'desc']],
          columns: [
            { data: 'create_dt', visible: false },
            {
              data: 'prstatus_txt',
              render: function (data, type, row) {
                let rfq_prefix = "";
                let proj_prefix = "";
                if (row.purpose_type == 3) {
                  rfq_prefix = "(RFQ)";
                }
                if (row.projectno.length >= 3) {
                  proj_prefix = "(P)";
                }
                let color;
                if (row.prstatus == 0) color = 'bg-label-primary';
                else if (row.prstatus == 1) color = 'bg-label-warning';
                else if (row.prstatus == 2 || row.prstatus == 3) color = 'bg-label-info';
                else if (row.prstatus == 5 || row.prstatus == 6) color = 'bg-label-danger';
                else if (row.prstatus == 7) color = 'bg-label-secondary';
                else if (row.prstatus == 9) color = 'bg-label-secondary';
                else color = 'bg-label-success';
                return `<b>PR No:</b> ${rfq_prefix}${proj_prefix}${row.prno} <br/> <b>Req Date:</b> ${row.reqDate_txt} <br/> <span class="badge rounded-pill ${color} me-1" onclick="javascript:OpenDetail('${row.prno}');" style="cursor:pointer">${data} 
                        </span>`;
                //return `${row.prstatus}`;
              }
            },
            {
              render: function (data, type, row) {
                if (row.pr_recvpono != '') {
                  return `<b>PO No:</b> ${row.pr_recvpono} <br/> <b>PO Receive Date:</b> ${row.pr_recvdt_txt}`;
                } else {
                  return `-`;
                }
              }
            },
            //{ data: 'procure_remark', className: 'testHidClass' }
            {
              render: function (data, type, row) {
                let rem_proc = "";
                let rem_approve = "";
                let rem_mgr = "";
                let rem_md = "";
                if (row.procure_remark != '') { rem_proc = '<b>Procurement: </b>' + row.procure_remark + '<br/>'; }
                if (row.remarkEmp != '') { rem_approve = '<b>Approval: </b>' + row.remarkEmp + '<br/>'; }
                if (row.remarkCount != '') { rem_mgr = '<b>MGR: </b>' + row.remarkCount + '<br/>'; }
                if (row.remarkAuth != '') { rem_md = '<b>MD: </b>' + row.remarkAuth; }
                let concat = rem_proc + rem_approve + rem_mgr + rem_md;
                return concat;
              }
            }

          ]
        });
      }
    },
    error: function (response) {
      //alert(response.responseText);
    }
  });

  $.ajax({
    type: "GET",
    url: "/Dashboards/GetMyPRApproval",
    success: function (response) {
      if (response.length > 0) {
        console.log(response);
        $("#cntDraft").html(response[0].count_draft);
        $("#cntActive").html(response[0].count_ongoing);
        $("#cntApproved").html(response[0].count_approved);

        $('#bindDataTableApproval').DataTable({
          "searching": true,
          //"bLengthChange": false,
          //"dom": 'rtip',
          "bLengthChange": false,
          "bFilter": false,
          data: response,
          order: [[0, 'desc']],
          columns: [
            { data: 'create_dt', visible: false },
            {
              data: 'prstatus_txt',
              render: function (data, type, row) {
                let rfq_prefix = "";
                let proj_prefix = "";
                if (row.purpose_type == 3) {
                  rfq_prefix = "(RFQ)";
                }
                if (row.projectno.length >= 3) {
                  proj_prefix = "(P)";
                }
                let color;
                if (row.prstatus == 0) color = 'bg-label-primary';
                else if (row.prstatus == 1) color = 'bg-label-warning';
                else if (row.prstatus == 2 || row.prstatus == 3) color = 'bg-label-info';
                else if (row.prstatus == 5 || row.prstatus == 6) color = 'bg-label-danger';
                else if (row.prstatus == 7) color = 'bg-label-secondary';
                else if (row.prstatus == 9) color = 'bg-label-secondary';
                else color = 'bg-label-success';
                return `<b>PR No:</b>${rfq_prefix}${proj_prefix}${row.prno} <br/> <b>Req Date:</b> ${row.reqDate_txt} <br/><span class="badge rounded-pill ${color} me-1" onclick="javascript:OpenDetail('${row.prno}');" style="cursor:pointer">${data}</span>`;
                //return `${row.prstatus}`;
              }
            },
            {
              render: function (data, type, row) {
                if (row.pr_recvpono != '') {
                  return `<b>PO No:</b> ${row.pr_recvpono} <br/> <b>PO Receive Date:</b> ${row.pr_recvdt_txt}`;
                } else {
                  return `-`;
                }
              }
            },
            {
              render: function (data, type, row) {
                let rem_proc = "";
                let rem_approve = "";
                let rem_mgr = "";
                let rem_md = "";
                let concat = rem_proc + rem_approve + rem_mgr + rem_md;
                return concat;
              }
            },
            {
              render: function (data, type, row) {
                let txt_appEmp = "";
                let txt_appEmp2 = "";
                let txt_countEmp = "";
                let txt_authEmp = "";


                if (row.appEmp != '') {
                  let flagEmp = "";
                  if (row.appFlag == 1) flagEmp = "<b>Approval: </b>Approved"; else flagEmp = "<b>Approval: </b>Pending";
                  txt_appEmp = flagEmp + '<br/>';
                }
                if (row.appEmp2 != '') {
                  let flagEmp2 = "";
                  if (row.appFlag2 == 1) flagEmp2 = "<b>Approval2: </b>Approved"; else flagEmp2 = "<b>Approval2: </b>Pending";
                  txt_appEmp2 = flagEmp2 + '<br/>';
                }
                if (row.countEmp != '') {
                  let flagCount = "";
                  if (row.countFlag == 1) flagCount = "<b>MGR: </b>Approved"; else flagCount = "<b>MGR: </b>Pending";
                  txt_countEmp = flagCount + '<br/>';
                }
                if (row.authEmp != '') {
                  let flagAuth = "";
                  if (row.authFlag == 1) flagAuth = "<b>MD: </b> Approved"; else flagAuth = "<b>MD: </b> Pending";
                  txt_authEmp = flagAuth;
                }

                let concat = txt_appEmp + txt_appEmp2 + txt_countEmp + txt_authEmp;
                return concat;
              }
            }

          ]
        });
      }
    },
    error: function (response) {
      //alert(response.responseText);
    }
  });

});

$("#btnSubmit").click(function () {
  
});

$("#btnModalClose").click(function () {
  location.href = "/";
})
function OpenDetail(prno) {
  location.href = "CTLAdmin/PRResultRequest?docno="+prno;
}
function OpenDetailApproval(prno) {
  location.href = "CTLAdmin/PRApproval?docno=" + prno;
}

