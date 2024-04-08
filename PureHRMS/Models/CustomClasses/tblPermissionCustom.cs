using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PureHRMS
{
    public class tblPermissionCustom
    {
        [Key]
        public int PermissionId { get; set; }
        public Nullable<int> refUserId { get; set; }
        public Nullable<int> refMenuId { get; set; }
        public Nullable<int> refMenuParentID { get; set; }
        public Nullable<bool> Allowed { get; set; }
    }
    [MetadataType(typeof(tblPermissionCustom))]
    public partial class tblPermissionMaster
    {

    }
}