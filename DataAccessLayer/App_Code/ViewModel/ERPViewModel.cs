using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.App_Code.ViewModel
{
    public class ERPViewModel
    {

    }

    public class GP_Airline_Gst_Party_Map
    {
        public string GPI_GST_REGN_NO { get; set; }
        public string AIRLINE_GST_REG_NUM { get; set; }
        public string PARTY_NAME { get; set; }
        public string PARTY_SITE { get; set; }
    }

    public class GP_Gst_Tax_Type_Map
    {
        public string Tax_Type_in_PDF { get; set; }
        public string Tax_Type_in_ERP { get; set; }
    }

    public class GP_Gpi_Gst_OU_Map
    {
        public string GPI_GST_REGN_NUMBER { get; set; }
        public string OPERATING_UNIT { get; set; }
        public string ORGANIZATION_NAME { get; set; }
        public string LOCATION_NAME { get; set; }
        public string ADJUSTMENT_ACCOUNT { get; set; }
        public string UTGST_OR_SGST { get; set; }
        public string LOC_EMAIL_ADD { get; set; }
    }

    public class GP_Gst_Tax_Rate_Map
    {
        public string GPI_GST_REG_NUMBER { get; set; }
        public string TAX_TYPE_IN_PDF { get; set; }
        public string TAX_RATE_PERCENTAGE { get; set; }
        public string TAX_RATE_ID { get; set; }
    }

    public class GP_Gst_Doc_Type_Map
    {
        public string DOCUMENT_TYPE_IN_PDF { get; set; }
        public string TRANSACTION_TYPE_IN_ERP { get; set; }
    }

    public class GP_Gst_Sac_Code_Map
    {
        public string SAC_CODE { get; set; }
        public string GST_ENTRY { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_CLASS { get; set; }
    }

    public class GP_Becon_Inv_Rcon
    {
        public string AGENCY_INVNO { get; set; }
        public string AMOUNT_PAID { get; set; }
        public string TICKET_NO { get; set; }
        public string VOUCHER_NO { get; set; }
    }

    public class RCON_Tr_Reconciliation
    {
        public string REQUEST_NUMBER { get; set; }
        public string AGENCY_INVNO { get; set; }
        public string TICKET_NO { get; set; }
    }

    public class RCON_Rn_Od_Rqst
    {
        public string TA_NUMBER { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string USER_NAME { get; set; }
        public string STATUS { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string LOCATION { get; set; }
    }

    public class RCON_Party_List
    {
        public string PARTY_NAME { get; set; }
        public string ORG_ID { get; set; }
    }

    public class RCON_VoucherType_List
    {
        public string INV_TYPE { get; set; }
        public string TAG { get; set; }
    }
}
