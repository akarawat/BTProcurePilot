using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspnetCoreMvcFull.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using ExcelDataReader;
using Azure;

namespace AspnetCoreMvcFull.Controllers;

public class PPStockController : Controller
{
  public DataAccess daAccess = new DataAccess();

  //public async Task<IActionResult> UserStockTools(IEnumerable<ViewModelVM> VMModel, string? search)
  public async Task<IActionResult> OrderDetail(IEnumerable<ViewOrdDetailModelVM> VMorderlist, string? docNo, string? user)
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
      //-->Response.Redirect(AuthenUrl);
      Response.Redirect(AuthenUrl + "?url="+URLApproval+"?docNo="+docNo+"");
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
    List<OrderDetailModel> result = new List<OrderDetailModel>();
    try
    {
      string itemDoc = "";
      string tmpDocNo = "";
      string tmpEmpFname = "";
      string tmpEmpCode = "";
      string tmpEmpApp = "";
      string tmpOrdStat = "";
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetOderDetail", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@ordNo", SqlDbType.VarChar).Value = docNo == null ? "-" : docNo.ToString();
        sql_cmnd.Parameters.Add("@emp_samacc", SqlDbType.VarChar, 225);
        sql_cmnd.Parameters["@emp_samacc"].Direction = ParameterDirection.Output;
        sql_cmnd.Parameters.Add("@doc_stat_txt", SqlDbType.VarChar, 225);
        sql_cmnd.Parameters["@doc_stat_txt"].Direction = ParameterDirection.Output;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            if (itemDoc == "")
            {
              itemDoc = reader["ItemDoc"].ToString();
            }
            if (tmpDocNo == "")
            {
              tmpDocNo = reader["ordNo"].ToString();
            }
            if (tmpEmpCode == "")
            {
              tmpEmpCode = reader["emp_code"].ToString();
            }
            if (tmpEmpFname == "")
            {
              tmpEmpFname = reader["emp_code_fname"].ToString();
            }
            if (tmpEmpApp == "")
            {
              tmpEmpApp = reader["emp_app_fname"].ToString();
            }
            if (tmpOrdStat == "")
            {
              tmpOrdStat = reader["ord_stat"].ToString();
            }

            result.Add(new OrderDetailModel()
            {
              //ID = reader["id"].ToString(),
              //ordNo = reader["ordNo"].ToString(),
              //emp_code = reader["emp_code"].ToString(),

              ID = reader["ID"].ToString(),
              ItemDoc = reader["ItemDoc"].ToString(),
              Itemno = reader["Itemno"].ToString(),
              Itemname = reader["Itemname"].ToString(),
              Itemrem = (int)reader["Itemrem"],
              Itemqty = (int)reader["Itemqty"],
              Itemapp = (int)reader["Itemapp"],
              ordNo = reader["ordNo"].ToString(),
              emp_code = reader["emp_code"].ToString(),
              emp_app = reader["emp_app"].ToString(),
              emp_code_fname = reader["emp_code_fname"].ToString(),
              emp_app_fname = reader["emp_app_fname"].ToString(),
              ord_stat = (int)reader["ord_stat"],
              locateno = reader["locateno"].ToString()

            });
          }
          ViewData["DOCNO"] = tmpDocNo;
          ViewData["EMPFNAME"] = tmpEmpFname;
          ViewData["EMPCODE"] = tmpEmpCode;
          ViewData["EMPAPP"] = tmpEmpApp;
          ViewData["ORDSTAT"] = tmpOrdStat;
          ViewData["ITEMDOC"] = itemDoc;
        }
        ViewData["EMPAPPSAMM"] = sql_cmnd.Parameters["@emp_samacc"].Value;
        ViewData["ORDSTATTXT"] = sql_cmnd.Parameters["@doc_stat_txt"].Value;
        conn.Close();
      }
    }
    catch (Exception ex)
    {
    }
    vm.VMOrderDetail = result.ToList();
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

    GetMessage();
    if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    {
      List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
      ViewData["OPENDOC"] = lstStr[0];
      ViewData["CLOSEDOC"] = lstStr[1];
    }

    if (HttpContext.Session.GetString(SessionModel.SAMNAME) != null)
    {
      ViewData["USR_STOCK"] = daAccess.GetUserPermissFunc(HttpContext.Session.GetString(SessionModel.SAMNAME), "STOCK").ToString();
      List<string> lstADUsr = daAccess.GetADUserDetail(SessionModel.SAMNAME);
      ViewData["EMPCODE"] = lstADUsr[0];
      ViewData["DISPNAME"] = lstADUsr[1];
      ViewData["EUMAIL"] = lstADUsr[2];
    }

    return View(vm);
  }
  
  public async Task<IActionResult> OrderList(IEnumerable<OrderListModel> orderlist, string? search, string? user)
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
    List<OrderListModel> result = (List<OrderListModel>)orderlist;
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetOrderingTools", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@search", SqlDbType.VarChar).Value = search == null ? "" : search.ToString();
        sql_cmnd.Parameters.Add("@rows", SqlDbType.Int);
        sql_cmnd.Parameters["@rows"].Direction = ParameterDirection.Output;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            result.Add(new OrderListModel()
            {
              ID = reader["id"].ToString(),
              ordNo = reader["ordNo"].ToString(),
              emp_code = reader["emp_code"].ToString(),
              emp_rec = reader["emp_rec"].ToString(),
              emp_code_fname = reader["emp_code_fname"].ToString(),
              emp_rec_fname = reader["emp_rec_fname"].ToString(),
              ordDate = (DateTime)reader["ordDate"],
              ord_stat = (int)reader["ord_stat"]
            });
          }
        }
        ViewData["COUNTROW"] = String.Format("{0:N0}", sql_cmnd.Parameters["@rows"].Value) + " Rows";
        conn.Close();
      }
    }
    catch (Exception ex)
    {
    }
    orderlist = result.ToList();
    GetMessage();
    if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    {
      List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
      ViewData["OPENDOC"] = lstStr[0];
      ViewData["CLOSEDOC"] = lstStr[1];
    }

    return View(orderlist);
  }
  [HttpGet]
  public async Task<List<OrderListModel>> GetOrderItemList()
  {
  //id, itemdoc, itemno, operation, toollife, itemqty, UPDATE_DATE, itemname
      IConfiguration _configuration = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json")
                          .Build();
      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
      //List<string> ddlValues = new List<string>();
      List<OrderListModel> res = new List<OrderListModel>();
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
                      res.Add(new OrderListModel()
                      {
                        ID = reader["id"].ToString(),
                        ordNo = reader["ordNo"].ToString(),
                        emp_code = reader["emp_code"].ToString(),
                        emp_rec = reader["emp_rec"].ToString(),
                        emp_code_fname = reader["emp_code_fname"].ToString(),
                        emp_rec_fname = reader["emp_rec_fname"].ToString(),
                        ordDate = (DateTime)reader["ordDate"]
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
  public async Task<IActionResult> StockTools(IEnumerable<PartsListModel> usrmodel, string? search)
  {
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    string page20 = _configuration[key: "TBCorApiServices:Page20"];
    List<PartsListModel> result = (List<PartsListModel>)usrmodel;
    try
    {
      using (SqlConnection conn = new SqlConnection(DBConn))
      {
        SqlCommand sql_cmnd = new SqlCommand("SP_GetPartsListTools", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@limit", SqlDbType.Int).Value = int.Parse(page20);
        sql_cmnd.Parameters.AddWithValue("@search", SqlDbType.VarChar).Value = search == null ? "" : search.ToString();
        sql_cmnd.Parameters.Add("@rows", SqlDbType.Int);
        sql_cmnd.Parameters["@rows"].Direction = ParameterDirection.Output;
        conn.Open();
        using (SqlDataReader reader = sql_cmnd.ExecuteReader())
        {
          while (reader.Read())
          {
            result.Add(new PartsListModel()
            {
              id = reader["id"].ToString(),
              itemdoc = reader["itemdoc"].ToString(),
              itemno = reader["itemno"].ToString(),
              operation = reader["operation"].ToString(),
              toollife = reader["toollife"].ToString(),
              itemqty = reader["itemqty"].ToString(),
              UPDATE_DATE = (DateTime)reader["UPDATE_DATE"], 
              itemname = reader["itemname"].ToString()
            });
          }
        }
        ViewData["COUNTROW"] = String.Format("{0:N0}", sql_cmnd.Parameters["@rows"].Value) + " Rows";
        conn.Close();
      }
    }
    catch (Exception ex)
    {
    }
    usrmodel = result.ToList();
    if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    {
      List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
      ViewData["OPENDOC"] = lstStr[0];
      ViewData["CLOSEDOC"] = lstStr[1];
    }
    return View(usrmodel);
  }
  public async Task<IActionResult> UserStockTools(IEnumerable<ViewModelVM> VMModel, string? search)
  {
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    string page20 = _configuration[key: "TBCorApiServices:Page20"];
    ViewModelVM vm = new ViewModelVM();

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
        ViewData["COUNTROW"] = String.Format("{0:N0}", sql_cmnd.Parameters["@rows"].Value) + " Rows";
        conn.Close();
      }
      vm.VMCosrUses = result.ToList();
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
        SqlCommand sql_cmnd = new SqlCommand("SP_GeUserCostEmail", conn);
        sql_cmnd.CommandType = CommandType.StoredProcedure;
        sql_cmnd.Parameters.AddWithValue("@USRFUNC", SqlDbType.VarChar).Value = "EMP";
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

    if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    {
      List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
      ViewData["OPENDOC"] = lstStr[0];
      ViewData["CLOSEDOC"] = lstStr[1];
    }

    return View(vm);
  }
  public async Task<IActionResult> ExcelImport()
  {
    if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    {
      List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
      ViewData["OPENDOC"] = lstStr[0];
      ViewData["CLOSEDOC"] = lstStr[1];
    }

    return View();
  }
  
  [HttpPost("ExcelImport")]
  public async Task<IActionResult> ExcelImport(IFormFile file)
  {
    if (file == null) return View();
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
          string _path = Path.Combine(ExcelRootPath, DateTime.Now.ToString("ddMMyyHHmm") + _FileName);
          if (file.Length > 0)
          {
              //<---- Start to Copy 1073741824
              using FileStream fs = new(_path, FileMode.Create);
              await file.OpenReadStream().CopyToAsync(fs, 1048576);
              file.OpenReadStream().Close();
              file.OpenReadStream().Flush();
          }
          
          if (_path != null){
              resInsRow = InsertSerialWo(_path);
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
  public async Task<IActionResult> AttendanceUplfile()
  {
    return View();
  }
  
  [HttpPost("AttendanceUplfile")]
  public async Task<IActionResult> AttendanceUplfile(IFormFile file)
  {
    if (file == null) return View();
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
          string _path = Path.Combine(ExcelRootPath, DateTime.Now.ToString("ddMMyyHHmm") + _FileName);
          if (file.Length > 0)
          {
              //<---- Start to Copy 1073741824
              using FileStream fs = new(_path, FileMode.Create);
              await file.OpenReadStream().CopyToAsync(fs, 1048576);
              file.OpenReadStream().Close();
              file.OpenReadStream().Flush();
          }
          
          if (_path != null){
              resInsRow = InsertAttendance(_path);
          }
          ViewBag.Message = "Success " + resInsRow.ToString() + " Rows";
      }
      catch
      {
          ViewBag.Message = "Error";
      }
            
      return View();
  }
  public int InsertAttendance(string fTxtAttendPath)
  {
      if (fTxtAttendPath == "") return 0;
      IConfiguration _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
      //Start to read <---
      List<string> sheetNames = new List<string>();
      DataTable tables = ReadFromTxtFile(fTxtAttendPath, ref sheetNames);
      int rows = 0;
      int iRtnRow = 0;

      if (tables != null)
      {
          string sMessage = "Error upload";
          bool bSuccess = false;
          bool bWrongFile = false;
          DataTable dt = new DataTable();
          dt.Columns.Add("EMPCODE");
          dt.Columns.Add("DDMMYY");
          dt.Columns.Add("ATTTIME");

          foreach (DataRow dr in tables.Rows)
          {
            DataRow row = dt.NewRow();
            row["EMPCODE"] = dr[0] == "" ? "" : dr[0].ToString().Trim();
            row["DDMMYY"] = dr[1] == "" ? "" : dr[1].ToString().Trim();
            row["ATTTIME"] = dr[2] == "" ? "" : dr[2].ToString().Trim();
            dt.Rows.Add(row);
            bWrongFile = true;
          }
          if (bWrongFile)
          {
              string DBConn = _configuration[key: "ConnectionStrings:BtLeaveOnline"];
              try
              {
                  using (SqlConnection conn = new SqlConnection(DBConn))
                  {
                      SqlCommand sql_cmnd = new SqlCommand("SP_MigTimeAttendance", conn);
                      sql_cmnd.CommandType = CommandType.StoredProcedure;
                      conn.Open();
                      var sqlParam = new SqlParameter();
                      sqlParam.ParameterName = "@TempTable";
                      sqlParam.SqlDbType = SqlDbType.Structured;
                      sqlParam.Value = dt;
                      sql_cmnd.Parameters.Add(sqlParam);
                      sql_cmnd.ExecuteNonQuery();
                      conn.Close();
                  }
                  sMessage = "Success " + dt.Rows.Count.ToString() + " Rows";
                  bSuccess = true;
                  iRtnRow = dt.Rows.Count;
              }
              catch (Exception ex) { }
          }
          
          
      return iRtnRow;
      }
      else
      {
      return iRtnRow;
      }
    }
    DataTable ReadFromTxtFile(string filePath, ref List<string> sheetNames)
    {
          try
          {
             DataTable dtImp = new DataTable();
             dtImp.Columns.Add("empcode");
             dtImp.Columns.Add("ddmmyy");
             dtImp.Columns.Add("atttime");
             string[] lines = System.IO.File.ReadAllLines(filePath);
             foreach (string line in lines) {
                DataRow dtrow = dtImp.NewRow();
                string[] arrcol = line.Split('\t');
                string[] arrdmy = line.Split(' ');
                if (arrcol.Length == 5 && arrdmy.Length == 2) {
                  dtrow["empcode"] = arrcol[0].ToString();
                  dtrow["ddmmyy"] = arrcol[4].ToString();
                  dtrow["atttime"] = arrdmy[1].ToString();
                  dtImp.Rows.Add(dtrow);
                }
             }
              /*
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
                          sheetNames.Add(table.TableName);
                      }
                  }

              
                //Console.WriteLine(String.Join(Environment.NewLine, lines));
              }
              return tableCollection;
              */
              return dtImp;
          }
          catch (Exception)
          {
              return null;
          }
      }

  public int InsertSerialWo(string fExcelPath)
  {
      if (fExcelPath == "") return 0;
      IConfiguration _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
      //Start to read <---
      List<string> sheetNames = new List<string>();
      DataTableCollection tables = ReadFromExcel(fExcelPath, ref sheetNames);
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
                  dtImp.Columns.Add("itemdoc");
                  dtImp.Columns.Add("itemno");
                  dtImp.Columns.Add("operation");
                  dtImp.Columns.Add("toollife");
                  dtImp.Columns.Add("itemqty");
                  dtImp.Columns.Add("itemname");                  
                  int iRow = 0;
                  string DocName = "";
                  int Operator = 0;
                  string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray(); 
                  foreach (DataRow row in dt.Rows)
                  {
                      iRow++;
                      if (row[0] != null)
                      {
                          if (iRow == 1)
                          {
                            DocName = row[1].ToString();  
                          }
                          if (iRow == 2)
                          {
                            Operator = row[1].ToString() == "" ? 0 : int.Parse(row[1].ToString());  
                          }
                          if (iRow >= 5)
                          {
                            DataRow dtrow = dtImp.NewRow();
                            dtrow["itemno"] = row[0].ToString();
                            dtrow["toollife"] = row[2].ToString();
                            dtrow["itemqty"] = row[3].ToString();
                            dtrow["itemdoc"] = row[4].ToString();
                            dtrow["itemname"] = row[5].ToString();
                            dtrow["operation"] = row[6].ToString();
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

                          //SqlCommand sql_cmnd = new SqlCommand("SP_MigExcelItemno", sqlCon);
                          //sql_cmnd.CommandType = CommandType.StoredProcedure;

                          //var sqlParam = new SqlParameter();
                          //sqlParam.ParameterName = "@TempTable";
                          //sqlParam.SqlDbType = SqlDbType.Structured;
                          //sqlParam.Value = dtImp; // dt;
                          //sql_cmnd.Parameters.Add(sqlParam);

                          //sql_cmnd.Parameters.Add("@IROW", SqlDbType.Int);
                          //sql_cmnd.Parameters["@IROW"].Direction = ParameterDirection.Output;

                          //sql_cmnd.ExecuteNonQuery();


                          SqlCommand sql_cmnd = new SqlCommand("SP_MigExcelItemno", sqlCon);
                          sql_cmnd.CommandType = CommandType.StoredProcedure;
                          sql_cmnd.Parameters.Add("@itemdoc", SqlDbType.VarChar, 25).Value = columnNames[1];
                          sql_cmnd.Parameters.Add("@itemname", SqlDbType.VarChar, 150).Value = DocName;
                          sql_cmnd.Parameters.Add("@operation", SqlDbType.Int).Value = Operator;
                          sql_cmnd.Parameters.Add("@TempTable", SqlDbType.Structured).Value = dtImp;
                        
                          sql_cmnd.Parameters.Add("@IROW", SqlDbType.Int);
                          sql_cmnd.Parameters["@IROW"].Direction = ParameterDirection.Output;
                          sql_cmnd.ExecuteNonQuery();
                          iRtnRow += int.Parse(sql_cmnd.Parameters["@IROW"].Value == null ? "0" : sql_cmnd.Parameters["@IROW"].Value.ToString());
            
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
    DataTableCollection ReadFromExcel(string filePath, ref List<string> sheetNames)
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
                        sheetNames.Add(table.TableName);
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
      public IActionResult PartList()
      {
        GetMessage();
        if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
        {
          List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
          ViewData["OPENDOC"] = lstStr[0];
          ViewData["CLOSEDOC"] = lstStr[1];
        }
        return View();
      }
      [HttpGet] //-- Get For edit
      public IActionResult GetPartList(string? search)
      {
          IConfiguration _configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();

        string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
        string page20 = _configuration[key: "TBCorApiServices:Page20"];
        List<PartsListNBatchNoModel> result = new List<PartsListNBatchNoModel>();
        try
        {
          using (SqlConnection conn = new SqlConnection(DBConn))
          {
            SqlCommand sql_cmnd = new SqlCommand("SP_GetPartsListToolsNBatchno", conn);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@limit", SqlDbType.Int).Value = int.Parse(page20);
            sql_cmnd.Parameters.AddWithValue("@search", SqlDbType.VarChar).Value = search == null ? "" : search.ToString();
            sql_cmnd.Parameters.Add("@rows", SqlDbType.Int);
            sql_cmnd.Parameters["@rows"].Direction = ParameterDirection.Output;
            conn.Open();
            using (SqlDataReader reader = sql_cmnd.ExecuteReader())
            {
              while (reader.Read())
              {
                result.Add(new PartsListNBatchNoModel()
                {
                  id = reader["id"].ToString(),
                  itemdoc = reader["itemdoc"].ToString(),
                  itemno = reader["itemno"].ToString(),
                  operation = reader["operation"].ToString(),
                  toollife = reader["toollife"].ToString(),
                  itemqty = reader["itemqty"].ToString(),
                  UPDATE_DATE = (DateTime)reader["UPDATE_DATE"], 
                  itemname = reader["itemname"].ToString(), 
                  locateno = reader["locateno"].ToString(),
                  toolsname = reader["toolsname"].ToString(),
                  batchno = reader["batchno"].ToString()
                });
              }
            }
            ViewData["COUNTROW"] = String.Format("{0:N0}", sql_cmnd.Parameters["@rows"].Value) + " Rows";
            conn.Close();
          }
        }
        catch (Exception ex)
        {
        }
        GetMessage();
        if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
        {
          List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
          ViewData["OPENDOC"] = lstStr[0];
          ViewData["CLOSEDOC"] = lstStr[1];
        }
        return Json(result.ToList());
      }
      
      [HttpGet]
      public IActionResult GetPartsManualEdit(string? id, string? batchno)
      {
          IConfiguration _configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();

        string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
        string page20 = _configuration[key: "TBCorApiServices:Page20"];
        List<PartListForEditModel> result = new List<PartListForEditModel>();
        try
        {
          using (SqlConnection conn = new SqlConnection(DBConn))
          {
            SqlCommand sql_cmnd = new SqlCommand("SP_GetPartsManualEdit", conn);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@id", SqlDbType.VarChar).Value = id == null ? "" : id;
            sql_cmnd.Parameters.AddWithValue("@batchno", SqlDbType.VarChar).Value = batchno == null ? "" : batchno;
            conn.Open();
            using (SqlDataReader reader = sql_cmnd.ExecuteReader())
            {
              while (reader.Read())
              {
                result.Add(new PartListForEditModel()
                {
                  prodname = reader["prodname"] == DBNull.Value ? "" : reader["prodname"].ToString(),
                  searchname = reader["searchname"] == DBNull.Value ? "" : reader["searchname"].ToString(),
                  dimension1 = reader["dimension1"] == DBNull.Value ? "" : reader["dimension1"].ToString(),
                  dimension2 = reader["dimension2"] == DBNull.Value ? "" : reader["dimension2"].ToString(),
                  warehouse = reader["warehouse"] == DBNull.Value ? "" : reader["warehouse"].ToString(),
                  batchno = reader["batchno"] == DBNull.Value ? "" : reader["batchno"].ToString(),
                  onlocat = reader["onlocat"] == DBNull.Value ? "" : reader["onlocat"].ToString(),
                  fincostamont = reader["fincostamont"] == DBNull.Value ? "" : reader["fincostamont"].ToString(),
                  phyinventory = reader["phyinventory"] == DBNull.Value ? "" : reader["phyinventory"].ToString(),
                  phyreserv = reader["phyreserv"] == DBNull.Value ? "" : reader["phyreserv"].ToString(),
                  avaiphy = reader["avaiphy"] == DBNull.Value ? "" : reader["avaiphy"].ToString(),
                  ordertotal = reader["ordertotal"] == DBNull.Value ? "" : reader["ordertotal"].ToString(),
                  onorder = reader["onorder"] == DBNull.Value ? "" : reader["onorder"].ToString(),
                  orderreserv = reader["orderreserv"] == DBNull.Value ? "" : reader["orderreserv"].ToString(),
                  totalavailable = reader["totalavailable"] == DBNull.Value ? "" : reader["totalavailable"].ToString(),
                  itemdoc = reader["itemdoc"] == DBNull.Value ? "" : reader["itemdoc"].ToString(),
                  itemno = reader["itemno"] == DBNull.Value ? "" : reader["itemno"].ToString(),
                  operation = reader["operation"] == DBNull.Value ? 0 : (int)reader["operation"],
                  toollife = reader["toollife"] == DBNull.Value ? 0 : (int)reader["toollife"],
                  itemqty = reader["itemqty"] == DBNull.Value ? 0 : (int)reader["itemqty"],
                  docname = reader["docname"] == DBNull.Value ? "" : reader["docname"].ToString(),
                  locno = reader["locno"] == DBNull.Value ? "" : reader["locno"].ToString(),
                  itemlocate = reader["itemlocate"].ToString()
                });
              }
            }
            conn.Close();
          }
        }
        catch (Exception ex)
        {
        }
        return Json(result.ToList());
      }

      [HttpGet]
      public IActionResult SetOrderApproveOrCancel(string? docNo, int stat)
      {
          string[] strMsg = new string[3];
          IConfiguration _configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();

        string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
        
        try
        {
          using (SqlConnection conn = new SqlConnection(DBConn))
          {
            SqlCommand sql_cmnd = new SqlCommand("SP_SetOrderingStatus", conn);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@docNo", SqlDbType.VarChar).Value = docNo;
            sql_cmnd.Parameters.AddWithValue("@stat", SqlDbType.Int).Value = stat;
            //sql_cmnd.Parameters.Add("@rows", SqlDbType.Int);
            //sql_cmnd.Parameters["@rows"].Direction = ParameterDirection.Output;
            conn.Open();
            sql_cmnd.ExecuteReader();
            //ViewData["COUNTROW"] = String.Format("{0:N0}", sql_cmnd.Parameters["@rows"].Value) + " Rows";
            conn.Close();
            strMsg[0] = "1";
            strMsg[1] = "success";
          }
        }
        catch (Exception ex)
        {
            strMsg[0] = "2";
            strMsg[1] = "error";
        }
        return Json(strMsg);
      }

  public async void GetMessage()
  {
    string[] strMsg = new string[3];
          IConfiguration _configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    ViewData["URLPATH"] = _configuration[key: "TBCorApiServices:RootURL"];
    SqlConnection con = null;

    try
        {
            con = new SqlConnection(DBConn);
            con.Open();
            SqlCommand cmnd = new SqlCommand("SP_GetConfigMessage", con);
            cmnd.CommandType = CommandType.StoredProcedure;

            cmnd.Parameters.Add("@Approve", SqlDbType.VarChar, 150);
            cmnd.Parameters["@Approve"].Direction = ParameterDirection.Output;

            cmnd.Parameters.Add("@Pending", SqlDbType.VarChar, 150);
            cmnd.Parameters["@Pending"].Direction = ParameterDirection.Output;

            cmnd.Parameters.Add("@Closejob", SqlDbType.VarChar, 150);
            cmnd.Parameters["@Closejob"].Direction = ParameterDirection.Output;

            cmnd.Parameters.Add("@Canceljob", SqlDbType.VarChar, 150);
            cmnd.Parameters["@Canceljob"].Direction = ParameterDirection.Output;

            cmnd.Parameters.Add("@ordemail", SqlDbType.VarChar, 255);
            cmnd.Parameters["@ordemail"].Direction = ParameterDirection.Output;


            cmnd.ExecuteNonQuery();
            HttpContext.Session.SetString(SessionModel.MSGAPPROVE, cmnd.Parameters["@Approve"].Value.ToString());
            HttpContext.Session.SetString(SessionModel.MSGPEND, cmnd.Parameters["@Pending"].Value.ToString());
            HttpContext.Session.SetString(SessionModel.MSGCLOSE, cmnd.Parameters["@Closejob"].Value.ToString());
            HttpContext.Session.SetString(SessionModel.MSGCANCEL, cmnd.Parameters["@Canceljob"].Value.ToString());
            ViewData["USRSTOCKCONTRL"] = cmnd.Parameters["@ordemail"].Value.ToString().Trim();
                
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
            con.Close();
        }
  }
  public async void GetOpenOrCloseDoc()
  {
    string[] strMsg = new string[3];
          IConfiguration _configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
    ViewData["URLPATH"] = _configuration[key: "TBCorApiServices:RootURL"];
    SqlConnection con = null;

    //exec SP_CountOpenCloseOrder @ORD_OPEN=@ORD_OPEN output, @ORD_CLOSE=@ORD_CLOSE output;
    try
        {
            con = new SqlConnection(DBConn);
            con.Open();
            SqlCommand cmnd = new SqlCommand("SP_CountOpenCloseOrder", con);
            cmnd.CommandType = CommandType.StoredProcedure;

            cmnd.Parameters.Add("@ORD_OPEN", SqlDbType.Int);
            cmnd.Parameters["@ORD_OPEN"].Direction = ParameterDirection.Output;

            cmnd.Parameters.Add("@ORD_CLOSE", SqlDbType.Int);
            cmnd.Parameters["@ORD_CLOSE"].Direction = ParameterDirection.Output;

            cmnd.ExecuteNonQuery();
            HttpContext.Session.SetString(SessionModel.ORDOPEN, cmnd.Parameters["@ORD_OPEN"].Value.ToString());
            HttpContext.Session.SetString(SessionModel.ORDCLOSE, cmnd.Parameters["@ORD_CLOSE"].Value.ToString());
                
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
            con.Close();
        }
  }

  [HttpPost]
  public JsonResult UpdateQtyOrderDetail(OrderDetailModel obj)
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
        SqlCommand cmnd = new SqlCommand("SP_UpdateQtyOrderDetail", con);
        cmnd.CommandType = CommandType.StoredProcedure;
        cmnd.Parameters.AddWithValue("@ID", SqlDbType.VarChar).Value = obj.ID;
        cmnd.Parameters.AddWithValue("@Itemqty", SqlDbType.Int).Value = obj.Itemqty;
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
  public JsonResult AddnewUserStock(UserStockModel obj)
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
        SqlCommand cmnd = new SqlCommand("SP_InsNewUserCostReduct", con);
        cmnd.CommandType = CommandType.StoredProcedure;
        cmnd.Parameters.AddWithValue("@EMPCODE", SqlDbType.VarChar).Value = obj.EMPCODE == null ? "" : obj.EMPCODE;
        cmnd.Parameters.AddWithValue("@TMP_FNAME", SqlDbType.VarChar).Value = obj.TMP_FNAME == null ? "" : obj.TMP_FNAME;
        cmnd.Parameters.AddWithValue("@USRROLE", SqlDbType.Int).Value = obj.USRROLE;
        cmnd.Parameters.AddWithValue("@USRFUNC", SqlDbType.VarChar).Value = obj.USRFUNC == null ? "" : obj.USRFUNC;
        cmnd.Parameters.AddWithValue("@ORDMAIL", SqlDbType.VarChar).Value = obj.ORDMAIL == null ? "" : obj.ORDMAIL;
        cmnd.Parameters.AddWithValue("@apprv_stock", SqlDbType.VarChar).Value = obj.apprv_stock == null ? "" : obj.apprv_stock;

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
  public JsonResult EditUserStock(UserStockModel obj)
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
        SqlCommand cmnd = new SqlCommand("SP_EditUserCostReduct", con);
        cmnd.CommandType = CommandType.StoredProcedure;
        cmnd.Parameters.AddWithValue("@ID", SqlDbType.VarChar).Value = obj.ID == null ? "" : obj.ID;
        cmnd.Parameters.AddWithValue("@apprv_stock", SqlDbType.VarChar).Value = obj.apprv_stock == null ? "" : obj.apprv_stock;
        cmnd.Parameters.AddWithValue("@TMP_FNAME", SqlDbType.VarChar).Value = obj.TMP_FNAME == null ? "" : obj.TMP_FNAME;

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
  public JsonResult EditUserApprovOrder(OrderModel obj)
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
        SqlCommand cmnd = new SqlCommand("SP_EditUserAproveOrder", con);
        cmnd.CommandType = CommandType.StoredProcedure;
        cmnd.Parameters.AddWithValue("@ordNo", SqlDbType.VarChar).Value = obj.ordNo == null ? "" : obj.ordNo;
        cmnd.Parameters.AddWithValue("@apprv_stock", SqlDbType.VarChar).Value = obj.emp_app == null ? "" : obj.emp_app;

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
  public JsonResult DeleteStockUser(UserStockModel obj)
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
        SqlCommand cmnd = new SqlCommand("SP_DeleteStockUser", con);
        cmnd.CommandType = CommandType.StoredProcedure;
        cmnd.Parameters.AddWithValue("@ID", SqlDbType.VarChar).Value = obj.ID == null ? "" : obj.ID;
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
  public JsonResult AddnewPartsItem(AddPartsManualModel obj)
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
        SqlCommand cmnd = new SqlCommand("SP_AddPartsManual", con);
        cmnd.CommandType = CommandType.StoredProcedure;

        cmnd.Parameters.AddWithValue("@mas_itemno", SqlDbType.VarChar).Value = obj.mas_itemno; 
        cmnd.Parameters.AddWithValue("@mas_itemnoname", SqlDbType.VarChar).Value = obj.mas_itemnoname;
        cmnd.Parameters.AddWithValue("@box_locno", SqlDbType.VarChar).Value = obj.box_locno == null ? "" : obj.box_locno;
        cmnd.Parameters.AddWithValue("@box_itemlocate", SqlDbType.VarChar).Value = obj.box_itemlocate == null ? "" : obj.box_itemlocate;
        cmnd.Parameters.AddWithValue("@par_itemdoc", SqlDbType.VarChar).Value = obj.par_itemdoc == null ? "" : obj.par_itemdoc;
        cmnd.Parameters.AddWithValue("@par_itemname", SqlDbType.VarChar).Value = obj.par_itemname == null ? "" : obj.par_itemname;
        cmnd.Parameters.AddWithValue("@par_operation", SqlDbType.Int).Value = obj.par_operation == null ? 0 : obj.par_operation;
        cmnd.Parameters.AddWithValue("@par_toollife", SqlDbType.Int).Value = obj.par_toollife == null ? 0 : obj.par_toollife;
        cmnd.Parameters.AddWithValue("@par_itemqty", SqlDbType.Int).Value = obj.par_itemqty == null ? 0 : obj.par_itemqty;
        cmnd.Parameters.AddWithValue("@searchname", SqlDbType.VarChar).Value = obj.searchname == null ? "" : obj.searchname;
        cmnd.Parameters.AddWithValue("@dimension1", SqlDbType.VarChar).Value = obj.dimension1 == null ? "" : obj.dimension1;
        cmnd.Parameters.AddWithValue("@dimension2", SqlDbType.VarChar).Value = obj.dimension2 == null ? "" : obj.dimension2;
        cmnd.Parameters.AddWithValue("@warehouse", SqlDbType.VarChar).Value = obj.warehouse == null ? "" : obj.warehouse;
        cmnd.Parameters.AddWithValue("@batchno", SqlDbType.VarChar).Value = obj.batchno == null ? "" : obj.batchno;
        cmnd.Parameters.AddWithValue("@onlocat", SqlDbType.VarChar).Value = obj.onlocat == null ? "" : obj.onlocat;
        cmnd.Parameters.AddWithValue("@fincostamont", SqlDbType.VarChar).Value = obj.fincostamont == null ? "" : obj.fincostamont;
        cmnd.Parameters.AddWithValue("@phyinventory", SqlDbType.VarChar).Value = obj.phyinventory == null ? "" : obj.phyinventory;
        cmnd.Parameters.AddWithValue("@phyreserv", SqlDbType.VarChar).Value = obj.phyreserv == null ? "" : obj.phyreserv;
        cmnd.Parameters.AddWithValue("@avaiphy", SqlDbType.VarChar).Value = obj.avaiphy == null ? "" : obj.avaiphy;
        cmnd.Parameters.AddWithValue("@ordertotal", SqlDbType.VarChar).Value = obj.ordertotal == null ? "" : obj.ordertotal;
        cmnd.Parameters.AddWithValue("@onorder", SqlDbType.VarChar).Value = obj.onorder == null ? "" : obj.onorder;
        cmnd.Parameters.AddWithValue("@orderreserv", SqlDbType.VarChar).Value = obj.orderreserv == null ? "" : obj.orderreserv;
        cmnd.Parameters.AddWithValue("@totalavailable", SqlDbType.VarChar).Value = obj.totalavailable == null ? "" : obj.totalavailable;

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
  public JsonResult UpdatePartsListTools(PartListForEditModel obj)
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
        SqlCommand cmnd = new SqlCommand("SP_UpdatePartsListTools", con); 
        cmnd.CommandType = CommandType.StoredProcedure;
        //**--->?
        cmnd.Parameters.AddWithValue("@id", SqlDbType.VarChar).Value = obj.id; 
        cmnd.Parameters.AddWithValue("@itemno", SqlDbType.VarChar).Value = obj.itemno;
        cmnd.Parameters.AddWithValue("@locno", SqlDbType.VarChar).Value = obj.locno == null ? "" : obj.locno;
        cmnd.Parameters.AddWithValue("@itemlocate", SqlDbType.VarChar).Value = obj.itemlocate == null ? "" : obj.itemlocate;
        cmnd.Parameters.AddWithValue("@itemdoc", SqlDbType.VarChar).Value = obj.itemdoc == null ? "" : obj.itemdoc;
        cmnd.Parameters.AddWithValue("@itemname", SqlDbType.VarChar).Value = obj.docname == null ? "" : obj.docname;
        cmnd.Parameters.AddWithValue("@operation", SqlDbType.Int).Value = obj.operation == null ? 0 : obj.operation;
        cmnd.Parameters.AddWithValue("@toollife", SqlDbType.Int).Value = obj.toollife == null ? 0 : obj.toollife;

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
