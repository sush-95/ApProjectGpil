﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AccountPaybleProcessor.App_Code;
using AccountPaybleProcessor.App_Code.PDFProcessor;
//using DataAccessLayer.App_Code.DBManager;
//using DataAccessLayer.DBModel;
using Newtonsoft.Json;
//using NLog;


namespace AccountPaybleProcessor.App_Code.PDFProcessor
{
    public class HITACHI_SYSTEMS_MICRO_CLINIC_PVT_LTD
    {

        //IGSTDBManager dbManager;
        //TBL_ProcessInstances tblProcessInstance;
        //TBL_ProcessInstanceDetails tblProcessInstanceDetails;
        //TBL_ProcessInstanceData tblProcessInstanceData;
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        //public HITACHI_SYSTEMS_MICRO_CLINIC_PVT_LTD(IGSTDBManager dbManager, TBL_ProcessInstances tblProcessInstance)
        //{
        //    this.dbManager = dbManager;
        //    this.tblProcessInstance = tblProcessInstance;
        //}
        public  void ExtractPDFData(ViewPdfModel pdfViewModel, string filePath, string ProcessedInvoicePath)
        {


            //this.tblProcessInstanceDetails = dbManager.GetProcessInstanceDetail(tblProcessInstance.ProcessInstanceId);
            //this.tblProcessInstanceData = dbManager.GetProcessInstanceData(tblProcessInstance.ProcessInstanceId);
            //string newFilePath = string.Empty;
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
                pdfObject.VendorGst = pdfObject.VendorGst.Split(':')[1].Replace("PAN No.","");
                pdfObject.GPIGST = pdf.Where(x => x.Contains("GSTIN")).ToList()[1].Replace(" ", "").Replace("GSTIN/UniqueID:", "");
                pdfObject.VendorName = "HITACHI SYSTEMS MICRO CLINIC PVT.LTD.";
                int invoiceDateIndex = pdf.FindIndex(x => x.ToUpper().Contains("Invoice Date".ToUpper()));
                pdfObject.InvoiceDate = pdf[invoiceDateIndex + 1].Trim().Replace(":","");
                
                pdfObject.InvoiceNumber = pdf.Where(x => x.ToUpper().Contains("Invoice Number".ToUpper())).FirstOrDefault().Trim();
                pdfObject.InvoiceNumber = pdfObject.InvoiceNumber.Split(':')[1].Trim();


                pdfObject.PONumber = pdf.Where(x => x.ToUpper().Contains("Customer PO No".ToUpper())).FirstOrDefault().Trim();
                pdfObject.PONumber = pdfObject.PONumber.Split(':')[1].Trim();

                pdfObject.InvoiceAmount = pdf.Where(x => x.ToUpper().Contains("After Tax".ToUpper())).FirstOrDefault().Trim();
                pdfObject.InvoiceAmount = pdfObject.InvoiceAmount.Split(' ')[pdfObject.InvoiceAmount.Split(' ').Length - 1];

               
                pdfObject.PdfContent = pdfViewModel.PdfContent;
                pdfObject.ListItem = ListofGoods(pdfObject);
                pdfObject.PdfContent = string.Empty;

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
            string description_str = pdfcontent.Substring(pdfcontent.IndexOf("Description"), pdfcontent.IndexOf("Amount in Word") - pdfcontent.IndexOf("Description"));

            List<string> textpdf = new List<string>(description_str.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            //string Nos = textpdf.Where(x => x.Contains("NOS")).LastOrDefault();
            foreach (var lineintextpdf in textpdf)
            {
                if (lineintextpdf.Split(' ').Length > 8)
                {
                    string line = string.Empty;
                    string LastValue = lineintextpdf.Split(new[] { " " }, StringSplitOptions.None)[lineintextpdf.Split(new[] { " " }, StringSplitOptions.None).Length - 1];
                    Boolean check = Regex.IsMatch(LastValue, @"[\d,.]+");
                    if (check)
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
                    per_Amount = itemstring[lastindex - 5],
                    Rate = itemstring[lastindex],
                    Quantity = itemstring[lastindex - 6].Split('.')[0],
                    HSN_SAC = itemstring[lastindex - 7],
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
            try {
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
                        desc = nameofGoods;
                        for(int h=(indexes[i]+1);h<pdflist.Count;h++)
                        {
                            desc += Environment.NewLine + pdflist[h];
                        }

                        //desc = nameofGoods + Environment.NewLine + pdflist[indexes[i] + 1] + Environment.NewLine + pdflist[indexes[i] + 2] + Environment.NewLine + pdflist[indexes[i] + 3] +
                        //    Environment.NewLine + pdflist[indexes[i] + 4] + Environment.NewLine;
                        strList.Add(desc);
                    }
                    nameofGoods = string.Empty;
                }

            }
            catch (Exception ex) {

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
                Value = "",
                Description = "Pdf Value Extracted Successfully."
            };
            jmodelObj.Detail = model;
            return JsonConvert.SerializeObject(jmodelObj);
        }
    }
}



