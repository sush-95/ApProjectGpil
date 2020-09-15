namespace AccountPaybleWeb.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GPIModel : DbContext
    {
        public GPIModel()
            : base("name=GPIModel")
        {
        }

        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TBL_AirlineGSTConfig> TBL_AirlineGSTConfig { get; set; }
        public virtual DbSet<TBL_BotInfo> TBL_BotInfo { get; set; }
        public virtual DbSet<TBL_BotProcessAssigment> TBL_BotProcessAssigment { get; set; }
        public virtual DbSet<TBL_EmailResponse> TBL_EmailResponse { get; set; }
        public virtual DbSet<TBL_EmailTracker> TBL_EmailTracker { get; set; }
        public virtual DbSet<TBL_ExcelRules> TBL_ExcelRules { get; set; }
        public virtual DbSet<TBL_Frequency> TBL_Frequency { get; set; }
        public virtual DbSet<TBL_InvoiceDetail> TBL_InvoiceDetail { get; set; }
        public virtual DbSet<TBL_MessageTracker> TBL_MessageTracker { get; set; }
        public virtual DbSet<TBL_Process_Frequency> TBL_Process_Frequency { get; set; }
        public virtual DbSet<TBL_Processes> TBL_Processes { get; set; }
        public virtual DbSet<TBL_ProcessExecution_Settings> TBL_ProcessExecution_Settings { get; set; }
        public virtual DbSet<TBL_ProcessInstanceData> TBL_ProcessInstanceData { get; set; }
        public virtual DbSet<TBL_ProcessInstanceDetails> TBL_ProcessInstanceDetails { get; set; }
        public virtual DbSet<TBL_ProcessInstanceError> TBL_ProcessInstanceError { get; set; }
        public virtual DbSet<TBL_ProcessInstances> TBL_ProcessInstances { get; set; }
        public virtual DbSet<TBL_States> TBL_States { get; set; }
        public virtual DbSet<TBL_UserLogin> TBL_UserLogin { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TBL_AirlineGSTConfig>()
                .Property(e => e.ProcessId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_AirlineGSTConfig>()
                .Property(e => e.ConfigKey)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_AirlineGSTConfig>()
                .Property(e => e.ConfigValue)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_BotInfo>()
                .Property(e => e.BotId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_BotInfo>()
                .Property(e => e.ResponseQueueName)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_BotInfo>()
                .Property(e => e.RequestQueueName)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_BotInfo>()
                .Property(e => e.MachineName)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_BotProcessAssigment>()
                .Property(e => e.BotId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_BotProcessAssigment>()
                .Property(e => e.ProcessId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_EmailResponse>()
                .Property(e => e.ZipPath)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_Frequency>()
                .Property(e => e.FrequencyId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_InvoiceDetail>()
                .Property(e => e.AirlineGSTNumber)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_InvoiceDetail>()
                .Property(e => e.InvoiceNumber)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_InvoiceDetail>()
                .Property(e => e.InvoiceDate)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_MessageTracker>()
                .Property(e => e.MessageID)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_MessageTracker>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_MessageTracker>()
                .Property(e => e.BotID)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_Process_Frequency>()
                .Property(e => e.ProcessId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_Process_Frequency>()
                .Property(e => e.FrequenceId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_Processes>()
                .Property(e => e.ProcessId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_Processes>()
                .Property(e => e.ProcessDescription)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_ProcessExecution_Settings>()
                .Property(e => e.ProcessId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_ProcessInstanceData>()
                .Property(e => e.MetaData)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_ProcessInstanceData>()
                .Property(e => e.MessageId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_ProcessInstanceData>()
                .Property(e => e.ErrorMessage)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_ProcessInstanceDetails>()
                .Property(e => e.StateId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_ProcessInstanceError>()
                .Property(e => e.StateId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_ProcessInstanceError>()
                .Property(e => e.MetaData)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_ProcessInstances>()
                .Property(e => e.ProcessId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_ProcessInstances>()
                .Property(e => e.AllocatedServer)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_ProcessInstances>()
                .HasMany(e => e.TBL_ProcessInstances1)
                .WithOptional(e => e.TBL_ProcessInstances2)
                .HasForeignKey(e => e.ParentProcessInstanceId);

            modelBuilder.Entity<TBL_States>()
                .Property(e => e.StateId)
                .IsUnicode(false);

            modelBuilder.Entity<TBL_States>()
                .Property(e => e.StateDescription)
                .IsUnicode(false);
        }
    }
}
