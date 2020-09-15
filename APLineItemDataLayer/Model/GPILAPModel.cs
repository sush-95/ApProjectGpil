namespace APLineItemDataLayer.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GPILAPModel : DbContext
    {
        public GPILAPModel()
            : base("name=GPILAPModel")
        {
        }

        public virtual DbSet<Tbl_AP_LineItemDetail> Tbl_AP_LineItemDetail { get; set; }
        public virtual DbSet<Tbl_AP_PODetail> Tbl_AP_PODetail { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
