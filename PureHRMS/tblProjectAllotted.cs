//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PureHRMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectAllotted
    {
        public int AllottedId { get; set; }
        public Nullable<int> refEmployeeId { get; set; }
        public Nullable<int> refProjectId { get; set; }
        public Nullable<decimal> AllowedHours { get; set; }
        public Nullable<decimal> PerHourRate { get; set; }
    
        public virtual tblProjectMaster tblProjectMaster { get; set; }
    }
}