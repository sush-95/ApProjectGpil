namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_Frequency
    {
        [Key]
        [StringLength(100)]
        public string FrequencyId { get; set; }

        public bool? IsActive { get; set; }
    }
}
