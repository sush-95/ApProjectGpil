namespace GetMailCredentialProcess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Tbl_AP_PODetail
    {
        [Key]
        public long InvoiceID { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string PONumber { get; set; }

        public string VendorName { get; set; }

        public string VedorSite { get; set; }

        public string VendorGstNO { get; set; }

        public string GPIGstNO { get; set; }

        public string InvoiceAmount { get; set; }

        public string InvoiceStatus { get; set; }

        public string PaymentReference { get; set; }

        public string MetaData { get; set; }

        public string FilePath { get; set; }

        [StringLength(10)]
        public string Department { get; set; }

        public string ViewMapped { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string Remarks { get; set; }
    }
}
