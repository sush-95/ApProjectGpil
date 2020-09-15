namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_UserLogin
    {
        public int ID { get; set; }

        [StringLength(100)]
        public string UserID { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
