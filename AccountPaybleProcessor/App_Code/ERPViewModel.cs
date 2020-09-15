using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountPaybleProcessor.App_Code
{
   public class ERPViewModel
    {
        public class XXGP_RPA_PO_VIEW_Model
        {
            public string PO_Number { get; set; }
            public string Vendor_Name { get; set; }
            public string Vendor_Site { get; set; }
            public string PO_Status { get; set; }
            public string Supplier_GSTN_number { get; set; }
            public string User_Name { get; set; }
            public string Match_option { get; set; }
        }
        public class XXGP_RPA_INV_PAY_REF_Model
        {
            public string PO_Number { get; set; }
            public string Invoice_Number { get; set; }
            public string Invoice_Date { get; set; }
            public string Payment_reference { get; set; }
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
        public class XXGP_RPA_3WAY_PO_VIEW_Model
        {
            public string Invoice_Number { get; set; }
            public string Invoice_Date { get; set; }
            public string Receipt_number { get; set; }
            public string Receipt_total { get; set; }
            public string GPI_GST_Regn_No { get; set; }
            public  string PO_Number { get; set; }

        }
        //public class XXGP_PO_INVOICE_AUTO_STG_Model
        //{

        //}

    }
}
