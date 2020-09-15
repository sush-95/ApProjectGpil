namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_ExcelRules
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string RuleName { get; set; }

        public bool? IsActive { get; set; }
    }
}
