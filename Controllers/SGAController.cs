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

public class SGAController : Controller
{
  public DataAccess daAccess = new DataAccess();
  //PRDPRDExcelImport, PRDRecordSheetList
  //public IActionResult PRDRecordSheet() => View();
  public async Task<IActionResult> PRDRecordSheet(IEnumerable<AddRecSheetDayModelVM> VMModel, string? search, string? scrid, string? user)
  {
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
      //-->Response.Redirect(AuthenUrl + "?url="+URLApproval+"?docNo="+docNo+"");
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

    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    string page20 = _configuration[key: "TBCorApiServices:Page20"];
    ViewData["URLAPPROVE"] = _configuration[key: "TBCorApiServices:URLScrapApproval"];
    AddRecSheetDayModelVM vm = new AddRecSheetDayModelVM();

    if (search != null)
    {
      string[] arrWo = search.Split(':');
      ViewData["WO-NO"] = arrWo[0];
      //--#1-> Get WO Detail With Operation
      List<RoutingResourceModel> res = new List<RoutingResourceModel>();
        try
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                SqlCommand sql_cmnd = new SqlCommand("SP_GetWOTimeSheetDet", conn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@ppWo", SqlDbType.NVarChar).Value = arrWo[0];
                sql_cmnd.Parameters.AddWithValue("@itempart", SqlDbType.NVarChar).Value = arrWo[1];
                conn.Open();
                using (SqlDataReader reader = sql_cmnd.ExecuteReader())
                {
                    string s_woitem_stat = null;
                    int opr_open = 0;
                    int opr_close = 0;
                    while (reader.Read())
                    {
                        string s_oprQty = reader["oprQty"] == DBNull.Value ? "0" : reader["oprQty"].ToString();
                        string s_procNext = reader["procNext"] == DBNull.Value ? "0" : reader["procNext"].ToString();
                        string s_rouResp = reader["rouResp"] == DBNull.Value ? "0" : reader["rouResp"].ToString();
                        string s_procQty = reader["procQty"] == DBNull.Value ? "0" : reader["procQty"].ToString();
                        string s_oprRuntime = reader["oprRuntime"] == DBNull.Value ? "0" : reader["oprRuntime"].ToString();
                        string s_mctime = reader["mctime"] == DBNull.Value ? "0" : reader["mctime"].ToString();
                        string s_setuptime = reader["setuptime"] == DBNull.Value ? "0" : reader["setuptime"].ToString();
                        if (s_woitem_stat == null)
                        {
                          s_woitem_stat = reader["woitem_stat"] == DBNull.Value ? "0" : reader["woitem_stat"].ToString();
                          ViewData["WOSTAT"] = s_woitem_stat;
                        }
                        
                        if (Convert.ToInt32(reader["opr_stat"]) == 1) opr_close++;
                        else opr_open++;
                        res.Add(new RoutingResourceModel()
                        {
                          ID = reader["ID"].ToString(),
                          itemParts = reader["itemParts"].ToString(),
                          itemName = reader["itemName"].ToString(),
                          oprQty = Convert.ToInt32(s_oprQty),
                          rouPriority = reader["rouPriority"] == DBNull.Value ? "" : reader["rouPriority"].ToString(),
                          oprCode = reader["oprCode"] == DBNull.Value ? "" : reader["oprCode"].ToString(),
                          oprCost = reader["oprCost"] == DBNull.Value ? "" : reader["oprCost"].ToString(),
                          oprRuntime = Convert.ToDecimal(s_oprRuntime),
                          mctime = Convert.ToDecimal(s_mctime),
                          procQty = Convert.ToDecimal(s_procQty),
                          procNext =  Convert.ToInt32(s_procNext),
                          rouGroup = reader["rouGroup"] == DBNull.Value ? "" : reader["rouGroup"].ToString(),
                          itemCode = reader["itemCode"] == DBNull.Value ? "" : reader["itemCode"].ToString(),
                          rouCode = reader["rouCode"] == DBNull.Value ? "" : reader["rouCode"].ToString(),
                          rouResp = Convert.ToInt32(s_rouResp),
                          create_dt = (DateTime)reader["create_dt"], 
                          setuptime = Convert.ToDecimal(s_setuptime),
                          woitem_stat = Convert.ToInt32(s_woitem_stat),
                          opr_stat = Convert.ToInt32(reader["opr_stat"])
                        });
                        if (ViewData["PLANITEMNO"] == null)
                        {
                          ViewData["PLANITEMNO"] = reader["planitemno"] == DBNull.Value ? "" : reader["planitemno"].ToString();
                          ViewData["PLANITEMNAME"] = reader["planitemname"] == DBNull.Value ? "" : reader["planitemname"].ToString();
                          ViewData["PLANQTY"] = reader["planqty"] == DBNull.Value ? "0" : reader["planqty"].ToString();
                        }
                    }
                    ViewData["OPROPEN"] = opr_open.ToString();
                    ViewData["OPRCLOSE"] = opr_close.ToString();
                    int doc_closed = opr_close - opr_open;
                    ViewData["DOCCLOSED"] = doc_closed.ToString();
                }
                conn.Close();
            }
            vm.VMRoutingResource = res.ToList();
        }
        catch (Exception ex)
        {
          
        }
        //----> Get All users from AD.
        List<HRUserModel> ressam = new List<HRUserModel>();
        try
        {
          using (SqlConnection conn = new SqlConnection(DBConn))
          {
            SqlCommand sql_cmnd = new SqlCommand("SP_CFGDropDown", conn);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@FUNC", SqlDbType.VarChar).Value = "APRV_STOCK_WEB";
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

    }    
    //---> Get User information
    if (HttpContext.Session.GetString(SessionModel.SAMNAME) != null)
    {
      ViewData["USR_ADMINDOC"] = daAccess.GetUserPermissFunc(HttpContext.Session.GetString(SessionModel.SAMNAME), "ADMINDOC").ToString();

      List<string> lstADUsr = daAccess.GetADUserDetail(HttpContext.Session.GetString(SessionModel.SAMNAME));
      ViewData["EMPCODE"] = lstADUsr[0];
      ViewData["DISPNAME"] = lstADUsr[1];
      ViewData["EUMAIL"] = lstADUsr[2];
    }
    ViewData["STRWO"] = search;
    ViewData["SCRAPID"] = scrid;
    return View(vm);
  }
  public async Task<IActionResult> PRDRecordSheet_Bak(IEnumerable<AddRecSheetModelVM> VMModel, string? search)
  {
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    string page20 = _configuration[key: "TBCorApiServices:Page20"];
    AddRecSheetModelVM vm = new AddRecSheetModelVM();

    List<UserStockModel> result = new List<UserStockModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetUsersListTools", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@search", SqlDbType.VarChar).Value = search == null ? "" : search.ToString();
        sql_cmnd.Parameters.Add("@rows", SqlDbType.Int);
        sql_cmnd.Parameters["@rows"].Direction = ParameterDirection.Output;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            result.Add(new UserStockModel()
            {
              ID = reader["ID"].ToString(),
              EMPCODE = reader["EMPCODE"] == DBNull.Value ? reader["USERLOGON"].ToString() : reader["EMPCODE"].ToString(),
              USERLOGON = reader["USERLOGON"].ToString(),
              USERFNAME = reader["USERFNAME"] == DBNull.Value ? reader["TMP_FNAME"].ToString() : reader["USERFNAME"].ToString(),
              USRFUNC = reader["USRFUNC"].ToString(),
              apprv_stock = reader["apprv_stock"].ToString()
            });
          }
        }
        ViewData["COUNTROW"] = System.String.Format("{0:N0}", sql_cmnd.Parameters["@rows"].Value) + " Rows";
        conn.Close();
      }
      vm.VMCosrUses = result.ToList();
    }
    catch (Exception ex)
    {
    }
    //----> Get WO
    List<WORoutingModel> ressam = new List<WORoutingModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetWOTimeSheet", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@wo", SqlDbType.VarChar).Value = "";
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            ressam.Add(new WORoutingModel()
            {
              ppWo = reader["ppWo"].ToString(),
              itemParts = reader["itemParts"] == DBNull.Value ? "" : reader["itemParts"].ToString(),
              itemName = reader["itemName"] == DBNull.Value ? "" : reader["itemName"].ToString(),
              oprQtyTxt = reader["oprQtyTxt"].ToString()
            });
          }
        }
        conn.Close();
      }
      vm.VMWORouting = ressam.ToList();
      }
      catch (Exception ex)
      {
      }

    //---> Get User information
    if (HttpContext.Session.GetString(SessionModel.SAMNAME) != null)
    {
      List<string> lstADUsr = daAccess.GetADUserDetail(HttpContext.Session.GetString(SessionModel.SAMNAME));
      ViewData["EMPCODE"] = lstADUsr[0];
      ViewData["DISPNAME"] = lstADUsr[1];
      ViewData["EUMAIL"] = lstADUsr[2];
    }
    ViewData["STRWO"] = search;
    return View(vm);
  }
  public async Task<IActionResult> PRDExcelImport()
  {
    if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    {
      List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
      ViewData["OPENDOC"] = lstStr[0];
      ViewData["CLOSEDOC"] = lstStr[1];
    }

    return View();
  }
  
  [HttpPost("PRDExcelImport")]
  public async Task<IActionResult> PRDExcelImport(IFormFile file, string? dataPln, string? dataRout)
  {
    string sheetName = null;
    string fType = null;
    if (dataPln == null && dataRout == null) {  return View(); }
    if (dataPln != null)
    {
      sheetName = dataPln;
      fType = "PLAN";
    }
    else if (dataRout != null)
    {
      sheetName = dataRout;
      fType = "ROUT";
    }
    else sheetName = null;

    if (file == null && sheetName == null) return View();
      string txtSheetname = file.FileName;
      int resInsRow = 0;
      string[] strMsg = new string[3];
      IConfiguration _configuration = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json")
                          .Build();

      string _FileName = "";
      string ExcelRootPath = _configuration[key: "TBCorApiServices:ExcelRootPath"];
      try
      {
          _FileName = Path.GetFileName(file.FileName);
          //string _path = Path.Combine("~/UploadedFiles", _FileName);
          string _path = Path.Combine(ExcelRootPath, DateTime.Now.ToString("ddMMyyHHmmss") + _FileName);
          if (file.Length > 0)
          {
              //<---- Start to Copy 1073741824
              using FileStream fs = new(_path, FileMode.Create);
              await file.OpenReadStream().CopyToAsync(fs, 1048576);
              file.OpenReadStream().Close();
              file.OpenReadStream().Flush();
          }
          
          if (_path != null && fType == "PLAN"){    
              resInsRow = InsertPlanSawingFeed(_path, sheetName);
          }
          else if (_path != null && fType == "ROUT")
          {
              resInsRow = InsertPlanRoutResouce(_path, sheetName);
          }
          ViewBag.Message = "Success " + resInsRow.ToString() + " Rows";
      }
      catch
      {
          ViewBag.Message = "Error";
      }
    //string txtUrl = HttpContext.Session.GetString(GlobUSERSession.EXELPath);
    //HttpContext.Session.SetString(GlobUSERSession.EXELPath, "");
    //ViewData["IMPROW"] = HttpContext.Session.GetString(GlobUSERSession.EXCROW);

    if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    {
      List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
      ViewData["OPENDOC"] = lstStr[0];
      ViewData["CLOSEDOC"] = lstStr[1];
    }
    return View();
  }
  public int InsertPlanSawingFeed(string fExcelPath, string excelSheet)
  {
      if (fExcelPath == "") return 0;
      IConfiguration _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
      //Start to read <---
      List<string> sheetNames = new List<string>();
      DataTableCollection tables = ReadFromExcel(fExcelPath, ref sheetNames, excelSheet);
      int rows = 0;
      int iRtnRow = 0;
      
      if (tables != null)
      {
          //--itemdoc, itemno, operation, toollife, itemqty, UPDATE_DATE, itemname
          foreach (DataTable dt in tables)
          {
              rows = dt.Rows.Count;
              if (rows != 0)
              {
                  DataTable dtImp = new DataTable();
                  dtImp.Columns.Add("ppDate");
                  dtImp.Columns.Add("itemTask");
                  dtImp.Columns.Add("d1");
                  dtImp.Columns.Add("d2");
                  dtImp.Columns.Add("itemParts");
                  dtImp.Columns.Add("partsName");
                  dtImp.Columns.Add("ppWo");
                  dtImp.Columns.Add("qty");
                  dtImp.Columns.Add("ppHour");
                  dtImp.Columns.Add("ppDay");
                  dtImp.Columns.Add("delivDate");
                  dtImp.Columns.Add("ppProcess");
                  dtImp.Columns.Add("ppRemark");          
                  int iRow = 0;
                  string DocName = "";
                  int Operator = 0;
                  string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray(); 
                  foreach (DataRow rowCol in dt.Rows)
                  {
                      iRow++;
                      if (rowCol[0] != null)
                      {
                          if (iRow >= 3 && rowCol[8] != null
                            && rowCol[5] != null 
                            ) //แถวที่ 3
                          {
                            // เช็ค Cell วันที่ก่อน
                            DateTime? dateValue = null;
                            if (rowCol[0] != null && rowCol[0] != DBNull.Value)
                            {
                                dateValue = (DateTime)rowCol[0];
                            }

                            DataRow dtrow = dtImp.NewRow();
                            dtrow["ppDate"] = (object?)dateValue ?? DBNull.Value;
                            dtrow["itemTask"] = rowCol[2] == null ? DBNull.Value : rowCol[2].ToString();
                            dtrow["d1"] = rowCol[3] == null ? DBNull.Value : rowCol[3].ToString();
                            dtrow["d2"] = rowCol[4] == null ? DBNull.Value : rowCol[4].ToString(); 
                            dtrow["itemParts"] = rowCol[5] == null ? DBNull.Value : rowCol[5].ToString(); 
                            dtrow["partsName"] = rowCol[6] == null ? DBNull.Value : rowCol[6].ToString(); 
                            dtrow["ppWo"] = rowCol[8] == null ? DBNull.Value : rowCol[8].ToString(); 
                            dtrow["qty"] = rowCol[9] == null ? DBNull.Value : rowCol[9].ToString(); 
                            dtrow["ppHour"] = rowCol[10] == null ? DBNull.Value : rowCol[10].ToString(); 
                            dtrow["ppDay"] = rowCol[11] == null ? DBNull.Value : rowCol[11].ToString();
                            dtrow["delivDate"] = rowCol[12] == null ? DBNull.Value : rowCol[12].ToString();
                            dtrow["ppProcess"] = rowCol[15] == null ? DBNull.Value : rowCol[15].ToString();
                            dtrow["ppRemark"] = rowCol[17] == null ? DBNull.Value : rowCol[17].ToString();
                            dtImp.Rows.Add(dtrow);
                          }
                      }
                  }

                  //--Insert into Data Collection โดยจะมองเห็นเป็น 1 Table ต่อ 1 Sheet
                  
                  try
                  {
                      string SqlconString = _configuration[key: "ConnectionStrings:BtCostReduct"];
                      SqlConnection sqlCon = null;
                      using (sqlCon = new SqlConnection(SqlconString))
                      {
                          sqlCon.Open();

                          SqlCommand sql_cmnd = new SqlCommand("SP_MigExcelWorkOrderParts", sqlCon);
                          sql_cmnd.CommandType = CommandType.StoredProcedure;
                          sql_cmnd.Parameters.Add("@TempTable", SqlDbType.Structured).Value = dtImp;
                        
                          sql_cmnd.Parameters.Add("@IROW", SqlDbType.Int);
                          sql_cmnd.Parameters["@IROW"].Direction = ParameterDirection.Output;
                          sql_cmnd.Parameters.Add("@UPDROW", SqlDbType.Int);
                          sql_cmnd.Parameters["@UPDROW"].Direction = ParameterDirection.Output;
                          sql_cmnd.ExecuteNonQuery();
                          iRtnRow = int.Parse(sql_cmnd.Parameters["@IROW"].Value == null ? "0" : sql_cmnd.Parameters["@IROW"].Value.ToString());
                          //iRtnRow = int.Parse(sql_cmnd.Parameters["@UPDROW"].Value == null ? "0" : sql_cmnd.Parameters["@UPDROW"].Value.ToString());
            
                          sqlCon.Close();
                      }
                  }
                  catch (Exception ex)
                  {
                  }
              }
          }
      return iRtnRow;
      }
      else
      {
      return iRtnRow;
      }
    }
  
  public int InsertPlanRoutResouce(string fExcelPath, string excelSheet)
  {
      if (fExcelPath == "") return 0;
      IConfiguration _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
      //Start to read <---
      List<string> sheetNames = new List<string>();
      DataTableCollection tables = ReadFromExcel(fExcelPath, ref sheetNames, excelSheet);
      int rows = 0;
      int iRtnRow = 0;
      
      if (tables != null)
      {
          //--itemdoc, itemno, operation, toollife, itemqty, UPDATE_DATE, itemname
          foreach (DataTable dt in tables)
          {
              rows = dt.Rows.Count;
              if (rows != 0)
              {
                  DataTable dtImp = new DataTable();
                  
                  dtImp.Columns.Add("itemParts"); 
                  dtImp.Columns.Add("itemName"); 
                  dtImp.Columns.Add("oprQty");
                  dtImp.Columns.Add("rouPriority");
                  dtImp.Columns.Add("oprCode");
                  dtImp.Columns.Add("oprCost");
                  dtImp.Columns.Add("oprRuntime"); 
                  dtImp.Columns.Add("procQty");
                  dtImp.Columns.Add("procNext"); 
                  dtImp.Columns.Add("rouGroup"); 
                  dtImp.Columns.Add("itemCode"); 
                  dtImp.Columns.Add("rouCode");
                  dtImp.Columns.Add("rouResp");
                  dtImp.Columns.Add("setuptime");

                  int iRow = 0;
                  string DocName = "";
                  int Operator = 0;
                  string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray(); 
                  foreach (DataRow rowCol in dt.Rows)
                  {
                      iRow++;
                      if (rowCol[0] != null)
                      {
                          if (iRow >= 1 && rowCol[0] != null
                            && rowCol[2] != null && rowCol[5] != null 
                            ) //แถวที่ 2
                          {
                            //string strProcNext = rowCol[8] == null ? "0" : rowCol[8].ToString().Replace("%27", "");
                            //strProcNext = strProcNext.Replace("%27", "");
                            //strProcNext = strProcNext.Replace(",", "");

                            string input = rowCol[8] == null ? "0" : rowCol[8].ToString();
                            // กำหนด Culture ที่รองรับเครื่องหมาย ' เป็นตัวคั่นหลักพัน
                            CultureInfo culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                            culture.NumberFormat.NumberGroupSeparator = "'";
                            culture.NumberFormat.NumberDecimalSeparator = ".";
                            int number = int.Parse(input, NumberStyles.Number, culture);

                            DataRow dtrow = dtImp.NewRow();
                            dtrow["itemParts"] = rowCol[0] == null ? DBNull.Value : rowCol[0].ToString();
                            dtrow["itemName"] = rowCol[1] == null ? DBNull.Value : rowCol[1].ToString();
                            dtrow["oprQty"] = rowCol[2] == null ? DBNull.Value : rowCol[2].ToString();
                            dtrow["rouPriority"] = rowCol[3] == null ? DBNull.Value : rowCol[3].ToString();
                            dtrow["oprCode"] = rowCol[4] == null ? DBNull.Value : rowCol[4].ToString();
                            dtrow["oprCost"] = rowCol[5] == null ? DBNull.Value : rowCol[5].ToString();
                            dtrow["oprRuntime"] = rowCol[6] == null ? DBNull.Value : rowCol[6].ToString();

                            dtrow["procQty"] = input == null ? DBNull.Value : number.ToString();
                            dtrow["procNext"] = rowCol[9] == null ? DBNull.Value : rowCol[9].ToString();
                            dtrow["rouGroup"] = rowCol[10] == null ? DBNull.Value : rowCol[10].ToString();
                            dtrow["itemCode"] = rowCol[11] == null ? DBNull.Value : rowCol[11].ToString();
                            dtrow["rouCode"] = rowCol[12] == null ? DBNull.Value : rowCol[12].ToString();
                            dtrow["rouResp"] = rowCol[13] == null ? DBNull.Value : rowCol[13].ToString();
                            dtrow["setuptime"] = rowCol[7] == null ? DBNull.Value : rowCol[7].ToString();
                            dtImp.Rows.Add(dtrow);
                          }
                      }
                  }

                  //--Insert into Data Collection โดยจะมองเห็นเป็น 1 Table ต่อ 1 Sheet
                  
                  try
                  {
                      string SqlconString = _configuration[key: "ConnectionStrings:BtCostReduct"];
                      SqlConnection sqlCon = null;
                      using (sqlCon = new SqlConnection(SqlconString))
                      {
                          sqlCon.Open();

                          SqlCommand sql_cmnd = new SqlCommand("SP_MigExcelRoutingResource", sqlCon);
                          sql_cmnd.CommandType = CommandType.StoredProcedure;
                          sql_cmnd.Parameters.Add("@TempTable", SqlDbType.Structured).Value = dtImp;
                        
                          sql_cmnd.Parameters.Add("@IROW", SqlDbType.Int);
                          sql_cmnd.Parameters["@IROW"].Direction = ParameterDirection.Output;
                          sql_cmnd.Parameters.Add("@UPDROW", SqlDbType.Int);
                          sql_cmnd.Parameters["@UPDROW"].Direction = ParameterDirection.Output;
                          sql_cmnd.ExecuteNonQuery();
                          iRtnRow = int.Parse(sql_cmnd.Parameters["@IROW"].Value == null ? "0" : sql_cmnd.Parameters["@IROW"].Value.ToString());
                          //iRtnRow = int.Parse(sql_cmnd.Parameters["@UPDROW"].Value == null ? "0" : sql_cmnd.Parameters["@UPDROW"].Value.ToString());
            
                          sqlCon.Close();
                      }
                  }
                  catch (Exception ex)
                  {

                  }
                  
              }
          }
      return iRtnRow;
      }
      else
      {
      return iRtnRow;
      }
    }
    DataTableCollection ReadFromExcel(string filePath, ref List<string> sheetNames, string sheetName)
    {
        try
        {
            DataTableCollection tableCollection = null;

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                    });

                    tableCollection = result.Tables;

                    foreach (DataTable table in tableCollection)
                    {
                        if (table.TableName == sheetName)
                        {
                          sheetNames.Add(table.TableName);
                        }
                    }
                }
            }

            return tableCollection;
        }
        catch (Exception)
        {
            return null;
        }
    }

    [HttpGet]
    public async Task<List<PartsListModel>> GetToolItemList()
    {
    //id, itemdoc, itemno, operation, toollife, itemqty, UPDATE_DATE, itemname
        IConfiguration _configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();
        string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
        //List<string> ddlValues = new List<string>();
        List<PartsListModel> res = new List<PartsListModel>();
        try
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                SqlCommand sql_cmnd = new SqlCommand("SP_GetItemPartsList", conn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                //sql_cmnd.Parameters.AddWithValue("@SAMACC", SqlDbType.NVarChar).Value = "";
                conn.Open();
                using (SqlDataReader reader = sql_cmnd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new PartsListModel()
                        {
                          itemdoc = reader["itemdoc"].ToString(),
                          itemno = reader["itemno"].ToString()
                        });
                    }
                }
                conn.Close();
            }
            return res;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
  [HttpGet]
  public IActionResult PRDRecordSheetList()
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
  public IActionResult GetPRDRecordSheetList(string? search)
  {
      IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    string page20 = _configuration[key: "TBCorApiServices:Page20"];
    List<WoListModel> result = new List<WoListModel>();
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_PRDRecordSheetList", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@wo", SqlDbType.VarChar).Value = search == null ? "" : search.ToString();
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            result.Add(new WoListModel()
            {
              ppWo = reader["ppWo"].ToString(),
              itemParts = reader["itemParts"].ToString(),
              itemName = reader["itemName"] == DBNull.Value ? "" : reader["itemName"].ToString(),
              wo_stat = reader["wo_stat"] == DBNull.Value ? "" : reader["wo_stat"].ToString()
            });
          }
        }
        conn.Close();
      }
    }
    catch (Exception ex)
    {
    }

    if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    {
      List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
      ViewData["OPENDOC"] = lstStr[0];
      ViewData["CLOSEDOC"] = lstStr[1];
    }
    return Json(result.ToList());
  }
  [HttpGet]
  public async Task<List<RoutingResourceModel>> GetWOTimeSheetDet(WORoutingModel obj)
  {
      IConfiguration _configuration = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json")
                          .Build();
      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
      List<RoutingResourceModel> res = new List<RoutingResourceModel>();
      try
      {
          using (SqlConnection conn = new SqlConnection(DBConn))
          {
              SqlCommand sql_cmnd = new SqlCommand("SP_GetWOTimeSheetDet", conn);
              sql_cmnd.CommandType = CommandType.StoredProcedure;
              sql_cmnd.Parameters.AddWithValue("@itempart", SqlDbType.NVarChar).Value = obj.itemParts;
              conn.Open();
              using (SqlDataReader reader = sql_cmnd.ExecuteReader())
              {
          
    //ID	itemParts	itemName	oprQty	rouPriority	oprCode	oprCost	oprRuntime	procQty	procNext	rouGroup	itemCode	rouCode	rouResp	create_dt
                  while (reader.Read())
                  {
                      res.Add(new RoutingResourceModel()
                      {
                        ID = reader["ID"].ToString(),
                        itemParts = reader["itemParts"].ToString(),
                        itemName = reader["itemName"].ToString(),
                        oprQty = reader["oprQty"] == DBNull.Value ? 0 : (int)reader["oprQty"],
                        rouPriority = reader["rouPriority"] == DBNull.Value ? "" : reader["rouPriority"].ToString(),
                        oprCode = reader["oprCode"] == DBNull.Value ? "" : reader["oprCode"].ToString(),
                        oprCost = reader["oprCost"] == DBNull.Value ? "" : reader["oprCost"].ToString(),
                        oprRuntime = reader["oprRuntime"] == DBNull.Value ? 0 : (decimal)reader["oprRuntime"],
                        procQty = reader["procQty"] == DBNull.Value ? 0 : (decimal)reader["procQty"],
                        procNext =  reader["procNext"] == DBNull.Value ? 0 : (int)reader["procNext"],
                        rouGroup = reader["rouGroup"] == DBNull.Value ? "" : reader["rouGroup"].ToString(),
                        itemCode = reader["itemCode"] == DBNull.Value ? "" : reader["itemCode"].ToString(),
                        rouCode = reader["rouCode"] == DBNull.Value ? "" : reader["rouCode"].ToString(),
                        rouResp = reader["rouResp"] == DBNull.Value ? 0 : (int)reader["rouResp"],
                        create_dt = (DateTime)reader["create_dt"]
                      });
                  }
              }
              conn.Close();
          }
          return res;
      }
      catch (Exception ex)
      {
          return null;
      }
  }
  //--> Add TimeSheet Records Into SP_InsTimeSheetRecord
  [HttpPost]
  public JsonResult AddInsTimeSheetRecord(TimeSheetModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    SqlConnection con = null;
    try
    {
      decimal d_mas_mc = Convert.ToDecimal(obj.mas_mc);
      decimal d_mas_lab =  Convert.ToDecimal(obj.mas_lab);
      decimal d_mas_stdtime =  Convert.ToDecimal(obj.mas_stdtime);
      decimal d_rec_setup =  Convert.ToDecimal(obj.rec_setup);
      decimal d_rec_mc =  Convert.ToDecimal(obj.rec_mc);
      decimal d_rec_lab =  Convert.ToDecimal(obj.rec_lab);
      decimal d_rec_aqty =  Convert.ToDecimal(obj.rec_aqty);
      decimal d_rec_atotal =  Convert.ToDecimal(obj.rec_atotal);
      decimal d_rec_eff =   Convert.ToDecimal(obj.rec_eff);
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_InsTimeSheetRecord", con);
      cmnd.CommandType = CommandType.StoredProcedure;

      cmnd.Parameters.AddWithValue("@mas_wo", SqlDbType.VarChar).Value = obj.mas_wo;
      cmnd.Parameters.AddWithValue("@mas_itemno", SqlDbType.VarChar).Value = obj.mas_itemno;
      cmnd.Parameters.AddWithValue("@mas_opr", SqlDbType.Int).Value = obj.mas_opr == null ? 0 : Convert.ToInt32(obj.mas_opr);
      cmnd.Parameters.AddWithValue("@mas_qty", SqlDbType.Int).Value = obj.mas_qty == null ? 0 : Convert.ToInt32(obj.mas_qty);
      cmnd.Parameters.AddWithValue("@mas_stdtime", SqlDbType.Decimal).Value = obj.mas_stdtime == null ? 0 : LimitDecimal(d_mas_mc);
      cmnd.Parameters.AddWithValue("@mas_resource", SqlDbType.VarChar).Value = obj.mas_resource;

      cmnd.Parameters.AddWithValue("@mas_mc", SqlDbType.Decimal).Value = obj.mas_mc == null ? 0 : LimitDecimal(d_mas_mc);
      cmnd.Parameters.AddWithValue("@mas_lab", SqlDbType.Decimal).Value = obj.mas_lab == null ? 0 : LimitDecimal(d_mas_lab);
      cmnd.Parameters.AddWithValue("@emp_code", SqlDbType.VarChar).Value = obj.emp_code;
      DateTime dt = (DateTime)obj.rec_date;
      cmnd.Parameters.AddWithValue("@rec_date", SqlDbType.VarChar).Value = dt.ToString("yyyy-MM-dd");

      cmnd.Parameters.AddWithValue("@rec_setup", SqlDbType.Decimal).Value = obj.rec_setup == null ? 0 : LimitDecimal(d_rec_setup);
      cmnd.Parameters.AddWithValue("@rec_mc", SqlDbType.Decimal).Value = obj.rec_mc == null ? 0 : LimitDecimal(d_rec_mc);
      cmnd.Parameters.AddWithValue("@rec_lab", SqlDbType.Decimal).Value = obj.rec_lab == null ? 0 : LimitDecimal(d_rec_lab);
      cmnd.Parameters.AddWithValue("@rec_aqty", SqlDbType.Decimal).Value = obj.rec_aqty == null ? 0 : LimitDecimal(d_rec_aqty);
      cmnd.Parameters.AddWithValue("@rec_atotal", SqlDbType.Decimal).Value = obj.rec_atotal == null ? 0 : LimitDecimal(d_rec_atotal);
      cmnd.Parameters.AddWithValue("@rec_eff", SqlDbType.Decimal).Value = obj.rec_eff == null ? 0 : LimitDecimal(d_rec_eff);

      cmnd.ExecuteNonQuery();                
    }
    catch (Exception ex)
    {
        strMsg[0] = "2";
        strMsg[1] = "error";
    }
    finally
    {
        strMsg[0] = "1";
        strMsg[1] = "success";
            
    }
    con.Close();
    return Json(strMsg);
  }
  decimal LimitDecimal(decimal input)
  {
      if (input > 99999.99m) return 99999.99m;
      if (input < -99999.99m) return -99999.99m;
      return Math.Round(input, 3);
  }
  //---> Get Time Sheet List
  [HttpPost]
  public JsonResult GetTimeSheetRecs(TimeSheetModel obj)
  {
      IConfiguration _configuration = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json")
                          .Build();
      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
      List<TimeSheetModel> res = new List<TimeSheetModel>();
      try
      {
          using (SqlConnection conn = new SqlConnection(DBConn))
          {
              SqlCommand sql_cmnd = new SqlCommand("SP_GetTimeSheetRecs", conn);
              sql_cmnd.CommandType = CommandType.StoredProcedure;
              //sql_cmnd.Parameters.AddWithValue("@itempart", SqlDbType.NVarChar).Value = obj.itemParts;
              sql_cmnd.Parameters.AddWithValue("@mas_wo", obj.mas_wo);
              sql_cmnd.Parameters.AddWithValue("@mas_itemno", obj.mas_itemno);
              sql_cmnd.Parameters.AddWithValue("@mas_opr", obj.mas_opr);

              conn.Open();
              using (SqlDataReader reader = sql_cmnd.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      res.Add(new TimeSheetModel()
                      {
                        ID = reader["ID"].ToString(),
                        mas_wo = reader["mas_wo"].ToString(),
                        mas_itemno = reader["mas_itemno"].ToString(),
                        mas_opr = reader["mas_opr"] == DBNull.Value ? 0 : (int)reader["mas_opr"],
                        mas_qty = reader["mas_qty"] == DBNull.Value ? 0 : (int)reader["mas_qty"],
                        mas_stdtime = reader["mas_stdtime"] == DBNull.Value ? 0 : (decimal)reader["mas_stdtime"],
                        mas_resource = reader["mas_resource"] == DBNull.Value ? "" : reader["mas_resource"].ToString(),
                        mas_mc = reader["mas_mc"] == DBNull.Value ? 0 : (decimal)reader["mas_mc"],
                        mas_lab = reader["mas_lab"] == DBNull.Value ? 0 : (decimal)reader["mas_lab"],
                        emp_code = reader["emp_code"] == DBNull.Value ? "" : reader["emp_code"].ToString(),
                        rec_date_txt = reader["rec_date"] != DBNull.Value ? Convert.ToDateTime(reader["rec_date"]).ToString("dd/MM/yyyy") : "",
                        rec_setup = reader["rec_setup"] == DBNull.Value ? 0 : (decimal)reader["rec_setup"],
                        rec_mc = reader["rec_mc"] == DBNull.Value ? 0 : (decimal)reader["rec_mc"],
                        rec_lab = reader["rec_lab"] == DBNull.Value ? 0 : (decimal)reader["rec_lab"],
                        rec_aqty = reader["rec_aqty"] == DBNull.Value ? 0 : (decimal)reader["rec_aqty"],
                        rec_atotal = reader["rec_atotal"] == DBNull.Value ? 0 : (decimal)reader["rec_atotal"],
                        rec_eff = reader["rec_eff"] == DBNull.Value ? 0 : (decimal)reader["rec_eff"],
                        opr_stat = reader["opr_stat"] == DBNull.Value ? 0 : (int)reader["opr_stat"],
                        create_dt = (DateTime)reader["create_dt"],
                        update_dt = (DateTime)reader["update_dt"]
                      });
                  }
              }
              conn.Close();
          }
          return Json(res.ToList());
      }
      catch (Exception ex)
      {
          return null;
      }
      //return Json(result, JsonRequestBehavior.AllowGet);
  }
  //---> Get Time Sheet List
  [HttpPost]
  public JsonResult GetTimeSheetRecsScrap(ScrapModel obj)
  {
      IConfiguration _configuration = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json")
                          .Build();
      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
      List<ScrapModel> res = new List<ScrapModel>();
      try
      {
          using (SqlConnection conn = new SqlConnection(DBConn))
          {
              SqlCommand sql_cmnd = new SqlCommand("SP_GetScrapTimeSheetRecs", conn);
              sql_cmnd.CommandType = CommandType.StoredProcedure;
              //sql_cmnd.Parameters.AddWithValue("@itempart", SqlDbType.NVarChar).Value = obj.itemParts;
              sql_cmnd.Parameters.AddWithValue("@mas_wo", obj.mas_wo);
              sql_cmnd.Parameters.AddWithValue("@mas_itemno", obj.mas_itemno);
              sql_cmnd.Parameters.AddWithValue("@mas_opr", obj.mas_opr);

              conn.Open();
              using (SqlDataReader reader = sql_cmnd.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      res.Add(new ScrapModel()
                      {
                        ID = reader["ID"].ToString(),
                        mas_wo = reader["mas_wo"].ToString(),
                        mas_itemno = reader["mas_itemno"].ToString(),
                        mas_opr = reader["mas_opr"] == DBNull.Value ? 0 : (int)reader["mas_opr"],
                        emp_code = reader["emp_code"] == DBNull.Value ? "" : reader["emp_code"].ToString(),
                        rec_date = Convert.ToDateTime(reader["rec_date"]),
                        rec_date_txt = reader["rec_date"] != DBNull.Value ? Convert.ToDateTime(reader["rec_date"]).ToString("dd/MM/yyyy") : "",

                        prd_setup = reader["prd_setup"] == DBNull.Value ? 0 : (decimal)reader["prd_setup"],
                        prd_tools = reader["prd_tools"] == DBNull.Value ? 0 : (decimal)reader["prd_tools"],
                        prd_surf = reader["prd_surf"] == DBNull.Value ? 0 : (decimal)reader["prd_surf"],
                        prd_dimout = reader["prd_dimout"] == DBNull.Value ? 0 : (decimal)reader["prd_dimout"],
                        prd_other = reader["prd_other"] == DBNull.Value ? 0 : (decimal)reader["prd_other"],
                        ven_hardness = reader["ven_hardness"] == DBNull.Value ? 0 : (decimal)reader["ven_hardness"],
                        ven_dimout = reader["ven_dimout"] == DBNull.Value ? 0 : (decimal)reader["ven_dimout"],
                        ven_surf = reader["ven_surf"] == DBNull.Value ? 0 : (decimal)reader["ven_surf"],
                        ven_other = reader["ven_other"] == DBNull.Value ? 0 : (decimal)reader["ven_other"],
                        opr_stat = reader["opr_stat"] == DBNull.Value ? 0 : (int)reader["opr_stat"]
                        //prd_setup = (decimal)reader["prd_setup"],
                        //prd_tools = (decimal)reader["prd_tools"],
                        //prd_surf =  0 ,
                        //prd_dimout = 0,
                        //prd_other = 0,
                        //ven_hardness = 0,
                        //ven_dimout = 0,
                        //ven_surf = 0,
                        //ven_other = 0
                                                
                        //create_dt = (DateTime)reader["create_dt"],
                        //update_dt = (DateTime)reader["update_dt"]
                      });
                  }
              }
              conn.Close();
          }
          return Json(res.ToList());
      }
      catch (Exception ex)
      {
          return null;
      }
      //return Json(result, JsonRequestBehavior.AllowGet);
  }
  //---> Get Time Sheet By ID
  [HttpPost]
  public JsonResult GetTimeSheetRecsScrapID(ScrapModel obj)
  {
      IConfiguration _configuration = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json")
                          .Build();
      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
      List<ScrapModel> res = new List<ScrapModel>();
      try
      {
          using (SqlConnection conn = new SqlConnection(DBConn))
          {
              SqlCommand sql_cmnd = new SqlCommand("SP_GetScrapTimeSheetRecsID", conn);
              sql_cmnd.CommandType = CommandType.StoredProcedure;
              sql_cmnd.Parameters.AddWithValue("@ID", obj.ID);

              conn.Open();
              using (SqlDataReader reader = sql_cmnd.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      res.Add(new ScrapModel()
                      {
                        ID = reader["ID"].ToString(),
                        mas_wo = reader["mas_wo"].ToString(),
                        mas_itemno = reader["mas_itemno"].ToString(),
                        mas_opr = reader["mas_opr"] == DBNull.Value ? 0 : (int)reader["mas_opr"],
                        emp_code = reader["emp_code"] == DBNull.Value ? "" : reader["emp_code"].ToString(),
                        rec_date = Convert.ToDateTime(reader["rec_date"]),
                        rec_date_txt = reader["rec_date"] != DBNull.Value ? Convert.ToDateTime(reader["rec_date"]).ToString("dd/MM/yyyy") : "",

                        prd_setup = reader["prd_setup"] == DBNull.Value ? 0 : (decimal)reader["prd_setup"],
                        prd_tools = reader["prd_tools"] == DBNull.Value ? 0 : (decimal)reader["prd_tools"],
                        prd_surf = reader["prd_surf"] == DBNull.Value ? 0 : (decimal)reader["prd_surf"],
                        prd_dimout = reader["prd_dimout"] == DBNull.Value ? 0 : (decimal)reader["prd_dimout"],
                        prd_other = reader["prd_other"] == DBNull.Value ? 0 : (decimal)reader["prd_other"],
                        scrap_remark = reader["scrap_remark"] == DBNull.Value ? "" : reader["scrap_remark"].ToString(), 

                        ven_hardness = reader["ven_hardness"] == DBNull.Value ? 0 : (decimal)reader["ven_hardness"],
                        ven_dimout = reader["ven_dimout"] == DBNull.Value ? 0 : (decimal)reader["ven_dimout"],
                        ven_surf = reader["ven_surf"] == DBNull.Value ? 0 : (decimal)reader["ven_surf"],
                        ven_other = reader["ven_other"] == DBNull.Value ? 0 : (decimal)reader["ven_other"],

                        vendor_remark = reader["vendor_remark"] == DBNull.Value ? "" : reader["vendor_remark"].ToString(),
                        other_remark = reader["other_remark"] == DBNull.Value ? "" : reader["other_remark"].ToString(), 
                        scrap_stat = reader["scrap_stat"] == DBNull.Value ? 0 : (int)reader["scrap_stat"],

                        app1_stat = reader["app1_stat"] == DBNull.Value ? 0 : (int)reader["app1_stat"],
                        app2_stat = reader["app2_stat"] == DBNull.Value ? 0 : (int)reader["app2_stat"],
                        app3_stat = reader["app3_stat"] == DBNull.Value ? 0 : (int)reader["app3_stat"],
                        app1_mail = reader["app1_mail"] == DBNull.Value ? "" : reader["app1_mail"].ToString(),
                        app2_mail = reader["app2_mail"] == DBNull.Value ? "" : reader["app2_mail"].ToString(),
                        app3_mail = reader["app3_mail"] == DBNull.Value ? "" : reader["app3_mail"].ToString()
                        //create_dt = (DateTime)reader["create_dt"],
                        //update_dt = (DateTime)reader["update_dt"]
                      });
                  }
              }
              conn.Close();
          }
          return Json(res.ToList());
      }
      catch (Exception ex)
      {
          return null;
      }
  }
  //---> Get Time Sheet List
  [HttpPost]
  public JsonResult GetTimeSheetRecsSum(TimeSheetModel obj)
  {
      IConfiguration _configuration = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json")
                          .Build();
      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
      List<TimeSheetModel> res = new List<TimeSheetModel>();
      try
      {
          using (SqlConnection conn = new SqlConnection(DBConn))
          {
              SqlCommand sql_cmnd = new SqlCommand("SP_GetTimeSheetRecsSum", conn);
              sql_cmnd.CommandType = CommandType.StoredProcedure;
              //sql_cmnd.Parameters.AddWithValue("@itempart", SqlDbType.NVarChar).Value = obj.itemParts;
              sql_cmnd.Parameters.AddWithValue("@mas_wo", obj.mas_wo);
              sql_cmnd.Parameters.AddWithValue("@mas_itemno", obj.mas_itemno);
              sql_cmnd.Parameters.AddWithValue("@mas_opr", obj.mas_opr);

              conn.Open();
              using (SqlDataReader reader = sql_cmnd.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      res.Add(new TimeSheetModel()
                      {
                        
                        rec_date_txt = reader["rec_date"] != DBNull.Value ? Convert.ToDateTime(reader["rec_date"]).ToString("dd/MM/yyyy") : "",
                        rec_setup = reader["rec_setup"] == DBNull.Value ? 0 : (decimal)reader["rec_setup"],
                        rec_mc = reader["rec_mc"] == DBNull.Value ? 0 : (decimal)reader["rec_mc"],
                        rec_lab = reader["rec_lab"] == DBNull.Value ? 0 : (decimal)reader["rec_lab"],
                        rec_aqty = reader["rec_aqty"] == DBNull.Value ? 0 : (decimal)reader["rec_aqty"],
                        rec_atotal = reader["rec_atotal"] == DBNull.Value ? 0 : (decimal)reader["rec_atotal"],
                        ng_qty = reader["ng_qty"] == DBNull.Value ? 0 : (decimal)reader["ng_qty"],
                        ng_total = reader["ng_total"] == DBNull.Value ? 0 : (decimal)reader["ng_total"]
                        
                      });
                  }
              }
              conn.Close();
          }
          return Json(res.ToList());
      }
      catch (Exception ex)
      {
          return null;
      }
      //return Json(result, JsonRequestBehavior.AllowGet);
  }
  //---> Get Scrap List
  [HttpPost]
  public JsonResult GetScrapSheetRecsSum(TimeSheetModel obj)
  {
      IConfiguration _configuration = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json")
                          .Build();
      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
      List<ScrapReportModel> res = new List<ScrapReportModel>();
      try
      {
          using (SqlConnection conn = new SqlConnection(DBConn))
          {
              SqlCommand sql_cmnd = new SqlCommand("SP_GetScrapSheetRecsSum", conn);
              sql_cmnd.CommandType = CommandType.StoredProcedure;
              //sql_cmnd.Parameters.AddWithValue("@itempart", SqlDbType.NVarChar).Value = obj.itemParts;
              sql_cmnd.Parameters.AddWithValue("@mas_wo", obj.mas_wo);
              sql_cmnd.Parameters.AddWithValue("@mas_itemno", obj.mas_itemno);
              sql_cmnd.Parameters.AddWithValue("@mas_opr", obj.mas_opr);

              conn.Open();
              using (SqlDataReader reader = sql_cmnd.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      res.Add(new ScrapReportModel()
                      {
                        
                        rec_date_txt = reader["rec_date"] != DBNull.Value ? Convert.ToDateTime(reader["rec_date"]).ToString("dd/MM/yyyy") : "",
                        prod_count = reader["prod_count"] == DBNull.Value ? 0 : (int)reader["prod_count"],
                        vedd_count = reader["vedd_count"] == DBNull.Value ? 0 : (int)reader["vedd_count"],
                        approval = reader["approval"] == DBNull.Value ? 0 : (int)reader["approval"],
                        approval_txt = reader["approval_txt"] == DBNull.Value ? "" : reader["approval_txt"].ToString()
                      });
                  }
              }
              conn.Close();
          }
          return Json(res.ToList());
      }
      catch (Exception ex)
      {
          return null;
      }
      //return Json(result, JsonRequestBehavior.AllowGet);
  }
  [HttpPut]
  public JsonResult UPDTimeSheetRecord(TimeSheetModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    SqlConnection con = null;
    try
    {
      decimal d1, d2, d3, d4, d5, d6;
        con = new SqlConnection(DBConn);
        con.Open();
        SqlCommand cmnd = new SqlCommand("SP_UPDTimeSheetRecord", con);
        cmnd.CommandType = CommandType.StoredProcedure;
        cmnd.Parameters.AddWithValue("@ID", SqlDbType.VarChar).Value = obj.ID;
        cmnd.Parameters.AddWithValue("@rec_setup", SqlDbType.Decimal).Value = obj.rec_setup == null ? 0 : obj.rec_setup;
        cmnd.Parameters.AddWithValue("@rec_mc", SqlDbType.Decimal).Value = obj.rec_mc == null ? 0 : obj.rec_mc;
        cmnd.Parameters.AddWithValue("@rec_lab", SqlDbType.Decimal).Value = obj.rec_lab == null ? 0 : obj.rec_lab;
        cmnd.Parameters.AddWithValue("@rec_aqty", SqlDbType.Decimal).Value = obj.rec_aqty == null ? 0 : obj.rec_aqty;
        cmnd.Parameters.AddWithValue("@rec_atotal", SqlDbType.Decimal).Value = obj.rec_atotal == null ? 0 : obj.rec_atotal;
        cmnd.Parameters.AddWithValue("@rec_eff", SqlDbType.Decimal).Value = obj.rec_eff == null ? 0 : obj.rec_eff;

        cmnd.ExecuteNonQuery();              
    }
    catch (Exception ex)
    {
        strMsg[0] = "2";
        strMsg[1] = "error";
    }
    finally
    {
        strMsg[0] = "1";
        strMsg[1] = "success";
            
    }
    con.Close();
    return Json(strMsg);
  }
  [HttpPut]
  public JsonResult UPDTimeSheetOprStat(RoutingReqModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    SqlConnection con = null;
    try
    {
        con = new SqlConnection(DBConn);
        con.Open();
        SqlCommand cmnd = new SqlCommand("SP_UPDTimeSheetOprStat", con);
        cmnd.CommandType = CommandType.StoredProcedure;
        cmnd.Parameters.AddWithValue("@ID", SqlDbType.VarChar).Value = obj.ID;
        cmnd.Parameters.AddWithValue("@opr_stat", SqlDbType.Int).Value = obj.opr_stat == null ? 0 : Convert.ToInt32(obj.opr_stat);
        cmnd.Parameters.AddWithValue("@ppWo", SqlDbType.VarChar).Value = obj.ppWo;
        cmnd.Parameters.AddWithValue("@itemParts", SqlDbType.VarChar).Value = obj.itemParts;

        cmnd.ExecuteNonQuery();              
    }
    catch (Exception ex)
    {
        strMsg[0] = "2";
        strMsg[1] = "error";
    }
    finally
    {
        strMsg[0] = "1";
        strMsg[1] = "success";
            
    }
    con.Close();
    return Json(strMsg);
  }
  [HttpDelete]
  public JsonResult DELTimeSheetRecord(string? ID)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    SqlConnection con = null;
    try
    {
        con = new SqlConnection(DBConn);
        con.Open();
        SqlCommand cmnd = new SqlCommand("SP_DELTimeSheet", con);
        cmnd.CommandType = CommandType.StoredProcedure;
        cmnd.Parameters.AddWithValue("@ID", SqlDbType.VarChar).Value = ID.Trim();
        cmnd.ExecuteNonQuery();                
    }
    catch (Exception ex)
    {
        strMsg[0] = "2";
        strMsg[1] = "error";
    }
    finally
    {
        strMsg[0] = "1";
        strMsg[1] = "success";
            
    }
    con.Close();
    return Json(strMsg);
  }
  [HttpPost]
  public JsonResult FUNCGetSumAct(TimeSheetModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    //string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    try
    {
        string SqlconString = _configuration[key: "ConnectionStrings:BtCostReduct"];
        using (SqlConnection conn = new SqlConnection(SqlconString))
        {
            using (SqlCommand cmd = new SqlCommand("SELECT dbo.FUNC_GetSumAct(@mas_wo, @mas_itemno, @mas_opr)", conn))
            {
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@mas_wo", obj.mas_wo);
                cmd.Parameters.AddWithValue("@mas_itemno", obj.mas_itemno);
                cmd.Parameters.AddWithValue("@mas_opr", obj.mas_opr);

                conn.Open();
                object scalarResult = cmd.ExecuteScalar();

                if (scalarResult != null && scalarResult != DBNull.Value)
                {
                    string result = "0";
                    strMsg[0] = "1";
                    strMsg[1] = "success";
                    result = Convert.ToInt32(scalarResult).ToString();
                    strMsg[2] = result;
                }
                conn.Close();
            }
        }
        
    }
    catch (Exception ex)
    {
      strMsg[0] = "2";
      strMsg[1] = "error";
      strMsg[2] = "";
    }

    return Json(strMsg);
  }
  [HttpPost]
  public JsonResult AddScrapTimeSheetRecord(ScrapModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    SqlConnection con = null;
    try
    {
      con = new SqlConnection(DBConn);
      con.Open();
      SqlCommand cmnd = new SqlCommand("SP_InsScrapTimeSheet", con);
      cmnd.CommandType = CommandType.StoredProcedure;

	    cmnd.Parameters.AddWithValue("@mas_wo", SqlDbType.VarChar).Value = obj.mas_wo;
	    cmnd.Parameters.AddWithValue("@mas_itemno", SqlDbType.VarChar).Value = obj.mas_itemno;
	    cmnd.Parameters.AddWithValue("@mas_opr", SqlDbType.Decimal).Value = obj.mas_opr == null ? 0 : obj.mas_opr;
	    cmnd.Parameters.AddWithValue("@emp_code", SqlDbType.VarChar).Value = obj.emp_code;

	    DateTime dt = (DateTime)obj.rec_date;
      cmnd.Parameters.AddWithValue("@rec_date", SqlDbType.VarChar).Value = dt.ToString("yyyy-MM-dd");

	    cmnd.Parameters.AddWithValue("@prd_setup", SqlDbType.Decimal).Value = obj.prd_setup == null ? 0 : Convert.ToDouble(obj.prd_setup);
	    cmnd.Parameters.AddWithValue("@prd_tools", SqlDbType.Decimal).Value = obj.prd_tools == null ? 0 : Convert.ToDouble(obj.prd_tools);
	    cmnd.Parameters.AddWithValue("@prd_surf", SqlDbType.Decimal).Value = obj.prd_surf == null ? 0 : Convert.ToDouble(obj.prd_surf);
	    cmnd.Parameters.AddWithValue("@prd_dimout", SqlDbType.Decimal).Value = obj.prd_dimout == null ? 0 : Convert.ToDouble(obj.prd_dimout);
	    cmnd.Parameters.AddWithValue("@prd_other", SqlDbType.Decimal).Value = obj.prd_other == null ? 0 : Convert.ToDouble(obj.prd_other);
	    cmnd.Parameters.AddWithValue("@scrap_remark", SqlDbType.VarChar).Value = obj.scrap_remark == null ? "" : obj.scrap_remark;
	    cmnd.Parameters.AddWithValue("@ven_hardness", SqlDbType.Decimal).Value = obj.ven_hardness == null ? 0 : Convert.ToDouble(obj.ven_hardness);
	    cmnd.Parameters.AddWithValue("@ven_dimout", SqlDbType.Decimal).Value = obj.ven_dimout == null ? 0 : Convert.ToDouble(obj.ven_dimout);
	    cmnd.Parameters.AddWithValue("@ven_surf", SqlDbType.Decimal).Value = obj.ven_surf == null ? 0 : Convert.ToDouble(obj.ven_surf);
	    cmnd.Parameters.AddWithValue("@ven_other", SqlDbType.Decimal).Value = obj.ven_other == null ? 0 : Convert.ToDouble(obj.ven_other);
	    cmnd.Parameters.AddWithValue("@vendor_remark", SqlDbType.VarChar).Value = obj.vendor_remark == null ? "" : obj.vendor_remark;

      cmnd.Parameters.AddWithValue("@other_remark", SqlDbType.VarChar).Value = obj.other_remark == null ? "" : obj.other_remark;
	    cmnd.Parameters.AddWithValue("@app1_stat", SqlDbType.Decimal).Value = obj.app1_stat == null ? 0 : Convert.ToDouble(obj.app1_stat);
	    cmnd.Parameters.AddWithValue("@app2_stat", SqlDbType.Decimal).Value = obj.app2_stat == null ? 0 : Convert.ToDouble(obj.app2_stat);	
	    cmnd.Parameters.AddWithValue("@app3_stat", SqlDbType.Decimal).Value = obj.app3_stat == null ? 0 : Convert.ToDouble(obj.app3_stat);

	    cmnd.Parameters.AddWithValue("@app1_mail", SqlDbType.VarChar).Value = obj.app1_mail == null ? "" : obj.app1_mail;
	    cmnd.Parameters.AddWithValue("@app2_mail", SqlDbType.VarChar).Value = obj.app2_mail == null ? "" : obj.app2_mail;
	    cmnd.Parameters.AddWithValue("@app3_mail", SqlDbType.VarChar).Value = obj.app3_mail == null ? "" : obj.app3_mail;
      //@scrid
      cmnd.Parameters.Add("@scrid", SqlDbType.VarChar, 50);
      cmnd.Parameters["@scrid"].Direction = ParameterDirection.Output;

      cmnd.ExecuteNonQuery();
      strMsg[2] = cmnd.Parameters["@scrid"].Value.ToString();
    }
    catch (Exception ex)
    {
        strMsg[0] = "2";
        strMsg[1] = "error";
    }
    finally
    {
        strMsg[0] = "1";
        strMsg[1] = "success";
            
    }
    con.Close();
    return Json(strMsg);
  }

  [HttpPut]
  public JsonResult ConfirmScrapSaveEdit(ScrapModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    SqlConnection con = null;
    try
    {
        con = new SqlConnection(DBConn);
        con.Open();
        SqlCommand cmnd = new SqlCommand("SP_UpdateScrapTimeSheetRecsID", con);
        cmnd.CommandType = CommandType.StoredProcedure;
        cmnd.Parameters.AddWithValue("@ID", SqlDbType.VarChar).Value = obj.ID;

        DateTime dt = (DateTime)obj.rec_date;
        cmnd.Parameters.AddWithValue("@rec_date", SqlDbType.VarChar).Value = dt.ToString("yyyy-MM-dd");

/*
 ID: $("#ID_scr").html(),
    rec_date: $("#edi_rec_date").val(),
    prd_setup: $("#edi_prd_setup").val(),
    prd_tools: $("#edi_prd_tools").val(),
    prd_surf: $("#edi_prd_surf").val(),
    prd_dimout: $("#edi_prd_dimout").val(),
    prd_other: $("#edi_prd_other").val(),
    scrap_remark: $("#edi_scrap_remark").val(),

    ven_hardness: $("#edi_ven_hardness").val(),
    ven_dimout: $("#edi_ven_dimout").val(),
    ven_surf: $("#edi_ven_surf").val(),
    ven_other: $("#edi_ven_other").val(),
    Supplier_remark: $("#edi_Supplier_remark").val(),

    app1_mail: App1,
    app2_mail: App2,
    app3_mail: App3
 */

        cmnd.Parameters.AddWithValue("@prd_setup", SqlDbType.Decimal).Value = obj.prd_setup == null ? 0 : Convert.ToDouble(obj.prd_setup);
        cmnd.Parameters.AddWithValue("@prd_tools", SqlDbType.Decimal).Value = obj.prd_tools == null ? 0 : Convert.ToDouble(obj.prd_tools);
        cmnd.Parameters.AddWithValue("@prd_surf", SqlDbType.Decimal).Value = obj.prd_surf == null ? 0 : Convert.ToDouble(obj.prd_surf);
        cmnd.Parameters.AddWithValue("@prd_dimout", SqlDbType.Decimal).Value = obj.prd_dimout == null ? 0 : Convert.ToDouble(obj.prd_dimout);
        cmnd.Parameters.AddWithValue("@prd_other", SqlDbType.Decimal).Value = obj.prd_other == null ? 0 : Convert.ToDouble(obj.prd_other);
        cmnd.Parameters.AddWithValue("@scrap_remark", SqlDbType.VarChar).Value = obj.scrap_remark == null ? "" : obj.scrap_remark;

        cmnd.Parameters.AddWithValue("@ven_hardness", SqlDbType.Decimal).Value = obj.ven_hardness == null ? 0 : Convert.ToDouble(obj.ven_hardness);
        cmnd.Parameters.AddWithValue("@ven_dimout", SqlDbType.Decimal).Value = obj.ven_dimout == null ? 0 : Convert.ToDouble(obj.ven_dimout);
        cmnd.Parameters.AddWithValue("@ven_surf", SqlDbType.Decimal).Value = obj.ven_surf == null ? 0 : Convert.ToDouble(obj.ven_surf);
        cmnd.Parameters.AddWithValue("@ven_other", SqlDbType.Decimal).Value = obj.ven_other == null ? 0 : Convert.ToDouble(obj.ven_other);        
        cmnd.Parameters.AddWithValue("@vendor_remark", SqlDbType.VarChar).Value = obj.vendor_remark == null ? "" : obj.vendor_remark;

        cmnd.Parameters.AddWithValue("@other_remark", SqlDbType.VarChar).Value = obj.other_remark == null ? "" : obj.other_remark;
        cmnd.Parameters.AddWithValue("@app1_stat", SqlDbType.Decimal).Value = obj.app1_stat == null ? 0 : Convert.ToDouble(obj.app1_stat);
	      cmnd.Parameters.AddWithValue("@app2_stat", SqlDbType.Decimal).Value = obj.app2_stat == null ? 0 : Convert.ToDouble(obj.app2_stat);	
	      cmnd.Parameters.AddWithValue("@app3_stat", SqlDbType.Decimal).Value = obj.app3_stat == null ? 0 : Convert.ToDouble(obj.app3_stat);

        cmnd.Parameters.AddWithValue("@app1_mail", SqlDbType.VarChar).Value = obj.app1_mail == null ? "" : obj.app1_mail;
        cmnd.Parameters.AddWithValue("@app2_mail", SqlDbType.VarChar).Value = obj.app2_mail == null ? "" : obj.app2_mail;
        cmnd.Parameters.AddWithValue("@app3_mail", SqlDbType.VarChar).Value = obj.app3_mail == null ? "" : obj.app3_mail;
        
        cmnd.ExecuteNonQuery();              
    }
    catch (Exception ex)
    {
        strMsg[0] = "2";
        strMsg[1] = "error";
    }
    finally
    {
        strMsg[0] = "1";
        strMsg[1] = "success";
            
    }
    con.Close();
    return Json(strMsg);
  }
  [HttpDelete]
  public JsonResult DelScrapTimeSheetRecsID(string? ID)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    SqlConnection con = null;
    try
    {
        con = new SqlConnection(DBConn);
        con.Open();
        SqlCommand cmnd = new SqlCommand("SP_DelScrapTimeSheetRecsID", con);
        cmnd.CommandType = CommandType.StoredProcedure;
        cmnd.Parameters.AddWithValue("@ID", SqlDbType.VarChar).Value = ID.Trim();
        cmnd.ExecuteNonQuery();                
    }
    catch (Exception ex)
    {
        strMsg[0] = "2";
        strMsg[1] = "error";
    }
    finally
    {
        strMsg[0] = "1";
        strMsg[1] = "success";
            
    }
    con.Close();
    return Json(strMsg);
  }

  //[HttpGet]
  //  public ActionResult GetSumAct(string mas_wo, string mas_itemno, int mas_opr)
  //  {
  //      int result = 0;

  //      // คุณสามารถดึง connection string จาก Web.config
  //      string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

  //      using (SqlConnection conn = new SqlConnection(connectionString))
  //      {
  //          using (SqlCommand cmd = new SqlCommand("SELECT dbo.FUNC_GetSumAct(@mas_wo, @mas_itemno, @mas_opr)", conn))
  //          {
  //              cmd.CommandType = CommandType.Text;

  //              cmd.Parameters.AddWithValue("@mas_wo", mas_wo);
  //              cmd.Parameters.AddWithValue("@mas_itemno", mas_itemno);
  //              cmd.Parameters.AddWithValue("@mas_opr", mas_opr);

  //              conn.Open();
  //              object scalarResult = cmd.ExecuteScalar();

  //              if (scalarResult != null && scalarResult != DBNull.Value)
  //              {
  //                  result = Convert.ToInt32(scalarResult);
  //              }
  //          }
  //      }

  //      return Json(new { sumAct = result }, JsonRequestBehavior.AllowGet);
  //  }
  [HttpGet]
  public async Task<IActionResult> GetExportTimeSheetData(string mas_wo, string mas_item)
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
      //mas_wo, mas_itemno, mas_opr, mas_qty, mas_stdtime, mas_resource, mas_mc, mas_lab, emp_code, rec_date, rec_setup, rec_mc, rec_lab, rec_aqty, rec_atotal, rec_eff, prd_setup, prd_tools, prd_surf, prd_dimout, prd_other, scrap_remark, ven_hardness, ven_dimout, ven_surf, ven_other, vendor_remark, other_remark, create_dt
          await conn.OpenAsync();
          using (var command = new SqlCommand("SP_ExcelExpTimeSheet", conn))
          {
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("@mas_wo", mas_wo);
              command.Parameters.AddWithValue("@mas_itemno", mas_item);

              command.Parameters.Add("@itemname", SqlDbType.VarChar, 255);
              command.Parameters["@itemname"].Direction = ParameterDirection.Output;

              using (var reader = await command.ExecuteReaderAsync())
              {
                  while (await reader.ReadAsync())
                  {
                      results.Add(new
                      {
                          mas_wo = reader["mas_wo"]?.ToString(),
                          mas_itemno = reader["mas_itemno"]?.ToString(),
                          mas_opr = reader["mas_opr"]?.ToString(),
                          mas_qty = reader["mas_qty"] != DBNull.Value ? Convert.ToInt32(reader["mas_qty"]) : 0,

                          mas_stdtime = reader["mas_stdtime"] != DBNull.Value ? Convert.ToDecimal(reader["mas_stdtime"]) : 0.0m,
                          mas_resource = reader["mas_resource"]?.ToString(),
                          mas_mc = reader["mas_mc"] != DBNull.Value ? Convert.ToDecimal(reader["mas_mc"]) : 0.0m,
                          mas_lab = reader["mas_lab"] != DBNull.Value ? Convert.ToDecimal(reader["mas_lab"]) : 0.0m,
                          emp_code = reader["emp_code"]?.ToString(), 
                          rec_date = Convert.ToDateTime(reader["rec_date"]).ToString("dd-MM-yyyy"),
                          rec_setup = reader["mas_stdtime"] != DBNull.Value ? Convert.ToDecimal(reader["mas_stdtime"]) : 0.0m,
                          rec_mc = reader["rec_mc"] != DBNull.Value ? Convert.ToDecimal(reader["rec_mc"]) : 0.0m,
                          rec_lab = reader["rec_lab"] != DBNull.Value ? Convert.ToDecimal(reader["rec_lab"]) : 0.0m,
                          rec_aqty = reader["rec_aqty"] != DBNull.Value ? Convert.ToInt32(reader["rec_aqty"]) : 0,
                          rec_atotal = reader["rec_atotal"] != DBNull.Value ? Convert.ToInt32(reader["rec_atotal"]) : 0,
                          prd_setup = reader["prd_setup"] != DBNull.Value ? Convert.ToDecimal(reader["prd_setup"]) : 0.0m,
                          prd_tools = reader["prd_tools"] != DBNull.Value ? Convert.ToDecimal(reader["prd_tools"]) : 0.0m,
                          prd_surf = reader["prd_surf"] != DBNull.Value ? Convert.ToDecimal(reader["prd_surf"]) : 0.0m,
                          prd_dimout = reader["prd_dimout"] != DBNull.Value ? Convert.ToDecimal(reader["prd_dimout"]) : 0.0m,
                          prd_other = reader["prd_other"] != DBNull.Value ? Convert.ToDecimal(reader["prd_other"]) : 0.0m,
                          scrap_remark = reader["scrap_remark"]?.ToString(), 
                          ven_hardness = reader["ven_hardness"] != DBNull.Value ? Convert.ToDecimal(reader["ven_hardness"]) : 0.0m,
                          ven_dimout = reader["ven_dimout"] != DBNull.Value ? Convert.ToDecimal(reader["ven_dimout"]) : 0.0m,
                          ven_surf = reader["ven_surf"] != DBNull.Value ? Convert.ToDecimal(reader["ven_surf"]) : 0.0m,
                          ven_other = reader["ven_other"] != DBNull.Value ? Convert.ToDecimal(reader["ven_other"]) : 0.0m,
                          vendor_remark = reader["vendor_remark"]?.ToString(),
                          other_remark = reader["other_remark"]?.ToString(),

                          rec_eff = reader["rec_eff"] != DBNull.Value ? Convert.ToDecimal(reader["rec_eff"]) : 0.0m,
                          create_dt = Convert.ToDateTime(reader["create_dt"]).ToString("dd-MM-yyyy")


                      });
                  }
              }
              itemname = command.Parameters["@itemname"].Value == DBNull.Value ? "" : command.Parameters["@itemname"].Value.ToString();
          }
          conn.CloseAsync();
      }

      //return Ok(results);
      return Ok(new { item_name = itemname, data = results });
  }
  
  [HttpPut]
  public JsonResult UpdWorkOrderPartsStat(WoListModel obj)
  {
    string[] strMsg = new string[3];
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    SqlConnection con = null;
    try
    {
        con = new SqlConnection(DBConn);
        con.Open();
        SqlCommand cmnd = new SqlCommand("SP_UpdWorkOrderPartsStat", con);
        cmnd.CommandType = CommandType.StoredProcedure;
        
        cmnd.Parameters.AddWithValue("@ppWo", SqlDbType.VarChar).Value = obj.ppWo == null ? "" : obj.ppWo;
        cmnd.Parameters.AddWithValue("@itemParts", SqlDbType.VarChar).Value = obj.itemParts == null ? "" : obj.itemParts;
        cmnd.Parameters.AddWithValue("@wo_stat", SqlDbType.Int).Value = obj.wo_stat == null ? 0 : Convert.ToInt32(obj.wo_stat);
        cmnd.ExecuteNonQuery();              
    }
    catch (Exception ex)
    {
        strMsg[0] = "2";
        strMsg[1] = "error";
    }
    finally
    {
        strMsg[0] = "1";
        strMsg[1] = "success";
            
    }
    con.Close();
    return Json(strMsg);
  }

}
