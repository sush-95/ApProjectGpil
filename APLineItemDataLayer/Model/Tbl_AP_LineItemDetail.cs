namespace APLineItemDataLayer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Tbl_AP_LineItemDetail
    {
        [Key]
        public long LineItemID { get; set; }

        public string PONumber { get; set; }

        public string VendorName { get; set; }

        public string InvoiceNoDate { get; set; }

        public string ReceiptNumber { get; set; }

        public int? LineId { get; set; }

        public string InvoiceDescription { get; set; }

        public string ItemQuantity { get; set; }

        [StringLength(15)]
        public string HSN { get; set; }

        public string Amount { get; set; }

        public string LineLocationID { get; set; }

        public string ReferenceNo { get; set; }

        [StringLength(8)]
        public string MatchOption { get; set; }

        public string ErrorDescrpion { get; set; }

        public string ViewMapped { get; set; }

        [StringLength(50)]
        public string AmountToBeMatched { get; set; }

        public string Rate { get; set; }
        public long? InvoiceID { get; set; }
    }
}
