using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PureHRMS.Models
{
    public class TeamLeavesDashboard
    {
        public int EUid { get; set; }
        public string EmpName { get; set; }
        public string Designation { get; set; }
        public DateTime? StartDate { get; set; }
        public string Days { get; set; }
        public string Department { get; set; }
    }
}