namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_AirlineGSTConfig
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string ProcessId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string ConfigKey { get; set; }

        [Required]
        [StringLength(1000)]
        public string ConfigValue { get; set; }
    }
}
