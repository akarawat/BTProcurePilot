using AspnetCoreMvcFull.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;

namespace AspnetCoreMvcFull.Controllers;

public class DashboardsController : Controller
{
  private readonly ILogger<DashboardsController> _logger;
  private readonly IConfiguration _configuration;
  DataAccess daAccess = new DataAccess();
  public DashboardsController(ILogger<DashboardsController> logger, IConfiguration configuration)
  {
    _logger = logger;
    _configuration = configuration;
  }
  public IActionResult IndexBak() => View();
  public IActionResult Index(string user) {

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
      // ควรจะส่งค่ากลับมาตามนี้ "BERNINATHAILAND\\SAKULCHAI.P"
      string[] arrSamName = user.Split(new char[] { '\\' });
      if (arrSamName.Length == 2)
      {
        HttpContext.Session.SetString(SessionModel.SAMNAME, arrSamName[1]);
        sUSERLOGON = arrSamName[1];
      }
    }
    
    // Debug Mode Skip Authen
    /*
    string sUSERLOGON = "";
    user = "BERNINATHAILAND\\chamaiporn.k"; //manu.m, Suriyothin.M, yuttaphum.p, Kitiphong.T, yannick.t, saowanee.s, kanit.c, Prasit.Y, Sompop.S, user = "BERNINATHAILAND\\nipaporn.u";
    string[] arrSamName = user.Split(new char[] { '\\' });
    if (arrSamName.Length == 2)
    {
      HttpContext.Session.SetString(SessionModel.SAMNAME, arrSamName[1]);
      sUSERLOGON = arrSamName[1];
    }
    */
    //--> End of Authen 
    if (HttpContext.Session.GetString(SessionModel.EMPCODE) != null)
    {
      List<string> lstStr = daAccess.GetOpenOrCloseDoc(HttpContext.Session.GetString(SessionModel.EMPCODE).Trim());
      ViewData["OPENDOC"] = lstStr[0];
      ViewData["CLOSEDOC"] = lstStr[1];
    }
    

    if ((HttpContext.Session.GetString(SessionModel.SAMNAME) != null ||
      HttpContext.Session.GetString(SessionModel.SAMNAME) != ""))
    {
      List<string> lstUsrInfo = daAccess.GetUserLoginfo(HttpContext.Session.GetString(SessionModel.SAMNAME));
      string strEmail = lstUsrInfo[1];
      if (strEmail != "")
      {
        HttpContext.Session.SetString(SessionModel.USEREMAIL, strEmail);
      }
    }
    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    //-- Get User Permission
    HttpContext.Session.SetString(SessionModel.UROLEADMIN, "0");
    HttpContext.Session.SetString(SessionModel.UROLESUPP, "0");
    HttpContext.Session.SetString(SessionModel.UROLEMGR, "0");
    HttpContext.Session.SetString(SessionModel.UROLEMD, "0");
    SqlConnection conpm = null;
    if (sUSERLOGON == "")
    {
      sUSERLOGON = HttpContext.Session.GetString(SessionModel.SAMNAME);
    }
    int ADMIN_PERMISS = 0;
    int SUPP_PERMISS = 0;
    int MGR_PERMISS = 0;
    int MD_PERMISS = 0;

    string sEMPCODE = "";
    string sDEPCODE = "";
    string sUEMAIL = "";
    string sMAILGROUPADMIN = "";

    try
    {
      conpm = new SqlConnection(DBConn);
      conpm.Open();
      SqlCommand cmnd = new SqlCommand("SP_GetUserPermission", conpm);
      cmnd.CommandType = CommandType.StoredProcedure;
      cmnd.Parameters.AddWithValue("@USERLOGON", SqlDbType.VarChar).Value = sUSERLOGON == null ? "" : sUSERLOGON;
      
      cmnd.Parameters.Add("@UROLEADMIN", SqlDbType.Int); cmnd.Parameters["@UROLEADMIN"].Direction = ParameterDirection.Output;
      cmnd.Parameters.Add("@UROLESUPP", SqlDbType.Int); cmnd.Parameters["@UROLESUPP"].Direction = ParameterDirection.Output;
      cmnd.Parameters.Add("@UROLEMGR", SqlDbType.Int); cmnd.Parameters["@UROLEMGR"].Direction = ParameterDirection.Output;
      cmnd.Parameters.Add("@UROLEMD", SqlDbType.Int); cmnd.Parameters["@UROLEMD"].Direction = ParameterDirection.Output;

      cmnd.Parameters.Add("@EMPCODE", SqlDbType.VarChar, 15); cmnd.Parameters["@EMPCODE"].Direction = ParameterDirection.Output;
      cmnd.Parameters.Add("@DEPCODE", SqlDbType.VarChar, 10); cmnd.Parameters["@DEPCODE"].Direction = ParameterDirection.Output;
      cmnd.Parameters.Add("@UEMAIL", SqlDbType.VarChar, 255); cmnd.Parameters["@UEMAIL"].Direction = ParameterDirection.Output;
      cmnd.Parameters.Add("@MAILGROUPADMIN", SqlDbType.VarChar, 512); cmnd.Parameters["@MAILGROUPADMIN"].Direction = ParameterDirection.Output;
      
      cmnd.ExecuteNonQuery();
      ADMIN_PERMISS = Convert.ToInt32(cmnd.Parameters["@UROLEADMIN"].Value);
      SUPP_PERMISS = Convert.ToInt32(cmnd.Parameters["@UROLESUPP"].Value);
      MGR_PERMISS = Convert.ToInt32(cmnd.Parameters["@UROLEMGR"].Value);
      MD_PERMISS = Convert.ToInt32(cmnd.Parameters["@UROLEMD"].Value);

      sEMPCODE = cmnd.Parameters["@EMPCODE"].Value.ToString();
      sDEPCODE = cmnd.Parameters["@DEPCODE"].Value.ToString();
      sUEMAIL = cmnd.Parameters["@UEMAIL"].Value.ToString();
      sMAILGROUPADMIN = cmnd.Parameters["@MAILGROUPADMIN"].Value.ToString();
    }
    catch (Exception ex)
    {
      HttpContext.Session.SetString(SessionModel.UROLEADMIN, "0");
      HttpContext.Session.SetString(SessionModel.UROLESUPP, "0");
      HttpContext.Session.SetString(SessionModel.UROLEMGR, "0");
      HttpContext.Session.SetString(SessionModel.UROLEMD, "0");
      HttpContext.Session.SetString(SessionModel.EMPCODE, "");
      HttpContext.Session.SetString(SessionModel.DEPCODE, "");
      HttpContext.Session.SetString(SessionModel.UEMAIL, "");
      HttpContext.Session.SetString(SessionModel.MAILGROUPADMIN, "");
    }
    finally
    {
      HttpContext.Session.SetString(SessionModel.UROLEADMIN, ADMIN_PERMISS.ToString());
      HttpContext.Session.SetString(SessionModel.UROLESUPP, SUPP_PERMISS.ToString());
      HttpContext.Session.SetString(SessionModel.UROLEMGR, MGR_PERMISS.ToString());
      HttpContext.Session.SetString(SessionModel.UROLEMD, MD_PERMISS.ToString());
      HttpContext.Session.SetString(SessionModel.EMPCODE, sEMPCODE);
      HttpContext.Session.SetString(SessionModel.DEPCODE, sDEPCODE);
      HttpContext.Session.SetString(SessionModel.UEMAIL, sUEMAIL);
      HttpContext.Session.SetString(SessionModel.MAILGROUPADMIN, sMAILGROUPADMIN);
    }
    conpm.Close();
    //-- End Get User Permission
    string txtUser = HttpContext.Session.GetString(SessionModel.UROLEADMIN);
    return View();
  }

  public IActionResult AllMyPrApprove() => View();
  //public class DataAccess
  //{
  //    IConfiguration _configuration = new ConfigurationBuilder()
  //                            .SetBasePath(Directory.GetCurrentDirectory())
  //                            .AddJsonFile("appsettings.json")
  //                            .Build();

  //}
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
  [HttpGet]
  public async Task<IActionResult> GetLowStockItems_RadarChart()
  {
      //IConfiguration _configuration = new ConfigurationBuilder()
      //                .SetBasePath(Directory.GetCurrentDirectory())
      //                .AddJsonFile("appsettings.json")
      //                .Build();
      //string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];

      //List<LowStockItem> lowStock = new List<LowStockItem>();
      //using (var conn = new SqlConnection(DBConn))
      //{
      //    await conn.OpenAsync();
      //    using (var command = new SqlCommand("SP_GetLowStockItems_RadarChart", conn))
      //    {
      //        command.CommandType = CommandType.StoredProcedure;
      //        command.Parameters.Add("@txtDatUpdate", SqlDbType.VarChar);
      //        command.Parameters["@txtDatUpdate"].Direction = ParameterDirection.Output;
              
      //        using (var reader = await command.ExecuteReaderAsync())
      //        {
      //            while (await reader.ReadAsync())
      //            {
      //                lowStock.Add(new LowStockItem 
      //                {
      //                    Itemno = reader["Itemno"]?.ToString(),
      //                    TotalInventory = (Int32)reader["TotalInventory"],
      //                    TotalMinimum = (Int32)reader["TotalMinimum"],
      //                    DiffQty = (Int32)reader["DiffQty"]

      //                });
      //            }
      //            ViewData["LASTUPD"] = command.Parameters["@txtDatUpdate"].Value.ToString();
      //        }
      //    }
      //    conn.CloseAsync();
      //}

      //return Ok(lowStock);
      IConfiguration _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
    string DBConn = _configuration["ConnectionStrings:BtCostReduct"];

    List<LowStockItem> lowStock = new List<LowStockItem>();
    string txtDatUpdate = string.Empty;

    using (var conn = new SqlConnection(DBConn))
    {
        await conn.OpenAsync();

        using (var command = new SqlCommand("SP_GetLowStockItems_RadarChart", conn))
        {
            command.CommandType = CommandType.StoredProcedure;

            // Add output parameter
            var outputParam = new SqlParameter("@txtDatUpdate", SqlDbType.VarChar, 25)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputParam);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    lowStock.Add(new LowStockItem
                    {
                        Itemno = reader["itemno"]?.ToString(),
                        TotalInventory = Convert.ToInt32(reader["TotalInventory"]),
                        TotalMinimum = Convert.ToInt32(reader["TotalMinimum"]),
                        DiffQty = Convert.ToInt32(reader["DiffQty"])
                    });
                }
            }

            // Get output parameter value
            txtDatUpdate = outputParam.Value?.ToString();
        }

        await conn.CloseAsync();
    }

    // Return combined object
    return Ok(new
    {
        dateUpdated = txtDatUpdate,
        data = lowStock
    });
  }

  [HttpGet]
  public IActionResult GetMyPRReq(string? search)
  {
    IConfiguration _configuration = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json")
                      .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    var results = new List<dynamic>();
    string sEMPCODE = HttpContext.Session.GetString(SessionModel.EMPCODE);

    using (var conn = new SqlConnection(DBConn))
    {
      conn.Open();
      using (var command = new SqlCommand("SP_GetMyPRReq", conn))
      {
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@emp_code", sEMPCODE);

        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            results.Add(new
            {
              prno = reader["prno"]?.ToString(),
              reqDate = (DateTime)reader["reqDate"],
              create_dt = (DateTime)reader["create_dt"],
              reqDate_txt = reader["reqDate_txt"]?.ToString(),
              prstatus = (Int32)reader["prstatus"],
              prstatus_txt = reader["prstatus_txt"]?.ToString(),
              count_draft = (Int32)reader["count_draft"],
              count_ongoing = (Int32)reader["count_ongoing"],
              count_reject = (Int32)reader["count_reject"],
              count_approved = (Int32)reader["count_approved"],
              pr_recvpono = reader["pr_recvpono"]?.ToString(),
              pr_recvdt_txt = reader["pr_recvdt_txt"]?.ToString(),

              procure_remark = reader["procure_remark"].ToString() != "" ? reader["procure_remark"].ToString() : "",
              remarkEmp = reader["remarkEmp"].ToString() != "" ? reader["remarkEmp"].ToString() : "",
              remarkCount = reader["remarkCount"].ToString() != "" ? reader["remarkCount"].ToString() : "",
              remarkAuth = reader["remarkAuth"].ToString() != "" ? reader["remarkAuth"].ToString() : "",

              purpose_type = (Int32)reader["purpose_type"],
              projectno = reader["projectno"].ToString() != "" ? reader["projectno"].ToString() : ""

            });
          }
        }

      }
      conn.CloseAsync();
    }
    return Json(results.ToList());
  }
  [HttpGet]
  public IActionResult GetMyPRApproval(string? search)
  {
    IConfiguration _configuration = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json")
                      .Build();

    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    var results = new List<dynamic>();
    string sEMPCODE = HttpContext.Session.GetString(SessionModel.EMPCODE);

    using (var conn = new SqlConnection(DBConn))
    {
      conn.Open();
      using (var command = new SqlCommand("SP_GetMyPRApproval", conn))
      {
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@emp_code", sEMPCODE);

        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            results.Add(new
            {
              prno = reader["prno"]?.ToString(),
              reqDate = (DateTime)reader["reqDate"],
              create_dt = (DateTime)reader["create_dt"],
              reqDate_txt = reader["reqDate_txt"]?.ToString(),
              prstatus = (Int32)reader["prstatus"],
              prstatus_txt = reader["prstatus_txt"]?.ToString(),
              count_draft = (Int32)reader["count_draft"],
              count_ongoing = (Int32)reader["count_ongoing"],
              count_reject = (Int32)reader["count_reject"],
              count_approved = (Int32)reader["count_approved"],
              pr_recvpono = reader["pr_recvpono"]?.ToString(),
              
              pr_recvdt_txt = reader["pr_recvdt_txt"]?.ToString(),

              procure_remark = reader["procure_remark"].ToString() != "" ? reader["procure_remark"].ToString() : "",
              remarkEmp = reader["remarkEmp"].ToString() != "" ? reader["remarkEmp"].ToString() : "",
              remarkCount = reader["remarkCount"].ToString() != "" ? reader["remarkCount"].ToString() : "",
              remarkAuth = reader["remarkAuth"].ToString() != "" ? reader["remarkAuth"].ToString() : "",

              purpose_type = (Int32)reader["purpose_type"],
              projectno = reader["projectno"].ToString() != "" ? reader["projectno"].ToString() : ""

            });
          }
        }

      }
      conn.CloseAsync();
    }
    return Json(results.ToList());
  }
  [HttpGet]
  public IActionResult GetMyPRApprovalPrivate(string? search, int? prstatus)
  {
    IConfiguration _configuration = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json")
                      .Build();
    //--> Start Authen
    string sUSERLOGON = "";
    string AuthenUrl = _configuration[key: "TBCorApiServices:AuthenUrl"];
    if ((HttpContext.Session.GetString(SessionModel.SAMNAME) == null ||
      HttpContext.Session.GetString(SessionModel.SAMNAME) == ""
      ))
    {
      //-->Response.Redirect("https://btauthen.berninathailand.com/?url=https://www.google.com/");
      Response.Redirect(AuthenUrl);
      //-->Response.Redirect(RootURL + "?url=https://www.google.com/");                
    }
    //--> End Authen

    string DBConn = _configuration[key: "ConnectionStrings:BtProcureConn"];
    var results = new List<dynamic>();
    string sEMPCODE = HttpContext.Session.GetString(SessionModel.EMPCODE);

    using (var conn = new SqlConnection(DBConn))
    {
      conn.Open();
      using (var command = new SqlCommand("SP_GetMyPRApprovalPrivate", conn))
      {
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@emp_code", sEMPCODE);
        command.Parameters.AddWithValue("@prstatus", prstatus == null ? 0 : prstatus);

        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            results.Add(new
            {
              prno = reader["prno"]?.ToString(),
              reqDate = (DateTime)reader["reqDate"],
              create_dt = (DateTime)reader["create_dt"],
              reqDate_txt = reader["reqDate_txt"]?.ToString(),
              prstatus = (Int32)reader["prstatus"],
              prstatus_txt = reader["prstatus_txt"]?.ToString(),
              count_draft = (Int32)reader["count_draft"],
              count_ongoing = (Int32)reader["count_ongoing"],
              count_reject = (Int32)reader["count_reject"],
              count_approved = (Int32)reader["count_approved"],
              pr_recvpono = reader["pr_recvpono"]?.ToString(),
              
              pr_recvdt_txt = reader["pr_recvdt_txt"]?.ToString(),

              procure_remark = reader["procure_remark"].ToString() != "" ? reader["procure_remark"].ToString() : "",
              remarkEmp = reader["remarkEmp"].ToString() != "" ? reader["remarkEmp"].ToString() : "",
              remarkCount = reader["remarkCount"].ToString() != "" ? reader["remarkCount"].ToString() : "",
              remarkAuth = reader["remarkAuth"].ToString() != "" ? reader["remarkAuth"].ToString() : "",

              purpose_type = (Int32)reader["purpose_type"],
              projectno = reader["projectno"].ToString() != "" ? reader["projectno"].ToString() : ""

            });
          }
        }

      }
      conn.CloseAsync();
    }
    return Json(results.ToList());
  }
}
