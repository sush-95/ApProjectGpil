namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_ProcessInstanceData
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ProcessInstanceId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SequenceId { get; set; }

        public string MetaData { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MetaDataSequenceId { get; set; }

        public DateTime? CreatedTS { get; set; }

        public bool? IsProcessed { get; set; }

        public bool? IsFinal { get; set; }

        public long? ChildInstanceId { get; set; }

        [StringLength(100)]
        public string MessageId { get; set; }

        public string ErrorMessage { get; set; }
    }
}
