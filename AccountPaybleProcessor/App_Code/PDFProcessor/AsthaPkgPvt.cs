using System;
using NLog;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using DataAccessLayer.DBModel;
using Common.App_Code.ViewModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DataAccessLayer.App_Code.DBManager;
using AccountPaybleProcessor.App_Code;

namespace AccountPaybleProcessor.App_Code.PDFProcessor
{
    public class AsthaPkgPvt : IPDFProcessor
    {
        IGSTDBManager dbManager;
        TBL_ProcessInstances tblProcessInstance;
        TBL_ProcessInstanceDetails tblProcessInstanceDetails;
        TBL_ProcessInstanceData tblProcessInstanceData;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public AsthaPkgPvt(IGSTDBManager dbManager, TBL_ProcessInstances tblProcessInstance)
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
                logger.Info("ExtractPDFData Started");
                long firstProcessInstanceId = tblProcessInstanceData.ProcessInstanceId;
                var pdf = pdfViewModel.PdfText;
                pdfObject.VendorGst = pdf.Where(x => x.Contains("GSTIN")).FirstOrDefault().Replace(" ", "").Replace("GSTIN/UIN:", "");
                pdfObject.VendorName = pdf.Where(x => x.ToUpper().Contains("ASTHA")).FirstOrDefault();
                int VnameIndex = pdf.FindIndex(x => x.Contains(pdfObject.VendorName));

                Regex pattern = new Regex(@"(([0-9])|([0-2][0-9])|([3][0-1]))\-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\-\d{4}");
                pdfObject.InvoiceDate = pattern.Match(pdfViewModel.PdfContent).Groups[0].Value;

                pattern = new Regex(@"\w+\/([0-9][0-9]+\-[0-9][0-9]+\/\d{4})");
                pdfObject.InvoiceNumber = pattern.Match(pdfViewModel.PdfContent).Groups[0].Value;
                int poindex = pdf.FindIndex(x => x.Contains("Order No"));
                pattern = new Regex(@"[0-9]{9,11}");
                pdfObject.PONumber = pdf[poindex - 1] + pdf[poindex + 1] + pdf[poindex + 2] + pdf[poindex + 3] + pdf[poindex + 4];
                pdfObject.PONumber = pattern.Match(pdfObject.PONumber).Groups[0].Value;


                pdfObject.GPIGST = pdf.Where(x => x.Contains("GSTIN")).ToList()[1].Replace(" ", "").Replace("GSTIN/UIN:", "");
                pdfObject.EwayBillNO = pdf[VnameIndex + 3].Replace(pdfObject.InvoiceNumber, "").Trim();

                pdfObject.InvoiceAmount = pdf[pdf.FindIndex(x => x.Contains("Amount Chargeable")) - 1].Trim();
                pdfObject.PdfContent = pdfViewModel.PdfContent;
                pdfObject.ListItem = ListofGoods(pdfObject);
                pdfObject.PdfContent = string.Empty; 

                pdfObject.VendorName = "M/S ASTHA PACKAGING PRIVATE LIMITED";
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
                logger.Info("ExtractPDFData Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
        public string GetMetadata(ViewPdfModel model)
        {
            JsonViewModel jmodelObj = new JsonViewModel();
            jmodelObj.Header = new Header()
            {
                CreatedTs = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss"),
                ProcessInstanceId = tblProcessInstance.ProcessInstanceId.ToString(),
                ProcessName = "APProcess",
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

        public static List<ItemList> ListofGoods(ViewPdfModel model)
        {
            string pdfcontent = model.PdfContent;
            List<ItemList> ListObj = new List<ItemList>();
            ItemList item = new ItemList();
            ViewPdfModel pdfmodel = new ViewPdfModel();
            List<int> indexarr = new List<int>();
            List<string> discription = new List<string>();
            string description_str = pdfcontent.Substring(pdfcontent.IndexOf("Description of Goods"), pdfcontent.IndexOf("This is a Computer Generated Invoice") - pdfcontent.IndexOf("Description of Goods"));

            List<string> textpdf = new List<string>(description_str.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            //string Nos = textpdf.Where(x => x.Contains("NOS")).LastOrDefault();
            foreach (var lineintextpdf in textpdf)
            {
                if (lineintextpdf.Contains("PCS."))
                {

                    if (!(lineintextpdf.Split(new[] { "PCS." }, StringSplitOptions.None)[1].Trim().Equals("")))
                    {
                        discription.Add(lineintextpdf);
                        indexarr.Add(textpdf.FindIndex(x => x.Contains(lineintextpdf)));
                    }
                }
            }
            int slno = 1;
            foreach (var listitem in discription)
            {

                string[] itemstring = listitem.Split(' ');
                int lastindex = itemstring.Length - 1;
                ItemList obj = new ItemList()
                {
                    LineNumber = slno.ToString(),
                    per_Amount = itemstring[lastindex],
                    Rate = itemstring[lastindex - 2],
                    Quantity = itemstring[lastindex - 4].Split('.')[0],
                    HSN_SAC = itemstring[lastindex - 5],
                    InvoiceNOandDate = model.InvoiceNumber + Environment.NewLine + model.InvoiceDate,
                    PONumber = model.PONumber,
                };
                ListObj.Add(obj);
                slno++;
            }
            var DescList = GetDescription(textpdf, indexarr);
            for (int i = 0; i < DescList.Count; i++)
            {
                ListObj[i].Description_of_Goods = DescList[i];
            }
            return ListObj;
        }
        public static List<string> GetDescription(List<string> pdflist, List<int> indexes)
        {
            string desc = string.Empty;
            List<string> strList = new List<string>();
            for (int i = 0; i < indexes.Count; i++)
            {
                desc = string.Empty;
                if (i != indexes.Count - 1)
                {
                    for (int j = indexes[i]; j < indexes[i + 1] - 1; j++)
                    {
                        desc += pdflist[j + 1] + Environment.NewLine;
                    }
                    strList.Add(desc);
                }
                else
                {
                    desc = pdflist[indexes[i]].Split()[0] + " " + pdflist[indexes[i]].Split()[1];
                    desc += pdflist[indexes[i] + 1] + Environment.NewLine + pdflist[indexes[i] + 2] + Environment.NewLine + pdflist[indexes[i] + 3] +
                        Environment.NewLine + pdflist[indexes[i] + 4] + Environment.NewLine;
                    strList.Add(desc);
                }

            }
            return strList;
        }
    }
}
