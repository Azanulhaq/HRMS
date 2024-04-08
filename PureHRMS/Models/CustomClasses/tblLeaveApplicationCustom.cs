using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PureHRMS
{
    public class tblLeaveApplicationCustom
    {
        [Key]
        public int BLId { get; set; }
        public Nullable<int> refEuid { get; set; }
        [Required(ErrorMessage ="StartDate is required")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [Required(ErrorMessage = "No of days are required")]
        public Nullable<decimal> NoOfDays { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string ApplicantNotes { get; set; }
        public string ApprovedNotes { get; set; }
        public Nullable<bool> ApproveStatus { get; set; }
    }
    [MetadataType(typeof(tblLeaveApplicationCustom))]
    public partial class tblLeaveApplicationTrack
    {

    }
}