using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.App_Code.ViewModel
{
    public class PDFViewModel
    {
        public PDFViewModel()
        {
            PDFText = new List<string>();
        }

        public string VendorName { get; set; }
        public string VendorGst { get; set; }
        public List<string> PDFText { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string GPILGST { get; set; }
        public string EwayBillNO { get; set; }
        public string TaxableAmount { get; set; }
        public string CGSTAmount { get; set; }
        public string SGSTAmount { get; set; }
        public string IGSTAmount { get; set; }
        public string CGSTRate { get; set; }
        public string SGSTRate { get; set; }
        public string IGSTRate { get; set; }
      

        //Data from DB
        public string RegimeCode { get; set; }
        public string Location { get; set; }
        public string OperatingUnit { get; set; }
        public string OrganisationName { get; set; }
        public string TransactionNo { get; set; }
        public string TransactionType { get; set; }
        public string PartyType { get; set; }
        public string TransactionDate { get; set; }
        public string PartyName { get; set; }
        public string PartySite { get; set; }
        public string ItemClass { get; set; }
        public string ItemName { get; set; }
        public string Remarks { get; set; }

        public string SACCode { get; set; }
        public string CGSTTaxRateCode { get; set; }
        public string SGSTTaxRateCode { get; set; }
        public string IGSTTaxRateCode { get; set; }
        public string AdjustmentAmount { get; set; }
        public string AccessibleValue { get; set; }
        public string Url { get; set; }

        public string DocTypeInPDF { get; set; }
        public string DocTypeInERP { get; set; }

        public string CGSTTaxType { get; set; }
        public string SGSTTaxType { get; set; }
        public string IGSTTaxType { get; set; }


    }
}
