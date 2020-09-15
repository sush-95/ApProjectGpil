namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_States
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TBL_States()
        {
            TBL_ProcessInstanceDetails = new HashSet<TBL_ProcessInstanceDetails>();
        }

        [Key]
        [StringLength(50)]
        public string StateId { get; set; }

        [StringLength(1000)]
        public string StateDescription { get; set; }

        public bool? IsActive { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_ProcessInstanceDetails> TBL_ProcessInstanceDetails { get; set; }
    }
}
