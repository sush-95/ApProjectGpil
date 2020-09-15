namespace AccountPaybleWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TBL_BotInfo
    {
        [Key]
        [StringLength(100)]
        public string BotId { get; set; }

        [StringLength(100)]
        public string ResponseQueueName { get; set; }

        [StringLength(100)]
        public string RequestQueueName { get; set; }

        [StringLength(200)]
        public string MachineName { get; set; }

        public int? MaxBotQueue { get; set; }

        public DateTime? CreatedTS { get; set; }

        public DateTime? ModifiedTS { get; set; }

        public bool? IsActive { get; set; }
    }
}
