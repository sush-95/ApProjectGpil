using DataAccessLayer.App_Code.DBManager;
using DataAccessLayer.DBModel;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AccountPaybleProcessor.App_Code.PDFProcessor
{
    class IMPRESSION_SSERVICES_PRIVATE_LIMITED: IPDFProcessor
    {

        IGSTDBManager dbManager;
        TBL_ProcessInstances tblProcessInstance;
        TBL_ProcessInstanceDetails tblProcessInstanceDetails;
        TBL_ProcessInstanceData tblProcessInstanceData;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public IMPRESSION_SSERVICES_PRIVATE_LIMITED(IGSTDBManager dbManager, TBL_ProcessInstances tblProcessInstance)
        {
            this.dbManager = dbManager;
            this.tblProcessInstance = tblProcessInstance;
        }
        

        public void ExtractPDFData(ViewPdfModel pdfViewModel, string filePath, string ProcessedInvoicePath)
        {

            this.tblProcessInstanceDetails = dbManager.GetProcessInstanceDetail(tblProcessInstance.ProcessInstanceId);
            this.tblProcessInstanceData = dbManager.GetProcessInstanceData(tblProcessInstance.ProcessInstanceId);
            string newFilePath = string.Empty;
            String[] rate = new String[] { };

            ViewPdfModel pdfObject = new ViewPdfModel();
            string strProcess = string.Empty;
            string refInvoiceNo = string.Empty;
            string refInvoiceDate = string.Empty;

            try
            {
                long firstProcessInstanceId = tblProcessInstanceData.ProcessInstanceId;
                var pdf = pdfViewModel.PdfText;
                Regex pattern = new Regex(@"\d{2}[A-Z]{5}\d{4}[A-Z]{1}[A-Z\d]{1}[Z]{1}[A-Z\d]{1}");
                List<string> gststrs = pdf.Where(x => x.Contains("GSTIN")).ToList();
                string gstvendor = gststrs.Where(x => x.Trim().IndexOf("GSTIN") == 0).FirstOrDefault();
                gststrs = gststrs.Where(x => x.Trim().IndexOf("GSTIN") != 0).ToList();
                string gstgpi = gststrs.Where(x => pattern.Match(x).Groups[0].Value.Trim() != "").First();
                pdfObject.VendorGst = pattern.Match(gstvendor).Groups[0].Value;
                pdfObject.GPIGST = pattern.Match(gstgpi).Groups[0].Value;
                pdfObject.VendorName = "IMPRESSIONS SERVICES PRIVATE LIMITED";
                pattern = new Regex(@"[0-9]+\-[0-9]+\-\d{4}");
                int invoiceDateIndex = pdf.FindIndex(x => x.ToUpper().Contains("Invoice Date".ToUpper()));
                pdfObject.InvoiceDate = pdf[invoiceDateIndex] + pdf[invoiceDateIndex + 1] + pdf[invoiceDateIndex + 2];
                pdfObject.InvoiceDate = pattern.Match(pdfObject.InvoiceDate).Groups[0].Value;
                pattern = new Regex(@"[0-9]+\/\d+\-\d+\/\d+");
                int InvoiceNumberindex = pdf.FindIndex(x => x.Contains("Invoice No"));
                pdfObject.InvoiceNumber = pdf[InvoiceNumberindex] + pdf[InvoiceNumberindex + 1] + pdf[InvoiceNumberindex + 2];
                pdfObject.InvoiceNumber = pattern.Match(pdfObject.InvoiceNumber).Groups[0].Value;
                int grossbillvalue = pdf.FindIndex(x => x.ToUpper().Contains("TotalAmount".ToUpper().Trim()));
                pdfObject.InvoiceAmount = (pdf[grossbillvalue] + pdf[grossbillvalue + 1]).Trim().Replace(",", "");
                pattern = new Regex(@"\d+(\.\d+)");
                pdfObject.InvoiceAmount = pattern.Match(pdfObject.InvoiceAmount).Groups[0].Value;
                pdfObject.PdfContent = pdfViewModel.PdfContent;
                InvoiceNumberindex = pdf.FindIndex(x => x.Contains("P.O."));
                pattern = new Regex(@"[0-9]{9,11}");
                pdfObject.PONumber = pdf[InvoiceNumberindex] + pdf[InvoiceNumberindex + 1];
                pdfObject.PONumber = pattern.Match(pdfObject.PONumber).Groups[0].Value;

                pdfObject.ListItem = ListofGoods(pdfObject);
                pdfObject.PdfContent = string.Empty;

                string newFolderPath = ProcessedInvoicePath + pdfObject.GPIGST + "\\\\" + DateTime.Now.ToString("MMM-yyyy") + "\\\\" + pdfObject.VendorName + "\\\\";
                newFilePath = newFolderPath + pdfObject.InvoiceNumber + ".pdf";
                newFilePath = newFilePath.Replace("/", "");
                newFolderPath = newFolderPath.Replace("/", "");
                pdfObject.NewInvoicepath = newFilePath;
                tblProcessInstanceData.ChildInstanceId = firstProcessInstanceId;
                dbManager.UpdateProcessInstanceData(tblProcessInstanceData);
                string pdfData = GetMetadata(pdfObject);
                dbManager.NextInstance(tblProcessInstance, tblProcessInstanceData, tblProcessInstanceDetails, pdfData, Common.Constants.Process.States.APPDFProcess.DataExtracted, firstProcessInstanceId);
                PDFManager.MovePdfToProcessedFolder(newFilePath, newFolderPath, filePath);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static List<ItemList> ListofGoods(ViewPdfModel model)
        {
            string pdfcontent = model.PdfContent;
            List<ItemList> ListObj = new List<ItemList>();
            ItemList item = new ItemList();
            ViewPdfModel pdfmodel = new ViewPdfModel();
            List<int> indexarr = new List<int>();
            List<string> discription = new List<string>();
            string description_str = pdfcontent.Substring(pdfcontent.IndexOf("Description"), pdfcontent.IndexOf("TOTAL") - pdfcontent.IndexOf("Description"));
            description_str = description_str.Replace(",", "");//To Remove ','s from Amount field
            List<string> textpdf = new List<string>(description_str.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            Regex pattern = new Regex(@"\d+(\.\d+)");
            textpdf = textpdf.Where(x => pattern.Match(x).Groups[0].Value.Trim() != "").ToList();
            for (int i = 0; i < textpdf.Count; i++)
            {
                item = new ItemList();
                item.PONumber = model.PONumber;
                item.InvoiceNOandDate = model.InvoiceNumber + Environment.NewLine + model.InvoiceDate;
                item.Quantity = "1";
                item.LineNumber = (i + 1).ToString();
                pattern = new Regex(@"\d+(\.\d+)");
                item.per_Amount = pattern.Match(textpdf[i]).Groups[0].Value;
                pattern = new Regex(@"\d{6}");
                item.HSN_SAC = pattern.Match(textpdf[i]).Groups[0].Value;
                item.Description_of_Goods = textpdf[i].Replace(item.HSN_SAC, "").Replace(item.per_Amount, "");
                ListObj.Add(item);
            }

            return ListObj;
        }
        public string GetMetadata(ViewPdfModel model)
        {
            JsonViewModel jmodelObj = new JsonViewModel();
            jmodelObj.Header = new Header()
            {
                CreatedTs = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss"),
                ProcessInstanceId = tblProcessInstance.ProcessInstanceId.ToString(),
                ProcessName = "APPdfExctract",
                StateId = tblProcessInstanceDetails.StateId
            };
            jmodelObj.Status = new Status()
            {
                Value = Common.Constants.Process.States.APInvoiceStatus.PdfExctracted,
                Description = "Pdf Value Extracted Successfully."
            };
            jmodelObj.Detail = model;
            return JsonConvert.SerializeObject(jmodelObj);
        }

    }


}
