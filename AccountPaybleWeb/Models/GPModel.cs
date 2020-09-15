namespace AccountPaybleWeb.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GPModel : DbContext
    {
        public GPModel()
            : base("name=GPModel")
        {
        }

        public virtual DbSet<Tbl_AP_LineItemDetail> Tbl_AP_LineItemDetail { get; set; }
        public virtual DbSet<Tbl_AP_PODetail> Tbl_AP_PODetail { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
