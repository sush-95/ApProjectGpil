using System;

using System.Linq;
using System.Text;

using System.Threading.Tasks;

using System.Collections.Generic;
using System.Text.RegularExpressions;
using AccountPaybleProcessor.App_Code;
using Newtonsoft.Json;

namespace PDFProcessorTestConsole.PDFProcessor
{
    public class AsthaPkgPvt 
    {
      

        public void ExtractPDFData(ViewPdfModel pdfViewModel, string filePath, string ProcessedInvoicePath)
        {
            //this.tblProcessInstanceDetails = dbManager.GetProcessInstanceDetail(tblProcessInstance.ProcessInstanceId);
            //this.tblProcessInstanceData = dbManager.GetProcessInstanceData(tblProcessInstance.ProcessInstanceId);
            string newFilePath = string.Empty;
            String[] rate = new String[] { };

            ViewPdfModel pdfObject = new ViewPdfModel();
            string strProcess = string.Empty;
            string refInvoiceNo = string.Empty;
            string refInvoiceDate = string.Empty;

            try
            {
                //logger.Info("ExtractPDFData Started");
                //long firstProcessInstanceId = tblProcessInstanceData.ProcessInstanceId;
                var pdf = pdfViewModel.PdfText;
                pdfObject.VendorGst = pdf.Where(x => x.Contains("GSTIN")).FirstOrDefault().Replace(" ", "").Replace("GSTIN/UIN:", "");
                pdfObject.VendorName = pdf.Where(x => x.ToUpper().Contains("ASTHA")).FirstOrDefault();
                int VnameIndex = pdf.FindIndex(x => x.Contains(pdfObject.VendorName));
                if (pdf[VnameIndex + 2].Contains(' '))
                {
                    pdfObject.InvoiceDate = pdf[VnameIndex + 2].Split(' ')[1];
                    pdfObject.InvoiceNumber = pdf[VnameIndex + 2].Split(' ')[0];
                }
                else
                {
                    pdfObject.InvoiceDate = pdf[VnameIndex + 2];
                    pdfObject.InvoiceNumber = pdf[VnameIndex + 3].Split(' ')[0].Trim();
                }
                pdfObject.PONumber = pdf[pdf.FindIndex(x => x.Contains("Order No")) + 2].Split(' ')[0];

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
                //tblProcessInstanceData.ChildInstanceId = firstProcessInstanceId;
                //dbManager.UpdateProcessInstanceData(tblProcessInstanceData);
                //string pdfData = GetMetadata(pdfObject);
                //dbManager.NextInstance(tblProcessInstance, tblProcessInstanceData, tblProcessInstanceDetails, pdfData, Common.Constants.Process.States.APPDFProcess.DataExtracted, firstProcessInstanceId);

                //PDFManager.MovePdfToProcessedFolder(newFilePath, newFolderPath, filePath);
                //logger.Info("ExtractPDFData Completed");
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                throw;
            }
        }
        public string GetMetadata(ViewPdfModel model)
        {
            JsonViewModel jmodelObj = new JsonViewModel();
            jmodelObj.Header = new Header()
            {
                CreatedTs = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss"),
                ProcessInstanceId = "",
                ProcessName = "APProcess",
                StateId = ""
            };
            jmodelObj.Status = new Status()
            {
                Value ="",
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
