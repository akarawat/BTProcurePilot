namespace AspnetCoreMvcFull.Models
{
  public class GlobalModel
  {
    public IEnumerable<DropdownModel> DDList { get; set; }
  }
  public class SessionModel
  {
    public const string SessionKeyName = "_Name";
    public const string SessionKeyAge = "_Age";
    public const string SAMNAME = "_SAMNAME";
    public const string USRDEPART = "_USRDEPART";
    public const string USEREMAIL = "_USEREMAIL";
    public const string ROLEDOCS = "_ROLEDOCS";
    public const string ROLEMGR = "_ROLEMGR";

    public const string EMPCODE = "_EMPCODE";
    public const string USERLOGON = "_USERLOGON";
    public const string DISPNAME = "_DISPNAME";
    public const string DISPNAMETH = "_DISPNAMETH";
    public const string UEMAIL = "_UEMAIL";
    public const string DEPART = "_DEPART";
    public const string DESCRIPT = "_DESCRIPT";
    public const string REPORTER = "_REPORTER";
    public const string DEPCODE = "_DEPCODE";
    public const string MAXSIZE = "_MAXSIZE";
    public const string EMPSHIFT = "_EMPSHIFT";

    public const string MSGAPPROVE = "_MSGAPPROVE";
    public const string MSGPEND = "_MSGPEND";
    public const string MSGCLOSE = "_MSGCLOSE";
    public const string MSGCANCEL = "_MSGCANCEL";
    public const string EMAILSTOCCTRL = "_EMAILSTOCCTRL";

    public const string ORDOPEN = "_ORDOPEN";
    public const string ORDCLOSE = "_ORDCLOSE";

    public const string UROLEADMIN = "_UROLEADMIN";
    public const string UROLESUPP = "_UROLESUPP";
    public const string UROLEMGR = "_UROLEMGR";
    public const string UROLEMD = "_UROLEMD";
    public const string MAILGROUPADMIN = "_MAILGROUPADMIN";

  }
  public class DropdownModel
  {
    public string DDLDISP { get; set; }
    public string DDLVAL { get; set; }
  }
  public class ResultModel
  {
    public string RtnStatus { get; set; }
    public string RtnMsg { get; set; }
    public string RtnCode { get; set; }
  }
  public class SenderMailModel
  {
      public string Body { get; set; }
      public string Form { get; set; }
      public string MailTo { get; set; }
      public string Subject { get; set; }
      public string Addresses { get; set; }
      public int Priority { get; set; }
  }
  public class LowStockItem
  {
      public string Itemno { get; set; }
      public int TotalInventory { get; set; }
      public int TotalMinimum { get; set; }
      public int DiffQty { get; set; }
  }
}
