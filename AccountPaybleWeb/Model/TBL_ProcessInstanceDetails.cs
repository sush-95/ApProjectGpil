namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_ProcessInstanceDetails
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ProcessInstanceId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SequenceId { get; set; }

        [StringLength(50)]
        public string StateId { get; set; }

        public bool? IsCompleted { get; set; }

        public DateTime? CreateTS { get; set; }

        public virtual TBL_States TBL_States { get; set; }
    }
}
