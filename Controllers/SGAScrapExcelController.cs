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

public class SGAScrapExcelController : Controller
{
  public DataAccess daAccess = new DataAccess();

  [HttpGet]
  public IActionResult PRDRecordScrapListExcel()
  {
    if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    {
      List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
      ViewData["OPENDOC"] = lstStr[0];
      ViewData["CLOSEDOC"] = lstStr[1];
    }
    return View();
  }
  //[HttpGet]
  //public IActionResult GetPRDRecordSheetListExcel(string? search)
  //{
  //  IConfiguration _configuration = new ConfigurationBuilder()
  //                    .SetBasePath(Directory.GetCurrentDirectory())
  //                    .AddJsonFile("appsettings.json")
  //                    .Build();
  //    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];

  //    var results = new List<dynamic>();
  //    string itemname = "";
  //    using (var conn = new SqlConnection(DBConn))
  //    {
  //    //rec_date, mas_wo, mas_itemno, mas_opr, mas_qty, mas_stdtime, mas_resource, mas_mc, mas_lab, emp_code, rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff
  //    //rec_date, mas_wo, mas_itemno, mas_itemname, mas_opr, mas_qty, mas_stdtime, mas_resource, mas_mc, mas_lab, emp_code, rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff
  //        conn.Open();
  //        using (var command = new SqlCommand("SP_ExcelExpTimeSheetByDate", conn))
  //        {
  //            command.CommandType = CommandType.StoredProcedure;
  //            command.Parameters.AddWithValue("@dt_st", DBNull.Value);
  //            command.Parameters.AddWithValue("@dt_en", DBNull.Value);

  //            using (var reader = command.ExecuteReader())
  //            {
  //                while (reader.Read())
  //                {
  //                    results.Add(new
  //                    {
  //                        rec_date = reader["rec_date"]?.ToString(),
  //                        mas_wo = reader["mas_wo"]?.ToString(),
  //                        mas_itemno = reader["mas_itemno"]?.ToString(),
  //                        mas_itemname = reader["mas_itemname"]?.ToString(),
  //                        mas_opr = reader["mas_opr"]?.ToString(),
  //                        mas_qty = reader["mas_qty"] != DBNull.Value ? Convert.ToInt32(reader["mas_qty"]) : 0,

  //                        mas_stdtime = reader["mas_stdtime"] != DBNull.Value ? Convert.ToDecimal(reader["mas_stdtime"]) : 0.0m,
  //                        mas_resource = reader["mas_resource"]?.ToString(),
  //                        mas_mc = reader["mas_mc"] != DBNull.Value ? Convert.ToDecimal(reader["mas_mc"]) : 0.0m,
  //                        mas_lab = reader["mas_lab"] != DBNull.Value ? Convert.ToDecimal(reader["mas_lab"]) : 0.0m,
  //                        emp_code = reader["emp_code"]?.ToString(), 
                          
  //                        rec_setup = reader["mas_stdtime"] != DBNull.Value ? Convert.ToDecimal(reader["mas_stdtime"]) : 0.0m,
  //                        rec_mc = reader["rec_mc"] != DBNull.Value ? Convert.ToDecimal(reader["rec_mc"]) : 0.0m,
  //                        rec_lab = reader["rec_lab"] != DBNull.Value ? Convert.ToDecimal(reader["rec_lab"]) : 0.0m,
  //                        rec_aqty = reader["rec_aqty"] != DBNull.Value ? Convert.ToInt32(reader["rec_aqty"]) : 0,
  //                        rec_atotal = reader["rec_atotal"] != DBNull.Value ? Convert.ToInt32(reader["rec_atotal"]) : 0,
  //                        rec_eff = reader["rec_eff"] != DBNull.Value ? Convert.ToDecimal(reader["rec_eff"]) : 0.0m
                          
  //                    });
  //                }
  //            }
              
  //        }
  //        conn.CloseAsync();
  //    }

  //    //return Ok(results);
      
        
  //  List<string> lstStr = daAccess.GetOpenOrCloseDoc();
  //  ViewData["OPENDOC"] = lstStr[0];
  //  return Ok(new { item_name = itemname, data = results.ToList() });
  //}

  [HttpGet]
  public async Task<IActionResult> GetExportScrapData(DateTime? dt_st, DateTime? dt_en)
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
          using (var command = new SqlCommand("SP_ExcelExpScrapByDate", conn))
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
                      string rec_date = reader["rec_date"]?.ToString();
                      string mas_wo = reader["mas_wo"]?.ToString();
                      string mas_itemno = reader["mas_itemno"]?.ToString();
                      string mas_itemname = reader["mas_itemname"]?.ToString();
                      string mas_opr = reader["mas_opr"]?.ToString();
                      string emp_code = reader["emp_code"]?.ToString();

                      decimal prd_setup = reader["prd_setup"] != DBNull.Value ? Convert.ToDecimal(reader["prd_setup"]) : 0.0m;
                      decimal prd_tools = reader["prd_tools"] != DBNull.Value ? Convert.ToDecimal(reader["prd_tools"]) : 0.0m;
                      decimal prd_surf = reader["prd_surf"] != DBNull.Value ? Convert.ToDecimal(reader["prd_surf"]) : 0.0m;
                      int prd_dimout = reader["prd_dimout"] != DBNull.Value ? Convert.ToInt32(reader["prd_dimout"]) : 0;
                      int prd_other = reader["prd_other"] != DBNull.Value ? Convert.ToInt32(reader["prd_other"]) : 0;
                      string scrap_remark = reader["scrap_remark"]?.ToString();

                      decimal ven_hardness = reader["ven_hardness"] != DBNull.Value ? Convert.ToDecimal(reader["ven_hardness"]) : 0.0m;
                      decimal ven_dimout = reader["ven_dimout"] != DBNull.Value ? Convert.ToDecimal(reader["ven_dimout"]) : 0.0m;
                      decimal ven_surf = reader["ven_surf"] != DBNull.Value ? Convert.ToDecimal(reader["ven_surf"]) : 0.0m;
                      int ven_other = reader["ven_other"] != DBNull.Value ? Convert.ToInt32(reader["ven_other"]) : 0;
                      string vendor_remark = reader["vendor_remark"]?.ToString();
                      string other_remark = reader["other_remark"]?.ToString();

                      string txt_app1 = reader["txt_app1"]?.ToString();
                      string txt_app2 = reader["txt_app2"]?.ToString();
                      string txt_app3 = reader["txt_app3"]?.ToString();

                      results.Add(new
                      {
                          rec_date = rec_date, 
                          mas_wo = mas_wo, 
                          mas_itemno = mas_itemno, 
                          mas_itemname = mas_itemname, 
                          mas_opr = mas_opr, 
                          emp_code = emp_code, 
                          prd_setup = prd_setup, 
                          prd_tools = prd_tools, 
                          prd_surf = prd_surf, 
                          prd_dimout = prd_dimout, 
                          prd_other = prd_other, 
                          scrap_remark = scrap_remark, 
                          ven_hardness = ven_hardness, 
                          ven_dimout = ven_dimout, 
                          ven_surf = ven_surf, 
                          ven_other = ven_other, 
                          vendor_remark = vendor_remark,
                          other_remark = other_remark, 
                          txt_app1 = txt_app1, 
                          txt_app2 = txt_app2, 
                          txt_app3 = txt_app3

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
  public async Task<IActionResult> GetExportScrapDataDash(DateTime? dt_st, DateTime? dt_en)
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
          using (var command = new SqlCommand("SP_DashExcelExpScrapByDate", conn))
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
                      string rec_date = reader["rec_date"]?.ToString();
                      string mas_wo = reader["mas_wo"]?.ToString();
                      string mas_itemno = reader["mas_itemno"]?.ToString();
                      string mas_itemname = reader["mas_itemname"]?.ToString();
                      string mas_opr = reader["mas_opr"]?.ToString();
                      string emp_code = reader["emp_code"]?.ToString();

                      decimal prd_setup = reader["prd_setup"] != DBNull.Value ? Convert.ToDecimal(reader["prd_setup"]) : 0.0m;
                      decimal prd_tools = reader["prd_tools"] != DBNull.Value ? Convert.ToDecimal(reader["prd_tools"]) : 0.0m;
                      decimal prd_surf = reader["prd_surf"] != DBNull.Value ? Convert.ToDecimal(reader["prd_surf"]) : 0.0m;
                      int prd_dimout = reader["prd_dimout"] != DBNull.Value ? Convert.ToInt32(reader["prd_dimout"]) : 0;
                      int prd_other = reader["prd_other"] != DBNull.Value ? Convert.ToInt32(reader["prd_other"]) : 0;
                      string scrap_remark = reader["scrap_remark"]?.ToString();

                      decimal ven_hardness = reader["ven_hardness"] != DBNull.Value ? Convert.ToDecimal(reader["ven_hardness"]) : 0.0m;
                      decimal ven_dimout = reader["ven_dimout"] != DBNull.Value ? Convert.ToDecimal(reader["ven_dimout"]) : 0.0m;
                      decimal ven_surf = reader["ven_surf"] != DBNull.Value ? Convert.ToDecimal(reader["ven_surf"]) : 0.0m;
                      int ven_other = reader["ven_other"] != DBNull.Value ? Convert.ToInt32(reader["ven_other"]) : 0;
                      string vendor_remark = reader["vendor_remark"]?.ToString();
                      string other_remark = reader["other_remark"]?.ToString();

                      string txt_app1 = reader["txt_app1"]?.ToString();
                      string txt_app2 = reader["txt_app2"]?.ToString();
                      string txt_app3 = reader["txt_app3"]?.ToString();

                      results.Add(new
                      {
                          rec_date = rec_date, 
                          mas_wo = mas_wo, 
                          mas_itemno = mas_itemno, 
                          mas_itemname = mas_itemname, 
                          mas_opr = mas_opr, 
                          emp_code = emp_code, 
                          prd_setup = prd_setup, 
                          prd_tools = prd_tools, 
                          prd_surf = prd_surf, 
                          prd_dimout = prd_dimout, 
                          prd_other = prd_other, 
                          scrap_remark = scrap_remark, 
                          ven_hardness = ven_hardness, 
                          ven_dimout = ven_dimout, 
                          ven_surf = ven_surf, 
                          ven_other = ven_other, 
                          vendor_remark = vendor_remark,
                          other_remark = other_remark, 
                          txt_app1 = txt_app1, 
                          txt_app2 = txt_app2, 
                          txt_app3 = txt_app3

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
