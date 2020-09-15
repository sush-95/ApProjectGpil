using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Threading.Tasks;

namespace Common.App_Code
{
    public class PDFManager : IDisposable
    {
        #region Private Variables

        String[] rates = new String[] { };
        String[] amts = new String[] { };
        String[] reqdata = new String[] { };
        String[] data = new String[] { };
        string InvoiceFolderPath;
        string pathpdf;

        #endregion

        public PDFManager()
        {
            this.InvoiceFolderPath = ConfigurationManager.AppSettings["InvoiceFolderPath"];
        }

        public void readPDF()
        {
            pathpdf = InvoiceFolderPath + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\";
            string[] filesindirectory = Directory.GetDirectories(pathpdf);
            foreach (string subdir in filesindirectory)
            {
                foreach (string file in Directory.GetFiles(subdir))
                {
                    data = pdfText(file);
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(data[i]))
                        {
                            data[i] = data[i].Trim();
                        }
                    }
                    pdfdataExtract(data, out rates, out amts, out reqdata);
                }
            }
        }

        public static string[] pdfText(string path)
        {
            PdfReader reader = new PdfReader(path);
            string text = string.Empty;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += PdfTextExtractor.GetTextFromPage(reader, page).Trim();
            }
            string[] textpdf = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            //List<string> y = textpdf.ToList<string>();
            //y.RemoveAll(p => string.IsNullOrEmpty(p));
            //textpdf = y.ToArray();
            reader.Close();
            return textpdf;
        }

        public static void pdfdataExtract(string[] data, out string[] rate, out string[] amt, out string[] values)
        {
            values = new String[] { };
            amt = new String[] { };
            rate = new String[] { };

            string arydata = string.Empty;
            if (data[0].Contains("InterGlobe Aviation Limited") || data[1].Contains("InterGlobe Aviation Limited"))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Contains("Number : "))
                    {
                        data[i] = data[i].ToString().Replace("Number : ", "").Trim();
                        values = data[i].Split(new string[] { " " }, StringSplitOptions.None);
                    }
                    if (data[i].Contains("Date\t\t\t\t\t  :  "))
                    {
                        data[i] = data[i].ToString().Replace("Date\t\t\t\t\t  :  ", "").Trim();
                        values = values.Concat(data[i].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                    if (data[i].Contains("Air Travel and related charges"))
                        rate = data[i + 1].Split(new string[] { " " }, StringSplitOptions.None);
                    if (data[i].Contains("Grand Total"))
                    {
                        amt = data[i - 1].Split(new string[] { " " }, StringSplitOptions.None);
                        values = values.Concat(amt[0].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                }
            }
            if (data[0].Contains("AIR INDIA LTD.") || data[1].Contains("AIR INDIA LTD."))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Contains("Invoice No."))
                        values = values.Concat(data[i - 1].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    if (data[i].Contains("Invoice Date"))
                        values = values.Concat(data[i - 1].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    if (data[i].Contains("Total") && !data[i].Contains("Discount"))
                    {

                    }
                    if (data[i].Contains("Total") && !data[i].Contains("Discount"))
                    {
                        data[i] = data[i].Replace("Total ", "").Trim();
                        amt = data[i].Split(new string[] { " " }, StringSplitOptions.None);
                    }
                }
            }
            if (data[0].Contains("Lufthansa German Airlines") || data[1].Contains("Lufthansa German Airlines") || data[2].Contains("Lufthansa German Airlines"))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Contains("Invoice Number:"))
                    {
                        arydata = data[i].ToString();
                        data[i] = data[i].Substring(data[i].IndexOf("Invoice"), data[i].Length - data[i].IndexOf("Invoice")).Trim();
                        data[i] = data[i].Replace("Invoice Number:", "").Trim();
                        values = data[i].Split(new string[] { " " }, StringSplitOptions.None);
                        arydata = arydata.Remove(arydata.IndexOf("Invoice"), arydata.Length - arydata.IndexOf("Invoice")).Trim();
                        arydata = arydata.Replace("Date of Issue:", "").Trim();
                        values = values.Concat(arydata.Split(new string[] { " " }, StringSplitOptions.None)).ToArray();

                    }
                    if (data[i].Contains("CGST") && data[i].Contains("SGST"))
                    {
                        data[i + 1] = data[i + 1].Replace("(", "").Replace(")", "").Replace("%", "").Trim();
                        rate = data[i + 1].Split(new string[] { " " }, StringSplitOptions.None);
                    }
                    if (data[i].Contains("fare"))
                    {
                        data[i] = data[i].Replace("(fare + YQ/YR ", "").Replace("INR", "").Trim();
                        amt = data[i].Split(new string[] { " " }, StringSplitOptions.None);
                        values = values.Concat(amt[0].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                }
            }
            if (data[0].Contains("THAI AIRWAYS") || data[1].Contains("THAI AIRWAYS"))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Contains("Invoice No. :"))
                    {
                        data[i] = data[i].Replace("Invoice No. :", "").Trim();
                        values = data[i].Split(new string[] { " " }, StringSplitOptions.None);
                    }
                    if (data[i].Contains("Invoice Date :"))
                    {
                        data[i] = data[i].Replace("Invoice Date :", "").Trim();
                        values = values.Concat(data[i].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                    if (data[i].Contains("Airfare Charge") && !data[i].Contains("Fees Charge"))
                    {
                        data[i + 1] = data[i + 1].Substring(0, data[i + 1].IndexOf(" ") + 1).Trim();
                        values = values.Concat(data[i + 1].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                    if (data[i].Contains("SGST :"))
                    {
                        data[i] = data[i].Replace("SGST :", "").Trim();
                        data[i] = data[i].Substring(0, data[i].IndexOf(" ") + 1).Trim();
                        amt = data[i].Split(new string[] { " " }, StringSplitOptions.None);
                    }
                    if (data[i].Contains("CGST :"))
                    {
                        data[i] = data[i].Replace("CGST :", "").Trim();
                        data[i] = data[i].Substring(0, data[i].IndexOf(" ") + 1).Trim();
                        amt = amt.Concat(data[i].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                    if (data[i].Contains("IGST :"))
                    {
                        data[i] = data[i].Replace("IGST :", "").Trim();
                        data[i] = data[i].Substring(0, data[i].IndexOf(" ") + 1).Trim();
                        amt = amt.Concat(data[i].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                }
            }
            if (data[0].Contains("Supplier Details"))   /* For Spice Jet */
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Contains("Invoice No."))
                    {
                        data[i] = data[i].Substring(data[i].IndexOf("Invoice No."), data[i].Length - data[i].IndexOf("Invoice No."));
                        data[i] = data[i].Replace("Invoice No. ", "").Replace("Name", "").Trim();
                        values = data[i].Split(new string[] { " " }, StringSplitOptions.None);
                    }
                    if (data[i].Contains(" Date of Invoice "))
                    {
                        data[i - 1] = data[i - 1].ToString().Replace("Name of ", "").Trim();
                        values = values.Concat(data[i - 1].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                    if (data[i].Contains("CGST"))
                    {
                        data[i - 1] = data[i - 1].Replace("Rate", "").Replace("%", "").Trim();
                        rate = rate.Concat(data[i - 1].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                        data[i + 1] = data[i + 1].Replace("Amount", "").Replace("INR", "").Trim();
                        amt = amt.Concat(data[i + 1].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                    if (data[i].Contains("SGST"))
                    {
                        data[i - 1] = data[i - 1].Replace("Rate", "").Replace("%", "").Trim();
                        rate = rate.Concat(data[i - 1].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                        data[i + 1] = data[i + 1].Replace("Amount", "").Replace("INR", "").Trim();
                        amt = amt.Concat(data[i + 1].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                    if (data[i].Contains("IGST"))
                    {
                        data[i - 1] = data[i - 1].Replace("Rate", "").Replace("%", "").Trim();
                        rate = rate.Concat(data[i - 1].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                        data[i + 1] = data[i + 1].Remove(data[i + 1].IndexOf("Total"), data[i + 1].Length - data[i + 1].IndexOf("Total"));
                        data[i + 1] = data[i + 1].Replace("Amount", "").Replace("INR", "").Trim();
                        amt = amt.Concat(data[i + 1].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                }
            }
            if (data[0].Contains("SpiceJet Limited") || data[1].Contains("SpiceJet Limited"))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Contains("Invoice No:"))
                    {
                        data[i] = data[i].Replace("Invoice No:", "").Trim();
                        values = data[i].Split(new string[] { " " }, StringSplitOptions.None);
                    }
                    if (data[i].Contains("Invoice Date:"))
                    {
                        data[i] = data[i].Replace("Invoice Date:", "").Trim();
                        if (data[i].ToLower().Contains("am") || data[i].ToLower().Contains("pm"))
                        {
                            data[i] = data[i].Substring(0, data[i].IndexOf(" ") + 1).Trim();
                        }
                        values = values.Concat(data[i].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                }
            }
            if (data[0].Contains("TATA SIA Airlines") || data[1].Contains("TATA SIA Airlines"))   /* For Vistara */
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Contains("Invoice No."))
                    {
                        data[i] = data[i].Replace("Invoice No.", "").Trim();
                        values = data[i].Split(new string[] { " " }, StringSplitOptions.None);
                    }
                    if (data[i].Contains("Invoice Date"))
                    {
                        data[i] = data[i].Replace("Invoice Date", "").Trim();
                        values = values.Concat(data[i].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                    }
                    if (data[i].Contains("Total") && !data[i].Contains("Invoice") && !data[i].Contains("Journey") && !data[i].Contains("Taxable"))
                    {
                        data[i] = data[i].Replace("Total", "").Trim();
                        amt = amt.Concat(data[i].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();
                        values = values.Concat(amt[3].Split(new string[] { " " }, StringSplitOptions.None)).ToArray();

                    }
                }
            }
        }

        public void Dispose()
        {
            
        }
    }
}
