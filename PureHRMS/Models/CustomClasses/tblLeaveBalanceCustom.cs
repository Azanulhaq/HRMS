using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PureHRMS.Models.CustomClasses
{
    public class tblLeaveBalanceCustom
    {
        [Key]
        public int LBId { get; set; }
        [Required(ErrorMessage = "Employee uid is required")]
        public Nullable<int> refEmpUid { get; set; }
        [Required(ErrorMessage = "Leave Id is required")]
        public Nullable<int> refLeaveID { get; set; }
        public Nullable<decimal> LeaveBalance { get; set; }
    }
    [MetadataType(typeof(tblLeaveBalanceCustom))]
    public partial class tblLeaveBalance
    {

    }
}