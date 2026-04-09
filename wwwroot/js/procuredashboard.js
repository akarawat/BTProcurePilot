let prChart;
let interval;

let lineChart;
let donutChart;
let monthlyChart;
let deptChart;

$(document).ready(function () {
  loadDashboard();
  loadMonthlyChart();

  loadDonutChart();
  loadLineChart();
  loadDeptChart();
});
function startAutoRefresh() {
  interval = setInterval(function () {
    loadDashboard();
    loadMonthlyChart();

    loadDonutChart();
    loadLineChart();
    loadDeptChart();
  }, 60000);
}

function loadDashboard() {
  let filter = getFilter();
  $.ajax({
    url: '/CTLAdmin/GetPRSummaryKPI',
    type: 'GET',
    data: filter,
    success: function (res) {

      $("#total_pr").text(res.total_pr);
      $("#open_pr").text(res.open_pr);
      $("#closed_pr").text(res.closed_pr);
      $("#yield_percent").text(res.yield_percent + " %");

    },
    error: function () {
      alert("❌ Failed to load dashboard data");
    }
  });
}
function loadMonthlyChart() {
  let filter = getFilter();
  $.ajax({
    url: '/CTLAdmin/GetPRMonthlySummary',
    type: 'GET',
    data: filter,
    success: function (data) {

      let labels = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
      let values = new Array(12).fill(0);

      data.forEach(item => {
        values[item.month_no - 1] = item.total_pr;
      });
      if (monthlyChart) {
        monthlyChart.destroy();
      }
      monthlyChart = new Chart(document.getElementById('prMonthlyChart'), {
        type: 'bar',
        data: {
          labels: labels,
          datasets: [{
            label: 'Total PR (Yearly)',
            data: values
          }]
        },
        options: {
          plugins: {
            datalabels: {
              anchor: 'end',
              align: 'top',
              formatter: (value) => value
            }
          }
        },
        plugins: [ChartDataLabels]
      });

    }
  });
}
function loadDonutChart() {

  let filter = getFilter();
  $.get('/CTLAdmin/GetPRStatusSummary', filter, function (res) {
    if (donutChart) {
      donutChart.destroy();
    }

    donutChart = new Chart(document.getElementById("donutChart"), {
      type: 'doughnut',
      data: {
        labels: ["Open", "Closed"],
        datasets: [{
          data: [res.open_pr, res.closed_pr]
        }]
      },
      options: {
        plugins: {
          datalabels: {
            formatter: (value, ctx) => {
              let sum = ctx.chart.data.datasets[0].data.reduce((a, b) => a + b, 0);
              let percent = (value * 100 / sum).toFixed(1) + "%";
              return value + " (" + percent + ")";
            },
            font: {
              weight: 'bold'
            }
          }
        }
      },
      plugins: [ChartDataLabels]
    });

  });
}
function loadLineChart() {
  let filter = getFilter();
  $.get('/CTLAdmin/GetPRDailyTrend', filter, function (data) {

    let labels = data.map(x => x.date);
    let values = data.map(x => x.total);

    if (lineChart) {
      lineChart.destroy();
    }

    lineChart = new Chart(document.getElementById("lineChart"), {
      type: 'line',
      data: {
        labels: labels,
        datasets: [{
          label: "PR per Day",
          data: values,
          fill: false
        }]
      },
      options: {
        plugins: {
          datalabels: {
            align: 'top',
            formatter: (value) => value
          }
        }
      },
      plugins: [ChartDataLabels]
    });

  });
}
function loadDeptChart() {
  let filter = getFilter();
  $.get('/CTLAdmin/GetTopDepartment', filter, function (data) {

    let labels = data.map(x => 'Dep [' + x.dept + ']');
    let values = data.map(x => x.total);
    if (deptChart) {
      deptChart.destroy();
    }
    deptChart = new Chart(document.getElementById("deptChart"), {
      type: 'bar',
      data: {
        labels: labels,
        datasets: [{
          label: "PR Count with Department",
          data: values
        }]
      },
      options: {
        plugins: {
          datalabels: {
            anchor: 'end',
            align: 'top',
            formatter: (value) => value
          }
        }
      },
      plugins: [ChartDataLabels]
    });

  });
}
// Toggle full screen
document.getElementById("toggleFullscreenBtn").addEventListener("click", function () {
  const isFull = localStorage.getItem("isFullscreen") === "true";
  localStorage.setItem("isFullscreen", (!isFull).toString());
  // Reload เพื่อใช้ Razor Layout ใหม่
  window.location.href = "?fullscreen=" + isFull;

});
$("#btnSearch").click(function () {
  reloadAll();
});
$("#btnClear").click(function () {
  $("#start_dt").val('');
  $("#end_dt").val('');
  reloadAll();
});
function getFilter() {
  return {
    start_dt: $("#start_dt").val(),
    end_dt: $("#end_dt").val()
  };
}
function reloadAll() {
  loadDashboard();
  loadMonthlyChart();
  loadDonutChart();
  loadLineChart();
  loadDeptChart();
}

$("#btnExport").click(function () {
  exportDashboard();
});

async function exportDashboard() {

  let filter = getFilter();

  const res = await fetch(`/CTLAdmin/GetPRMonthlySummary?start_dt=${filter.start_dt}&end_dt=${filter.end_dt}`);
  const data = await res.json();

  let rows = [];

  rows.push(["PR Dashboard Report"]);
  rows.push(["Date:", new Date().toLocaleString()]);
  rows.push([""]);

  rows.push(["Month", "Total PR"]);

  data.forEach(x => {
    rows.push([x.month_name, x.total_pr]);
  });

  const ws = XLSX.utils.aoa_to_sheet(rows);
  const wb = XLSX.utils.book_new();

  XLSX.utils.book_append_sheet(wb, ws, "Monthly PR");

  XLSX.writeFile(wb, "PR_Dashboard_Report.xlsx");
}
