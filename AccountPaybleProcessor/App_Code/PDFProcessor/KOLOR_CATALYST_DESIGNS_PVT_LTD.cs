using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AccountPaybleProcessor.App_Code;
using DataAccessLayer.App_Code.DBManager;
using DataAccessLayer.DBModel;
using Newtonsoft.Json;
using NLog;
using AccountPaybleProcessor.App_Code.PDFProcessor;

namespace AccountPaybleProcessor.App_Code.PDFProcessor
{
    public class KOLOR_CATALYST_DESIGNS_PVT_LTD : IPDFProcessor
    {
        IGSTDBManager dbManager;
        TBL_ProcessInstances tblProcessInstance;
        TBL_ProcessInstanceDetails tblProcessInstanceDetails;
        TBL_ProcessInstanceData tblProcessInstanceData;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public KOLOR_CATALYST_DESIGNS_PVT_LTD(IGSTDBManager dbManager, TBL_ProcessInstances tblProcessInstance)
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
                pdfObject.VendorGst = pdf.Where(x => x.Contains("GSTIN")).FirstOrDefault();
                pdfObject.VendorGst = pdfObject.VendorGst.Split(':')[1].Trim();
                pdfObject.GPIGST = pdf.Where(x => x.Contains("GSTIN")).ToList()[1].Replace(" ", "").Replace("GSTIN/UIN:", "");


                pdfObject.VendorName = "Kolor Catalyst Designs Pvt Ltd.";
                // int VnameIndex = pdf.FindIndex(x => x.Contains(pdfObject.VendorName));

                Regex pattern = new Regex(@"(([0-9])|([0-2][0-9])|([3][0-1]))\-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\-\d{4}");
                pdfObject.InvoiceDate = pattern.Match(pdfViewModel.PdfContent).Groups[0].Value;

                pattern = new Regex(@"[0-9]+\/[0-9]+\-[0-9]+\/\w{3}");
                pdfObject.InvoiceNumber = pattern.Match(pdfViewModel.PdfContent).Groups[0].Value;

                pdfObject.PONumber = pdf.Where(x => x.ToUpper().Contains("Buyer’s Order No".ToUpper())).FirstOrDefault().Trim();
                int buyerindex = pdf.FindIndex(x => x.Contains(pdfObject.PONumber));
                string potext = pdf[buyerindex - 1] + pdf[buyerindex + 1] + pdf[buyerindex + 2] + pdf[buyerindex + 3] + pdf[buyerindex + 4];
                pattern = new Regex(@"[0-9]{9,11}");
                pdfObject.PONumber = pattern.Match(potext).Groups[0].Value;
               
                pdfObject.InvoiceAmount = pdf.Where(x => x.ToUpper().Contains("Amount Chargeable".ToUpper())).FirstOrDefault();
                int chargeableamount = pdf.FindIndex(x => x.Contains(pdfObject.InvoiceAmount));
                pdfObject.InvoiceAmount = pdf[chargeableamount - 1].Trim();
                pdfObject.PdfContent = pdfViewModel.PdfContent;
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
        public static List<ItemList> ListofGoods(ViewPdfModel model)
        {
            string pdfcontent = model.PdfContent;
            List<ItemList> ListObj = new List<ItemList>();
            ItemList item = new ItemList();
            ViewPdfModel pdfmodel = new ViewPdfModel();
            List<int> indexarr = new List<int>();
            List<string> discription = new List<string>();
            string description_str = pdfcontent.Substring(pdfcontent.IndexOf("Description of"), pdfcontent.IndexOf("This is a Computer Generated Invoice") - pdfcontent.IndexOf("Description of"));

            List<string> textpdf = new List<string>(description_str.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            //string Nos = textpdf.Where(x => x.Contains("NOS")).LastOrDefault();
            foreach (var lineintextpdf in textpdf)
            {
                if (lineintextpdf.Contains("Sq.ft"))
                {
                    string line = string.Empty;
                    if (!(lineintextpdf.Split(new[] { "Sq.ft" }, StringSplitOptions.None)[1].Trim().Equals("")))
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
            string nameofGoods = string.Empty;
            string desc = string.Empty;
            List<string> strList = new List<string>();
            for (int i = 0; i < indexes.Count; i++)
            {
                desc = string.Empty;

                nameofGoods = pdflist[indexes[i]].Split(new[] { "Sq.ft" }, StringSplitOptions.None)[0];
                nameofGoods = Regex.Replace(nameofGoods, @"[\d\.]", string.Empty).Trim();
                if (i != indexes.Count - 1)
                {
                    for (int j = indexes[i]; j < indexes[i + 1] - 1; j++)
                    {
                        desc += pdflist[j + 2] + Environment.NewLine;
                    }
                    desc = nameofGoods + Environment.NewLine + desc;
                    strList.Add(desc);
                }
                else
                {
                    desc = nameofGoods + Environment.NewLine;
                    for (int d = 2; d < 5; d++)
                    {
                        try
                        {
                            desc += pdflist[indexes[i] + d] + Environment.NewLine;
                        }
                        catch
                        {

                        }
                        strList.Add(desc);
                    }
                    nameofGoods = string.Empty;
                }
                return strList;
            }

           return strList;
        }
    }
}




