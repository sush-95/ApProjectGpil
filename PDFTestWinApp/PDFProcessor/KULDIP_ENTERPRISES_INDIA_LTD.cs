using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AccountPaybleProcessor.App_Code;

using Newtonsoft.Json;
//using NLog;


namespace PDFProcessorTestConsole.PDFProcessor
{
    public class KULDIP_ENTERPRISES_INDIA_LTD

    { 

        public void ExtractPDFData(ViewPdfModel pdfViewModel, string filePath, string ProcessedInvoicePath)
        {
           
            string newFilePath = string.Empty;
            String[] rate = new String[] { };
            ViewPdfModel pdfObject = new ViewPdfModel();
            string strProcess = string.Empty;
            string refInvoiceNo = string.Empty;
            string refInvoiceDate = string.Empty;

            try
            {
                //long firstProcessInstanceId = tblProcessInstanceData.ProcessInstanceId;
                var pdf = pdfViewModel.PdfText;
                pdfObject.VendorGst = pdf.Where(x => x.Contains("GSTIN")).FirstOrDefault();
                pdfObject.VendorGst = pdfObject.VendorGst.Split(':')[1].Trim();

                pdfObject.GPIGST = pdf.Where(x => x.Contains("GSTIN")).ToList()[1].Replace(" ", "").Replace("GSTIN/UIN:", "");
                pdfObject.VendorName = "Kuldip Enterprises India Limited";
                pdfObject.InvoiceDate = pdf.Where(x => x.ToUpper().Contains("Kuldip Enterprises".ToUpper())).FirstOrDefault().Trim();
                int kuldipindex = pdf.FindIndex(x => x.Contains(pdfObject.InvoiceDate));
                pdfObject.InvoiceDate = pdf[kuldipindex + 2].Split(' ')[1];
                pdfObject.InvoiceNumber = pdf[kuldipindex + 2].Split(' ')[0];
                int Grandtotalindex = pdf.FindIndex(x => x.ToUpper().Contains("Amount Chargeable".ToUpper()));
                pdfObject.InvoiceAmount = Regex.Replace(pdf[Grandtotalindex - 1].Trim(), @"[A-Za-z]+", "").Trim();
                pdfObject.PdfContent = pdfViewModel.PdfContent;
                pdfObject.ListItem = ListofGoods(pdfObject);
                pdfObject.PdfContent = string.Empty;
                string pdfData = GetMetadata(pdfObject);

                //string newFolderPath = ProcessedInvoicePath + pdfObject.GPIGST + "\\\\" + DateTime.Now.ToString("MMM-yyyy") + "\\\\" + pdfObject.VendorName + "\\\\";
                //newFilePath = newFolderPath + pdfObject.InvoiceNumber + ".pdf";
                //newFilePath = newFilePath.Replace("/", "");
                //newFolderPath = newFolderPath.Replace("/", "");
                //pdfObject.NewInvoicepath = newFilePath;
                //tblProcessInstanceData.ChildInstanceId = firstProcessInstanceId;
                //dbManager.UpdateProcessInstanceData(tblProcessInstanceData);
                //string pdfData = GetMetadata(pdfObject);
                //dbManager.NextInstance(tblProcessInstance, tblProcessInstanceData, tblProcessInstanceDetails, pdfData, Common.Constants.Process.States.APPDFProcess.DataExtracted, firstProcessInstanceId);
                //PDFManager.MovePdfToProcessedFolder(newFilePath, newFolderPath, filePath);
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
                ProcessInstanceId = "",
                ProcessName = "APPdfExctract",
                StateId =""
            };
            jmodelObj.Status = new Status()
            {
                Value = "",
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
            string description_str = pdfcontent.Substring(pdfcontent.IndexOf("Description of Goods"), pdfcontent.IndexOf("Declaration") - pdfcontent.IndexOf("Description of Goods"));
            description_str = description_str.Replace("Sqft", "sft");
            List<string> textpdf = new List<string>(description_str.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            //string Nos = textpdf.Where(x => x.Contains("NOS")).LastOrDefault();
            foreach (var lineintextpdf in textpdf)
            {
                if (lineintextpdf.Contains("sft"))
                {
                    string line = string.Empty;
                    if (!(lineintextpdf.Split(new[] { "sft" }, StringSplitOptions.None)[1].Trim().Equals("")))
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
            string desc = string.Empty;
            string nameofGoods = string.Empty;
            List<string> strList = new List<string>();
            for (int i = 0; i < indexes.Count; i++)
            {
                desc = string.Empty;
                nameofGoods = pdflist[indexes[i]].Split(new[] { "sft" }, StringSplitOptions.None)[0];
                nameofGoods = Regex.Replace(nameofGoods, @"[\d\.]", string.Empty).Trim();

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
                    desc = nameofGoods + Environment.NewLine + pdflist[indexes[i] + 2];
                    strList.Add(desc);
                }
                nameofGoods = string.Empty;
            }
            return strList;
        }

    }
}




