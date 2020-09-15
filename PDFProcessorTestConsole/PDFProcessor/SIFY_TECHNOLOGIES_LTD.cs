using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AccountPaybleProcessor.App_Code;
using AccountPaybleProcessor.App_Code.PDFProcessor;

using Newtonsoft.Json;

namespace PDFProcessorTestConsole.PDFProcessor
{
    public class SIFY_TECHNOLOGIES_LTD
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
                //long firstProcessInstanceId = tblProcessInstanceData.ProcessInstanceId;
                var pdf = pdfViewModel.PdfText;
                pdfObject.VendorGst = pdf.Where(x => x.Contains("GSTIN")).FirstOrDefault();
                pdfObject.VendorGst = pdfObject.VendorGst.Split(':')[1].Trim();
                pdfObject.GPIGST = pdf.Where(x => x.Contains("GSTIN")).ToList()[1].Replace(" ", "").Replace("GSTIN:", "");
                pdfObject.VendorName = "Sify Technologies Limited";
                pdfObject.InvoiceDate = pdf.Where(x => x.ToUpper().Contains("Invoice Date".ToUpper())).FirstOrDefault().Trim();
                pdfObject.InvoiceDate = pdfObject.InvoiceDate.Split(':')[1].Trim();
                pdfObject.InvoiceNumber = pdf.Where(x => x.ToUpper().Contains("Invoice Number".ToUpper())).FirstOrDefault().Trim();
                pdfObject.InvoiceNumber = pdfObject.InvoiceNumber.Split(':')[1].Trim();
                string PONumber = pdf.Where(x => x.Contains("Customer PO")).FirstOrDefault();
                pdfObject.PONumber = Regex.Replace(PONumber.Split(':')[1].Trim(), @"([A-Za-z])\w+", "").Trim();
                if (string.IsNullOrEmpty(pdfObject.PONumber.Trim()))
                {
                    int indx = pdf.FindIndex(x => x.Contains("Customer PO"));
                    pdfObject.PONumber = pdf[indx - 1].Trim();
                }
                int invoiceAmountindex = pdf.FindIndex(x => x.ToUpper().Contains("Invoice Amount".ToUpper().Trim()));

                pdfObject.InvoiceAmount = pdf[invoiceAmountindex + 1].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[pdf[invoiceAmountindex + 1].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length - 1];
                pdfObject.PdfContent = pdfViewModel.PdfContent;
                pdfObject.ListItem = ListofGoods(pdfObject);
                pdfObject.PdfContent = string.Empty;

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
                StateId = ""
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
            List<ItemList> ListObj = new List<ItemList>();
            try
            {
                string pdfcontent = model.PdfContent;

                ItemList item = new ItemList();
                ViewPdfModel pdfmodel = new ViewPdfModel();
                List<int> indexarr = new List<int>();
                List<string> discription = new List<string>();


                string description_str = pdfcontent.Substring(pdfcontent.IndexOf("Invoice Line Details"), pdfcontent.IndexOf("End of Invoice") - pdfcontent.IndexOf("Invoice Line Details"));

                List<string> textpdf = new List<string>(description_str.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
                if (textpdf[1].Contains("Service/s"))//Service Line Items
                {
                    item.per_Amount =textpdf[textpdf.FindIndex(x=>x.Contains("Total"))-1];
                    int indx = textpdf.FindIndex(x=>x.Contains("MBPS"));
                    indx = (indx < 0 ? textpdf.FindIndex(x => x.Contains("--")) : indx);
                    item.HSN_SAC = textpdf[indx].Split(' ').Last();
                    item.LineNumber = "1";
                    item.PONumber = model.PONumber;
                    item.Description_of_Goods = GetDescription(textpdf);
                    item.InvoiceNOandDate = model.InvoiceNumber + Environment.NewLine + model.InvoiceDate;
                    ListObj.Add(item);
                }
                else
                {
                    GetProductLineItems(textpdf, ref ListObj, model);//Product Line Items
                }
            }
            catch
            { }
            return ListObj;
        }
        public static void GetProductLineItems(List<string> textpdf, ref List<ItemList> List, ViewPdfModel model)
        {

            List<int> indexarr = new List<int>();
            List<string> description = new List<string>();
            try
            {
                foreach (var lineintextpdf in textpdf)
                {
                    if (lineintextpdf.Contains("Days"))
                    {
                        string line = string.Empty;
                        if (!(lineintextpdf.Split(new[] { "Days" }, StringSplitOptions.None)[1].Trim().Equals("")))
                        {
                            line = Regex.Replace(lineintextpdf.ToString(), @"\s+", " ");
                            description.Add(line);
                            indexarr.Add(textpdf.FindIndex(x => x.Contains(lineintextpdf)));
                        }
                    }
                }

                int slno = 1;
                foreach (var listitem in description)
                {
                    string[] itemstring = listitem.Split(' ');
                    itemstring = itemstring.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    int lastindex = itemstring.Length - 1;
                    ItemList obj = new ItemList()
                    {
                        LineNumber = slno.ToString(),
                        per_Amount = itemstring[lastindex],
                        Rate = itemstring[lastindex - 2],
                        Quantity = itemstring[lastindex - 1],
                        HSN_SAC = itemstring[lastindex - 4],
                        InvoiceNOandDate = model.InvoiceNumber + Environment.NewLine + model.InvoiceDate,
                        PONumber = model.PONumber,
                    };
                    List.Add(obj);
                    slno++;

                }
                var desclist = GetProductDesc(textpdf, indexarr);
                for (int i = 0; i < desclist.Count; i++)
                {
                    List[i].Description_of_Goods = desclist[i];
                }
            }
            catch
            {

            }

        }
        public static string GetDescription(List<string> list)
        {
            string desc = string.Empty;
            try
            {
                int indx = list.FindIndex(x => x.Contains("Service/s HSN Code"));
                desc += list[indx + 2] + Environment.NewLine + list[indx + 3] + Environment.NewLine;
                if (!string.IsNullOrEmpty(list.Where(x => x.Contains("Service Charges")).FirstOrDefault()))
                {
                    desc += list.Where(x => x.Contains("Service Charges")).FirstOrDefault() + Environment.NewLine;
                }
                string str = list.Where(x => x.Contains("MBPS")).FirstOrDefault();
                desc += (string.IsNullOrEmpty(str) ? "--" : (str.Split(' ').Length > 1 ? str.Replace(str.Split(' ').Last(), "") : str)) + Environment.NewLine;
                desc += list.Where(x => x.Contains("LINK ID")).FirstOrDefault().Trim() + Environment.NewLine;
                desc += list.Where(x => x.Contains("PERIOD")).FirstOrDefault();
                return desc;
            }
            catch
            {
                return desc;
            }
        }

        public static List<string> GetProductDesc(List<string> pdflist, List<int> indexes)
        {
            List<string> strList = new List<string>();

            string nameofGoods = string.Empty;
            string desc = string.Empty;

            for (int i = 0; i < indexes.Count; i++)
            {
                try
                {
                    nameofGoods = pdflist[indexes[i]].Split(new[] { "Days" }, StringSplitOptions.None)[0];
                    nameofGoods = Regex.Replace(nameofGoods, @"[\d\.]", string.Empty).Trim();
                    if (i != indexes.Count - 1)
                    {
                        desc = nameofGoods + Environment.NewLine;
                        for (int j = indexes[i]; j < indexes[i + 1] - 1; j++)
                        {
                            desc += pdflist[j + 1] + Environment.NewLine;
                        }
                        desc = nameofGoods + Environment.NewLine + desc;
                        strList.Add(desc);
                    }
                    else
                    {
                        desc = nameofGoods + Environment.NewLine;
                        for (int d = 1; d < 6; d++)
                        {
                            try
                            {
                                desc += pdflist[indexes[i] + d] + Environment.NewLine;
                            }
                            catch{}
                        }
                        nameofGoods = string.Empty;
                        strList.Add(desc);
                    }
                }
                catch (Exception ex)
                {
                    strList.Add(" ");
                }

            }
            return strList;
        }
    }
}




