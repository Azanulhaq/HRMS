using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PureHRMS
{
    public class tblTaskMasterCustom
    {
        [Key]
        public int TaskId { get; set; }
        public Nullable<int> AssignTo { get; set; }
        public Nullable<int> AssignBy { get; set; }
        [Required(ErrorMessage ="Start date is required")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> StartDate { get; set; }
        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> EndDate { get; set; }
        [Required(ErrorMessage = "Please enter details")]
        public string Details { get; set; }
        public string FinishNotes { get; set; }
    }
    [MetadataType(typeof(tblTaskMasterCustom))]
    public partial class tblTaskMaster
    {

    }
}