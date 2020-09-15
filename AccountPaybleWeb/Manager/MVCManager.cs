using AccountPaybleWeb.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using static AccountPaybleWeb.Manager.ViewModelClass;

namespace AccountPaybleWeb.Manager
{
    public static class MVCManager
    {

        #region-----------------------------------List--------------------------------------------

        public static List<string> GetVendorList(string user)
        {
            List<string> list = new List<string>();
            try
            {
                using (GPModel db = new GPModel())
                {
                    list = db.Tbl_AP_PODetail.Where(x => x.Department.Equals(user)).Select(x => x.VendorName).Distinct().ToList();
                }

            }
            catch (Exception ex)
            {

            }
            return list;
        }
        

        public static void UpdateReceiptNumber(long id, string receiptnumber)
        {
            try
            {
                using (GPModel db = new GPModel())
                {
                    var obj = db.Tbl_AP_LineItemDetail.Where(x => x.LineItemID == id).FirstOrDefault();
                    List<Tbl_AP_LineItemDetail> linelist = db.Tbl_AP_LineItemDetail.Where(x => x.InvoiceID == obj.InvoiceID).ToList();
                    foreach (var item in linelist)
                    {
                        item.ReceiptNumber = receiptnumber;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static void UpdateLineLocID(long id, string LocID)
        {
            try
            {
                using (GPModel db = new GPModel())
                {
                    var obj = db.Tbl_AP_LineItemDetail.Where(x => x.LineItemID == id).FirstOrDefault();
                    obj.LineLocationID = LocID;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static void ApproveWarnigMessae(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                string[] stridarr = ids.Split(','); long id;
                stridarr = stridarr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                Tbl_AP_LineItemDetail lineobj = new Tbl_AP_LineItemDetail();
                try
                {
                    using (GPModel db = new GPModel())
                    {
                        foreach (string item in stridarr)
                        {
                            id = Convert.ToInt64(item);
                            lineobj = db.Tbl_AP_LineItemDetail.Where(x => x.LineItemID == id).FirstOrDefault();
                            if (lineobj.MatchOption.ToLower().Equals("2-way"))
                            {
                                lineobj.ViewMapped += ",XXGP_RPA_2WAY_PO_VIEW_010" + ",";
                                var errmodel = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewModelClass.ErrorModel>(lineobj.ErrorDescrpion);
                                errmodel.WarningMessage = ".....";
                                lineobj.ErrorDescrpion = Newtonsoft.Json.JsonConvert.SerializeObject(errmodel);
                            }
                            else
                            {
                                var list = db.Tbl_AP_LineItemDetail.Where(x => x.InvoiceID == lineobj.InvoiceID).ToList();
                                foreach(var i in list)
                                {
                                    i.ViewMapped += "XXGP_RPA_3WAY_PO_VIEW_01" + ",";
                                    var errmodel = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewModelClass.ErrorModel>(i.ErrorDescrpion);
                                    errmodel.WarningMessage = ".....";
                                    i.ErrorDescrpion = Newtonsoft.Json.JsonConvert.SerializeObject(errmodel);


                                }                            
                            }
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

        }

        public static void ApprovePO(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                string[] stridarr = ids.Split(','); long id;
                stridarr = stridarr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                Tbl_AP_PODetail obj = new Tbl_AP_PODetail();
                try
                {
                    using (GPModel db = new GPModel())
                    {
                        foreach (string item in stridarr)
                        {
                            id = Convert.ToInt64(item);
                            obj = db.Tbl_AP_PODetail.Where(x => x.InvoiceID == id).FirstOrDefault();
                            obj.InvoiceStatus = Manager.Constants.PODetailInvoiceStatus.Approved;
                            UpdateInvoiceInErp(id, db);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex) { }
            }

        }
        public static void UpdateInvoiceInErp(long id, GPModel db)
        {
            try
            {
                string invoiceUrl = ConfigurationManager.AppSettings["url"].ToString();
                var pOItem = db.Tbl_AP_PODetail.Where(x => x.InvoiceID == id).FirstOrDefault();
                invoiceUrl = invoiceUrl + pOItem.InvoiceID;
                List<Tbl_AP_LineItemDetail> LineItem = db.Tbl_AP_LineItemDetail.Where(x => x.InvoiceID == pOItem.InvoiceID).ToList();
                if (LineItem[0].MatchOption.ToLower().Equals("2-way"))
                {
                    foreach (var lItem in LineItem)
                    {
                        XXGP_PO_INVOICE_AUTO_TModel tblobj = new XXGP_PO_INVOICE_AUTO_TModel();
                        tblobj.PO_Number = pOItem.PONumber;
                        tblobj.PO_MATCH_OPTION = lItem.MatchOption;
                        tblobj.INVOICE_NUMBER = pOItem.InvoiceNo;
                        tblobj.INVOICE_DATE = pOItem.InvoiceDate.ToString();
                        tblobj.RECEIPT_NUMBER = lItem.ReceiptNumber;
                        tblobj.LINE_LOCATION_ID = lItem.LineLocationID;
                        tblobj.LINE_AMOUNT = lItem.Amount;
                        tblobj.LINE_DESCRIPTION = lItem.InvoiceDescription;
                        tblobj.INVOICE_AMOUNT = pOItem.InvoiceAmount;
                        tblobj.INVOICE_PDF_URL = invoiceUrl;
                        SaveUsingOracleBulkCopy(tblobj);
                    }
                }
                else
                {
                    XXGP_PO_INVOICE_AUTO_TModel tblobj = new XXGP_PO_INVOICE_AUTO_TModel();
                    tblobj.PO_Number = pOItem.PONumber;
                    tblobj.PO_MATCH_OPTION = LineItem[0].MatchOption;
                    tblobj.INVOICE_NUMBER = pOItem.InvoiceNo;
                    tblobj.INVOICE_DATE = pOItem.InvoiceDate.ToString();
                    tblobj.RECEIPT_NUMBER = LineItem[0].ReceiptNumber;
                    tblobj.LINE_LOCATION_ID = string.Empty;
                    tblobj.LINE_AMOUNT = string.Empty;
                    tblobj.INVOICE_AMOUNT = pOItem.InvoiceAmount;
                    tblobj.INVOICE_PDF_URL = invoiceUrl;
                    SaveUsingOracleBulkCopy(tblobj);
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

        }
        public static void SaveUsingOracleBulkCopy(XXGP_PO_INVOICE_AUTO_TModel tblobj)
        {
            try
            {
                string connString = ConfigurationManager.AppSettings["ERPDB"];

                using (var connection = new OracleConnection(connString))
                {

                    string[] PO_NUMBER = new string[1];
                    string[] PO_MATCH_OPTION = new string[1];
                    string[] INVOICE_NUMBER = new string[1];
                    DateTime[] INVOICE_DATE = new DateTime[1];
                    string[] RECEIPT_NUMBER = new string[1];
                    long?[] LINE_LOCATION_ID = new long?[1];
                    string[] LINE_DESCRIPTION = new string[1];
                    long[] LINE_AMOUNT = new long[1];
                    long[] INVOICE_TOTAL = new long[1];
                    string[] READ_FLAG = new string[1];
                    DateTime[] CREATION_DATE = new DateTime[1];
                    string[] InvoiceUrL = new string[1];

                    PO_NUMBER[0] = tblobj.PO_Number;
                    PO_MATCH_OPTION[0] = tblobj.PO_MATCH_OPTION;
                    INVOICE_NUMBER[0] = tblobj.INVOICE_NUMBER;
                    INVOICE_DATE[0] = Convert.ToDateTime(tblobj.INVOICE_DATE);
                    RECEIPT_NUMBER[0] = tblobj.RECEIPT_NUMBER;
                    LINE_LOCATION_ID[0] = (string.IsNullOrEmpty(tblobj.LINE_LOCATION_ID) ? 0 : Convert.ToInt64(tblobj.LINE_LOCATION_ID));
                    LINE_DESCRIPTION[0] = tblobj.LINE_DESCRIPTION;
                    LINE_AMOUNT[0] = (string.IsNullOrEmpty(tblobj.LINE_AMOUNT) ? 0 : Convert.ToInt64(Convert.ToDecimal(tblobj.LINE_AMOUNT)));
                    INVOICE_TOTAL[0] = Convert.ToInt64(Convert.ToDecimal(tblobj.INVOICE_AMOUNT));
                    READ_FLAG[0] = "N";
                    CREATION_DATE[0] = DateTime.Today;
                    InvoiceUrL[0] = tblobj.INVOICE_PDF_URL;


                    OracleParameter ponum = new OracleParameter();
                    ponum.OracleDbType = OracleDbType.Varchar2;
                    ponum.Value = PO_NUMBER;

                    OracleParameter match = new OracleParameter();
                    match.OracleDbType = OracleDbType.Varchar2;
                    match.Value = PO_MATCH_OPTION;

                    OracleParameter invoiceno = new OracleParameter();
                    invoiceno.OracleDbType = OracleDbType.Varchar2;
                    invoiceno.Value = INVOICE_NUMBER;

                    OracleParameter invDate = new OracleParameter();
                    invDate.OracleDbType = OracleDbType.Date;
                    invDate.Value = INVOICE_DATE;

                    OracleParameter ReceiptNumber = new OracleParameter();
                    ReceiptNumber.OracleDbType = OracleDbType.Varchar2;
                    ReceiptNumber.Value = RECEIPT_NUMBER;

                    OracleParameter LineLoc = new OracleParameter();
                    LineLoc.OracleDbType = OracleDbType.Int64;
                    if (string.IsNullOrEmpty(tblobj.LINE_LOCATION_ID))
                    {
                        LineLoc.Value = null;
                    }
                    else
                    {
                        LineLoc.Value = LINE_LOCATION_ID;
                    }


                    OracleParameter Linedesc = new OracleParameter();
                    Linedesc.OracleDbType = OracleDbType.Varchar2;
                    Linedesc.Value = LINE_DESCRIPTION;

                    OracleParameter lineAmt = new OracleParameter();
                    lineAmt.OracleDbType = OracleDbType.Int64;
                    if (string.IsNullOrEmpty(tblobj.LINE_AMOUNT))
                    {
                        lineAmt.Value = null;
                    }
                    else
                    {
                        lineAmt.Value = LINE_AMOUNT;
                    }


                    OracleParameter InvTotal = new OracleParameter();
                    InvTotal.OracleDbType = OracleDbType.Int64;
                    InvTotal.Value = INVOICE_TOTAL;

                    OracleParameter flag = new OracleParameter();
                    flag.OracleDbType = OracleDbType.Varchar2;
                    flag.Value = READ_FLAG;

                    OracleParameter crdate = new OracleParameter();
                    crdate.OracleDbType = OracleDbType.Date;
                    crdate.Value = CREATION_DATE;

                    OracleParameter invUrl = new OracleParameter();
                    invUrl.OracleDbType = OracleDbType.Varchar2;
                    invUrl.Value = InvoiceUrL;



                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO apps.XXGP_PO_INVOICE_AUTO_STG(PO_NUMBER,PO_MATCH_OPTION,INVOICE_NUMBER,INVOICE_DATE,RECEIPT_NUMBER,LINE_LOCATION_ID,LINE_DESCRIPTION,LINE_AMOUNT,INVOICE_TOTAL,READ_FLAG,CREATION_DATE,INVOICE_PDF_URL)  VALUES(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12)";
                    cmd.ArrayBindCount = 1;
                    cmd.Parameters.Add(ponum);
                    cmd.Parameters.Add(match);
                    cmd.Parameters.Add(invoiceno);
                    cmd.Parameters.Add(invDate);
                    cmd.Parameters.Add(ReceiptNumber);
                    cmd.Parameters.Add(LineLoc);
                    cmd.Parameters.Add(Linedesc);
                    cmd.Parameters.Add(lineAmt);
                    cmd.Parameters.Add(InvTotal);
                    cmd.Parameters.Add(flag);
                    cmd.Parameters.Add(crdate);
                    cmd.Parameters.Add(invUrl);
                    connection.Open();
                    var x = cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetInviceStatus(string str)
        {
            switch (str.Trim())
            {
                case Constants.PODetailInvoiceStatus.Approved:
                    return "Processed";
                case Constants.PODetailInvoiceStatus.Error:
                    return "Error";
                case Constants.PODetailInvoiceStatus.Extracted:
                    return "Extracted";
                case Constants.PODetailInvoiceStatus.PendingForApproval:
                    return "Pending For Approval";
                default:
                    return "Rejected";
            }

        }

        public static void RejectInvoice(long id)
        {
            try
            {
                using (GPModel db = new GPModel())
                {
                    var obj = db.Tbl_AP_PODetail.Where(x => x.InvoiceID == id).FirstOrDefault();
                    obj.InvoiceStatus = Constants.PODetailInvoiceStatus.Rejected;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

        }
        #endregion----------------------------------------------------------------------


        #region-----------------------------login--------------------------------------------
        public static ViewModelClass.LoginModel CheckLogin(string uname, string pwd)
        {
            ViewModelClass.LoginModel Obj = new ViewModelClass.LoginModel();
            try
            {
                var List = GetList();
                Obj = List.Where(x => x.UserName.Equals(uname) && x.Password.Equals(pwd)).FirstOrDefault();
                return Obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static List<ViewModelClass.LoginModel> GetList()
        {
            List<ViewModelClass.LoginModel> ListDt = new List<ViewModelClass.LoginModel>();
            try
            {
                string connString = ConfigurationManager.AppSettings["ERPDB"];
                using (var conn = new OracleConnection(connString))
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "select * from apps.XXGP_RPA_AP_USERS";
                    using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                    {

                        DataTable dt = new DataTable();
                        dap.Fill(dt);
                        foreach (DataRow item in dt.Rows)
                        {
                            ViewModelClass.LoginModel obj = new ViewModelClass.LoginModel()
                            {
                                UserName = Convert.ToString(item["User_Name"]),
                                Password = Convert.ToString(item["User_Password"])
                            };
                            ListDt.Add(obj);
                        }
                    }

                };
                return ListDt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion--------------------------------------------------------------------------

        

        #region----------------------Erp Manager------------------------------------------


        public static List<ViewModelClass.XXGP_RPA_3WAY_PO_VIEW_Model> Get3WayList(string ponumber)
        {
            List<ViewModelClass.XXGP_RPA_3WAY_PO_VIEW_Model> ListDt = new List<ViewModelClass.XXGP_RPA_3WAY_PO_VIEW_Model>();

            try
            {
                string connString = ConfigurationManager.AppSettings["ERPDB"];
                using (var conn = new OracleConnection(connString))
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "select * from apps.XXGP_RPA_3WAY_PO_VIEW";
                    using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                    {

                        DataTable dt = new DataTable();
                        dap.Fill(dt);
                        foreach (DataRow item in dt.Rows)
                        {
                            ViewModelClass.XXGP_RPA_3WAY_PO_VIEW_Model obj = new ViewModelClass.XXGP_RPA_3WAY_PO_VIEW_Model()
                            {
                                PO_Number = Convert.ToString(item["PO_Number"]),
                                Invoice_Number = Convert.ToString(item["Invoice_Number"]),
                                Invoice_Date = Convert.ToString(item["Invoice_Date"]),
                                Receipt_number = Convert.ToString(item["Receipt_number"]),
                                Receipt_total = Convert.ToString(item["Receipt_total"]),
                                GPI_GST_Regn_No = Convert.ToString(item["GPI_GST_Regn_No"]),
                            };
                            ListDt.Add(obj);
                        }
                    }
                    ListDt = ListDt.Where(x => x.PO_Number.Equals(ponumber)).ToList();
                };
            }
            catch (Exception ex)
            {

            }
            return ListDt;
        }

        public static List<ViewModelClass.XXGP_RPA_2WAY_PO_VIEW_Model> Get2WayList(string ponumber)
        {
            List<ViewModelClass.XXGP_RPA_2WAY_PO_VIEW_Model> ListDt = new List<ViewModelClass.XXGP_RPA_2WAY_PO_VIEW_Model>();
            try
            {
                string connString = ConfigurationManager.AppSettings["ERPDB"];
                using (var conn = new OracleConnection(connString))
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "select * from apps.XXGP_RPA_2WAY_PO_VIEW";
                    using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        dap.Fill(dt);
                        foreach (DataRow item in dt.Rows)
                        {
                            ViewModelClass.XXGP_RPA_2WAY_PO_VIEW_Model obj = new ViewModelClass.XXGP_RPA_2WAY_PO_VIEW_Model()
                            {
                                PO_Number = Convert.ToString(item["PO_Number"]),
                                Line_Description = Convert.ToString(item["Line_Description"]),
                                GPI_GST_Regn_No = Convert.ToString(item["GPI_GST_Regn_No"]),
                                Line_location_id = Convert.ToString(item["Line_location_id"]),
                                Line_price = Convert.ToString(item["Line_price"]),
                                Available_Qty = Convert.ToString(item["Available_Qty"]),
                                Available_Amount = Convert.ToString(item["Available_Amount"]),
                                Location_code = Convert.ToString(item["Location_code"]),
                                Tax_Rate = Convert.ToString(item["Tax_Rate"]),

                            };
                            ListDt.Add(obj);
                        }
                    }
                    ListDt = ListDt.Where(x => x.PO_Number.Equals(ponumber)).ToList();
                };
            }
            catch (Exception ex)
            {

            }
            return ListDt;
        }
        #endregion-------------------------------------------------------------------------


        public static string GetInvoicepathById(long id)
        {
            string path = string.Empty;
            using (GPModel db = new GPModel())
            {
                path = db.Tbl_AP_PODetail.Where(x => x.InvoiceID == id).FirstOrDefault().FilePath;
            }
            return path;
        }
    }
}