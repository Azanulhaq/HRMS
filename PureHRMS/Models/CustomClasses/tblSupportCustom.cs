using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PureHRMS
{
    public class tblSupportCustom
    {
        [Key]
        public int Tid { get; set; }
        [Required(ErrorMessage ="Subject is required")]
        public string TSubject { get; set; }
        [Required(ErrorMessage ="Please Select Department")]
        public Nullable<int> TDeptid { get; set; }
        public string TStatus { get; set; }
        public Nullable<int> UserInvolved { get; set; }
    }
    [MetadataType(typeof(tblSupportCustom))]
    public partial class tblSupportTickedMaster
    {

    }

}