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
    
    public partial class tblLeaveBalance
    {
        public int LBId { get; set; }
        public Nullable<int> refEmpUid { get; set; }
        public Nullable<int> refLeaveID { get; set; }
        public Nullable<decimal> LeaveBalance { get; set; }
    
        public virtual tblUserMaster tblUserMaster { get; set; }
    }
}
