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
    
    public partial class tblAttendanceMaster
    {
        public int AId { get; set; }
        public Nullable<int> refEuid { get; set; }
        public Nullable<System.DateTime> AttendanceDate { get; set; }
        public Nullable<System.TimeSpan> ClockIn { get; set; }
        public Nullable<System.TimeSpan> ClockOut { get; set; }
        public string AttandanceCode { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    
        public virtual tblUserMaster tblUserMaster { get; set; }
    }
}
