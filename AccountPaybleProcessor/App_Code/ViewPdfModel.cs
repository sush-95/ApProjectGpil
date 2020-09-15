using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountPaybleProcessor.App_Code
{
    public class ViewPdfModel
    {
        public List<string> PdfText { get; set; }
        public string VendorGst { get; set; }
        public string NewInvoicepath { get; set; }
        public string InvoiceDate { get; set; }
        public string VendorName { get; set; }
        public string VendorSite { get; set; }
        public string InvoiceNumber { get; set; }
        public string PONumber { get; set; }       
        public string GPIGST { get; set; }        
        public string EwayBillNO { get; set; }
        public string PdfContent { get; set; }
        public string PaymentReference { get; set; }
        public string InvoiceAmount { get; set; }
        public string Department { get; set; }
        public string ViewMapped { get; set; }
        public List<ItemList> ListItem { get; set; }
    }
    public class ItemList
    {
        public string InvoiceNOandDate { get; set; }
        public string PONumber { get; set; }
        public string VendorName { get; set; }
        public string LineNumber { get; set; }
        public string LineLocationID { get; set; }
        public string Description_of_Goods { get; set; }
        public string HSN_SAC { get; set; }
        public string ReceiptNo { get; set; }
        public string MatchOption { get; set; }
        public string Quantity { get; set; }
        public string Rate { get; set; }       
        public string AmountMatched { get; set; }
        public string per_Amount { get; set; }
        public string Error { get; set; }

    }
}
