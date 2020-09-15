using Common.App_Code;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GetMailCredentialProcess.VieweModelClass;

namespace GetMailCredentialProcess
{
    class Program
    {
        static void Main(string[] args)
        {

            UpdateErpTable obj = new UpdateErpTable();
            obj.Update_XXGP_PO_INVOICE_AUTO_T();
            //get_XXGP_RPA_AP_MAILBOX_ID();
            // SendEmail();
        }
       public static void get_XXGP_RPA_AP_MAILBOX_ID()
        {
            try
            {
                string connString = ConfigurationManager.AppSettings["ERPDB"];
                string path = ConfigurationManager.AppSettings["ApCredentialPath"];
                using (var conn = new OracleConnection(connString))
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "select * from apps.XXGP_RPA_AP_MAILBOX_ID";
                    using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                    {
                        List<GET_GP_MailIDcls> ListDt = new List<GET_GP_MailIDcls>();
                        DataTable dt = new DataTable();
                        dap.Fill(dt);
                        foreach (DataRow item in dt.Rows)
                        {
                            GET_GP_MailIDcls obj = new GET_GP_MailIDcls()
                            {
                                AP_MAIL_BOX_ID = Convert.ToString(item["AP_MAIL_BOX_ID"]),
                                MAIL_PASSWORD = Convert.ToString(item["MAIL_PASSWORD"])
                            };
                            ListDt.Add(obj);
                        }                     

                        System.IO.File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(ListDt));
                    }

                };
            }
            catch (Exception ex)
            {
               
            }



        }


        public static void SendEmail()
        {
            EMailManager Email = new EMailManager();
            Email.SendMail("", "sushil.beura@gridinfocom.com", "TEst", "ksddsd");
        }
    }
}
