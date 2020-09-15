namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_Processes
    {
        [Key]
        [StringLength(100)]
        public string ProcessId { get; set; }

        [StringLength(1000)]
        public string ProcessDescription { get; set; }

        public bool? IsActive { get; set; }
    }
}
