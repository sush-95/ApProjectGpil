﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AccountPaybleProcessor.App_Code;

using Newtonsoft.Json;


namespace PDFProcessorTestConsole.PDFProcessor
{
    public class NIYOGI_OFFSET_PVT_LTD
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
                // long firstProcessInstanceId = tblProcessInstanceData.ProcessInstanceId;
                var pdf = pdfViewModel.PdfText;
                pdfObject.VendorGst = pdf.Where(x => x.Contains("GSTIN/UIN")).FirstOrDefault();
                pdfObject.VendorGst = pdfObject.VendorGst.Split(':')[1].Trim().Split(' ')[0];
                pdfObject.GPIGST = pdf.Where(x => x.Contains("GSTIN")).ToList()[1];
                pdfObject.GPIGST = pdfObject.GPIGST.Split(':')[1].Trim().Split(' ')[0];

                pdfObject.VendorName = "NIYOGI OFFSET PVT LTD.";
                int VnameIndex = pdf.FindIndex(x => x.Contains(pdfObject.VendorName));

                Console.WriteLine(pdf[VnameIndex + 1].Split(' ').Length);
                if (!(pdf[VnameIndex + 1].Split(' ').Length == 1))
                {
                    pdfObject.InvoiceDate = pdf[VnameIndex + 1].Split(' ')[1].Trim();

                    pdfObject.InvoiceNumber = pdf[VnameIndex + 1].Split(' ')[0].Trim();
                }
                else
                {
                    pdfObject.InvoiceDate = pdf[VnameIndex + 1].Trim();
                    pdfObject.InvoiceNumber = pdf.Where(x => x.ToUpper().Contains("E-Mail".ToUpper())).FirstOrDefault();
                    int Emailindex = pdf.FindIndex(x => x.Contains(pdfObject.InvoiceNumber));
                    pdfObject.InvoiceNumber = ((pdf[Emailindex + 1].Contains("/") && pdf[Emailindex].Contains("-")) ? pdf[Emailindex + 1] : pdf[Emailindex - 1]);
                }
                pdfObject.PONumber = pdf.Where(x => x.ToUpper().Contains("Buyer’s Order No".ToUpper())).FirstOrDefault();
                int ByerIndex = pdf.FindIndex(x => x.Contains(pdfObject.PONumber));
                string[] pox = pdf[ByerIndex + 1].Split(' ');
                pdfObject.PONumber = (pox.Length >= 2 ? pox[pox.Length - 2] : "").Trim();

                int chargebleIndex1 = pdf.FindIndex(x => x.ToUpper().Contains("Amount Chargeable".ToUpper()));
                // Console.WriteLine(pdf[chargebleIndex1 -1]);

                //Console.WriteLine(chargebleIndex1);
                pdfObject.InvoiceAmount = pdf[chargebleIndex1 - 1].Split(' ')[pdf[chargebleIndex1 - 1].Split(' ').Length - 1].Trim();// Total amount on the basic of chargeble amount

                pdfObject.PdfContent = pdfViewModel.PdfContent;
                pdfObject.ListItem = ListofGoods(pdfObject);
                pdfObject.PdfContent = string.Empty;


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
                Console.WriteLine(ex);
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
            string description_str = pdfcontent.Substring(pdfcontent.IndexOf("Description of Goods"), pdfcontent.IndexOf("Amount Chargeable") - pdfcontent.IndexOf("Description of Goods"));

            List<string> textpdf = new List<string>(description_str.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            var tempPdf = textpdf;
            
            for(int ipdf= 0;ipdf < textpdf.Count;ipdf++)
            {
                if (textpdf[ipdf].Contains("NOS"))
                {
                    string line = string.Empty;
                    if (!(textpdf[ipdf].Split(new[] { "NOS" }, StringSplitOptions.None)[1].Trim().Equals("")))
                    {
                        line = Regex.Replace(textpdf[ipdf].ToString(), @"\s+", " ");
                        discription.Add(line);
                        indexarr.Add(ipdf);                        
                    }
                }
            }
            int slno = 1;
            foreach (var listitem in discription)
            {
                string[] itemstring = listitem.Split(' ');

                string[] Rate = textpdf[indexarr[slno - 1] + 1].Split(' ');
                if (Rate.Length == 1)
                {
                    Rate = textpdf[indexarr[slno - 1]].Split(new[] { "NOS" }, StringSplitOptions.None);
                }
                ItemList obj = new ItemList();
                try
                {
                    itemstring = itemstring.Where(x => !x.Equals("NOS")).ToArray();
                    int lastindex = itemstring.Length - 1;
                    obj.InvoiceNOandDate = model.InvoiceNumber + Environment.NewLine + model.InvoiceDate;
                    obj.PONumber = model.PONumber;
                    obj.LineNumber = slno.ToString();
                    obj.per_Amount = itemstring[lastindex];
                    if (Rate.Length ==4)
                    {
                        obj.Rate = Rate[Rate.Length - 2];
                        obj.HSN_SAC = Rate[Rate.Length - 3];
                        obj.Quantity = itemstring[lastindex - 1];
                    }
                    else
                    {
                        obj.Rate = Rate[Rate.Length - 2];
                        obj.HSN_SAC = itemstring[lastindex - 3].Replace(")", "").Replace("(", "");
                        obj.Quantity = itemstring[lastindex - 2].Split('.')[0];
                    }  
                }
                catch { }
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
                nameofGoods = pdflist[indexes[i]].Split(new[] { "NOS" }, StringSplitOptions.None)[0];
                // nameofGoods = Regex.Replace(nameofGoods, @"[\d\.]", string.Empty).Trim();
                nameofGoods = nameofGoods.Split(')')[0] + ")";

                if (i != indexes.Count - 1)
                {
                    for (int j = indexes[i]; j < indexes[i + 1] - 2; j++)
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
                            if (!string.IsNullOrEmpty(pdflist[indexes[i] + d].Trim()))
                            {
                                desc += pdflist[indexes[i] + d].Trim() + Environment.NewLine;
                            }
                        }
                        catch { }
                    }

                    strList.Add(desc);
                }

                nameofGoods = string.Empty;
            }
            return strList;
        }

        //public string GetMetadata(ViewPdfModel model)
        //{
        //    JsonViewModel jmodelObj = new JsonViewModel();
        //    jmodelObj.Header = new Header()
        //    {
        //        CreatedTs = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss"),
        //        ProcessInstanceId = tblProcessInstance.ProcessInstanceId.ToString(),
        //        ProcessName = "APPdfExctract",
        //        StateId = tblProcessInstanceDetails.StateId
        //    };
        //    jmodelObj.Status = new Status()
        //    {
        //        Value = Common.Constants.Process.States.APInvoiceStatus.PdfExctracted,
        //        Description = "Pdf Value Extracted Successfully."
        //    };
        //    jmodelObj.Detail = model;
        //    return JsonConvert.SerializeObject(jmodelObj);
        //}

    }
}

