using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PureHRMS
{
    public class tblProjectAllottedCustom
    {
        [Key]
        public int AllottedId { get; set; }
        [Required(ErrorMessage = "Employee Selection is required")]
        public Nullable<int> refEmployeeId { get; set; }
        [Required(ErrorMessage = "Project Selection is required")]
        public Nullable<int> refProjectId { get; set; }
        [Required(ErrorMessage = "Allotted Hours are required")]
        public Nullable<decimal> AllowedHours { get; set; }
        [Required(ErrorMessage = "Per Hour Rate is required")]
        public Nullable<decimal> PerHourRate { get; set; }
    }
    [MetadataType(typeof(tblProjectAllottedCustom))]
    partial class tblProjectAllotted
    {

    }
}