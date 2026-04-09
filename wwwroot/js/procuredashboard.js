let prChart;
let interval;
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
  $.ajax({
    url: '/CTLAdmin/GetPRSummaryKPI',
    type: 'GET',
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

  $.ajax({
    url: '/CTLAdmin/GetPRMonthlySummary',
    type: 'GET',
    success: function (data) {

      let labels = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
      let values = new Array(12).fill(0);

      data.forEach(item => {
        values[item.month_no - 1] = item.total_pr;
      });

      new Chart(document.getElementById('prMonthlyChart'), {
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
  $.get('/CTLAdmin/GetPRStatusSummary', function (res) {

    new Chart(document.getElementById("donutChart"), {
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
  $.get('/CTLAdmin/GetPRDailyTrend', function (data) {

    let labels = data.map(x => x.date);
    let values = data.map(x => x.total);

    new Chart(document.getElementById("lineChart"), {
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
  $.get('/CTLAdmin/GetTopDepartment', function (data) {

    let labels = data.map(x => 'Dep [' + x.dept + ']');
    let values = data.map(x => x.total);

    new Chart(document.getElementById("deptChart"), {
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
