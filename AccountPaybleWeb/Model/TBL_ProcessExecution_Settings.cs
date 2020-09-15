namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_ProcessExecution_Settings
    {
        [Required]
        [StringLength(100)]
        public string ProcessId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndData { get; set; }

        public bool? IsComplete { get; set; }

        public long Id { get; set; }

        public long? ProcessInstanceId { get; set; }

        public DateTime? RequestDate { get; set; }
    }
}
