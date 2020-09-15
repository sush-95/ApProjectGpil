using AccountPaybleProcessor.App_Code;
using AccountPaybleProcessor.App_Code.PDFProcessor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFProcessorTestConsole
{
    class Program
    {
       
        static void Main(string[] args)
        {
            ExtractPdf();
        }

        public static  void ExtractPdf()
        {
            string FileLoc = ConfigurationManager.AppSettings["Folder"];
            string FileName = FileLoc + "SifyTest.pdf";

            ViewPdfModel pdfViewModel = PDFManager.GetPDFText(FileName);
            string newFilePath = string.Empty;
            PDFProcessorTestConsole.PDFProcessor.SIFY_TECHNOLOGIES_LTD obj = new PDFProcessorTestConsole.PDFProcessor.SIFY_TECHNOLOGIES_LTD();
            obj.ExtractPDFData(pdfViewModel, FileName, "");
        }
    }
}
