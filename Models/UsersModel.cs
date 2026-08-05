using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using System.Reflection.Emit;
using System.Security.AccessControl;

namespace AspnetCoreMvcFull.Models
{
  public class UsersModel
  {
    public string USERID { get; set; }
    public string DISPNAME { get; set; }
    public string UEMAIL { get; set; }
    public string SAMACC { get; set; }
    public string appleave { get; set; }
  }
  public class AttendanceModel
  {
      public string pkattdn { get; set; }
      public string empcode { get; set; }
      public string ddmmyy { get; set; }
      public string atttime { get; set; }
      public string empcodeFull { get; set; }
      public DateTime? ddmmyyFull { get; set; }
      public string atttimeFull { get; set; }
  }
  public class PPStockModel
  {
    public string itemno { get; set; }
    public string prodname { get; set; }
    public string searchname { get; set; }
    public string  dimension1 { get; set; }
    public string dimension2 { get; set; }
    public string warehouse { get; set; }
    public string batchno { get; set; }
    public string onlocat { get; set; }
    public string fincostamont { get; set; }
    public string phyinventory { get; set; }
    public string phyreserv { get; set; }
    public string avaiphy { get; set; }
    public string ordertotal { get; set; }
    public string onorder { get; set; }
    public string orderreserv { get; set; }
    public string totalavailable { get; set; }
    public DateTime? UPDATE_DATE { get; set; }
  }
  public class PartsListModel
  {
    public string id { get; set; }
    public string itemdoc { get; set; }
    public string itemno { get; set; }
    public string operation { get; set; }
    public string toollife { get; set; }
    public string itemqty { get; set; }
    public DateTime? UPDATE_DATE { get; set; }
    public string itemname { get; set; }
    public string locateno { get; set; }
    public string toolsname { get; set; }
  }
  public class PartsListNBatchNoModel
  {
    public string id { get; set; }
    public string itemdoc { get; set; }
    public string itemno { get; set; }
    public string operation { get; set; }
    public string toollife { get; set; }
    public string itemqty { get; set; }
    public DateTime? UPDATE_DATE { get; set; }
    public string itemname { get; set; }
    public string locateno { get; set; }
    public string toolsname { get; set; }
    public string batchno { get; set; }
  }
  public class UserStockModel
  {
    public string ID { get; set; }
    public string EMPCODE { get; set; }
    public string TMP_FNAME { get; set; }
    public string USERFNAME { get; set; }
    public string USERLOGON { get; set; }
    public int? USRROLE { get; set; }
    public string USRFUNC { get; set; }
    public DateTime? CREATE_DT { get; set; }
    public DateTime? CREATE_USER { get; set; }
    public string UPDATE_DT { get; set; }
    public string UPDATE_USER { get; set; }
    public string ORDMAIL { get; set; }
    public string apprv_stock { get; set; }
    
  }
  public class OrderListModel
  {
    public string ID { get; set; }
    public string ordNo { get; set; }
    public DateTime? ordDate { get; set; }
    public string emp_code { get; set; }
    public string emp_rec { get; set; }
    public string emp_code_fname { get; set; }
    public string emp_rec_fname { get; set; }
    public string print1 { get; set; }
    public string print2 { get; set; }
    public int ord_stat  { get; set; }
  }
  public class OrderModel
  {
    public string ID { get; set; }
    public string ordNo { get; set; }
    public DateTime? ordDate { get; set; }
    public string emp_code { get; set; }
    public string emp_rec { get; set; }
    public int print1 { get; set; }
    public int print2 { get; set; }
    public string emp_app { get; set; }
    public string emp_app_email { get; set; }
    public int ord_stat { get; set; }
  }
  public class OrderDetailModel
  {
    public string ID { get; set; }
    public string ItemDoc { get; set; }
    public string Itemno { get; set; }
    public string Itemname { get; set; }
    public int Itemrem { get; set; }
    public int Itemqty { get; set; }
    public int Itemapp { get; set; }
    public string ordNo { get; set; }
    public string emp_code { get; set; }
    public string emp_app { get; set; }
    public string emp_code_fname { get; set; }
    public string emp_app_fname { get; set; }
    public int ord_stat  { get; set; }
    public string locateno { get; set; }
  }
  public class AllSammUsers
  {
    public string USERLOGON { get; set; }
    public string UEMAIL {  get; set; }
  }
  public class HRUserModel
  {
    public string EMP_CODE { get; set; }
    public string DISPNAME {  get; set; }
    public string UEMAIL {  get; set; }
  }
  public class WORoutingModel
  {
    //ppWo	itemParts	itemName	oprQtyTxt
    public string ppWo { get; set; }
    public string itemParts {  get; set; }
    public string itemName {  get; set; }
    public string oprQtyTxt {  get; set; }
  }
  public class ViewModelVM
  {        
      public List <UserStockModel> VMCosrUses { get; set; }
      //public List <AllSammUsers> VMAllSamAcc { get; set; }
      public List<HRUserModel> VMHrUserModel { get; set;}
  }

  public class AddRecSheetModelVM
  {        
      public List <WORoutingModel> VMWORouting { get; set; }
      public List <UserStockModel> VMCosrUses { get; set; }
  }
  public class AddRecSheetDayModelVM
  {        
      public List <RoutingResourceModel> VMRoutingResource { get; set; }
      public List<HRUserModel> VMHrUserModel { get; set;}
  }
  public class ViewOrdDetailModelVM
  {        
      public List <OrderDetailModel> VMOrderDetail { get; set; }
      //public List <AllSammUsers> VMAllSamAcc { get; set; }
      public List<HRUserModel> VMHrUserModel { get; set;}
  }
  public class RoutingResourceModel
  {
    public string ID { get; set; }
    public string itemParts { get; set; }
    public string itemName { get; set; }
    public int oprQty { get; set; }
    public string rouPriority { get; set; }
    public string oprCode { get; set; }
    public string oprCost { get; set; }
    public decimal oprRuntime { get; set; }
    public decimal mctime { get; set; }
    public decimal procQty { get; set; }
    public int procNext { get; set; }
    public string rouGroup { get; set; }
    public string itemCode { get; set; }
    public string rouCode { get; set; }
    public int rouResp { get; set; }
    public DateTime? create_dt { get; set; }
    public decimal setuptime {  get; set; }
    public string planitemno {  get; set; }
    public string planitemname {  get; set; }
    public int planqty {  get; set; }
    public int woitem_stat {  get; set; }
    public int opr_stat {  get; set; }
  }
  public class RoutingReqModel
  {
    public string ID { get; set; }
    public string itemParts { get; set; }
    public string ppWo { get; set; }
    public int opr_stat {  get; set; }
  }

  public class AddPartsManualModel
  {
    public string mas_itemno { get; set; }
    public string mas_itemnoname  { get; set; }
    public string box_locno  { get; set; }
    public string box_itemlocate  { get; set; }
    public string par_itemdoc  { get; set; }
    public string par_itemname  { get; set; }
    public int par_operation  { get; set; }
    public int par_toollife  { get; set; }
    public int par_itemqty  { get; set; }
    public string searchname  { get; set; } 
    public string dimension1  { get; set; }
    public string dimension2  { get; set; }
    public string warehouse  { get; set; }
    public string batchno  { get; set; }
    public string onlocat  { get; set; }
    public string fincostamont  { get; set; }
    public string phyinventory  { get; set; }
    public string phyreserv  { get; set; }
    public string avaiphy  { get; set; }
    public string ordertotal  { get; set; } 
    public string onorder  { get; set; }
    public string orderreserv  { get; set; }
    public string totalavailable  { get; set; }
  }

  public class PartListForEditModel
  {
    public string id {  get; set; }
    public string prodname { get; set; }
    public string searchname { get; set; }
    public string dimension1 { get; set; }
    public string dimension2 { get; set; }
    public string warehouse { get; set; }
    public string batchno { get; set; }
    public string onlocat { get; set; }
    public string fincostamont { get; set; }
    public string phyinventory { get; set; }
    public string phyreserv { get; set; }
    public string avaiphy { get; set; }
    public string ordertotal { get; set; }
    public string onorder { get; set; }
    public string orderreserv { get; set; }
    public string totalavailable { get; set; }
    public string itemdoc { get; set; }
    public string itemno { get; set; }
    public int operation { get; set; }
    public int toollife { get; set; }
    public int itemqty { get; set; }
    public string docname { get; set; }
    public string locno { get; set; }
    public string itemlocate { get; set; }

  }
  public class WoListModel
  {
    public string ppWo { get; set; }
    public string itemParts { get; set; }
    public string itemName { get; set; }
    public string wo_stat {  get; set; }
  }
  public class TimeSheetModel
  {
    public string ID {  get; set; }
    public string mas_wo {  get; set; }
    public string mas_itemno {  get; set; }
    public int mas_opr {  get; set; }
    public int mas_qty {  get; set; }
    public decimal mas_stdtime {  get; set; }
    public string mas_resource {  get; set; }
    public decimal mas_mc {  get; set; }
    public decimal mas_lab {  get; set; }
    public string emp_code {  get; set; }
    public string rec_date_txt {  get; set; }
    public DateTime? rec_date {  get; set; }
    public decimal rec_setup {  get; set; }
    public decimal rec_mc		 {  get; set; }
    public decimal rec_lab	 {  get; set; }
    public decimal rec_aqty	 {  get; set; }
    public decimal rec_atotal	 {  get; set; }
    public decimal rec_eff		 {  get; set; }
    public DateTime create_dt {  get; set; }
    public DateTime update_dt {  get; set; }
    public decimal ng_qty {  get; set; }
    public decimal ng_total{  get; set; }
    public int opr_stat {  get; set; }

  }
  public class ScrapModel
  {
    public string ID { get; set; }
    public string mas_wo { get; set; }
    public string mas_itemno { get; set; }
    public int mas_opr { get; set; }
    public string emp_code { get; set; }
    public DateTime rec_date { get; set; }
    public string rec_date_txt { get; set; }
    public decimal prd_setup { get; set; }
    public decimal prd_tools { get; set; }
    public decimal prd_surf { get; set; }
    public decimal prd_dimout { get; set; }
    public decimal prd_other { get; set; }
    public string scrap_remark { get; set; }
    public decimal ven_hardness { get; set; }
    public decimal ven_dimout { get; set; }
    public decimal ven_surf { get; set; }
    public decimal ven_other { get; set; }
    public string vendor_remark { get; set; }
    public string other_remark { get; set; }
    public int app1_stat { get; set; }
    public int app2_stat { get; set; }
    public int app3_stat { get; set; }
    public string app1_mail { get; set; }
    public string app2_mail { get; set; }
    public string app3_mail { get; set; }
    public DateTime app1_date { get; set; }
    public DateTime app2_date { get; set; }
    public DateTime app3_date { get; set; }
    public DateTime create_dt { get; set; }
    public DateTime update_dt { get; set; }
    public int scrap_stat {  get; set; }
    public int opr_stat {  get; set; }
  }
  public class ScrapReportModel
  {
    public DateTime rec_date {  get;  set; }
    public string rec_date_txt {  get;  set; }
    public int prod_count {  get;  set; }
    public int vedd_count {  get;  set; }
    public int approval {  get;  set; }
    public string approval_txt {  get; set; }
  }
  public class ProcureUserModel
  {

    public string ID { get; set; }
    public string EMP_CODE { get; set; }
    public string USERLOGON { get; set; }
    public string DISPNAME { get; set; }
    public string UEMAIL { get; set; }
    public int USRROLE { get; set; }
    public int apprv_proc { get; set; }
    public string apprv_proc_txt { get; set; }
    public string USERROLE_TXT { get; set; }

  }
  public class NewPRModelVM
  {
    public List<HRUserModel> VMHRUserModel { get; set; }
    public List<ProcureUserModel> VMProcureUserModel { get; set; }
    public List<VenModel> VMVenModel { get; set; }
    public List<ProjModel> VMProjModel { get; set; }
    public List<AccModel> VMAccModel { get; set; }
    public List<CurrencyModel> VMCurrencyModel { get; set; }
    public List<ApprovalListModel> VMApprovalListRole1Model { get; set; }
    public List<ApprovalListModel> VMApprovalListRole2Model { get; set; }
    public List<ApprovalListModel> VMApprovalListRole3Model { get; set; }
    public List<ItemsPartsModel> VMItemsPartsModel { get; set; }

  }
  //VenName, VenCode, Vencurrency
  //ProjNo	ProjName	ProjStat	ActiveTo
  //AccMain, AccName, AccType, AccCat
  public class CurrencyModel
  {
    public string Vencurrency { get; set; }
  }
  public class AccModel
  {
    public string AccMainTmp { get; set; }
    public string AccMain { get; set; }
    public string AccName { get; set; }
    public string AccType { get; set; }
    public string AccCat { get; set; }
  }
  public class VenModel
  {
    public Guid? id { get; set; }
    public string VenName { get; set; }
    public string VenCode { get; set; }
    public string Vencurrency { get; set; }
  }
  public class BtItemPartModel
  {
    public string itemnoTmp { get; set; }
    public string itemno { get; set; }
    public string itemname { get; set; }
    public string itemdoc { get; set; }
  }
  public class ProjModel
  {
    public string ProjNoTmp { get; set; }
    public string ProjNo { get; set; }
    public string ProjName { get; set; }
    public string ProjStat { get; set; }
    public DateTime? ActiveTo { get; set; }
    public string ActiveToTxt { get; set; }
  }
  public class PRHeaderModel
  {
    public string id { get; set; }
    public string prno { get; set; }
    public string projectno { get; set; }
    public string empcode { get; set; }
    public string empcode_txt { get; set; }
    public int? approx_type { get; set; }
    public DateTime? approx_dt { get; set; }
    public string approx_dt_txt { get; set; }
    public string invcreditno { get; set; }
    public int? purpose_type { get; set; }
    public string ref_docs { get; set; }
    public string pr_reason { get; set; }
    public DateTime? pr_recvdt { get; set; }
    public string pr_recvdt_txt { get; set; }
    public string pr_recvpono { get; set; }
    public string attach_flag { get; set; }
    public string reqDepCode { get; set; }
    public DateTime? reqDate { get; set; }
    public int? reqFlag { get; set; }
    public string appEmp { get; set; }
    public DateTime? appDate { get; set; }
    public int? appFlag { get; set; }
    public string countEmp { get; set; }
    public DateTime? countDate { get; set; }
    public int? countFlag { get; set; }
    public string authEmp { get; set; }
    public DateTime? authDate { get; set; }
    public int? authFlag { get; set; }
    public int prstatus { get; set; }
    public DateTime? create_dt { get; set; }
    public DateTime? update_dt { get; set; }
    public string update_dt_txt { get; set; }
    public string prstatus_txt { get; set; }
    public string reqDate_txt { get; set; }
    public string pub_remark { get; set; }
    public string prcurrency { get; set; }

    public string id_supp1 { get; set; }
    public string name_supp1 { get; set; }
    public string vc_supp1 { get; set; }
    public string contact_supp1 { get; set; }
    public string email_supp1 { get; set; }
    public string tel_supp1 { get; set; }
    public string remark_supp1 { get; set; }
    public string quoref_supp1 { get; set; }
    public DateTime? refnodt_supp1 { get; set; }
    public string id_supp2 { get; set; }
    public string name_supp2 { get; set; }
    public string vc_supp2 { get; set; }
    public string contact_supp2 { get; set; }
    public string email_supp2 { get; set; }
    public string tel_supp2 { get; set; }
    public string remark_supp2 { get; set; }
    public string quoref_supp2 { get; set; }
    public DateTime? refnodt_supp2 { get; set; }
    public string id_supp3 { get; set; }
    public string name_supp3 { get; set; }
    public string vc_supp3 { get; set; }
    public string contact_supp3 { get; set; }
    public string email_supp3 { get; set; }
    public string tel_supp3 { get; set; }
    public string remark_supp3 { get; set; }
    public string quoref_supp3 { get; set; }
    public DateTime? refnodt_supp3 { get; set; }
    public string id_supp4 { get; set; }
    public string name_supp4 { get; set; }
    public string vc_supp4 { get; set; }
    public string contact_supp4 { get; set; }
    public string email_supp4 { get; set; }
    public string tel_supp4 { get; set; }
    public string remark_supp4 { get; set; }
    public string quoref_supp4 { get; set; }
    public DateTime? refnodt_supp4 { get; set; }
    public string id_supp5 { get; set; }
    public string name_supp5 { get; set; }
    public string vc_supp5 { get; set; }
    public string contact_supp5 { get; set; }
    public string email_supp5 { get; set; }
    public string tel_supp5 { get; set; }
    public string remark_supp5 { get; set; }
    public string quoref_supp5 { get; set; }
    public DateTime? refnodt_supp5 { get; set; }
    public string reqEmail  { get; set; }
    public string appEmail  { get; set; }
    public string appEmail2  { get; set; }
    public string CountEmail { get; set; }
    public string authEmail { get; set; }
    public string appEmp_txt { get; set; }
    public string appDate_txt { get; set; }
    public string countEmp_txt { get; set; }
    public string countDate_txt { get; set; }
    public string authEmp_txt { get; set; }
    public string authDate_txt { get; set; }
    public int approve_step { get; set; }
    public int flagm_proc { get; set;  }
    public int procure_flag { get; set; }
    public string procure_remark { get; set; }
    public string codelog { get; set; }
    public string remarkEmp { get; set; }
    public string remarkCount { get; set; }
    public string remarkAuth { get; set; }
    public int quo_return { get; set; }
    public decimal? total_disc { get; set; }
    public string projectname { get; set; }
    public string appEmp2 { get; set; }
    public DateTime? appDate2 { get; set; }
    public string appDate2_txt { get; set; }
    public string appEmp2_txt { get; set; }
    public int? appFlag2 { get; set; }
    public string remarkEmp2 { get; set; }
    public int revision_no { get; set; }
  }
  public class PRSuggVendorModel
  {
    public Guid id { get; set; }
    public Guid ref_prid { get; set; }
    public int sugg_item { get; set; }
    public string vencode { get; set; }
    public string venname { get; set; }
    public string venvc { get; set; }
    public string vencontact { get; set; }
    public string venemail { get; set; }
    public string ventelfax { get; set; }
    public string venremark { get; set; }
    public string rfq_no { get; set; }
    public DateTime? create_dt { get; set; }
    public DateTime? update_dt { get; set; }
    public string name_supp { get; set; }
    public string quoref_supp { get; set; }
    public DateTime? refnodt_supp { get; set; }
    public string currency { get; set; }
    public string refnodt_supp_txt { get; set; }
  }
  public class PRDetailModel
  {
    public string ID { get; set; }
    public string prno { get; set; }
    public string itemno { get; set; }
    public string itemname { get; set; }
    public int? itemqty { get; set; }
    public string itemunit { get; set; }
    public decimal? itemprice { get; set; }
    public decimal? itemtotal { get; set; }
    public string itemremark { get; set; }
    public int? itemstat { get; set; }
    public decimal? item_disc { get; set; }
  }

  public class PRModelVM
  {
    public List<PRHeaderModel> VMPRHeader { get; set; }
    public List<PRDetailModel> VMPRDetail { get; set; }
    public List<HRUserModel> VMHrUserModel { get; set; }
    public List<ProcureUserModel> VMProcureUserModel { get; set; }
    public List<VenModel> VMVenModel { get; set; }
    public List<ProjModel> VMProjModel { get; set; }
    public List<AccModel> VMAccModel { get; set; }
  }
  public class MyPRReqModel 
  {
    public string prno { get; set; }
    public DateTime? reqDate { get; set; }
    public string reqDate_txt { get; set; }
    public int prstatus { get; set; }
    public string prstatus_txt { get; set; }
  }
  
  public class PRItemDetailModel
  {
    public string prno { get; set; }               // Purchase Request Number
    public Guid id { get; set; }                  // Primary key
    public string item_btnumber { get; set; }     // BT Number
    public string item_descript { get; set; }     // Description / Spec
    public string item_model { get; set; }        // Brand / Model
    public string item_acccode { get; set; }      // Account Code
    public string item_costdep { get; set; }      // Cost Dept

    public decimal? item_qty { get; set; }        // Quantity
    public string item_unit { get; set; }         // Unit of Measure
    public string item_unitprice { get; set; }    // Unit Price (string for flexible formatting)
    public decimal? item_amount { get; set; }     // Total Amount
    public decimal? item_disc { get; set; }     // Total Amount

    public DateTime? create_dt { get; set; }      // Created Date
    public DateTime? update_dt { get; set; }      // Updated Date
  }
  public class FileAttachedModel
  {
    public Guid id { get; set; }
    public Guid ref_prid { get; set; }   // FK -> PRHeader.id
    public int filetype { get; set; }    // กำหนดประเภทไฟล์ตามระบบของคุณ
    public string filetype_txt { get; set; }    // กำหนดประเภทไฟล์ตามระบบของคุณ
    public string filename { get; set; } // ชื่อไฟล์ที่แสดง (original)
    public string filepath { get; set; } // พาธที่บันทึกในเว็บ (เช่น /uploads/xxxx.pdf)
    public DateTime? create_dt { get; set; }
  }
  public class ApprovalListModel
  {
    public string emp_code { get; set; }
    public int dep_code { get; set; }
    public string uemail { get; set; }
    public int usrrole { get; set; }
    public string dispname { get; set; }
  }
  public class ApprovalStausModel
  {
    public string prno { get; set; }
    public int app_role { get; set; }
    public int app_status { get; set; }
    public string approval_remark { get; set; }
    public int? person { get; set; }
  }
  public class ReviseSuggesVendorModel
  {
    public string prno { get; set; }
    public int sugg_item { get; set; }
    public string id_supp { get; set; }
    public string name_supp { get; set; }
    public string vc_supp { get; set; }
    public string contact_supp { get; set; }
    public string email_supp { get; set; }
    public string tel_supp { get; set; }
    public string remark_supp { get; set; }
    public string quoref_supp { get; set; }
    public DateTime? refnodt_supp { get; set; }
  }
  public class ItemsPartsModel
  {
    public Guid id { get; set; }
    public string itemdoc { get; set; }
    public string itemno { get; set; }
    public int operation { get; set; }
    public int toollife { get; set; }
    public int itemqty { get; set; }
    public DateTime? UPDATE_DATE { get; set; }
    public string itemname { get; set; }
    public int mflg { get; set; }

  }
  public class PrHistoryRemarkModel
  {
    public Guid id { get; set; }
    public Guid ref_prid { get; set; }
    public string empcode { get; set; }
    public string remarks { get; set; }
    public DateTime? create_dt { get; set; }
    public DateTime? update_dt { get; set; }
    public string prno { get; set; }
    public string create_dt_txt { get; set; }
  }
  public class PRHeaderViewModel
  {
    public string codelog { get; set; }
    public string prstatus_txt { get; set; }
    public string projectno { get; set; }
    public string prno { get; set; }
    public string approx_dt_txt { get; set; }
    public string update_dt_txt { get; set; }
    public string empcode_txt { get; set; }
    public string pr_reason { get; set; }
    public string pr_recvdt_txt { get; set; }
    public string pr_recvpono { get; set; }
    public string reqDepCode { get; set; }
    public string create_dt_txt { get; set; }
    public DateTime? create_dt { get; set; }
    public string procure_remark { get; set; }
    public int purpose_type { get; set; }
    public int quo_return { get; set; }
    public string appEmp_txt { get; set; }
    public string appEmp2_txt { get; set; }
    public string countFlag_txt { get; set; }
    public string authEmp_txt { get; set; }
    public decimal? total_disc { get; set; }
    public decimal? total_exp { get; set; }
    public string prcurrency { get; set; }
    public int revision_no { get; set; }
    public string revision_dt_txt { get; set; }
  }
  public class PROutputModel
  {
    public int submited { get; set; }
    public int inprocess { get; set; }
    public int procure_rec { get; set; }
    public int all_pr { get; set; }
    public int completed { get; set; }
  }
  public class PRPrintViewModel
  {
    public PRHeaderModel PRHeader { get; set; }
    public List<PRItemDetailModel> DetailList { get; set; }
    public List<PRSuggVendorModel> VendorList { get; set; }
    public List<FileAttachedModel> AttachList { get; set; }
  }
  public class EmailLogRequestModel
  {
    public string MailFrom { get; set; }
    public string MailTo { get; set; }
    public string Subject { get; set; }
  }
}
