using AccountPaybleProcessor.App_Code.PDFProcessor;
using AccountPaybleProcessor.ERPBusinessRule;
using APLineItemDataLayer.ApLineItem;
using APLineItemDataLayer.Model;
using Common.App_Code;
using DataAccessLayer.App_Code.DBManager;
using DataAccessLayer.DBModel;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AccountPaybleProcessor.App_Code.ERPViewModel;

namespace AccountPaybleProcessor.App_Code
{
    class PDFExctractProcessor : Processor, IDisposable
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

        public PDFExctractProcessor(TBL_ProcessInstances tblProcessInstance, IGSTDBManager dbManager)
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
                case Common.Constants.Process.States.APPDFProcess.InitialState:
                    ExtractPDF();
                    break;
                case Common.Constants.Process.States.APPDFProcess.DataExtracted:
                    ImplementBusinessRules();
                    break;
            }
        }


        private void ExtractPDF()
        {
            string filePath = string.Empty, Sender = string.Empty;
            try
            {
                string processpath = ProcessedInvoicePath;
                logger.Info("ExtractPDF Started");
                tblProcessInstanceDetails = this.dbManager.GetProcessInstanceDetailByState(tblProcessInstance.ProcessInstanceId, Common.Constants.Process.States.APPDFProcess.InitialState);
                tblProcessInstanceData = this.dbManager.GetProcessInstanceDataBySequence(tblProcessInstance.ProcessInstanceId, 1);
                if (!string.IsNullOrWhiteSpace(tblProcessInstanceData.MetaData) && tblProcessInstanceData.MetaData.ToLower().Contains("filename"))
                {
                    string pdfPath = JsonConvert.DeserializeObject<MailDownloadViewModel>(tblProcessInstanceData.MetaData).FileName;
                    Sender = JsonConvert.DeserializeObject<MailDownloadViewModel>(tblProcessInstanceData.MetaData).FromMailId;
                    filePath = SharedLocationPath + pdfPath;
                    ViewPdfModel pdfViewModel = PDFManager.GetPDFText(filePath);
                    string newFilePath = string.Empty;
                    pdfProcessor = PDFManager.CreateClientInvoiceObject(pdfViewModel, dbManager, tblProcessInstance);
                    pdfProcessor.ExtractPDFData(pdfViewModel, filePath, processpath);

                }
                logger.Info("ExtractPDF Ended.");
            }
            catch (Exception ex)
            {
                logger.Info("ExtractPDF Failed.");
                SendMail(filePath,Sender, "Unable to read pdf.", "AP:Invalid Pdf");
                dbManager.NextInstance(tblProcessInstance, tblProcessInstanceData, tblProcessInstanceDetails, "", Common.Constants.Process.States.APPDFProcess.DataExtractionFailed, tblProcessInstance.ProcessInstanceId, ex.Message);
                dbManager.CompleteProcess(tblProcessInstance.ProcessInstanceId, Common.Constants.Process.States.APPDFProcess.Complete);
            }
        }
        private void ImplementBusinessRules()
        {
            try
            {
                string ErrorMesssage = string.Empty;
                logger.Info("ImplementBusinessRules Started");
                tblProcessInstanceDetails = this.dbManager.GetProcessInstanceDetailByState(tblProcessInstance.ProcessInstanceId, Common.Constants.Process.States.APPDFProcess.DataExtracted);
                tblProcessInstanceData = this.dbManager.GetProcessInstanceDataBySequence(tblProcessInstance.ProcessInstanceId, tblProcessInstanceDetails.SequenceId);
                JsonViewModel JsonMetaModel = JsonConvert.DeserializeObject<JsonViewModel>(tblProcessInstanceData.MetaData);
                ViewPdfModel pdfViewModel = JsonMetaModel.Detail;
                ERPDBHelper ERpHelper = new ERPDBHelper();
                if (CheckBusinessData(pdfViewModel, ERpHelper, ref ErrorMesssage))
                {
                    UpdateInvoiceTable(JsonMetaModel);
                }
                else
                {
                    tblProcessInstanceData = this.dbManager.GetProcessInstanceDataBySequence(tblProcessInstance.ProcessInstanceId, 1);
                    var metaobj = JsonConvert.DeserializeObject<MailDownloadViewModel>(tblProcessInstanceData.MetaData);
                    string msg = ErrorMesssage + Environment.NewLine + "PFA.";
                    SendMail(SharedLocationPath + metaobj.FileName, metaobj.FromMailId, msg, "AP:Invoice status");
                    dbManager.NextInstance(tblProcessInstance, tblProcessInstanceData, tblProcessInstanceDetails, "", Common.Constants.Process.States.APPDFProcess.BusinessRuleFailed, tblProcessInstance.ProcessInstanceId, msg);

                }
                dbManager.CompleteProcess(tblProcessInstance.ProcessInstanceId, Common.Constants.Process.States.APPDFProcess.Complete);


                logger.Info("ImplementBusinessRules Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dbManager.NextInstance(tblProcessInstance, tblProcessInstanceData, tblProcessInstanceDetails, "", Common.Constants.Process.States.APPDFProcess.BusinessRuleException, tblProcessInstance.ProcessInstanceId, ex.Message);

            }
        }
        public void ManagePDFExtractionError()
        {
            try
            {
                logger.Info("ManagePDFExtractionError Started");
                tblProcessInstance = dbManager.GetProcessInstanceByChildId(tblProcessInstance.ProcessInstanceId);
                //string errorMessage = "{\"" + Common.Constants.JSON.Tags.Message.Details.Key + "\":" +"Invalid PDF Format"+ "}";
                string errorMessage = "{\"Detail\":{\"" + Common.Constants.JSON.Tags.Message.Details.PDF + "\":\"" + Common.Constants.JSON.Tags.Values.Action.InvalidPDF + "\"}}";
                NextInstance(string.Empty, Common.Constants.Process.States.InvoiceBillProcess.DataExtractionFailed, false, errorMessage);
                // logger.Info("ManagePDFExtractionError Completed");
            }
            catch (Exception ex)
            {
                // logger.Error(ex);
            }
        }
        private void NextInstance(string metaData, string nextStateId, bool isComplete = false, string errorMessage = "")
        {
            tblProcessInstanceDetails = dbManager.GetProcessInstanceDetail(tblProcessInstance.ProcessInstanceId);
            tblProcessInstanceData = dbManager.GetProcessInstanceData(tblProcessInstance.ProcessInstanceId);

            int sequenceId = tblProcessInstanceDetails.SequenceId;
            dbManager.AddProcessInstanceDetails(tblProcessInstance.ProcessInstanceId, sequenceId, nextStateId, isComplete, DateTime.Now);

            TBL_ProcessInstanceData tblprocessData = new TBL_ProcessInstanceData();
            tblprocessData.ProcessInstanceId = tblProcessInstance.ProcessInstanceId;
            tblprocessData.MetaData = metaData;
            tblprocessData.SequenceId = sequenceId + 1;
            tblprocessData.CreatedTS = DateTime.Now;
            tblprocessData.MetaDataSequenceId = tblProcessInstanceData.MetaDataSequenceId + 1;
            tblprocessData.ErrorMessage = errorMessage;

            dbManager.AddProcessInstanceData(tblprocessData);
        }
        private bool CheckBusinessData(ViewPdfModel pdfViewModel, ERPDBHelper DbObj, ref string ErrorMessage)
        {
            bool flag = false;
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                //GetDate(pdfViewModel);
                if (!string.IsNullOrEmpty(pdfViewModel.PONumber))
                {
                    var polist = DbObj.Get_XXGP_RPA_PO_VIEW_Map(pdfViewModel.PONumber.Trim());
                    if (polist.Count == 0)
                    {
                        dict.Add("PO Number", "N/A");
                        ErrorMessage = "PO Number is not correct.Please send the invoice again with correct PO Number.";
                    }
                    else if (!polist[0].PO_Status.ToUpper().Trim().Equals("APPROVED"))
                    {
                        dict.Add("PO Number", "N/A");
                        ErrorMessage = "PO Number status is "+ polist[0].PO_Status + ".Only approved POs invoices are accepted.Get the PO approved";
                    }
                    else
                    {
                        DateTime dt = GetDateVendorWise(pdfViewModel.InvoiceDate.Trim(), pdfViewModel.VendorName.Trim());
                        if (!LineItemManager.CheckPOAvailable(pdfViewModel.PONumber.Trim(), pdfViewModel.InvoiceNumber.Trim(), dt))
                        {
                            dict.Add("PO Number", "Exist");
                            ErrorMessage = "This invoice is already received by us.";
                        }
                    }

                }
                else
                {
                    dict.Add("PO Number", "N/A");
                    ErrorMessage = "Po number missing in pdf.Please send the invoice again with PO Number.";
                }
                if (dict.Count > 0)
                {

                }
                else
                {
                    flag = true;
                }
                logger.Info("ValidateBusinessRules Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return flag;
        }


        private void UpdateInvoiceTable(JsonViewModel JsonMetaModel)
        {
            ViewPdfModel pdfobj = JsonMetaModel.Detail;
            Tbl_AP_PODetail podetail = new Tbl_AP_PODetail()
            {
                FilePath = pdfobj.NewInvoicepath,
                VendorGstNO = pdfobj.VendorGst,
                InvoiceDate = GetDateVendorWise(pdfobj.InvoiceDate.Trim(), pdfobj.VendorName.Trim()),
                InvoiceNo = pdfobj.InvoiceNumber,
                PONumber = pdfobj.PONumber,
                InvoiceAmount = pdfobj.InvoiceAmount,
                InvoiceStatus = Common.Constants.PODetailInvoiceStatus.Extracted,
                VendorName = pdfobj.VendorName,
                GPIGstNO = pdfobj.GPIGST,
                MetaData = JsonConvert.SerializeObject(JsonMetaModel),
                CreatedDate = DateTime.Now

            };
            long InvoiceID = LineItemManager.SaveInvoice(podetail);
            List<Tbl_AP_LineItemDetail> LineList = new List<Tbl_AP_LineItemDetail>();
            foreach (var item in pdfobj.ListItem)
            {
                LineList.Add(
                    new Tbl_AP_LineItemDetail()
                    {
                        InvoiceNoDate = item.InvoiceNOandDate,
                        PONumber = item.PONumber,
                        LineId = Convert.ToInt32(item.LineNumber),
                        InvoiceDescription = item.Description_of_Goods,
                        HSN = item.HSN_SAC,
                        ItemQuantity = item.Quantity,
                        Rate = item.Rate,
                        Amount = item.per_Amount,
                        InvoiceID = InvoiceID
                    });
            }
            LineItemManager.SaveLineIetm(LineList);

        }

        private void SendMail(string attachment, string tomail, string body, string sub)
        {
            EMailManager Mail = new EMailManager();
            Mail.SendMail(attachment, tomail, sub, body);
        }


        static DateTime GetDateVendorWise(string date, string venderName)
        {
            DateTime dt = new DateTime();
            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(venderName))
                return dt;


            switch (venderName)
            {
                case "M/S ASTHA PACKAGING PRIVATE LIMITED":
                case "Kuldip Enterprises India Limited":
                case "NIYOGI OFFSET PVT LTD.":
                case "RAJPUTANA SECURITY SERVICES":
                case "SANGAT PRINTERS PVT. LTD.":
                case "Kolor Catalyst Designs Pvt Ltd.":
                    if (date.Count() == 10)
                        dt = DateTime.ParseExact(date, "d-MMM-yyyy", null);
                    else
                        dt = DateTime.ParseExact(date, "dd-MMM-yyyy", null);
                    break;
                case "HITACHI SYSTEMS MICRO CLINIC PVT.LTD.":
                case "MAXIMUS PACKERS":
                    if (date.Count() == 9)
                        dt = DateTime.ParseExact(date, "d-MM-yyyy", null);
                    else
                        dt = DateTime.ParseExact(date, "dd-MM-yyyy", null);
                    break;
                case "GDX FACILITY & MANAGEMENT SERVICES PVT LTD.":
                    if (date.Count() == 9)
                        dt = DateTime.ParseExact(date, "m/dd/yyyy", null);
                    else
                        dt = DateTime.ParseExact(date, "mm/dd/yyyy", null);
                    break;
                case "Sify Technologies Limited":
                    if (date.Count() == 8)
                        dt = DateTime.ParseExact(date, "d-MMM-yy", null);
                    else
                        dt = DateTime.ParseExact(date, "dd-MMM-yy", null);
                    break;
                default:
                    dt = Convert.ToDateTime(date);
                    break;

            }
            //Console.WriteLine(dt.ToString("dd-MMMM-yyyy"));
            return dt;

        }
    }
}
