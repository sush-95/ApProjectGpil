using NLog;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DBModel;
using DataAccessLayer.App_Code.DBManager;
using System.IO;
using Common.App_Code.ViewModel;

namespace AccountPaybleProcessor.App_Code.PDFProcessor
{
    public class PDFManager
    {//
        public static IPDFProcessor CreateClientInvoiceObject(ViewPdfModel pdfViewModel, IGSTDBManager dbManager, TBL_ProcessInstances tblProcessInstance)
       {
            if (pdfViewModel.PdfContent.Contains("M/S ASTHA PACKAGING PRIVATE LIMITED") || pdfViewModel.PdfContent.Contains("M/S ASTHA PACKAGING PRIVATE LIMITED"))
            {
                return new AsthaPkgPvt(dbManager, tblProcessInstance);
            }
            else if (pdfViewModel.PdfContent.Contains("GDX"))
            {
                return new GDX_FACILITY_AND_MANAGEMENT_SERVICES(dbManager, tblProcessInstance);
            }
            else if (pdfViewModel.PdfContent.Contains("HITACHI SYSTEMS MICRO CLINIC PVT.LTD."))
            {
                return new HITACHI_SYSTEMS_MICRO_CLINIC_PVT_LTD(dbManager, tblProcessInstance);
            }
            else if (pdfViewModel.PdfContent.Contains("Kolor Catalyst Designs Pvt. Ltd."))
            {
                return new KOLOR_CATALYST_DESIGNS_PVT_LTD(dbManager, tblProcessInstance);
            }
            else if (pdfViewModel.PdfContent.Contains("MAXIMUS PACKERS"))
            {
                return new MAXIMUS_PACKERS(dbManager, tblProcessInstance);
            }
            else if (pdfViewModel.PdfContent.Contains("RAJPUTANA SECURITY SERVICES"))
            {
                return new RAJPUTANA_SECURITY_SERVICES(dbManager, tblProcessInstance);
            }
            else if (pdfViewModel.PdfContent.Contains("Kuldip Enterprises India Limited"))
            {
                return new KULDIP_ENTERPRISES_INDIA_LTD(dbManager, tblProcessInstance);
            }
            else if (pdfViewModel.PdfContent.Contains("Sify Technologies Limited"))
            {
                return new SIFY_TECHNOLOGIES_LTD(dbManager, tblProcessInstance);
            }
            else if (pdfViewModel.PdfContent.Contains("NIYOGI OFFSET PVT LTD."))
            {
                return new NIYOGI_OFFSET_PVT_LTD(dbManager, tblProcessInstance);
            }
            else if (pdfViewModel.PdfContent.Contains("SANGAT PRINTERS PVT"))
            {
                return new Sangat_EnterPrise(dbManager, tblProcessInstance);
            }
            else if (pdfViewModel.PdfContent.Contains("IMPRESSIONSSERVICESPRIVATELIMITED"))
            {
                return new IMPRESSION_SSERVICES_PRIVATE_LIMITED(dbManager, tblProcessInstance);
            }
            else
            {
                throw new Exception("Vendor Not Implemented");
            }
        }

        public static ViewPdfModel GetPDFText(string path)
        {
            try
            {
                PdfReader reader = new PdfReader(path);
                StringBuilder text = new StringBuilder();

                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, page).Trim());
                }
                reader.Close();

                List<string> textpdf = new List<string>(text.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));

                ViewPdfModel pdfObject = new ViewPdfModel();
                pdfObject.PdfText = textpdf.Select(x => x.Replace("\t", "")).ToList();
                pdfObject.PdfContent = text.ToString();
                return pdfObject;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static long  CreateInvoiceInstance(string fileName, long childInstanceId, IGSTDBManager dbManager,TBL_ProcessInstances tblProcessInstance,TBL_ProcessInstanceDetails tblProcessInstanceDetails)
        {
            try
            {
                long processInstanceId = dbManager.AddProcessInstance(Common.Constants.Process.ProcessID.InvoiceBillProcess, tblProcessInstance.ParentProcessInstanceId);
                tblProcessInstance = dbManager.GetProcessInstanceByInstanceId(processInstanceId);
                dbManager.AddProcessInstanceDetails(processInstanceId, 0, Common.Constants.Process.States.InvoiceBillProcess.InitialState, false, DateTime.Now);

                TBL_ProcessInstanceData lastProcessData = dbManager.GetProcessInstanceData(processInstanceId);

                string metaData = "{\"" + Common.Constants.JSON.Tags.Message.Details.Key + "\":{\"" + Common.Constants.JSON.Tags.Message.Details.InvoicePath + "\":\"" + fileName + "\"}}";
                tblProcessInstanceDetails = dbManager.GetProcessInstanceDetailByState(processInstanceId, Common.Constants.Process.States.InvoiceBillProcess.InitialState);

                TBL_ProcessInstanceData processData = new TBL_ProcessInstanceData();
                processData.ProcessInstanceId = processInstanceId;
                processData.SequenceId = tblProcessInstanceDetails.SequenceId;
                processData.MetaDataSequenceId = lastProcessData == null ? 0 : lastProcessData.MetaDataSequenceId + 1; ;
                processData.MetaData = metaData;
                processData.CreatedTS = DateTime.Now;
                processData.IsProcessed = false;
                // processData.IsFinal = false;
                processData.ChildInstanceId = childInstanceId;
                dbManager.AddProcessInstanceData(processData);
                return processInstanceId;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

     
        public static void MovePdfToProcessedFolder(string newFilePath, string newFolderPath, string oldPath)
        {
         
            string fileName = string.Empty;
            if (!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
            }
            if (!System.IO.File.Exists(newFilePath))
            {
                File.Copy(oldPath, newFilePath);
            }
        }
    }
}
