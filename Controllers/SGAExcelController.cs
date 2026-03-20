using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspnetCoreMvcFull.Models;

using Microsoft.Data.SqlClient;
using System.Data;
using ExcelDataReader;
using System.Runtime.CompilerServices;
using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Emit;
using System.Globalization;

namespace AspnetCoreMvcFull.Controllers;

public class SGAExcelController : Controller
{
  public DataAccess daAccess = new DataAccess();

  [HttpGet]
  public IActionResult PRDRecordSheetListExcel()
  {
    if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    {
      List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
      ViewData["OPENDOC"] = lstStr[0];
      ViewData["CLOSEDOC"] = lstStr[1];
    }
    return View();
  }
  [HttpGet]
  public IActionResult GetPRDRecordSheetListExcel(string? search)
  {
    IConfiguration _configuration = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json")
                      .Build();
      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];

      var results = new List<dynamic>();
      string itemname = "";
      using (var conn = new SqlConnection(DBConn))
      {
      //rec_date, mas_wo, mas_itemno, mas_opr, mas_qty, mas_stdtime, mas_resource, mas_mc, mas_lab, emp_code, rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff
      //rec_date, mas_wo, mas_itemno, mas_itemname, mas_opr, mas_qty, mas_stdtime, mas_resource, mas_mc, mas_lab, emp_code, rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff
          conn.Open();
          using (var command = new SqlCommand("SP_ExcelExpTimeSheetByDate", conn))
          {
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("@dt_st", DBNull.Value);
              command.Parameters.AddWithValue("@dt_en", DBNull.Value);

              using (var reader = command.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      results.Add(new
                      {
                          rec_date = reader["rec_date"]?.ToString(),
                          mas_wo = reader["mas_wo"]?.ToString(),
                          mas_itemno = reader["mas_itemno"]?.ToString(),
                          mas_itemname = reader["mas_itemname"]?.ToString(),
                          mas_opr = reader["mas_opr"]?.ToString(),
                          mas_qty = reader["mas_qty"] != DBNull.Value ? Convert.ToInt32(reader["mas_qty"]) : 0,

                          mas_stdtime = reader["mas_stdtime"] != DBNull.Value ? Convert.ToDecimal(reader["mas_stdtime"]) : 0.0m,
                          mas_resource = reader["mas_resource"]?.ToString(),
                          mas_mc = reader["mas_mc"] != DBNull.Value ? Convert.ToDecimal(reader["mas_mc"]) : 0.0m,
                          mas_lab = reader["mas_lab"] != DBNull.Value ? Convert.ToDecimal(reader["mas_lab"]) : 0.0m,
                          emp_code = reader["emp_code"]?.ToString(), 
                          
                          rec_setup = reader["mas_stdtime"] != DBNull.Value ? Convert.ToDecimal(reader["mas_stdtime"]) : 0.0m,
                          rec_mc = reader["rec_mc"] != DBNull.Value ? Convert.ToDecimal(reader["rec_mc"]) : 0.0m,
                          rec_lab = reader["rec_lab"] != DBNull.Value ? Convert.ToDecimal(reader["rec_lab"]) : 0.0m,
                          rec_aqty = reader["rec_aqty"] != DBNull.Value ? Convert.ToInt32(reader["rec_aqty"]) : 0,
                          rec_atotal = reader["rec_atotal"] != DBNull.Value ? Convert.ToInt32(reader["rec_atotal"]) : 0,
                          rec_eff = reader["rec_eff"] != DBNull.Value ? Convert.ToDecimal(reader["rec_eff"]) : 0.0m
                          
                      });
                  }
              }
              
          }
          conn.CloseAsync();
      }

    //return Ok(results);


    if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    {
      List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
      ViewData["OPENDOC"] = lstStr[0];
      ViewData["CLOSEDOC"] = lstStr[1];
    }
    return Ok(new { item_name = itemname, data = results.ToList() });
  }

  [HttpGet]
  public async Task<IActionResult> GetExportTimeSheetData(DateTime? dt_st, DateTime? dt_en)
  {
      IConfiguration _configuration = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json")
                      .Build();
      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];

      var results = new List<dynamic>();
      string itemname = "";
      using (var conn = new SqlConnection(DBConn))
      {
      //rec_date, mas_wo, mas_itemno, mas_opr, mas_qty, mas_stdtime, mas_resource, mas_mc, mas_lab, emp_code, rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff
      //rec_date, mas_wo, mas_itemno, mas_itemname, mas_opr, mas_qty, mas_stdtime, mas_resource, mas_mc, mas_lab, emp_code, rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff
          await conn.OpenAsync();
          using (var command = new SqlCommand("SP_ExcelExpTimeSheetByDate", conn))
          {
              command.CommandType = CommandType.StoredProcedure;
              DateTime _st = (DateTime)dt_st;
              DateTime _en = (DateTime)dt_en;
              command.Parameters.AddWithValue("@dt_st", _st);
              command.Parameters.AddWithValue("@dt_en", _en);

              using (var reader = await command.ExecuteReaderAsync())
              {
                  while (await reader.ReadAsync())
                  {
                      results.Add(new
                      {
                        // น้องเตยขอแสดงบางคอลัมภ์

                        rec_date = reader["rec_date"]?.ToString(),
                        mas_wo = reader["mas_wo"]?.ToString(),
                        mas_itemno = reader["mas_itemno"]?.ToString(),
                        mas_itemname = reader["mas_itemname"]?.ToString(),
                        mas_opr = reader["mas_opr"]?.ToString(),
                        mas_qty = reader["mas_qty"] != DBNull.Value ? Convert.ToInt32(reader["mas_qty"]) : 0,

                        mas_stdtime = reader["mas_stdtime"] != DBNull.Value ? Convert.ToDecimal(reader["mas_stdtime"]) : 0.0m,
                        mas_resource = reader["mas_resource"]?.ToString(),
                        mas_mc = reader["mas_mc"] != DBNull.Value ? Convert.ToDecimal(reader["mas_mc"]) : 0.0m,
                        mas_lab = reader["mas_lab"] != DBNull.Value ? Convert.ToDecimal(reader["mas_lab"]) : 0.0m,
                        emp_code = reader["emp_code"]?.ToString(),

                        rec_setup = reader["rec_setup"] != DBNull.Value ? Convert.ToDecimal(reader["rec_setup"]) : 0.0m,
                        rec_mc = reader["rec_mc"] != DBNull.Value ? Convert.ToDecimal(reader["rec_mc"]) : 0.0m,
                        rec_lab = reader["rec_lab"] != DBNull.Value ? Convert.ToDecimal(reader["rec_lab"]) : 0.0m,
                        rec_aqty = reader["rec_aqty"] != DBNull.Value ? Convert.ToInt32(reader["rec_aqty"]) : 0,
                        rec_atotal = reader["rec_atotal"] != DBNull.Value ? Convert.ToInt32(reader["rec_atotal"]) : 0,
                        rec_eff = reader["rec_eff"] != DBNull.Value ? Convert.ToDecimal(reader["rec_eff"]) : 0.0m
                        /*
                        mas_wo = reader["mas_wo"]?.ToString(),
                        mas_opr = reader["mas_opr"]?.ToString(),

                        mas_resource = reader["mas_resource"]?.ToString(),

                        rec_setup = reader["mas_stdtime"] != DBNull.Value ? Convert.ToDecimal(reader["mas_stdtime"]) : 0.0m,
                        rec_mc = reader["rec_mc"] != DBNull.Value ? Convert.ToDecimal(reader["rec_mc"]) : 0.0m,
                        rec_lab = reader["rec_lab"] != DBNull.Value ? Convert.ToDecimal(reader["rec_lab"]) : 0.0m,
                        rec_aqty = reader["rec_aqty"] != DBNull.Value ? Convert.ToInt32(reader["rec_aqty"]) : 0
                        */
                      });
                  }
              }
              
          }
          conn.CloseAsync();
      }

      //return Ok(results);
      return Ok(new { item_name = itemname, data = results });
  }

  [HttpGet]
  public async Task<IActionResult> GetExportTimeSheetDataDash(DateTime? dt_st, DateTime? dt_en)
  {
      IConfiguration _configuration = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json")
                      .Build();
      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];

      var results = new List<dynamic>();
      string itemname = "";
      using (var conn = new SqlConnection(DBConn))
      {
      //rec_date, mas_wo, mas_itemno, mas_opr, mas_qty, mas_stdtime, mas_resource, mas_mc, mas_lab, emp_code, rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff
      //rec_date, mas_wo, mas_itemno, mas_itemname, mas_opr, mas_qty, mas_stdtime, mas_resource, mas_mc, mas_lab, emp_code, rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff
          await conn.OpenAsync();
          using (var command = new SqlCommand("SP_DashExcelExpTimeSheetByDate", conn))
          {
              command.CommandType = CommandType.StoredProcedure;
              DateTime _st = (DateTime)dt_st;
              DateTime _en = (DateTime)dt_en;
              command.Parameters.AddWithValue("@dt_st", _st);
              command.Parameters.AddWithValue("@dt_en", _en);

              using (var reader = await command.ExecuteReaderAsync())
              {
                  while (await reader.ReadAsync())
                  {
                      results.Add(new
                      {
                        
                        rec_date = reader["rec_date"]?.ToString(),
                        mas_wo = reader["mas_wo"]?.ToString(),
                        mas_itemno = reader["mas_itemno"]?.ToString(),
                        mas_itemname = reader["mas_itemname"]?.ToString(),
                        mas_opr = reader["mas_opr"]?.ToString(),
                        mas_qty = reader["mas_qty"] != DBNull.Value ? Convert.ToInt32(reader["mas_qty"]) : 0,

                        mas_stdtime = reader["mas_stdtime"] != DBNull.Value ? Convert.ToDecimal(reader["mas_stdtime"]) : 0.0m,
                        mas_resource = reader["mas_resource"]?.ToString(),
                        mas_mc = reader["mas_mc"] != DBNull.Value ? Convert.ToDecimal(reader["mas_mc"]) : 0.0m,
                        mas_lab = reader["mas_lab"] != DBNull.Value ? Convert.ToDecimal(reader["mas_lab"]) : 0.0m,
                        emp_code = reader["emp_code"]?.ToString(),

                        rec_setup = reader["rec_setup"] != DBNull.Value ? Convert.ToDecimal(reader["rec_setup"]) : 0.0m,
                        rec_mc = reader["rec_mc"] != DBNull.Value ? Convert.ToDecimal(reader["rec_mc"]) : 0.0m,
                        rec_lab = reader["rec_lab"] != DBNull.Value ? Convert.ToDecimal(reader["rec_lab"]) : 0.0m,
                        rec_aqty = reader["rec_aqty"] != DBNull.Value ? Convert.ToInt32(reader["rec_aqty"]) : 0,
                        rec_atotal = reader["rec_atotal"] != DBNull.Value ? Convert.ToInt32(reader["rec_atotal"]) : 0,
                        rec_eff = reader["rec_eff"] != DBNull.Value ? Convert.ToDecimal(reader["rec_eff"]) : 0.0m

                      });
                  }
              }
              
          }
          conn.CloseAsync();
      }

      //return Ok(results);
      return Ok(new { item_name = itemname, data = results });
  }

}
