using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountPaybleWeb.Manager
{
    public class ViewModelClass
    {
        public class ErrorModel
        {
            public string ErrorMessage { get; set; }
            public string WarningMessage { get; set; }
        }
        public class LoginModel
        {
            public string UserName { get; set; }
            public string Password { get; set; }

        }
        public class SearchViewModel
        {
            public DateTime fromdate { get; set; }
            public DateTime todate { get; set; }
            public string Status { get; set; }
            public string Vendor { get; set; }
            public string ponumber { get; set; }
            public string tab { get; set; }
            public string Search { get; set; }
        }
        public class XXGP_RPA_3WAY_PO_VIEW_Model
        {
            public string Invoice_Number { get; set; }
            public string Invoice_Date { get; set; }
            public string Receipt_number { get; set; }
            public string Receipt_total { get; set; }
            public string GPI_GST_Regn_No { get; set; }
            public string PO_Number { get; set; }

        }
        public class XXGP_RPA_2WAY_PO_VIEW_Model
        {
            public string PO_Number { get; set; }
            public string Line_Description { get; set; }
            public string Location_code { get; set; }
            public string Line_location_id { get; set; }
            public string Line_price { get; set; }
            public string Available_Qty { get; set; }
            public string Available_Amount { get; set; }
            public string GPI_GST_Regn_No { get; set; }
            public string Tax_Rate { get; set; }
        }

        public class JsonViewModel
        {           

            public Header Header { get; set; }
            public ViewPdfModel Detail { get; set; }
            public string Error { get; set; }
            public Status Status { get; set; }
        }
        public class Header
        {
            public string ProcessInstanceId { get; set; }
            public string ProcessName { get; set; }
            public string CreatedTs { get; set; }
            public string StateId { get; set; }
        }
        public class Status
        {
            public string Value { get; set; }
            public string Description { get; set; }

        }

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
            public List<ItemList> ListItem { get; set; }
            public string EwayBillNO { get; set; }
            public string PdfContent { get; set; }
            public string PaymentReference { get; set; }
            public string InvoiceAmount { get; set; }
            public string Department { get; set; }
            public string ViewMapped { get; set; }
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

        public class XXGP_PO_INVOICE_AUTO_TModel
        {
            public string PO_Number { get; set; }
            public string PO_MATCH_OPTION { get; set; }
            public string INVOICE_NUMBER { get; set; }
            public string INVOICE_DATE { get; set; }
            public string RECEIPT_NUMBER { get; set; }
            public string LINE_LOCATION_ID { get; set; }
            public string LINE_AMOUNT { get; set; }
            public string LINE_DESCRIPTION { get; set; }
            public string INVOICE_AMOUNT { get; set; }
            public string INVOICE_PDF_URL { get; set; }

        }
    }
}