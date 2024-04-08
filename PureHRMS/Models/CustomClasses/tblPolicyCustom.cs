using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PureHRMS
{
    public class tblPolicyCustom
    {
        [Key]
        public int PolicyId { get; set; }
        public string PolicyText { get; set; }
        public Nullable<System.DateTime> LastModified { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    }
    [MetadataType(typeof(tblPolicyCustom))]
    public partial class tblPolicyMaster
    {

    }
}