using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PureHRMS
{
    public class tblAttendanceCustom
    {
        [Key]
        public int AId { get; set; }
        [Required]
        public Nullable<int> refEuid { get; set; }
        [Required]
        public Nullable<System.DateTime> AttendanceDate { get; set; }
        public Nullable<System.TimeSpan> ClockIn { get; set; }
        public Nullable<System.TimeSpan> ClockOut { get; set; }
        [Required]
        public string AttandanceCode { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
      
      
    }
    [MetadataType(typeof(tblAttendanceCustom))]
    public partial class tblAttendanceMaster
    {

    }
}