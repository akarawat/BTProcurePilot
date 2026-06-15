using AspnetCoreMvcFull.Models;
using Azure;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AspnetCoreMvcFull.Controllers;

public class CTLAdminController : Controller
{
  //public DataAccess daAccess = new DataAccess();
  //PRDPRDExcelImport, PRDRecordSheetList
  //public IActionResult PRDRecordSheet() => View();
  private readonly IWebHostEnvironment _env;
  private readonly IConfiguration _config;
  DataAccess daAccess = new DataAccess();
  [HttpGet]
  public IActionResult AccCodeList()
  {
    //List<string> lstStr = daAccess.GetOpenOrCloseDoc();
    //ViewData["OPENDOC"] = lstStr[0];
    //ViewData["CLOSEDOC"] = lstStr[1];
    if (HttpContext.Session.GetString(SessionModel.UROLEADMIN) != null)
    {
      ViewData["UROLEADMIN"] = HttpContext.Session.GetString(SessionModel.UROLEADMIN);
    }

    return View();
  }
  [HttpGet]
  public IActionResult ProcureReport(string? search)
  {
    var isFullscreen = Request.Query["fullscreen"]; // หรือ TempData
    ViewData["isFullscreen"] = isFullscreen.ToString().ToLower();

    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int submited = 0;
    int inprocess = 0;
    int procure_rec = 0;
    int all_pr = 0;
    int completed = 0;
    int rfqcount = 0;
    int rfqcount_quo = 0;

    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_GetProcureRPTOut", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.Add("@submited", SqlDbType.Int); cmnd.Parameters["@submited"].Direction = ParameterDirection.Output;
      cmnd.Parameters.Add("@inprocess", SqlDbType.Int); cmnd.Parameters["@inprocess"].Direction = ParameterDirection.Output;
      cmnd.Parameters.Add("@procure_rec", SqlDbType.Int); cmnd.Parameters["@procure_rec"].Direction = ParameterDirection.Output;
      cmnd.Parameters.Add("@all_pr", SqlDbType.Int); cmnd.Parameters["@all_pr"].Direction = ParameterDirection.Output;
      cmnd.Parameters.Add("@completed", SqlDbType.Int); cmnd.Parameters["@completed"].Direction = ParameterDirection.Output;
      cmnd.Parameters.Add("@rfqcount", SqlDbType.Int); cmnd.Parameters["@rfqcount"].Direction = ParameterDirection.Output;
      cmnd.Parameters.Add("@rfqcount_quo", SqlDbType.Int); cmnd.Parameters["@rfqcount_quo"].Direction = ParameterDirection.Output;

      cmnd.ExecuteNonQuery();
      submited = Convert.ToInt32(cmnd.Parameters["@submited"].Value);
      inprocess = Convert.ToInt32(cmnd.Parameters["@inprocess"].Value);
      procure_rec = Convert.ToInt32(cmnd.Parameters["@procure_rec"].Value);
      all_pr = Convert.ToInt32(cmnd.Parameters["@all_pr"].Value);
      completed = Convert.ToInt32(cmnd.Parameters["@completed"].Value);
      rfqcount = Convert.ToInt32(cmnd.Parameters["@rfqcount"].Value);
      rfqcount_quo = Convert.ToInt32(cmnd.Parameters["@rfqcount_quo"].Value);
    }
    catch (Exception ex)
    {
      
    }
    finally
    {
      ViewData["submited"] = submited;
      ViewData["inprocess"] = inprocess;
      ViewData["procure_rec"] = procure_rec;
      ViewData["all_pr"] = all_pr;
      ViewData["completed"] = completed;
      ViewData["rfqcount"] = rfqcount;
      ViewData["rfqcount_quo"] = rfqcount_quo;
    }
    con.Close();
    string txtEMPCODE = HttpContext.Session.GetString(SessionModel.EMPCODE);
    List<string> lstPending = daAccess.GetPendingApprove(txtEMPCODE);
    ViewData["PENDING"] = lstPending[0];

    return View();
  }
  [HttpGet]
  public IActionResult ProcureDashboard(string? search)
  {
    var isFullscreen = Request.Query["fullscreen"]; // หรือ TempData
    ViewData["isFullscreen"] = isFullscreen.ToString().ToLower();

    string txtEMPCODE = HttpContext.Session.GetString(SessionModel.EMPCODE);
    List<string> lstPending = daAccess.GetPendingApprove(txtEMPCODE);
    ViewData["PENDING"] = lstPending[0];
    return View();
  }
  [HttpGet]
  public JsonResult GetPRSummaryKPI(string start_dt, string end_dt, string dept)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    string connStr = config.GetConnectionString("BtProcureConn");

    PRSummaryKPIModel result = new PRSummaryKPIModel();
    DateTime? start = string.IsNullOrEmpty(start_dt) ? null : Convert.ToDateTime(start_dt);
    DateTime? end = string.IsNullOrEmpty(end_dt) ? null : Convert.ToDateTime(end_dt);

    using (SqlConnection con = new SqlConnection(connStr))
    {
      con.Open();
      SqlCommand cmd = new SqlCommand("SP_GetPRSummaryKPI", con);
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.Parameters.Add(new SqlParameter("@start_dt", SqlDbType.DateTime) { Value = (object)start ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@end_dt", SqlDbType.DateTime) { Value = (object)end ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@dept", SqlDbType.NVarChar, 50) { Value = string.IsNullOrEmpty(dept) ? (object)DBNull.Value : dept });

      using (SqlDataReader rd = cmd.ExecuteReader())
      {
        if (rd.Read())
        {
          result.total_pr = Convert.ToInt32(rd["total_pr"]);
          result.open_pr = Convert.ToInt32(rd["open_pr"]);
          result.closed_pr = Convert.ToInt32(rd["closed_pr"]);
          result.yield_percent = Convert.ToDecimal(rd["yield_percent"]);
        }
      }
    }

    return Json(result);
  }
  [HttpGet]
  public JsonResult GetPRStatusSummary(string start_dt, string end_dt, string dept)
  {
    var result = new { open_pr = 0, closed_pr = 0 };
    DateTime? start = string.IsNullOrEmpty(start_dt) ? null : Convert.ToDateTime(start_dt);
    DateTime? end = string.IsNullOrEmpty(end_dt) ? null : Convert.ToDateTime(end_dt);

    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();
    string connStr = config.GetConnectionString("BtProcureConn");
    using (SqlConnection con = new SqlConnection(connStr))
    {
      con.Open();
      SqlCommand cmd = new SqlCommand("SP_GetPRStatusSummary", con);
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.Parameters.Add(new SqlParameter("@start_dt", SqlDbType.DateTime) { Value = (object)start ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@end_dt", SqlDbType.DateTime) { Value = (object)end ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@dept", SqlDbType.NVarChar, 50) { Value = string.IsNullOrEmpty(dept) ? (object)DBNull.Value : dept });

      using (SqlDataReader rd = cmd.ExecuteReader())
      {
        if (rd.Read())
        {
          result = new
          {
            open_pr = Convert.ToInt32(rd["open_pr"]),
            closed_pr = Convert.ToInt32(rd["closed_pr"])
          };
        }
      }
    }

    return Json(result);
  }
  [HttpGet]
  public JsonResult GetPRDailyTrend(string start_dt, string end_dt, string dept)
  {
    var list = new List<object>();
    DateTime? start = string.IsNullOrEmpty(start_dt) ? null : Convert.ToDateTime(start_dt);
    DateTime? end = string.IsNullOrEmpty(end_dt) ? null : Convert.ToDateTime(end_dt);

    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();
    string connStr = config.GetConnectionString("BtProcureConn");
    using (SqlConnection con = new SqlConnection(connStr))
    {
      con.Open();
      SqlCommand cmd = new SqlCommand("SP_GetPRDailyTrend", con);
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.Parameters.Add(new SqlParameter("@start_dt", SqlDbType.DateTime) { Value = (object)start ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@end_dt", SqlDbType.DateTime) { Value = (object)end ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@dept", SqlDbType.NVarChar, 50) { Value = string.IsNullOrEmpty(dept) ? (object)DBNull.Value : dept });

      using (SqlDataReader rd = cmd.ExecuteReader())
      {
        while (rd.Read())
        {
          list.Add(new
          {
            date = Convert.ToDateTime(rd["pr_date"]).ToString("yyyy-MM-dd"),
            total = Convert.ToInt32(rd["total_pr"])
          });
        }
      }
    }

    return Json(list);
  }
  [HttpGet]
  public JsonResult GetPRMonthlySummary(string start_dt, string end_dt, string dept)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    string connStr = config.GetConnectionString("BtProcureConn");

    var list = new List<object>();
    DateTime? start = string.IsNullOrEmpty(start_dt) ? null : Convert.ToDateTime(start_dt);
    DateTime? end = string.IsNullOrEmpty(end_dt) ? null : Convert.ToDateTime(end_dt);

    using (SqlConnection con = new SqlConnection(connStr))
    {
      con.Open();
      SqlCommand cmd = new SqlCommand("SP_GetPRMonthlySummary", con);
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.Parameters.Add(new SqlParameter("@start_dt", SqlDbType.DateTime) { Value = (object)start ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@end_dt", SqlDbType.DateTime) { Value = (object)end ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@dept", SqlDbType.NVarChar, 50) { Value = string.IsNullOrEmpty(dept) ? (object)DBNull.Value : dept });

      using (SqlDataReader rd = cmd.ExecuteReader())
      {
        while (rd.Read())
        {
          list.Add(new
          {
            month_no = Convert.ToInt32(rd["month_no"]),
            month_name = rd["month_name"].ToString(),
            total_pr = Convert.ToInt32(rd["total_pr"])
          });
        }
      }
    }

    return Json(list);
  }
  [HttpGet]
  public JsonResult GetTopDepartment(string start_dt, string end_dt, string dept)
  {
    var list = new List<object>();
    DateTime? start = string.IsNullOrEmpty(start_dt) ? null : Convert.ToDateTime(start_dt);
    DateTime? end = string.IsNullOrEmpty(end_dt) ? null : Convert.ToDateTime(end_dt);

    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();
    string connStr = config.GetConnectionString("BtProcureConn");
    using (SqlConnection con = new SqlConnection(connStr))
    {
      con.Open();
      SqlCommand cmd = new SqlCommand("SP_GetTopDepartment", con);
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.Parameters.Add(new SqlParameter("@start_dt", SqlDbType.DateTime) { Value = (object)start ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@end_dt", SqlDbType.DateTime) { Value = (object)end ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@dept", SqlDbType.NVarChar, 50) { Value = string.IsNullOrEmpty(dept) ? (object)DBNull.Value : dept });

      using (SqlDataReader rd = cmd.ExecuteReader())
      {
        while (rd.Read())
        {
          list.Add(new
          {
            dept = rd["reqDepCode"].ToString(),
            total = Convert.ToInt32(rd["total_pr"])
          });
        }
      }
    }

    return Json(list);
  }
  [HttpGet]
  public IActionResult GetAccCodeList(string? search)
  {
      IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    var results = new List<dynamic>();
      
      using (var conn = new SqlConnection(DBConn))
      {
          conn.Open();
          using (var command = new SqlCommand("SP_GetAccCode", conn))
          {
              command.CommandType = CommandType.StoredProcedure;
              //command.Parameters.AddWithValue("@dt_st", DBNull.Value);
              //command.Parameters.AddWithValue("@dt_en", DBNull.Value);

              using (var reader = command.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      results.Add(new
                      {
                          AccMain = reader["AccMain"]?.ToString(),
                          AccName = reader["AccName"]?.ToString(),
                          AccType = reader["AccType"]?.ToString(),
                          AccCat = reader["AccCat"]?.ToString()
                      });
                  }
              }
              
          }
          conn.CloseAsync();
      }
        
    //List<string> lstStr = daAccess.GetOpenOrCloseDoc();
    //ViewData["OPENDOC"] = lstStr[0];
    //ViewData["CLOSEDOC"] = lstStr[1];
    return Json(results.ToList());
  }
  [HttpDelete]
  public JsonResult DelAccCode(AccModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_DeleteAccCode", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@AccMain", SqlDbType.VarChar).Value = obj.AccMain == null ? "" : obj.AccMain;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }

    con.Close();
    return Json(strMsg);
  }
  [HttpPost]
  public JsonResult SaveAccCode(AccModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_SaveAccCode", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@AccMainTmp", SqlDbType.VarChar).Value = obj.AccMainTmp == null ? "" : obj.AccMainTmp;
      cmnd.Parameters.AddWithValue("@AccMain", SqlDbType.VarChar).Value = obj.AccMain == null ? "" : obj.AccMain;
      cmnd.Parameters.AddWithValue("@AccName", SqlDbType.VarChar).Value = obj.AccName == null ? "" : obj.AccName;
      cmnd.Parameters.AddWithValue("@AccType", SqlDbType.VarChar).Value = obj.AccType == null ? "" : obj.AccType;
      cmnd.Parameters.AddWithValue("@AccCat", SqlDbType.VarChar).Value = obj.AccCat == null ? "" : obj.AccCat;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }

    con.Close();
    return Json(strMsg);
  }
  [HttpPost]
  public JsonResult AddNewAccCode(AccModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_InsAddNewAccCode", con);
      cmnd.CommandType = CommandType.StoredProcedure;

      cmnd.Parameters.AddWithValue("@AccMain", SqlDbType.VarChar).Value = obj.AccMain == null ? "" : obj.AccMain;
      cmnd.Parameters.AddWithValue("@AccName", SqlDbType.VarChar).Value = obj.AccName == null ? "" : obj.AccName;
      cmnd.Parameters.AddWithValue("@AccType", SqlDbType.VarChar).Value = obj.AccType == null ? "" : obj.AccType;
      cmnd.Parameters.AddWithValue("@AccCat", SqlDbType.VarChar).Value = obj.AccCat == null ? "" : obj.AccCat;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }

    con.Close();
    return Json(strMsg);
  }
  [HttpPost]
  public JsonResult AddNewProjCode(ProjModel obj)
  {

    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_InsAddNewProjCode", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@ProjNo", SqlDbType.VarChar).Value = obj.ProjNo == null ? "" : obj.ProjNo;
      cmnd.Parameters.AddWithValue("@ProjName", SqlDbType.VarChar).Value = obj.ProjName == null ? "" : obj.ProjName;
      cmnd.Parameters.AddWithValue("@ProjStat", SqlDbType.VarChar).Value = obj.ProjStat == null ? "" : obj.ProjStat;
      cmnd.Parameters.AddWithValue("@ActiveTo", SqlDbType.VarChar).Value = obj.ActiveTo == null ? "" : obj.ActiveTo;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }
    con.Close();
    return Json(strMsg);
  }
  [HttpPost]
  public JsonResult SaveProjCode(ProjModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_SaveProjCode", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@ProjNoTmp", SqlDbType.VarChar).Value = obj.ProjNoTmp == null ? "" : obj.ProjNoTmp;
      cmnd.Parameters.AddWithValue("@ProjNo", SqlDbType.VarChar).Value = obj.ProjNo == null ? "" : obj.ProjNo;
      cmnd.Parameters.AddWithValue("@ProjName", SqlDbType.VarChar).Value = obj.ProjName == null ? "" : obj.ProjName;
      cmnd.Parameters.AddWithValue("@ProjStat", SqlDbType.VarChar).Value = obj.ProjStat == null ? "" : obj.ProjStat;
      cmnd.Parameters.AddWithValue("@ActiveTo", SqlDbType.DateTime).Value = obj.ActiveTo == null ? "" : obj.ActiveTo;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }

    con.Close();
    return Json(strMsg);
  }
  [HttpDelete]
  public JsonResult DelProjCode(ProjModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_DeleteProjCode", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@ProjNo", SqlDbType.VarChar).Value = obj.ProjNo == null ? "" : obj.ProjNo;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }

    con.Close();
    return Json(strMsg);
  }

  [HttpPost]
  public JsonResult AddNewVenCode(VenModel obj)
  {

    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_InsAddNewVendor", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@VenName", SqlDbType.NVarChar).Value = obj.VenName == null ? "" : obj.VenName;
      cmnd.Parameters.AddWithValue("@VenCode", SqlDbType.VarChar).Value = obj.VenCode == null ? "" : obj.VenCode;
      cmnd.Parameters.AddWithValue("@Vencurrency", SqlDbType.VarChar).Value = obj.Vencurrency == null ? "" : obj.Vencurrency;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }
    con.Close();
    return Json(strMsg);
  }
  [HttpPost]
  public JsonResult SaveVenCode(VenModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_SaveVenCode", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@VenCodeTmp", SqlDbType.VarChar).Value = obj.VenCodeTmp == null ? "" : obj.VenCodeTmp;
      cmnd.Parameters.AddWithValue("@VenCode", SqlDbType.VarChar).Value = obj.VenCode == null ? "" : obj.VenCode;
      cmnd.Parameters.AddWithValue("@VenName", SqlDbType.NVarChar).Value = obj.VenName == null ? "" : obj.VenName;
      cmnd.Parameters.AddWithValue("@Vencurrency", SqlDbType.VarChar).Value = obj.Vencurrency == null ? "" : obj.Vencurrency;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }

    con.Close();
    return Json(strMsg);
  }
  [HttpDelete]
  public JsonResult DelVendor(VenModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_DeleteVenCode", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@VenCode", SqlDbType.VarChar).Value = obj.VenCode == null ? "" : obj.VenCode;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }

    con.Close();
    return Json(strMsg);
  }

  [HttpPost]
  public JsonResult InsAddUserProcure(ProcureUserModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_InsAddUserProcure", con);
      cmnd.CommandType = CommandType.StoredProcedure;

      cmnd.Parameters.AddWithValue("@USRROLE", SqlDbType.Int).Value = obj.USRROLE == null ? 0 : obj.USRROLE;
      cmnd.Parameters.AddWithValue("@USERLOGON", SqlDbType.VarChar).Value = obj.USERLOGON == null ? "" : obj.USERLOGON;
      cmnd.Parameters.AddWithValue("@EMP_CODE", SqlDbType.VarChar).Value = obj.EMP_CODE == null ? "" : obj.EMP_CODE;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }

    con.Close();
    return Json(strMsg);
  }
  [HttpPut]
  public JsonResult UpdateAddUserProcure(ProcureUserModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_UpdateUserProcure", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@ID", SqlDbType.VarChar).Value = obj.ID == null ? "" : obj.ID;
      cmnd.Parameters.AddWithValue("@USERLOGON", SqlDbType.VarChar).Value = obj.USERLOGON == null ? "" : obj.USERLOGON;
      cmnd.Parameters.AddWithValue("@USRROLE", SqlDbType.Int).Value = obj.USRROLE == null ? 0 : obj.USRROLE;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }

    con.Close();
    return Json(strMsg);
  }
  [HttpDelete]
  public JsonResult DeleteAddUserProcure(ProcureUserModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_DeleteUserProcure", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@ID", SqlDbType.VarChar).Value = obj.ID == null ? "" : obj.ID;
      cmnd.ExecuteNonQuery();
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = "1";
    }

    con.Close();
    return Json(strMsg);
  }
  [HttpGet]
  public IActionResult ProjCodeList()
  {
    //List<string> lstStr = daAccess.GetOpenOrCloseDoc();
    //ViewData["OPENDOC"] = lstStr[0];
    //ViewData["CLOSEDOC"] = lstStr[1];
    if (HttpContext.Session.GetString(SessionModel.UROLEADMIN) != null)
    {
      ViewData["UROLEADMIN"] = HttpContext.Session.GetString(SessionModel.UROLEADMIN);
    }


    return View();
  }
  [HttpGet]
  public IActionResult GetProjCodeList(string? search)
  {
      IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    var results = new List<dynamic>();
      
      using (var conn = new SqlConnection(DBConn))
      {
          conn.Open();
          using (var command = new SqlCommand("SP_GetProjCode", conn)) 
          {
              command.CommandType = CommandType.StoredProcedure;
              //command.Parameters.AddWithValue("@dt_st", DBNull.Value);
              //command.Parameters.AddWithValue("@dt_en", DBNull.Value);
              //ProjNo	ProjName	ProjStat	ActiveTo

              using (var reader = command.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      results.Add(new
                      {
                          ProjNo = reader["ProjNo"]?.ToString(),
                          ProjName = reader["ProjName"]?.ToString(),
                          ProjStat = reader["ProjStat"]?.ToString(),
                          ActiveTo = (DateTime)reader["ActiveTo"],
                          ActiveToTxt = reader["ActiveToTxt"]?.ToString()
                      });
                  }
              }
              
          }
          conn.CloseAsync();
      }

    //List<string> lstStr = daAccess.GetOpenOrCloseDoc();
    //ViewData["OPENDOC"] = lstStr[0];
    //ViewData["CLOSEDOC"] = lstStr[1];
    return Json(results.ToList());
  }
[HttpGet]
  public IActionResult VendorList()
  {
    //List<string> lstStr = daAccess.GetOpenOrCloseDoc();
    //ViewData["OPENDOC"] = lstStr[0];
    //ViewData["CLOSEDOC"] = lstStr[1];
    if (HttpContext.Session.GetString(SessionModel.UROLEADMIN) != null)
    {
      ViewData["UROLEADMIN"] = HttpContext.Session.GetString(SessionModel.UROLEADMIN);
    }

    return View();
  }
  [HttpGet]
  public IActionResult GetVendorList(string? search)
  {
      IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    var results = new List<dynamic>();
      
      using (var conn = new SqlConnection(DBConn))
      {
          conn.Open();
          using (var command = new SqlCommand("SP_GetVendor", conn)) 
          {
              command.CommandType = CommandType.StoredProcedure;
              //command.Parameters.AddWithValue("@dt_st", DBNull.Value);
              //command.Parameters.AddWithValue("@dt_en", DBNull.Value);
              //VenName, VenCode, Vencurrency

              using (var reader = command.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      results.Add(new
                      {
                          VenName = reader["VenName"]?.ToString(),
                          VenCode = reader["VenCode"]?.ToString(),
                          Vencurrency = reader["Vencurrency"]?.ToString()
                      });
                  }
              }
          }
          conn.CloseAsync();
      }

    //List<string> lstStr = daAccess.GetOpenOrCloseDoc();
    //ViewData["OPENDOC"] = lstStr[0];
    //ViewData["CLOSEDOC"] = lstStr[1];

    return Json(results.ToList());
  }
  [HttpGet] 
  public IActionResult GetUsersList()
  {
    IConfiguration _configuration = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json")
                      .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    List<ProcureUserModel> results = new List<ProcureUserModel>();

    using (var conn = new SqlConnection(DBConn))
    {
      conn.Open();
      using (var command = new SqlCommand("SP_GetProcureUser", conn))
      {
        command.CommandType = CommandType.StoredProcedure;

        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            results.Add(new ProcureUserModel()
            {
              ID = reader["ID"].ToString(),
              EMP_CODE = reader["EMP_CODE"]?.ToString(),
              DISPNAME = reader["DISPNAME"]?.ToString(),
              UEMAIL = reader["UEMAIL"].ToString(),
              USRROLE = Convert.ToInt32(reader["USRROLE"]),
              apprv_proc = Convert.ToInt32(reader["apprv_proc"]),
              apprv_proc_txt = reader["apprv_proc_txt"].ToString(),
              USERROLE_TXT = reader["USERROLE_TXT"].ToString()
            });
          }
        }
      }
      conn.CloseAsync();
    }
    return Json(results.ToList());
  }
  public IActionResult UserSetting(string? user)
  {
    ViewOrdDetailModelVM vm = new ViewOrdDetailModelVM();
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    //-- AUTHEN START
    string AuthenUrl = _configuration[key: "TBCorApiServices:AuthenUrl"];
    string URLApproval = _configuration[key: "TBCorApiServices:URLApproval"];
    if (user == null && (HttpContext.Session.GetString(SessionModel.SAMNAME) == null ||
      HttpContext.Session.GetString(SessionModel.SAMNAME) == ""
      ))
    {
      //-->Response.Redirect("https://btauthen.berninathailand.com/?url=https://www.google.com/");
      Response.Redirect(AuthenUrl);
      //-->Response.Redirect(AuthenUrl + "?url=" + URLApproval + "?docNo=" + docNo + "");
    }
    else if ((HttpContext.Session.GetString(SessionModel.SAMNAME) == null ||
      HttpContext.Session.GetString(SessionModel.SAMNAME) == "") && user != null)
    {
      string[] arrSamName = user.Split(new char[] { '\\' });
      if (arrSamName.Length == 2)
      {
        HttpContext.Session.SetString(SessionModel.SAMNAME, arrSamName[1]);
      }
    }
    //-- AUTHEN END
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    //-- Get User Permission
    //HttpContext.Session.SetString(SessionModel.UROLEADMIN, ADMIN_PERMISS.ToString());

    if (HttpContext.Session.GetString(SessionModel.UROLEADMIN) != null)
    {
      ViewData["UROLEADMIN"] = HttpContext.Session.GetString(SessionModel.UROLEADMIN);
    }

    //----> Get All users from AD.
    List<HRUserModel> ressam = new List<HRUserModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_CFGDropDown", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@FUNC", SqlDbType.VarChar).Value = "HR_USER";
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            ressam.Add(new HRUserModel()
            {
              EMP_CODE = reader["EMP_CODE"].ToString(),
              DISPNAME = reader["DISPNAME"].ToString(),
              UEMAIL = reader["UEMAIL"].ToString()
            });
          }
        }
        conn.Close();
      }
      vm.VMHrUserModel = ressam.ToList();
    }
    catch (Exception ex)
    {
    }
    //List<string> lstStr = daAccess.GetOpenOrCloseDoc();
    //ViewData["OPENDOC"] = lstStr[0];
    //ViewData["CLOSEDOC"] = lstStr[1];
    return View(vm);
  }
  public IActionResult PRNewRequest()
  {
    var isFullscreen = Request.Query["fullscreen"]; // หรือ TempData
    ViewData["isFullscreen"] = isFullscreen.ToString().ToLower();

    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    // Authen URL
    //-->Response.Redirect("https://btauthen.berninathailand.com/?url=https://www.google.com/");
    string AuthenUrl = _configuration[key: "TBCorApiServices:AuthenUrl"];
    if ((HttpContext.Session.GetString(SessionModel.SAMNAME) == null ||
      HttpContext.Session.GetString(SessionModel.SAMNAME) == ""
      ))
    {
      //-->Response.Redirect("https://btauthen.berninathailand.com/?url=https://www.google.com/");
      Response.Redirect(AuthenUrl);
      //-->Response.Redirect(RootURL + "?url=https://www.google.com/");                
    }

    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    if (HttpContext.Session.GetString(SessionModel.UROLEADMIN) != null)
    {
      ViewData["UROLEADMIN"] = HttpContext.Session.GetString(SessionModel.UROLEADMIN);
    }
    NewPRModelVM VM = new NewPRModelVM();

    //----> Vendor List
    List<VenModel> lstVen = new List<VenModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetVendor", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstVen.Add(new VenModel()
            {
              VenCode = reader["VenCode"].ToString(),
              VenName = reader["VenName"].ToString(),
              Vencurrency = reader["Vencurrency"].ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMVenModel = lstVen.ToList();
    }
    catch (Exception ex)
    {
    }
    //----> Procure User List
    List<ProcureUserModel> lstUsr = new List<ProcureUserModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetProcureUser", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstUsr.Add(new ProcureUserModel()
            {
              ID = reader["ID"].ToString(),
              EMP_CODE = reader["EMP_CODE"]?.ToString(),
              DISPNAME = reader["DISPNAME"]?.ToString(),
              UEMAIL = reader["UEMAIL"].ToString(),
              USRROLE = Convert.ToInt32(reader["USRROLE"]),
              apprv_proc = Convert.ToInt32(reader["apprv_proc"]),
              apprv_proc_txt = reader["apprv_proc_txt"].ToString(),
              USERROLE_TXT = reader["USERROLE_TXT"].ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMProcureUserModel = lstUsr.ToList();
    }
    catch (Exception ex)
    {
    }
    //----> Get All users from AD.
    /*
    List<HRUserModel> ressam = new List<HRUserModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_CFGDropDown", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@FUNC", SqlDbType.VarChar).Value = "HR_USER";
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            ressam.Add(new HRUserModel()
            {
              EMP_CODE = reader["EMP_CODE"].ToString(),
              DISPNAME = reader["DISPNAME"].ToString(),
              UEMAIL = reader["UEMAIL"].ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMHRUserModel = ressam.ToList();
    }
    catch (Exception ex)
    {
    }
    */
    //----> Get All Project Code
    List<ProjModel> lstProj = new List<ProjModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetProjCodeActive", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstProj.Add(new ProjModel()
            {
              ProjNo = reader["ProjNo"].ToString(),
              ProjName = reader["ProjName"].ToString(),
              ProjStat = reader["ProjStat"].ToString(),
              ActiveToTxt = reader["ActiveToTxt"].ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMProjModel = lstProj.ToList();
    }
    catch (Exception ex)
    {
    }
    //----> Get All Account Code
    List<AccModel> lstAcc = new List<AccModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetAccCode", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstAcc.Add(new AccModel()
            {
              AccMain = reader["AccMain"].ToString(),
              AccName = reader["AccName"].ToString(),
              AccType = reader["AccType"].ToString(),
              AccCat = reader["AccCat"].ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMAccModel = lstAcc.ToList();
    }
    catch (Exception ex)
    {
    }

    //----> Get Currency Code
    List<CurrencyModel> lstCurr = new List<CurrencyModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GroupAllCurrency", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstCurr.Add(new CurrencyModel()
            {
              Vencurrency = reader["Vencurrency"].ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMCurrencyModel = lstCurr.ToList();
    }
    catch (Exception ex)
    {
    }

    //--> Get Draft PR No.
    try
    {
      string sEMP_CODE = HttpContext.Session.GetString(SessionModel.EMPCODE);
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetDraftDocno", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@emp_code", SqlDbType.VarChar).Value = sEMP_CODE;
        sql_cmnd.Parameters.Add("@doc_number", SqlDbType.VarChar, 25);
        sql_cmnd.Parameters["@doc_number"].Direction = ParameterDirection.Output;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstAcc.Add(new AccModel()
            {
              AccMain = reader["AccMain"].ToString(),
              AccName = reader["AccName"].ToString(),
              AccType = reader["AccType"].ToString(),
              AccCat = reader["AccCat"].ToString()
            });
          }
          ViewData["DOC_NUMBER"] = sql_cmnd.Parameters["@doc_number"].Value.ToString();
        }
        conn.Close();
      }
    }
    catch (Exception ex)
    {
    }

    //if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    //{
    //  List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
    //  ViewData["OPENDOC"] = lstStr[0];
    //  ViewData["CLOSEDOC"] = lstStr[1];
    //}
    string txtEMPCODE = HttpContext.Session.GetString(SessionModel.EMPCODE);

    List<string> lstPending = daAccess.GetPendingApprove(txtEMPCODE);
    ViewData["PENDING"] = lstPending[0];
    return View(VM);
  }

  [HttpPost]
  public JsonResult SaveNewPRHeader(PRHeaderModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration["ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;

    try
    {
      con = new SqlConnection(DBConn);
      con.Open();

      SqlCommand cmnd = new SqlCommand("SP_InsertPRHeader", con);
      cmnd.CommandType = CommandType.StoredProcedure;

      cmnd.Parameters.AddWithValue("@prno", obj.prno ?? "");
      cmnd.Parameters.AddWithValue("@projectno", obj.projectno ?? "");
      cmnd.Parameters.AddWithValue("@empcode", obj.empcode ?? "");
      cmnd.Parameters.AddWithValue("@approx_type", obj.approx_type);
      cmnd.Parameters.AddWithValue("@approx_dt", obj.approx_dt);
      cmnd.Parameters.AddWithValue("@invcreditno", obj.invcreditno ?? "");
      cmnd.Parameters.AddWithValue("@purpose_type", obj.purpose_type);
      cmnd.Parameters.AddWithValue("@ref_docs", obj.ref_docs ?? "");
      cmnd.Parameters.AddWithValue("@pr_reason", obj.pr_reason ?? "");
      cmnd.Parameters.AddWithValue("@pr_recvdt", DBNull.Value);
      cmnd.Parameters.AddWithValue("@pr_recvpono", obj.pr_recvpono ?? "");
      cmnd.Parameters.AddWithValue("@attach_flag", obj.attach_flag ?? "");
      cmnd.Parameters.AddWithValue("@reqDepCode", obj.reqDepCode ?? "");
      cmnd.Parameters.AddWithValue("@reqDate", DBNull.Value);
      cmnd.Parameters.AddWithValue("@reqFlag", obj.reqFlag);
      cmnd.Parameters.AddWithValue("@appEmp", obj.appEmp ?? "");
      cmnd.Parameters.AddWithValue("@appEmp2", obj.appEmp2 ?? "");
      //cmnd.Parameters.AddWithValue("@appDate", obj.appDate);
      //cmnd.Parameters.AddWithValue("@appFlag", obj.appFlag);
      cmnd.Parameters.AddWithValue("@countEmp", obj.countEmp ?? "");
      //cmnd.Parameters.AddWithValue("@countDate", obj.countDate);
      //cmnd.Parameters.AddWithValue("@countFlag", obj.countFlag);
      cmnd.Parameters.AddWithValue("@authEmp", obj.authEmp ?? "");
      //cmnd.Parameters.AddWithValue("@authDate", obj.authDate);
      //cmnd.Parameters.AddWithValue("@authFlag", obj.authFlag);
      cmnd.Parameters.AddWithValue("@prstatus", obj.prstatus);
      cmnd.Parameters.AddWithValue("@pub_remark", obj.pub_remark ?? "");
      cmnd.Parameters.AddWithValue("@prcurrency", obj.prcurrency ?? "");

      cmnd.Parameters.AddWithValue("@id_supp1", obj.id_supp1 ?? "");
      cmnd.Parameters.AddWithValue("@name_supp1", obj.name_supp1 ?? "");
      cmnd.Parameters.AddWithValue("@vc_supp1", obj.vc_supp1 ?? "");
      cmnd.Parameters.AddWithValue("@contact_supp1", obj.contact_supp1 ?? "");
      cmnd.Parameters.AddWithValue("@email_supp1", obj.email_supp1 ?? "");
      cmnd.Parameters.AddWithValue("@tel_supp1", obj.tel_supp1 ?? "");
      cmnd.Parameters.AddWithValue("@remark_supp1", obj.remark_supp1 ?? "");
      cmnd.Parameters.AddWithValue("@quoref_supp1", obj.quoref_supp1 ?? "");
      cmnd.Parameters.AddWithValue("@refnodt_supp1", obj.refnodt_supp1 == null ? DBNull.Value : obj.refnodt_supp1);

      cmnd.Parameters.AddWithValue("@id_supp2", obj.id_supp2 ?? "");
      cmnd.Parameters.AddWithValue("@name_supp2", obj.name_supp2 ?? "");
      cmnd.Parameters.AddWithValue("@vc_supp2", obj.vc_supp2 ?? "");
      cmnd.Parameters.AddWithValue("@contact_supp2", obj.contact_supp2 ?? "");
      cmnd.Parameters.AddWithValue("@email_supp2", obj.email_supp2 ?? "");
      cmnd.Parameters.AddWithValue("@tel_supp2", obj.tel_supp2 ?? "");
      cmnd.Parameters.AddWithValue("@remark_supp2", obj.remark_supp2 ?? "");
      cmnd.Parameters.AddWithValue("@quoref_supp2", obj.quoref_supp2 ?? "");
      cmnd.Parameters.AddWithValue("@refnodt_supp2", obj.refnodt_supp2 == null ? DBNull.Value : obj.refnodt_supp2);

      cmnd.Parameters.AddWithValue("@id_supp3", obj.id_supp3 ?? "");
      cmnd.Parameters.AddWithValue("@name_supp3", obj.name_supp3 ?? "");
      cmnd.Parameters.AddWithValue("@vc_supp3", obj.vc_supp3 ?? "");
      cmnd.Parameters.AddWithValue("@contact_supp3", obj.contact_supp3 ?? "");
      cmnd.Parameters.AddWithValue("@email_supp3", obj.email_supp3 ?? "");
      cmnd.Parameters.AddWithValue("@tel_supp3", obj.tel_supp3 ?? "");
      cmnd.Parameters.AddWithValue("@remark_supp3", obj.remark_supp3 ?? "");
      cmnd.Parameters.AddWithValue("@quoref_supp3", obj.quoref_supp3 ?? "");
      cmnd.Parameters.AddWithValue("@refnodt_supp3", obj.refnodt_supp3 == null ? DBNull.Value : obj.refnodt_supp3);

      cmnd.Parameters.AddWithValue("@id_supp4", obj.id_supp4 ?? "");
      cmnd.Parameters.AddWithValue("@name_supp4", obj.name_supp4 ?? "");
      cmnd.Parameters.AddWithValue("@vc_supp4", obj.vc_supp4 ?? "");
      cmnd.Parameters.AddWithValue("@contact_supp4", obj.contact_supp4 ?? "");
      cmnd.Parameters.AddWithValue("@email_supp4", obj.email_supp4 ?? "");
      cmnd.Parameters.AddWithValue("@tel_supp4", obj.tel_supp4 ?? "");
      cmnd.Parameters.AddWithValue("@remark_supp4", obj.remark_supp4 ?? "");
      cmnd.Parameters.AddWithValue("@quoref_supp4", obj.quoref_supp4 ?? "");
      cmnd.Parameters.AddWithValue("@refnodt_supp4", obj.refnodt_supp4 == null ? DBNull.Value : obj.refnodt_supp4);

      cmnd.Parameters.AddWithValue("@id_supp5", obj.id_supp5 ?? "");
      cmnd.Parameters.AddWithValue("@name_supp5", obj.name_supp5 ?? "");
      cmnd.Parameters.AddWithValue("@vc_supp5", obj.vc_supp5 ?? "");
      cmnd.Parameters.AddWithValue("@contact_supp5", obj.contact_supp5 ?? "");
      cmnd.Parameters.AddWithValue("@email_supp5", obj.email_supp5 ?? "");
      cmnd.Parameters.AddWithValue("@tel_supp5", obj.tel_supp5 ?? "");
      cmnd.Parameters.AddWithValue("@remark_supp5", obj.remark_supp5 ?? "");
      cmnd.Parameters.AddWithValue("@quoref_supp5", obj.quoref_supp5 ?? "");
      cmnd.Parameters.AddWithValue("@refnodt_supp5", obj.refnodt_supp5 == null ? DBNull.Value : obj.refnodt_supp5);

      //cmnd.Parameters.AddWithValue("@create_dt", obj.create_dt);
      //cmnd.Parameters.AddWithValue("@update_dt", obj.update_dt);

      SqlParameter output = new SqlParameter("@RESULT", SqlDbType.Int){
        Direction = ParameterDirection.Output
      };
      SqlParameter output2 = new SqlParameter("@RTN_PROJNO", SqlDbType.VarChar, 50)
      {
        Direction = ParameterDirection.Output
      };

      cmnd.Parameters.Add(output);
      cmnd.Parameters.Add(output2);

      cmnd.ExecuteNonQuery();

      int result = Convert.ToInt32(output.Value);
      strMsg[0] = result == 1 ? "0" : "-1";  // 0=success, -1=duplicate
      strMsg[1] = result == 1 ? "Insert success" : "Duplicate PRNo";
      strMsg[2] = output2.Value.ToString();
    }
    catch (Exception ex)
    {
      strMsg[0] = "-1";
      strMsg[1] = $"Error: {ex.Message}";
      strMsg[2] = "";
    }
    finally
    {
      if (con != null && con.State == ConnectionState.Open)
        con.Close();
    }

    return Json(strMsg);
  }
  public IActionResult PRResultRequest(string? docno, string? user)
  {
    var isFullscreen = Request.Query["fullscreen"]; // หรือ TempData
    ViewData["isFullscreen"] = isFullscreen.ToString().ToLower();

    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    // Authen URL
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    string URLApproval = _configuration[key: "TBCorApiServices:URLApproval"];
    ViewData["URLApproval"] = URLApproval;
    string URLResult = _configuration[key: "TBCorApiServices:URLResult"];
    ViewData["URLResult"] = URLResult;
    //--> Start Authen
    string sUSERLOGON = "";
    string AuthenUrl = _configuration[key: "TBCorApiServices:AuthenUrl"];
    if (user == null && (HttpContext.Session.GetString(SessionModel.SAMNAME) == null ||
      HttpContext.Session.GetString(SessionModel.SAMNAME) == ""
      ))
    {
      //-->Response.Redirect("https://btauthen.berninathailand.com/?url=https://www.google.com/");
      Response.Redirect(AuthenUrl);
      //-->Response.Redirect(RootURL + "?url=https://www.google.com/");                
    }
    else if ((HttpContext.Session.GetString(SessionModel.SAMNAME) == null ||
      HttpContext.Session.GetString(SessionModel.SAMNAME) == "") && user != null)
    {
      string[] arrSamName = user.Split(new char[] { '\\' });
      if (arrSamName.Length == 2)
      {
        HttpContext.Session.SetString(SessionModel.SAMNAME, arrSamName[1]);
        sUSERLOGON = arrSamName[1];
      }
    }
    //--> End of Authen



    if (HttpContext.Session.GetString(SessionModel.UROLEADMIN) != null)
    {
      ViewData["UROLEADMIN"] = HttpContext.Session.GetString(SessionModel.UROLEADMIN);
    }
    NewPRModelVM VM = new NewPRModelVM();

    //----> Vendor List
    List<VenModel> lstVen = new List<VenModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetVendor", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstVen.Add(new VenModel()
            {
              VenCode = reader["VenCode"].ToString(),
              VenName = reader["VenName"].ToString(),
              Vencurrency = reader["Vencurrency"].ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMVenModel = lstVen.ToList();
    }
    catch (Exception ex)
    {
    }
    //----> Procure User List
    List<ProcureUserModel> lstUsr = new List<ProcureUserModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetProcureUser", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstUsr.Add(new ProcureUserModel()
            {
              ID = reader["ID"].ToString(),
              EMP_CODE = reader["EMP_CODE"]?.ToString(),
              DISPNAME = reader["DISPNAME"]?.ToString(),
              UEMAIL = reader["UEMAIL"].ToString(),
              USRROLE = Convert.ToInt32(reader["USRROLE"]),
              apprv_proc = Convert.ToInt32(reader["apprv_proc"]),
              apprv_proc_txt = reader["apprv_proc_txt"].ToString(),
              USERROLE_TXT = reader["USERROLE_TXT"].ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMProcureUserModel = lstUsr.ToList();
    }
    catch (Exception ex)
    {
    }
    //----> Get All Project Code
    List<ProjModel> lstProj = new List<ProjModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetProjCodeActive", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstProj.Add(new ProjModel()
            {
              ProjNo = reader["ProjNo"].ToString(),
              ProjName = reader["ProjName"].ToString(),
              ProjStat = reader["ProjStat"].ToString(),
              ActiveToTxt = reader["ActiveToTxt"].ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMProjModel = lstProj.ToList();
    }
    catch (Exception ex)
    {
    }
    //----> Get All Account Code
    List<AccModel> lstAcc = new List<AccModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetAccCode", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstAcc.Add(new AccModel()
            {
              AccMain = reader["AccMain"].ToString(),
              AccName = reader["AccName"].ToString(),
              AccType = reader["AccType"].ToString(),
              AccCat = reader["AccCat"].ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMAccModel = lstAcc.ToList();
    }
    catch (Exception ex)
    {
    }
    //--> Get Draft PR No.
    try
    {
      string sEMP_CODE = HttpContext.Session.GetString(SessionModel.EMPCODE);
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetDraftDocno", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@emp_code", SqlDbType.VarChar).Value = sEMP_CODE;
        sql_cmnd.Parameters.Add("@doc_number", SqlDbType.VarChar, 25);
        sql_cmnd.Parameters["@doc_number"].Direction = ParameterDirection.Output;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstAcc.Add(new AccModel()
            {
              AccMain = reader["AccMain"].ToString(),
              AccName = reader["AccName"].ToString(),
              AccType = reader["AccType"].ToString(),
              AccCat = reader["AccCat"].ToString()
            });
          }
          ViewData["DOC_NUMBER"] = sql_cmnd.Parameters["@doc_number"].Value.ToString();
        }
        conn.Close();
      }
    }
    catch (Exception ex)
    {
    }
    //----> Get Currency Code
    List<CurrencyModel> lstCurr = new List<CurrencyModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GroupAllCurrency", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstCurr.Add(new CurrencyModel()
            {
              Vencurrency = reader["Vencurrency"].ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMCurrencyModel = lstCurr.ToList();
    }
    catch (Exception ex)
    {
    }

    //-->  Get Approval list for 1st role
    List<ApprovalListModel> lstAppSupper = new List<ApprovalListModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetUserByRole", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@ROLE", 1);
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstAppSupper.Add(new ApprovalListModel()
            {
              emp_code = reader["emp_code"].ToString(),
              dep_code = Convert.ToInt32(reader["dep_code"]),
              uemail = reader["uemail"]?.ToString(),
              usrrole = Convert.ToInt32(reader["usrrole"]),
              dispname = reader["dispname"]?.ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMApprovalListRole1Model = lstAppSupper.ToList();
    }
    catch (Exception ex)
    {
    }
    //-->  Get Approval list for 2-3st role
    List<ApprovalListModel> lstAppMgr = new List<ApprovalListModel>();
    List<ApprovalListModel> lstAppMD = new List<ApprovalListModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetUserByRole", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@ROLE", 0);
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            if (Convert.ToInt32(reader["usrrole"]) == 2)
            {
              lstAppMgr.Add(new ApprovalListModel()
              {
                emp_code = reader["emp_code"].ToString(),
                dep_code = Convert.ToInt32(reader["dep_code"]),
                uemail = reader["uemail"]?.ToString(),
                usrrole = Convert.ToInt32(reader["usrrole"]), 
                dispname = reader["dispname"]?.ToString()
              });
            }
            else if (Convert.ToInt32(reader["usrrole"]) == 3)
            {
              lstAppMD.Add(new ApprovalListModel()
              {
                emp_code = reader["emp_code"].ToString(),
                dep_code = Convert.ToInt32(reader["dep_code"]),
                uemail = reader["uemail"]?.ToString(),
                usrrole = Convert.ToInt32(reader["usrrole"]),
                dispname = reader["dispname"]?.ToString()
              });
            }
              
          }
        }
        conn.Close();
      }
      VM.VMApprovalListRole2Model = lstAppMgr.ToList();
      VM.VMApprovalListRole3Model = lstAppMD.ToList();
    }
    catch (Exception ex)
    {
    }
    //--> Get Items part model
    List<ItemsPartsModel> lstItems = new List<ItemsPartsModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetPPItemPartListV102", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            lstItems.Add(new ItemsPartsModel()
            {
              itemno = reader["itemno"].ToString() != "" ? reader["itemno"].ToString().Replace(".", "") : "",
              itemdoc = reader["itemdoc"].ToString() != "" ? reader["itemdoc"].ToString().Replace(".","") : "",
              itemname = reader["itemname"]?.ToString()
            });
          }
        }
        conn.Close();
      }
      VM.VMItemsPartsModel = lstItems.ToList();
    }
    catch (Exception ex)
    {
    }


    return View(VM);
  }

  public IActionResult PRApproval(string? docno, string? user)
  {
    var isFullscreen = Request.Query["fullscreen"]; // หรือ TempData
    ViewData["isFullscreen"] = isFullscreen.ToString().ToLower();
    ViewData["DOC_NUMBER"] = docno;

    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    // Authen URL
    string AuthenUrl = _configuration[key: "TBCorApiServices:AuthenUrl"];
    if (user == null && (HttpContext.Session.GetString(SessionModel.SAMNAME) == null ||
      HttpContext.Session.GetString(SessionModel.SAMNAME) == ""
      ))
    {
      //-->Response.Redirect("https://btauthen.berninathailand.com/?url=https://www.google.com/");
      Response.Redirect(AuthenUrl);
      //-->Response.Redirect(AuthenUrl + "?url=" + URLApproval + "?docNo=" + docNo + "");
    }
    else if ((HttpContext.Session.GetString(SessionModel.SAMNAME) == null ||
      HttpContext.Session.GetString(SessionModel.SAMNAME) == "") && user != null)
    {
      string[] arrSamName = user.Split(new char[] { '\\' });
      if (arrSamName.Length == 2)
      {
        HttpContext.Session.SetString(SessionModel.SAMNAME, arrSamName[1]);
      }
    }
    //-- AUTHEN END

    string URLApproval = _configuration[key: "TBCorApiServices:URLApproval"];
    ViewData["URLApproval"] = URLApproval;
    string URLResult = _configuration[key: "TBCorApiServices:URLResult"];
    ViewData["URLResult"] = URLResult;

    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    if (HttpContext.Session.GetString(SessionModel.UROLEADMIN) != null)
    {
      ViewData["UROLEADMIN"] = HttpContext.Session.GetString(SessionModel.UROLEADMIN);
    }

    string txtEMPCODE = HttpContext.Session.GetString(SessionModel.EMPCODE);
    List<string> lstPending = daAccess.GetPendingApprove(txtEMPCODE);
    ViewData["PENDING"] = lstPending[0];

    return View();
  }

  [HttpGet]
  public JsonResult GetPRByDocNo(string docno)
  {
    PRHeaderModel model = new PRHeaderModel();

    IConfiguration _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    string connStr = _configuration["ConnectionStrings:BtProcureConn"];

    using (SqlConnection con = new SqlConnection(connStr))
    {
      using (SqlCommand cmd = new SqlCommand("SP_GetPRDocno", con))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@docno", docno ?? "");

        con.Open();
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            model.id = reader["id"]?.ToString();
            model.prno = reader["prno"]?.ToString();
            model.projectno = reader["projectno"]?.ToString();
            model.empcode = reader["empcode"]?.ToString();
            model.empcode_txt = reader["empcode_txt"]?.ToString();
            model.approx_type = reader["approx_type"] as int?;
            model.approx_dt = reader["approx_dt"] as DateTime?;
            model.invcreditno = reader["invcreditno"]?.ToString();
            model.purpose_type = reader["purpose_type"] as int?;
            model.ref_docs = reader["ref_docs"]?.ToString();
            model.pr_reason = reader["pr_reason"]?.ToString();
            model.pr_recvdt = reader["pr_recvdt"] as DateTime?;
            model.pr_recvpono = reader["pr_recvpono"]?.ToString();
            model.attach_flag = reader["attach_flag"]?.ToString();
            model.reqDepCode = reader["reqDepCode"]?.ToString();
            model.reqDate = reader["reqDate"] as DateTime?;
            model.reqFlag = reader["reqFlag"] as int?;
            model.appEmp = reader["appEmp"]?.ToString();
            model.appDate = reader["appDate"] as DateTime?;
            model.appFlag = reader["appFlag"] as int?;

            model.appEmp2 = reader["appEmp2"]?.ToString();
            model.appDate2 = reader["appDate2"] as DateTime?;
            model.appFlag2 = reader["appFlag2"] as int?;

            model.countEmp = reader["countEmp"]?.ToString();
            model.countDate = reader["countDate"] as DateTime?;
            model.countFlag = reader["countFlag"] as int?;
            model.authEmp = reader["authEmp"]?.ToString();
            model.authDate = reader["authDate"] as DateTime?;
            model.authFlag = reader["authFlag"] as int?;
            model.prstatus = Convert.ToInt32(reader["prstatus"]);
            model.create_dt = reader["create_dt"] as DateTime?;
            model.update_dt = reader["update_dt"] as DateTime?;
            model.update_dt_txt = reader["update_dt_txt"]?.ToString();
            model.reqDate_txt = reader["reqDate_txt"]?.ToString();
            model.prstatus_txt = reader["prstatus_txt"]?.ToString();
            model.pub_remark = reader["pub_remark"]?.ToString();
            model.prcurrency = reader["prcurrency"]?.ToString();
            model.reqEmail = reader["reqEmail"]?.ToString();
            model.appEmail = reader["appEmail"]?.ToString();
            model.appEmail2 = reader["appEmail2"]?.ToString();
            model.CountEmail = reader["CountEmail"]?.ToString();
            model.authEmail = reader["authEmail"]?.ToString();

            model.appEmp_txt = reader["appEmp_txt"]?.ToString();
            model.appDate_txt = reader["appDate_txt"]?.ToString();

            model.appEmp2_txt = reader["appEmp2_txt"]?.ToString();
            model.appDate2_txt = reader["appDate2_txt"]?.ToString();

            model.countEmp_txt = reader["countEmp_txt"]?.ToString();
            model.countDate_txt = reader["countDate_txt"]?.ToString();
            model.authEmp_txt = reader["authEmp_txt"]?.ToString();
            model.authDate_txt = reader["authDate_txt"]?.ToString();

            model.approve_step = Convert.ToInt32(reader["approve_step"]);
            model.flagm_proc = Convert.ToInt32(reader["flagm_proc"]);
            model.procure_flag = Convert.ToInt32(reader["procure_flag"]);
            model.procure_remark = reader["procure_remark"]?.ToString();
            model.codelog = reader["codelog"]?.ToString();
            model.remarkEmp = reader["remarkEmp"]?.ToString();
            model.remarkCount = reader["remarkCount"]?.ToString();
            model.remarkAuth = reader["remarkAuth"]?.ToString();
            model.total_disc = Convert.ToDecimal(reader["total_disc"]?.ToString());
            model.quo_return = Convert.ToInt32(reader["quo_return"]?.ToString());
            model.projectname = reader["projectname"]?.ToString();
          }
        }
      }
    }

    return Json(model);
  }
  [HttpGet]
  public JsonResult GetSuppByDocno(string docno, int sugg_item)
  {
    List<PRSuggVendorModel> result = new List<PRSuggVendorModel>();

    IConfiguration _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    string connStr = _configuration["ConnectionStrings:BtProcureConn"];

    using (SqlConnection con = new SqlConnection(connStr))
    {
      using (SqlCommand cmd = new SqlCommand("SP_GetSuppByDocno", con))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@docno", docno ?? "");
        cmd.Parameters.AddWithValue("@sugg_item", sugg_item);

        con.Open();
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
          while (reader.Read())
          {
            result.Add(new PRSuggVendorModel
            {
              id = reader["id"] != DBNull.Value ? Guid.Parse(reader["id"].ToString()) : Guid.Empty,
              ref_prid = reader["ref_prid"] != DBNull.Value ? Guid.Parse(reader["ref_prid"].ToString()) : Guid.Empty,
              sugg_item = Convert.ToInt32(reader["sugg_item"]),
              vencode = reader["vencode"]?.ToString(),
              venvc = reader["venvc"]?.ToString(),
              vencontact = reader["vencontact"]?.ToString(),
              venemail = reader["venemail"]?.ToString(),
              ventelfax = reader["ventelfax"]?.ToString(),
              venremark = reader["venremark"]?.ToString(),
              name_supp = reader["name_supp"]?.ToString(),
              quoref_supp = reader["quoref_supp"]?.ToString(),
              refnodt_supp = reader["refnodt_supp"] as DateTime?,
              create_dt = reader["create_dt"] as DateTime?,
              update_dt = reader["update_dt"] as DateTime?,
              currency = reader["currency"]?.ToString()
            });
          }
        }
      }
    }

    return Json(result);
  }
  [HttpPost]
  public JsonResult AddPRItemDetail(PRItemDetailModel item)
  {
    string[] result = new string[2];
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    try
    {
      con.Open();
      SqlCommand cmd = new SqlCommand("SP_InsertPRItemDetail", con);
      cmd.CommandType = CommandType.StoredProcedure;

      cmd.Parameters.AddWithValue("@prno", item.prno);
      cmd.Parameters.AddWithValue("@item_btnumber", item.item_btnumber ?? "");
      cmd.Parameters.AddWithValue("@item_descript", item.item_descript ?? "");
      cmd.Parameters.AddWithValue("@item_model", item.item_model ?? "");
      cmd.Parameters.AddWithValue("@item_acccode", item.item_acccode ?? "");
      cmd.Parameters.AddWithValue("@item_costdep", item.item_costdep ?? "");
      cmd.Parameters.AddWithValue("@item_qty", item.item_qty ?? 0);

      cmd.Parameters.AddWithValue("@item_unit", item.item_unit ?? "");
      cmd.Parameters.AddWithValue("@item_unitprice", item.item_unitprice ?? "0");
      cmd.Parameters.AddWithValue("@item_amount", item.item_amount ?? 0);

      cmd.ExecuteNonQuery();
      result[0] = "0";
      result[1] = "Success";
    }
    catch (Exception ex)
    {
      result[0] = "-1";
      result[1] = ex.Message;
    }

    return Json(result);
  }
  [HttpGet]
  public JsonResult GetPRItemDetailByDocno(string prno)
  {
    List<PRItemDetailModel> list = new List<PRItemDetailModel>();
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();
    SqlCommand cmd = new SqlCommand("SP_GetPRItemDetailByDocNo", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prno", prno);

    using SqlDataReader rdr = cmd.ExecuteReader();
    while (rdr.Read())
    {
      list.Add(new PRItemDetailModel
      {
        id = Guid.Parse(rdr["id"].ToString()),
        item_btnumber = rdr["item_btnumber"].ToString(),
        item_descript = rdr["item_descript"].ToString(),
        item_model = rdr["item_model"].ToString(),
        item_acccode = rdr["item_acccode"].ToString(),
        item_costdep = rdr["item_costdep"].ToString(),
        item_qty = Convert.ToDecimal(rdr["item_qty"]),
        item_unit = rdr["item_unit"].ToString(),
        item_unitprice = rdr["item_unitprice"].ToString(),
        item_amount = Convert.ToDecimal(rdr["item_amount"]),
        item_disc = Convert.ToDecimal(rdr["item_disc"])
      });
    }

    return Json(list);
  }

  [HttpGet]
  public JsonResult loadFileAttachTable(Guid prid)
  {
    List<FileAttachedModel> list = new List<FileAttachedModel>();
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();
    SqlCommand cmd = new SqlCommand("SP_GetFileAttachTable", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prid", prid);

    using SqlDataReader rdr = cmd.ExecuteReader();
    while (rdr.Read())
    {
      list.Add(new FileAttachedModel
      {
        id = Guid.Parse(rdr["id"].ToString()),
        ref_prid = Guid.Parse(rdr["ref_prid"].ToString()),
        filetype = int.Parse(rdr["filetype"].ToString()),
        filetype_txt = rdr["filetype_txt"].ToString(),
        filename = rdr["filename"].ToString(),
        filepath = rdr["filepath"].ToString()
      });
    }

    return Json(list);
  }
  [HttpPost]
  public JsonResult DeletePRItemDetailById(Guid id)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_DeletePRItemDetailById", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@id", id);
    int result = cmd.ExecuteNonQuery();

    return Json(new string[] { result > 0 ? "0" : "-1", "Deleted" });
  }
  [HttpPost]
  public JsonResult DeleteFileItemById_Bak(Guid id)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_DeleteFileItemById", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@id", id);
    int result = cmd.ExecuteNonQuery();

    return Json(new string[] { result > 0 ? "0" : "-1", "Deleted" });
  }
  [HttpPost]
  public JsonResult DeleteFileItemById(Guid id, string filepath)
  {
    string[] ret = new string[2];
    try
    {
      // 1) ลบไฟล์จากดิสก์ (map web path -> physical path)
      if (!string.IsNullOrWhiteSpace(filepath))
      {
        // ป้องกัน decode
        var webPath = Uri.UnescapeDataString(filepath);
        if (webPath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
        {
          var physical = Path.Combine(_env.WebRootPath, webPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
          if (System.IO.File.Exists(physical))
          {
            System.IO.File.Delete(physical);
          }
        }
      }

      // 2) ลบ record จาก DB (ผ่าน SP ของคุณ)
      using var con = new SqlConnection(_config.GetConnectionString("BtProcureConn"));
      using var cmd = new SqlCommand("SP_DeleteFileItemById", con);
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.Parameters.AddWithValue("@id", id);
      con.Open();
      cmd.ExecuteNonQuery();

      ret[0] = "0"; ret[1] = "success";
    }
    catch (Exception ex)
    {
      ret[0] = "-1"; ret[1] = ex.Message;
    }
    return Json(ret);
  }
  [HttpPost]
  public JsonResult DeleteSuggSupplierById(Guid id)
  { 
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_DeleteSuggSupplierById", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@id", id);
    int result = cmd.ExecuteNonQuery();

    return Json(new string[] { result > 0 ? "0" : "-1", "Deleted" });
  }
  [HttpPut]
  public JsonResult ReviseSuggSupplierById(ReviseSuggesVendorModel obj)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_UpdatePRSuggVendor", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prno", obj.prno ?? "");
    cmd.Parameters.AddWithValue("@sugg_item", obj.sugg_item);
    cmd.Parameters.AddWithValue("@id_supp", obj.id_supp);
    cmd.Parameters.AddWithValue("@name_supp", obj.name_supp ?? "");
    cmd.Parameters.AddWithValue("@vc_supp", obj.vc_supp ?? "");
    cmd.Parameters.AddWithValue("@contact_supp", obj.contact_supp ?? "");
    cmd.Parameters.AddWithValue("@email_supp", obj.email_supp ?? "");
    cmd.Parameters.AddWithValue("@tel_supp", obj.tel_supp ?? "");
    cmd.Parameters.AddWithValue("@remark_supp", obj.remark_supp ?? "");
    cmd.Parameters.AddWithValue("@quoref_supp", obj.quoref_supp ?? "");
    cmd.Parameters.AddWithValue("@refnodt_supp", obj.refnodt_supp == null ? (object)DBNull.Value : obj.refnodt_supp);
    int result = cmd.ExecuteNonQuery();
    return Json(new string[] { result > 0 ? "0" : "0", "Updated" });
  }
  [HttpGet]
  public JsonResult GetPRItemDetailById(Guid id)
  {
    PRItemDetailModel item = new PRItemDetailModel();
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();
    // SP_GetPRItemDetailById

    SqlCommand cmd = new SqlCommand("SP_GetPRItemDetailById", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@id", id);

    using SqlDataReader rdr = cmd.ExecuteReader();
    if (rdr.Read())
    {
      item.id = Guid.Parse(rdr["id"].ToString());
      item.item_btnumber = rdr["item_btnumber"].ToString();
      item.item_descript = rdr["item_descript"].ToString();
      item.item_model = rdr["item_model"].ToString();
      item.item_acccode = rdr["item_acccode"].ToString();
      item.item_costdep = rdr["item_costdep"].ToString();
      item.item_qty = Convert.ToDecimal(rdr["item_qty"]);
      item.item_unit = rdr["item_unit"].ToString();
      item.item_unitprice = rdr["item_unitprice"].ToString();
      item.item_amount = Convert.ToDecimal(rdr["item_amount"]);
      item.item_disc = Convert.ToDecimal(rdr["item_disc"]);
    }

    return Json(item);
  }
  [HttpPost]
  public JsonResult UpdatePRItemDetail(PRItemDetailModel item)
  {
    string[] result = new string[2];
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();
    try
    {
      SqlCommand cmd = new SqlCommand("SP_UpdatePRItemDetail", con);
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.Parameters.AddWithValue("@id", item.id);
      cmd.Parameters.AddWithValue("@item_btnumber", item.item_btnumber ?? "");
      cmd.Parameters.AddWithValue("@item_descript", item.item_descript ?? "");
      cmd.Parameters.AddWithValue("@item_model", item.item_model ?? "");
      cmd.Parameters.AddWithValue("@item_acccode", item.item_acccode ?? "");
      cmd.Parameters.AddWithValue("@item_costdep", item.item_costdep ?? "");
      cmd.Parameters.AddWithValue("@item_qty", item.item_qty ?? 0);
      cmd.Parameters.AddWithValue("@item_unit", item.item_unit ?? "");
      cmd.Parameters.AddWithValue("@item_unitprice", item.item_unitprice ?? "0");
      cmd.Parameters.AddWithValue("@item_amount", item.item_amount ?? 0);
      cmd.Parameters.AddWithValue("@item_disc", item.item_disc ?? 0);

      cmd.ExecuteNonQuery();
      result[0] = "0";
      result[1] = "Success";
    }
    catch (Exception ex)
    {
      result[0] = "-1";
      result[1] = ex.Message;
    }

    return Json(result);
  }
  public CTLAdminController(IWebHostEnvironment env, IConfiguration config)
  {
    _env = env;
    _config = config;
  }

  [HttpPost]
  public async Task<JsonResult> UploadAttachment(IFormFile file, Guid ref_prid, int filetype)
  {
    // response format: { code: 0/-1, message: "", filename: "", filepath: "" }
    var res = new { code = 0, message = "", filename = "", filepath = "" };

    try
    {
      if (file == null || file.Length == 0)
        return Json(new { code = -1, message = "ไม่พบไฟล์", filename = "", filepath = "" });

      // ตรวจขนาด ≤ 2MB
      if (file.Length > (2 * 1024 * 1024))
        return Json(new { code = -1, message = "ไฟล์เกิน 2 MB", filename = "", filepath = "" });

      // (ทางเลือก) ตรวจนามสกุลไฟล์
      var allowedExt = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".zip" };
      var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
      if (!allowedExt.Contains(ext))
        return Json(new { code = -1, message = "ชนิดไฟล์ไม่อนุญาต", filename = "", filepath = "" });

      // สร้างโฟลเดอร์ถ้ายังไม่มี
      var uploadRoot = Path.Combine(_env.WebRootPath, "uploads");
      if (!Directory.Exists(uploadRoot))
        Directory.CreateDirectory(uploadRoot);

      // ตั้งชื่อไฟล์ใหม่ ป้องกันชื่อชนกัน
      var newName = $"{Guid.NewGuid():N}{ext}";
      var savePath = Path.Combine(uploadRoot, newName);
      var webPath = $"/uploads/{newName}"; // พาธที่เก็บใน DB

      using (var stream = new FileStream(savePath, FileMode.Create))
      {
        await file.CopyToAsync(stream);
      }

      // บันทึก DB ผ่าน Stored Procedure
      using var con = new SqlConnection(_config.GetConnectionString("BtProcureConn"));
      using var cmd = new SqlCommand("SP_InsertFileAttached", con);
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.Parameters.AddWithValue("@ref_prid", ref_prid);
      cmd.Parameters.AddWithValue("@filetype", filetype);
      cmd.Parameters.AddWithValue("@filename", Path.GetFileName(file.FileName)); // ชื่อไฟล์ต้นฉบับ
      cmd.Parameters.AddWithValue("@filepath", webPath); // พาธสำหรับเว็บ

      await con.OpenAsync();
      await cmd.ExecuteNonQueryAsync();

      return Json(new { code = 0, message = "success", filename = Path.GetFileName(file.FileName), filepath = webPath });
    }
    catch (Exception ex)
    {
      return Json(new { code = -1, message = ex.Message, filename = "", filepath = "" });
    }
  }

  [HttpPut]
  public JsonResult PRApprovalStatus(ApprovalStausModel obj)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_UpdatePRApprovalStatusV200", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prno", obj.prno ?? "");
    cmd.Parameters.AddWithValue("@person", obj.person == null ? 0 : obj.person);
    cmd.Parameters.AddWithValue("@app_role", obj.app_role);
    cmd.Parameters.AddWithValue("@app_status", obj.app_status);
    cmd.Parameters.AddWithValue("@approval_remark", obj.approval_remark);
    int result = cmd.ExecuteNonQuery();
    return Json(new string[] { result > 0 ? "0" : "0", "Updated" });
  }
  [HttpPut]
  public JsonResult UpdatePRMultiSectionhead(ApprovalStausModel obj)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_UpdatePRMultiSectionhead", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prno", obj.prno ?? "");
    cmd.Parameters.AddWithValue("@person", obj.person);
    cmd.Parameters.AddWithValue("@approval_remark", obj.approval_remark);
    int result = cmd.ExecuteNonQuery();
    return Json(new string[] { result > 0 ? "0" : "0", "Updated" });
  }
  [HttpPut]
  public JsonResult UpdatePRMultiSectionheadReject(ApprovalStausModel obj)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_UpdatePRMultiSectionheadReject", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prno", obj.prno ?? "");
    cmd.Parameters.AddWithValue("@person", obj.person);
    cmd.Parameters.AddWithValue("@approval_remark", obj.approval_remark);
    int result = cmd.ExecuteNonQuery();
    return Json(new string[] { result > 0 ? "0" : "0", "Updated" });
  }
  [HttpPut]
  public JsonResult UpdatePRRemark(PRHeaderModel obj)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_UpdatePRRemark", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prno", obj.prno ?? "");
    cmd.Parameters.AddWithValue("@pub_remark", obj.pub_remark);
    int result = cmd.ExecuteNonQuery();
    return Json(new string[] { result > 0 ? "0" : "0", "Updated" });
  }
  [HttpPut]
  public JsonResult UpdateProcureMailFlag(PRHeaderModel obj)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_UpdateProcMailFlag", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prno", obj.prno ?? "");
    cmd.Parameters.AddWithValue("@flagm_proc", obj.flagm_proc);
    int result = cmd.ExecuteNonQuery();
    return Json(new string[] { result > 0 ? "0" : "0", "Updated" });
  }
  [HttpPut]
  public JsonResult UpdateProcStatus(PRHeaderModel obj)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_UpdateProcStatus", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prno", obj.prno ?? "");
    cmd.Parameters.AddWithValue("@procure_flag", obj.procure_flag);
    cmd.Parameters.AddWithValue("@pr_recvdt", obj.pr_recvdt);
    cmd.Parameters.AddWithValue("@pr_recvpono", obj.pr_recvpono);
    cmd.Parameters.AddWithValue("@procure_remark", obj.procure_remark ?? "");

    int result = cmd.ExecuteNonQuery();
    return Json(new string[] { result > 0 ? "0" : "0", "Updated" });
  }
  [HttpPut]
  public JsonResult UpdatePRCodeLog(PRHeaderModel obj)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_UpdatePRCodeLog", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prno", obj.prno ?? "");
    cmd.Parameters.AddWithValue("@codelog", obj.codelog);
    int result = cmd.ExecuteNonQuery();
    return Json(new string[] { result > 0 ? "0" : "0", "Updated" });
  }

  [HttpPut]
  public JsonResult AddHisPRRemark(PrHistoryRemarkModel obj)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();
    string emp_code = HttpContext.Session.GetString(SessionModel.EMPCODE);
    SqlCommand cmd = new SqlCommand("SP_InsertHistoryRemark", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prno", obj.prno ?? "");
    cmd.Parameters.AddWithValue("@empcode", emp_code ?? "");
    cmd.Parameters.AddWithValue("@remarks", obj.remarks);
    int result = cmd.ExecuteNonQuery();
    return Json(new string[] { result > 0 ? "0" : "0", "Updated" });
  }
  [HttpGet]
  public JsonResult GetPRHisRemark(string prno)
  {
    List<PrHistoryRemarkModel> list = new List<PrHistoryRemarkModel>();
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();
    SqlCommand cmd = new SqlCommand("SP_GetHistoryRemark", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prno", prno);

    using SqlDataReader rdr = cmd.ExecuteReader();
    while (rdr.Read())
    {
      list.Add(new PrHistoryRemarkModel
      {
        id = Guid.Parse(rdr["id"].ToString()),
        empcode = rdr["empcode"].ToString(),
        remarks = rdr["remarks"].ToString(),
        create_dt_txt = rdr["create_dt_txt"].ToString()
      });
    }

    return Json(list);
  }
  [HttpDelete]
  public JsonResult DeleteHisPRRemark(PrHistoryRemarkModel obj)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_DeleteHistoryRemark", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@id", obj.id);
    int result = cmd.ExecuteNonQuery();
    return Json(new string[] { result > 0 ? "0" : "0", "Deleted" });
  }
  [HttpGet]
  public JsonResult GetAllPRData()
  {
    List<PRHeaderViewModel> prList = new List<PRHeaderViewModel>();
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    using SqlCommand cmd = new SqlCommand("SP_GetProcureAllPR", con);
    cmd.CommandType = CommandType.StoredProcedure;

    try
    {
      con.Open();
      using SqlDataReader rdr = cmd.ExecuteReader();
      while (rdr.Read())
      {
        prList.Add(new PRHeaderViewModel
        {
          codelog = rdr["codelog"].ToString(),
          prstatus_txt = rdr["prstatus_txt"].ToString(),
          projectno = rdr["projectno"].ToString(),
          prno = rdr["prno"].ToString(),
          approx_dt_txt = rdr["approx_dt_txt"].ToString(),
          update_dt_txt = rdr["update_dt_txt"].ToString(),
          empcode_txt = rdr["empcode_txt"].ToString(),
          pr_reason = rdr["pr_reason"].ToString(),
          pr_recvdt_txt = rdr["pr_recvdt_txt"].ToString(),
          pr_recvpono = rdr["pr_recvpono"].ToString(),
          reqDepCode = rdr["reqDepCode"].ToString(),
          create_dt = Convert.ToDateTime(rdr["create_dt"]),
          create_dt_txt = rdr["create_dt_txt"].ToString(),
          procure_remark = rdr["procure_remark"].ToString(),
          purpose_type = rdr["purpose_type"] != DBNull.Value ? Convert.ToInt32(rdr["purpose_type"]) : 0,
          appEmp_txt = rdr["appEmp_txt"].ToString(),
          appEmp2_txt = rdr["appEmp2_txt"].ToString(),
          countFlag_txt = rdr["countFlag_txt"].ToString(),
          authEmp_txt = rdr["authEmp_txt"].ToString(),

          total_disc = rdr["total_disc"] != DBNull.Value ? Convert.ToDecimal(rdr["total_disc"]) : 0,
          total_exp = rdr["total_exp"] != DBNull.Value ? Convert.ToDecimal(rdr["total_exp"]) : 0,
          prcurrency = rdr["prcurrency"].ToString()
        });
      }
    }
    catch (Exception ex)
    {
      return Json(new { data = prList, error = ex.Message });
    }
    return Json(new { data = prList });
  }
  public IActionResult PrintPR(string prno)
  {
    string UIDPRID = "";
    PRHeaderModel header = new PRHeaderModel();
    IConfiguration _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    string connStr = _configuration["ConnectionStrings:BtProcureConn"];
    //--> Start of Header
    using (SqlConnection conHead = new SqlConnection(connStr))
    {
      using (SqlCommand cmd = new SqlCommand("SP_GetPRDocno", conHead))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@docno", prno ?? "");

        conHead.Open();
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            UIDPRID = reader["id"].ToString();
            header.id = reader["id"]?.ToString();
            header.prno = reader["prno"]?.ToString();
            header.projectno = reader["projectno"]?.ToString();
            header.empcode = reader["empcode"]?.ToString();
            header.empcode_txt = reader["empcode_txt"]?.ToString();
            header.approx_type = reader["approx_type"] as int?;
            header.approx_dt = reader["approx_dt"] as DateTime?;
            header.approx_dt_txt = reader["approx_dt_txt"]?.ToString();
            header.invcreditno = reader["invcreditno"]?.ToString();
            header.purpose_type = reader["purpose_type"] as int?;
            header.ref_docs = reader["ref_docs"]?.ToString();
            header.pr_reason = reader["pr_reason"]?.ToString();
            header.pr_recvdt = reader["pr_recvdt"] as DateTime?;
            header.pr_recvpono = reader["pr_recvpono"]?.ToString();
            header.attach_flag = reader["attach_flag"]?.ToString();
            header.reqDepCode = reader["reqDepCode"]?.ToString();
            header.reqDate = reader["reqDate"] as DateTime?;
            header.reqDate_txt = reader["reqDate_txt"]?.ToString();
            header.reqFlag = reader["reqFlag"] as int?;
            header.appEmp = reader["appEmp"]?.ToString();
            header.appDate = reader["appDate"] as DateTime?;
            header.appFlag = reader["appFlag"] as int?;
            header.countEmp = reader["countEmp"]?.ToString();
            header.countDate = reader["countDate"] as DateTime?;
            header.countFlag = reader["countFlag"] as int?;
            header.authEmp = reader["authEmp"]?.ToString();
            header.authDate = reader["authDate"] as DateTime?;
            header.authFlag = reader["authFlag"] as int?;
            header.prstatus = Convert.ToInt32(reader["prstatus"]);
            header.create_dt = reader["create_dt"] as DateTime?;
            header.update_dt = reader["update_dt"] as DateTime?;
            header.update_dt_txt = reader["update_dt_txt"]?.ToString();
            header.prstatus_txt = reader["prstatus_txt"]?.ToString();
            header.pub_remark = reader["pub_remark"]?.ToString();
            header.prcurrency = reader["prcurrency"]?.ToString();
            header.reqEmail = reader["reqEmail"]?.ToString();
            header.appEmail = reader["appEmail"]?.ToString();
            header.CountEmail = reader["CountEmail"]?.ToString();
            header.authEmail = reader["authEmail"]?.ToString();
            header.pr_recvdt_txt = reader["pr_recvdt_txt"].ToString();
            
            header.appEmp_txt = reader["appEmp_txt"]?.ToString();
            header.appDate_txt = reader["appDate_txt"]?.ToString();
            header.countEmp_txt = reader["countEmp_txt"]?.ToString();
            header.countDate_txt = reader["countDate_txt"]?.ToString();
            header.authEmp_txt = reader["authEmp_txt"]?.ToString();
            header.authDate_txt = reader["authDate_txt"]?.ToString();
            
            header.approve_step = Convert.ToInt32(reader["approve_step"]);
            header.flagm_proc = Convert.ToInt32(reader["flagm_proc"]);
            header.procure_flag = Convert.ToInt32(reader["procure_flag"]);
            header.procure_remark = reader["procure_remark"]?.ToString();
            header.codelog = reader["codelog"]?.ToString();
            header.remarkEmp = reader["remarkEmp"]?.ToString();
            header.remarkCount = reader["remarkCount"]?.ToString();
            header.remarkAuth = reader["remarkAuth"]?.ToString();

          }
        }
        conHead.Close();
      }
    }
    //--> End ofHeader
    //--> Start of Detail
    List<PRItemDetailModel> detail = new List<PRItemDetailModel>();
    using SqlConnection conDet = new SqlConnection(connStr);
    SqlCommand cmdDetail = new SqlCommand("SP_GetPRItemDetailByDocNo", conDet);
    conDet.Open();    
    cmdDetail.CommandType = CommandType.StoredProcedure;
    cmdDetail.Parameters.AddWithValue("@prno", prno);

    using SqlDataReader rdr = cmdDetail.ExecuteReader();
    while (rdr.Read())
    {
      detail.Add(new PRItemDetailModel
      {
        id = Guid.Parse(rdr["id"].ToString()),
        item_btnumber = rdr["item_btnumber"].ToString(),
        item_descript = rdr["item_descript"].ToString(),
        item_model = rdr["item_model"].ToString(),
        item_acccode = rdr["item_acccode"].ToString(),
        item_costdep = rdr["item_costdep"].ToString(),
        item_qty = Convert.ToDecimal(rdr["item_qty"]),
        item_unit = rdr["item_unit"].ToString(),
        item_unitprice = rdr["item_unitprice"].ToString(),
        item_amount = Convert.ToDecimal(rdr["item_amount"])
      });
    }
    conDet.Close();
    //--> End of Detail
    //--> Start of Vendor
    List<PRSuggVendorModel> resultVend = new List<PRSuggVendorModel>();

    using (SqlConnection conVend = new SqlConnection(connStr))
    {
      using (SqlCommand cmd = new SqlCommand("SP_GetSuppByDocno", conVend))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@docno", prno ?? "");
        cmd.Parameters.AddWithValue("@sugg_item", 99); // all items

        conVend.Open();
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
          while (reader.Read())
          {
            resultVend.Add(new PRSuggVendorModel
            {
              id = reader["id"] != DBNull.Value ? Guid.Parse(reader["id"].ToString()) : Guid.Empty,
              ref_prid = reader["ref_prid"] != DBNull.Value ? Guid.Parse(reader["ref_prid"].ToString()) : Guid.Empty,
              sugg_item = Convert.ToInt32(reader["sugg_item"]),
              vencode = reader["vencode"]?.ToString(),
              venvc = reader["venvc"]?.ToString(),
              vencontact = reader["vencontact"]?.ToString(),
              venemail = reader["venemail"]?.ToString(),
              ventelfax = reader["ventelfax"]?.ToString(),
              venremark = reader["venremark"]?.ToString(),
              name_supp = reader["name_supp"]?.ToString(),
              quoref_supp = reader["quoref_supp"]?.ToString(),
              refnodt_supp = reader["refnodt_supp"] as DateTime?,
              create_dt = reader["create_dt"] as DateTime?,
              update_dt = reader["update_dt"] as DateTime?,
              currency = reader["currency"]?.ToString(),
              refnodt_supp_txt = reader["refnodt_supp_txt"]?.ToString()
            });
          }
        }
        conVend.Close();
      }
    }
    //--> End of Vendor
    //--> Start of Attach File

    List<FileAttachedModel> listAttach = new List<FileAttachedModel>();
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection conAttach = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    conAttach.Open();
    SqlCommand cmdAttach = new SqlCommand("SP_GetFileAttachTable", conAttach);
    cmdAttach.CommandType = CommandType.StoredProcedure;

    SqlParameter sqlParameter = cmdAttach.Parameters.AddWithValue("@prid", Guid.Parse(UIDPRID));

    using SqlDataReader rdrAttach = cmdAttach.ExecuteReader();
    while (rdrAttach.Read())
    {
      listAttach.Add(new FileAttachedModel
      {
        id = Guid.Parse(rdrAttach["id"].ToString()),
        ref_prid = Guid.Parse(rdrAttach["ref_prid"].ToString()),
        filetype = int.Parse(rdrAttach["filetype"].ToString()),
        filetype_txt = rdrAttach["filetype_txt"].ToString(),
        filename = rdrAttach["filename"].ToString(),
        filepath = rdrAttach["filepath"].ToString()
      });
    }

    //--> End of Attach File

    var viewModel = new PRPrintViewModel
    {
      PRHeader = header,
      DetailList = detail,
      VendorList = resultVend,
      AttachList = listAttach
    };

    return View("PR_PrintReport", viewModel);
  }

  [HttpGet]
  public IActionResult ItemPartList()
  {
    if (HttpContext.Session.GetString(SessionModel.UROLEADMIN) != null)
    {
      ViewData["UROLEADMIN"] = HttpContext.Session.GetString(SessionModel.UROLEADMIN);
    }
    return View();
  }
  [HttpGet]
  public IActionResult GetItempartList(string? search)
  {
    IConfiguration _configuration = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json")
                      .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    var results = new List<dynamic>();

    using (var conn = new SqlConnection(DBConn))
    {
      conn.Open();
      using (var command = new SqlCommand("SP_GetProcureItempart", conn))
      {
        command.CommandType = CommandType.StoredProcedure;
        //itemdoc, itemno, itemname

        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            results.Add(new
            {
              itemno = reader["itemno"]?.ToString(),
              itemname = reader["itemname"]?.ToString(),
              itemdoc = reader["itemdoc"]?.ToString()
            });
          }
        }
      }
      conn.CloseAsync();
    }

    //List<string> lstStr = daAccess.GetOpenOrCloseDoc();
    //ViewData["OPENDOC"] = lstStr[0];
    //ViewData["CLOSEDOC"] = lstStr[1];

    return Json(results.ToList());
  }

  [HttpPost]
  public JsonResult AddNewBTItempart(BtItemPartModel obj)
  {

    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_ADDTBProcureItemParts", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@itemno", SqlDbType.VarChar).Value = obj.itemno == null ? "" : obj.itemno;
      cmnd.Parameters.AddWithValue("@itemname", SqlDbType.NVarChar).Value = obj.itemname == null ? "" : obj.itemname;
      cmnd.Parameters.AddWithValue("@itemdoc", SqlDbType.VarChar).Value = obj.itemdoc == null ? "" : obj.itemdoc;
      cmnd.Parameters.Add("@RESULT", SqlDbType.Int);
      cmnd.Parameters["@RESULT"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@RESULT"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }
    con.Close();
    return Json(strMsg);
  }
  [HttpPost]
  public JsonResult SaveItempart(BtItemPartModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_SaveItempart", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@itemnoTmp", SqlDbType.VarChar).Value = obj.itemnoTmp == null ? "" : obj.itemnoTmp;
      cmnd.Parameters.AddWithValue("@itemno", SqlDbType.VarChar).Value = obj.itemno == null ? "" : obj.itemno;
      cmnd.Parameters.AddWithValue("@itemname", SqlDbType.NVarChar).Value = obj.itemname == null ? "" : obj.itemname;
      cmnd.Parameters.AddWithValue("@itemdoc", SqlDbType.VarChar).Value = obj.itemdoc == null ? "" : obj.itemdoc;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }

    con.Close();
    return Json(strMsg);
  }
  [HttpDelete]
  public JsonResult DelItempart(BtItemPartModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    SqlConnection con = null;
    int flag = 0;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_DeleteItempart", con);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@itemno", SqlDbType.VarChar).Value = obj.itemno == null ? "" : obj.itemno;
      cmnd.Parameters.Add("@flag", SqlDbType.Int);
      cmnd.Parameters["@flag"].Direction = ParameterDirection.Output;
      cmnd.ExecuteNonQuery();
      flag = Convert.ToInt32(cmnd.Parameters["@flag"].Value);
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error " + ex.Message;
      strMsg[2] = "0";
    }
    finally
    {
      strMsg[0] = "1";
      strMsg[1] = "success";
      strMsg[2] = flag.ToString();
    }

    con.Close();
    return Json(strMsg);
  }

  [HttpPut]
  public JsonResult SaveQuoReturn(PRHeaderViewModel obj)
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    using SqlConnection con = new SqlConnection(config.GetConnectionString("BtProcureConn"));
    con.Open();

    SqlCommand cmd = new SqlCommand("SP_SaveQuoReturn", con);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@prno", obj.prno);
    cmd.Parameters.AddWithValue("@quo_return", obj.quo_return);
    int result = cmd.ExecuteNonQuery();
    return Json(new string[] { result > 0 ? "0" : "0", "Updated" });
  }
  [HttpPut]
  public IActionResult SaveEmailLog([FromBody] EmailLogRequestModel req)
  {
    try
    {
      var root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
      if (!Directory.Exists(root)) Directory.CreateDirectory(root);
      var logFile = Path.Combine(root, "email_logs.txt");
      // Format log เช่น: 2026-01-09 10:25 | from -> to | subject
      var logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {req.MailFrom} -> {req.MailTo} | {req.Subject}{Environment.NewLine}";
      System.IO.File.AppendAllText(logFile, logLine, System.Text.Encoding.UTF8);
      return Ok(new { status = "ok" });
    }
    catch (Exception ex)
    {
      return BadRequest(new { status = "error", msg = ex.Message });
    }
  }
}
