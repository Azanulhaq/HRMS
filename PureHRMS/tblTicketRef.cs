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
    
    public partial class tblTicketRef
    {
        public int STId { get; set; }
        public Nullable<int> refTId { get; set; }
        public string TText { get; set; }
        public string TFile { get; set; }
        public Nullable<System.DateTime> TDate { get; set; }
        public Nullable<int> UserInvolved { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<bool> AdminRead { get; set; }
        public Nullable<bool> UserRead { get; set; }
    
        public virtual tblSupportTickedMaster tblSupportTickedMaster { get; set; }
    }
}