using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.App_Code.ViewModel;

namespace AccountPaybleProcessor.App_Code.PDFProcessor
{
    public interface IPDFProcessor
    {
        void ExtractPDFData(ViewPdfModel pdfObject, string filePath, string ProcessedInvoicePath);
    }
}
