using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.App_Code
{
  public  class ViewModelClass
    {
        public class GET_GP_MailIDcls
        {
            public string AP_MAIL_BOX_ID { get; set; }
            public string MAIL_PASSWORD { get; set; }
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
           
        }
    }
}
