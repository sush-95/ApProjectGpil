namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_MessageTracker
    {
        [Key]
        [StringLength(100)]
        public string MessageID { get; set; }

        public int? RetrySequence { get; set; }

        [StringLength(150)]
        public string Status { get; set; }

        public string RequestJsonData { get; set; }

        public string ResponseJsonData { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [StringLength(150)]
        public string BotID { get; set; }

        public long ProcessInstanceID { get; set; }

        public int Sequence { get; set; }

        public int? IncrementTimeout { get; set; }
    }
}
