namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_Process_Frequency
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string ProcessId { get; set; }

        [StringLength(100)]
        public string FrequenceId { get; set; }
    }
}
