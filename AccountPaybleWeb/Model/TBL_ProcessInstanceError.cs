namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_ProcessInstanceError
    {
        public long? ProcessInstanceId { get; set; }

        [StringLength(50)]
        public string StateId { get; set; }

        public string MetaData { get; set; }

        public DateTime? CreateTS { get; set; }

        public long Id { get; set; }
    }
}
