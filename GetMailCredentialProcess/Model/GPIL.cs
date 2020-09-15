namespace GetMailCredentialProcess.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GPIL : DbContext
    {
        public GPIL()
            : base("name=GPIL")
        {
        }

        public virtual DbSet<Tbl_AP_LineItemDetail> Tbl_AP_LineItemDetail { get; set; }
        public virtual DbSet<Tbl_AP_PODetail> Tbl_AP_PODetail { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
