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
    
    public partial class tblMenuRelatedAction
    {
        public int MenuRelatedActionsId { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public Nullable<int> refMenuId { get; set; }
    }
}
