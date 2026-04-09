using AspnetCoreMvcFull.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace AspnetCoreMvcFull.Controllers
{
  public class DataAccess
  {
    IConfiguration _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    public List<string> GetOpenOrCloseDoc(string emp_code)
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
              SqlCommand cmnd = new SqlCommand("SP_CountOpenClosePRq", con);
              cmnd.CommandType = CommandType.StoredProcedure;
              cmnd.Parameters.AddWithValue("@emp_code", SqlDbType.VarChar).Value = emp_code;

              cmnd.Parameters.Add("@DOC_OPEN", SqlDbType.Int);
              cmnd.Parameters["@DOC_OPEN"].Direction = ParameterDirection.Output;

              cmnd.Parameters.Add("@DOC_CLOSE", SqlDbType.Int);
              cmnd.Parameters["@DOC_CLOSE"].Direction = ParameterDirection.Output;

              cmnd.ExecuteNonQuery();
              strMsg[0] = cmnd.Parameters["@DOC_OPEN"].Value.ToString();
              strMsg[1] = cmnd.Parameters["@DOC_CLOSE"].Value.ToString();
          }
          catch (Exception ex)
          {
              strMsg[0] = "0";
              strMsg[1] = "0";
              strMsg[2] = "error";
          }
          finally
          {
              strMsg[2] = "success";
              con.Close();
          }
      return strMsg.ToList();
    }
    public List<string> GetPendingApprove(string SAMACC)
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
        SqlCommand cmnd = new SqlCommand("SP_CountPendingApprove", con);
        cmnd.CommandType = CommandType.StoredProcedure;
        cmnd.Parameters.AddWithValue("@emp_code", SqlDbType.VarChar).Value = SAMACC;
        cmnd.Parameters.Add("@PendCount", SqlDbType.Int);
        cmnd.Parameters["@PendCount"].Direction = ParameterDirection.Output;
        cmnd.ExecuteNonQuery();
        strMsg[0] = cmnd.Parameters["@PendCount"].Value.ToString();
        con.Close();
      }
      catch (Exception ex)
      {
        strMsg[0] = "0";
        strMsg[1] = "error";
      }
      finally
      {
        strMsg[1] = "success";
        con.Close();
      }
      return strMsg.ToList();
    }
    public List<string> GetUserLoginfo(string SAMACC)
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
              SqlCommand cmnd = new SqlCommand("SP_GetUserLoginfo", con);
              cmnd.CommandType = CommandType.StoredProcedure;
              cmnd.Parameters.AddWithValue("@SAMACC", SqlDbType.VarChar).Value = SAMACC;
              cmnd.Parameters.Add("@EMAIL", SqlDbType.VarChar);
              cmnd.Parameters["@EMAIL"].Direction = ParameterDirection.Output;

              cmnd.ExecuteNonQuery();
              strMsg[0] = "1";
              strMsg[1] = cmnd.Parameters["@EMAIL"].Value.ToString();
          }
          catch (Exception ex)
          {
              strMsg[0] = "0";
              strMsg[1] = "";
              strMsg[2] = "error";
          }
          finally
          {
              strMsg[2] = "success";
              con.Close();
          }
      return strMsg.ToList();
    }
    public int GetUserPermissFunc(string emp_code, string ufunc)
    {
      int usrFunc = 0;
            IConfiguration _configuration = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json")
                              .Build();

      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
      SqlConnection con = null;

      //exec SP_CountOpenCloseOrder @ORD_OPEN=@ORD_OPEN output, @ORD_CLOSE=@ORD_CLOSE output;
      try
          {
              con = new SqlConnection(DBConn);
              con.Open();
              SqlCommand cmnd = new SqlCommand("SP_GetUserPermiss", con);
              cmnd.CommandType = CommandType.StoredProcedure;
              cmnd.Parameters.AddWithValue("@USERLOGON", SqlDbType.VarChar).Value = emp_code;
              cmnd.Parameters.AddWithValue("@USRFUNC", SqlDbType.VarChar).Value = ufunc;

              cmnd.Parameters.Add("@AUTH", SqlDbType.Int);
              cmnd.Parameters["@AUTH"].Direction = ParameterDirection.Output;

              cmnd.ExecuteNonQuery();
              usrFunc = (int)cmnd.Parameters["@AUTH"].Value;
          }
          catch (Exception ex){}
          finally
          {
              con.Close();
          }
      return usrFunc;
    }
    public async void GetOpenOrCloseDoc_Bak()
    {
      string[] strMsg = new string[3];
            IConfiguration _configuration = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json")
                              .Build();

      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
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
              //Session.SetString(SessionModel.ORDOPEN, cmnd.Parameters["@ORD_OPEN"].Value.ToString());
              //Session.SetString(SessionModel.ORDCLOSE, cmnd.Parameters["@ORD_CLOSE"].Value.ToString());
                
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
    public List<HRUserModel> GetAllUserInDomain()
    {
      //----> Get All users from AD.
      List<HRUserModel> ressam = new List<HRUserModel>();
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
      }
      catch (Exception ex)
      {
      }
      return ressam.ToList();
    }
    public List<string> GetADUserDetail(string SAMACC)
    {
      string[] strMsg = new string[3];
            IConfiguration _configuration = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json")
                              .Build();

      string DBConn = _configuration[key: "ConnectionStrings:BtCostReduct"];
      SqlConnection con = null;

      //exec SP_CountOpenCloseOrder @ORD_OPEN=@ORD_OPEN output, @ORD_CLOSE=@ORD_CLOSE output;
      try
          {
              con = new SqlConnection(DBConn);
              con.Open();
              SqlCommand cmnd = new SqlCommand("SP_GeSAMADUser", con);
              cmnd.CommandType = CommandType.StoredProcedure;
              cmnd.Parameters.AddWithValue("@SAMACC", SqlDbType.VarChar).Value = SAMACC;

              cmnd.Parameters.Add("@emp_code", SqlDbType.VarChar, 25);
              cmnd.Parameters["@emp_code"].Direction = ParameterDirection.Output;

              cmnd.Parameters.Add("@DISPNAME", SqlDbType.VarChar, 255);
              cmnd.Parameters["@DISPNAME"].Direction = ParameterDirection.Output;

              cmnd.Parameters.Add("@UEMAIL", SqlDbType.VarChar, 255);
              cmnd.Parameters["@UEMAIL"].Direction = ParameterDirection.Output;

              cmnd.ExecuteNonQuery();
              strMsg[0] = cmnd.Parameters["@emp_code"].Value == DBNull.Value ? "" : cmnd.Parameters["@emp_code"].Value.ToString();
              strMsg[1] = cmnd.Parameters["@DISPNAME"].Value == DBNull.Value ? "" : cmnd.Parameters["@DISPNAME"].Value.ToString();
              strMsg[2] = cmnd.Parameters["@UEMAIL"].Value == DBNull.Value ? "" : cmnd.Parameters["@UEMAIL"].Value.ToString();
          }
          catch (Exception ex)
          {
              strMsg[0] = "";
              strMsg[1] = "";
              strMsg[2] = "error";
          }
          finally
          {
              con.Close();
          }
      return strMsg.ToList();
    }

  }
}
