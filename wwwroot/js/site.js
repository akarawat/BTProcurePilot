// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
const now = new Date();
function formatDateTime(date) {
  const pad = (num) => num.toString().padStart(2, '0');

  const day = pad(date.getDate());
  // Month is 0-indexed, so add 1
  const month = pad(date.getMonth() + 1);
  const year = date.getFullYear();
  const hours = pad(date.getHours());
  const minutes = pad(date.getMinutes());
  const seconds = pad(date.getSeconds());

  return `${day}-${month}-${year} ${hours}${minutes}${seconds}`;
}

// Write your JavaScript code.
$("#btnGotoHome").click(function () {
  //window.document.location.href = "/";
  var win = window.open("about:blank", "_self");
  win.close();
});
function ConfirmLogout() {
  if (!confirm("Confirm to logout?")) return;
  window.open('', '_self').close();
}
function setDateInput(inputId, dateValue) {
  if (dateValue) {
    $('#' + inputId).val(dateValue.split('T')[0]);
  } else {
    $('#' + inputId).val('');
  }
}
function formatNumber(number) {
  return number.toLocaleString('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
}
function formatNumber(value, fix) {
  if (isNaN(value)) return '';

  const rounded = parseFloat(value).toFixed(fix); // ปัดเศษทศนิยมให้เหลือ 4 ตำแหน่ง
  const parts = rounded.split(".");
  const integerPart = parseInt(parts[0]).toLocaleString(); // ใส่ comma separator
  const decimalPart = parts[1];

  return `${integerPart}.${decimalPart}`;
}
function saveEmailLog(mailFrom, mailTo, mailSubject) {
  $.ajax({
    url: '/CTLAdmin/SaveEmailLog',
    type: 'PUT',
    contentType: 'application/json',
    data: JSON.stringify({
      mailFrom: mailFrom,
      mailTo: mailTo,
      subject: mailSubject
    }),
    success: function (res) {
      console.log("Email log saved.");
    },
    error: function (err) {
      console.error("Log error:", err);
    }
  });
}
