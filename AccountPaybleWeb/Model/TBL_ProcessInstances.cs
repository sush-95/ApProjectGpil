namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_ProcessInstances
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TBL_ProcessInstances()
        {
            TBL_ProcessInstances1 = new HashSet<TBL_ProcessInstances>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ProcessInstanceId { get; set; }

        [StringLength(100)]
        public string ProcessId { get; set; }

        public DateTime? CreatedTS { get; set; }

        public long? ParentProcessInstanceId { get; set; }

        public bool? IsCompleted { get; set; }

        [StringLength(200)]
        public string AllocatedServer { get; set; }

        public DateTime? AllocatedTS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_ProcessInstances> TBL_ProcessInstances1 { get; set; }

        public virtual TBL_ProcessInstances TBL_ProcessInstances2 { get; set; }
    }
}
