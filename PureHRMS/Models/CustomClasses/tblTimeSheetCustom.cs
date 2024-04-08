using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PureHRMS
{
    public class tblTimeSheetCustom
    {
        [Key]
        public int TimeId { get; set; }
        [Required(ErrorMessage ="Date is required")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> TimeDate { get; set; }
        [Required(ErrorMessage = "Hours worked are required")]
        public Nullable<decimal> TimeHours { get; set; }
        [Required(ErrorMessage = "Comments are required")]
        public string Comments { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> refTimeEmpId { get; set; }
        public Nullable<int> refTimeProjectId { get; set; }
    }
    [MetadataType(typeof(tblTimeSheetCustom))]
    partial class tblTimeSheetManager
    {

    }
}