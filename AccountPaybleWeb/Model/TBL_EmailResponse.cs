namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_EmailResponse
    {
        public int Id { get; set; }

        public DateTime? MailSentDate { get; set; }

        public bool? IsMailSent { get; set; }

        [StringLength(600)]
        public string ZipPath { get; set; }
    }
}
