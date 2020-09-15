using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.App_Code.QueueMessage
{
    public class QDetailsPDF : QDetails
    {
        public string PDFText { get; set; }
        //public QDetailsPDF(string pdfText)
        //{
        //    PDFText = pdfText;
        //}
        protected override void CreateFromJSON(string json)
        {

        }
        protected override string DetailString()
        {
            return PDFText;
        }

        protected override string DetailStringForBeat()
        {
            return "";
        }
    }
}
