
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
    using System.Collections.Generic;
    
public partial class TBL_ProcessInstanceData
{

    public long ProcessInstanceId { get; set; }

    public int SequenceId { get; set; }

    public string MetaData { get; set; }

    public int MetaDataSequenceId { get; set; }

    public Nullable<System.DateTime> CreatedTS { get; set; }

    public Nullable<bool> IsProcessed { get; set; }

    public Nullable<bool> IsFinal { get; set; }

    public Nullable<long> ChildInstanceId { get; set; }

    public string MessageId { get; set; }

    public string ErrorMessage { get; set; }

}

}
