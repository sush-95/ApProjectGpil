
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace DataAccessLayer.DBModel
{

using System;
    
public partial class USP_AllocateInstance_Result
{

    public long ProcessInstanceId { get; set; }

    public string ProcessId { get; set; }

    public Nullable<System.DateTime> CreatedTS { get; set; }

    public Nullable<long> ParentProcessInstanceId { get; set; }

    public Nullable<bool> IsCompleted { get; set; }

    public string AllocatedServer { get; set; }

    public Nullable<System.DateTime> AllocatedTS { get; set; }

}

}
