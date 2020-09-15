using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.App_Code;
using DataAccessLayer.DBModel;
using DataAccessLayer.App_Code.DBManager;
using Common.App_Code.QueueMessage;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using DataAccessLayer.App_Code.ViewModel;
using Common.Constants;
using System.Globalization;
using NLog;
using Common.App_Code.ViewModel;
using ActiveUp.Net.Mail;
using System.Text.RegularExpressions;
using AccountPaybleProcessor.App_Code.PDFProcessor;
using AccountPaybleProcessor.ERPBusinessRule;
using static AccountPaybleProcessor.App_Code.ERPViewModel;
using APLineItemDataLayer.Model;
using APLineItemDataLayer.ApLineItem;

namespace AccountPaybleProcessor.App_Code
{
    class InvoiceProcessor : Processor, IDisposable
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        public string SharedLocationPath { get { return System.Configuration.ConfigurationManager.AppSettings["SharedLocationPath"]; } }
        public string ProcessedInvoicePath { get { return System.Configuration.ConfigurationManager.AppSettings["ProcessedLocationPath"]; } }

        #region Private Variable

        TBL_ProcessInstances tblProcessInstance;
        IGSTDBManager dbManager;
        TBL_ProcessInstanceDetails tblProcessInstanceDetails;
        TBL_ProcessInstanceData tblProcessInstanceData;
        IPDFProcessor pdfProcessor;

        #endregion


        public InvoiceProcessor(TBL_ProcessInstances tblProcessInstance, IGSTDBManager dbManager)
        {
            this.tblProcessInstance = tblProcessInstance;
            this.dbManager = dbManager;
        }

        public void Dispose()
        {

        }

        public override void ExecuteState()
        {
            tblProcessInstanceDetails = this.dbManager.GetProcessInstanceDetail(tblProcessInstance.ProcessInstanceId);
            switch (tblProcessInstanceDetails.StateId)
            {
                case Common.Constants.Process.States.APProcess.InitialState:
                    DownloadInvoices();
                    break;
                
            }
        }

        #region----------------------- State Methods-----------------------------------------------------------------------


        private void DownloadInvoices()
        {
            try
            {
                logger.Info("Mail download started.");
                dbManager.UpdateProcessInstance(tblProcessInstance.ProcessInstanceId);
                dbManager.UpdateProcessInstanceDetailsCreateTS(tblProcessInstance.ProcessInstanceId);
                EMailManager mailProcesor = new EMailManager();
                {
                    string path = SharedLocationPath;
                    ActiveUp.Net.Mail.Mailbox mailBox = mailProcesor.DownloadInvoiceFromMail();
                    ActiveUp.Net.Mail.MessageCollection messageCollection = new ActiveUp.Net.Mail.MessageCollection();

                    for (int n = 1; n < mailBox.MessageCount + 1; n++)
                    {
                       
                        try
                        {
                            ActiveUp.Net.Mail.Message newMessage = mailBox.Fetch.MessageObject((n==1?n:n-1));
                            string subject = newMessage.Subject;                            
                            string body = newMessage.BodyHtml.Text;
                            string FromMail = newMessage.From.Email;                          
                            int movemessage = 0;
                            messageCollection.Add(newMessage);
                            if (newMessage.Attachments.Count != 0)
                            {
                                foreach (MimePart mmc in newMessage.Attachments)
                                {
                                    if (mmc.Filename.ToLower().EndsWith(".pdf"))
                                    {
                                        string fileName = string.Empty;
                                        if (!Directory.Exists(path))
                                        {
                                            Directory.CreateDirectory(path);
                                        }
                                        fileName = Regex.Replace(DateTime.Now.ToString(), "[^0-9]", "") + n + mmc.Filename;
                                        mmc.StoreToFile(path + fileName);
                                        MailDownloadViewModel metaobj = new MailDownloadViewModel() { FromMailId = FromMail, FileName = fileName };
                                        string metadata = JsonConvert.SerializeObject(metaobj);
                                        CreatePDFInstance(Common.Constants.Process.ProcessID.APPDFExtractProcess, Common.Constants.Process.States.APPDFProcess.InitialState, metadata, tblProcessInstance);
                                       
                                        movemessage = 1;

                                    }
                                    else
                                    {                                     
                                        movemessage = 0;
                                    }
                                }
                            }
                            else
                            {
                                movemessage = 0;
                            }
                            if (movemessage == 1)
                            {
                                mailBox.MoveMessage((n == 1 ? n : n - 1), "Processed");
                            }
                            else
                            {
                               mailBox.MoveMessage((n == 1 ? n : n - 1), "Processed");
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Info("Mail download failed.");
                           // throw;
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                //dbManager.NextInstance(tblProcessInstance, tblProcessInstanceData, tblProcessInstanceDetails, "", Common.Constants.Process.States.APProcess.InvoiceDownloadFailed, tblProcessInstance.ProcessInstanceId, ex.Message);
            }
        }


        private void CreatePDFInstance(string processid, string state, string metadata, TBL_ProcessInstances processInstances)
        {
            dbManager.CreateApPdfInstance(processid, state, metadata, processInstances);
        }
        #endregion------------------------------------------------------------------------------------------------------------------
    }
}
