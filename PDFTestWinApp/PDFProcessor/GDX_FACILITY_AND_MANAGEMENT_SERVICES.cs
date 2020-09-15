using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AccountPaybleProcessor.App_Code;

using Newtonsoft.Json;


namespace PDFTestWinApp.PDFProcessor
{
    public class GDX_FACILITY_AND_MANAGEMENT_SERVICES
    {
       

        public  void ExtractPDFData(ViewPdfModel pdfViewModel, string filePath, string ProcessedInvoicePath)
        {

            
            String[] rate = new String[] { };

            ViewPdfModel pdfObject = new ViewPdfModel();
            string strProcess = string.Empty;
            string refInvoiceNo = string.Empty;
            string refInvoiceDate = string.Empty;

            try
            {
                //long firstProcessInstanceId = tblProcessInstanceData.ProcessInstanceId;
                var pdf = pdfViewModel.PdfText;
                pdfObject.VendorGst = pdf.Where(x => x.Contains("GSTIN")).LastOrDefault();
                pdfObject.VendorGst = pdfObject.VendorGst.Split(':')[1].Trim();
                pdfObject.GPIGST = pdf.Where(x => x.Contains("GSTIN")).FirstOrDefault().Split(':')[0];
                pdfObject.GPIGST = Regex.Match(pdfObject.GPIGST, "(?<=GSTIN ).*(?= MSME)").ToString();
                pdfObject.VendorName = "G D X";

                int invoiceDateIndex = pdf.FindIndex(x => x.ToUpper().Contains("Invoice Date".ToUpper()));
                pdfObject.InvoiceDate = pdf[invoiceDateIndex - 1].Trim();
                int InvoiceNumberindex = pdf.FindIndex(x => x.ToUpper().Contains("Invoice No".ToUpper()));
                pdfObject.InvoiceNumber = pdf[InvoiceNumberindex + 1].Trim();
                int grossbillvalue = pdf.FindIndex(x => x.ToUpper().Contains("Gross Bill Value".ToUpper().Trim()));
                pdfObject.InvoiceAmount = pdf[grossbillvalue + 1].Trim();
                pdfObject.PdfContent = pdfViewModel.PdfContent;
                //List Item part wil be done once get more files.
                pdfObject.ListItem = ListofGoods(pdfObject);
                pdfObject.PdfContent = string.Empty;
                //pdfObject.PONumber = pdf.Where(x => x.ToUpper().Contains("P.O No".ToUpper())).FirstOrDefault().Trim();
                //pdfObject.PONumber = pdfObject.PONumber.Split(':')[2].Trim();

                string newFolderPath = ProcessedInvoicePath + pdfObject.GPIGST + "\\\\" + DateTime.Now.ToString("MMM-yyyy") + "\\\\" + pdfObject.VendorName + "\\\\";
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
                ProcessInstanceId = "",
                ProcessName = "APPdfExctract",
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
    }
}
