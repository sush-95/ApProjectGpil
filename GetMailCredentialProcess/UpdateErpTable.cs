using GetMailCredentialProcess.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.App_Code.ViewModelClass;

namespace GetMailCredentialProcess
{
    public class UpdateErpTable
    {
        public void Update_XXGP_PO_INVOICE_AUTO_T()
        {
            List<Tbl_AP_PODetail> poitem = new List<Tbl_AP_PODetail>();
            var LineItem = new List<Tbl_AP_LineItemDetail>();
            using (GPIL db = new GPIL())
            {
                poitem = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals("AP-POSTAT-010")).ToList();
            }
            if (poitem.Count > 0)
            {
              
                foreach (var item in poitem)
                {
                    GPIL db = new GPIL();
                    LineItem = db.Tbl_AP_LineItemDetail.Where(x => x.PONumber.Equals(item.PONumber) && x.InvoiceNoDate.Contains(item.InvoiceNo)).ToList();
                  if(LineItem[0].MatchOption.ToLower().Equals("2way"))
                    {
                        foreach(var lItem in LineItem)
                        {
                            XXGP_PO_INVOICE_AUTO_TModel tblobj = new XXGP_PO_INVOICE_AUTO_TModel();
                            tblobj.PO_Number = item.PONumber;
                            tblobj.PO_MATCH_OPTION = lItem.MatchOption;
                            tblobj.INVOICE_NUMBER = item.InvoiceNo;
                            tblobj.INVOICE_DATE = item.InvoiceDate.ToString();
                            tblobj.RECEIPT_NUMBER = " ";
                            tblobj.LINE_LOCATION_ID = lItem.LineLocationID;
                            tblobj.LINE_AMOUNT = lItem.Amount;
                            SaveUsingOracleBulkCopy(tblobj);
                        }
                       
                       
                    }
                   

                   
                    
                }

            }
        }

        public void SaveUsingOracleBulkCopy(XXGP_PO_INVOICE_AUTO_TModel tblobj)
        {
            try
            {
                string connString = ConfigurationManager.AppSettings["ERPDB"];

                using (var connection = new OracleConnection(connString))
                {
                    connection.Open();
                    string[] PO_NUMBER = new string[1];
                    string[] PO_MATCH_OPTION = new string[1];
                    string[] INVOICE_NUMBER = new string[1];
                    DateTime[] INVOICE_DATE = new DateTime[1];
                    string[] RECEIPT_NUMBER = new string[1];
                    int[] LINE_LOCATION_ID = new int[1];
                    string[] LINE_DESCRIPTION = new string[1];
                    int[] LINE_AMOUNT = new int[1];
                    int[] INVOICE_TOTAL = new int[1];
                    string[] READ_FLAG = new string[1];
                    DateTime[] CREATION_DATE = new DateTime[1];

                    PO_NUMBER[0] = tblobj.PO_Number;
                    PO_MATCH_OPTION[0] = tblobj.PO_MATCH_OPTION;
                    INVOICE_NUMBER[0] = tblobj.INVOICE_NUMBER;
                    INVOICE_DATE[0] = Convert.ToDateTime(tblobj.INVOICE_DATE);
                    RECEIPT_NUMBER[0] = tblobj.RECEIPT_NUMBER;
                    LINE_LOCATION_ID[0] = Convert.ToInt32(tblobj.LINE_LOCATION_ID);
                    LINE_DESCRIPTION[0] = " ";
                    LINE_AMOUNT[0] = Convert.ToInt32(Convert.ToDecimal(tblobj.LINE_AMOUNT));
                    INVOICE_TOTAL[0] = 0;
                    READ_FLAG[0] ="N";
                    CREATION_DATE[0] = DateTime.Today;



                    OracleParameter ponum = new OracleParameter();
                    ponum.OracleDbType = OracleDbType.Varchar2;
                    ponum.Value =PO_NUMBER;

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
                    LineLoc.OracleDbType = OracleDbType.Int32;
                    LineLoc.Value = LINE_LOCATION_ID;

                    OracleParameter Linedesc = new OracleParameter();
                    Linedesc.OracleDbType = OracleDbType.Varchar2;
                    Linedesc.Value = LINE_DESCRIPTION;

                    OracleParameter lineAmt = new OracleParameter();
                    lineAmt.OracleDbType = OracleDbType.Int32;
                    lineAmt.Value = LINE_AMOUNT;

                    OracleParameter InvTotal = new OracleParameter();
                    InvTotal.OracleDbType = OracleDbType.Int32;
                    InvTotal.Value = INVOICE_TOTAL;

                    OracleParameter flag = new OracleParameter();
                    flag.OracleDbType = OracleDbType.Varchar2;
                    flag.Value = READ_FLAG;

                    OracleParameter crdate = new OracleParameter();
                    crdate.OracleDbType = OracleDbType.Date;
                    crdate.Value = CREATION_DATE;

                    // create command and set properties
                    
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO apps.XXGP_PO_INVOICE_AUTO_STG(PO_NUMBER,PO_MATCH_OPTION,INVOICE_NUMBER,INVOICE_DATE,RECEIPT_NUMBER,LINE_LOCATION_ID,LINE_DESCRIPTION,LINE_AMOUNT,INVOICE_TOTAL,READ_FLAG,CREATION_DATE)  VALUES(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11)";
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
                    var x = cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
