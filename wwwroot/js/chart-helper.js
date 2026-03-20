$(document).ready(function () {
  
  $.ajax({
    url: "/Dashboards/GetLowStockItems_RadarChart",
    method: "GET",
    success: function (response) {
      //console.log(response);
      const labels = response.data.map(x => x.Itemno);
      const diffQty = response.data.map(x => x.DiffQty);
      //$("#lastUpdateLabel").html(" Updated on: " + response.dateUpdated);
      $("#lastUpdate").html(" on: " + response.dateUpdated);

      //const labels = data.map(x => x.Itemno);     // แก้ให้ตรง case
      //const diffQty = data.map(x => x.DiffQty);   // แก้ให้ตรง case

      const ctx = document.getElementById("radarChart").getContext("2d");

      if (ctx.radarChart) {
        ctx.radarChart.destroy();
      }
      ctx.radarChart = 

      new Chart(ctx, {
        type: "radar",
        data: {
          labels: labels,
          datasets: [{
            label: "Shortage (Qty)",
            data: diffQty,
            backgroundColor: "rgba(255, 99, 132, 0.2)",
            borderColor: "rgba(255, 99, 132, 1)",
            pointBackgroundColor: "rgba(255, 99, 132, 1)",
            borderWidth: 2
          }]
        },
        options: {
          responsive: true,
          scales: {
            r: {
              beginAtZero: false, // ✅ ให้สามารถแสดงค่าติดลบได้
              suggestedMin: Math.min(...diffQty) - 10,
              suggestedMax: 10,
              ticks: {
                stepSize: 10,
                callback: function (value) {
                  return value + " qty";
                }
              }
            }
          },
          plugins: {
            tooltip: {
              callbacks: {
                label: function (context) {
                  return `Diff: ${context.raw} units`;
                }
              }
            }
          }
        }
      });
    },
    error: function (err) {
      console.error("Error loading radar chart data:", err);
    }
  });
});

function exportToExcel() {
  $.ajax({
    url: "/Dashboards/GetLowStockItems_RadarChart",
    method: "GET",
    success: function (response) {
      const worksheet = XLSX.utils.json_to_sheet(response.data);
      const workbook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(workbook, worksheet, "LowStockItems");

      // Save
      XLSX.writeFile(workbook, "LowStockItems_" + response.dateUpdated.replace(/\//g, '-') + ".xlsx");
    },
    error: function (err) {
      console.error("Error exporting to Excel:", err);
    }
  });
}
