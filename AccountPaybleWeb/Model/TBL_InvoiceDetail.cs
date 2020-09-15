namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_InvoiceDetail
    {
        [Required]
        [StringLength(50)]
        public string AirlineGSTNumber { get; set; }

        [Key]
        [StringLength(100)]
        public string InvoiceNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceDate { get; set; }

        public long ProcessInstanceId { get; set; }
    }
}
