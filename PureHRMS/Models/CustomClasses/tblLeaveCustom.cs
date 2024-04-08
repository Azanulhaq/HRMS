using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PureHRMS
{
    public class tblLeaveCustom
    {
        [Key]
        public int LId { get; set; }
        [Required(ErrorMessage ="Leave name is required")]
        public string LeaveName { get; set; }
        [Required(ErrorMessage = "Leave value is required")]
        public string LeveValue { get; set; }
    }
    [MetadataType(typeof(tblLeaveCustom))]
    public partial class tblLeaveMaster
    {

    }
}