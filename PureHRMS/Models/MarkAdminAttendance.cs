using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PureHRMS.Models
{
    public class MarkAdminAttendance
    {
        public int Euid { get; set; }
        public string EmpId { get; set; }
        public string EmployeeName { get; set; }
        public string InTime { get; set; } //COVERTED TO STRING FOR BETTER DISPLAY
        public string OutTime { get; set; } //COVERTED TO STRING FOR BETTER DISPLAY
        public string AC { get; set; }
        public string PendingLeaves {get;set;}

    }

   
}