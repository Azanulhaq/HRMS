using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PureHRMS
{
    public class tblSupportRefCustom
    {
        [Key]
        public int STId { get; set; }
        public Nullable<int> refTId { get; set; }
        public string TText { get; set; }
        public string TFile { get; set; }
        public Nullable<System.DateTime> TDate { get; set; }
        public Nullable<int> UserInvolved { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<bool> AdminRead { get; set; }
        public Nullable<bool> UserRead { get; set; }
    }
    [MetadataType(typeof(tblSupportRefCustom))]
    public partial class tblTicketRef
    {

    }
}