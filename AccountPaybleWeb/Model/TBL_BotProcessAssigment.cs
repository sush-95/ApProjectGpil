namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_BotProcessAssigment
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string BotId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string ProcessId { get; set; }
    }
}
