namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_EmailTracker
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ToEmailId { get; set; }

        public DateTime SentDate { get; set; }

        [StringLength(50)]
        public string GstNumber { get; set; }
    }
}
