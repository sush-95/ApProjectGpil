using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AccountPaybleProcessor.App_Code;
using DataAccessLayer.App_Code.DBManager;
using DataAccessLayer.DBModel;
using Newtonsoft.Json;
using NLog;

namespace AccountPaybleProcessor.App_Code.PDFProcessor
{
    public class GDX_FACILITY_AND_MANAGEMENT_SERVICES:IPDFProcessor
    {
        IGSTDBManager dbManager;
        TBL_ProcessInstances tblProcessInstance;
        TBL_ProcessInstanceDetails tblProcessInstanceDetails;
        TBL_ProcessInstanceData tblProcessInstanceData;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public GDX_FACILITY_AND_MANAGEMENT_SERVICES(IGSTDBManager dbManager, TBL_ProcessInstances tblProcessInstance)
        {
            this.dbManager = dbManager;
            this.tblProcessInstance = tblProcessInstance;
        }

        public  void ExtractPDFData(ViewPdfModel pdfViewModel, string filePath, string ProcessedInvoicePath)
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
                int gstindex = pdf.FindLastIndex(x => x.Contains("GSTIN"));
                pdfObject.VendorGst = pdf[gstindex] + pdf[gstindex + 1];
                pdfObject.VendorGst = pattern.Match(pdfObject.VendorGst).Groups[0].Value;
                gstindex = pdf.FindIndex(x => x.Contains("GSTIN"));
                pdfObject.GPIGST = pdf[gstindex - 2] + " " + pdf[gstindex - 1] + " " + pdf[gstindex] + pdf[gstindex + 1];
                pdfObject.GPIGST = pattern.Match(pdfObject.GPIGST).Groups[0].Value;
                pdfObject.VendorName = "GDX FACILITY & MANAGEMENT SERVICES PVT LTD.";
                pattern = new Regex(@"[0-9]+\/[0-9]+\/\d{4}");
                int invoiceDateIndex = pdf.FindIndex(x => x.ToUpper().Contains("Invoice Date".ToUpper()));
                pdfObject.InvoiceDate = pdf[invoiceDateIndex - 2] + pdf[invoiceDateIndex - 1] + pdf[invoiceDateIndex] + pdf[invoiceDateIndex + 1] + pdf[invoiceDateIndex + 2];
                pdfObject.InvoiceDate = pattern.Match(pdfObject.InvoiceDate).Groups[0].Value;
                pattern = new Regex(@"\w+\/\d+\-\d+\/\d+");
                int InvoiceNumberindex = pdf.FindIndex(x => x.Contains("Invoice No"));
                pdfObject.InvoiceNumber = pdf[InvoiceNumberindex - 2] + pdf[InvoiceNumberindex - 1] + pdf[InvoiceNumberindex] + pdf[InvoiceNumberindex + 1] + pdf[InvoiceNumberindex + 2];
                pdfObject.InvoiceNumber = pattern.Match(pdfObject.InvoiceNumber).Groups[0].Value;

                int grossbillvalue = pdf.FindIndex(x => x.ToUpper().Contains("Grand Total".ToUpper().Trim()));
                pdfObject.InvoiceAmount = (pdf[grossbillvalue] + pdf[grossbillvalue + 1]).Trim().Replace(",", "");
                pattern = new Regex(@"\d+(\.\d+)");
                pdfObject.InvoiceAmount = pattern.Match(pdfObject.InvoiceAmount).Groups[0].Value;
                pdfObject.PdfContent = pdfViewModel.PdfContent;
                InvoiceNumberindex = pdf.FindIndex(x => x.Contains("PO No"));
                pattern = new Regex(@"[0-9]{9,11}");
                pdfObject.PONumber = pdf[InvoiceNumberindex - 2] + pdf[InvoiceNumberindex - 1] + pdf[InvoiceNumberindex] + pdf[InvoiceNumberindex + 1] + pdf[InvoiceNumberindex + 2];
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
            string description_str = pdfcontent.Substring(pdfcontent.IndexOf("DESCRIPTION/DETAILS"), pdfcontent.IndexOf("E.&O.E") - pdfcontent.IndexOf("DESCRIPTION/DETAILS"));

            List<string> textpdf = new List<string>(description_str.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            //string Nos = textpdf.Where(x => x.Contains("NOS")).LastOrDefault();
            foreach (var lineintextpdf in textpdf)
            {
                if ((lineintextpdf.Split(' ').Length > 3 && lineintextpdf.Split(' ').Length < 8) && !Regex.IsMatch(lineintextpdf, @"([A-Za-z])\w+"))
                {
                    string line = string.Empty;
                    string LastValue = lineintextpdf.Split(new[] { " " }, StringSplitOptions.None)[lineintextpdf.Split(new[] { " " }, StringSplitOptions.None).Length - 1];
                    string SecondLastValue = lineintextpdf.Split(new[] { " " }, StringSplitOptions.None)[lineintextpdf.Split(new[] { " " }, StringSplitOptions.None).Length - 2];
                    Boolean LastVlaluecheck = Regex.IsMatch(LastValue, @"[\d,.]+");
                    Boolean SecondLastVlaluecheck = Regex.IsMatch(SecondLastValue, @"[\d,.]+");
                    if (LastVlaluecheck && SecondLastVlaluecheck)
                    {
                        line = Regex.Replace(lineintextpdf.ToString(), @"\s+", " ");
                        discription.Add(line);
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
                    Rate = itemstring[lastindex - 3],
                    //Quantity = itemstring[lastindex - 6].Split('.')[0],
                    HSN_SAC = itemstring[lastindex - 4],
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
            string nameofGoods = string.Empty;
            string desc = string.Empty;
            List<string> strList = new List<string>();
            for (int i = 0; i < indexes.Count; i++)
            {
                desc = string.Empty;
                //nameofGoods = pdflist[indexes[i]].
                nameofGoods = Regex.Replace(pdflist[indexes[i]], @"[\d-,\.]", string.Empty);
                if (i != indexes.Count - 1)
                {
                    for (int j = indexes[i]; j < indexes[i + 1] - 1; j++)
                    {
                        desc += pdflist[j + 1] + Environment.NewLine;
                    }
                    desc = nameofGoods + Environment.NewLine + desc;
                    strList.Add(desc);
                }
                else
                {
                    desc = nameofGoods + Environment.NewLine + pdflist[indexes[i] + 1] + Environment.NewLine + pdflist[indexes[i] + 2] + Environment.NewLine;
                    strList.Add(desc);
                }
                nameofGoods = string.Empty;
            }
            return strList;
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
